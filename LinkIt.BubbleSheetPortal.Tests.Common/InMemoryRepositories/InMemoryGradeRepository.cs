//using System.Collections.Generic;
//using System.Linq;
//using Envoc.Core.Shared.Data;
//using LinkIt.BubbleSheetPortal.Data.Entities;
//using LinkIt.BubbleSheetPortal.Data.Repositories;
//using LinkIt.BubbleSheetPortal.Models;

//namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
//{
//    public class InMemoryGradeRepository : IReadOnlyRepository<Grade>, IGradeRepository
//    {
//        private List<Grade> table; 

//        public InMemoryGradeRepository()
//        {
//            table = AddGrades();
//        }

//        private List<Grade> AddGrades()
//        {
//            return new List<Grade>
//                       {
//                           new Grade{ Id = 1, Name = "One", Order = 1 },
//                           new Grade{ Id = 2, Name = "Two", Order = 2 },
//                           new Grade{ Id = 3, Name = "Three", Order = 3 },
//                           new Grade{ Id = 4, Name = "Four", Order = 4 },
//                           new Grade{ Id = 5, Name = "Five", Order = 5 }
//                       };
//        }

//        public IQueryable<Grade> Select()
//        {
//            return table.AsQueryable();
//        }

//        public List<GetGradeByUserIdResult> GetGradeByUserId(int userId)
//        {
//            throw new System.NotImplementedException();
//        }

//        public List<Grade> GetGradesByUserIdDistrictIdRoleId(int userId, int districtId, int roleId)
//        {
//            throw new System.NotImplementedException();
//        }

//        public IQueryable<Grade> GetStateSubjectGradeByStateAndSubject(string stateCode, string subject)
//        {
//            throw new System.NotImplementedException();
//        }

//        public List<Grade> GetGradesByStateID(int stateId)
//        {
//            return null;
//        }
//         public List<Grade> ACTGetGradesByUserIdDistrictIdRoleId(int userId, int districtId, int roleId)
//        {
//            throw new System.NotImplementedException();
//        }


//        public List<Grade> GetGradesForItemSetSaveTest(int userId, int districtId, int roleId)
//        {
//            throw new System.NotImplementedException();
//        }

//        public List<Grade> SATGetGradesByUserIdDistrictIdRoleId(int userId, int districtId, int roleId)
//        {
//            throw new System.NotImplementedException();
//        }
//    }
//}