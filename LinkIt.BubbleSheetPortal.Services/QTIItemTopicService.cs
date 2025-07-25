using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class QTIItemTopicService
    {
        private readonly IRepository<QTIItemTopic> _repository;

        public QTIItemTopicService(IRepository<QTIItemTopic> repository)
        {
            this._repository = repository;
        }

        public IQueryable<QTIItemTopic> GetAll()
        {
            return _repository.Select();
        }

        public void Save(QTIItemTopic item)
        {
            if (item != null)
            {
                _repository.Save(item);
            }
        }


        public void Delete(int qTIItemTopicID)
        {
            var item = _repository.Select().FirstOrDefault(x => x.QTIItemTopicID == qTIItemTopicID);
            if(item!=null)
            {
                _repository.Delete(item);
            }
        }
        public void Delete(int qtiItemId,int topicId)
        {
            var item = _repository.Select().FirstOrDefault(x => x.QTIItemID == qtiItemId && x.TopicId == topicId);
            if (item != null)
            {
                _repository.Delete(item);
            }
        }
        public List<Topic> GetMutualTopicsOfManyQtiItems(string qtiItemIdString)
        {
            List<int> idList = ServiceUtil.GetIdListFromIdString(qtiItemIdString);
            var itemTopics = _repository.Select().Where(x => idList.Contains(x.QTIItemID)).Select(
                    x => new QTIItemTopic { QTIItemID = x.QTIItemID, TopicId = x.TopicId, Name = x.Name }).ToList();
            var topics = itemTopics.GroupBy(x => x.TopicId).Select(g => g.First()).Select(x => new Topic { TopicID = x.TopicId, Name = x.Name });
            List<Topic> resutl = new List<Topic>();
            foreach (var topic in topics)
            {
                var count = itemTopics.Where(x => x.TopicId == topic.TopicID).Count();
                if (count == idList.Count)//the mutal topic
                {
                    resutl.Add(topic);
                }
            }
            return resutl;
        }
    }
}
