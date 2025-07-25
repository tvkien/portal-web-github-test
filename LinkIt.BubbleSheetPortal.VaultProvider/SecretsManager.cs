using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using LinkIt.BubbleSheetPortal.Common;

namespace LinkIt.BubbleSheetPortal.VaultProvider
{
    public class SecretsManager : ISecretsManager
    {
        private readonly IAmazonSecretsManager _secretsManager;

        public SecretsManager(IAmazonSecretsManager secretsManager)
        {
            _secretsManager = secretsManager;
        }

        public string GetSecretString(string secretId)
        {
            return CacheManager.GetOrAdd(secretId, () =>
            {
                var request = new GetSecretValueRequest
                {
                    SecretId = secretId
                };

                var response = _secretsManager.GetSecretValue(request);
                return string.IsNullOrEmpty(response?.SecretString) ? null : response.SecretString;
            });
        }
    }
}
