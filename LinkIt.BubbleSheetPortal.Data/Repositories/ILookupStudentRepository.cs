using System;
using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.StudentLookup;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface ILookupStudentRepository
    {
        List<StudentLoginSlipDto> GetStudentLoginSlip(string studentIds, string url, string logo);
        List<TestResultMetaDto> GetSessionStudent(int studentId);
        IEnumerable<StudentLookupResult> LookupStudent(LookupStudentCustom obj, int skip, int pageSize,
            string sortColumns, string selectedUserIds = "");

        List<LookupStudent> SGOLookupStudent(LookupStudentCustom obj, int pageIndex, int pageSize, ref int? totalRecords,
           string sortColumns);

        List<Race> LookupStudentGetRace(int districtId, int userId, int roleId);

        List<School> LookupStudentGetAdminSchool(int districtId, int userId, int roleId);

        void GenRCode(Dictionary<int, string> studentRCodes);

        Student CheckExistCodeStartWithZero(int districtId, string code, int studentId);
        void TrackingLastSendDistributeEmail(int studentId, DateTime utcNow);
    }
}
