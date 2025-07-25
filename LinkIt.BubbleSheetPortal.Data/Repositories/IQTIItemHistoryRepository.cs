using System.Collections.Generic;
using System.Linq;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Common.Enum;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IQTIItemHistoryRepository : IReadOnlyRepository<QTIItemHistory>
    {
        void Save(QTIItemHistory item);
    }
}
