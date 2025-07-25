using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.NavigatorReport
{
    public class NavigatorConfigurationRepository : INavigatorConfigurationRepository
    {
        private readonly Table<NavigatorConfigurationEntity> table;
        private readonly NavigatorReportDataContext _navigatorReportDataContext;

        public NavigatorConfigurationRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _navigatorReportDataContext = new NavigatorReportDataContext(connectionString);
            table = _navigatorReportDataContext.GetTable<NavigatorConfigurationEntity>();
        }

        public void Delete(NavigatorConfigurationDTO item)
        {
        }


        public void Save(NavigatorConfigurationDTO item)
        {
        }

        public IQueryable<NavigatorConfigurationDTO> Select()
        {
            return table.Select(c => new NavigatorConfigurationDTO()
            {
                CanPublishDistrictAdmin = c.CanPublishDistrictAdmin,
                CanPublishSchoolAdmin = c.CanPublishSchoolAdmin,
                CanPublishStudent = c.CanPublishStudent,
                CanPublishTeacher = c.CanPublishTeacher,
                NavigatorConfigurationID = c.NavigatorConfigurationID,
                UseClass = c.UseClass,
                ReportName = c.ReportName,
                UseSchool = c.UseSchool,
                UseStudent = c.UseStudent,
                UseUser = c.UseUser,
                KeywordMandatory = c.KeywordMandatory,
                ShortName  = c.ShortName,
                NavigatorCategoryID = c.NavigatorCategoryID,
                PeriodMandatory = c.PeriodMandatory,
                ReportTypePattern = c.ReportTypePattern,
                SchoolPattern = c.SchoolPattern,
                SuffixPattern = c.SuffixPattern
            });
        }
    }
}
