using LinkIt.BubbleSheetPortal.Models.Isolating;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.DynamoIsolating.Services
{
    public interface IGetOnlineTestSessionAnswerService
    {
        IQueryable<OnlineTestSessionAnswerDTO> GetOnlineTestSessionAnswer(string qtiOnlineTestSessionIDs);
    }
}
