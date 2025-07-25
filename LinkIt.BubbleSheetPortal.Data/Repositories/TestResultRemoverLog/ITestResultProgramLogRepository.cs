using System.Collections.Generic;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.TestResultRemoverLog
{
    public interface ITestResultProgramLogRepository
    {
        void Save(IList<TestResultProgramLog> testResultProgramLogs);
    }
}
