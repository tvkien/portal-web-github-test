using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IAutoGradingQueueRepository : IRepository<AutoGradingQueueData>
    {
        AutoGradingQueueData GetAutoGradingQueueByQTOnlineTestSessionID(int qTIOnlineTestSessionID);
        IEnumerable<AutoGradingQueueData> GetAutoGradingQueueByQTOnlineTestSessionID(IEnumerable<int> qTIOnlineTestSessionIds);
    }
}
