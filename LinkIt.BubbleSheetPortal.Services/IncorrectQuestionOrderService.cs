using System.Linq;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class IncorrectQuestionOrderService
    {
        private readonly IIncorrectQuestionOrderRepository repository;

        public IncorrectQuestionOrderService(IIncorrectQuestionOrderRepository repository)
        {
            this.repository = repository;
        }

        public int CheckIfTestRequiresCorrection(int testId, bool? pointPossibleLargeThan25)
        {
            return repository.GetStatusVirtualTest(testId, pointPossibleLargeThan25);
        }

        public bool CheckIfTestRequiresCorrectionOld(int testId)
        {
            return repository.Select().Any(x => x.VirtualTestId.Equals(testId));
        }
    }
}
