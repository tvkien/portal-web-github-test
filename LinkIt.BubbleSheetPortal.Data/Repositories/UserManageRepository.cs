using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class UserManageRepository : IUserManageRepository
    {
        private readonly Table<UserManageView> table;
        private readonly UserDataContext _userDataContext;


        public UserManageRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = UserDataContext.Get(connectionString).GetTable<UserManageView>();
            _userDataContext = UserDataContext.Get(connectionString);
        }

        public IQueryable<UserManage> Select()
        {
            return table.Select(user => new UserManage
                                            {
                                                SchoolList = user.SchoolList ?? string.Empty,
                                                UserId = user.UserID,
                                                UserStatusId = user.UserStatusID,
                                                DistrictName = user.DistrictName,
                                                FirstName = user.NameFirst,
                                                LastName = user.NameLast,
                                                RoleName = user.RoleName,
                                                StateName = user.StateName,
                                                UserName = user.UserName,
                                                DistrictId = user.DistrictID,
                                                RoleId = user.RoleID,
                                                SchoolIdList = user.SchoolIDList == null
                                                                   ? new List<string>()
                                                                   : user.SchoolIDList.Substring(0,
                                                                                                 user.SchoolIDList.
                                                                                                     Length - 1).Split(
                                                                                                         '|').ToList(),
                                                StateId = user.StateID,
                                            });
        }

        public IQueryable<UserManage> GetManageUsersByRole(int userId, int districtId, int roleId, int schoolID, string staffName, bool inactive, int? otherUserId = null)
        {
            var query = _userDataContext.GetUserManageByRole(userId, districtId, roleId, schoolID, staffName, inactive, otherUserId).ToList();
            return query.Select(user => new UserManage()
            {
                SchoolList = user.SchoolList ?? string.Empty,
                UserId = user.UserID,
                UserStatusId = user.UserStatusID,
                DistrictName = user.DistrictName,
                FirstName = user.NameFirst,
                LastName = user.NameLast,
                RoleName = user.RoleName,
                StateName = user.StateName,
                UserName = user.UserName,
                DistrictId = user.DistrictID,
                RoleId = user.RoleID,
                SchoolIdList = user.SchoolIDList == null
                                   ? new List<string>()
                                   : user.SchoolIDList.Substring(0, user.SchoolIDList.Length - 1).Split('|').ToList(),
                StateId = user.StateID,

            }).AsQueryable();
        }

        public int GetUserLoginFailedCount(int userId)
        {
            var data = _userDataContext.GetUserLoginFailedCount(userId).FirstOrDefault();
            return data == null ? 1 : data.LoginFailedCount.GetValueOrDefault(1);
        }

        public void DeleteUserLoginFailedCount(int userId)
        {
            _userDataContext.DeleteUserFailedCount(userId);
        }

        public void IncreaseUserLoginFailedCount(int userId)
        {
            _userDataContext.IncreaseUserLoginFailedCount(userId);
        }

        public bool CheckUserCodeExistsStartWithRezoByDistrictID (int districtId, string userCode, int userId)
        {
            var query = _userDataContext.GetAllUserStartWithCodeByDistrictID(districtId, userCode).ToList();
            if(userId > 0)
            {
                if (query.Any(o => o.Code.TrimStart('0').Equals(userCode) && o.UserID != userId))
                    return true;
            }
            else
            {
                if (query.Any(o => o.Code.TrimStart('0').Equals(userCode)))
                    return true;
            }            
            return false;
        }

        public UserWelcomeInfo GetRoleAndGroupNameByUserId (int userId)
        {
            var result = new UserWelcomeInfo();
            var lstUserGroup = _userDataContext.DisplayUserGroupAndRoleViews.Where(o => o.UserID == userId).ToList();
            if (lstUserGroup != null && lstUserGroup.Count > 0)
            {
                var strUserGroup = lstUserGroup[0].RoleDisplay;
                result.RoleName = strUserGroup;
                foreach (var item in lstUserGroup)
                {
                    if (string.IsNullOrEmpty(item.GroupName) == false)
                    {
                        strUserGroup += ", " + item.GroupName;
                    }
                }
                result.GroupName = string.Join(", ", lstUserGroup.Where(p => !string.IsNullOrEmpty(p.GroupName)).Select(p => p.GroupName));
                result.RoleAndGroupName = strUserGroup.Trim();
            }
            return result;
        }


        public GetGroupUserResponse GetUserManageByRoleInGroup(GetGroupUserRequest request)
        {
            var result = new GetGroupUserResponse();
            int? totalRecord = 0;

            var data = _userDataContext.GetUserManageByRoleInGroup(request.RoleID, request.UserID, request.DistrictID, request.XLIGroupID, request.IsShowInactiveUser,
                 request.GeneralSearch, request.SortColumn, request.SortDirection, request.StartRow, request.PageSize, ref totalRecord);

            result.TotalRecord = totalRecord.HasValue ? totalRecord.Value : 0;
            result.Data = data.Select(user => new UserManage()
            {
                SchoolList = user.SchoolList ?? string.Empty,
                UserId = user.UserID,
                UserStatusId = user.UserStatusID,
                FirstName = user.NameFirst,
                LastName = user.NameLast,
                RoleName = user.RoleName,
                UserName = user.UserName,
                DistrictId = user.DistrictID,
                RoleId = user.RoleID,
                GroupName = user.GroupName
            }).ToList();

            return result;
        }

        public GetUserForAddGroupResponse GetUserAvailableForAddGroup(GetUserForAddGroupRequest request)
        {
            var result = new GetUserForAddGroupResponse();
            int? totalRecord = 0;

            var data = _userDataContext.GetUserAvailableForAddGroup(request.UserRoleID, request.UserID, request.DistrictID, request.XLIGroupID, request.IsShowInactiveUser,
                 request.GeneralSearch,request.SchoolID,request.RoleID, request.SortColumn, request.SortDirection, request.StartRow, request.PageSize, ref totalRecord);

            result.TotalRecord = totalRecord.HasValue ? totalRecord.Value : 0;
            result.Data = data.Select(user => new UserManage()
            {
                SchoolList = user.SchoolList ?? string.Empty,
                UserId = user.UserID,
                UserStatusId = user.UserStatusID,
                FirstName = user.NameFirst,
                LastName = user.NameLast,
                RoleName = user.RoleName,
                UserName = user.UserName,
                DistrictId = user.DistrictID,
                RoleId = user.RoleID,
                GroupName = user.GroupName
            }).ToList();

            return result;
        }

        
        public bool RemoveUserFromGroup(int userID, int groupID)
        {
            int result = _userDataContext.RemoveUserFromGroup(userID, groupID);
            return result > 0;
        }


        public void AddUsersToGroup(string userIDs, int groupID)
        {
            _userDataContext.AddUsersToGroup(userIDs, groupID);
        }

        public List<GetRolesForManageUserGroupResult> GetRolesForManageUserGroup(int userRoleID, int groupID)
        {
            var result = new List<GetRolesForManageUserGroupResult>();
            var data = _userDataContext.GetRolesForManageUserGroup(userRoleID, groupID);
            result = data.ToList();
            return result;
        }
    }
}
