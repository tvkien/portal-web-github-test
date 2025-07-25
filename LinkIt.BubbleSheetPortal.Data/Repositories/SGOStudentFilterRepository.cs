using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class SGOStudentFilterRepository : IRepository<SGOStudentFilter>
    {
        private readonly Table<SGOStudentFilterEntity> table;

        public SGOStudentFilterRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = SGODataContext.Get(connectionString).GetTable<SGOStudentFilterEntity>();
        }

        public IQueryable<SGOStudentFilter> Select()
        {
            return table.Select(x => new SGOStudentFilter
                                     {
                                         SGOID = x.SGOID,
                                         FilterID = x.FilterID,
                                         FilterType = x.FilterType,
                                         SGOStudentFilterID = x.SGOStudentFilterID
                                     });
        }

        public void Save(SGOStudentFilter item)
        {
            var entity = table.FirstOrDefault(x => x.SGOStudentFilterID.Equals(item.SGOStudentFilterID));

            if (entity == null)
            {
                entity = new SGOStudentFilterEntity();
                table.InsertOnSubmit(entity);
            }

            MapModelToEntity(item, entity);

            table.Context.SubmitChanges();
            item.SGOStudentFilterID = entity.SGOStudentFilterID;
        }

        public void Delete(SGOStudentFilter item)
        {
            var entity = table.FirstOrDefault(x => x.SGOStudentFilterID.Equals(item.SGOStudentFilterID));
            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        private void MapModelToEntity(SGOStudentFilter model, SGOStudentFilterEntity entity)
        {
            entity.SGOID = model.SGOID;
            entity.FilterID = model.FilterID;
            entity.FilterType = model.FilterType;
        }
    }
}
