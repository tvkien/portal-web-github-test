using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryClassStudentRepository : IRepository<ClassStudent>
    {
        public static readonly List<ClassStudent> Data = GetData();

        public static void Clear()
        {
            Data.Clear();
            Data.AddRange(GetData());
        }

        public IQueryable<ClassStudent> Select()
        {
            return Data.AsQueryable();
        }

        public void Save(ClassStudent item)
        {

            var entity = Data.FirstOrDefault(x => x.Id == item.Id);
            if (entity == null)
            {
                item.Id = Data.Max(x => (int?) x.Id).GetValueOrDefault() + 1;
            }
            else
            {
                Data.Remove(entity);
            }
            Data.Add(item);
        }

        public void Delete(ClassStudent item)
        {
            var entity = Data.Single(x => x.Id == item.Id);
            Data.Remove(entity);
        }

        private static List<ClassStudent> GetData()
        {
            return new List<ClassStudent>
            {
                new ClassStudent{ Id=1, ClassId = 1, StudentId = 10, FirstName = "Student", LastName = "One" },
                new ClassStudent{ Id=2, ClassId = 1, StudentId = 11, FirstName = "Student", LastName = "Two" },
                new ClassStudent{ Id=3, ClassId = 2, StudentId = 13, FirstName = "Student", LastName = "Three" },
                new ClassStudent{ Id=4, ClassId = 3, StudentId = 14, FirstName = "Student", LastName = "Four" },
                new ClassStudent{ Id=5, ClassId = 7, StudentId = 15, FirstName = "Student", LastName = "Five" }
            };
        }
    }
}