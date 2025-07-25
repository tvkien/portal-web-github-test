using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Common.CustomException;
using LinkIt.BubbleSheetPortal.VaultProvider.Model;
using Newtonsoft.Json;

namespace LinkIt.BubbleSheetPortal.VaultProvider
{
    public class DynamoDBVaultProvider : IVaultProvider
    {
        private readonly HttpContextBase _context;
        private readonly ISecretsManager _secretsManager;
        private readonly Table _vaultTable;
        private readonly IAmazonDynamoDB _client;

        public DynamoDBVaultProvider(
            HttpContextBase context,
            IAmazonDynamoDB client,
            ISecretsManager secretsManager)
        {
            _context = context;
            _secretsManager = secretsManager;
            _client = client;

            _vaultTable = Table.LoadTable(client, VaultInfo.VaultTable);
        }

        public Vault GetVaultByCode(string code = "")
        {
            if (string.IsNullOrEmpty(code)) code = GetCodeFromUrl();

            var key = $"VaultTable-{code}";
            return CacheManager.GetOrAdd(key, () =>
            {
                return GetByCode(code);
            }, 30);
        }

        private Vault GetByCode(string code)
        {
            if (code == "sso")
            {
                code = "demo";
            }

            var filter = new QueryOperationConfig
            {
                Filter = new QueryFilter("Code", QueryOperator.Equal, code),
                IndexName = "Code-Index"
            };

            return QueryByOperationConfig(filter);
        }

        private Vault QueryByOperationConfig(QueryOperationConfig filter)
        {
            var result = _vaultTable.Query(filter);
            Vault vault = null;
            do
            {
                var documents = result.GetNextSet();
                if (documents.Any())
                {
                    var firstDocument = documents.FirstOrDefault();
                    vault = JsonConvert.DeserializeObject<Vault>(firstDocument.ToJson());
                    DecryptPasswordFromVault(vault);
                    break;
                }
            } while (!result.IsDone);
            return vault;
        }

        private void DecryptPasswordFromVault(Vault vault)
        {
            if (!string.IsNullOrEmpty(vault?.SecretManager))
            {
                var secretString = _secretsManager.GetSecretString(vault.SecretManager);

                var secret = JsonConvert.DeserializeObject<Dictionary<string, string>>(secretString);

                if (secret != null)
                {
                    if (secret.ContainsKey(nameof(vault.DBServers)))
                    {
                        vault.DBServers = JsonConvert.DeserializeObject<List<DatabaseServer>>(secret[nameof(vault.DBServers)]);
                    }

                    if(secret.ContainsKey(nameof(vault.EmailCredentials)))
                    {
                        vault.EmailCredentials = JsonConvert.DeserializeObject<List<EmailCredential>>(secret[nameof(vault.EmailCredentials)]);
                    }

                    if (secret.ContainsKey(nameof(vault.ASPState)))
                    {
                        vault.ASPState = JsonConvert.DeserializeObject<ASPStateDto>(secret[nameof(vault.ASPState)]);
                        if (!string.IsNullOrEmpty(vault.ASPState?.RedisConnection?.RedisCachePassword))
                        {
                            vault.ASPState.RedisConnection.RedisCachePassword = DecryptString(vault.ASPState.RedisConnection.RedisCachePassword, VaultInfo.VaultPasswordPassphrase);
                        }
                        if (!string.IsNullOrEmpty(vault.ASPState?.SQLConnection?.PortalPassword))
                        {
                            vault.ASPState.SQLConnection.PortalPassword = DecryptString(vault.ASPState.SQLConnection.PortalPassword, VaultInfo.VaultPasswordPassphrase);
                        }
                    }

                    if (secret.ContainsKey(nameof(vault.CognitoCredential)))
                    {
                        vault.CognitoCredential = JsonConvert.DeserializeObject<CognitoCredential>(secret[nameof(vault.CognitoCredential)]);
                    }
                }

                foreach (var databaseServer in vault.DBServers)
                {
                    databaseServer.PortalPassword = DecryptString(databaseServer.PortalPassword, VaultInfo.VaultPasswordPassphrase);
                }
            }
        }

        private string GetCodeFromUrl()
        {
            if (_context != null && _context.Request != null)
            {
                string subDomain = _context.Request.Url.Host.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0];
                return subDomain;
            }
            return string.Empty;
        }

        private string DecryptString(string message, string passphrase)
        {
            byte[] Results;
            var UTF8 = new System.Text.UTF8Encoding();

            // Step 1. We hash the passphrase using MD5
            // We use the MD5 hash generator as the result is a 128 bit byte array
            // which is a valid length for the TripleDES encoder we use below

            var HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(passphrase));

            // Step 2. Create a new TripleDESCryptoServiceProvider object
            var TDESAlgorithm = new TripleDESCryptoServiceProvider();

            // Step 3. Setup the decoder
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;

            // Step 4. Convert the input string to a byte[]
            byte[] DataToDecrypt = Convert.FromBase64String(message);

            // Step 5. Attempt to decrypt the string
            try
            {
                ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
                Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
            }
            finally
            {
                // Clear the TripleDes and Hashprovider services of any sensitive information
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }

            // Step 6. Return the decrypted string in UTF8 format
            return UTF8.GetString(Results);
        }

        public string GetLICodeByCleveDistrictId(string cleverDistrictId)
        {
            Dictionary<string, AttributeValue> lastKeyEvaluated = null;
            do
            {
                var request = new ScanRequest
                {
                    TableName = VaultInfo.VaultTable,
                    Limit = 100,
                    ExclusiveStartKey = lastKeyEvaluated,
                    FilterExpression = "ThirdPartySettings.CleverSSO.DistrictId = :val",
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                    {
                        { ":val", new AttributeValue { S = cleverDistrictId } }
                    },
                    ProjectionExpression = nameof(Vault.Code),
                };

                var response = _client.Scan(request);

                if (response.Items.Any())
                {
                    var validVault = response.Items.First();
                    return validVault[nameof(Vault.Code)].S;
                }
                lastKeyEvaluated = response.LastEvaluatedKey;

            } while (lastKeyEvaluated != null && lastKeyEvaluated.Count != 0);

            throw new NotFoundException($"{nameof(cleverDistrictId)} not belong to any vault");
        }
    }
}
