//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using Envoc.Core.Shared.Data;
//using LinkIt.BubbleSheetPortal.Data.Repositories;
//using LinkIt.BubbleSheetPortal.Models;

//namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
//{
//    public class InMemoryBankRepository : IBankRepository
//    {
//        private readonly List<Bank> table = new List<Bank>();

//        public InMemoryBankRepository()
//        {
//            table = AddBanks();
//        }

//        private List<Bank> AddBanks()
//        {
//            return new List<Bank>
//                       {
//                           new Bank{Id = 1, Name = "Bank1", SubjectID = 5},
//                           new Bank{Id = 2, Name = "Bank2", SubjectID = 4},
//                           new Bank{Id = 3, Name = "Bank3", SubjectID = 5},
//                           new Bank{Id = 4, Name = "Bank4", SubjectID = 7}
//                       };
//        }

//        public IQueryable<Bank> Select()
//        {
//            return table.AsQueryable();
//        }

//        public void Save(Bank item)
//        {
//            throw new System.NotImplementedException();
//        }

//        public void Delete(Bank item)
//        {
//            throw new System.NotImplementedException();
//        }

//        public BankProperty GetBankProperty(int bankId)
//        {
//            return null;
//        }

//        public bool CanDeleteBankByID(int bankId)
//        {
//            return false;
//        }

//        public void DeleteBankByID(int bankId)
//        {
           
//        }
//        public bool CheckBankLock(int bankId, int userId, int districtId)
//        {
//            return false;
//        }

//        public PrintTestOptions CheckPermissionPrintQuestionAndAnswerKey(int bankId, int userId, int districtId)
//        {
//            throw new HttpUnhandledException();
//        }
//    }
//}