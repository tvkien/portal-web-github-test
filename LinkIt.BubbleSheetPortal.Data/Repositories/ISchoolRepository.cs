using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.Commons;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface ISchoolRepository
    {
        bool CheckUserCanAccessSchool(int userId, int roleId, int schoolId);
        string GetSchoolNameById(int id);
        TimeZoneDto GetSchoolTimeZone(int schoolId);
        ItemValue CreateSurveySchoolClass(int districtId, int termId, string surveyName, int userId);
        IQueryable<School> GetSchoolsByDistrictV2(GetSchoolRequestModel input, ref int? totalRecords);
        List<School> GetTLDSSchoolsByDistrict(int districtId);
    }
}
