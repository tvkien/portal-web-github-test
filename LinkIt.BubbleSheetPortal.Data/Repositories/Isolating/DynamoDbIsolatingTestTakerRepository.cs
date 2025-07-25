using System.Linq;
using Amazon.DynamoDBv2.DocumentModel;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models.Isolating;
using Amazon.DynamoDBv2;
using LinkIt.BubbleSheetPortal.DynamoIsolating.Services;
using System;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.Isolating
{
    public class DynamoDbIsolatingTestTakerRepository : IIsolatingTestTakerRepository
    {
        private IGetTestStateService _getTestStateService;
        private IGetOnlineTestSessionAnswerService _getOnlineTestSessionAnswerService;
        private IGetOnlineTestSessionStatusIsolatingService _getOnlineTestSessionStatusIsolatingService;
        private IPausedOnlineTestService _pausedOnlineTestService;
        private ISubmitOnlineTestService _submitOnlineTestService;
        private IUpdateAnswerTextService _updateAnswerTextService;
        private IGetQTIOnlineTestSessionStatusService _getQTIOnlineTestSessionStatusService;
        private IReopenFailedTestSessionService _reopenFailedTestSessionService;
        private IPostAnswerLogService _postAnswerLogService;
        public DynamoDbIsolatingTestTakerRepository(IGetTestStateService getTestStateService
            , IGetOnlineTestSessionAnswerService getOnlineTestSessionAnswerService
            , IGetOnlineTestSessionStatusIsolatingService getOnlineTestSessionStatusIsolatingService
            , IPausedOnlineTestService pausedOnlineTestService
            , ISubmitOnlineTestService submitOnlineTestService
            , IUpdateAnswerTextService updateAnswerTextService
            , IGetQTIOnlineTestSessionStatusService getQTIOnlineTestSessionStatusService,
            IReopenFailedTestSessionService reopenFailedTestSessionService
            ,IPostAnswerLogService postAnswerLogService)
        {
            _getTestStateService = getTestStateService;
            _getOnlineTestSessionAnswerService = getOnlineTestSessionAnswerService;
            _getOnlineTestSessionStatusIsolatingService = getOnlineTestSessionStatusIsolatingService;
            _pausedOnlineTestService = pausedOnlineTestService;
            _submitOnlineTestService = submitOnlineTestService;
            _updateAnswerTextService = updateAnswerTextService;
            _getQTIOnlineTestSessionStatusService = getQTIOnlineTestSessionStatusService;
            _reopenFailedTestSessionService = reopenFailedTestSessionService;
            _postAnswerLogService = postAnswerLogService;
        }

        public IQueryable<IsolatingTestSessionAnswerDTO> GetTestState(int qtiOnlineTestSessionID)
        {
            var result = _getTestStateService.GetTestState(qtiOnlineTestSessionID);
            return result;
        }

        public IQueryable<OnlineTestSessionAnswerDTO> GetOnlineTestSessionAnswer(string qtiOnlineTestSessionIDs)
        {
            var result = _getOnlineTestSessionAnswerService.GetOnlineTestSessionAnswer(qtiOnlineTestSessionIDs);
            return result;
        }

        public IQueryable<OnlineTestSessionDTO> GetOnlineTestSessionStatusIsolating(string qtiTestClassAssignmentIDs)
        {
            var result = _getOnlineTestSessionStatusIsolatingService.GetOnlineTestSessionStatusIsolating(qtiTestClassAssignmentIDs);
            return result;
        }

        public void PausedOnlineTest(string onlineTestSessionIDs)
        {
            _pausedOnlineTestService.PausedOnlineTest(onlineTestSessionIDs);
        }

        public void SubmitOnlineTest(string onlineTestSessionIDs)
        {
            _submitOnlineTestService.SubmitOnlineTest(onlineTestSessionIDs);
        }

        public void UpdateAnswerText(int? answerID, int? answerSubID, bool? saved)
        {
            _updateAnswerTextService.UpdateAnswerText(answerID, answerSubID, saved);
        }

        public int? GetQTIOnlineTestSessionStatus(int qtiOnlineTestSessionID)
        {
            var result = _getQTIOnlineTestSessionStatusService.GetQTIOnlineTestSessionStatus(qtiOnlineTestSessionID);
            return result;
        }

        public void IsolatingReopenFailedTestSession(int? qtiOnlineTestSessionID)
        {
            if(qtiOnlineTestSessionID.HasValue)
                _reopenFailedTestSessionService.ReopenFailedTestSession(qtiOnlineTestSessionID.Value);
        }

        public IQueryable<IsolatingPostAnswerLogDTO> GetPostAnswerLogs(int qtiOnlineTestSessionID)
        {
            var result = _postAnswerLogService.GetPostAnswerLogs(qtiOnlineTestSessionID);
            return result;
        }

        /// <summary>
        /// Support Recover answer from post answer log
        /// </summary>
        /// <param name="answerText"></param>
        /// <param name="answerId"></param>
        /// <param name="answerSubID"></param>
        //PostAnswerLog
        public void UpdateAnswerText(string answerText, int answerId, int? answerSubID)
        {
            _updateAnswerTextService.UpdateAnswerText(answerText, answerId, answerSubID);
        }
    }
}
