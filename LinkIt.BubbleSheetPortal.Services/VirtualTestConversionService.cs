using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class VirtualTestConversionService
    {
        private readonly IReadOnlyRepository<VirtualTestConversion> repository;

        public VirtualTestConversionService(IReadOnlyRepository<VirtualTestConversion> repository)
        {
            this.repository = repository;
        }

        public IQueryable<VirtualTestConversion> GetByVirtualTestId(int virtualTestId)
        {
            return repository.Select().Where(x => x.VirtualTestID == virtualTestId);
        }
    }
}