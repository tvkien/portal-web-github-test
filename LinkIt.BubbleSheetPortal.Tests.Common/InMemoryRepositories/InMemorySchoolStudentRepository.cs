using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemorySchoolStudentRepository : IReadOnlyRepository<SchoolStudent>
    {
        private readonly List<SchoolStudent> table;

        public InMemorySchoolStudentRepository()
        {
            table = AddClassLists();
        }

        private List<SchoolStudent> AddClassLists()
        {
            return new List<SchoolStudent>
                       {
                           new SchoolStudent{ StudentID = 1, SchoolID = 1, FirstName = "Jim", MiddleName = "Ryan", LastName = "Jones", StateCode = "LA"},
                           new SchoolStudent{ StudentID = 2, SchoolID = 1, FirstName = "Ashley", MiddleName = "Marie", LastName = "Smith", StateCode = "LA"},
                           new SchoolStudent{ StudentID = 3, SchoolID = 1, FirstName = "Jeremy", MiddleName = "Thomas", LastName = "Leblanc", StateCode = "LA"},
                           new SchoolStudent{ StudentID = 7, SchoolID = 2, FirstName = "Shaun", MiddleName = "Ray", LastName = "Thompson", StateCode = "LA"},
                       };
        }

        public IQueryable<SchoolStudent> Select()
        {
            return table.AsQueryable();
        }
    }
}
