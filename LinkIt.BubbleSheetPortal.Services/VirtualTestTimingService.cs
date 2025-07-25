using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class VirtualTestTimingService
    {
        private readonly IRepository<VirtualTestTiming> repository;

        public VirtualTestTimingService(IRepository<VirtualTestTiming> repository)
        {
            this.repository = repository;
        }

        public IQueryable<VirtualTestTiming> Select()
        {
            return repository.Select();
        }
    }
}