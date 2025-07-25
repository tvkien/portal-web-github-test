using System.Collections.Generic;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.InteractiveRubric.Models.Entities;

namespace LinkIt.BubbleSheetPortal.InteractiveRubric.Repositories
{
    public interface IRubricQuestionCategoryRepository : IRepository<RubricQuestionCategory>
    {
        IEnumerable<RubricQuestionCategory> Insert(List<RubricQuestionCategory> rubricQuestionCategories);

        IEnumerable<RubricQuestionCategory> Update(IEnumerable<RubricQuestionCategory> rubricQuestionCategories);

        void Delete(IEnumerable<RubricQuestionCategory> rubricQuestionCategories);

        void Delete(IEnumerable<int> rubricQuestionCategoryIds);
    }
}
