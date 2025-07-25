using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class TimingOptionService
    {
        private readonly IRepository<TimingOption> repository;

        public TimingOptionService(IRepository<TimingOption> repository)
        {
            this.repository = repository;
        }

        public IQueryable<TimingOption> Select()
        {
            return repository.Select();
        }
    }
}