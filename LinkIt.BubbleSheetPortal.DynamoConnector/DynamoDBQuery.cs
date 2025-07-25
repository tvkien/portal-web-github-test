using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using LinkIt.BubbleSheetPortal.DynamoConnector.Common;
using System.Collections.Generic;
using System.Linq;
using LinkIt.BubbleSheetPortal.DynamoConnector.DynamoPrefixTableNameProvider;
using Newtonsoft.Json;

namespace LinkIt.BubbleSheetPortal.DynamoConnector
{
    public static class DynamoDBQuery
    {
        public static Table GetTable<T>(IAmazonDynamoDB client, IDynamoPrefixTableNameProvider dynamoPrefixTableNameProvider) where T : class
        {
            var tableName = ExpressionUtil<T>.GetClassName();
            if (dynamoPrefixTableNameProvider != null &&
                !string.IsNullOrWhiteSpace(dynamoPrefixTableNameProvider.DynamoPrefixTableName))
            {
                tableName = string.Format("{0}{1}", dynamoPrefixTableNameProvider.DynamoPrefixTableName, tableName);
            }
            if (string.IsNullOrWhiteSpace(tableName)) return null;

            var vaultTable = Table.LoadTable(client, tableName);

            return vaultTable;
        }
        public static Table GetTable<T>(IAmazonDynamoDB client, string dynamoPrefixTableName) where T : class
        {
            var tableName = ExpressionUtil<T>.GetClassName();
            if (!string.IsNullOrWhiteSpace(dynamoPrefixTableName))
            {
                tableName = string.Format("{0}{1}", dynamoPrefixTableName, tableName);
            }
            if (string.IsNullOrWhiteSpace(tableName)) return null;

            var vaultTable = Table.LoadTable(client, tableName);

            return vaultTable;
        }

        //public static Table GetTable(DynamoDBInfo config, string tableName)
        //{
        //    if (config == null || string.IsNullOrWhiteSpace(tableName)) return null;

        //    var client = new AmazonDynamoDBClient(config.AccessKey, config.SecretKey, new AmazonDynamoDBConfig { ServiceURL = config.RegionUrl });
        //    var vaultTable = Table.LoadTable(client, tableName);

        //    return vaultTable;
        //}

        public static List<Document> ScanDocuments(Table table, ScanFilter filter)
        {
            if (table == null) return null;

            Search search = table.Scan(filter);
            List<Document> result = new List<Document>();
            do
            {
                var documentList = search.GetNextSet();
                foreach (var document in documentList)
                {
                    result.Add(document);
                }
            } while (!search.IsDone);

            return result;
        }

        public static List<Document> QueryDocuments(Table table, QueryFilter filter)
        {
            if (table == null) return null;

            Search search = table.Query(filter);
            List<Document> result = new List<Document>();
            do
            {
                var documentList = search.GetNextSet();
                foreach (var document in documentList)
                {
                    result.Add(document);
                }
            } while (!search.IsDone);

            return result;
        }
        public static IEnumerable<T> QueryDocument<T>(Table table, QueryOperationConfig config)
        {
            var result = new List<T>();
            var queryResult = table.Query(config);
            var listDocument = new List<Document>();
            do
            {
                var documentSet = queryResult.GetNextSet();
                foreach (var document in documentSet)
                {
                    listDocument.Add(document);
                }
            } while (!queryResult.IsDone);

            if (listDocument.Any())
            {
                foreach (var document in listDocument)
                {
                    var data = JsonConvert.DeserializeObject<T>(document.ToJson());
                    result.Add(data);
                }

            }
            return result;
        }
        public static T QueryFirstDocument<T>(Table table, QueryOperationConfig config)
        {
            var queryResult = table.Query(config);
            var document = queryResult.GetRemaining().FirstOrDefault();

            if (document != null)
                return JsonConvert.DeserializeObject<T>(document.ToJson());

            return default;

        }
    }
}
