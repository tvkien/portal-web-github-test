using LinkIt.BubbleSheetPortal.DynamoIsolating.Model;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.DynamoIsolating.Repositories
{
    public interface IQTIOnlineTestSessionAnswerDynamo
    {
        QTIOnlineTestSessionAnswer GetByID(int qtiOnlineTestSessionAnswerID);
        List<QTIOnlineTestSessionAnswer> Search(int qtiOnlineTestSessionID);
        void UpdateAnswerText(int qtiOnlineTestSessionID, int answerID, string answerText);
        void UpdateAnswerTemp(int qtiOnlineTestSessionID, int answerID, string answerTemp);
        void UpdateQTIOnlineTestSessionAnswerSubs(int qtiOnlineTestSessionID, int answerID, List<QTIOnlineTestSessionAnswerSub> answerSubs);
        List<QTIOnlineTestSessionAnswer> SearchAnswerOfStudent(int qtiOnlineTestSessionID, int virtualQuestionId);
        void UpdatePointsEarned(int qtiOnlineTestSessionID, int answerID, int? pointsEarned = null);
    }
}
