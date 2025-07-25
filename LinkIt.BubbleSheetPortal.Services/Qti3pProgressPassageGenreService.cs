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
    public class Qti3pProgressPassageGenreService
    {
        private readonly IRepository<Qti3pProgressPassageGenre> _repository;

        public Qti3pProgressPassageGenreService(IRepository<Qti3pProgressPassageGenre> repository)
        {
            _repository = repository;
        }

        public IQueryable<Qti3pProgressPassageGenre> GetAll()
        {
            return _repository.Select();
        }

        public void Save(Qti3pProgressPassageGenre item)
        {
            _repository.Save(item);
        }

    }
}
