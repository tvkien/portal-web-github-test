using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class ClassDistrictTermService
    {
        private readonly IReadOnlyRepository<ClassDistrictTerm> repository;

        public ClassDistrictTermService(IReadOnlyRepository<ClassDistrictTerm> repository)
        {
            this.repository = repository;
        }

        public IQueryable<ClassDistrictTerm> GetClassDistrictTermBySchoolId(int schoolId)
        {
            return repository.Select().Where(x => x.SchoolId.Equals(schoolId));
        }
    }
}