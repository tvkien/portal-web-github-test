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
    public class QTI3pTextTypeService 
    {
        private readonly IReadOnlyRepository<QTI3pTextType> _repository;

        public QTI3pTextTypeService(IReadOnlyRepository<QTI3pTextType> repository)
        {
            _repository = repository;
        }

        public IQueryable<QTI3pTextType> GetAll()
        {
            return _repository.Select();
        }
    }
}
