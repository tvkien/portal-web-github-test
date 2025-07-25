using System.Collections.Generic;
using System.Linq;
using LinkIt.BubbleSheetPortal.DynamoIsolating.Model;
using Amazon.DynamoDBv2;
using LinkIt.BubbleSheetPortal.DynamoConnector;
using Amazon.DynamoDBv2.DocumentModel;
using LinkIt.BubbleSheetPortal.DynamoConnector.Common;
using LinkIt.BubbleSheetPortal.DynamoConnector.DynamoPrefixTableNameProvider;
using System;
using LinkIt.BubbleSheetPortal.Common;

namespace LinkIt.BubbleSheetPortal.DynamoIsolating.Repositories
{
    public class QTIOnlineTestSessionDynamo : IQTIOnlineTestSessionDynamo
    {
        private IAmazonDynamoDB _client;
        private string _dynamoPrefixTableName;
        private Lazy<Table> _table;

        public QTIOnlineTestSessionDynamo(IDynamoPrefixTableNameProvider dynamoPrefixTableNameProvider)
        {
            _client = new AmazonDynamoDBClient();
            this._dynamoPrefixTableName = dynamoPrefixTableNameProvider.DynamoPrefixTableName;
            this._table = new Lazy<Table>(() =>
            {
                return DynamoDBQuery.GetTable<QTIOnlineTestSession>(_client, _dynamoPrefixTableName);
            });
        }
        public QTIOnlineTestSession GetByID(int qtiOnlineTestSessionID)
        {
            var table = _table.Value;
            var data = table.GetItem(qtiOnlineTestSessionID);
            var result = DocumentModelTransform.Transform<QTIOnlineTestSession>(data);

            return result;
        }

        public List<QTIOnlineTestSession> Search(string assignmentGuid)
        {

            var table = _table.Value;

            QueryFilter filter = new QueryFilter();
            filter.AddCondition(ExpressionUtil<QTITestClassAssignment>.GetPath(o => o.AssignmentGUID), QueryOperator.Equal, assignmentGuid);
            QueryOperationConfig config = new QueryOperationConfig()
            {
                Filter = filter,
                Select = SelectValues.AllAttributes,
                IndexName = "AssignmentGUID-StudentID-index"//Use existing index AssignmentGUID-StudentID-index (created by TestTaker )

            };

            var dynamoData = DynamoDBQuery.QueryDocument<QTIOnlineTestSession>(table, config);
            if (dynamoData == null)
            {
                return null;
            }
            else
            {
                return dynamoData.ToList();
            }
        }

        public void ChangeStatus(int qtiOnlineTestSessionID, int statusID)
        {
            var document = new Document();
            document[ExpressionUtil<QTIOnlineTestSession>.GetPath(o => o.QTIOnlineTestSessionID)] = qtiOnlineTestSessionID;
            document[ExpressionUtil<QTIOnlineTestSession>.GetPath(o => o.StatusID)] = statusID;

            var table = _table.Value;
            TableHelper.UpdateDocument(table, document,
                TableHelper.BuildCheckExistedQtiOnlineTestSessionIdUpdateConfig(qtiOnlineTestSessionID));
            //table.UpdateItem(document);
        }

        public IEnumerable<T> SearchCustom<T>(string assignmentGuid)
        {
            var table = _table.Value;

            QueryFilter filter = new QueryFilter();
            filter.AddCondition(ExpressionUtil<QTITestClassAssignment>.GetPath(o => o.AssignmentGUID), QueryOperator.Equal, assignmentGuid);
            QueryOperationConfig config = new QueryOperationConfig()
            {
                Filter = filter,
                Select = SelectValues.SpecificAttributes,
                IndexName = "AssignmentGUID-StudentID-index",
                AttributesToGet = typeof(T).GetAllPropertiesName().ToList()
            };

            var dynamoData = DynamoDBQuery.QueryDocument<T>(table, config);
            if (dynamoData == null)
                return new T[0];

            return dynamoData;
        }
    }
}
