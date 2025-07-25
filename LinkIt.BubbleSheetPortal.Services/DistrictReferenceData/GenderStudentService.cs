using System.Linq;
using LinkIt.BubbleSheetPortal.Models.DistrictReferenceData;
using Envoc.Core.Shared.Data;

namespace LinkIt.BubbleSheetPortal.Services.DistrictReferenceData
{
    public class GenderStudentService
    {
        private readonly IReadOnlyRepository<GenderStudent> repository;

        public GenderStudentService(IReadOnlyRepository<GenderStudent> repository)
        {
            this.repository = repository;
        }

        public IQueryable<GenderStudent> GetGenderByDistrictID(int districtID)
        {
            return repository.Select().Where(x => x.DistrictID.Equals(districtID)).OrderBy(x => x.Name).ThenBy(x => x.Code);
        }
    }
}