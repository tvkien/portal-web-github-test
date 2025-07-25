using System.Collections.Generic;
using System.Linq;
using LinkIt.BubbleSheetPortal.DynamoIsolating.Model;
using LinkIt.BubbleSheetPortal.DynamoConnector;
using Amazon.DynamoDBv2.DocumentModel;
using LinkIt.BubbleSheetPortal.DynamoConnector.Common;
using Amazon.DynamoDBv2;
using LinkIt.BubbleSheetPortal.DynamoConnector.DynamoPrefixTableNameProvider;
using System;

namespace LinkIt.BubbleSheetPortal.DynamoIsolating.Repositories
{
    public class QTIOnlineTestSessionAnswerTimeTrackDynamo : IQTIOnlineTestSessionAnswerTimeTrackDynamo
    {
        private IAmazonDynamoDB _client;
        private IDynamoPrefixTableNameProvider _dynamoPrefixTableNameProvider;
        public QTIOnlineTestSessionAnswerTimeTrackDynamo(IDynamoPrefixTableNameProvider dynamoPrefixTableNameProvider)
        {
            _client = new AmazonDynamoDBClient();
            _dynamoPrefixTableNameProvider = dynamoPrefixTableNameProvider;
        }
        public List<QTIOnlineTestSessionAnswerTimeTrack> GetQTIOnlineTestSessionAnswerTimeTrack(int qtiOnlineTestSessionId)
        {
            var table = DynamoDBQuery.GetTable<QTIOnlineTestSessionAnswerTimeTrack>(_client, _dynamoPrefixTableNameProvider);            
            var queryFilter = new QueryFilter(ExpressionUtil<QTIOnlineTestSessionAnswerTimeTrack>.GetPath(o => o.QTIOnlineTestSessionID), QueryOperator.Equal, qtiOnlineTestSessionId);
            QueryOperationConfig config = new QueryOperationConfig()
            {
                Filter = queryFilter,
                Select = SelectValues.AllAttributes,
                IndexName = "QTIOnlineTestSessionID-index" //this Index sets qtiOnlineTestSessionId as primary key
            };
            var result = DynamoDBQuery.QueryDocument<QTIOnlineTestSessionAnswerTimeTrack>(table, config);
            return result.ToList();
        }

        public int GetTotalSpentTimeByQTIOnlineTestSessionID(int qtiOnlineTestSessionId)
        {
            var table = DynamoDBQuery.GetTable<QTIOnlineTestSessionAnswerTimeTrack>(_client, _dynamoPrefixTableNameProvider);
            var queryFilter = new QueryFilter(ExpressionUtil<QTIOnlineTestSessionAnswerTimeTrack>.GetPath(o => o.QTIOnlineTestSessionID), QueryOperator.Equal, qtiOnlineTestSessionId);
            QueryOperationConfig config = new QueryOperationConfig()
            {
                Filter = queryFilter,
                Select = SelectValues.AllAttributes,
                IndexName = "QTIOnlineTestSessionID-index" //this Index sets qtiOnlineTestSessionId as primary key
            };
            var results = DynamoDBQuery.QueryDocument<QTIOnlineTestSessionAnswerTimeTrack>(table, config);
            if (results != null && results.Any())
            {
                int totalSpentTimePerQuestion = 0;
                foreach (var result in results)
                {
                    try
                    {
                        DateTime startDate = DateTime.UtcNow;
                        DateTime endDate = DateTime.UtcNow;

                        if (DateTime.TryParse(result.EndTimeUTC.GetValueOrDefault().ToString(), out endDate) && DateTime.TryParse(result.StartTimeUTC.GetValueOrDefault().ToString(), out startDate))
                        {
                            int spentTimePerQuestion = (int)(endDate - startDate).TotalSeconds;
                            if (spentTimePerQuestion > 0)
                            {
                                totalSpentTimePerQuestion += spentTimePerQuestion;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
                return totalSpentTimePerQuestion;
            }
            return 0;
        }
    }
}
