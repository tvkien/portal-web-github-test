using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data;
using LinkIt.BubbleSheetPortal.InteractiveRubric.DataContext;
using LinkIt.BubbleSheetPortal.InteractiveRubric.Models.Entities;

namespace LinkIt.BubbleSheetPortal.InteractiveRubric.Repositories
{
    public class RubricCategoryTierRepository : IRubricCategoryTierRepository
    {
        private readonly Table<RubricCategoryTierEntity> table;

        public RubricCategoryTierRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = RubricDataContext.Get(connectionString).GetTable<RubricCategoryTierEntity>();
        }

        public void Delete(RubricCategoryTier item)
        {
            throw new System.NotImplementedException();
        }

        public void Save(RubricCategoryTier item)
        {
            throw new System.NotImplementedException();
        }

        public void Insert(IEnumerable<RubricCategoryTier> rubricCategoryTiers)
        {
            var newEntries = rubricCategoryTiers.Select(item => new RubricCategoryTierEntity
            {
                Description = item.Description,
                RubricQuestionCategoryID = item.RubricQuestionCategoryID,
                Label = item.Label,
                Point = item.Point,
                OrderNumber = item.OrderNumber
            });

            table.InsertAllOnSubmit(newEntries);
            table.Context.SubmitChanges();
        }

        public IQueryable<RubricCategoryTier> Select()
        {
            return table.Select(x => new RubricCategoryTier
            {
                RubricQuestionCategoryID = x.RubricQuestionCategoryID,
                RubricCategoryTierID = x.RubricCategoryTierID,
                OrderNumber = x.OrderNumber,
                Description = x.Description,
                Label = x.Label,
                Point = x.Point
            });
        }

        public void Delete(IEnumerable<RubricCategoryTier> rubricCategoryTiers)
        {
            var rubricCategoryTierIds = rubricCategoryTiers.Select(x => x.RubricCategoryTierID).ToArray();
            var entities = table.Where(x => rubricCategoryTierIds.Contains(x.RubricCategoryTierID)).ToArray();
            if (entities?.Length > 0)
            {
                table.DeleteAllOnSubmit(entities);

                table.Context.SubmitChanges();
            }
        }
    }
}
