using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.TestResultRemover;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.TestResultRemover
{
    public class AuthorTestRepository  : IReadOnlyRepository<AuthorTest>
    {
        private readonly Table<AuthorTestView> _table;

        public AuthorTestRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _table = TestDataContext.Get(connectionString).GetTable<AuthorTestView>();
        }

        public IQueryable<AuthorTest> Select()
        {
            return _table.Select(x => new AuthorTest
                                    {
                                        UserId = x.UserID,
                                        UserName = x.UserName,
                                        NameFirst = x.NameFirst,
                                        NameLast = x.NameLast,
                                        DistrictId = x.DistrictID,
                                        ClassId = x.ClassID  ,
                                        VirtualTestSourceId = x.VirtualTestSourceID
                                    });
        } 
    }
}
