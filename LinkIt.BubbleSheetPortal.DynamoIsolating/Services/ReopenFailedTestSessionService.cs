using LinkIt.BubbleSheetPortal.DynamoIsolating.Repositories;

namespace LinkIt.BubbleSheetPortal.DynamoIsolating.Services
{
    public class ReopenFailedTestSessionService : IReopenFailedTestSessionService
    {
        private IQTIOnlineTestSessionDynamo _qtiOnlineTestSessionDynamo;
        private IQTIOnlineTestSessionAnswerDynamo _qtiOnlineTestSessionAnswerDynamo;

        public ReopenFailedTestSessionService(IQTIOnlineTestSessionDynamo qtiOnlineTestSessionDynamo,
            IQTIOnlineTestSessionAnswerDynamo qtiOnlineTestSessionAnswerDynamo)
        {
            _qtiOnlineTestSessionDynamo = qtiOnlineTestSessionDynamo;
            _qtiOnlineTestSessionAnswerDynamo = qtiOnlineTestSessionAnswerDynamo;
        }
        public void ReopenFailedTestSession(int onlineTestSessionId)
        {
            _qtiOnlineTestSessionDynamo.ChangeStatus(onlineTestSessionId, 1);

            //Update PointsEarned from QTIOnlineTestSessionAnswer and QTIOnlineTestSessionAnswerSub
            var answerList = _qtiOnlineTestSessionAnswerDynamo.Search(onlineTestSessionId);
            foreach (var answer in answerList)
            {
                _qtiOnlineTestSessionAnswerDynamo.UpdatePointsEarned(onlineTestSessionId, answer.QTIOnlineTestSessionAnswerID);

                var answerSubList = answer.QTIOnlineTestSessionAnswerSubs;
                if (answerSubList != null)
                {
                    foreach (var answerSub in answerSubList)
                    {
                        answerSub.PointsEarned = null;
                    }
                    _qtiOnlineTestSessionAnswerDynamo.UpdateQTIOnlineTestSessionAnswerSubs(onlineTestSessionId,
                        answer.QTIOnlineTestSessionAnswerID, answerSubList);
                }
            }           
        }
    }
}
