using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class DistrictRosterUploadService
    {
        private readonly IReadOnlyRepository<DistrictRosterUpload> repository;

        public DistrictRosterUploadService(IReadOnlyRepository<DistrictRosterUpload> repository)
        {
            this.repository = repository;
        }

        public bool DistrictHasAutoUpdatingRosters(int districtId)
        {
            return repository.Select().Any(x => x.DistrictId.Equals(districtId));
        }
    }
}