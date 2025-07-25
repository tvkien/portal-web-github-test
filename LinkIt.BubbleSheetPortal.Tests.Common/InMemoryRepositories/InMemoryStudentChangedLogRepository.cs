using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryStudentChangedLogRepository : IRepository<StudentChangedLog>
    {
        private List<StudentChangedLog> table;
        private static int nextUniqueID = 1;

        public InMemoryStudentChangedLogRepository()
        {
            table = AddStudentChangedLog();
        }

        private List<StudentChangedLog> AddStudentChangedLog()
        {
            return new List<StudentChangedLog>
                       {
                           new StudentChangedLog{ LogID = 1, StudentIDChanged = 1, UpdatedBy = 5},
                           new StudentChangedLog{ LogID = 2, StudentIDChanged = 3, UpdatedBy = 5}
                       };
        }

        public IQueryable<StudentChangedLog> Select()
        {
            return table.AsQueryable();
        }

        public void Save(StudentChangedLog item)
        {
            var entity = table.FirstOrDefault(x => x.LogID.Equals(item.LogID));

            if (entity.IsNull())
            {
                item.LogID = nextUniqueID;
                nextUniqueID++;
                table.Add(item);
            }
            else
            {
                Mapper.CreateMap<Student, Student>();
                Mapper.Map(item, entity);
            }
        }

        public void Delete(StudentChangedLog item)
        {
            var entity = table.FirstOrDefault(x => x.LogID.Equals(item.LogID));
            if (entity.IsNotNull())
            {
                table.Remove(entity);
            }
        }

    }
}