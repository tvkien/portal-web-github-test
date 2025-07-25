using LinkIt.BubbleSheetPortal.DynamoIsolating.Repositories;

namespace LinkIt.BubbleSheetPortal.DynamoIsolating.Services
{
    public class PausedOnlineTestService : IPausedOnlineTestService
    {
        private const int PausedStatusID = 3;

        private IQTIOnlineTestSessionDynamo _qtiOnlineTestSessionDynamo;
        public PausedOnlineTestService(IQTIOnlineTestSessionDynamo qtiOnlineTestSessionDynamo)
        {
            _qtiOnlineTestSessionDynamo = qtiOnlineTestSessionDynamo;
        }
        public void PausedOnlineTest(string onlineTestSessionIDs)
        {
            if (string.IsNullOrWhiteSpace(onlineTestSessionIDs)) return;
            var ids = onlineTestSessionIDs.Split(',');
            foreach(var id in ids)
            {
                var qtiOnlineTestSessionID = int.Parse(id);
                _qtiOnlineTestSessionDynamo.ChangeStatus(qtiOnlineTestSessionID, PausedStatusID);
            }
        }
    }
}
