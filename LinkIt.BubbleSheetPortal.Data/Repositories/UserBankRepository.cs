using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models.DTOs.UserBank;
using System;
using LinkIt.BubbleSheetPortal.Data.Extensions;
using LinkIt.BubbleSheetPortal.Models.DTOs.UserDefinedTypesDto;
using System.Data;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class UserBankRepository : IUserBankRepository
    {
        private readonly Table<UserBankView> table;
        private readonly TestDataContext _testDataContext;
        private DataLockerContextDataContext _dataLockerContext;
        private ManageTestRepository _manageTestRepository;
        public UserBankRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = UserDataContext.Get(connectionString).GetTable<UserBankView>();
            _testDataContext = new TestDataContext(connectionString);
            _dataLockerContext = new DataLockerContextDataContext(connectionString);
            _manageTestRepository = new ManageTestRepository(conn);
        }

        public IQueryable<UserBank> Select()
        {
            return table.Select(x => new UserBank
            {
                Id = x.BankID,
                BankName = x.BankName,
                SubjectId = x.SubjectID,
                UserId = x.UserID
            });
        }
  
        public List<ListItem> GetBanksBySubjectId(int subjectId, int districtId, int userId, int userRole)
        {
            var tmp = _testDataContext.ProcBank(subjectId, districtId, userId, userRole);
            if (tmp != null)
                return tmp.Select(o => new ListItem
                {
                    Id = o.bankId,
                    Name = o.name
                }).ToList();
            return new List<ListItem>();
        }
        public List<ListItem> GetBanks(int subjectId, int districtId, int userId, int userRole)
        {
            var tmp = _testDataContext.GetBanks(subjectId, districtId, userId, userRole);
            if (tmp != null)
                return tmp.Select(o => new ListItem
                {
                    Id = o.bankId,
                    Name = o.name
                }).ToList();
            return new List<ListItem>();
        }

        public List<ListItem> ACTGetBanksBySubjectId(int subjectId, int districtId, int userId, int userRole)
        {
            var tmp = _testDataContext.ACTProcBank(subjectId, districtId, userId, userRole);
            if (tmp != null)
                return tmp.Select(o => new ListItem
                {
                    Id = o.bankId,
                    Name = o.name
                }).ToList();
            return new List<ListItem>();
        }

        public List<ListItem> ACTGetTestByBankId(int bankId, int districtId, int userId, int userRole)
        {
            var tmp = _testDataContext.ACTProcVirtualTest(bankId, districtId, userId, userRole);
            if (tmp != null)
                return tmp.Select(o => new ListItem
                {
                    Id = o.VirtualTestID,
                    Name = o.Name
                }).ToList();


            return new List<ListItem>();
        }
        public List<ListItem> GetBanksForItemSetSaveTest(int subjectId, int districtId, int userId, int userRole)
        {
            var tmp = _testDataContext.GetBanksForItemSetSaveTest(subjectId, districtId, userId, userRole);
            if (tmp != null)
                return tmp.Select(o => new ListItem
                {
                    Id = o.bankId,
                    Name = o.name
                }).ToList();
            return new List<ListItem>();
        }

        public List<ListItem> GetUserBanksBySubjectName(SearchBankCriteria criteria)
        {
            var result = new List<ListItem>();

            var tmp = _testDataContext.GetBanksBySubjectName(null, criteria.SubjectName, criteria.DistrictId, criteria.GradeId, criteria.UserId, criteria.UserRole);
            if (tmp != null)
                result = tmp.Select(o => new ListItem
                {
                    Id = o.BankID,
                    Name = o.Name
                }).ToList();

            var userBankAccess = _manageTestRepository.GetUserBankAccess(new UserBankAccessCriteriaDTO
            {
                UserId = criteria.UserId,
                DistrictId = criteria.DistrictId,
                GradeIds = new List<int> { criteria.GradeId ?? 0 },
                SubjectNames = new List<string> { criteria.SubjectName }
            });

            // include
            var includeBanks = _testDataContext.BankEntities
                .Where(m => userBankAccess.BankIncludeIds.Contains(m.BankID))
                .Select(m => new ListItem
                {
                    Id = m.BankID,
                    Name = m.Name
                }).ToList();

            result.AddRange(includeBanks);
            result = result.DistinctBy(m => m.Id).ToList();

            // exclude
            result.RemoveAll(m => userBankAccess.BankExcludeIds.Contains(m.Id));

            return result.DistinctBy(m => m.Id).ToList();
        }

        public List<GetBanksBySubjectNameResult> GetBanksBySubjectNamesAndGradeIDs(IEnumerable<string> subjectNames, IEnumerable<int> gradeIds, int districtId, int userId, int userRole)
        {
            var parameters = new List<(string, string, SqlDbType, object, ParameterDirection)>
            {
                ("SubjectNameList", "StringList", SqlDbType.Structured, subjectNames.ConvertToStringList().ToDataTable(), ParameterDirection.Input),
                ("GradeIDList", "IntegerList", SqlDbType.Structured, gradeIds.ConvertToIntegerList().ToDataTable(), ParameterDirection.Input),
                ("DistrictId", "", SqlDbType.Int, districtId, ParameterDirection.Input),
                ("UserId", "", SqlDbType.Int, userId, ParameterDirection.Input),
                ("UserRoleId", "", SqlDbType.Int, userRole, ParameterDirection.Input)
            };

            return _testDataContext.Query<GetBanksBySubjectNameResult>(new SqlParameterRequest()
            {
                StoredName = "GetBanksBySubjectNamesAndGradeIDs",
                Parameters = parameters
            }, out _);
        }

        public List<UserBank> GetBanksBySubjectNamesAndGradeIDs(SearchBankAdvancedFilter criteria)
        {
            var result = new List<UserBank>();

            var tmp = GetBanksBySubjectNamesAndGradeIDs(criteria.SubjectNames, criteria.GradeIds, criteria.DistrictId, criteria.UserId, criteria.UserRole);
            if (tmp != null)
                result = tmp.Select(o => new UserBank
                {
                    Id = o.BankID,
                    BankName = o.Name,
                    SubjectId = o.SubjectID
                }).ToList();

            var userBankAccess = _manageTestRepository.GetUserBankAccess(new UserBankAccessCriteriaDTO
            {
                UserId = criteria.UserId,
                DistrictId = criteria.DistrictId,
                GradeIds = criteria.GradeIds.ToList(),
                SubjectNames = criteria.SubjectNames.ToList()
            });

            // include
            var includeBanks = _testDataContext.BankEntities
                .Where(m => userBankAccess.BankIncludeIds.Contains(m.BankID))
                .Select(m => new UserBank
                {
                    Id = m.BankID,
                    BankName = m.Name,
                    SubjectId = m.SubjectID,
                }).ToList();

            result.AddRange(includeBanks);
            result = result.DistinctBy(m => m.Id).ToList();

            // exclude
            result.RemoveAll(m => userBankAccess.BankExcludeIds.Contains(m.Id));

            return result.DistinctBy(m => m.Id).ToList();
        }

        public List<ListItem> SATGetBanksBySubjectId(int subjectId, int districtId, int userId, int userRole)
        {
            var tmp = _testDataContext.SATProcBank(subjectId, districtId, userId, userRole);
            if (tmp != null)
                return tmp.Select(o => new ListItem
                {
                    Id = o.BankID,
                    Name = o.Name,

                }).ToList();
            return new List<ListItem>();
        }

        public List<ListItem> SATGetTestByBankId(int bankId, int districtId, int userId, int userRole)
        {
            var tmp = _testDataContext.SATProcVirtualTest(bankId, districtId, userId, userRole);
            if (tmp != null)
                return tmp.Select(o => new ListItem
                {
                    Id = o.VirtualTestID,
                    Name = o.Name
                }).ToList();
            return new List<ListItem>();
        }

        public List<ListItem> GetFormBanksBySubjectId(FormBankCriteria criteria)
        {
            var result = new List<ListItem>();
            var tmp = _dataLockerContext.ProcBankFormBank(criteria.SubjectId, criteria.DistrictId, criteria.UserId, criteria.UserRole, criteria.IsFromMultiDate ?? false, criteria.UsingMultiDate);

            if (tmp != null)
                result = tmp.Select(o => new ListItem
                {
                    Id = o.BankID,
                    Name = o.Name
                }).ToList();

            var userBankAccess = _manageTestRepository.GetUserBankAccess(new UserBankAccessCriteriaDTO
            {
                UserId = criteria.UserId,
                DistrictId = criteria.DistrictId,
                SubjectId = criteria.SubjectId,
                SubjectNames = new List<string> { criteria.SubjectName },
                GradeIds = new List<int> { criteria.GradeId ?? 0 }
            });

            // include
            var includeBanks = _testDataContext.BankEntities.Where(m => userBankAccess.BankIncludeIds.Contains(m.BankID)).Select(o => new ListItem
            {
                Id = o.BankID,
                Name = o.Name
            }).ToList();

            result.AddRange(includeBanks);
            // exclude
            result = result.Where(m => !userBankAccess.BankExcludeIds.Contains(m.Id)).ToList();

            return result.AsEnumerable().DistinctBy(m => m.Id).ToList();
        }


        public List<ListItem> GetFormBanksByMultipleSubjectIds(LoadBankByMultipleSubjectIdsCriteria criteria)
        {
            var result = new List<ListItem>();
            var formBanks = _dataLockerContext.GetFormBanksByMultipleSubjectIds(criteria.SubjectIds, criteria.DistrictId, criteria.UserId, criteria.UserRole, criteria.IsFromMultiDate ?? false, criteria.UsingMultiDate);

            if (formBanks != null)
                result = formBanks.Select(o => new ListItem
                {
                    Id = o.BankID,
                    Name = o.Name
                }).ToList();

            var subjectIds = criteria.SubjectIds
                .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(c => int.TryParse(c, out int _))
                .Select(c => int.Parse(c))
                .ToList();
            var userBankAccess = _manageTestRepository.GetUserBankAccess(new UserBankAccessCriteriaDTO
            {
                UserId = criteria.UserId,
                DistrictId = criteria.DistrictId,
                SubjectIds = subjectIds,
                SubjectNames = new List<string> { criteria.SubjectName },
                GradeIds = new List<int> { criteria.GradeId ?? 0 }
            });

            // include
            var includeBanks = _testDataContext.BankEntities.Where(m => userBankAccess.BankIncludeIds.Contains(m.BankID)).Select(o => new ListItem
            {
                Id = o.BankID,
                Name = o.Name
            }).ToList();

            result.AddRange(includeBanks);
            // exclude
            result = result.Where(m => !userBankAccess.BankExcludeIds.Contains(m.Id)).ToList();

            return result.AsEnumerable().DistinctBy(m => m.Id).ToList();
        }
    }
}
