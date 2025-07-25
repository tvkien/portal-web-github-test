using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemorySchoolTeacherListRepository : IReadOnlyRepository<SchoolTeacherList>
    {
        private List<SchoolTeacherList> table;

        public InMemorySchoolTeacherListRepository()
        {
            table = AddSchoolTeacherList();
        }

        private List<SchoolTeacherList> AddSchoolTeacherList()
        {
            return new List<SchoolTeacherList>
                       {
                           new SchoolTeacherList{ Active = true, ClassID = "1", ClassName = "Home Room", FirstName = "Bob", LastName = "Arneet", RoleId = 2, SchoolID = 1, UserID = 1, UserName = "barnett"},
                           new SchoolTeacherList{ Active = true, ClassID = "2", ClassName = "Reading", FirstName = "Jiilian", LastName = "Smith", RoleId = 2, SchoolID = 2, UserID = 3, UserName = "jsmith"},
                           new SchoolTeacherList{ Active = true, ClassID = "3", ClassName = "Language", FirstName = "Terry", LastName = "Passman", RoleId = 2, SchoolID = 1, UserID = 5, UserName = "tpassman"}
                       };
        }

        public IQueryable<SchoolTeacherList> Select()
        {
            return table.AsQueryable();
        }
    }
}