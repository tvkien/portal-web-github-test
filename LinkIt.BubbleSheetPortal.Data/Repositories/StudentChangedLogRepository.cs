using System;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using Envoc.Core.Shared.Data;
using System.Data.Linq;
using LinkIt.BubbleSheetPortal.Models;
using AutoMapper;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class StudentChangedLogRepository : IRepository<StudentChangedLog>
    {
        private readonly Table<StudentChangedLogEntity> table;

        public StudentChangedLogRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = StudentDataContext.Get(connectionString).GetTable<StudentChangedLogEntity>();
            Mapper.CreateMap<StudentChangedLog, StudentChangedLogEntity>();
        }

        public IQueryable<StudentChangedLog> Select()
        {
            return table.Select(x => new StudentChangedLog
            {
                LogID = x.LogID,
                StudentIDChanged = x.StudentIDChanged
            });
        }

        public void Save(StudentChangedLog item)
        {
            var entity = table.FirstOrDefault(x => x.LogID == item.LogID);

            if (entity.IsNull())
            {
                entity = new StudentChangedLogEntity();
                table.InsertOnSubmit(entity);
            }

            Mapper.Map(item, entity);
            entity.UpdateDateTime = DateTime.Now;
            table.Context.SubmitChanges();
            item.LogID = entity.LogID;
        }

        public void Delete(StudentChangedLog item)
        {
            var entity = table.FirstOrDefault(x => x.LogID == item.LogID);

            if (!entity.IsNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}