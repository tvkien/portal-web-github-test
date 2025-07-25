using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryDistrictRepository : IRepository<District>
    {
        private readonly List<District> table = new List<District>();
        private static int nextUniqueID = 1;

        public InMemoryDistrictRepository()
        {
            table = AddDistricts();
        }

        private List<District> AddDistricts()
        {
            return new List<District>
                       {
                           new District { Id = 1, Name = "District 1", StateId = 4, LICode = "DisOne" },
                           new District { Id = 2, Name = "District 2", StateId = 2, LICode = "DisTwo" },
                           new District { Id = 3, Name = "District 3", StateId = 1, LICode = "DisThree" },
                           new District { Id = 4, Name = "District 4", StateId = 5, LICode = "DisFour" },
                           new District { Id = 5, Name = "District 5", StateId = 7, LICode = "DisFive" },
                           new District { Id = 6, Name = "District 6", StateId = 4 },
                           new District { Id = 8, Name = "Search 1", StateId = 3 },
                           new District { Id = 9, Name = "Search 2", StateId = 3 },
                           new District { Id = 10, Name = "Search 3", StateId = 3 },
                           new District { Id = 11, Name = "Search 4", StateId = 3 },
                           new District { Id = 12, Name = "Search 5", StateId = 3 },
                           new District { Id = 13, Name = "Search 6", StateId = 3 },
                           new District { Id = 14, Name = "Search 7", StateId = 3 },
                           new District { Id = 15, Name = "Search 8", StateId = 3 },
                           new District { Id = 16, Name = "Search 9", StateId = 3 }
                       };
        }

        public IQueryable<District> Select()
        {
            return table.AsQueryable();
        }

        public void Save(District item)
        {
            var entity = table.FirstOrDefault(x => x.Id.Equals(item.Id));

            if (entity.IsNull())
            {
                item.Id = nextUniqueID;
                nextUniqueID++;
                table.Add(item);
            }
            else
            {
                Mapper.CreateMap<District, District>();
                Mapper.Map(item, entity);
            }
        }

        public void Delete(District item)
        {
            var entity = table.FirstOrDefault(x => x.Id.Equals(item.Id));
            if (entity.IsNotNull())
            {
                table.Remove(entity);
            }
        }
    }
}
