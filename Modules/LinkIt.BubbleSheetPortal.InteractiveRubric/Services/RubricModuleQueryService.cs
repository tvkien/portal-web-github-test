using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.InteractiveRubric.Models.Entities;
using LinkIt.BubbleSheetPortal.InteractiveRubric.Models.Enums;
using LinkIt.BubbleSheetPortal.InteractiveRubric.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs;

namespace LinkIt.BubbleSheetPortal.InteractiveRubric.Services
{
    public class RubricModuleQueryService : IRubricModuleQueryService
    {
        private readonly IRubricQuestionCategoryRepository _rubricQuestionCategoryRepository;
        private readonly IRubricCategoryTierRepository _rubricCategoryTierRepository;
        private readonly IRubricTestResultScoreRepository _rubricTestResultScoreRepository;
        private readonly IRubricCategoryTagRepository _rubricCategoryTagRepository;

        private readonly IRepository<VirtualQuestionData> _repositoryVirtualQuestion;

        private readonly IRubricTagService _rubricTagService;

        public RubricModuleQueryService(
            IRubricCategoryTierRepository rubricCategoryTierRepository,
            IRubricQuestionCategoryRepository rubricQuestionCategoryRepository,
            IRubricTestResultScoreRepository rubricTestResultScoreRepository,
            IRubricCategoryTagRepository rubricCategoryTagRepository,
            IRubricTagService rubricTagService,
            IRepository<VirtualQuestionData> repositoryVirtualQuestion
            )
        {
            _rubricCategoryTierRepository = rubricCategoryTierRepository;
            _rubricQuestionCategoryRepository = rubricQuestionCategoryRepository;
            _rubricTestResultScoreRepository = rubricTestResultScoreRepository;
            _rubricCategoryTagRepository = rubricCategoryTagRepository;
            _rubricTagService = rubricTagService;
            _repositoryVirtualQuestion = repositoryVirtualQuestion;
        }

        private IEnumerable<RubricQuestionCategoryDto> MappingRubricCategoryAndTier(IEnumerable<RubricQuestionCategoryDto> rubricQuestionCategories,
            IEnumerable<RubricCategoryTier> rubricCategoryTiers)
        {
            foreach (var item in rubricQuestionCategories)
            {
                item.RubricCategoryTiers = rubricCategoryTiers.Where(x => x.RubricQuestionCategoryID == item.RubricQuestionCategoryID).Select(x => new RubricCategoryTierDto
                {
                    Description = x.Description,
                    RubricQuestionCategoryID = x.RubricQuestionCategoryID,
                    Label = x.Label,
                    OrderNumber = x.OrderNumber,
                    Point = (int)(x.Point ?? 0),
                    RubricCategoryTierID = x.RubricCategoryTierID,
                    Selected = x.Point > 0 && x.Point == item.PointEarn,
                }).ToList();
            }
            return rubricQuestionCategories;
        }

        private IEnumerable<RubricQuestionCategoryDto> MappingRubricCategoryAndTestResultScore(IEnumerable<RubricQuestionCategoryDto> rubricQuestionCategories,
           IEnumerable<RubricCategoryTier> rubricCategoryTiers, IEnumerable<RubricTestResultScore> rubricTestResultScores, int qTIOnlineTestSessionID)
        {
            foreach (var item in rubricQuestionCategories)
            {
                var findScore = rubricTestResultScores.FirstOrDefault(x => x.RubricQuestionCategoryID == item.RubricQuestionCategoryID && x.VirtualQuestionID == item.VirtualQuestionID && x.QTIOnlineTestSessionID == qTIOnlineTestSessionID);
                item.PointEarn = findScore?.Score;
                item.RubricCategoryTiers = rubricCategoryTiers.Where(x => x.RubricQuestionCategoryID == item.RubricQuestionCategoryID).Select(x => new RubricCategoryTierDto
                {
                    Description = x.Description,
                    RubricQuestionCategoryID = x.RubricQuestionCategoryID,
                    Label = x.Label,
                    OrderNumber = x.OrderNumber,
                    Point = (int)(x.Point ?? 0),
                    RubricCategoryTierID = x.RubricCategoryTierID,
                    Selected = x.Point.HasValue && x.Point == item.PointEarn,
                }).ToList();
            }
            return rubricQuestionCategories;
        }

