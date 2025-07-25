using System;
using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.InteractiveRubric.Models.Entities;
using LinkIt.BubbleSheetPortal.InteractiveRubric.Models.Enums;
using LinkIt.BubbleSheetPortal.InteractiveRubric.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs;

namespace LinkIt.BubbleSheetPortal.InteractiveRubric.Services
{
    public class RubricModuleCommandService : IRubricModuleCommandService
    {
        private readonly IRubricQuestionCategoryRepository _rubricQuestionCategoryRepository;
        private readonly IRubricCategoryTierRepository _rubricCategoryTierRepository;
        private readonly IRubricTestResultScoreRepository _rubricTestResultScoreRepository;
        private readonly IRubricCategoryTagRepository _rubricCategoryTagRepository;
        private readonly IRepository<TestResult> _testResultRepository;
        private readonly IRepository<VirtualQuestionData> _virtualQuestionRepository;
        private readonly IRubricTagService _rubricTagService;

        public RubricModuleCommandService(
            IRubricCategoryTierRepository rubricCategoryTierRepository,
            IRubricQuestionCategoryRepository rubricQuestionCategoryRepository,
            IRubricTestResultScoreRepository rubricTestResultScoreRepository,
            IRubricCategoryTagRepository rubricCategoryTagRepository,
            IRepository<VirtualQuestionData> virtualQuestionRepository,
            IRepository<TestResult> testResultRepository,
            IRubricTagService rubricTagService)
        {
            _rubricCategoryTierRepository = rubricCategoryTierRepository;
            _rubricQuestionCategoryRepository = rubricQuestionCategoryRepository;
            _rubricTestResultScoreRepository = rubricTestResultScoreRepository;
            _rubricCategoryTagRepository = rubricCategoryTagRepository;

            _virtualQuestionRepository = virtualQuestionRepository;
            _rubricTagService = rubricTagService;
            _testResultRepository = testResultRepository;
        }

