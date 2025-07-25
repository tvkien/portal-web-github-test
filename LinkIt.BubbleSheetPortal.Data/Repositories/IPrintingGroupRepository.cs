using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IPrintingGroupRepository : IRepository<PrintingGroup>
    {
        List<int> GetListgroupIdsByUserId(int districtId, int userId);
    }
}
