using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class SGOStudentDataPointRepository : IRepository<SGOStudentDataPoint>
    {
        private readonly Table<SGOStudentDataPointEntity> table;

        public SGOStudentDataPointRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = SGODataContext.Get(connectionString).GetTable<SGOStudentDataPointEntity>();
        }

        public IQueryable<SGOStudentDataPoint> Select()
        {
            return table.Select(x => new SGOStudentDataPoint
                                     {
                                         SGODataPointID = x.SGODataPointID,
                                         SGOStudentDataPointID = x.SGOStudentDataPointID,
                                         SGOStudentID = x.SGOStudentID,
                                         TestResultID = x.TestResultID
                                     });
        }

        public void Save(SGOStudentDataPoint item)
        {
            var entity = table.FirstOrDefault(x => x.SGOStudentDataPointID.Equals(item.SGOStudentDataPointID));

            if (entity == null)
            {
                entity = new SGOStudentDataPointEntity();
                table.InsertOnSubmit(entity);
            }

            MapModelToEntity(item, entity);

            table.Context.SubmitChanges();
            item.SGOStudentDataPointID = entity.SGOStudentDataPointID;
        }

        public void Delete(SGOStudentDataPoint item)
        {
            var entity = table.FirstOrDefault(x => x.SGOStudentDataPointID.Equals(item.SGOStudentDataPointID));
            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        private void MapModelToEntity(SGOStudentDataPoint model, SGOStudentDataPointEntity entity)
        {
            entity.SGODataPointID = model.SGODataPointID;
            entity.SGOStudentID = model.SGOStudentID;
            entity.TestResultID = model.TestResultID;
        }
    }
}