        public void CloneVirtualTest(int fromVirtualTestId, int toVirtualTestId)
        {
            var oldQuestions = _virtualQuestionRepository.Select().Where(o => o.VirtualTestID == fromVirtualTestId).OrderBy(x => x.QuestionOrder).ToList();
            var oldQuestionIds = oldQuestions.Select(x => x.VirtualQuestionID).ToArray();
            var newQuestions = _virtualQuestionRepository.Select().Where(o => o.VirtualTestID == toVirtualTestId).OrderBy(x => x.QuestionOrder).ToList();
            var dicQuestionIds = new Dictionary<int, int>();
            for (int i = 0; i < oldQuestionIds.Length; i++)
            {
                dicQuestionIds.Add(oldQuestionIds[i], newQuestions[i].VirtualQuestionID);
            }
            var getOldRubricCategories = _rubricQuestionCategoryRepository.Select().Where(x => oldQuestionIds.Contains(x.VirtualQuestionID)).OrderBy(x => x.RubricQuestionCategoryID).ToList();

            var getRubricCategoriesIds = getOldRubricCategories.OrderBy(x => x.RubricQuestionCategoryID).Select(x => x.RubricQuestionCategoryID).ToArray();
            var getRubricRubricCategoryTiers = _rubricCategoryTierRepository.Select().Where(x => getRubricCategoriesIds.Contains(x.RubricQuestionCategoryID)).OrderBy(x => x.RubricCategoryTierID).ToArray();
            var rubricQuestionCategoryDtos = new List<RubricQuestionCategoryDto>();
            var getRubricRubricCategoryTags = _rubricCategoryTagRepository.Select().Where(x => getRubricCategoriesIds.Contains(x.RubricQuestionCategoryID)).OrderBy(x => x.RubricCategoryTagID).ToArray();
            foreach (var item in getOldRubricCategories)
            {
                var findTiers = getRubricRubricCategoryTiers.Where(x => x.RubricQuestionCategoryID == item.RubricQuestionCategoryID).Select(x => new RubricCategoryTierDto
                {
                    RubricQuestionCategoryID = x.RubricQuestionCategoryID,
                    Description = x.Description,
                    Label = x.Label,
                    OrderNumber = x.OrderNumber,
                    Point = (int)x.Point,
                }).ToList();
                var findTags = getRubricRubricCategoryTags.Where(x => x.RubricQuestionCategoryID == item.RubricQuestionCategoryID).Select(x => new RubricCategoryTagDto
                {
                    RubricCategoryTagID = x.RubricCategoryTagID,
                    RubricQuestionCategoryID = x.RubricQuestionCategoryID,
                    TagCategoryID = x.TagCategoryID ?? 0,
                    TagCategoryName = x.TagCategoryName,
                    TagDescription = x.TagDescription,
                    TagID = x.TagID,
                    TagName = x.TagName,
                    TagType = x.TagType,
                }).ToList();
                rubricQuestionCategoryDtos.Add(new RubricQuestionCategoryDto
                {
                    CategoryCode = item.CategoryCode,
                    CategoryName = item.CategoryName,
                    OrderNumber = item.OrderNumber,
                    PointsPossible = item.PointsPossible,
                    VirtualQuestionID = item.VirtualQuestionID,
                    RubricQuestionCategoryID = item.RubricQuestionCategoryID,
                    RubricCategoryTiers = findTiers,
                    RubricCategoryTags = findTags
                });
            }

            var newRubricQuestionCategories = new List<RubricQuestionCategory>();
            foreach (var item in getOldRubricCategories)
            {
                var findVirtualQuestion = getOldRubricCategories.FirstOrDefault(x => x.VirtualQuestionID == item.VirtualQuestionID);
                var newVirtualQuestionID = dicQuestionIds.FirstOrDefault(x => x.Key == item.VirtualQuestionID).Value;
                findVirtualQuestion.VirtualQuestionID = newVirtualQuestionID;
                findVirtualQuestion.RubricQuestionCategoryID = 0;
                newRubricQuestionCategories.Add(findVirtualQuestion);
            }
            newRubricQuestionCategories = _rubricQuestionCategoryRepository.Insert(newRubricQuestionCategories.ToList()).ToList();

            var newRubricQuestionCategoriesIds = newRubricQuestionCategories.OrderBy(x => x.RubricQuestionCategoryID).Select(x => x.RubricQuestionCategoryID).ToArray();

            var dicRubricCategoryIds = new Dictionary<int, int>();
            for (int i = 0; i < getRubricCategoriesIds.Length; i++)
            {
                dicRubricCategoryIds.Add(getRubricCategoriesIds[i], newRubricQuestionCategoriesIds[i]);
            }
            foreach (var item in rubricQuestionCategoryDtos)
            {
                var newCatID = dicRubricCategoryIds.FirstOrDefault(x => x.Key == item.RubricQuestionCategoryID).Value;
                item.RubricQuestionCategoryID = newCatID;
                foreach (var tier in item.RubricCategoryTiers)
                {
                    tier.RubricQuestionCategoryID = newCatID;
                }
                foreach (var tag in item.RubricCategoryTags)
                {
                    tag.RubricQuestionCategoryID = newCatID;
                    tag.VirtualQuestionID = newRubricQuestionCategories.FirstOrDefault(x => x.RubricQuestionCategoryID == newCatID).VirtualQuestionID;
                }
                _rubricCategoryTierRepository.Insert(item.RubricCategoryTiers.Select(dic => new RubricCategoryTier
                {
                    RubricQuestionCategoryID = dic.RubricQuestionCategoryID,
                    Description = dic.Description,
                    Label = dic.Label,
                    OrderNumber = dic.OrderNumber,
                    Point = dic.Point,
                }));
                _rubricCategoryTagRepository.Insert(item.RubricCategoryTags.Select(tag => new RubricCategoryTag
                {
                    RubricQuestionCategoryID = tag.RubricQuestionCategoryID,
                    TagCategoryID = tag.TagCategoryID,
                    TagCategoryName = tag.TagCategoryName,
                    TagDescription = tag.TagDescription,
                    TagID = tag.TagID,
                    TagName = tag.TagName,
                    TagType = tag.TagType,
                    VirtualQuestionID = tag.VirtualQuestionID
                }));
            }
        }

        public void DeleteRubricTestResultScoreWhenDeleteTestResult(IEnumerable<int> listTestResultIds)
        {
            var findQTIOnlineTestSessionIDs = _testResultRepository.Select().Where(x => listTestResultIds.Contains(x.TestResultId) && x.QTIOnlineTestSessionID > 0).Select(y => y.QTIOnlineTestSessionID).ToArray();
            var findTestResultScoreToDelete = _rubricTestResultScoreRepository.Select().Where(x => findQTIOnlineTestSessionIDs.Contains(x.QTIOnlineTestSessionID)).Select(x => x.RubricTestResultScoreID).ToArray();
            _rubricTestResultScoreRepository.Delete(findTestResultScoreToDelete);
        }

