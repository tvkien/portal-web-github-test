using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class QtiItemQTI3pPassageService
    {
        private readonly IRepository<QtiItemQTI3pPassage> _repository;
        private readonly IReadOnlyRepository<QTI3pPassage> _qti3pPassageRepository;

        public QtiItemQTI3pPassageService(IRepository<QtiItemQTI3pPassage> repository, IReadOnlyRepository<QTI3pPassage> qti3pPassageRepository)
        {
            _repository = repository;
            _qti3pPassageRepository = qti3pPassageRepository;
        }

        public IQueryable<QtiItemQTI3pPassage> GetAll()
        {
            return _repository.Select();
        }


        public void Save(QtiItemQTI3pPassage item)
        {
            _repository.Save(item);
        }

        public void Delete(int qtiItemQTI3pPassageId)
        {
            _repository.Delete(new QtiItemQTI3pPassage { QtiItemQTI3pPassageID = qtiItemQTI3pPassageId });
        }
        public void Delete(QtiItemQTI3pPassage item)
        {
            _repository.Delete(item);
        }

        public void Assign(int qtiItemId, int qTI3pPassageId)
        {
            if (!_repository.Select().Any(x => x.QtiItemId == qtiItemId && x.QTI3pPassageId == qTI3pPassageId))
            {
                var newAssign = new QtiItemQTI3pPassage { QtiItemId = qtiItemId, QTI3pPassageId = qTI3pPassageId };
                _repository.Save(newAssign);
            }
        }
        public void Deassign(int qtiItemId, string link)
        {
            //link such as http://www.linkit.com/NWEA00/Production/01 Full Item Bank/04 96DPI JPG and MathML/01 ELA 96DPI JPG and MathML/Grade 01Language Arts-0/passages/3279.htm
            if(link==null)
            {
                link = string.Empty;
            }
            var htmlpage = link.Split(new char[] {'/'});
            var number = string.Empty;
            if(htmlpage!=null)
            {
                number = htmlpage[htmlpage.Length - 1].Split(new char[] { '.' })[0];

            }
            if(!string.IsNullOrWhiteSpace(number))
            {
                var passage = _qti3pPassageRepository.Select().FirstOrDefault(x => x.Number.Equals(number));
                if(passage != null)
                {
                    var assign =
                    _repository.Select().FirstOrDefault(x => x.QtiItemId == qtiItemId && x.QTI3pPassageId == passage.QTI3pPassageID);

                    if (assign != null)
                    {
                        _repository.Delete(assign);
                    }
                }
            }
           
        }
    }
}
