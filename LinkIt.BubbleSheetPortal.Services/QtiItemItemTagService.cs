using System;
using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class QtiItemItemTagService
    {
        private readonly IRepository<QtiItemItemTag> _qtiItemItemTagRepository;
        private readonly IRepository<ItemTag> _itemTagRepository;

        public QtiItemItemTagService(IRepository<QtiItemItemTag> qtiItemItemTagRepository, IRepository<ItemTag> itemTagRepository)
        {
            _qtiItemItemTagRepository = qtiItemItemTagRepository;
            _itemTagRepository = itemTagRepository;
        }

        public IQueryable<QtiItemItemTag> GetAll()
        {
            return _qtiItemItemTagRepository.Select();
        }
        public void Save(QtiItemItemTag item)
        {
            _qtiItemItemTagRepository.Save(item);
        }
        public void Delete(int qtiItemItemTagId)
        {
            var item = _qtiItemItemTagRepository.Select().FirstOrDefault(x => x.QtiItemItemTagID == qtiItemItemTagId);
            if(item!=null)
            {
                _qtiItemItemTagRepository.Delete(item);
            }
        }
        public void Delete(int qtiItemId, int itemTagId)
        {
            var item = _qtiItemItemTagRepository.Select().FirstOrDefault(x => x.QtiItemID == qtiItemId && x.ItemTagID==itemTagId);
            if (item != null)
            {
                _qtiItemItemTagRepository.Delete(item);
            }
        }
        public void DeleteQtiItemTagOfTag(int itemTagId)
        {
            var items = _qtiItemItemTagRepository.Select().Where(x => x.ItemTagID == itemTagId);
            foreach (var item in items)
            {
                _qtiItemItemTagRepository.Delete(item);
            }
        }
        public List<ItemTag> GetMutualItemTagOfManyQtiItems(string qtiItemIdString)
        {
            List<int> idList = ServiceUtil.GetIdListFromIdString(qtiItemIdString);
            var qtiItemItemTag = _qtiItemItemTagRepository.Select().Where(x => idList.Contains(x.QtiItemID)).Select(
                    x => new QtiItemItemTag { QtiItemID = x.QtiItemID, ItemTagID = x.ItemTagID}).ToList();

            var itemTagIds = qtiItemItemTag.GroupBy(x => x.ItemTagID).Select(g => g.First()).Select(x => x.ItemTagID).ToList();
            
            List<int> mutalItemTagIds = new List<int>();

            foreach (var tagId in itemTagIds)
            {
                var count = qtiItemItemTag.Where(x => x.ItemTagID == tagId).Count();
                if (count == idList.Count)//the mutal topic
                {
                    mutalItemTagIds.Add(tagId);
                }
            }
            List<ItemTag> resutl = new List<ItemTag>();
            resutl = _itemTagRepository.Select().Where(x => mutalItemTagIds.Contains(x.ItemTagID)).ToList();
            return resutl;
        }
       
    }
}