using System.Collections.Generic;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.TestResultRemover
{
    public interface ICustomAuthorTestRepository
    {
        List<GetAuthorBySchoolAdminResult> GetAuthorBySchoolAdmin(int schoolAdminId);
        List<AuthorTestProcResult> GetAuthorTestByDistrictId(int districtId);
    }
}
