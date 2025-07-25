using LinkIt.BubbleSheetPortal.DynamoIsolating.Model;

namespace LinkIt.BubbleSheetPortal.DynamoIsolating.Repositories
{
    public interface IQTITestClassAssignmentDynamo
    {
        QTITestClassAssignment GetByID(int QTITestClassAssignmentID);
        //List<QTIOnlineTestSessionAnswer> Search(int qtiOnlineTestSessionID);
    }
}
