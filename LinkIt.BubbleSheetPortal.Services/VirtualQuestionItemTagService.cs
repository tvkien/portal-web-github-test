using System;
using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Algorithmic;
using LinkIt.BubbleSheetPortal.Models.Old.UnGroup;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class VirtualQuestionItemTagService
    {
        private readonly IVirtualQuestionItemTagRepository _repository;
        private readonly IRepository<ItemTag> _itemTagRepository;

        public VirtualQuestionItemTagService(IVirtualQuestionItemTagRepository repository, IRepository<ItemTag> itemTagRepository)
        {
            this._repository = repository;
            this._itemTagRepository = itemTagRepository;
        }

        public IQueryable<VirtualQuestionItemTag> Select()
        {
            return _repository.Select();
        }
        public void Delete(int virtualQuestionId, int itemTagId)
        {
            var item =
                _repository.Select().FirstOrDefault(
                    x => x.VirtualQuestionId == virtualQuestionId && x.ItemTagId == itemTagId);
            if (item != null)
            {
                _repository.Delete(item);
            }
        }
        public void Save(VirtualQuestionItemTag virtualTestData)
        {
            _repository.Save(virtualTestData);
        }
        public List<ItemTag> GetMutualItemTagOfVirtualQuestions(string virtualQuestionIdString)
        {
            List<int> idList = ServiceUtil.GetIdListFromIdString(virtualQuestionIdString);
            var qtiItemItemTag = _repository.Select().Where(x => idList.Contains(x.VirtualQuestionId)).Select(
                    x => new VirtualQuestionItemTag() { VirtualQuestionId = x.VirtualQuestionId, ItemTagId = x.ItemTagId,Name = x.Name}).ToList();

            var itemTagIds = qtiItemItemTag.GroupBy(x => x.ItemTagId).Select(g => g.First()).Select(x => x.ItemTagId).ToList();

            List<int> mutalItemTagIds = new List<int>();

            foreach (var tagId in itemTagIds)
            {
                var count = qtiItemItemTag.Where(x => x.ItemTagId == tagId).Count();
                if (count == idList.Count)//the mutal topic
                {
                    mutalItemTagIds.Add(tagId);
                }
            }
            List<ItemTag> resutl = new List<ItemTag>();
            resutl = _itemTagRepository.Select().Where(x => mutalItemTagIds.Contains(x.ItemTagID)).ToList();
            return resutl;
        }
        public string AssignDistrictTagForVirtualQuestions(string virtualQuestionIdString, string tagIdString)
        {
            List<int> virtualQuestionIdList = ServiceUtil.GetIdListFromIdString(virtualQuestionIdString);
            List<int> tagIdList = ServiceUtil.GetIdListFromIdString(tagIdString);

            foreach (var virtualQuestionId in virtualQuestionIdList)
            {
                foreach (var tagId in tagIdList)
                {
                    //assign each tag to each qtiItem
                    var tag = new VirtualQuestionItemTag();
                    tag.VirtualQuestionId = virtualQuestionId;
                    tag.ItemTagId = tagId;
                    //check exists before saving
                    var exists = _repository.Select().Any(x => x.VirtualQuestionId == virtualQuestionId && x.ItemTagId == tagId);
                    if (!exists)
                    {
                        _repository.Save(tag);
                        if (tag.VirtualQuestionItemTagId == 0)
                        {
                            return "Error inserting tag to question.";
                            //error message "Error inserting tag to question." is gotten from flash
                        }
                    }
                }
            }
            return string.Empty;
        }
        public string RemoveDistrictTagForVirtualQuestions(string virtualQuestionIdString, int itemTagId)
        {
            List<int> idList = virtualQuestionIdString.ParseIdsFromString();
            try
            {
                foreach (int id in idList)
                {
                    Delete(id, itemTagId);
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return string.Format("Error deleting tag: {0}", ex.Message);
            }
            return string.Empty;
        }

        public void CloneVirtualQuestionItemTag(List<CloneVirtualQuestion> cloneVirtualQuestions)
        {
            var oldVirtualQuestionIDs = cloneVirtualQuestions.Select(x => x.OldVirtualQuestionID);

            var olds = _repository.Select().Where(o => oldVirtualQuestionIDs.Contains(o.VirtualQuestionId)).ToList();

            if (olds.Count == 0)
            {
                return;
            }

            olds.ForEach(item =>
            {
                var newVirtualQuestionId = cloneVirtualQuestions.FirstOrDefault(x => x.OldVirtualQuestionID == item.VirtualQuestionId).NewVirtualQuestionID;

                item.VirtualQuestionItemTagId = 0;
                item.VirtualQuestionId = newVirtualQuestionId;
            });

            _repository.InsertMultipleRecord(olds);
        }

        public IQueryable<VirtualQuestionItemTag> GetVirtualQuestionItemTagByTagCategoryID(int tagCategoryID)
        {
            var listItemTagID = _itemTagRepository.Select().Where(x => x.ItemTagCategoryID == tagCategoryID)
                .Select(x => x.ItemTagID).Distinct().ToList();
            return _repository.Select().Where(x => listItemTagID.Contains(x.ItemTagId));
        }
    }
}
