using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class MasterStandardResourceService
    {
        private readonly IReadOnlyRepository<MasterStandardResource> repository;

        public MasterStandardResourceService(IReadOnlyRepository<MasterStandardResource> repository)
        {
            this.repository = repository;
        }

        public IQueryable<MasterStandardResource> GetMasterStandards()
        {
            return repository.Select();
        }


    }
}
