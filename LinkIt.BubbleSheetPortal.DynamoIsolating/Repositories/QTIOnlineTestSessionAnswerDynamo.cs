using System;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using LinkIt.BubbleSheetPortal.DynamoConnector;
using LinkIt.BubbleSheetPortal.DynamoConnector.Common;
using LinkIt.BubbleSheetPortal.DynamoIsolating.Model;
using System.Collections.Generic;
using System.Linq;
using LinkIt.BubbleSheetPortal.DynamoConnector.DynamoPrefixTableNameProvider;

namespace LinkIt.BubbleSheetPortal.DynamoIsolating.Repositories
{
    public class QTIOnlineTestSessionAnswerDynamo : IQTIOnlineTestSessionAnswerDynamo
    {
        private IAmazonDynamoDB _client;
        private IDynamoPrefixTableNameProvider _dynamoPrefixTableNameProvider;
        public QTIOnlineTestSessionAnswerDynamo(IDynamoPrefixTableNameProvider dynamoPrefixTableNameProvider)
        {
            _client = new AmazonDynamoDBClient();
            _dynamoPrefixTableNameProvider = dynamoPrefixTableNameProvider;
        }

        public QTIOnlineTestSessionAnswer GetByID(int qtiOnlineTestSessionAnswerID)
        {
            //var table = DynamoDBQuery.GetTable<QTIOnlineTestSessionAnswer>(_client, _dynamoPrefixTableNameProvider);

            //ScanFilter filter = new ScanFilter();
            //filter.AddCondition(ExpressionUtil<QTIOnlineTestSessionAnswer>.GetPath(o => o.QTIOnlineTestSessionAnswerID), ScanOperator.Equal, qtiOnlineTestSessionAnswerID);

            //var documents = DynamoDBQuery.ScanDocuments(table, filter);

            //var data = DocumentModelTransform.Transform<QTIOnlineTestSessionAnswer>(documents);

            //if (data == null) return null;

            //var result = data.FirstOrDefault();

            //return result;
            var table = DynamoDBQuery.GetTable<QTIOnlineTestSessionAnswer>(_client, _dynamoPrefixTableNameProvider);
            var filter = new QueryFilter();
            filter.AddCondition("QTIOnlineTestSessionAnswerID", QueryOperator.Equal, qtiOnlineTestSessionAnswerID);//An index must be created with QtiOnlineTestSessionAnswerID is Partion Key

            QueryOperationConfig config = new QueryOperationConfig()
            {
                Filter = filter,
                Select = SelectValues.AllAttributes,
                IndexName = "QTIOnlineTestSessionAnswerID-index"

            };
            var dynamoData = DynamoDBQuery.QueryDocument<QTIOnlineTestSessionAnswer>(table, config);
            if (dynamoData == null)
            {
                return null;
            }
            else
            {
                return dynamoData.FirstOrDefault();
            }
            
        }

        public List<QTIOnlineTestSessionAnswer> Search(int qtiOnlineTestSessionID)
        {
            var table = DynamoDBQuery.GetTable<QTIOnlineTestSessionAnswer>(_client,_dynamoPrefixTableNameProvider);

            QueryFilter filter = new QueryFilter(ExpressionUtil<QTIOnlineTestSessionAnswer>.GetPath(o => o.QTIOnlineTestSessionID), QueryOperator.Equal, qtiOnlineTestSessionID);

            var documents = DynamoDBQuery.QueryDocuments(table, filter);

            var result = DocumentModelTransform.Transform<QTIOnlineTestSessionAnswer>(documents);

            return result;
        }
        public List<QTIOnlineTestSessionAnswer> SearchAnswerOfStudent(int qtiOnlineTestSessionID, int virtualQuestionId)
        {
            var table = DynamoDBQuery.GetTable<QTIOnlineTestSessionAnswer>(_client, _dynamoPrefixTableNameProvider);

            QueryFilter filter = new QueryFilter(ExpressionUtil<QTIOnlineTestSessionAnswer>.GetPath(o => o.QTIOnlineTestSessionID), QueryOperator.Equal, qtiOnlineTestSessionID);
            filter.AddCondition(ExpressionUtil<QTIOnlineTestSessionAnswer>.GetPath(o => o.VirtualQuestionID), QueryOperator.Equal, virtualQuestionId);

            var documents = DynamoDBQuery.QueryDocuments(table, filter);

            var result = DocumentModelTransform.Transform<QTIOnlineTestSessionAnswer>(documents);

            return result;
        }
        public void UpdateAnswerText(int qtiOnlineTestSessionID, int answerID, string answerText)
        {
            var document = new Document();
            document[ExpressionUtil<QTIOnlineTestSessionAnswer>.GetPath(o => o.QTIOnlineTestSessionID)] = qtiOnlineTestSessionID;
            document[ExpressionUtil<QTIOnlineTestSessionAnswer>.GetPath(o => o.QTIOnlineTestSessionAnswerID)] = answerID;
            document[ExpressionUtil<QTIOnlineTestSessionAnswer>.GetPath(o => o.AnswerText)] = answerText;
            document[ExpressionUtil<QTIOnlineTestSessionAnswer>.GetPath(o => o.AnswerTemp)] = null;
            document[ExpressionUtil<QTIOnlineTestSessionAnswer>.GetPath(o => o.Answered)] = true;

            var table = DynamoDBQuery.GetTable<QTIOnlineTestSessionAnswer>(_client, _dynamoPrefixTableNameProvider);
            TableHelper.UpdateDocument(table, document,
                TableHelper.BuildCheckExistedQtiOnlineTestSessionIdUpdateConfig(qtiOnlineTestSessionID));
            //table.UpdateItem(document);
        }

