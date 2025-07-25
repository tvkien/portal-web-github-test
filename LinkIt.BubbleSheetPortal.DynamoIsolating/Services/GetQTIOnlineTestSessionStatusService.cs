using LinkIt.BubbleSheetPortal.DynamoIsolating.Repositories;

namespace LinkIt.BubbleSheetPortal.DynamoIsolating.Services
{
    public class GetQTIOnlineTestSessionStatusService : IGetQTIOnlineTestSessionStatusService
    {
        private IQTIOnlineTestSessionDynamo _qtiOnlineTestSessionDynamo;
        public GetQTIOnlineTestSessionStatusService(IQTIOnlineTestSessionDynamo qtiOnlineTestSessionDynamo)
        {
            _qtiOnlineTestSessionDynamo = qtiOnlineTestSessionDynamo;
        }

        public int? GetQTIOnlineTestSessionStatus(int qtiOnlineTestSessionID)
        {
            var qtiOnlineTestSession = _qtiOnlineTestSessionDynamo.GetByID(qtiOnlineTestSessionID);
            if (qtiOnlineTestSession == null) return null;

            var result = qtiOnlineTestSession.StatusID;

            return result;
        }
    }
}