        public void PurgeTestByVirtualTestId(int virtualTestId)
        {
            var findVirtualQuestionIds = _virtualQuestionRepository.Select().Where(x => x.VirtualTestID == virtualTestId).Select(x => x.VirtualQuestionID).ToArray();
            var findRubricQuestionCategoryIds = _rubricQuestionCategoryRepository.Select().Where(x => findVirtualQuestionIds.Contains(x.VirtualQuestionID)).Select(x => x.RubricQuestionCategoryID).ToArray();
            if (findRubricQuestionCategoryIds?.Count() > 0)
            {
                var findRubricTestResultScoreIds = _rubricTestResultScoreRepository.Select().Where(x => findRubricQuestionCategoryIds.Contains(x.RubricQuestionCategoryID)).Select(x => x.RubricTestResultScoreID).ToArray();
                if (findRubricTestResultScoreIds?.Count() > 0)
                {
                    _rubricTestResultScoreRepository.Delete(findRubricTestResultScoreIds);
                }

                _rubricQuestionCategoryRepository.Delete(findRubricQuestionCategoryIds);
            }
        }

        public IEnumerable<RubricQuestionCategoryDto> AddRubricCategories(IEnumerable<RubricQuestionCategoryDto> rubricQuestionCategories, int createdBy)
        {
            var listRubricQuestionCategories = rubricQuestionCategories.Select(x => new RubricQuestionCategory
            {
                CategoryCode = x.CategoryCode,
                CategoryName = x.CategoryName,
                OrderNumber = x.OrderNumber,
                VirtualQuestionID = x.VirtualQuestionID,
                CreatedBy = createdBy,
                PointsPossible = x.RubricCategoryTiers?.Max(p => p.Point ?? 0)
            }).ToList();
            var findVirtualQuestionID = rubricQuestionCategories.FirstOrDefault();
            if (findVirtualQuestionID != null)
            {
                listRubricQuestionCategories = _rubricQuestionCategoryRepository.Insert(listRubricQuestionCategories).ToList();
                var rubricCategoryTiers = new List<RubricCategoryTier>();
                for (var i = 0; i < rubricQuestionCategories.Count(); i++)
                {
                    var item = rubricQuestionCategories.ElementAt(i);

                    item.RubricQuestionCategoryID = listRubricQuestionCategories[i].RubricQuestionCategoryID;
                    foreach (var tier in item.RubricCategoryTiers)
                    {
                        rubricCategoryTiers.Add(new RubricCategoryTier
                        {
                            RubricQuestionCategoryID = item.RubricQuestionCategoryID,
                            Description = tier.Description,
                            Label = tier.Label,
                            OrderNumber = tier.OrderNumber,
                            Point = tier.Point
                        });
                    }
                }

                _rubricCategoryTierRepository.Insert(rubricCategoryTiers);
            }
            return rubricQuestionCategories;
        }

