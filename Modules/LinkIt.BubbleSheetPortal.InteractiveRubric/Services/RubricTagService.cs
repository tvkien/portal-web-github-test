using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.InteractiveRubric.Models.Entities;
using LinkIt.BubbleSheetPortal.InteractiveRubric.Models.Enums;
using LinkIt.BubbleSheetPortal.InteractiveRubric.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs;

namespace LinkIt.BubbleSheetPortal.InteractiveRubric.Services
{
    public class RubricTagService : IRubricTagService
    {
        private readonly IRubricQuestionCategoryRepository _rubricQuestionCategoryRepository;
        private readonly IRubricCategoryTagRepository _rubricCategoryTagRepository;
        private readonly IMasterStandardRepository _masterStandardData;
        private readonly IRepository<Topic> _topicRepository;
        private readonly IRepository<LessonOne> _lessonOneRepository;
        private readonly IRepository<LessonTwo> _lessonTwoRepository;
        private readonly IRepository<ItemTag> _itemTagRepository;

        public RubricTagService(

            IRubricQuestionCategoryRepository rubricQuestionCategoryRepository,
            IRubricCategoryTagRepository rubricCategoryTagRepository,
             IMasterStandardRepository masterStandardData,
            IRepository<Topic> topicRepository,
            IRepository<LessonOne> lessonOneRepository,
            IRepository<LessonTwo> lessonTwoRepository,
            IRepository<ItemTag> itemTagRepository)
        {
            _rubricQuestionCategoryRepository = rubricQuestionCategoryRepository;
            _rubricCategoryTagRepository = rubricCategoryTagRepository;
            _masterStandardData = masterStandardData;
            _topicRepository = topicRepository;
            _lessonOneRepository = lessonOneRepository;
            _lessonTwoRepository = lessonTwoRepository;
            _itemTagRepository = itemTagRepository;
        }

        public IEnumerable<RubricQuestionCategoryItem> GetSelectListRubricQuestionCategoryByQuestionIds(IEnumerable<int> virtualQuestionIds)
        {
            return _rubricQuestionCategoryRepository.Select().Where(x => virtualQuestionIds.Contains(x.VirtualQuestionID))
                .Select(z => new RubricQuestionCategoryItem
                {
                    RubricQuestionCategoryID = z.RubricQuestionCategoryID,
                    CategoryName = z.CategoryName,
                    CategoryCode = z.CategoryCode,
                    OrderNumber = z.OrderNumber,
                    VirtualQuestionID = z.VirtualQuestionID
                }).OrderBy(x => x.OrderNumber);
        }

        public IEnumerable<RubricTagByCategoryDisplay> GetRubricTagsByQuestionIds(string virtualQuestionIds, List<string> acceptableStandards)
        {
            var tagsToDisplay = new List<RubricTagByCategoryDisplay>();
            var _virtualQuestionIds = virtualQuestionIds.ToIntArray();
            var rubricCatQuestions = GetSelectListRubricQuestionCategoryByQuestionIds(_virtualQuestionIds).Distinct();
            var categoryIds = rubricCatQuestions.Select(x => x.RubricQuestionCategoryID).ToArray();
            var rubricTagByCatIds = _rubricCategoryTagRepository.Select().Where(x => categoryIds.Contains(x.RubricQuestionCategoryID) && _virtualQuestionIds.Contains(x.VirtualQuestionID ?? 0));
            var tagDefaultInformations = _rubricCategoryTagRepository.GetAllTagsByVirtualQuestion(virtualQuestionIds);
            foreach (var questionId in _virtualQuestionIds)
            {
                var tagsByQuestion = new RubricTagByCategoryDisplay()
                {
                    VirtualQuestionID = questionId
                };
                var catsByQuestion = rubricCatQuestions.Where(x => categoryIds.Contains(x.RubricQuestionCategoryID) && x.VirtualQuestionID == questionId).ToArray();

                tagsByQuestion.Standards = BuildDisplayTagByType(rubricTagByCatIds, tagDefaultInformations, catsByQuestion, TagTypeEnum.Standards, acceptableStandards);

                tagsByQuestion.Topics = BuildDisplayTagByType(rubricTagByCatIds, tagDefaultInformations, catsByQuestion, TagTypeEnum.Topics);

                tagsByQuestion.Skills = BuildDisplayTagByType(rubricTagByCatIds, tagDefaultInformations, catsByQuestion, TagTypeEnum.Skills);

                tagsByQuestion.Others = BuildDisplayTagByType(rubricTagByCatIds, tagDefaultInformations, catsByQuestion, TagTypeEnum.Others);

                tagsByQuestion.Customs = BuildDisplayTagByType(rubricTagByCatIds, tagDefaultInformations, catsByQuestion, TagTypeEnum.Customs);

                tagsToDisplay.Add(tagsByQuestion);
            }
            return tagsToDisplay;
        }

