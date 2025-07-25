using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class SGODataPointFilterRepository : IRepository<SGODataPointFilter>
    {
        private readonly Table<SGODataPointFilterEntity> table;

        public SGODataPointFilterRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = SGODataContext.Get(connectionString).GetTable<SGODataPointFilterEntity>();
        }

        public IQueryable<SGODataPointFilter> Select()
        {
            return table.Select(x => new SGODataPointFilter
                                     {
                                         SGODataPointID = x.SGODataPointID,
                                         FilterID = x.FilterID,
                                         FilterType = x.FilterType,
                                         SGODataPointFilterID = x.SGODataPointFilterID
                                     });
        }

        public void Save(SGODataPointFilter item)
        {
            var entity = table.FirstOrDefault(x => x.SGODataPointFilterID.Equals(item.SGODataPointFilterID));

            if (entity == null)
            {
                entity = new SGODataPointFilterEntity();
                table.InsertOnSubmit(entity);
            }

            MapModelToEntity(item, entity);

            table.Context.SubmitChanges();
            item.SGODataPointFilterID = entity.SGODataPointFilterID;
        }

        public void Delete(SGODataPointFilter item)
        {
            var entity = table.FirstOrDefault(x => x.SGODataPointFilterID.Equals(item.SGODataPointFilterID));
            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        private void MapModelToEntity(SGODataPointFilter model, SGODataPointFilterEntity entity)
        {
            entity.SGODataPointID = model.SGODataPointID;
            entity.FilterID = model.FilterID;
            entity.FilterType = model.FilterType;
        }
    }
}