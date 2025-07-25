using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.InteractiveRubric.Models.Enums;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs;

namespace LinkIt.BubbleSheetPortal.InteractiveRubric.Services
{
    public interface IRubricModuleCommandService
    {
        void CloneVirtualTest(int fromVirtualTestId, int toVirtualTestId);

        void DeleteRubricTestResultScoreWhenDeleteTestResult(IEnumerable<int> listTestResultIds);

        void PurgeTestByVirtualTestId(int virtualTestId);

        IEnumerable<RubricQuestionCategoryDto> AddRubricCategories(IEnumerable<RubricQuestionCategoryDto> rubricQuestionCategories, int createdBy);

        IEnumerable<RubricQuestionCategoryDto> SaveRubricCategories(IEnumerable<RubricQuestionCategoryDto> rubricQuestionCategories, int createdBy);

        void UpdateVirtualQuestionToRubricBase(VirtualQuestionData virtualQuestion, bool? isRubricBaseQuestion, int? pointsPossible = 0);

        void SaveRubricTestResultScores(IEnumerable<RubricTestResultScoreDto> rubricTestResultScores, int qTIOnlineTestSessionId, int virtualQuestionId, int createdBy);

        int DeleteCategoryTagByQuestionIds(int virtualQuestionId, int tagId, int[] rubricQuestionCategoryIds, TagTypeEnum tagType);

        string AssignCategoryTagByQuestionIds(IEnumerable<VirtualQuestionData> virtualQuestionDatas, int tagId, List<RubricQuestionCategoryTag> questionCategoryTags, TagTypeEnum tagType, int itemTagCategoryId = 0);

        string AssignCategoryTagByQuestionIds(int virtualQuestionId, List<RubricCategoryTagDto> rubricCategoryTagDtos);
    }
}
