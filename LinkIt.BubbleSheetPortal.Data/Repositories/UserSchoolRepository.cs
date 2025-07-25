using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Data.Extensions;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class UserSchoolRepository : IUserSchoolRepository<UserSchool>
    {
        private readonly Table<UserSchoolEntity> userSchoolEntity;
        private readonly Table<UserContext_SchoolManagementView> schoolManagementView;
        private readonly Table<TeacherListView> teacherListView;

        private UserDataContext dataContext;
        private DbDataContext dbContext;

        public UserSchoolRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            dataContext = UserDataContext.Get(connectionString);
            dbContext = DbDataContext.Get(connectionString);
            userSchoolEntity = dataContext.GetTable<UserSchoolEntity>();
            schoolManagementView = dataContext.GetTable<UserContext_SchoolManagementView>();
            teacherListView = dataContext.GetTable<TeacherListView>();
        }

        public IQueryable<UserSchool> Select()
        {
            return userSchoolEntity.Select(x => new UserSchool
                {
                    UserSchoolId = x.UserSchoolID,
                    UserId = x.UserID,
                    SchoolId = x.SchoolID,
                    SchoolName = x.SchoolEntity.Name
                });
        }

        public IQueryable<UserSchool> SelectFromSchoolManagementView()
        {
            return schoolManagementView.Select(x => new UserSchool
                {
                    UserSchoolId = x.UserSchoolID,
                    UserId = x.UserID,
                    UserName = x.UserName,
                    Role = (Permissions) x.RoleID,
                    RoleName = x.RoleName,
                    UserStatusId = x.UserStatusID ?? (int)UserStatus.Inactive,
                    SchoolId = x.SchoolID,
                    DistrictId = x.DistrictID,
                    FirstName = x.NameFirst,
                    LastName = x.NameLast,
                    SchoolName = x.SchoolName,
                    DistrictName = x.DistrictName,
                    StateName = x.StateName
                });
        }

        public void Save(UserSchool item)
        {
            var entity = userSchoolEntity.FirstOrDefault(x => x.UserSchoolID.Equals(item.UserSchoolId));

            if (entity.IsNull())
            {
                entity = new UserSchoolEntity();
                userSchoolEntity.InsertOnSubmit(entity);
            }
            BindUserSchoolEntityToUserSchoolItem(entity, item);
            userSchoolEntity.Context.SubmitChanges();
            item.UserSchoolId = entity.UserSchoolID;
        }

        public void Delete(UserSchool item)
        {
            var entity = userSchoolEntity.FirstOrDefault(x => x.UserSchoolID.Equals(item.UserSchoolId));
            if (entity.IsNotNull())
            {
                userSchoolEntity.DeleteOnSubmit(entity);
                userSchoolEntity.Context.SubmitChanges();
            }
        }

        private void BindUserSchoolEntityToUserSchoolItem(UserSchoolEntity entity, UserSchool item)
        {
            entity.SchoolID = item.SchoolId.GetValueOrDefault();
            entity.UserID = item.UserId;
            entity.DateActive = item.DateActive;
        }

        public IQueryable<UserSchool> SelectFromTeacherListView()
        {
            return teacherListView.Select(x => new UserSchool
            {
                UserId = x.UserID.Value,
                UserName = x.UserName,
                Role = (Permissions)x.RoleID,
                UserStatusId = x.UserStatusID ?? (int)UserStatus.Inactive,
                SchoolId = x.SchoolID,
                FirstName = x.FirstName,
                LastName = x.LastName
            });
        }

        public IQueryable<UserSchool> GetTeacherBySchooolIdProc(int schoolId, int userId, int roleId, int districtId)
        {
            var data = dataContext.GetTeacherForAddClass(schoolId, userId, roleId, districtId).Select(x => new UserSchool
            {
                UserId = x.UserId,
                FirstName = x.FirstName,
                LastName = x.LastName,
                UserName = x.UserName
            });
            return data.AsQueryable();
        }

        public IQueryable<UserSchool> GetTeacherSchoolByTermProc(int schoolId, bool isTeacherHasTerm, int userId, int roleId,string validUserSchoolRoleId, bool isIncludeDistrictAdmin = false)
        {
            var data = dataContext.GetTeacherSchoolByTerm(schoolId, isTeacherHasTerm, userId, roleId, validUserSchoolRoleId, isIncludeDistrictAdmin).Select(x => new UserSchool
            {
                UserId = x.UserID,
                UserName = x.UserName,
                FirstName = x.NameFirst,
                LastName = x.NameLast
            });
            return data.AsQueryable();
        }
    }
}
