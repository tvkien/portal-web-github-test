using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryTeacherDistrictTermRepository : IReadOnlyRepository<TeacherDistrictTerm>
    {
        private readonly List<TeacherDistrictTerm> table;

        public InMemoryTeacherDistrictTermRepository()
        {
            table = AddTeacherDisrictTerms();
        }

        private List<TeacherDistrictTerm> AddTeacherDisrictTerms()
        {
            return new List<TeacherDistrictTerm>
                       {
                           new TeacherDistrictTerm { UserId = 1, SchoolId = 1, DistrictTermId = 1, DistrictName = "District1" },
                           new TeacherDistrictTerm { UserId = 1, SchoolId = 1, DistrictTermId = 2, DistrictName = "District2" },
                           new TeacherDistrictTerm { UserId = 1, SchoolId = 1, DistrictTermId = 3, DistrictName = "District3" },
                           new TeacherDistrictTerm { UserId = 2, SchoolId = 2, DistrictTermId = 4, DistrictName = "District4" },
                           new TeacherDistrictTerm { UserId = 3, SchoolId = 2, DistrictTermId = 5, DistrictName = "District5" },
                       };
        }

        public IQueryable<TeacherDistrictTerm> Select()
        {
            return table.AsQueryable();
        }
    }
}
