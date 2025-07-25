using LinkIt.BubbleSheetPortal.Data.Entities;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IPerformanceBandVirtualTestRepository
    {
        IEnumerable<PerformanceBandVirtualTest> Get(IEnumerable<int> virtualTestIDs);
    }
}
