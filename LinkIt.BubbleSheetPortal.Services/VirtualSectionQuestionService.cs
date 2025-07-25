using System.Linq;
using LinkIt.BubbleSheetPortal.Models;
using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Data.Repositories;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class VirtualSectionQuestionService
    {
        private readonly IVirtualSectionQuestionRepository _virtualSectionQuestionRepository;

        public VirtualSectionQuestionService(IVirtualSectionQuestionRepository virtualSectionQuestionRepository)
        {
            this._virtualSectionQuestionRepository = virtualSectionQuestionRepository;
        }

        public IQueryable<VirtualSectionQuestion> Select()
        {
            return _virtualSectionQuestionRepository.Select();
        }

        public List<VirtualSectionQuestion> GetVirtualSectionQuestionBySection(int virtualTestId,int virtualSectionId)
        {
            return _virtualSectionQuestionRepository.Select().Where(en => en.VirtualSectionId == virtualSectionId && en.VirtualTestId==virtualTestId).ToList();
        }

        public void Save(VirtualSectionQuestion item)
        {
            if (item.VirtualSectionQuestionId == 0)//Add new
            {
                //Check if there's any VirtualQuestionId in VirtualSestionQuestion
                var existingSectionQuesion =
                    _virtualSectionQuestionRepository.Select().Where(x => x.VirtualQuestionId == item.VirtualQuestionId)
                        .ToList();
                if(existingSectionQuesion!=null)
                {
                    foreach (var virtualSectionQuestion in existingSectionQuesion)
                    {
                        _virtualSectionQuestionRepository.Delete(virtualSectionQuestion);//Delete to avoid duplicate
                    }
                }
            }
            _virtualSectionQuestionRepository.Save(item);
        }

        public void Delete(VirtualSectionQuestion item)
        {
            _virtualSectionQuestionRepository.Delete(item);
        }

        public VirtualSectionQuestion GetVirtualSectionQuestionById(int id)
        {
            return _virtualSectionQuestionRepository.Select().FirstOrDefault(o => o.VirtualSectionQuestionId == id);
        }

        public VirtualSectionQuestion GetByVirtualQuestionId(int virtualQuestionId)
        {
            var sectionQuestion = _virtualSectionQuestionRepository.Select().FirstOrDefault(x => x.VirtualQuestionId == virtualQuestionId);
            return sectionQuestion;
        }

        public void InsertMultipleRecord(List<VirtualSectionQuestion> items)
        {
            _virtualSectionQuestionRepository.InsertMultipleRecord(items);
        }
    }
}
