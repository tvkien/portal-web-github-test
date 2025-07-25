using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using Envoc.Core.Shared.Data;
using System.Data.Linq;
using LinkIt.BubbleSheetPortal.Models;
using AutoMapper;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class StudentChangedLogValueRepository : IRepository<StudentChangedLogValue>
    {
        private readonly Table<StudentChangedLogValueEntity> table;

        public StudentChangedLogValueRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = StudentDataContext.Get(connectionString).GetTable<StudentChangedLogValueEntity>();
            Mapper.CreateMap<StudentChangedLogValue, StudentChangedLogValueEntity>();
        }

        public IQueryable<StudentChangedLogValue> Select()
        {
            return table.Select(x => new StudentChangedLogValue
            {
                LogID = x.LogID,
                LogValueID = x.LogValueID,
                NewValue = x.NewValue,
                OldValue = x.OldValue,
                ValueChanged = x.ValueChanged
            });
        }

        public void Save(StudentChangedLogValue item)
        {
            var entity = table.FirstOrDefault(x => x.LogValueID == item.LogValueID);

            if (entity.IsNull())
            {
                entity = new StudentChangedLogValueEntity();
                table.InsertOnSubmit(entity);
            }

            Mapper.Map(item, entity);
            table.Context.SubmitChanges();
            item.LogValueID = entity.LogValueID;
        }

        public void Delete(StudentChangedLogValue item)
        {
            var entity = table.FirstOrDefault(x => x.LogValueID == item.LogValueID);

            if (!entity.IsNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}