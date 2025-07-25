using System.Collections.Generic;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IAuthorGroupRepository
    {
        void DeleteAuthorGroup(int authorGroupId);
        IQueryable<GetAuthorGroupListResult> GetAuthorGroupListHasAccessTo(int userId, int stateId, int districtId, int schoolId);
        IQueryable<AuthorGroupList> GetAuthorGroupList(int stateId, int districtId, int schoolId);
        IQueryable<User> GetUsersUserHasAccessTo(int userId, int stateId, int districtId, int schoolId);
        List<GetAuthorGroupBanksResult> GetAuthorGroupBanks(int bankID ,int userId);

        IQueryable<GetAuthorGroupNotInBankResult> GetAuthorGroupNotInBank(int userId, int stateId, int districtId,
            int schoolId, int bankID);
    }
}