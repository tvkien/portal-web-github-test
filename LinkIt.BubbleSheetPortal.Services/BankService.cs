using System;
using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class BankService
    {
        private readonly IBankRepository _repository;
        private readonly IRepository<BankDistrict> _bankDistrictRepository;

        public BankService(IBankRepository repository, IRepository<BankDistrict> bankDistrictRepository)
        {
            this._repository = repository;
            this._bankDistrictRepository = bankDistrictRepository;
        }

        public IQueryable<Bank> GetBanksBySubject(int subjectId)
        {
            return _repository.Select().Where(x => x.SubjectID.Equals(subjectId));
        }

        public Bank GetBankById(int bankId)
        {
            return _repository.Select().FirstOrDefault(x => x.Id == bankId);
        }

        public IQueryable<Bank> Select()
        {
            return _repository.Select();
        }

        public void Save(Bank bank)
        {
            _repository.Save(bank);
        }

        public BankProperty GetBankProperty(int bankid)
        {
            return _repository.GetBankProperty(bankid);
        }

        public bool UpdateBank(string bankName, int subjectId, int bankId)
        {
            var bank = _repository.Select().FirstOrDefault(o => o.Id == bankId);
            if (bank != null)
            {
                bank.Name = bankName;
                bank.SubjectID = subjectId;
                bank.UpdatedDate = DateTime.UtcNow;
                _repository.Save(bank);
            }
            return false;
        }

        public bool CanDeleteBankByID(int bankId)
        {
            return _repository.CanDeleteBankByID(bankId);
        }

        public void DeleteBankByID(int bankId)
        {
            _repository.DeleteBankByID(bankId);
        }

        public bool CheckIfBankLocked(int districtId, int bankId)
        {
            return _bankDistrictRepository.Select()
                .Any(bd => bd.DistrictId == districtId && bd.BankId == bankId && bd.BankDistrictAccessId == (int)BankAccessEnum.Restricted);
        }

        public PrintTestOptions CheckPermissionPrintQuestionAndAnswerkey(int bankId, int userId, int districtId)
        {
            return _repository.CheckPermissionPrintQuestionAndAnswerKey(bankId, userId, districtId);
        }
        public bool UpdateBankArchive(int bankId, bool archived)
        {
            var bank = _repository.Select().FirstOrDefault(o => o.Id == bankId);
            if (bank != null)
            {
                bank.Archived = archived;
                bank.UpdatedDate = DateTime.UtcNow;
                _repository.Save(bank);
            }
            return false;
        }

        public List<BankOrder> GetBankOrders(int districtId)
        {
            return _repository.GetBankOrders(districtId);
        }

        public List<ListItem> SortBanks(List<ListItem> banks, List<BankOrder> bankOrders)
        {
            var maxOrder = bankOrders.Max(x => x.Order) + 100;

            var query = from bak in banks
                        join bakOder in bankOrders on bak.Id equals bakOder.BankId into ps
                        from p in ps.DefaultIfEmpty()
                        select new { bak.Id, bak.Name, Order = p == null ? maxOrder : p.Order };

            var list = query.OrderBy(x => x.Order).ThenBy(x => x.Name).ToList();

            var result = list.Select(o => new ListItem
            {
                Id = o.Id,
                Name = o.Name
            }).ToList();

            return result;
        }

        public bool UpdateBankName(string bankName, int bankId)
        {
            var bank = _repository.Select().FirstOrDefault(o => o.Id == bankId);
            if (bank != null)
            {
                bank.Name = bankName;
                bank.UpdatedDate = DateTime.UtcNow;
                _repository.Save(bank);
            }
            return false;
        }
    }
}
