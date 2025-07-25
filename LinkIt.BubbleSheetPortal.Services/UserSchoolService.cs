using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class UserSchoolService
    {
        private readonly IReadOnlyRepository<UserSchool> repositoryView;
        private readonly IUserSchoolRepository<UserSchool> repository;
        private readonly IUserManageRepository _userManageRepository;
        private readonly IXLIGroupRepository _xliGroupRepo;
        private readonly IXLIGroupUserRepository _xliGroupUserRepository;

        public UserSchoolService(IUserSchoolRepository<UserSchool> repository, IReadOnlyRepository<UserSchool> repositoryView,
            IUserManageRepository userManageRepository,
            IXLIGroupRepository xliGroupRepository,
            IXLIGroupUserRepository xliGroupUserRepository)
        {
            this.repository = repository;
            this.repositoryView = repositoryView;
            _userManageRepository = userManageRepository;
            _xliGroupRepo = xliGroupRepository;
            _xliGroupUserRepository = xliGroupUserRepository;
        }

        public IQueryable<UserSchool> GetAllUsers()
        {
            return repositoryView.Select();
        }

        public IQueryable<UserManage> GetAllUserManage()
        {
            //Only Publisher, District Admin, Shool Admin, Teacher
            return _userManageRepository.Select()
                .Where(o=> o.UserStatusId.HasValue 
                    && (o.UserStatusId.Value ==(int) UserStatus.Active 
                        || o.UserStatusId.Value == (int) UserStatus.Inactive )
                    && (o.RoleId == (int) Permissions.Teacher || o.RoleId == (int) Permissions.SchoolAdmin ||
                    o.RoleId == (int)Permissions.DistrictAdmin || o.RoleId == (int)Permissions.Publisher || o.RoleId == (int)Permissions.NetworkAdmin));
        }

        public IQueryable<UserSchool> GetSchoolsUserHasAccessTo(int userId)
        {
            return repository.Select().Where(x => x.UserId.Equals(userId) && x.UserSchoolId != null);
        }

        public IQueryable<UserSchool> GetSchoolsUserByUserlId(int userId)
        {
            return repository.SelectFromSchoolManagementView().Where(x => x.UserId == userId);
        }
        public IQueryable<UserSchool> GetUsersBasedOnPermissions(User currentUser)
        {
            var permissionLevel = (Permissions) currentUser.RoleId;
            if (currentUser.IsDistrictAdminOrPublisher)
            {
                var userSchools = repositoryView.Select().Where(x => x.DistrictId.Equals(currentUser.DistrictId));
                return FilterUsersByPermissionLevel(userSchools, permissionLevel);
            }

            var schoolIdList = AssignSchoolIdList(currentUser).Distinct();
            var endUsers = repositoryView.Select().Where(x => schoolIdList.Contains(x.SchoolId.GetValueOrDefault()));
            return FilterUsersByPermissionLevel(endUsers, permissionLevel);
        }

        public List<int> GetListUserBySchoolId(int schoolID)
        {
            return repository.Select().Where(m => m.SchoolId == schoolID).Select(m => m.UserId).ToList();
        }

        public IQueryable<UserManage> GetUserManageBasedOnPermissions(User currentUser, IList<int> listDistrictId)
        {
            var permissionLevel = (Permissions)currentUser.RoleId;
            if (currentUser.IsDistrictAdmin)
            {
                var userManages = _userManageRepository.Select().Where(x => x.DistrictId.Equals(currentUser.DistrictId));
                return FilterUserManageByPermissionLevel(userManages, permissionLevel);
            }
            if (currentUser.IsNetworkAdmin)
            {
                var userManages = _userManageRepository.Select().Where(x => listDistrictId.Contains(x.DistrictId.Value));
                return FilterUserManageByPermissionLevel(userManages, permissionLevel);
            }

            var schoolIdList = AssignSchoolIdList(currentUser).Distinct();
            var endUsers = _userManageRepository.Select().ToList().Where(x => HasAccessToSchool(schoolIdList, x.SchoolIdList)).AsQueryable();
            return FilterUserManageByPermissionLevel(endUsers, permissionLevel);
        }

        private bool HasAccessToSchool(IEnumerable<int> currentUserAccessSchool, List<string> userAccessSchool)
        {
            return currentUserAccessSchool.Any(x => userAccessSchool.Contains(x.ToString()));
        }

        public bool CanAccessUser(User currentUser, User editingUser)
        {
            return GetUsersBasedOnPermissions(currentUser).Any(x => x.UserId.Equals(editingUser.Id));
        }

        public UserSchool VerifyUserSchoolExists(int userSchoolId, int userId, int schoolId)
        {
            return repository.Select().FirstOrDefault(x => x.UserSchoolId.Equals(userSchoolId) && x.UserId.Equals(userId) && x.SchoolId.Equals(schoolId));
        }

        private IQueryable<UserSchool> FilterUsersByPermissionLevel(IQueryable<UserSchool> users, Permissions permissionLevel)
        {
            switch (permissionLevel)
            {
                case Permissions.SchoolAdmin:
                    return users.Where(x => x.Role.Equals(Permissions.Teacher) || x.Role.Equals(Permissions.SchoolAdmin));
                case Permissions.DistrictAdmin:
                    return users.Where(x => x.Role != Permissions.Publisher && x.Role != Permissions.LinkItAdmin);
                case Permissions.Publisher:
                    return users.Where(x => x.Role != Permissions.LinkItAdmin);
                case Permissions.LinkItAdmin:
                    return users;
                default:
                    return new List<UserSchool>().AsQueryable();
            }
        }

        private IQueryable<UserManage> FilterUserManageByPermissionLevel(IQueryable<UserManage> users, Permissions permissionLevel)
        {
            switch (permissionLevel)
            {
                case Permissions.SchoolAdmin:
                    return users.Where(x => x.RoleId == (int)Permissions.Teacher || x.RoleId == (int)Permissions.SchoolAdmin);
                case Permissions.DistrictAdmin:
                    return users.Where(x => x.RoleId != (int)Permissions.Publisher && x.RoleId != (int)Permissions.LinkItAdmin);
                case Permissions.Publisher:
                    return users.Where(x => x.RoleId != (int)Permissions.LinkItAdmin);
                case Permissions.NetworkAdmin:
                    return users.Where(x => x.RoleId != (int)Permissions.Publisher && x.RoleId != (int)Permissions.LinkItAdmin);
                case Permissions.LinkItAdmin:
                    return users;
                default:
                    return new List<UserManage>().AsQueryable();
            }
        }

        private IEnumerable<int> AssignSchoolIdList(User currentUser)
        {
            var schoolIdList = new List<int>();
            var schools = GetSchoolsUserHasAccessTo(currentUser.Id).Select(userSchool => userSchool.SchoolId.GetValueOrDefault());
            schoolIdList.AddRange(schools);

            return schoolIdList;
        }

        public void InsertUserSchool(UserSchool userSchool)
        {
            repository.Save(userSchool);
        }

        public void RemoveUserSchool(UserSchool userSchool)
        {
            repository.Delete(userSchool);
        }

        public IQueryable<UserSchool> GetSchoolsUserBySchoolId(int schoolId)
        {
            return repository.SelectFromSchoolManagementView().Where(x => x.SchoolId.Equals(schoolId) && x.UserSchoolId != null);
        }

        public IEnumerable<int> ListUserIdBySchoolId(int schoolId)
        {
            return repository.SelectFromSchoolManagementView().Where(o => o.SchoolId == schoolId).Select(userSchool => userSchool.UserId).ToList();
        }

        public UserSchool GetUserSchoolById(int userSchoolId)
        {
            return repository.SelectFromSchoolManagementView().FirstOrDefault(o => o.UserSchoolId == userSchoolId);
        }

        public bool CanAccessSchoolByAdminSchool(int schoolId, User u)
        {
            if (schoolId > 0 && u.IsNotNull() && u.RoleId.Equals((int)Permissions.SchoolAdmin))
            {
                return repository.SelectFromSchoolManagementView().Any(o => o.SchoolId == schoolId && o.UserId == u.Id);
            }

            return false;
        }

        public UserSchool GetUserSchoolByUserIdSchoolId(int userId, int schoolId)
        {
            return repository.Select().FirstOrDefault(o => o.UserId.Equals(userId) && o.SchoolId.Equals(schoolId));
        }

        public List<int> GetListUserBySchoolAdminId(int userId)
        {
            var query = repository.Select().Where(x => x.UserId.Equals(userId) && x.UserSchoolId != null);
            if (query.Any())
            {
                List<int> lstSchoolId = query.Select(o => o.SchoolId.GetValueOrDefault()).ToList();
                var vUserschool = repository.Select().Where(o => lstSchoolId.Contains(o.SchoolId.GetValueOrDefault()));
                if (vUserschool.Any())
                {
                    return vUserschool.Select(o => o.UserId).ToList();
                }
            }
            return  new List<int>();
        }
         public List<int> GetListSchoolIdByUserId(int userId)
        {
            var query = repository.Select().Where(o => o.UserId == userId);
            if (query.Any())
            {
                return query.Select(o => o.SchoolId.GetValueOrDefault()).ToList();
            }
            return new List<int>();
        }

        /// <summary>
        /// Get All User By Role [Use StoreProcedure]
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="districtId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public IQueryable<UserManage> GetManageUserByRole(int userId, int districtId, int roleId, int schoolID, string staffName, bool inactive)
        {
            return _userManageRepository.GetManageUsersByRole(userId, districtId, roleId, schoolID, staffName, inactive);
        }

        public GetGroupUserResponse GetUserManageByRoleInGroup(GetGroupUserRequest request)
        {
            return _userManageRepository.GetUserManageByRoleInGroup(request);
        }

        public GetUserForAddGroupResponse GetUserAvailableForAddGroup(GetUserForAddGroupRequest request)
        {
            return _userManageRepository.GetUserAvailableForAddGroup(request);
        }

        public List<ListItem> GetRolesForManageUserGroup(int userRoleID, int groupID)
        {
            var result  = new List<ListItem>();
            var roles = _userManageRepository.GetRolesForManageUserGroup(userRoleID, groupID);
            foreach (var item in roles)
            {
                result.Add(new ListItem()
                {
                    Id = item.RoleID.Value,
                    Name = item.RoleName
                });
            }

            return result;
        }
        
        public IQueryable<UserSchool> GetTeachersBySchoolId(int schoolId)
        {
            return repository.SelectFromTeacherListView().Where(x => x.SchoolId.Equals(schoolId));
        }

        public IQueryable<UserSchool> GetTeacherBySchooolIdProc(int schoolId, int userId, int roleId, int districtId)
        {
            return repository.GetTeacherBySchooolIdProc(schoolId, userId, roleId, districtId);
        }

        public IQueryable<UserSchool> GetTeacherSchoolByTermProc(int schoolId, bool isTeacherHasTerm, int userId, int roleId, string validUserSchoolRoleId, bool isIncludeDistrictAdmin = false)
        {
            return repository.GetTeacherSchoolByTermProc(schoolId, isTeacherHasTerm, userId, roleId, validUserSchoolRoleId, isIncludeDistrictAdmin);
        }
    }
}
