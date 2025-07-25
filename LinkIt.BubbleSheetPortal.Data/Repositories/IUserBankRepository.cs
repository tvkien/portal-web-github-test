using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.UserBank;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IUserBankRepository : IReadOnlyRepository<UserBank>
    {
        List<ListItem> GetBanksBySubjectId(int subjectId, int districtId, int userId, int userRole);
        List<ListItem> GetBanks(int subjectId, int districtId, int userId, int userRole);

        List<ListItem> ACTGetBanksBySubjectId(int subjectId, int districtId, int userId, int userRole);
        List<ListItem> ACTGetTestByBankId(int bankId, int districtId, int userId, int userRole);
        List<ListItem> GetBanksForItemSetSaveTest(int subjectId, int districtId, int userId, int userRole);
        List<ListItem> GetUserBanksBySubjectName(SearchBankCriteria criteria);

        List<ListItem> SATGetBanksBySubjectId(int subjectId, int districtId, int userId, int userRole);
        List<ListItem> SATGetTestByBankId(int bankId, int districtId, int userId, int userRole);
        List<ListItem> GetFormBanksBySubjectId(FormBankCriteria criteria);
        List<ListItem> GetFormBanksByMultipleSubjectIds(LoadBankByMultipleSubjectIdsCriteria criteria);
        List<UserBank> GetBanksBySubjectNamesAndGradeIDs(SearchBankAdvancedFilter criteria);
    }
}
