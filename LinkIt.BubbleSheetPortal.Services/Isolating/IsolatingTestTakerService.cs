using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Repositories.Isolating;
using LinkIt.BubbleSheetPortal.Models.Isolating;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Services.Isolating
{
    public class IsolatingTestTakerService
    {
        private readonly IIsolatingTestTakerRepository _isolatingTestTakerRepository;

        public IsolatingTestTakerService(IIsolatingTestTakerRepository isolatingTestTakerRepository)
        {
            _isolatingTestTakerRepository = isolatingTestTakerRepository;
        }

        public IQueryable<IsolatingTestSessionAnswerDTO> GetTestState(int qtiOnlineTestSessionID)
        {
            var result = _isolatingTestTakerRepository.GetTestState(qtiOnlineTestSessionID);
            return result;
        }

        public IQueryable<OnlineTestSessionAnswerDTO> GetOnlineTestSessionAnswer(string qtiOnlineTestSessionIDs)
        {
            var result = _isolatingTestTakerRepository.GetOnlineTestSessionAnswer(qtiOnlineTestSessionIDs);
            return result;
        }

        public IQueryable<OnlineTestSessionDTO> GetOnlineTestSessionStatusIsolating(string qtiTestClassAssignmentIDs)
        {
            var result = _isolatingTestTakerRepository.GetOnlineTestSessionStatusIsolating(qtiTestClassAssignmentIDs);
            return result;
        }

        public void PausedOnlineTest(string onlineTestSessionIDs)
        {
            _isolatingTestTakerRepository.PausedOnlineTest(onlineTestSessionIDs);
        }

        public void SubmitOnlineTest(string onlineTestSessionIDs)
        {
            _isolatingTestTakerRepository.SubmitOnlineTest(onlineTestSessionIDs);
        }
      
        public void UpdateAnswerText(int? answerID, int? answerSubID, bool? saved)
        {
            _isolatingTestTakerRepository.UpdateAnswerText(answerID, answerSubID, saved);
        }

        public int? GetQTIOnlineTestSessionStatus(int qtiOnlineTestSessionID)
        {
            var result = _isolatingTestTakerRepository.GetQTIOnlineTestSessionStatus(qtiOnlineTestSessionID);
            return result;
        }

        public void IsolatingReopenFailedTestSession(int? qtiOnlineTestSessionID)
        {
            _isolatingTestTakerRepository.IsolatingReopenFailedTestSession(qtiOnlineTestSessionID);
        }

        public void IsolatingReopenFailedTestSessions(List<int?> qtiOnlineTestSessionIDs)
        {
            if (qtiOnlineTestSessionIDs == null || qtiOnlineTestSessionIDs.Count == 0) return;

            foreach (var qtiOnlineTestSessionID in qtiOnlineTestSessionIDs)
            {
                if (!qtiOnlineTestSessionID.HasValue) continue;
                _isolatingTestTakerRepository.IsolatingReopenFailedTestSession(qtiOnlineTestSessionID);
            }
        }

        public IQueryable<IsolatingPostAnswerLogDTO> GetPostAnswerLogs(int qtiOnlineTestSessionID)
        {
            var result = _isolatingTestTakerRepository.GetPostAnswerLogs(qtiOnlineTestSessionID);
            return result;
        }

        /// <summary>
        /// Support recover answer from PostAnswerLog
        /// </summary>
        /// <param name="answerID"></param>
        /// <param name="answerSubID"></param>
        /// <param name="saved"></param>
        public void UpdateAnswerText(string answerText, int answerId, int? answerSubID)
        {
            _isolatingTestTakerRepository.UpdateAnswerText(answerText, answerId, answerSubID);
        }
    }
}
