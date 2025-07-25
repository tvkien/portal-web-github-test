using LinkIt.BubbleSheetPortal.DynamoIsolating.Repositories;

namespace LinkIt.BubbleSheetPortal.DynamoIsolating.Services
{
    public class SubmitOnlineTestService : ISubmitOnlineTestService
    {
        private const int PendingReviewStatusID = 5;

        private IQTIOnlineTestSessionDynamo _qtiOnlineTestSessionDynamo;
        public SubmitOnlineTestService(IQTIOnlineTestSessionDynamo qtiOnlineTestSessionDynamo)
        {
            _qtiOnlineTestSessionDynamo = qtiOnlineTestSessionDynamo;
        }
        public void SubmitOnlineTest(string onlineTestSessionIDs)
        {
            if (string.IsNullOrWhiteSpace(onlineTestSessionIDs)) return;
            var ids = onlineTestSessionIDs.Split(',');
            foreach (var id in ids)
            {
                var qtiOnlineTestSessionID = int.Parse(id);
                _qtiOnlineTestSessionDynamo.ChangeStatus(qtiOnlineTestSessionID, PendingReviewStatusID);
            }
        }
    }
}
