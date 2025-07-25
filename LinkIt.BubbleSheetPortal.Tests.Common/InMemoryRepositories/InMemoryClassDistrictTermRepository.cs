using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryClassDistrictTermRepository : IReadOnlyRepository<ClassDistrictTerm>
    {
        private readonly List<ClassDistrictTerm> table;

        public InMemoryClassDistrictTermRepository()
        {
            table = AddClassDistrictTerms();
        }

        private List<ClassDistrictTerm> AddClassDistrictTerms()
        {
            return new List<ClassDistrictTerm>
                       {
                           new ClassDistrictTerm { ClassId = 1, SchoolId = 1},
                           new ClassDistrictTerm { ClassId = 2, SchoolId = 1},
                       };
        }

        public IQueryable<ClassDistrictTerm> Select()
        {
            return table.AsQueryable();
        }
    }
}
