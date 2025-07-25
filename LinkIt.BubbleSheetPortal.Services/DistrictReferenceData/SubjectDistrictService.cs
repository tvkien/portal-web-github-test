using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.DistrictReferenceData;

namespace LinkIt.BubbleSheetPortal.Services.DistrictReferenceData
{
    public class SubjectDistrictService
    {
        private readonly IReadOnlyRepository<SubjectDistrict> repository;

        public SubjectDistrictService(IReadOnlyRepository<SubjectDistrict> repository)
        {
            this.repository = repository;
        }

        public IQueryable<SubjectDistrict> GetSubjectByDistrictID(int districtID)
        {
            return repository.Select().Where(x => x.DistrictID.Equals(districtID)).OrderBy(x=>x.Name).ThenBy(x=>x.ShortName);
        }
    }
}