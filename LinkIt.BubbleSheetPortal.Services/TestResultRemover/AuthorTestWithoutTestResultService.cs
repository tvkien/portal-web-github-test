using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Data.Repositories.TestResultRemover;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.TestResultRemover;

namespace LinkIt.BubbleSheetPortal.Services.TestResultRemover
{
    public class AuthorTestWithoutTestResultService
    {
        private readonly IReadOnlyRepository<AuthorTestWithoutTestResult> repository;

        public AuthorTestWithoutTestResultService(IReadOnlyRepository<AuthorTestWithoutTestResult> repository )
            
        {
            this.repository = repository;
        }

        public IQueryable<AuthorTestWithoutTestResult> GetAuthorTestWithoutTestResult(int districtId, int roleId, int userId)
        {
            if (roleId == (int)Permissions.Publisher || roleId == (int)Permissions.DistrictAdmin
                || roleId == (int)Permissions.NetworkAdmin)
            {
                return repository.Select().Where(x => x.DistrictId.Equals(districtId))
                    .OrderBy(o => o.NameLast)
                    .ThenBy(o => o.NameLast)
                    .ThenBy(o => o.UserName)
                    .Distinct();
            }
            else if (roleId == (int)Permissions.Teacher)
            {
                return repository.Select().Where(x => x.DistrictId.Equals(districtId) && x.UserId.Equals(userId));
            }
            return null;
        } 
    }
}
