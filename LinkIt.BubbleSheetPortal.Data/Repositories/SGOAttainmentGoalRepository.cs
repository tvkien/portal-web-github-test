using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Xml.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class SGOAttainmentGoalRepository : IRepository<SGOAttainmentGoal>
    {
        private readonly Table<SGOAttainmentGoalEntity> table;

        public SGOAttainmentGoalRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = SGODataContext.Get(connectionString).GetTable<SGOAttainmentGoalEntity>();
        }

        public IQueryable<SGOAttainmentGoal> Select()
        {
            return table.Select(x => new SGOAttainmentGoal
                                     {
                                         Name = x.Name,
                                         Order = x.Order,
                                         SGOAttainmentGoalId = x.SGOAttainmentGoalID,
                                         SGOId = x.SGOID,
                                         DefaultGoal = x.DefaultGoal,
                                         ComparisonType = x.ComparisonType
                                     });
        }

        public void Save(SGOAttainmentGoal item)
        {
            var entity = table.FirstOrDefault(x => x.SGOAttainmentGoalID.Equals(item.SGOAttainmentGoalId));

            if (entity == null)
            {
                entity = new SGOAttainmentGoalEntity();
                table.InsertOnSubmit(entity);
            }

            MapModelToEntity(item, entity);

            table.Context.SubmitChanges();
            item.SGOAttainmentGoalId = entity.SGOAttainmentGoalID;
        }

        public void Delete(SGOAttainmentGoal item)
        {
            var entity = table.FirstOrDefault(x => x.SGOAttainmentGoalID.Equals(item.SGOAttainmentGoalId));
            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        private void MapModelToEntity(SGOAttainmentGoal model, SGOAttainmentGoalEntity entity)
        {
            entity.SGOAttainmentGoalID = model.SGOAttainmentGoalId;
            entity.SGOID = model.SGOId;
            entity.Name = model.Name;
            entity.Order = model.Order;
            entity.DefaultGoal = model.DefaultGoal;
            entity.ComparisonType = model.ComparisonType;
        }
    }
}