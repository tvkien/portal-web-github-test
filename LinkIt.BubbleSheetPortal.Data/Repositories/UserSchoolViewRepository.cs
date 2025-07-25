using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class UserSchoolViewRepository : IReadOnlyRepository<UserSchool>
    {
        private readonly Table<UserSchoolView> table;

        public UserSchoolViewRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            var dataContext = UserDataContext.Get(connectionString);
            table = dataContext.GetTable<UserSchoolView>();
        }

        public IQueryable<UserSchool> Select()
        {
            return table.Select(x => new UserSchool
                {
                    UserSchoolId = x.UserSchoolID,
                    UserId = x.UserID,
                    UserName = x.UserName,
                    Role = (Permissions) x.RoleID,
                    UserStatusId = x.UserStatusID ?? (int)UserStatus.Inactive,
                    DistrictId = x.DistrictID,
                    FirstName = x.NameFirst,
                    LastName = x.NameLast,
                    SchoolId = x.SchoolID,
                    SchoolCount = x.SchoolCount,
                    DistrictName = x.DistrictName,
                    StateName = x.StateName,
                    InActive = x.IsActive.GetValueOrDefault(),
                    RoleName = x.RoleName
                });
        }
    }
}