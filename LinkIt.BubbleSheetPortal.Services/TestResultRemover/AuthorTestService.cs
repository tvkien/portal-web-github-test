using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.TestResultRemover;

namespace LinkIt.BubbleSheetPortal.Services.TestResultRemover
{
    public class AuthorTestService
    {
        private readonly IReadOnlyRepository<AuthorTest> repository;

        public AuthorTestService(IReadOnlyRepository<AuthorTest> repository )
        {
            this.repository = repository;
        }

        public IQueryable<AuthorTest> GetAuthorTestByDistrictId(int districtId, bool isRegrader)
        {
            if(isRegrader)
            {
                return repository.Select().Where(x => x.DistrictId.Equals(districtId) && x.VirtualTestSourceId != 3);
            }
            return repository.Select().Where(x => x.DistrictId.Equals(districtId));
        } 
    }
}
