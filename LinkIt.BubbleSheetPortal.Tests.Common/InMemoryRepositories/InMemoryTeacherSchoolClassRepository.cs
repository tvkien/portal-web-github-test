using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryTeacherSchoolClassRepository : IReadOnlyRepository<TeacherSchoolClass>
    {
        private List<TeacherSchoolClass> table;

        public InMemoryTeacherSchoolClassRepository()
        {
            table = AddTeacherSchoolClass();
        }

        private List<TeacherSchoolClass> AddTeacherSchoolClass()
        {
            return new List<TeacherSchoolClass>
                       {
                           new TeacherSchoolClass{ ClassId = 1, SchoolId = 2, TeacherFirstName = "Mrs.", TeacherLastName = "TeacherLady", UserId = 3}
                       };
        }

        public IQueryable<TeacherSchoolClass> Select()
        {
            return table.AsQueryable();
        }
    }
}