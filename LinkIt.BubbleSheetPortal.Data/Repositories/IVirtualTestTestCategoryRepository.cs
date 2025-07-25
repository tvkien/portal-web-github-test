using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IVirtualTestTestCategoryRepository : IRepository<VirtualTestTestCategoryData>
    {
        void InsertItems(List<VirtualTestTestCategoryData> items);
        void Update(List<VirtualTestTestCategoryData> items);
    }
}