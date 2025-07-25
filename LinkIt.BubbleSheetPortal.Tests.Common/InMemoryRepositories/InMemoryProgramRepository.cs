using System.Collections.Generic;
using System.Linq;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Data;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryProgramRepository : IReadOnlyRepository<Program>
    {
        private readonly List<Program> table;

        public InMemoryProgramRepository()
        {
            table = AddPrograms();
        }

        private List<Program> AddPrograms()
        {
            return new List<Program>()
            {
                new Program { AccessLevelID = 4, Code = null, DistrictID = 1, Id = 1, Name = "SpecEd: 01" },
                new Program { AccessLevelID = 4, Code = null, DistrictID = 1, Id = 2, Name = "SpecEd: 02" },
                new Program { AccessLevelID = 4, Code = null, DistrictID = 1, Id = 3, Name = "SpecEd: 03" },
                new Program { AccessLevelID = 4, Code = null, DistrictID = 1, Id = 4, Name = "SpecEd: 04" }
            };
        }

        public IQueryable<Program> Select()
        {
            return table.AsQueryable();
        }
    }
}
