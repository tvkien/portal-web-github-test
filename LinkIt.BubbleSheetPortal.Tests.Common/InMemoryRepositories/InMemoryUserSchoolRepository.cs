using System.Collections.Generic;
using System.Linq;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryUserSchoolRepository : IUserSchoolRepository<UserSchool>
    {
        private readonly List<UserSchool> table;

        public InMemoryUserSchoolRepository()
        {               
            table = AddUserSchools();
        }

        private List<UserSchool> AddUserSchools()
        {
            return new List<UserSchool>
                       {
                           new UserSchool{UserSchoolId= 1, UserStatusId = 3, DistrictId = 1, UserName = "user2", UserId = 1, SchoolId = 3, Role = Permissions.SchoolAdmin },
                           new UserSchool{UserSchoolId= 2, UserStatusId = 1, DistrictId = 1, UserName = "user3", UserId = 1, SchoolId = 2, Role = Permissions.Teacher },
                           new UserSchool{UserSchoolId= 3, UserStatusId = 1, DistrictId = 1, UserName = "user4", UserId = 1, SchoolId = 1, Role = Permissions.DistrictAdmin },
                           new UserSchool{UserSchoolId= 4, UserStatusId = 3, DistrictId = 1, UserName = "user5", UserId = 5, SchoolId = 123, Role = Permissions.LinkItAdmin },
                           new UserSchool{UserSchoolId= 5, UserStatusId = 1, DistrictId = 1, UserName = "user6", UserId = 6, SchoolId = 123, Role = Permissions.SchoolAdmin },
                           new UserSchool{UserSchoolId= 6, UserStatusId = 1, DistrictId = 1, UserName = "user7", UserId = 7, SchoolId = 123, Role = Permissions.DistrictAdmin },
                           new UserSchool{UserSchoolId= 7, UserStatusId = 1, DistrictId = 1, UserName = "user8", UserId = 8, SchoolId = 123, Role = Permissions.LinkItAdmin },
                           new UserSchool{UserSchoolId= 8, UserStatusId = 1, DistrictId = 1, UserName = "user9", UserId = 9, SchoolId = 123, Role = Permissions.Publisher },
                           new UserSchool{UserSchoolId= 9, UserStatusId = 3, DistrictId = 1, UserName = "user10", UserId = 10, SchoolName = "School1", SchoolId = 123, Role = Permissions.Publisher },
                           new UserSchool{UserSchoolId= 10, UserStatusId = 1, DistrictId = 1, UserName = "user11", UserId = 10, SchoolName = "School2", SchoolId = 123, Role = Permissions.SchoolAdmin },
                           new UserSchool{UserSchoolId= 11, UserStatusId = 1, DistrictId = 1, UserName = "user12", UserId = 10, SchoolName = "School3", SchoolId = 123, Role = Permissions.Teacher },
                           new UserSchool{UserSchoolId= 12, UserStatusId = 1, DistrictId = 1, UserName = "user13", UserId = 10, SchoolName = "School4", SchoolId = 123, Role = Permissions.LinkItAdmin },
                           new UserSchool{UserSchoolId= 13, UserStatusId = 1, DistrictId = 1, UserName = "user14", UserId = 14, SchoolId = 123, Role = Permissions.Publisher },
                           new UserSchool{UserSchoolId= 14, UserStatusId = 3, DistrictId = 1, UserName = "user15", UserId = 15, SchoolId = 123, Role = Permissions.DistrictAdmin },
                           new UserSchool{UserSchoolId= 15, UserStatusId = 1, DistrictId = 1, UserName = "user16", UserId = 16, SchoolId = 123, Role = Permissions.SchoolAdmin },
                           new UserSchool{UserSchoolId= 16, UserStatusId = 1, DistrictId = 1, UserName = "user17", UserId = 17, SchoolId = 123, Role = Permissions.Teacher },
                           new UserSchool{UserSchoolId= 17, UserStatusId = 3, DistrictId = 1, UserName = "user18", UserId = 18, SchoolId = 123, Role = Permissions.SchoolAdmin }
                       };
        }

        public IQueryable<UserSchool> Select()
        {
            return table.AsQueryable();
        }

        public void Save(UserSchool us)
        {
            table.Add(us);
        }

        public void Delete(UserSchool us)
        {
            table.Remove(us);
        }

        public IQueryable<UserSchool> SelectFromSchoolManagementView()
        {
            return table.AsQueryable();
        }

        public bool CheckUserCanAccessSchool(int userId, int roleId, int schoolId)
        {
            return true;
        }
    }
}