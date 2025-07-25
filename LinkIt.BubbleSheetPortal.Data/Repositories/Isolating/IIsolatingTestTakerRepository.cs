using System.Linq;
using LinkIt.BubbleSheetPortal.Models.Isolating;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.Isolating
{
    public interface IIsolatingTestTakerRepository
    {
        IQueryable<IsolatingTestSessionAnswerDTO> GetTestState(int qtiOnlineTestSessionID);
        IQueryable<OnlineTestSessionAnswerDTO> GetOnlineTestSessionAnswer(string qtiOnlineTestSessionIDs);
        IQueryable<OnlineTestSessionDTO> GetOnlineTestSessionStatusIsolating(string qtiTestClassAssignmentIDs);
        void PausedOnlineTest(string onlineTestSessionIDs);
        void SubmitOnlineTest(string onlineTestSessionIDs);
        void UpdateAnswerText(int? answerID, int? answerSubID, bool? saved);
        int? GetQTIOnlineTestSessionStatus(int qtiOnlineTestSessionID);
        void IsolatingReopenFailedTestSession(int? qtiOnlineTestSessionID);
        IQueryable<IsolatingPostAnswerLogDTO> GetPostAnswerLogs(int qtiOnlineTestSessionID);
        void UpdateAnswerText(string answerText, int answerId, int? answerSubID);
    }
}
