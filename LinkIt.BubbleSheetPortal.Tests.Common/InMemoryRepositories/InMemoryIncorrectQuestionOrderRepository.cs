//using System.Collections.Generic;
//using System.Linq;
//using Envoc.Core.Shared.Data;
//using LinkIt.BubbleSheetPortal.Data.Repositories;
//using LinkIt.BubbleSheetPortal.Models;

//namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
//{
//    public class InMemoryIncorrectQuestionOrderRepository : IIncorrectQuestionOrderRepository
//    {
//        private readonly List<IncorrectQuestionOrder> table = new List<IncorrectQuestionOrder>();

//        public InMemoryIncorrectQuestionOrderRepository()
//        {
//            table = AddIncorrectQuestionOrders();
//        }

//        private List<IncorrectQuestionOrder> AddIncorrectQuestionOrders()
//        {
//            return new List<IncorrectQuestionOrder>
//                       {
//                           new IncorrectQuestionOrder{ VirtualTestId = 1, DistinctItems = 45, TotalQuestionCount = 23},
//                           new IncorrectQuestionOrder{ VirtualTestId = 2, DistinctItems = 123,TotalQuestionCount = 2}
//                       };
//        }

//        public IQueryable<IncorrectQuestionOrder> Select()
//        {
//            return table.AsQueryable();
//        }

//        public int GetStatusVirtualTest(int virtualTestId, bool? checkPointPossible)
//        {
//            return 0;
//        }
//    }
//}