        public static RubricQuestionCategoryDto ToDtoModel(RubricQuestionCategory category)
        {
            return new RubricQuestionCategoryDto
            {
                RubricQuestionCategoryID = category.RubricQuestionCategoryID,
                CategoryCode = category.CategoryCode,
                CategoryName = category.CategoryName,
                OrderNumber = category.OrderNumber,
                PointsPossible = category?.PointsPossible ?? 0,
                VirtualQuestionID = category.VirtualQuestionID,
                RubricCategoryTags = new List<RubricCategoryTagDto>(),
                RubricCategoryTiers = new List<RubricCategoryTierDto>(),
            };
        }

        public IEnumerable<RubricQuestionCategoryDto> GetRubicQuestionCategoriesByVirtualQuestionIds(int virtualQuestionIds)
        {
            var findCategories = _rubricQuestionCategoryRepository.Select().Where(x => x.VirtualQuestionID == virtualQuestionIds).Select(item => ToDtoModel(item)).ToList();
            return findCategories;
        }

        public IEnumerable<RubricQuestionCategoryDto> GetRubicQuestionCategoriesByVirtualQuestionIds(int[] virtualQuestionIds)
        {
            var findCategories = _rubricQuestionCategoryRepository.Select().Where(x => virtualQuestionIds.Contains(x.VirtualQuestionID)).Select(item => ToDtoModel(item)).ToList();

            if (findCategories?.Count > 0)
            {
                var findCategoryIds = findCategories.Select(x => x.RubricQuestionCategoryID).ToArray();

                var rubricCategoryTags = _rubricCategoryTagRepository.Select().Where(x => findCategoryIds.Contains(x.RubricQuestionCategoryID)).ToList();
                if (rubricCategoryTags?.Count > 0)
                {
                    findCategories.ForEach(x => x.RubricCategoryTags = rubricCategoryTags.Where(s => s.RubricQuestionCategoryID == x.RubricQuestionCategoryID).Select(t => new RubricCategoryTagDto
                    {
                        RubricCategoryTagID = t.RubricCategoryTagID,
                        RubricQuestionCategoryID = t.RubricQuestionCategoryID,
                        TagID = t.TagID,
                        TagType = t.TagType,
                        TagCategoryID = t.TagCategoryID ?? 0,
                        TagCategoryName = t.TagCategoryName,
                        TagDescription = t.TagDescription,
                        TagName = t.TagName
                    }).ToList());
                }
                var findCategoryTiers = _rubricCategoryTierRepository.Select().Where(x => findCategoryIds.Contains(x.RubricQuestionCategoryID)).ToList();
                if (findCategoryTiers?.Count > 0)
                {
                    return MappingRubricCategoryAndTier(findCategories, findCategoryTiers);
                }
            }

            return findCategories;
        }

