using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class LessonTwoService
    {
        private readonly IRepository<LessonTwo> _repository;
        private readonly ILessonTwoRepository _lessonTwoRepository;

        public LessonTwoService(IRepository<LessonTwo> repository, ILessonTwoRepository lessonTwoRepository)
        {
            this._repository = repository;
            this._lessonTwoRepository = lessonTwoRepository;
        }

        public IQueryable<LessonTwo> GetAll()
        {
            return _repository.Select();
        }

        public void Save(LessonTwo item)
        {
            if (item != null)
            {
                _repository.Save(item);
            }
        }

        
        public void Delete(int lessonTwoId)
        {
            var item = _repository.Select().FirstOrDefault(x => x.LessonTwoID == lessonTwoId);
            if(item!=null)
            {
                _repository.Delete(item);
            }
        }

        public List<string> GetLessonTwosBySearchText(string textToSearch, string inputIdString, string type)
        {
            return _lessonTwoRepository.GetLessonTwosBySearchText(textToSearch, inputIdString, type).ToList();
        }
    }
}
