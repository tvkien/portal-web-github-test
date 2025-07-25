using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class TopicService
    {
        private readonly IRepository<Topic> _repository;
        private readonly ITopicRepository _topicRepository;

        public TopicService(IRepository<Topic> repository, ITopicRepository topicRepository)
        {
            this._repository = repository;
            this._topicRepository = topicRepository;
        }

        public IQueryable<Topic> GetAll()
        {
            return _repository.Select();
        }

        public void Save(Topic item)
        {
            if (item != null)
            {
                _repository.Save(item);
            }
        }

        
        public void Delete(int topicId)
        {
            var item = _repository.Select().FirstOrDefault(x => x.TopicID == topicId);
            if(item!=null)
            {
                _repository.Delete(item);
            }
        }

        public List<string> GetTopicsBySearchText(string textToSearch, string inputIdString, string type)
        {
            return _topicRepository.GetTopicsBySearchText(textToSearch, inputIdString, type).ToList();
        }
    }
}
