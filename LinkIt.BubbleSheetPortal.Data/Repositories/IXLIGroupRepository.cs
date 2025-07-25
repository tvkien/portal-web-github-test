using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IXLIGroupRepository : IReadOnlyRepository<XLIGroup>
    {
        void Add(XLIGroup item);

        bool Update(XLIGroup item);

        bool Delete(int xliGroupId, int districtId);
    }
}
