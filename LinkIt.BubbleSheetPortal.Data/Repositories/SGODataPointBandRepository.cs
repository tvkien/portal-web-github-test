using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class SGODataPointBandRepository : IRepository<SGODataPointBand>
    {
        private readonly Table<SGODataPointBandEntity> table;

        public SGODataPointBandRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = SGODataContext.Get(connectionString).GetTable<SGODataPointBandEntity>();
        }

        public IQueryable<SGODataPointBand> Select()
        {
            return table.Select(x => new SGODataPointBand
                                     {
                                         SGODataPointID = x.SGODataPointID,
                                         Name = x.Name,
                                         SGODataPointBandID = x.SGODataPointBandID,
                                         HighValue = x.HighValue,
                                         LowValue = x.LowValue
                                     });
        }

        public void Save(SGODataPointBand item)
        {
            var entity = table.FirstOrDefault(x => x.SGODataPointBandID.Equals(item.SGODataPointBandID));

            if (entity == null)
            {
                entity = new SGODataPointBandEntity();
                table.InsertOnSubmit(entity);
            }

            MapModelToEntity(item, entity);

            table.Context.SubmitChanges();
            item.SGODataPointBandID = entity.SGODataPointBandID;
        }

        public void Delete(SGODataPointBand item)
        {
            var entity = table.FirstOrDefault(x => x.SGODataPointBandID.Equals(item.SGODataPointBandID));
            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        private void MapModelToEntity(SGODataPointBand model, SGODataPointBandEntity entity)
        {
            entity.SGODataPointID = model.SGODataPointID;
            entity.Name = model.Name;
            entity.LowValue = model.LowValue;
            entity.HighValue = model.HighValue;
        }
    }
}