using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Common;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class ListBankService
    {
        private readonly IReadOnlyRepository<ListBank> repository;
        private readonly IManageTestRepository _manageTestRepository;
        private readonly IBankRepository _bankRepository;

        public ListBankService(IReadOnlyRepository<ListBank> repository, IManageTestRepository manageTestRepository, IBankRepository bankRepository)
        {
            this.repository = repository;
            _manageTestRepository = manageTestRepository;
            _bankRepository = bankRepository;
        }

        public IQueryable<ListBank> GetBankById(int districtId)
        {
            return repository.Select().Where(o =>(  o.CreateBankDistrictId == districtId) ||  o.DistrictId == districtId);
        }

        public IQueryable<ListBank> GetBankByDistrictIdAndSubjectId(int districtId, int subjectId, int bankAccessId, int gradeId)
        {
            //var query = repository.Select().Where(o => (o.BankDistrictId == 0 && o.CreateBankDistrictId == districtId) || (o.BankDistrictId > 0 && o.DistrictId == districtId));
            var query = repository.Select().Where(o =>   o.CreateBankDistrictId == districtId  ||  o.DistrictId == districtId);
            if (subjectId > 0)
            {
                query = query.Where(o => o.SubjectId == subjectId);
            }
            if (gradeId > 0)
            {
                query = query.Where(o => o.GradeId == gradeId);
            }
            if (bankAccessId > 0)
            {
                query = query.Where(o => o.BankDistrictAccessId == bankAccessId || (o.BankAccessId == bankAccessId && o.BankDistrictAccessId == 0));
            }

            return query;
        }

        public IQueryable<ListBank> GetBankByDistrictIdAndSubjectName(int districtId, List<int> subjectIds, int bankAccessId, int gradeId, int userId = 0)
        {
            //var query = repository.Select().Where(o => (o.BankDistrictId == 0 && o.CreateBankDistrictId == districtId) || (o.BankDistrictId > 0 && o.DistrictId == districtId));
            var query = repository.Select().Where(o => o.DistrictId == districtId);//TODO: function Lock/Unlock only show bank on BankDistrict
            if (subjectIds != null && subjectIds.Count > 0)
            {
                query = query.Where(o => subjectIds.Contains(o.SubjectId));
            }
            if (gradeId > 0)
            {
                query = query.Where(o => o.GradeId == gradeId);
            }
            if (bankAccessId > 0)
            {
                query = query.Where(o => o.BankDistrictAccessId == bankAccessId || (o.BankAccessId == bankAccessId && o.BankDistrictAccessId == 0));
            }

            // user bank access
            if(userId > 0)
            {
                var userBankAccess = _manageTestRepository.GetUserBankAccess(new UserBankAccessCriteriaDTO {
                    UserId = userId,
                    DistrictId = districtId,
                    BankAccessId = bankAccessId,
                    GradeIds = new List<int> { gradeId },
                    SubjectIds = subjectIds
                });

                // include
                var includeBanks = repository.Select().Where(m => userBankAccess.BankIncludeIds.Contains(m.BankId)).Select(m => new ListBank
                {
                    BankId = m.BankId,
                    Name = m.Name,
                    BankAccessId = m.BankAccessId,
                    CreateByUserId = m.CreateByUserId,
                    Archived = m.Archived,
                    BankDistrictId = m.BankDistrictId,
                    BankDistrictAccessId = m.BankDistrictAccessId,
                    CreateBankDistrictId = m.CreateBankDistrictId,
                    DistrictId = districtId,
                    GradeId = m.GradeId,
                    GradeName = m.GradeName,
                    Hide = m.Hide,
                    SubjectId = m.SubjectId,
                    SubjectName = m.SubjectName
                }).AsQueryable();

                query = query.Union(includeBanks);
                query = query.AsEnumerable().DistinctBy(m => m.BankId).AsQueryable();

                // exclude
                query = query.Where(m => !userBankAccess.BankExcludeIds.Contains(m.BankId));
            }

            return query;
        }

        public ListBank GetBankDistrictByDistrictIdAndBankId(int districtId, int bankId)
        {
            //var query = repository.Select().Where(o => (o.BankDistrictId == 0 && o.CreateBankDistrictId == districtId) || (o.BankDistrictId > 0 && o.DistrictId == districtId));
            var query = repository.Select().Where(o =>  o.CreateBankDistrictId == districtId ||  o.DistrictId == districtId);
            return query.FirstOrDefault(o => o.BankId == bankId);

        }

        public ListBank GetBankDistrict(int districtId, int bankId)
        {
            var query = repository.Select().Where(x => x.DistrictId == districtId && x.BankId == bankId);
            return query.FirstOrDefault(o => o.BankId == bankId);
        }
    }
}
