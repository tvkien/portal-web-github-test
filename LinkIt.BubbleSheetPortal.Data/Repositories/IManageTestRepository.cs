using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Models.ManageTest;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.Survey;
using LinkIt.BubbleSheetPortal.Models.Old.ManageTest;
using LinkIt.BubbleSheetPortal.Models.Old.UnGroup;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IManageTestRepository
    {
        IList<BankData> GetBanksByUserID(GetBanksByUserIDFilter filter);
        IList<BankData> GetFormBanksByUserID(int userID, int roleID, int schoolID, int districtID, bool showArchived, bool? hideBankOnlyTest, bool filterByDistrict = true);
        bool HasRightToEditTestBankForNetWorkAdmin(int bankId, int userId);
        UserBankAccessDTO GetUserBankAccess(UserBankAccessCriteriaDTO criteria);
        List<ListItem> GetGradeIncludes(int userId);
        List<Subject> GetSubjectIncludes(int userId);
        IList<BankData> GetSurveyBanksByUserID(int userID, int roleID, int districtID, bool showArchived, bool filterByDistrict = true);
        void UpdateSubScoreLabelSurveyTemplate(SurveyItem item);
        IEnumerable<ReviewSurveyData> GetReviewSurveys(int userID, int roleId, int? districtId, int? termId, int? surveyAssignmentType, int? surveyBankId, int? surveyId, bool showActiveAssignment, string sort, string search, int? skip, int? take);
        IList<ListItem> GetAssignSurveyBanksByUserID(int? districtId, int roleId, int userId);
    }
}
