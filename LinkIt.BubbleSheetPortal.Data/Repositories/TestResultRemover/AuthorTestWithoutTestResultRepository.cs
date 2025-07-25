using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.TestResultRemover;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.TestResultRemover
{
    public class AuthorTestWithoutTestResultRepository : IReadOnlyRepository<AuthorTestWithoutTestResult>
    {
        private readonly Table<AuthorTestWithoutTestResultView> _table;

        public AuthorTestWithoutTestResultRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _table = TestDataContext.Get(connectionString).GetTable<AuthorTestWithoutTestResultView>();
        }

        public IQueryable<AuthorTestWithoutTestResult> Select()
        {
            return _table.Select(x => new AuthorTestWithoutTestResult()
                                    {
                                        UserId = x.UserID,
                                        UserName = x.UserName,
                                        NameFirst = x.NameFirst,
                                        NameLast = x.NameLast,
                                        DistrictId = x.DistrictID ?? 0
                                    });
        }
    }
}
