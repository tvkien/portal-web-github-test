using System.Collections.Generic;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.TestResultRemover
{
    public class CustomAuthorTestRepository : ICustomAuthorTestRepository
    {
        private string _connectionString = null;

        public CustomAuthorTestRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _connectionString = connectionString;
        }

        public List<GetAuthorBySchoolAdminResult> GetAuthorBySchoolAdmin(int schoolAdminId)
        {
            var result = TestDataContext.Get(_connectionString)
                                      .GetAuthorBySchoolAdmin(schoolAdminId);
            return result.ToList();
        }

        public List<AuthorTestProcResult> GetAuthorTestByDistrictId(int districtId)
        {
            var result = TestDataContext.Get(_connectionString).AuthorTestProc(districtId);
            return result.ToList();
        }
    }
}
