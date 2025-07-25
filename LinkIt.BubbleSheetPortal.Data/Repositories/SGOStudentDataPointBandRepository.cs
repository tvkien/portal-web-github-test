using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class SGOStudentDataPointBandRepository : IRepository<SGOStudentDataPointBand>
    {
        private readonly Table<SGOStudentDataPointBandEntity> table;

        public SGOStudentDataPointBandRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = SGODataContext.Get(connectionString).GetTable<SGOStudentDataPointBandEntity>();
        }

        public IQueryable<SGOStudentDataPointBand> Select()
        {
            return table.Select(x => new SGOStudentDataPointBand
                                     {
                                         SGOStudentDataPointID = x.SGOStudentDataPointID,
                                         SGODataPointBandID = x.SGODataPointBandID,
                                         SGOStudentDataPointBandID = x.SGOStudentDataPointBandID
                                     });
        }

        public void Save(SGOStudentDataPointBand item)
        {
            var entity = table.FirstOrDefault(x => x.SGOStudentDataPointBandID.Equals(item.SGOStudentDataPointBandID));

            if (entity == null)
            {
                entity = new SGOStudentDataPointBandEntity();
                table.InsertOnSubmit(entity);
            }

            MapModelToEntity(item, entity);

            table.Context.SubmitChanges();
            item.SGOStudentDataPointBandID = entity.SGOStudentDataPointBandID;
        }

        public void Delete(SGOStudentDataPointBand item)
        {
            var entity = table.FirstOrDefault(x => x.SGOStudentDataPointBandID.Equals(item.SGOStudentDataPointBandID));
            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        private void MapModelToEntity(SGOStudentDataPointBand model, SGOStudentDataPointBandEntity entity)
        {
            entity.SGOStudentDataPointID = model.SGOStudentDataPointID;
            entity.SGODataPointBandID = model.SGODataPointBandID;
        }
    }
}