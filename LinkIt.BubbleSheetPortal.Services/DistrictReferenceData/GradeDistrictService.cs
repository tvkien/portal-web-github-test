using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.DistrictReferenceData;

namespace LinkIt.BubbleSheetPortal.Services.DistrictReferenceData
{
    public class GradeDistrictService
    {
        private readonly IReadOnlyRepository<GradeDistrict> repository;

        public GradeDistrictService(IReadOnlyRepository<GradeDistrict> repository)
        {
            this.repository = repository;
        }

        public IQueryable<GradeDistrict> GetGradeByDistrictID(int districtId)
        {
            return repository.Select().Where(x => x.DistrictID.Equals(districtId)).OrderBy(x => x.Order);
        }
    }
}