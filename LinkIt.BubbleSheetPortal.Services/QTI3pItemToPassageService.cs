using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class QTI3pItemToPassageService
    {
        private readonly IReadOnlyRepository<QTI3pItemToPassage> _qti3pItemToPassageRepository;
        private readonly QTI3pPassageRepository _qti3pPassageRepository;

        public QTI3pItemToPassageService(IReadOnlyRepository<QTI3pItemToPassage> passageRepository, QTI3pPassageRepository qti3pPassageRepository)
        {
            _qti3pItemToPassageRepository = passageRepository;
            _qti3pPassageRepository = qti3pPassageRepository;
        }

        public IQueryable<QTI3pItemToPassage> GetAll()
        {
            return _qti3pItemToPassageRepository.Select();
        }

        public QTI3pPassage GetAllQTI3pPassageById(int qTI3pPassageID)
        {
            var passage = _qti3pPassageRepository.Select().FirstOrDefault(m=>m.QTI3pPassageID == qTI3pPassageID);
            return passage;
        }
    }
}
