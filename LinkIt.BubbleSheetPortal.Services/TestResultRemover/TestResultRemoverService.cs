using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.TestResultRemover;

namespace LinkIt.BubbleSheetPortal.Services.TestResultRemover
{
    public class TestResultRemoverService
    {
        private VirtualTestDistrictService _virtualTestDistrictService;
        private TestResultLogService _testResultLogService;
        private TestResultService _testResultService;

        public TestResultRemoverService(VirtualTestDistrictService virtualTestDistrictService, TestResultLogService testResultLogService
            , TestResultService testResultService)
        {
            _virtualTestDistrictService = virtualTestDistrictService;
            _testResultLogService = testResultLogService;
            _testResultService = testResultService;
        }

        public void LogTestResultDataBeforeRemove(TestResultAuditModel model)
        {
            var testResultIDStr = model.TestResultID.ToString(CultureInfo.InvariantCulture);

            var testResultAudit = CreateTestResultAudit(model.VisitorsIPAddr, model.TestResultID, model.UserID);
            _testResultLogService.Save(testResultAudit);

            var testResultLog =
                _virtualTestDistrictService.GetTestResultDetails(testResultIDStr).ToList();
            _testResultLogService.Save(testResultLog);

            var testResultScoreLogs = _virtualTestDistrictService.GetTestResultScores(testResultIDStr);
            _testResultLogService.Save(testResultScoreLogs);

            var testResultScoreIds = testResultScoreLogs.Select(x => x.TestResultScoreID);
            var listTestResultScoreIds = string.Join(",", testResultScoreIds);
            var testResultSubScoreLogs = _virtualTestDistrictService.GetTestResultSubScores(listTestResultScoreIds);
            _testResultLogService.Save(testResultSubScoreLogs);

            var testResultProgamLogs = _virtualTestDistrictService.GetTestResultProgram(testResultIDStr);
            _testResultLogService.Save(testResultProgamLogs);

            var answers = _virtualTestDistrictService.GetAnswersByTestResultId(testResultIDStr);
            _testResultLogService.Save(answers);

            var answerIds = answers.Select(x => x.AnswerID);
            var listAnswerIds = string.Join(",", answerIds);
            var answerSubLogs = _virtualTestDistrictService.GetAnswerSubsByAnswerId(listAnswerIds);

            _testResultLogService.Save(answerSubLogs);
        }

        private TestResultAudit CreateTestResultAudit(string visitorsIPAddr, int testResultID, int userID)
        {
            var testResultAudit = new TestResultAudit()
            {
                TestResultId = testResultID,
                AuditDate = DateTime.Now,
                IPAddress = visitorsIPAddr,
                UserId = userID,
                Type = ContaintUtil.Remover
            };

            return testResultAudit;
        }
    }
}
