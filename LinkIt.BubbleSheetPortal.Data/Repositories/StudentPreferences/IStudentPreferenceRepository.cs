using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs;
using LinkIt.BubbleSheetPortal.Models.DTOs.StudentPreference;
using LinkIt.BubbleSheetPortal.Models.StudentPreferenceDTOs;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.StudentPreferences
{
    public interface IStudentPreferenceRepository
    {
        StudentPreferencesEntity GetByLevel(string level, int levelID, int? virtualTestID, int? dataSetCategoryID);
        IQueryable<StudentPreferencesEntity> GetByLevelIds(string level, int[] levelIds, int? virtualTestID, int? dataSetCategoryID);
        IList<TestTypeDto> GetListTestType(int districtID, int userID, int roleID, int schoolID = 0);
        void SetStudentPreference(StudentPreferenceDto model);
        void SetStudentsPreference(List<StudentPreferenceDto> models);
        TestForStudentPreferenceResponseDto GetTestForStudentPreferences(StudentPreferenceRequestDto criteria);
        List<SubjectGradeDto> GetSubjectGradeByUserID(int districtId, int userId, int roleId, int schoolId);
        IEnumerable<GetAvailableTestTypeGradeAndSubjectForStudentPreferenceResult> GetAvailableTestTypeGradeAndSubjectForStudentPreference(SearchBankCriteria criteria);
        IEnumerable<ClassDto> GetAssociatedClassesThatHasTestResult(int userId, int? districtId, int schoolId, int roleId, int? virtualTestId = null);
        IEnumerable<DataSetCategoryDTO> GetDataSetCategories(GetDatasetCatogoriesParams catogoriesParams);
    }
}