        public void UpdateAnswerTemp(int qtiOnlineTestSessionID, int answerID, string answerTemp)
        {
            var document = new Document();
            document[ExpressionUtil<QTIOnlineTestSessionAnswer>.GetPath(o => o.QTIOnlineTestSessionID)] = qtiOnlineTestSessionID;
            document[ExpressionUtil<QTIOnlineTestSessionAnswer>.GetPath(o => o.QTIOnlineTestSessionAnswerID)] = answerID;
            document[ExpressionUtil<QTIOnlineTestSessionAnswer>.GetPath(o => o.AnswerTemp)] = answerTemp;

            var table = DynamoDBQuery.GetTable<QTIOnlineTestSessionAnswer>(_client, _dynamoPrefixTableNameProvider);
            TableHelper.UpdateDocument(table, document,
                TableHelper.BuildCheckExistedQtiOnlineTestSessionIdUpdateConfig(qtiOnlineTestSessionID));
            //table.UpdateItem(document);
        }

        public void UpdateAnswerSubText(int qtiOnlineTestSessionID, int answerID, int answerSubID, string answerText)
        {
            var document = new Document();
            document[ExpressionUtil<QTIOnlineTestSessionAnswer>.GetPath(o => o.QTIOnlineTestSessionID)] = qtiOnlineTestSessionID;
            document[ExpressionUtil<QTIOnlineTestSessionAnswer>.GetPath(o => o.QTIOnlineTestSessionAnswerID)] = answerID;
            document[ExpressionUtil<QTIOnlineTestSessionAnswer>.GetPath(o => o.AnswerText)] = answerText;
            document[ExpressionUtil<QTIOnlineTestSessionAnswer>.GetPath(o => o.AnswerTemp)] = null;
            document[ExpressionUtil<QTIOnlineTestSessionAnswer>.GetPath(o => o.Answered)] = true;

            var table = DynamoDBQuery.GetTable<QTIOnlineTestSessionAnswer>(_client, _dynamoPrefixTableNameProvider);
            TableHelper.UpdateDocument(table, document,
                TableHelper.BuildCheckExistedQtiOnlineTestSessionIdUpdateConfig(qtiOnlineTestSessionID));
            //table.UpdateItem(document);
        }
        public void UpdatePointsEarned(int qtiOnlineTestSessionID, int answerID, int? pointsEarned = null)
        {
            var document = new Document();
            document[ExpressionUtil<QTIOnlineTestSessionAnswer>.GetPath(o => o.QTIOnlineTestSessionID)] = qtiOnlineTestSessionID;
            document[ExpressionUtil<QTIOnlineTestSessionAnswer>.GetPath(o => o.QTIOnlineTestSessionAnswerID)] = answerID;
            document[ExpressionUtil<QTIOnlineTestSessionAnswer>.GetPath(o => o.PointsEarned)] = pointsEarned;

            var table = DynamoDBQuery.GetTable<QTIOnlineTestSessionAnswer>(_client, _dynamoPrefixTableNameProvider);
            TableHelper.UpdateDocument(table, document,
                TableHelper.BuildCheckExistedQtiOnlineTestSessionIdUpdateConfig(qtiOnlineTestSessionID));
        }
        public void UpdateQTIOnlineTestSessionAnswerSubs(int qtiOnlineTestSessionID, int answerID, List<QTIOnlineTestSessionAnswerSub> answerSubs)
        {
            var document = new Document();
            document[ExpressionUtil<QTIOnlineTestSessionAnswer>.GetPath(o => o.QTIOnlineTestSessionID)] = qtiOnlineTestSessionID;
            document[ExpressionUtil<QTIOnlineTestSessionAnswer>.GetPath(o => o.QTIOnlineTestSessionAnswerID)] = answerID;
            document[ExpressionUtil<QTIOnlineTestSessionAnswer>.GetPath(o => o.QTIOnlineTestSessionAnswerSubs)] = DocumentModelTransform.Transform(answerSubs);

            var table = DynamoDBQuery.GetTable<QTIOnlineTestSessionAnswer>(_client, _dynamoPrefixTableNameProvider);
            TableHelper.UpdateDocument(table, document,
                TableHelper.BuildCheckExistedQtiOnlineTestSessionIdUpdateConfig(qtiOnlineTestSessionID));
            //table.UpdateItem(document);
        }
    }
}
