using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IXLIGroupUserRepository : IReadOnlyRepository<XLIGroupUser>
    {
        void RemoveUsersFromGroup(List<int> userIds);
    }
}
