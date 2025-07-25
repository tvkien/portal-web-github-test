using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.InteractiveRubric.Models.Enums;
using LinkIt.BubbleSheetPortal.Models.DTOs;

namespace LinkIt.BubbleSheetPortal.InteractiveRubric.Services
{
    public interface IRubricModuleQueryService
    {
        IEnumerable<RubricQuestionCategoryDto> GetRubicQuestionCategoriesByVirtualQuestionIds(int[] virtualQuestionIds);

        IEnumerable<RubricQuestionCategoryDto> GetRubicQuestionCategoriesIncludeTestResultScoreByQTIOnlineTestSessionID(int[] virtualQuestionIds, int qTIOnlineTestSessionID);

        IEnumerable<RubricQuestionCategoryItem> GetSelectListRubricQuestionCategoryByQuestionIds(IEnumerable<int> virtualQuestionIds);

        IEnumerable<RubricQuestionCategorySelectList> GetSelectListRubricQuestionCategoryItem(IEnumerable<int> virtualQuestionIds);

        IEnumerable<RubricTagByCategoryDisplay> GetRubricTagsByQuestionIds(string virtualQuestionIds, List<string> acceptableStandards = null);

        IEnumerable<RubricCategoryTagDto> GetRubricCategoryTagSelectListByIds(int[] virtualQuestionIds, TagTypeEnum tagType, int? rubricCategoryId = 0);

        IEnumerable<RubricCategoryTagDto> GetAllTagsByVirtualQuestion(string virtualQuestionIds);

        IEnumerable<RubricCategoryTagDto> GetAllTagsOfRubricByVirtualQuestion(int virtualQuestionId);

        IEnumerable<RubricCategoryTagDto> GetAllTagsOfRubricByTagId(int tagId, int[] virtualQuestionIds);

        IEnumerable<RubricQuestionCategoryDto> GetRubicQuestionCategoriesByVirtualQuestionIds(int virtualQuestionIds);
    }
}
