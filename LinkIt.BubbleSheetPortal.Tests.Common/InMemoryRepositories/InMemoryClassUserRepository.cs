using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryClassUserRepository : IRepository<ClassUser>
    {
        private readonly List<ClassUser> table = new List<ClassUser>();
        private static int nextUniqueID = 1;

        public InMemoryClassUserRepository()
        {
            table = AddClassUsers();
        }

        private List<ClassUser> AddClassUsers()
        {
            return new List<ClassUser>
                       {
                           new ClassUser {Id = 1, ClassId = 1, UserId = 1, ClassUserLOEId = 1},
                           new ClassUser {Id = 2, ClassId = 1, UserId = 2, ClassUserLOEId = 2},
                       };
        }

        public IQueryable<ClassUser> Select()
        {
            return table.AsQueryable();
        }

        public void Save(ClassUser item)
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
                Mapper.CreateMap<ClassUser, ClassUser>();
                Mapper.Map(item, entity);
            }
        }

        public void Delete(ClassUser item)
        {
            var entity = table.FirstOrDefault(x => x.Id.Equals(item.Id));
            if (entity.IsNotNull())
            {
                table.Remove(entity);
            }
        }
    }
}