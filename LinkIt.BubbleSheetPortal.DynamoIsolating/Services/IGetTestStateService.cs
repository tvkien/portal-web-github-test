using LinkIt.BubbleSheetPortal.Models.Isolating;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.DynamoIsolating.Services
{
    public interface IGetTestStateService
    {
        IQueryable<IsolatingTestSessionAnswerDTO> GetTestState(int qtiOnlineTestSessionID);
    }
}
