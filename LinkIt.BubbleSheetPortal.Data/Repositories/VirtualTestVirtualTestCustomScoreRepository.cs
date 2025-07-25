using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class VirtualTestVirtualTestCustomScoreRepository : IRepository<VirtualTestVirtualTestCustomScore>
    {
        private readonly Table<VirtualTest_VirtualTestCustomScoreEntity> table;

        public VirtualTestVirtualTestCustomScoreRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = SGODataContext.Get(connectionString).GetTable<VirtualTest_VirtualTestCustomScoreEntity>();
        }

        public IQueryable<VirtualTestVirtualTestCustomScore> Select()
        {
            return table.Select(x => new VirtualTestVirtualTestCustomScore
                                     {
                                         VirtualTestCustomScoreId = x.VirtualTestCustomScoreID,
                                         VirtualTestVirtualTestCustomScoreId = x.VirtualTest_VirtualTestCustomScore1,
                                         VirtualTestId = x.VirtualTestID
                                     });
        }

        public void Save(VirtualTestVirtualTestCustomScore item)
        {
            var entity = table.FirstOrDefault(x => x.VirtualTest_VirtualTestCustomScore1.Equals(item.VirtualTestVirtualTestCustomScoreId));

            if (entity == null)
            {
                entity = new VirtualTest_VirtualTestCustomScoreEntity();
                table.InsertOnSubmit(entity);
            }

            MapModelToEntity(item, entity);

            table.Context.SubmitChanges();
            item.VirtualTestVirtualTestCustomScoreId = entity.VirtualTest_VirtualTestCustomScore1;
        }

        public void Delete(VirtualTestVirtualTestCustomScore item)
        {
            var entity = table.FirstOrDefault(x => x.VirtualTest_VirtualTestCustomScore1.Equals(item.VirtualTestVirtualTestCustomScoreId));
            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        private void MapModelToEntity(VirtualTestVirtualTestCustomScore model, VirtualTest_VirtualTestCustomScoreEntity entity)
        {
            entity.VirtualTestCustomScoreID = model.VirtualTestCustomScoreId;
            entity.VirtualTest_VirtualTestCustomScore1 = model.VirtualTestVirtualTestCustomScoreId;
            entity.VirtualTestID = model.VirtualTestId;
        }
    }
}