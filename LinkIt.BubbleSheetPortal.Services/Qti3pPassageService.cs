using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class Qti3pPassageService
    {
        private readonly IReadOnlyRepository<QTI3pPassage> _passageRepository;

        public Qti3pPassageService(IReadOnlyRepository<QTI3pPassage> passageRepository)
        {
            _passageRepository = passageRepository;
        }

        public QTI3pPassage GetQti3PassageByName(string passageName)
        {
            return _passageRepository.Select().Where(x => x.PassageName.ToLower().Equals(passageName.ToLower())).FirstOrDefault();
        }
    }
}
