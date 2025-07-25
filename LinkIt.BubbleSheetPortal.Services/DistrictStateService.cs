using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class DistrictStateService
    {
         private readonly IReadOnlyRepository<DistrictState> repository;

         public DistrictStateService(IReadOnlyRepository<DistrictState> repository)
        {
            this.repository = repository;
        }

        public IQueryable<DistrictState> GetDistricts()
        {
            return repository.Select().OrderBy(o=>o.DistrictNameCustom);
        }
    }
}
