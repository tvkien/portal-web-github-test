using LinkIt.BubbleSheetPortal.Models.Isolating;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.DynamoIsolating.Services
{
    public interface IPostAnswerLogService
    {
        IQueryable<IsolatingPostAnswerLogDTO> GetPostAnswerLogs(int qtiOnlineTestSessionID);
    }
}
