using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IQTIOnlineTestSessionRepository : IRepository<QTIOnlineTestSession>
    {
        void ReopenTest(int qtiOnlineTestSession, string imageIndexs);
        bool CanReopenTest(int qtiOnlineTestSessionID);
        IQueryable<GetTestStudentSessionExportResponse> GetTestStudentSessionsExport(GetTestStudentSessionExportRequest request);
        SubmitOnlineTestResult SubmitOnlineTest(int qtiOnlineTestSessionID, bool timeOver, string token, int type, int? requestUserID);
        bool HasExistTestInProgress(int virtualTestId);
    }
}
