using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class BankDistrictService
    {
         private readonly IRepository<BankDistrict> repository;
         private readonly IReadOnlyRepository<District> _districtRepository;
        private readonly IRepository<Bank> bankRepository;
        private readonly IQTITestClassAssignmentReadOnlyRepository _qTITestClassAssignmentRepository;

        public BankDistrictService(IRepository<BankDistrict> repository,
            IReadOnlyRepository<District> districtRepository,
            IQTITestClassAssignmentReadOnlyRepository qTITestClassAssignmentRepository, 
            IRepository<Bank> bankRepository)
        {
            this.repository = repository;
            this._districtRepository = districtRepository;
            this._qTITestClassAssignmentRepository = qTITestClassAssignmentRepository;
            this.bankRepository = bankRepository;
        }

        public BankDistrict GetBankDistrictById(int bankdistrictId)
        {
            return repository.Select().FirstOrDefault(x => x.BankDistrictId == bankdistrictId);
        }
        
        public IQueryable<BankDistrict> GetBankDistrictByDistrictId(int districtId)
        {
            return repository.Select().Where(x => x.DistrictId == districtId);
        }

         

        public void Save(BankDistrict bankDistrict)
        {
            repository.Save(bankDistrict);
        }

        public void Delete(int bankDistrictId)
        {
            BankDistrict bd = GetBankDistrictById(bankDistrictId);
            if (bd.IsNotNull())
            {
                repository.Delete(bd);
            }
        }

        public bool UpdateStatus(int bankdistrictId, User currentUser, List<int> listDistrictId)
        {
            BankDistrict bd = GetBankDistrictById(bankdistrictId);
            //if (bd.IsNotNull() && CheckUserAllowToChangeBankDistrictStatus(bd, currentUser, listDistrictId)) //check right HasRightToEditTestBank on controller and  no use CheckUserAllowToChangeBankDistrictStatus  because CheckUserAllowToChangeBankDistrictStatus might prevent owner district stop sharing to other district 
            if (bd.IsNotNull())
            {
                if (bd.BankDistrictAccessId == null)
                {
                    bd.BankDistrictAccessId = (int)LockBankStatus.Restricted;
                    
                }
                else if (bd.BankDistrictAccessId == (int) LockBankStatus.Open)
                {
                    bd.BankDistrictAccessId = (int) LockBankStatus.Restricted;
                    
                }
                else
                {
                    bd.BankDistrictAccessId = (int) LockBankStatus.Open;
                }
                repository.Save(bd);
                if (bd.BankDistrictAccessId== (int)LockBankStatus.Restricted)
                {
                    _qTITestClassAssignmentRepository.InactiveQtiTestClassAssignment(bd.DistrictId, bd.BankId);
                }
                     
                return true;
            }
            return false;
        }

        private bool CheckUserAllowToChangeBankDistrictStatus(BankDistrict bd, User currentUser, List<int> listDistrictId)
        {
            var allow = false;
            
            if (currentUser.IsPublisher
                || (currentUser.IsDistrictAdmin && currentUser.DistrictId.GetValueOrDefault() == bd.DistrictId)
                || (currentUser.IsNetworkAdmin && listDistrictId.Contains(bd.DistrictId)))
            {
                var bank = bankRepository.Select().FirstOrDefault(x => x.Id == bd.BankId);
                if (bank != null)
                {
                    allow = bank.BankAccessID != 2;
                }
            }

            return allow;
        }

        public IQueryable<BankDistrict> GetBankDistrictByBankId(int bankId)
        {
            return repository.Select().Where(x => x.BankId == bankId);
        }
        public IQueryable<District> GetUnPublishedDistrict(int stateId, int bankId)
        {
            var districts = _districtRepository.Select().Where(x => x.StateId.Equals(stateId)).ToList();
            var publishedDistricts = repository.Select().Where(x => x.BankId.Equals(bankId)).ToList();

            var unpublishedDistrictIds = districts.Select(x => x.Id).Except(publishedDistricts.Select(x => x.DistrictId));

            return districts.Where(x => unpublishedDistrictIds.Contains(x.Id)).OrderBy(x=>x.Name).AsQueryable();
        }
        public IQueryable<BankDistrict> Select()
        {
            return repository.Select();
        }
        public void Delete(BankDistrict item)
        {
            repository.Delete(item);
        }

        public bool IsLocked(int bankId,int districtId)
        {
            var currentDate = DateTime.UtcNow;
            var  locked =
                repository.Select()
                    .Any(
                        x =>
                            x.BankId == bankId && x.DistrictId == districtId && (x.BankDistrictAccessId == (int)LockBankStatus.Restricted));
            return locked;
        }
        public bool HideBankDistrict(int bankdistrictId,bool hide, User currentUser, List<int> listDistrictId)
        {
            BankDistrict bd = GetBankDistrictById(bankdistrictId);
            if (bd.IsNotNull())
            {
                bd.Hide = hide;

                repository.Save(bd);
                return true;
            }
            return false;
        }
    }
}