        public IEnumerable<RubricQuestionCategoryDto> SaveRubricCategories(IEnumerable<RubricQuestionCategoryDto> rubricQuestionCategories, int createdBy)
        {
            var rubricQuestionCategoriesResult = new List<RubricQuestionCategoryDto>();

            var listRubricQuestionCategories = rubricQuestionCategories.Select(x => new RubricQuestionCategory
            {
                RubricQuestionCategoryID = x.RubricQuestionCategoryID,
                CategoryCode = x.CategoryCode,
                CategoryName = x.CategoryName,
                OrderNumber = x.OrderNumber,
                VirtualQuestionID = x.VirtualQuestionID,
                CreatedBy = createdBy,
                UpdatedBy = createdBy
            }).ToList();
            var findVirtualQuestionID = rubricQuestionCategories.FirstOrDefault();
            if (findVirtualQuestionID != null)
            {
                var listRubricQuestionCategoryToUpdate = new List<RubricQuestionCategory>();
                var listRubricQuestionCategoryToDelete = new List<RubricQuestionCategory>();

                var findCategoriesExists = _rubricQuestionCategoryRepository.Select().Where(x => x.VirtualQuestionID == findVirtualQuestionID.VirtualQuestionID).ToList();
                if (findCategoriesExists?.Count > 0)
                {
                    foreach (var item in findCategoriesExists)
                    {
                        var findMap = listRubricQuestionCategories.FirstOrDefault(x => x.RubricQuestionCategoryID == item.RubricQuestionCategoryID);
                        if (findMap != null)
                        {
                            listRubricQuestionCategoryToUpdate.Add(findMap);
                        }
                        else
                        {
                            listRubricQuestionCategoryToDelete.Add(item);
                        }
                    }
                }
                if (listRubricQuestionCategoryToDelete?.Count > 0)
                {
                    _rubricQuestionCategoryRepository.Delete(listRubricQuestionCategoryToDelete);
                }
                if (listRubricQuestionCategoryToUpdate?.Count > 0)
                {
                    var findCategoriesExistsIds = listRubricQuestionCategoryToUpdate.Select(x => x.RubricQuestionCategoryID).ToArray();
                    var findTierExists = _rubricCategoryTierRepository.Select().Where(x => findCategoriesExistsIds.Contains(x.RubricQuestionCategoryID)).ToList();
                    if (findTierExists?.Count > 0)
                    {
                        _rubricCategoryTierRepository.Delete(findTierExists);
                    }
                    var rubricCategoryTiers = new List<RubricCategoryTier>();
                    foreach (var item in listRubricQuestionCategoryToUpdate)
                    {
                        var findRubricQuestionCategory = rubricQuestionCategories.FirstOrDefault(x => x.RubricQuestionCategoryID == item.RubricQuestionCategoryID);
                        if (findRubricQuestionCategory != null)
                        {
                            foreach (var tier in findRubricQuestionCategory.RubricCategoryTiers)
                            {
                                rubricCategoryTiers.Add(new RubricCategoryTier
                                {
                                    RubricQuestionCategoryID = item.RubricQuestionCategoryID,
                                    Description = tier.Description,
                                    Label = tier.Label,
                                    OrderNumber = tier.OrderNumber,
                                    Point = tier.Point
                                });
                            }

                            rubricQuestionCategoriesResult.Add(new RubricQuestionCategoryDto { RubricQuestionCategoryID = findRubricQuestionCategory.RubricQuestionCategoryID });
                        }
                    }
                    _rubricCategoryTierRepository.Insert(rubricCategoryTiers);
                    listRubricQuestionCategoryToUpdate.ForEach(x => x.PointsPossible = rubricCategoryTiers.Where(t => t.RubricQuestionCategoryID == x.RubricQuestionCategoryID)?.Max(y => y.Point ?? 0));
                    _rubricQuestionCategoryRepository.Update(listRubricQuestionCategoryToUpdate);
                }
                var listRubricQuestionCategoryToInsert = rubricQuestionCategories.Where(x => x.RubricQuestionCategoryID < 1).ToList();
                if (listRubricQuestionCategoryToInsert?.Count > 0)
                {
                    var inserted = AddRubricCategories(listRubricQuestionCategoryToInsert, createdBy);
                    rubricQuestionCategoriesResult.AddRange(inserted);
                }
            }

            return rubricQuestionCategoriesResult;
        }

        public void UpdateVirtualQuestionToRubricBase(VirtualQuestionData virtualQuestion, bool? isRubricBaseQuestion, int? pointsPossible = 0)
        {
            if (virtualQuestion != null)
            {
                virtualQuestion.IsRubricBasedQuestion = isRubricBaseQuestion;
                if (pointsPossible > 0)
                {
                    virtualQuestion.PointsPossible = pointsPossible ?? 0;
                }
                _virtualQuestionRepository.Save(virtualQuestion);
            }
            if (isRubricBaseQuestion == null)
            {
                var findRubricQuestionCategoryToDelete = _rubricQuestionCategoryRepository.Select().Where(x => x.VirtualQuestionID == virtualQuestion.VirtualQuestionID).ToList();
                if (findRubricQuestionCategoryToDelete?.Count > 0)
                {
                    _rubricQuestionCategoryRepository.Delete(findRubricQuestionCategoryToDelete);
                }
            }
        }

        public void SaveRubricTestResultScores(IEnumerable<RubricTestResultScoreDto> rubricTestResultScores, int qTIOnlineTestSessionId, int virtualQuestionId, int createdBy)
        {
            _rubricTestResultScoreRepository.SaveRubricTestResultScores(rubricTestResultScores, qTIOnlineTestSessionId, virtualQuestionId, createdBy);
        }

        public int DeleteCategoryTagByQuestionIds(int virtualQuestionId, int tagId, int[] rubricQuestionCategoryIds, TagTypeEnum tagType)
        {
            if (rubricQuestionCategoryIds.Length > 0 && tagId > 0 && virtualQuestionId > 0)
            {
                _rubricCategoryTagRepository.DeleteTagByVirtualQuestionIDs(tagId, rubricQuestionCategoryIds, virtualQuestionId, tagType);
                var findExisted = _rubricCategoryTagRepository.Select().Where(x => x.TagID == tagId && x.VirtualQuestionID == virtualQuestionId && x.TagType == tagType.ToString()).ToArray();
                return findExisted.Length;
            }

            return 0;
        }