        public IEnumerable<RubricQuestionCategoryDto> GetRubicQuestionCategoriesIncludeTestResultScoreByQTIOnlineTestSessionID(int[] virtualQuestionIds, int qTIOnlineTestSessionID)
        {
            var findCategories = _rubricQuestionCategoryRepository.Select().Where(x => virtualQuestionIds.Contains(x.VirtualQuestionID)).Select(item => ToDtoModel(item)).ToList();

            if (findCategories?.Count > 0)
            {
                var findCategoryIds = findCategories.Select(x => x.RubricQuestionCategoryID).ToArray();

                var rubricTestResultScores = _rubricTestResultScoreRepository.Select().Where(x => findCategoryIds.Contains(x.RubricQuestionCategoryID) && x.QTIOnlineTestSessionID == qTIOnlineTestSessionID).ToList();

                var findCategoryTiers = _rubricCategoryTierRepository.Select().Where(x => findCategoryIds.Contains(x.RubricQuestionCategoryID)).ToList();
                if (findCategoryTiers?.Count > 0)
                {
                    return MappingRubricCategoryAndTestResultScore(findCategories, findCategoryTiers, rubricTestResultScores, qTIOnlineTestSessionID);
                }
            }

            return findCategories;
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
                    PointsPossible = z.PointsPossible ?? 0,
                    VirtualQuestionID = z.VirtualQuestionID
                }).OrderBy(x => x.OrderNumber).ToList();
        }

        public IEnumerable<RubricQuestionCategorySelectList> GetSelectListRubricQuestionCategoryItem(IEnumerable<int> virtualQuestionIds)
        {
            return GetSelectListRubricQuestionCategoryByQuestionIds(virtualQuestionIds)
                    .GroupBy(x => x.VirtualQuestionID)
                    .Select(d => new RubricQuestionCategorySelectList
                    {
                        VirtualQuestionID = d.Key,
                        RubricQuestionCategories = d.Select(z => new RubricQuestionCategoryItem
                        {
                            RubricQuestionCategoryID = z.RubricQuestionCategoryID,
                            CategoryName = z.CategoryName,
                            CategoryCode = z.CategoryCode,
                            OrderNumber = z.OrderNumber,
                            PointsPossible = z.PointsPossible ?? 0
                        }).ToList()
                    });
        }

        public IEnumerable<RubricTagByCategoryDisplay> GetRubricTagsByQuestionIds(string virtualQuestionIds, List<string> acceptableStandards = null)
        {
            return _rubricTagService.GetRubricTagsByQuestionIds(virtualQuestionIds, acceptableStandards);
        }

        public IEnumerable<RubricCategoryTagDto> GetRubricCategoryTagSelectListByIds(int[] virtualQuestionIds, TagTypeEnum tagType, int? rubricCategoryId = 0)
        {
            var virtualQuestionIdsRubric = _repositoryVirtualQuestion.Select().
                                        Where(x => virtualQuestionIds.Contains(x.VirtualQuestionID) && x.IsRubricBasedQuestion == true).Select(s => s.VirtualQuestionID).ToArray();
            if (virtualQuestionIdsRubric?.Count() > 0)
            {
                return _rubricTagService.GetRubricCategoryTagSelectListByIds(virtualQuestionIdsRubric, tagType, rubricCategoryId);
            }

            return new List<RubricCategoryTagDto>();
        }

        public IEnumerable<RubricCategoryTagDto> GetAllTagsByVirtualQuestion(string virtualQuestionIds)
        {
            return _rubricTagService.GetAllTagsByVirtualQuestion(virtualQuestionIds);
        }

        public IEnumerable<RubricCategoryTagDto> GetAllTagsOfRubricByVirtualQuestion(int virtualQuestionId)
        {
            return _rubricTagService.GetAllTagsOfRubricByVirtualQuestion(virtualQuestionId);
        }

        public IEnumerable<RubricCategoryTagDto> GetAllTagsOfRubricByTagId(int tagId, int[] virtualQuestionIds)
        {
            return _rubricCategoryTagRepository.Select().Where(x => virtualQuestionIds.Contains(x.VirtualQuestionID ?? 0) && x.TagID == tagId).Select(t => new RubricCategoryTagDto
            {
                TagID = t.TagID,
                TagCategoryID = t.TagCategoryID ?? 0,
                RubricCategoryTagID = t.RubricCategoryTagID,
                RubricQuestionCategoryID = t.RubricQuestionCategoryID,
                TagCategoryName = t.TagCategoryName,
                TagName = t.TagName,
                TagDescription = t.TagDescription,
                TagType = t.TagType,
                VirtualQuestionID = t.VirtualQuestionID ?? 0
            }).ToList();
        }
    }
}
