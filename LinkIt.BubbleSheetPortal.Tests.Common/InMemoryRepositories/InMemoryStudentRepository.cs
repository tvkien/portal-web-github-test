using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryStudentRepository : IRepository<Student>
    {
        private readonly List<Student> table;
        private static int nextUniqueID = 1;

        public InMemoryStudentRepository()
        {
            table = AddStudentData();
        }

        private List<Student> AddStudentData()
        {
            return new List<Student>
                       {
                           new Student{ AltCode = "A", Id = 1, FirstName = "Jim", LastName = "James" },
                           new Student{ AltCode = "A", Id = 1, FirstName = "Maria", LastName = "Lazarus" },
                           new Student{ AltCode = "A", Id = 1, FirstName = "Sheryl", LastName = "Abby" }
                       };
        }

        public IQueryable<Student> Select()
        {
            return table.AsQueryable();
        }

        public void Save(Student item)
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
                Mapper.CreateMap<Student, Student>();
                Mapper.Map(item, entity);
            }
        }

        public void Delete(Student item)
        {
            var entity = table.FirstOrDefault(x => x.Id.Equals(item.Id));
            if (entity.IsNotNull())
            {
                table.Remove(entity);
            }
        }
    }
}