using LinkIt.BubbleSheetPortal.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.NavigatorReport
{
    public interface INavigatorReportDetailRepository
    {
        IQueryable<NavigatorReportDetailEntity> SelectAllActive();
        IQueryable<int> GetAssociateTeacherIds(int navigatorReportID);
    }
}
