using System.Linq;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using System.Data.Linq;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class DelegateTeacherRepository : IRepository<DelegateTeacher>
    {
        private readonly Table<DelegateTeacherEntity> table;

        public DelegateTeacherRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = DbDataContext.Get(connectionString).GetTable<DelegateTeacherEntity>();
        }

        public void Delete(DelegateTeacher item)
        {
            var entity = table.FirstOrDefault(x => x.DelegateTeacherID.Equals(item.DelegateTeacherID));
            if (entity.IsNotNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        public void Save(DelegateTeacher item)
        {
            var entity = table.FirstOrDefault(x => x.DelegateTeacherID.Equals(item.DelegateTeacherID));

            if (entity.IsNull())
            {
                entity = new DelegateTeacherEntity();
                table.InsertOnSubmit(entity);
            }

            MapToEntity(item, entity);
            table.Context.SubmitChanges();
            item.DelegateTeacherID = entity.DelegateTeacherID;
        }

        public IQueryable<DelegateTeacher> Select()
        {
            return table.Select(x => new DelegateTeacher
            {
                DelegateTeacherID = x.DelegateTeacherID,
                OriginalTeacherID = x.OriginalTeacherID,
                NewTeacherID = x.NewTeacherID,
                ClassID = x.ClassID,
                TimeStart = x.TimeStart,
                TimeEnd = x.TimeEnd,
                UserID = x.UserID,
                CreatedDate = x.CreatedDate,
                LastUpdatedDate = x.LastUpdatedDate,
                IsActive = x.IsActive
            });
        }

        private void MapToEntity(DelegateTeacher item, DelegateTeacherEntity entity)
        {
            if (item.IsNotNull() && entity.IsNotNull())
            {
                entity.DelegateTeacherID = item.DelegateTeacherID;
                entity.OriginalTeacherID = item.OriginalTeacherID;
                entity.NewTeacherID = item.NewTeacherID;
                entity.ClassID = item.ClassID;
                entity.TimeStart = item.TimeStart;
                entity.TimeEnd = item.TimeEnd;
                entity.UserID = item.UserID;
                entity.CreatedDate = item.CreatedDate;
                entity.LastUpdatedDate = item.LastUpdatedDate;
                entity.IsActive = item.IsActive;
            }
        }
    }
}