        private List<RubricCategoryTagDto> BuildRubricTagToAssign(int tagId, TagTypeEnum tagType, List<int> virtualQuestionIds, List<RubricCategoryTagDto> existRubricTags, RubricCategoryTagDto tagInformations, RubricQuestionCategoryTag catTags)
        {
            List<RubricCategoryTagDto> tagsToAssign = new List<RubricCategoryTagDto>();
            var categoryIds = catTags.RubricQuestionCategoryIDs.ToIntArray();
            foreach (var categoryId in categoryIds)
            {
                var existRubricTag = existRubricTags.FirstOrDefault(x => virtualQuestionIds.Contains(x.VirtualQuestionID) && x.RubricQuestionCategoryID == categoryId);
                if (existRubricTag == null)
                {
                    tagsToAssign.Add(new RubricCategoryTagDto
                    {
                        RubricQuestionCategoryID = categoryId,
                        TagID = tagId,
                        TagType = tagType.ToString(),
                        TagName = tagInformations.TagName,
                        TagDescription = tagInformations.TagDescription,
                        TagCategoryID = tagInformations.TagCategoryID,
                        TagCategoryName = tagInformations.TagCategoryName,
                        VirtualQuestionID = virtualQuestionIds.FirstOrDefault()
                    });
                }
            }

            return tagsToAssign;
        }

        public string AssignCategoryTagByQuestionIds(IEnumerable<VirtualQuestionData> virtualQuestionDatas, int tagId, List<RubricQuestionCategoryTag> questionCategoryTags,
                                                        TagTypeEnum tagType, int itemTagCategoryId = 0)
        {
            var virtualQuestionIds = virtualQuestionDatas.Where(x => x.IsRubricBasedQuestion == true).Select(x => x.VirtualQuestionID).ToList();
            if (tagId > 0 && questionCategoryTags?.Count > 0 && virtualQuestionIds?.Count > 0)
            {
                try
                {
                    var rubricQuestionCategoryIds = string.Join(",", questionCategoryTags.Where(t => virtualQuestionIds.Contains(t.VirtualQuestionID)).Select(x => x.RubricQuestionCategoryIDs));

                    var existRubricTags = _rubricCategoryTagRepository.GetByTagIdsInCategoryId(tagId.ToString(), rubricQuestionCategoryIds, tagType.ToString()).ToList();
                    var tagInformations = _rubricTagService.GetTagInformationsByType(tagId, tagType, itemTagCategoryId);
                    if (tagInformations != null)
                    {
                        foreach (var catTags in questionCategoryTags)
                        {
                            var tagsToAssign = BuildRubricTagToAssign(tagId, tagType, virtualQuestionIds, existRubricTags, tagInformations, catTags);
                            if (tagsToAssign?.Count > 0)
                                _rubricCategoryTagRepository.AssignTagByVirtualQuestionIDs(tagsToAssign);
                        }
                    }
                }
                catch (Exception exception)
                {
                    return $"Error assigning new {tagType}: {exception.Message}";
                }
            }

            return string.Empty;
        }

        public string AssignCategoryTagByQuestionIds(int virtualQuestionId, List<RubricCategoryTagDto> rubricCategoryTagDtos)
        {
            try
            {
                var rubricCategoryTagExists = _rubricCategoryTagRepository.Select().Where(x => x.VirtualQuestionID == virtualQuestionId).ToList();
                var tagsToAssign = new List<RubricCategoryTagDto>();
                foreach (var item in rubricCategoryTagDtos)
                {
                    var existRubricTag = rubricCategoryTagExists.FirstOrDefault(x => x.VirtualQuestionID == virtualQuestionId && x.RubricQuestionCategoryID == item.RubricQuestionCategoryID && x.TagType == item.TagType && x.TagID == item.TagID);
                    if (existRubricTag == null)
                    {
                        tagsToAssign.Add(item);
                    }
                }

                if (tagsToAssign?.Count > 0)
                    _rubricCategoryTagRepository.AssignTagByVirtualQuestionIDs(tagsToAssign);

                return string.Empty;
            }
            catch (Exception exception)
            {
                return $"Error assigning new: {exception.Message}";
            }
        }
    }
}
