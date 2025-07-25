using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemorySchoolTeacherRepository :IReadOnlyRepository<SchoolTeacher>
    {
        private List<SchoolTeacher> table;

        public InMemorySchoolTeacherRepository()
        {
            table = AddSchoolTeachers();
        }

        private List<SchoolTeacher> AddSchoolTeachers()
        {
            return new List<SchoolTeacher>
                       {
                           new SchoolTeacher {FirstName = "Chase", LastName = "Pierce", SchoolId = 1, TeacherName = "Mark", UserId = 1 },
                           new SchoolTeacher {FirstName = "Kyle", LastName = "Joiner", SchoolId = 1, TeacherName = "Joe", UserId = 1 },
                           new SchoolTeacher {FirstName = "Chase", LastName = "Pierce", SchoolId = 2, TeacherName = "Mark", UserId = 1 }
                       };
        }

        public IQueryable<SchoolTeacher> Select()
        {
            return table.AsQueryable();
        }
    }
}
