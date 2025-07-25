using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class LessonOneService
    {
        private readonly IRepository<LessonOne> _repository;
        private readonly ILessonOneRepository _lessonOneRepository;

        public LessonOneService(IRepository<LessonOne> repository, ILessonOneRepository lessonOneRepository)
        {
            this._repository = repository;
            this._lessonOneRepository = lessonOneRepository;
        }

        public IQueryable<LessonOne> GetAll()
        {
            return _repository.Select();
        }

        public void Save(LessonOne item)
        {
            if (item != null)
            {
                _repository.Save(item);
            }
        }

        
        public void Delete(int lessonOneId)
        {
            var item = _repository.Select().FirstOrDefault(x => x.LessonOneID == lessonOneId);
            if(item!=null)
            {
                _repository.Delete(item);
            }
        }

        public List<string> GetLessonOnesBySearchText(string textToSearch, string inputIdString, string type)
        {
            return _lessonOneRepository.GetLessonOnesBySearchText(textToSearch, inputIdString, type).ToList();
        }
    }
}
