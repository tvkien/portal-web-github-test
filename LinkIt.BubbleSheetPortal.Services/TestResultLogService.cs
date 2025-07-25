using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Repositories.TestResultRemoverLog;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class TestResultLogService
    {
        private readonly ITestResultAuditRepository testResultAuditRepository;
         private readonly ITestResultLogRepository testResultLogRepository;
         private readonly ITestResultScoreLogRepository testResultScoreLogRepository;
         private readonly ITestResultSubScoreLogRepository testResultSubScoreLogRepository;
         private readonly ITestResultProgramLogRepository testResultProgramLogRepository;
         private readonly IAnswerLogRepository answerLogRepository;
         private readonly IAnswerSubLogRepository answerSubLogRepository;

         public TestResultLogService(ITestResultAuditRepository testResultAuditRepository,
             ITestResultLogRepository testResultLogRepository,
             ITestResultScoreLogRepository testResultScoreLogRepository,
             ITestResultSubScoreLogRepository testResultSubScoreLogRepository,
             ITestResultProgramLogRepository testResultProgramLogRepository,
             IAnswerLogRepository answerLogRepository,
             IAnswerSubLogRepository answerSubLogRepository)
        {
            this.testResultAuditRepository = testResultAuditRepository;
            this.testResultLogRepository = testResultLogRepository;
            this.testResultScoreLogRepository = testResultScoreLogRepository;
            this.testResultSubScoreLogRepository = testResultSubScoreLogRepository;
            this.testResultProgramLogRepository = testResultProgramLogRepository;
            this.answerLogRepository = answerLogRepository;
            this.answerSubLogRepository = answerSubLogRepository;
        }

         public void Save(TestResultAudit testResultAudit)
         {
             testResultAuditRepository.Save(testResultAudit);
         }

         public void Save(List<TestResultAudit> testResultAudit)
        {
            testResultAuditRepository.Save(testResultAudit);
        }
         public void Save(List<TestResultLog> testResultLog)
         {
             testResultLogRepository.Save(testResultLog);
         }
         public void Save(List<TestResultScoreLog> testResultScoreLogs)
         {
             testResultScoreLogRepository.Save(testResultScoreLogs);
         }
         public void Save(List<TestResultSubScoreLog> testResultSubScoreLogs)
         {
             testResultSubScoreLogRepository.Save(testResultSubScoreLogs);
         }
         public void Save(List<TestResultProgramLog> testResultProgramLogs)
         {
             testResultProgramLogRepository.Save(testResultProgramLogs);
         }
         public void Save(List<AnswerLog> answerLogs)
         {
             answerLogRepository.Save(answerLogs);
         }
        
         public void Save(List<AnswerSubLog> answerSubLogs)
         {
             answerSubLogRepository.Save(answerSubLogs);
         }

        public void SaveTestResultRemoverLog(TestResultAudit testResultAudit)
        {
            testResultAuditRepository.SaveTestResultRemoverLog(testResultAudit);
        }
    }
}
