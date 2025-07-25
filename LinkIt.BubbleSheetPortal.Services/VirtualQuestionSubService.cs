using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class VirtualQuestionSubService
    {
        private readonly IRepository<VirtualQuestionSub> _repository;

        public VirtualQuestionSubService(IRepository<VirtualQuestionSub> repository)
        {
            _repository = repository;
        }

        public IQueryable<VirtualQuestionSub> Select()
        {
            return _repository.Select();
        }
        public void Delete(int virtualQuestionSubId)
        {
            var item = _repository.Select().FirstOrDefault(x => x.VirtualQuestionSubId == virtualQuestionSubId);
            if (item != null)
            {
                _repository.Delete(item);
            }
        }
        public void Save(VirtualQuestionSub virtualQuestionSub)
        {
            _repository.Save(virtualQuestionSub);
        }
    }
}
