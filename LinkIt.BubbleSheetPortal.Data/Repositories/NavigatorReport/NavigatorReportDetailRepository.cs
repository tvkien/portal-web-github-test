using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Data.Entities;
using System;
using System.Data.Linq;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.NavigatorReport
{
    public class NavigatorReportDetailRepository : INavigatorReportDetailRepository
    {
        private readonly Table<NavigatorReportDetailEntity> table;
        private readonly NavigatorReportDataContext _navigatorReportDataContext;

        public NavigatorReportDetailRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _navigatorReportDataContext = new NavigatorReportDataContext(connectionString);
            table = _navigatorReportDataContext.GetTable<NavigatorReportDetailEntity>();
        }

        public IQueryable<NavigatorReportDetailEntity> SelectAllActive()
        {
            var query = table.Where(c => (c.Status ?? "") != Constanst.Deleted);
            return query;
        }

        public IQueryable<int> GetAssociateTeacherIds(int navigatorReportID)
        {
            return SelectAllActive().Where(c =>
            c.NavigatorReportID == navigatorReportID
            && c.UserID != null
            ).Select(c => c.UserID.Value);
        }
    }
}
