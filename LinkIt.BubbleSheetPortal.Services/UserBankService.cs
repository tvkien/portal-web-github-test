using System;
using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.UserBank;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class UserBankService
    {
        private readonly IUserBankRepository repository;
        private readonly IBankRepository _bankRepository;

        public UserBankService(IUserBankRepository repository, IBankRepository bankRepository)
        {
            this.repository = repository;
            this._bankRepository = bankRepository;
        }

        public IQueryable<UserBank> GetUserBanksBySubjectAndUser(int subjectId, int userId)
        {
            return repository.Select().Where(x => x.SubjectId.Equals(subjectId) && x.UserId.Equals(userId));
        }

        public List<ListItem> GetUserBanksBySubjectId(int subjectId, int districtId, int userId, int userRoleId)
        {
            return repository.GetBanksBySubjectId(subjectId, districtId, userId, userRoleId);
        }
        public List<ListItem> GetUserBanks(int subjectId, int districtId, int userId, int userRoleId)
        {
            return repository.GetBanks(subjectId, districtId, userId, userRoleId);
        }

        public List<ListItem> ACTGetUserBanksBySubjectId(int subjectId, int districtId, int userId, int userRoleId)
        {
            return repository.ACTGetBanksBySubjectId(subjectId, districtId, userId, userRoleId);
        }

        public List<ListItem> ACTGetTestByBankId(int bankId, int districtId, int userId, int userRoleId)
        {
            return repository.ACTGetTestByBankId(bankId, districtId, userId, userRoleId);
        }
        public List<ListItem> GetBanksForItemSetSaveTest(int subjectId, int districtId, int userId, int userRole)
        {
            return repository.GetBanksForItemSetSaveTest(subjectId, districtId, userId, userRole);
        }
        public List<ListItem> GetUserBanksBySubjectName(SearchBankCriteria criteria)
        {
            return repository.GetUserBanksBySubjectName(criteria);
        }

        public List<UserBank> GetBanksBySubjectNamesAndGradeIDs(SearchBankAdvancedFilter criteria)
        {
            return repository.GetBanksBySubjectNamesAndGradeIDs(criteria);
        }

        public List<ListItem> SATGetUserBanksBySubjectId(int subjectId, int districtId, int userId, int userRoleId)
        {
            return repository.SATGetBanksBySubjectId(subjectId, districtId, userId, userRoleId);
        }

        public List<ListItem> SATGetTestByBankId(int bankId, int districtId, int userId, int userRoleId)
        {
            return repository.SATGetTestByBankId(bankId, districtId, userId, userRoleId);
        }

        public List<ListItem> GetFormBanksBySubjectId(FormBankCriteria criteria)
        {
            return repository.GetFormBanksBySubjectId(criteria);
        }

        public List<ListItem> GetFormBanksByMultipleSubjectIds(LoadBankByMultipleSubjectIdsCriteria criteria)
        {
            return repository.GetFormBanksByMultipleSubjectIds(criteria);
        }

        public int? GetSubjectIdOfABankByBankId(int bankId)
        {
            int? subjectid = _bankRepository.Select().Where(c => c.Id == bankId).Select(c => c.SubjectID).FirstOrDefault();
            return subjectid;
        }
    }
}
