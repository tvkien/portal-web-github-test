using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs;
using LinkIt.BubbleSheetPortal.Models.Old.UnGroup;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class VirtualQuestionTopicService
    {
        private readonly IVirtualQuestionTopicRepository _repository;
        private readonly IRepository<Topic> _topicRepository;

        public VirtualQuestionTopicService(IVirtualQuestionTopicRepository repository, IRepository<Topic> topicRepository)
        {
            this._repository = repository;
            this._topicRepository = topicRepository;
        }

        public IQueryable<VirtualQuestionTopic> Select()
        {
            return _repository.Select();
        }

        public void Save(VirtualQuestionTopic virtualTestData)
        {
            _repository.Save(virtualTestData);
        }
        public void Delete(VirtualQuestionTopic virtualTestData)
        {
            _repository.Delete(virtualTestData);
        }
        public void Delete(int virtualQuestionId, int topicId)
        {
            var item =
                _repository.Select().FirstOrDefault(
                    x => x.VirtualQuestionId == virtualQuestionId && x.TopicId == topicId);
            if (item != null)
            {
                _repository.Delete(item);
            }
        }
        public List<Topic> GetMutualTopicsOfVirtualQuestions(string virtualQuestionIdString)
        {
            List<int> idList = ServiceUtil.GetIdListFromIdString(virtualQuestionIdString);
            var itemTopics = _repository.Select().Where(x => idList.Contains(x.VirtualQuestionId)).Select(
                    x => new VirtualQuestionTopic() { VirtualQuestionId = x.VirtualQuestionId, TopicId = x.TopicId, Name = x.Name }).ToList();
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
        public SaveStatusDTO AssignTopicTagForVirtualQuestions(string virtualQuestionIdString, string name)
        {
            var result = new SaveStatusDTO();
            try
            {
                name = HttpUtility.UrlDecode(name);
                //if tag already existed, no need to insert
                var topic = _topicRepository.Select().Where(x => x.Name.ToLower().Equals(name.ToLower())).FirstOrDefault();
                int topicId = 0;
                if (topic == null)
                {
                    //insert topic
                    var newItem = new Topic();
                    newItem.Name = name;
                    _topicRepository.Save(newItem);
                    topicId = newItem.TopicID;
                }
                else
                {
                    topicId = topic.TopicID;
                }

                if (topicId > 0)
                {
                    result.Id = topicId;
                    List<int> idList = ServiceUtil.GetIdListFromIdString(virtualQuestionIdString);
                    foreach (var virtualQuestionId in idList)
                    {
                        //check if this topic is existing for item or not
                        var item = _repository.Select().FirstOrDefault(x => x.VirtualQuestionId == virtualQuestionId && x.TopicId == topicId);
                        if (item == null)
                        {
                            var tag = new VirtualQuestionTopic();
                            tag.VirtualQuestionId = virtualQuestionId;
                            tag.TopicId = topicId;

                            _repository.Save(tag);
                            if (tag.VirtualQuestionTopicId == 0)
                            {
                                result.Error = "Error inserting tag to question.";
                                return result;
                            }
                        }

                    }
                }
                else
                {
                    result.Error = "Error inserting tag to question.";
                }

            }
            catch (Exception ex)
            {
               result.Error = string.Format("Error inserting new tag: {0}", ex.Message);
            }
            return result;
        }
        public string RemoveTopicTagForVirtualQuestions(string virtualQuestionIdString, int topicId)
        {
            try
            {
                List<int> idList = ServiceUtil.GetIdListFromIdString(virtualQuestionIdString);
                foreach (int id in idList)
                {
                    Delete(id, topicId);
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return string.Format("Error deleting tag: {0}", ex.Message);
            }
            return string.Empty;
        }

        public void CloneVirtualQuestionTopic(List<CloneVirtualQuestion> cloneVirtualQuestions)
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

                item.VirtualQuestionTopicId = 0;
                item.VirtualQuestionId = newVirtualQuestionId;
            });

            _repository.InsertMultipleRecord(olds);
        }
    }
}