        public IEnumerable<RubricCategoryTagDto> GetRubricCategoryTagSelectListByIds(int[] virtualQuestionIds, TagTypeEnum tagType, int? rubricCategoryId = 0)
        {
            var rubricCatQuestions = GetSelectListRubricQuestionCategoryByQuestionIds(virtualQuestionIds).Distinct().ToList();
            var rubricCategoryTagsData = _rubricCategoryTagRepository.Select().Where(x => x.TagType == tagType.ToString() && virtualQuestionIds.Contains(x.VirtualQuestionID ?? 0));
            if (rubricCategoryId > 0)
            {
                rubricCategoryTagsData = rubricCategoryTagsData.Where(x => x.RubricQuestionCategoryID == rubricCategoryId);
            }
            var rubricCategoryTags = rubricCategoryTagsData.ToList();

            return rubricCategoryTags.Join(
                    rubricCatQuestions,
                    tag => tag.RubricQuestionCategoryID,
                    cat => cat.RubricQuestionCategoryID,
                    (tag, cat) => new RubricCategoryTagDto
                    {
                        RubricCategoryTagID = tag.RubricCategoryTagID,
                        RubricQuestionCategoryID = tag.RubricQuestionCategoryID,
                        RubricQuestionCategoryName = cat.CategoryName,
                        TagID = tag.TagID,
                        TagName = tag.TagName,
                        TagDescription = tag.TagDescription,
                        TagType = tag.TagType,
                        TagCategoryID = tag.TagCategoryID ?? 0,
                        TagCategoryName = tag.TagCategoryName,
                        VirtualQuestionID = tag.VirtualQuestionID ?? 0
                    }
                );
        }

        private List<string> BuildDisplayTagByType(IQueryable<RubricCategoryTag> rubricTagByCatIds,
            IEnumerable<RubricCategoryTagDto> categoryTagDtosExisted,
            IEnumerable<RubricQuestionCategoryItem> questionCategoryItems, TagTypeEnum tagType, List<string> acceptableStandards = null)
        {
            var rubricTagByCat = questionCategoryItems.Join(
                    rubricTagByCatIds,
                    left => left.RubricQuestionCategoryID,
                    right => right.RubricQuestionCategoryID,
                    (cat, tag) =>
                    new
                    {
                        RubricQuestionCategoryItem = cat,
                        RubricCategoryTag = tag
                    }
                );

            if (tagType == TagTypeEnum.Standards && acceptableStandards != null)
            {
                categoryTagDtosExisted = categoryTagDtosExisted.Where(c => acceptableStandards.Contains(c.TagName));
            }
            if (tagType == TagTypeEnum.Customs)
            {
                var tagsToDisplay = new List<string>();
                var queryCustom = from tagExists in categoryTagDtosExisted
                                  join rubricTag in rubricTagByCat on tagExists.TagID equals rubricTag.RubricCategoryTag.TagID
                                  where tagExists.TagType == tagType.ToString()
                                  select new
                                  {
                                      tagExists.TagType,
                                      tagExists.TagID,
                                      tagExists.TagName,
                                      rubricTag.RubricCategoryTag.TagCategoryID,
                                      TagCategoryName = rubricTag.RubricCategoryTag.TagCategoryName,
                                      rubricTag.RubricQuestionCategoryItem,
                                      rubricTag.RubricQuestionCategoryItem.RubricQuestionCategoryID
                                  };

                var groupQueryCustom = queryCustom.Where(x => x.TagType == tagType.ToString())
                                    .GroupBy(g => new
                                    {
                                        g.RubricQuestionCategoryItem.RubricQuestionCategoryID,
                                        g.RubricQuestionCategoryItem.CategoryName
                                    })
                                    .Where(w => !string.IsNullOrEmpty(w.Key.CategoryName))
                                    .ToList();
                foreach (var item in groupQueryCustom)
                {
                    var tagToDisplay = $"<b>{item.Key.CategoryName}:</b><br />";
                    var tags = queryCustom.Where(x => x.RubricQuestionCategoryID == item.Key.RubricQuestionCategoryID).Select(x => new { x.TagName, x.TagCategoryName }).ToList();
                    var groupByTagCategoryNames = tags.GroupBy(x => x.TagCategoryName).Select(x => new
                    {
                        TagCategoryName = x.Key,
                        TagByKeys = tags.Where(t => t.TagCategoryName == x.Key)
                    }).ToList();

                    foreach (var tag in groupByTagCategoryNames)
                    {
                        tagToDisplay += "&nbsp;&nbsp;" + tag.TagCategoryName + ": " + string.Join(", ", tag.TagByKeys.Select(ss => ss.TagName)) + "<br />";
                    }
                    tagsToDisplay.Add(tagToDisplay);
                }

                return tagsToDisplay;
            }
            var query = from tagExists in categoryTagDtosExisted
                        join rubricTag in rubricTagByCat on tagExists.TagID equals rubricTag.RubricCategoryTag.TagID
                        where tagExists.TagType == tagType.ToString()
                        select new
                        {
                            tagExists.TagType,
                            tagExists.TagID,
                            tagExists.TagName,
                            rubricTag.RubricCategoryTag.TagCategoryID,
                            rubricTag.RubricCategoryTag.TagCategoryName,
                            rubricTag.RubricQuestionCategoryItem
                        };

            return query.Where(x => x.TagType == tagType.ToString())
                                .GroupBy(g => new
                                {
                                    g.RubricQuestionCategoryItem.RubricQuestionCategoryID,
                                    g.RubricQuestionCategoryItem.CategoryName
                                })
                                .Where(w => !string.IsNullOrEmpty(w.Key.CategoryName))
                                .Select(s => $"<b>{s.Key.CategoryName}:</b><br />&nbsp;&nbsp; {string.Join(", ", s.Select(ss => ss.TagName).Distinct())}")
                                .ToList();
        }

