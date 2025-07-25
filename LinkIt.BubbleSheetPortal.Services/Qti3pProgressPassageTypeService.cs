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
    public class Qti3pProgressPassageTypeService
    {
        private readonly IRepository<Qti3pProgressPassageType> _repository;

        public Qti3pProgressPassageTypeService(IRepository<Qti3pProgressPassageType> repository)
        {
            _repository = repository;
        }

        public IQueryable<Qti3pProgressPassageType> GetAll()
        {
            return _repository.Select();
        }

        public void Save(Qti3pProgressPassageType item)
        {
            _repository.Save(item);
        }

    }
}
