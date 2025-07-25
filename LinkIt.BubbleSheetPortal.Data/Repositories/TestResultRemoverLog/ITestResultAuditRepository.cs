using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.TestResultRemoverLog
{
    public interface ITestResultAuditRepository
    {
        void Save(IList<TestResultAudit> testResultAudits);
        void Save(TestResultAudit testResultAudit);
        void SaveTestResultRemoverLog(TestResultAudit testResultAudit);
    }
}
