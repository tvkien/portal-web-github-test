using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class SGOAttainmentGroupRepository : IRepository<SGOAttainmentGroup>
    {
        private readonly Table<SGOAttainmentGroupEntity> table;

        public SGOAttainmentGroupRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = SGODataContext.Get(connectionString).GetTable<SGOAttainmentGroupEntity>();
        }

        public IQueryable<SGOAttainmentGroup> Select()
        {
            return table.Select(x => new SGOAttainmentGroup
                                     {
                                         GoalValue = x.GoalValue,
                                         Name = x.Name,
                                         Order = x.Order,
                                         SGOAttainmentGroupId = x.SGOAttainmentGroupID,
                                         SGOGroupId = x.SGOGroupID,
                                         SGOAttainmentGoalId = x.SGOAttainmentGoalID,
                                         GoalValueCustom = x.GoalValueCustom
                                     });
        }

        public void Save(SGOAttainmentGroup item)
        {
            var entity = table.FirstOrDefault(x => x.SGOAttainmentGroupID.Equals(item.SGOAttainmentGroupId));

            if (entity == null)
            {
                entity = new SGOAttainmentGroupEntity();
                table.InsertOnSubmit(entity);
            }

            MapModelToEntity(item, entity);

            table.Context.SubmitChanges();
            item.SGOAttainmentGroupId = entity.SGOAttainmentGroupID;
        }

        public void Delete(SGOAttainmentGroup item)
        {
            var entity = table.FirstOrDefault(x => x.SGOAttainmentGroupID.Equals(item.SGOAttainmentGroupId));
            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        private void MapModelToEntity(SGOAttainmentGroup model, SGOAttainmentGroupEntity entity)
        {
            entity.GoalValue = model.GoalValue;
            entity.Name = model.Name;
            entity.Order = model.Order;
            entity.SGOGroupID = model.SGOGroupId;
            entity.SGOAttainmentGoalID = model.SGOAttainmentGoalId;
            entity.GoalValueCustom = model.GoalValueCustom;
        }
    }
}