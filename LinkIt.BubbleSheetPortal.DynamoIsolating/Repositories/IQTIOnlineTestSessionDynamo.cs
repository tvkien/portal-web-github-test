using LinkIt.BubbleSheetPortal.DynamoIsolating.Model;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.DynamoIsolating.Repositories
{
    public interface IQTIOnlineTestSessionDynamo
    {
        QTIOnlineTestSession GetByID(int qtiOnlineTestSessionID);
        List<QTIOnlineTestSession> Search(string assignmentGuid);
        void ChangeStatus(int qtiOnlineTestSessionID, int statusID);
    }
}
