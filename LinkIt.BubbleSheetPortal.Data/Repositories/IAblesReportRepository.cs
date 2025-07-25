using System;
using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IAblesReportRepository
    {
        List<ReportType> GetAllReportTypes();
        List<AblesPointsEarnedResponseData> GetPointsEarnedResponsed(int districtId, string studentList, int schoolId,
            string termIdList, string testIdList, bool isASD);

        List<AblesDataDropDown> GetAblesDataDropDown(int? districtId, int? schoolId, int? teacherId, int userId, int roleId,
             string districtTermIdStr, string testIdList, bool isASD, int year);
        List<AblesReportJobData> GetAblesReportJobs(int districtId, int userId, DateTime fromDate, DateTime toDate);

        List<AblesStudent> GetStudentHasData(int? districtId, int? classId, string districtTermIdStr, string testIdList, bool isASD);
        int? GetMinResultYearBySchool(int? schoolId);
        List<AblesPointsEarnedResponseData> GetAblesDataForAdminReporting(int studentId, int testresultId);
    }
}