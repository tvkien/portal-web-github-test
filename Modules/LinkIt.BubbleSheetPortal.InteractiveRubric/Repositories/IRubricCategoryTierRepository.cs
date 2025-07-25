using System.Collections.Generic;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.InteractiveRubric.Models.Entities;

namespace LinkIt.BubbleSheetPortal.InteractiveRubric.Repositories
{
    public interface IRubricCategoryTierRepository : IRepository<RubricCategoryTier>
    {
        void Insert(IEnumerable<RubricCategoryTier> rubricCategoryTiers);

        void Delete(IEnumerable<RubricCategoryTier> rubricCategoryTiers);
    }
}
