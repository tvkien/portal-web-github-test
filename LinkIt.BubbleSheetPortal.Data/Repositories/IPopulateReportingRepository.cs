using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IPopulateReportingRepository
    {
        List<Grade> ReportingGetGrades(int userId, int districtId, int roleId, 
            int virtualTestSubTypeId, DateTime? resultDateFrom, DateTime? resultDateTo, bool? isGetAllClass);

        List<Subject> ReportingGetSubjects(int gradeId, int districtId, int userId, int userRole,
            int virtualTestSubTypeId, DateTime? resultDateFrom, DateTime? resultDateTo, bool? isGetAllClass);

        List<ListItem> ReportingGetBanks(int subjectId, int districtId, int userId, int userRole,
            int virtualTestSubTypeId, DateTime? resultDateFrom, DateTime? resultDateTo, bool? isGetAllClass);

        List<ListItem> ReportingGetTests(int? gradeId, int? subjectId, int? bankId, int districtId, int userId, int userRole,
            int virtualTestSubTypeId, DateTime? resultDateFrom, DateTime? resultDateTo, bool? isGetAllClass);
    }
}
