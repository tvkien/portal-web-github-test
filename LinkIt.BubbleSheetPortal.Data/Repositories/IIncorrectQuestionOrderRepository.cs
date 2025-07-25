using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IIncorrectQuestionOrderRepository : IReadOnlyRepository<IncorrectQuestionOrder>
    {
        int GetStatusVirtualTest(int virtualTestId, bool? checkPointPossible);
    }
}