        public RubricCategoryTagDto GetTagInformationsByType(int tagId, TagTypeEnum tagType, int itemTagCategoryId = 0)
        {
            return GetTagInformationsByType(new int[] { tagId }, tagType, itemTagCategoryId).FirstOrDefault();
        }

        public IEnumerable<RubricCategoryTagDto> GetTagInformationsByType(int[] tagIds, TagTypeEnum tagType, int itemTagCategoryId = 0)
        {
            switch (tagType)
            {
                case TagTypeEnum.Standards:
                    return GetRubricTagMasterStandards(tagIds, tagType);

                case TagTypeEnum.Topics:
                    return GetRubricTagTopics(tagIds, tagType);

                case TagTypeEnum.Skills:
                    return GetRubricTagSkills(tagIds, tagType);

                case TagTypeEnum.Others:
                    return GetRubricTagOthers(tagIds, tagType);

                case TagTypeEnum.Customs:
                    return GetRubricCustomTags(tagIds, tagType, itemTagCategoryId);
            }
            return new List<RubricCategoryTagDto>();
        }

        private IEnumerable<RubricCategoryTagDto> GetRubricCustomTags(int[] tagIds, TagTypeEnum tagType, int itemTagCategoryId)
        {
            return _itemTagRepository.Select().Where(x => x.ItemTagCategoryID == itemTagCategoryId && tagIds.Contains(x.ItemTagID))
                .Select(t => new RubricCategoryTagDto
                {
                    TagID = t.ItemTagID,
                    TagType = tagType.ToString(),
                    TagName = t.Name,
                    TagDescription = t.Description,
                    TagCategoryID = t.ItemTagCategoryID,
                    TagCategoryName = t.Category,
                }).ToList();
        }

        private IEnumerable<RubricCategoryTagDto> GetRubricTagOthers(int[] tagIds, TagTypeEnum tagType)
        {
            return _lessonTwoRepository.Select().Where(x => tagIds.Contains(x.LessonTwoID)).Select(t => new RubricCategoryTagDto
            {
                TagID = t.LessonTwoID,
                TagType = tagType.ToString(),
                TagName = t.Name,
            }).ToList();
        }

        private IEnumerable<RubricCategoryTagDto> GetRubricTagSkills(int[] tagIds, TagTypeEnum tagType)
        {
            return _lessonOneRepository.Select().Where(x => tagIds.Contains(x.LessonOneID)).Select(t => new RubricCategoryTagDto
            {
                TagID = t.LessonOneID,
                TagType = tagType.ToString(),
                TagName = t.Name,
            }).ToList();
        }

        private IEnumerable<RubricCategoryTagDto> GetRubricTagTopics(int[] tagIds, TagTypeEnum tagType)
        {
            return _topicRepository.Select().Where(x => tagIds.Contains(x.TopicID)).Select(t => new RubricCategoryTagDto
            {
                TagID = t.TopicID,
                TagType = tagType.ToString(),
                TagName = t.Name,
            }).ToList();
        }

        private IEnumerable<RubricCategoryTagDto> GetRubricTagMasterStandards(int[] tagIds, TagTypeEnum tagType)
        {
            return _masterStandardData.Select().Where(x => tagIds.Contains(x.MasterStandardID))
                .Select(x => new RubricCategoryTagDto
                {
                    TagID = x.MasterStandardID,
                    TagType = tagType.ToString(),
                    TagName = string.IsNullOrEmpty(x.Number) ? x.Description.ReplaceWeirdCharacters() : x.Number,
                    TagDescription = x.Description.ReplaceWeirdCharacters()
                }).ToList();
        }

        public IEnumerable<RubricCategoryTagDto> GetAllTagsByVirtualQuestion(string virtualQuestionIds)
        {
            return _rubricCategoryTagRepository.GetAllTagsByVirtualQuestion(virtualQuestionIds);
        }

        public IEnumerable<RubricCategoryTagDto> GetAllTagsOfRubricByVirtualQuestion(int virtualQuestionId)
        {
            return _rubricCategoryTagRepository.Select().Where(y => y.VirtualQuestionID == virtualQuestionId).Select(x => new RubricCategoryTagDto
            {
                VirtualQuestionID = x.VirtualQuestionID ?? 0,
                TagCategoryID = x.TagCategoryID ?? 0,
                RubricCategoryTagID = x.RubricCategoryTagID,
                RubricQuestionCategoryID = x.RubricQuestionCategoryID,
                TagID = x.TagID,
                TagName = x.TagName,
                TagType = x.TagType,
                TagDescription = x.TagDescription,
                TagCategoryName = x.TagCategoryName,
            }).ToList();
        }
    }
}
