using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IVirtualSectionRepository : IRepository<VirtualSection>
    {
        IEnumerable<VirtualSection> GetPartialRetakeSections(int virtualTestId, string studentIds, string guid);
    }
}
