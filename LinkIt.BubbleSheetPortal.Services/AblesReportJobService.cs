using System;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class AblesReportJobService
    {
        private readonly AblesReportJobRepository _repository;

        public AblesReportJobService(AblesReportJobRepository repository)
        {
            _repository = repository;
        }
        public IQueryable<AblesReportJob> Select()
        {
            return _repository.Select();
        }
        
        public void Save(AblesReportJob item)
        {
            _repository.Save(item);
        }
    }
}