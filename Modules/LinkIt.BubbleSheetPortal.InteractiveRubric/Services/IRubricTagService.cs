using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.InteractiveRubric.Models.Enums;
using LinkIt.BubbleSheetPortal.Models.DTOs;

namespace LinkIt.BubbleSheetPortal.InteractiveRubric.Services
{
    public interface IRubricTagService
    {
        IEnumerable<RubricTagByCategoryDisplay> GetRubricTagsByQuestionIds(string virtualQuestionIds, List<string> acceptableStandards);

        IEnumerable<RubricCategoryTagDto> GetRubricCategoryTagSelectListByIds(int[] virtualQuestionIds, TagTypeEnum tagType, int? rubricCategoryId = 0);

        RubricCategoryTagDto GetTagInformationsByType(int tagId, TagTypeEnum tagType, int itemTagCategoryId = 0);

        IEnumerable<RubricCategoryTagDto> GetTagInformationsByType(int[] tagIds, TagTypeEnum tagType, int itemTagCategoryId = 0);

        IEnumerable<RubricCategoryTagDto> GetAllTagsByVirtualQuestion(string virtualQuestionIds);

        IEnumerable<RubricCategoryTagDto> GetAllTagsOfRubricByVirtualQuestion(int virtualQuestionId);
    }
}
