using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories.TestResultRemover;
using LinkIt.BubbleSheetPortal.Models.TestResultRemover;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories.TestRomover
{
    public class InMemoryClassAdminSchoolRepository : IReadOnlyRepository<ClassAdminSchool>
    {
        private readonly List<ClassAdminSchool> table = new List<ClassAdminSchool>();

        public InMemoryClassAdminSchoolRepository()
        {
            table = AddInMemoryClassAdminSchoolRepository();
        }

        private List<ClassAdminSchool> AddInMemoryClassAdminSchoolRepository()
        {
            return new List<ClassAdminSchool>
                    {                           
                        new ClassAdminSchool{ClassId = 1, ClassName = "Home Room 1", SchoolId = 2, UserId = 3}    ,
                        new ClassAdminSchool{ClassId = 3, ClassName = "Home Room 3", SchoolId = 4, UserId = 5}    ,
                        new ClassAdminSchool{ClassId = 5, ClassName = "Home Room 5", SchoolId = 6, UserId = 7}    ,
                        new ClassAdminSchool{ClassId = 7, ClassName = "Home Room 7", SchoolId = 8, UserId = 9}    ,
                        new ClassAdminSchool{ClassId = 9, ClassName = "Home Room 9", SchoolId = 10, UserId = 11}    ,
                        new ClassAdminSchool{ClassId = 11, ClassName = "Home Room 11", SchoolId = 12, UserId = 13}    ,
                        new ClassAdminSchool{ClassId = 13, ClassName = "Home Room 13", SchoolId = 14, UserId = 15}    ,
                        new ClassAdminSchool{ClassId = 15, ClassName = "Home Room 1", SchoolId = 2, UserId = 3}    ,
                        new ClassAdminSchool{ClassId = 17, ClassName = "Home Room 17", SchoolId = 18, UserId = 19}    ,
                        new ClassAdminSchool{ClassId = 19, ClassName = "Home Room 19", SchoolId = 20, UserId = 21}    
                    };
        }

        public IQueryable<ClassAdminSchool> Select()
        {
            return table.AsQueryable();
        }
    }
}
