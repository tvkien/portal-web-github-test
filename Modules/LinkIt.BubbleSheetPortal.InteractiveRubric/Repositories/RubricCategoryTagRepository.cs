using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using AutoMapper;
using LinkIt.BubbleSheetPortal.Data;
using LinkIt.BubbleSheetPortal.InteractiveRubric.DataContext;
using LinkIt.BubbleSheetPortal.InteractiveRubric.Models.Entities;
using LinkIt.BubbleSheetPortal.InteractiveRubric.Models.Enums;
using LinkIt.BubbleSheetPortal.Models.DTOs;

namespace LinkIt.BubbleSheetPortal.InteractiveRubric.Repositories
{
    public class RubricCategoryTagRepository : IRubricCategoryTagRepository
    {
        private readonly Table<RubricCategoryTagEntity> table;
        private readonly RubricDataContext _rubricDataContext;

        public RubricCategoryTagRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = RubricDataContext.Get(connectionString).GetTable<RubricCategoryTagEntity>();
            _rubricDataContext = RubricDataContext.Get(connectionString);
            Mapper.CreateMap<RubricCategoryTagDto, RubricCategoryTagEntity>();
        }

        public void DeleteTagByVirtualQuestionIDs(int tagId, int[] rubricQuestionCategoryIds, int virtualQuestionId, TagTypeEnum tagType)
        {
            var rubricTags = table.Where(x => x.TagID == tagId && rubricQuestionCategoryIds.Contains(x.RubricQuestionCategoryID) && x.VirtualQuestionID == virtualQuestionId && x.TagType == tagType.ToString()).ToArray();
            if (rubricTags?.Length > 0)
            {
                table.DeleteAllOnSubmit(rubricTags);
                table.Context.SubmitChanges();
            }
        }

        public void DeleteTagByVirtualQuestionIDs(IEnumerable<int> rubricTagIds)
        {
            var rubricTags = table.Where(x => rubricTagIds.Contains(x.RubricCategoryTagID)).ToArray();
            if (rubricTags?.Length > 0)
            {
                table.DeleteAllOnSubmit(rubricTags);
                table.Context.SubmitChanges();
            }
        }

        public void AssignTagByVirtualQuestionIDs(IEnumerable<RubricCategoryTagDto> rubricCategoryTags)
        {
            foreach (var rubricTag in rubricCategoryTags)
            {
                var entity = new RubricCategoryTagEntity();
                Mapper.Map(rubricTag, entity);
                table.InsertOnSubmit(entity);
            }
            table.Context.SubmitChanges();
        }

        public void Delete(RubricCategoryTag item)
        {
            throw new System.NotImplementedException();
        }

        public void Save(RubricCategoryTag item)
        {
            throw new System.NotImplementedException();
        }

        public IQueryable<RubricCategoryTag> Select()
        {
            return table.Select(x => new RubricCategoryTag
            {
                RubricQuestionCategoryID = x.RubricQuestionCategoryID,
                RubricCategoryTagID = x.RubricCategoryTagID,
                TagID = x.TagID,
                TagType = x.TagType,
                TagCategoryID = x.TagCategoryID,
                TagCategoryName = x.TagCategoryName,
                TagDescription = x.TagDescription,
                TagName = x.TagName,
                VirtualQuestionID = x.VirtualQuestionID
            });
        }

        public IEnumerable<RubricCategoryTagDto> GetAllTagsByVirtualQuestion(string virtualQuestionIds)
        {
            return _rubricDataContext.GetAllTagsByVirtualQuestion(virtualQuestionIds)
               .Select(x => new RubricCategoryTagDto
               {
                   TagCategoryID = x.TagCategoryID,
                   TagCategoryName = x.TagCategoryName,
                   TagID = x.TagID,
                   TagName = x.TagName,
                   TagType = x.TagType,
                   VirtualQuestionID = x.VirtualQuestionID
               }).ToList();
        }

        public IQueryable<RubricCategoryTagDto> GetByTagIdsInCategoryId(string tagIds, string categoryIds, string tagType)
        {
            return _rubricDataContext.GetRubricTagByCategoryIdAndIds(tagIds, categoryIds, tagType)
                .Select(x => new RubricCategoryTagDto
                {
                    RubricCategoryTagID = x.RubricCategoryTagID,
                    RubricQuestionCategoryID = x.RubricQuestionCategoryID,
                    TagCategoryID = x.TagCategoryID,
                    TagCategoryName = x.TagCategoryName,
                    TagDescription = x.TagDescription,
                    TagID = x.TagID,
                    TagName = x.TagName,
                    TagType = x.TagType,
                    VirtualQuestionID = x.VirtualQuestionID
                }).AsQueryable();
        }

        public void Insert(IEnumerable<RubricCategoryTag> rubricCategoryTags)
        {
            var newEntries = rubricCategoryTags.Select(item => new RubricCategoryTagEntity
            {
                TagType = item.TagType,
                TagName = item.TagName,
                TagID = item.TagID,
                TagDescription = item.TagDescription,
                TagCategoryName = item.TagCategoryName,
                TagCategoryID = item.TagCategoryID,
                RubricCategoryTagID = item.RubricCategoryTagID,
                RubricQuestionCategoryID = item.RubricQuestionCategoryID,
                VirtualQuestionID = item.VirtualQuestionID
            });

            table.InsertAllOnSubmit(newEntries);
            table.Context.SubmitChanges();
        }
    }
}
