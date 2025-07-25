using System.Collections.Generic;
using System.Linq;
using LinkIt.BubbleSheetPortal.DynamoIsolating.Model;
using Amazon.DynamoDBv2;
using LinkIt.BubbleSheetPortal.DynamoConnector;
using Amazon.DynamoDBv2.DocumentModel;
using LinkIt.BubbleSheetPortal.DynamoConnector.Common;
using LinkIt.BubbleSheetPortal.DynamoConnector.DynamoPrefixTableNameProvider;
using Newtonsoft.Json;

namespace LinkIt.BubbleSheetPortal.DynamoIsolating.Repositories
{
    public class SGOManagerLogDynamo : ISGOManagerLogDynamo
    {
        private IAmazonDynamoDB _client;
        private IDynamoPrefixTableNameProvider _dynamoPrefixTableNameProvider;
        public SGOManagerLogDynamo(IDynamoPrefixTableNameProvider dynamoPrefixTableNameProvider)
        {
            _client = new AmazonDynamoDBClient();
            _dynamoPrefixTableNameProvider = dynamoPrefixTableNameProvider;
        }
        public SGOManagerLog GetByID(string sgoManagerLogId)
        {
            var table = DynamoDBQuery.GetTable<SGOManagerLog>(_client, _dynamoPrefixTableNameProvider);
            var data = table.GetItem(sgoManagerLogId);
            var result = DocumentModelTransform.Transform<SGOManagerLog>(data);

            return result;
        }

        public void PutItem(SGOManagerLog sgoManagerLog)
        {
            Document document = Document.FromJson(JsonConvert.SerializeObject(sgoManagerLog));

            var table = DynamoDBQuery.GetTable<SGOManagerLog>(_client, _dynamoPrefixTableNameProvider);
            table.PutItem(document);
        }

        public void UpdateItem(SGOManagerLog sgoManagerLog)
        {
            Document document = Document.FromJson(JsonConvert.SerializeObject(sgoManagerLog));

            var table = DynamoDBQuery.GetTable<SGOManagerLog>(_client, _dynamoPrefixTableNameProvider);
            table.UpdateItem(document);
        }                
    }
}
