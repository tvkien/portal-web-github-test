using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IAnswerRepository : IRepository<Answer>
    {
        bool RegradeTestByTestResultId(int testResultId);
        bool PurgeTest(int virtualTestId);
    }
}
