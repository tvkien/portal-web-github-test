using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.InteractiveRubric.Models.Entities;
using LinkIt.BubbleSheetPortal.InteractiveRubric.Models.Enums;
using LinkIt.BubbleSheetPortal.Models.DTOs;

namespace LinkIt.BubbleSheetPortal.InteractiveRubric.Repositories
{
    public interface IRubricCategoryTagRepository : IRepository<RubricCategoryTag>
    {
        void DeleteTagByVirtualQuestionIDs(int tagId, int[] rubricQuestionCategoryIds, int virtualQuestionId, TagTypeEnum tagType);

        void DeleteTagByVirtualQuestionIDs(IEnumerable<int> rubricTagIds);

        IQueryable<RubricCategoryTagDto> GetByTagIdsInCategoryId(string tagIds, string categoryIds, string tagType);

        void AssignTagByVirtualQuestionIDs(IEnumerable<RubricCategoryTagDto> rubricCategoryTags);

        void Insert(IEnumerable<RubricCategoryTag> rubricCategoryTags);

        IEnumerable<RubricCategoryTagDto> GetAllTagsByVirtualQuestion(string virtualQuestionIds);
    }
}
