using System.Data.Linq;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class IncorrectQuestionOrderRepository : IIncorrectQuestionOrderRepository
    {
        private readonly Table<IncorrectQuestionOrderView> table;
        private readonly TestDataContext _testDataContext;

        public IncorrectQuestionOrderRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = TestDataContext.Get(connectionString).GetTable<IncorrectQuestionOrderView>();
            _testDataContext = TestDataContext.Get(connectionString);
        }

        public IQueryable<IncorrectQuestionOrder> Select()
        {
            return table.Select(x => new IncorrectQuestionOrder
                                         {
                                             VirtualTestId = x.VirtualTestID,
                                             TotalQuestionCount = x.Total_Question_Count.GetValueOrDefault(),
                                             DistinctItems = x.DistinctItems.GetValueOrDefault()
                                         });
        }

        public int GetStatusVirtualTest(int virtualTestId, bool? checkPointPossible)
        {
            bool chekPointPossibleLargeThan25 = false;
            if (checkPointPossible.HasValue) chekPointPossibleLargeThan25 = checkPointPossible.Value;

            var obj = _testDataContext.CheckValidVirtualTest(virtualTestId,chekPointPossibleLargeThan25);
            if (obj != null)
            {
                CheckValidVirtualTestResult first = null;
                foreach (CheckValidVirtualTestResult result in obj)
                {
                    first = result;
                    break;
                }
                return first.VirtualTestStatus.Value;
            }
            return 0;
        }
    }
}
