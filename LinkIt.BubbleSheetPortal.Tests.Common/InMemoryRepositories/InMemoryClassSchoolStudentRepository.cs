using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryClassSchoolStudentRepository : IReadOnlyRepository<ClassSchoolStudent>
    {
        private List<ClassSchoolStudent> table;

        public InMemoryClassSchoolStudentRepository()
        {
            table = AddClassStudents();
        }

        private List<ClassSchoolStudent> AddClassStudents()
        {
            return new List<ClassSchoolStudent>
                       {
                           new ClassSchoolStudent{ ClassID = 1, StudentID = 10, FirstName = "Student", LastName = "One", SchoolID = 1},
                           new ClassSchoolStudent{ ClassID = 1, StudentID = 12, FirstName = "Student", LastName = "Two", SchoolID = 1},
                           new ClassSchoolStudent{ ClassID = 6, StudentID = 15, FirstName = "Jeremy", LastName = "Allan", SchoolID = 2},
                       };
        }

        public IQueryable<ClassSchoolStudent> Select()
        {
            return table.AsQueryable();
        }
    }
}