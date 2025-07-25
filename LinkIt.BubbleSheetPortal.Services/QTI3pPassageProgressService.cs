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
    public class QTI3pPassageProgressService
    {
        private readonly IRepository<QTI3pPassageProgress> _repository;

        public QTI3pPassageProgressService(IRepository<QTI3pPassageProgress> repository)
        {
            _repository = repository;
        }

        public IQueryable<QTI3pPassageProgress> GetAll()
        {
            return _repository.Select();
        }
      
        public void Save(QTI3pPassageProgress item)
        {
            _repository.Save(item);
        }

    }
}
