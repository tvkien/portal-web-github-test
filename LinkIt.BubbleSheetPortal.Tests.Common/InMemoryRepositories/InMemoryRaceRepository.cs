using System.Collections.Generic;
using System.Linq;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Data;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryRaceRepository: IRepository<Race>
    {
        private readonly List<Race> table;

        public InMemoryRaceRepository()
        {
            table = AddRaces();
        }

        private List<Race> AddRaces()
        {
            return new List<Race>()
            {
                new Race{ Id = 1, Name = "Other", DistrictID = 1, Code = "O", AltCode = "AltCode1" },
                new Race{ Id = 2, Name = "W", DistrictID = 1, Code = "W", AltCode = "AltCode2" },
                new Race{ Id = 3, Name = "H", DistrictID = 1, Code = "H", AltCode = "AltCode3" },
                new Race{ Id = 4, Name = "A", DistrictID = 1, Code = "A", AltCode = "AltCode4" },
                new Race{ Id = 5, Name = "B", DistrictID = 1, Code = "B", AltCode = "AltCode5" }
            };
        }

        public IQueryable<Race> Select()
        {
            return table.AsQueryable();
        }

        public void Save(Race item)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(Race item)
        {
            throw new System.NotImplementedException();
        }
    }
}
