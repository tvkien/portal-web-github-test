using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class SGODataPointClusterScoreRepository : IRepository<SGODataPointClusterScore>
    {
        private readonly Table<SGODataPointClusterScoreEntity> table;

        public SGODataPointClusterScoreRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = SGODataContext.Get(connectionString).GetTable<SGODataPointClusterScoreEntity>();
        }

        public IQueryable<SGODataPointClusterScore> Select()
        {
            return table.Select(x => new SGODataPointClusterScore
                                     {
                                         SGODataPointID = x.SGODataPointID,
                                         SGODataPointClusterScoreID = x.SGODataPointClusterScoreID,
                                         TestResultSubScoreName = x.TestResultSubScoreName,
                                         VirtualTestCustomSubScoreId = x.VirtualTestCustomSubScoreID ?? 0
                                     });
        }

        public void Save(SGODataPointClusterScore item)
        {
            var entity = table.FirstOrDefault(x => x.SGODataPointClusterScoreID.Equals(item.SGODataPointClusterScoreID));

            if (entity == null)
            {
                entity = new SGODataPointClusterScoreEntity();
                table.InsertOnSubmit(entity);
            }

            MapModelToEntity(item, entity);

            table.Context.SubmitChanges();
            item.SGODataPointClusterScoreID = entity.SGODataPointClusterScoreID;
        }

        public void Delete(SGODataPointClusterScore item)
        {
            var entity = table.FirstOrDefault(x => x.SGODataPointClusterScoreID.Equals(item.SGODataPointClusterScoreID));
            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        private void MapModelToEntity(SGODataPointClusterScore model, SGODataPointClusterScoreEntity entity)
        {
            entity.SGODataPointID = model.SGODataPointID;
            entity.TestResultSubScoreName = model.TestResultSubScoreName;
            entity.VirtualTestCustomSubScoreID = model.VirtualTestCustomSubScoreId;
        }
    }
}