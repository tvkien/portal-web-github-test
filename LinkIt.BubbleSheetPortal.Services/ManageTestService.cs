using System.Collections.Generic;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.ManageTest;
using LinkIt.BubbleSheetPortal.Models.Old.UnGroup;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class ManageTestService
    {
        private readonly IManageTestRepository manageTestRepository;

        public ManageTestService(IManageTestRepository manageTestRepository)
        {
            this.manageTestRepository = manageTestRepository;
        }

        public List<BankData> GetBanksByUserID(GetBanksByUserIDFilter filter)
        {
            return manageTestRepository.GetBanksByUserID(filter).ToList();
        }
        public List<BankData> GetFormBanksByUserID(int userID, int userRoleID, int userSchoolID, int userDistrictID, bool? hideBankOnlyTest, bool showArchived = true, bool filterByDistrict = true)
        {
            return manageTestRepository.GetFormBanksByUserID(userID, userRoleID, userSchoolID, userDistrictID, showArchived, hideBankOnlyTest, filterByDistrict).ToList();
        }

        public List<ListItem> GetGradeIncludes(int userId)
        {
            return manageTestRepository.GetGradeIncludes(userId);
        }

        public List<Subject> GetSubjectIncludes(int userId)
        {
            return manageTestRepository.GetSubjectIncludes(userId);
        }

    }
}
