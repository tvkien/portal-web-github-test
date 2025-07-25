using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IXLIIconAccessRepository : IReadOnlyRepository<XLIIconAccess>
    {
        List<XliArea> GetAllXliAreasByUserId(int userId, int districtId, int userRoleId);
    }
}
