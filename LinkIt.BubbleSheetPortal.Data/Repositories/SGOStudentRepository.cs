using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class SGOStudentRepository : IRepository<SGOStudent>
    {
        private readonly Table<SGOStudentEntity> table;

        public SGOStudentRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = SGODataContext.Get(connectionString).GetTable<SGOStudentEntity>();
        }

        public IQueryable<SGOStudent> Select()
        {
            return table.Select(x => new SGOStudent
            {
                ClassID = x.ClassID,
                SGOID = x.SGOID,
                SGOStudentID = x.SGOStudentID,
                SGOGroupID = x.SGOGroupID,
                StudentID = x.StudentID,
                Type = x.Type,
                ArchievedTarget = x.AchievedTarget.HasValue ? (x.AchievedTarget.Value ? 1 : 0) : 0
            });
        }

        public void Save(SGOStudent item)
        {
            var entity = table.FirstOrDefault(x => x.SGOStudentID.Equals(item.SGOStudentID));

            if (entity == null)
            {
                entity = new SGOStudentEntity();
                table.InsertOnSubmit(entity);
            }

            MapModelToEntity(item, entity);

            table.Context.SubmitChanges();
            item.SGOStudentID = entity.SGOStudentID;
        }

        public void Delete(SGOStudent item)
        {
            var entity = table.FirstOrDefault(x => x.SGOStudentID.Equals(item.SGOStudentID));
            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        private void MapModelToEntity(SGOStudent model, SGOStudentEntity entity)
        {
            entity.ClassID = model.ClassID;
            entity.SGOID = model.SGOID;
            entity.SGOGroupID = model.SGOGroupID;
            entity.StudentID = model.StudentID;
            entity.Type = model.Type;
            entity.AchievedTarget = model.ArchievedTarget.GetValueOrDefault() == 1;
        }
    }
}