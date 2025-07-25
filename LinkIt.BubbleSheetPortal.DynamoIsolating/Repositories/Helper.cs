using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;

namespace LinkIt.BubbleSheetPortal.DynamoIsolating.Repositories
{
    public class TableHelper
    {
        internal static void UpdateDocument(Table table, Document document, UpdateItemOperationConfig config)
        {
            try
            {
                table.UpdateItem(document, config);
            }
            catch (ConditionalCheckFailedException)
            {
                ;//do nothing, just not update this document
            }
        }

        internal static Expression BuildCheckExistedQtiOnlineTestSessionIdExpression(int qtiOnlineTestSessionID)
        {
            var mustExistedExpression = new Expression();
            mustExistedExpression.ExpressionStatement = "QTIOnlineTestSessionID = :qtiOnlineTestSessionID";
            mustExistedExpression.ExpressionAttributeValues[":qtiOnlineTestSessionID"] = qtiOnlineTestSessionID;
            return mustExistedExpression;
        }

        internal static UpdateItemOperationConfig BuildCheckExistedQtiOnlineTestSessionIdUpdateConfig(
            int qtiOnlineTestSessionID)
        {
            return new UpdateItemOperationConfig
            {
                ConditionalExpression = BuildCheckExistedQtiOnlineTestSessionIdExpression(qtiOnlineTestSessionID)
            };
        }
    }
}