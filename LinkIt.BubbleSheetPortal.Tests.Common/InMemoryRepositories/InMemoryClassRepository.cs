using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryClassRepository : IRepository<Class>
    {
        private readonly List<Class> table = new List<Class>();
        private static int nextUniqueID = 1;

        public InMemoryClassRepository()
        {
            table = AddClasses();
        }

        private List<Class> AddClasses()
        {
            return new List<Class>
                    {
                        new Class{Id = 1, Name = "Class1", SchoolId = 1, TermId = 1, DistrictTermId = 1, UserId = 1},
                        new Class{Id = 2, Name = "Class2", SchoolId = 2, TermId = 2, DistrictTermId = 1, UserId = 1},
                        new Class{Id = 3, Name = "Class3", SchoolId = 3, TermId = 3, DistrictTermId = 3, UserId = 3},
                        new Class{Id = 4, Name = "Class4", SchoolId = 4, TermId = 4, DistrictTermId = 4, UserId = 4},
                    };
        }

        public IQueryable<Class> Select()
        {
            return table.AsQueryable();
        }

        public void Save(Class item)
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
                Mapper.CreateMap<Class, Class>();
                Mapper.Map(item, entity);
            }
        }

        public void Delete(Class item)
        {
            var entity = table.FirstOrDefault(x => x.Id.Equals(item.Id));
            if (entity.IsNotNull())
            {
                table.Remove(entity);
            }
        }
    }
}
