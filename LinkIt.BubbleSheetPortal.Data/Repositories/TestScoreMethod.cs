using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class TestScoreMethodRepository : IReadOnlyRepository<TestScoreMethod>
    {
        private readonly Table<TestScoreMethodEntity> table;

        public TestScoreMethodRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = TestDataContext.Get(connectionString).GetTable<TestScoreMethodEntity>();
        }

        public IQueryable<TestScoreMethod> Select()
        {
            return table.Select(x => new TestScoreMethod
                {
                   TestScoreMethodId = x.TestScoreMethodID,
                   Name = x.Name
                });
        }
    }
}