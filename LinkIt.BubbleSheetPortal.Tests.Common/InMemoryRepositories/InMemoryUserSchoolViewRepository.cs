using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryUserSchoolViewRepository : IReadOnlyRepository<UserSchool>
    {
        private readonly List<UserSchool> table;

        public InMemoryUserSchoolViewRepository()
        {
            table = AddUserSchools();
        }

        private List<UserSchool> AddUserSchools()
        {
            return new List<UserSchool>
                       {
                           new UserSchool{UserSchoolId= 1, UserStatusId = 3, DistrictId = 1, UserName = "user2", UserId = 2, SchoolId = 123, Role = Permissions.SchoolAdmin },
                           new UserSchool{UserSchoolId= 2, UserStatusId = 1, DistrictId = 1, UserName = "user3", UserId = 3, SchoolId = 123, Role = Permissions.Teacher },
                           new UserSchool{UserSchoolId= 3, UserStatusId = 1, DistrictId = 1, UserName = "user4", UserId = 4, SchoolId = 123, Role = Permissions.DistrictAdmin },
                           new UserSchool{UserSchoolId= 4, UserStatusId = 3, DistrictId = 1, UserName = "user5", UserId = 5, SchoolId = 123, Role = Permissions.LinkItAdmin },
                           new UserSchool{UserSchoolId= 5, UserStatusId = 1, DistrictId = 1, UserName = "user6", UserId = 6, SchoolId = 123, Role = Permissions.SchoolAdmin },
                           new UserSchool{UserSchoolId= 6, UserStatusId = 1, DistrictId = 1, UserName = "user7", UserId = 7, SchoolId = 123, Role = Permissions.DistrictAdmin },
                           new UserSchool{UserSchoolId= 7, UserStatusId = 1, DistrictId = 1, UserName = "user8", UserId = 8, SchoolId = 123, Role = Permissions.LinkItAdmin },
                           new UserSchool{UserSchoolId= 8, UserStatusId = 1, DistrictId = 1, UserName = "user9", UserId = 9, SchoolId = 123, Role = Permissions.Publisher },
                           new UserSchool{UserSchoolId= 11, UserStatusId = 1, DistrictId = 1, UserName = "user12", UserId = 10, SchoolId = 123, SchoolName = "School3", Role = Permissions.Teacher },
                           new UserSchool{UserSchoolId= 13, UserStatusId = 1, DistrictId = 4, UserName = "user14", UserId = 14, SchoolId = 123, Role = Permissions.Publisher },
                           new UserSchool{UserSchoolId= 14, UserStatusId = 3, DistrictId = 4, UserName = "user15", UserId = 15, SchoolId = 123, Role = Permissions.DistrictAdmin },
                           new UserSchool{UserSchoolId= 15, UserStatusId = 1, DistrictId = 4, UserName = "user16", UserId = 16, SchoolId = 123, Role = Permissions.SchoolAdmin },
                           new UserSchool{UserSchoolId= 16, UserStatusId = 1, DistrictId = 4, UserName = "user17", UserId = 17, SchoolId = 123, Role = Permissions.Teacher },
                           new UserSchool{UserSchoolId= 17, UserStatusId = 3, DistrictId = 4, UserName = "user18", UserId = 18, SchoolId = 123, Role = Permissions.SchoolAdmin }
                       };
        }

        public IQueryable<UserSchool> Select()
        {
            return table.AsQueryable();
        }
    }
}