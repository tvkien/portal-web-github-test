using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using LinkIt.BubbleSheetPortal.DynamoConnector;
using LinkIt.BubbleSheetPortal.DynamoConnector.Common;
using LinkIt.BubbleSheetPortal.DynamoConnector.DynamoPrefixTableNameProvider;
using LinkIt.BubbleSheetPortal.DynamoIsolating.Model;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.DynamoIsolating.Repositories
{
    public class PostAnswerLogDynamo : IPostAnswerLogDynamo
    {
        private IAmazonDynamoDB _client;
        private IDynamoPrefixTableNameProvider _dynamoPrefixTableNameProvider;

        public PostAnswerLogDynamo(IDynamoPrefixTableNameProvider dynamoPrefixTableNameProvider)
        {
            _client = new AmazonDynamoDBClient();
            _dynamoPrefixTableNameProvider = dynamoPrefixTableNameProvider;
        }

        public List<PostAnswerLog> GetPostAnswerLogs(int qtiOnlineTestSessionID)
        {
            var result = new List<PostAnswerLog>();
            var table = DynamoDBQuery.GetTable<PostAnswerLog>(_client, _dynamoPrefixTableNameProvider);

            QueryFilter filter = new QueryFilter();
            filter.AddCondition(ExpressionUtil<PostAnswerLog>.GetPath(o => o.QTIOnlineTestSessionID), QueryOperator.Equal, qtiOnlineTestSessionID);
            QueryOperationConfig config = new QueryOperationConfig()
            {
                Filter = filter,
                Select = SelectValues.AllAttributes
                //IndexName = "QTIOnlineTestSessionID-index"
            };

            var dynamoData = DynamoDBQuery.QueryDocument<PostAnswerLog>(table, config);
            if (dynamoData != null)
            {
                result = dynamoData.ToList();
            }

            return result;
        }
    }
}
