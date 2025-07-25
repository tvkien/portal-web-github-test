using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class ItemTagService
    {
        private readonly IRepository<ItemTagCategory> _itemCategoryRepository;
        private readonly IRepository<ItemTag> _itemTagRepository;
        private readonly IItemTagRepository _itemTagRepositoryNew;

        public ItemTagService(IRepository<ItemTagCategory> itemCategoryRepository, IRepository<ItemTag> itemTagRepository, IItemTagRepository itemTagRepositoryNew)
        {
            _itemCategoryRepository = itemCategoryRepository;
            _itemTagRepository = itemTagRepository;
            _itemTagRepositoryNew = itemTagRepositoryNew;
        }
        #region ItemTagCategory
        public IQueryable<ItemTagCategory> GetAllItemCategory()
        {
            return _itemCategoryRepository.Select();
        }
        public ItemTagCategory GetItemCategory(int itemTagCategoryId)
        {
            return _itemCategoryRepository.Select().Where(x=>x.ItemTagCategoryID==itemTagCategoryId).FirstOrDefault();
        }
        public bool IsExistCategory(int districtId, string name)
        {
            return _itemCategoryRepository.Select().Any(
                    x => x.DistrictID == districtId && x.Name.ToLower().Equals(name.ToLower()));
        }
        public bool IsExistCategory(int districtId,int itemTagCategoryId, string name)
        {
            return _itemCategoryRepository.Select().Any(
                    x =>x.ItemTagCategoryID !=itemTagCategoryId && x.DistrictID == districtId && x.Name.ToLower().Equals(name.ToLower()));
        }
        public void SaveItemTagCategory(ItemTagCategory item)
        {
            _itemCategoryRepository.Save(item);
        }
        public void DeleteItemTagCategory(ItemTagCategory item)
        {
            if (item != null)
            {
                _itemCategoryRepository.Delete(item);
            }
        }
        public void DeleteItemTagCategory(int itemTagCategoryId)
        {
            var item =_itemCategoryRepository.Select().Where(x => x.ItemTagCategoryID == itemTagCategoryId).FirstOrDefault();
            if (item != null)
            {
                _itemCategoryRepository.Delete(item);
            }
        }
        #endregion ItemTagCategory

        #region ItemTag
        public IQueryable<ItemTag> GetAllItemTag()
        {
            return _itemTagRepository.Select();
        }
        public IQueryable<ItemTag> GetAllItemTagByCategory(int categoryId)
        {
            return _itemTagRepository.Select().Where(x => x.ItemTagCategoryID == categoryId);
        }
        public bool IsExistTag(int itemTagCategoryId, string name)
        {
            return _itemTagRepository.Select().Any(
                    x => x.ItemTagCategoryID== itemTagCategoryId && x.Name.ToLower().Equals(name.ToLower()));
        }
        public bool IsExistTag(int itemTagCategoryId, int itemTagId, string name)
        {
            return _itemTagRepository.Select().Any(
                    x => x.ItemTagID != itemTagId && x.ItemTagCategoryID == itemTagCategoryId && x.Name.ToLower().Equals(name.ToLower()));
        }
        public void SaveItemTag(ItemTag item)
        {
            _itemTagRepository.Save(item);
        }
        public ItemTag GetItemTag(int itemTagId)
        {
            return _itemTagRepository.Select().Where(x => x.ItemTagID == itemTagId).FirstOrDefault();
        }
        public void DeleteItemTag(ItemTag item)
        {
            if (item != null)
            {
                _itemTagRepository.Delete(item);
            }
        }

        public List<string> GetSuggestTags(int districtId, string textToSearch)
        {
            return _itemTagRepositoryNew.GetSuggestTags(districtId, textToSearch).ToList();
        }

        public ItemTag GetItemTagInfo(int itemTagId)
        {
            return _itemTagRepositoryNew.Select().FirstOrDefault(x => x.ItemTagID == itemTagId);
        }

        #endregion ItemTag
        public bool HasRightToEditItemTags(List<int> itemTagIdList, User currentUser, List<int> listDistrictId)
        {
            if (currentUser.IsPublisher)
            {
                return true;
            }
            var authorizedItemTag = new List<int>();
            if (currentUser.IsNetworkAdmin)
            {
                
                  authorizedItemTag=  _itemTagRepository.Select()
                        .Where(x => listDistrictId.Contains(x.DistrictID) && itemTagIdList.Contains(x.ItemTagID))
                        .Select(x => x.ItemTagID)
                        .ToList();
               
            }
            else
            {
                authorizedItemTag = _itemTagRepository.Select()
                     .Where(x => x.DistrictID == currentUser.DistrictId && itemTagIdList.Contains(x.ItemTagID))
                     .Select(x => x.ItemTagID)
                     .ToList();
            }
            foreach (var itemTagId in itemTagIdList)
            {
                if (!authorizedItemTag.Contains(itemTagId))
                {
                    return false;//if there's only one unauthorized item tag, return false
                }
            }
            return true;
        }
        public List<ItemTag> GetAuthorizedItemTags(List<ItemTag> itemTags, User currentUser, List<int> listDistrictId)
        {
            if (currentUser.IsPublisher)
            {
                return itemTags;
            }
            if (currentUser.IsNetworkAdmin)
            {
                return itemTags.Where(x => listDistrictId.Contains(x.DistrictID)).ToList();

            }
            else
            {
                return itemTags.Where(x => x.DistrictID==currentUser.DistrictId).ToList();
            }
        }
    }
}
