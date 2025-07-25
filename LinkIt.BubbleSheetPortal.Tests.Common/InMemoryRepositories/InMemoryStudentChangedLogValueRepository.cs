using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryStudentChangedLogValueRepository : IRepository<StudentChangedLogValue>
    {
        private List<StudentChangedLogValue> table;
        private static int nextUniqueID = 1;

        public InMemoryStudentChangedLogValueRepository()
        {
            table = AddStudentChangedLogValue();
        }

        private List<StudentChangedLogValue> AddStudentChangedLogValue()
        {
            return new List<StudentChangedLogValue>
                       {
                           new StudentChangedLogValue{ LogID = 1, LogValueID = 1, NewValue = "Val New", OldValue = "Val New", ValueChanged = "Some Val"}
                       };
        }

        public IQueryable<StudentChangedLogValue> Select()
        {
            return table.AsQueryable();
        }

        public void Save(StudentChangedLogValue item)
        {
            var entity = table.FirstOrDefault(x => x.LogValueID.Equals(item.LogValueID));

            if (entity.IsNull())
            {
                item.LogValueID = nextUniqueID;
                nextUniqueID++;
                table.Add(item);
            }
            else
            {
                Mapper.CreateMap<StudentChangedLogValue, StudentChangedLogValue>();
                Mapper.Map(item, entity);
            }
        }

        public void Delete(StudentChangedLogValue item)
        {
            var entity = table.FirstOrDefault(x => x.LogValueID.Equals(item.LogValueID));
            if (entity.IsNotNull())
            {
                table.Remove(entity);
            }
        }
    }
}