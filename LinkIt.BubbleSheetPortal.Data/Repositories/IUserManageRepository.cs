using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Data.Entities;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IUserManageRepository: IReadOnlyRepository<UserManage>
    {
        IQueryable<UserManage> GetManageUsersByRole(int userId, int districtId, int roleId, int schoolID, string staffName, bool inactive, int? otherUserId = null);
        int GetUserLoginFailedCount(int userId);
        void DeleteUserLoginFailedCount(int userId);
        void IncreaseUserLoginFailedCount(int userId);
        bool CheckUserCodeExistsStartWithRezoByDistrictID(int districtId, string userCode, int userId);

        UserWelcomeInfo GetRoleAndGroupNameByUserId(int userId);

        GetGroupUserResponse GetUserManageByRoleInGroup(GetGroupUserRequest request);
        GetUserForAddGroupResponse GetUserAvailableForAddGroup(GetUserForAddGroupRequest request);
        bool RemoveUserFromGroup(int userID, int groupID);
        void AddUsersToGroup(string userIDs, int groupID);
        List<GetRolesForManageUserGroupResult> GetRolesForManageUserGroup(int userID, int groupID);
    }
}

