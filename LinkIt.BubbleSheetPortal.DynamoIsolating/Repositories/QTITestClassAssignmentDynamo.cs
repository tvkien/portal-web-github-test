using LinkIt.BubbleSheetPortal.DynamoIsolating.Model;
using Amazon.DynamoDBv2;
using LinkIt.BubbleSheetPortal.DynamoConnector;
using Amazon.DynamoDBv2.DocumentModel;
using LinkIt.BubbleSheetPortal.DynamoConnector.Common;
using System.Linq;
using LinkIt.BubbleSheetPortal.DynamoConnector.DynamoPrefixTableNameProvider;

namespace LinkIt.BubbleSheetPortal.DynamoIsolating.Repositories
{
    public class QTITestClassAssignmentDynamo : IQTITestClassAssignmentDynamo
    {
        private IAmazonDynamoDB _client;
        private IDynamoPrefixTableNameProvider _dynamoPrefixTableNameProvider;
        public QTITestClassAssignmentDynamo(IDynamoPrefixTableNameProvider dynamoPrefixTableNameProvider)
        {
            _client = new AmazonDynamoDBClient();
            _dynamoPrefixTableNameProvider = dynamoPrefixTableNameProvider;
        }
        public QTITestClassAssignment GetByID(int qtiTestClassAssignmentID)
        {
            var table = DynamoDBQuery.GetTable<QTITestClassAssignment>(_client, _dynamoPrefixTableNameProvider);

            QueryFilter filter = new QueryFilter();
            filter.AddCondition(ExpressionUtil<QTITestClassAssignment>.GetPath(o => o.QTITestClassAssignmentID), QueryOperator.Equal, qtiTestClassAssignmentID);
            QueryOperationConfig config = new QueryOperationConfig()
            {
                Filter = filter,
                Select = SelectValues.AllAttributes,
                IndexName = "QTITestClassAssignmentID-index"//An index must be created with QTITestClassAssignmentID is Partion Key

            };

            var dynamoData = DynamoDBQuery.QueryDocument<QTITestClassAssignment>(table, config);
            if (dynamoData == null)
            {
                return null;
            }
            else
            {
                return dynamoData.FirstOrDefault();
            }

        }
    }
}
