using System.Collections;
using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IChytenReportRepository
    {
        IList<ListItem> GetBankByDistrictId(int districtId, string schoolIdString, int userId, int roleId);
        IList<ListItem> GetSchoolByDistrictIdAndBankId(int districtId, int bankId, string schoolIdString, int userId, int roleId);
        IList<ListItem> GetTeacherByDistrictIdAndBankIdAndSchoolId(int districtId, int bankId, int schoolId, string schoolIdString, int userId, int roleId);
        IList<ListItem> GetClassedHaveTestResult(int districtId, int bankId, int schoolId, int teacherId, int termId, string schoolIdString, int userId, int roleId);
        IList<ListItem> GetTermsHaveTestResult(int districtId, int bankId, int schoolId, int teacherId, string schoolIdString, int userId, int roleId);

        IList<SpecializedTestResult> GetTestResultFilter(int districtId, int bankId, int schoolId, int teacherId,
            int classId, int termId, string schoolIdString, int userId, int roleId);

        IList<string> GetTestCenterZipCodesByEmail(string email);
        bool CheckTestCenterInActive(string zipcode);
        bool CheckTestCenterActive(string zipcode);
        int? GetSchoolIdTestCenter(string zipcode);
        IList<int?> GetTestCenterSchoolIdsByEmail(string email);
    }
}