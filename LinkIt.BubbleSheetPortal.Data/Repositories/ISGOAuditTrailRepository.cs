using System;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface ISGOAuditTrailRepository : IRepository<SGOAuditTrailData>
    {
        IQueryable<SGOAuditTrailSearchItem> GetAuditTrailBySGOID(int sgoID);
    }
}
