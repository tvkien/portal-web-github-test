using LinkIt.BubbleSheetPortal.Data.Entities;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class PerformanceBandVirtualTestRepository : IPerformanceBandVirtualTestRepository
    {
        private readonly Table<PerformanceBandVirtualTest> _performanceBandVirtualTest;
        public PerformanceBandVirtualTestRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _performanceBandVirtualTest = UserDataContext.Get(connectionString).GetTable<PerformanceBandVirtualTest>();
        }

        public IEnumerable<PerformanceBandVirtualTest> Get(IEnumerable<int> virtualTestIDs)
        {
            var performanceBandVirtualTests = _performanceBandVirtualTest
                .Where(x => virtualTestIDs.Contains(x.VirtualTestID))
                .ToList();
            return performanceBandVirtualTests;
        }
    }
}
