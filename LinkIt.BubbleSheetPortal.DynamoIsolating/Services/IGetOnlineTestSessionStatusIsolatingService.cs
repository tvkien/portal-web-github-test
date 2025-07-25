using LinkIt.BubbleSheetPortal.Models.Isolating;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.DynamoIsolating.Services
{
    public interface IGetOnlineTestSessionStatusIsolatingService
    {
        IQueryable<OnlineTestSessionDTO> GetOnlineTestSessionStatusIsolating(string qtiTestClassAssignmentIDs);
        List<int> GetSectionSubmiteds(int qtiOnlineTestSessionId);
    }
}
