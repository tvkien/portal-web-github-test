using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.NavigatorReport
{
    public interface INavigatorReportLogRepository
    {
        void Add(NavigatorReportLogEntity log);
        void AddRange(List<NavigatorReportLogDto> lstReportDetailError);
    }
}
