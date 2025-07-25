using System;
using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryUserBankRepository : IUserBankRepository
    {
        private readonly List<UserBank> table = new List<UserBank>();

        public InMemoryUserBankRepository()
        {
            table = AddBanks();
        }

        private List<UserBank> AddBanks()
        {
            return new List<UserBank>
                       {
                           new UserBank{Id = 1, BankName = "Bank1", SubjectId = 5,UserId = 10},
                           new UserBank{Id = 2, BankName = "Bank2", SubjectId = 5,UserId = 10},
                           new UserBank{Id = 3, BankName = "Bank3", SubjectId = 5,UserId = 12},
                           new UserBank{Id = 4, BankName = "Bank4", SubjectId = 7,UserId = 15}
                       };
        }

        public IQueryable<UserBank> Select()
        {
            return table.AsQueryable();
        }

        public List<ListItem> GetBanksBySubjectId(int subjectId, int districtId, int userId, int userRole)
        {
            return  new List<ListItem>();
        }
        public List<ListItem> GetBanks(int subjectId, int districtId, int userId, int userRole)
        {
            return new List<ListItem>();
        }

        public List<ListItem> ACTGetBanksBySubjectId(int subjectId, int districtId, int userId, int userRole)
        {
            return null;
        }

        public List<ListItem> ACTGetTestByBankId(int bankId, int districtId, int userId, int userRole)
        {
            return null;
        }
        public List<ListItem> GetBanksForItemSetSaveTest(int subjectId, int districtId, int userId, int userRole)
        {
            return null;
        }

        public List<ListItem> GetUserBanksBySubjectName(SearchBankCriteria criteria)
        {
            return null;
        }

        public List<ListItem> SATGetBanksBySubjectId(int subjectId, int districtId, int userId, int userRole)
        {
            return null;
        }

        public List<ListItem> SATGetTestByBankId(int bankId, int districtId, int userId, int userRole)
        {
            return null;
        }

        public List<ListItem> GetFormBanksBySubjectId(FormBankCriteria criteria)
        {
            return null;
        }
    }
}
