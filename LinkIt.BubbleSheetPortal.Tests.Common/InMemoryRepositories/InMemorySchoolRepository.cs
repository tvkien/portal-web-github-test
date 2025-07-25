using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemorySchoolRepository : IRepository<School>
    {
        private readonly List<School> table;
        private static int nextUniqueID = 1;

        public InMemorySchoolRepository()
        {
            table = AddSchools();
        }

        private List<School> AddSchools()
        {
            return new List<School>
                {
                    new School{Id = 123, DistrictId = 1, Name = "School1", Code = "code", StateCode = "code"},
                    new School{Id = 122, DistrictId = 2, Name = "School2"},
                    new School{Id = 124, DistrictId = 4, Name = "School3"},
                    new School{Id = 12, DistrictId = 4, Name = "School4"},
                };
        }

        public IQueryable<School> Select()
        {
            return table.AsQueryable();
        }

        public void Save(School item)
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
                Mapper.CreateMap<School, School>();
                Mapper.Map(item, entity);
            }
        }

        public void Delete(School item)
        {
            var entity = table.FirstOrDefault(x => x.Id.Equals(item.Id));
            if (entity.IsNotNull())
            {
                table.Remove(entity);
            }
        }
    }
}