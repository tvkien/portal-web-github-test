using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class SGOMilestoneRepository : IRepository<SGOMilestone>
    {
        private readonly Table<SGOMilestoneEntity> table;

        public SGOMilestoneRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = SGODataContext.Get(connectionString).GetTable<SGOMilestoneEntity>();
        }

        public IQueryable<SGOMilestone> Select()
        {
            return table.Select(x => new SGOMilestone
                                     {
                                         MilestoneDate = x.MilestoneDate,
                                         SGOID = x.SGOID,
                                         SGOStatusID = x.SGOStatusID,
                                         SGOMilestoneID = x.SGOMilestoneID,
                                         UserID = x.UserID
                                     });
        }

        public void Save(SGOMilestone item)
        {
            var entity = table.FirstOrDefault(x => x.SGOMilestoneID.Equals(item.SGOMilestoneID));

            if (entity == null)
            {
                entity = new SGOMilestoneEntity();
                table.InsertOnSubmit(entity);
            }

            MapModelToEntity(item, entity);

            table.Context.SubmitChanges();
            item.SGOMilestoneID = entity.SGOMilestoneID;
        }

        public void Delete(SGOMilestone item)
        {
            var entity = table.FirstOrDefault(x => x.SGOMilestoneID.Equals(item.SGOMilestoneID));
            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        private void MapModelToEntity(SGOMilestone model, SGOMilestoneEntity entity)
        {
            entity.SGOID = model.SGOID;
            entity.SGOStatusID = model.SGOStatusID;
            entity.MilestoneDate = model.MilestoneDate;
            entity.UserID = model.UserID;
        }
    }
}