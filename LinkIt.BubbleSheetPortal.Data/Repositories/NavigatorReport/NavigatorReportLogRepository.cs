using AutoMapper;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.NavigatorReport
{
    public class NavigatorReportLogRepository : INavigatorReportLogRepository
    {
        private readonly Table<NavigatorReportLogEntity> table;
        private string _connectionString;
        private readonly NavigatorReportDataContext _navigatorReportDataContext;

        public NavigatorReportLogRepository(IConnectionString conn)
        {
            this._connectionString = conn.GetLinkItConnectionString();
            _navigatorReportDataContext = new NavigatorReportDataContext(_connectionString);
            table = _navigatorReportDataContext.GetTable<NavigatorReportLogEntity>();
        }

        public void Add(NavigatorReportLogEntity log)
        {
            table.InsertOnSubmit(log);
            table.Context.SubmitChanges();
        }

        public void AddRange(List<NavigatorReportLogDto> lstReportDetailError)
        {
            var entities = lstReportDetailError?.Select(c => Mapper.Map<NavigatorReportLogEntity>(c))?.ToList();
            if (entities != null)
            {
                foreach (var entity in entities)
                {
                    table.InsertOnSubmit(entity);
                }
                table.Context.SubmitChanges();
            }
        }
    }
}
