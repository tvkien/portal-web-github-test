using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class QtiBankService
    {
        private readonly IQtiBankRepository _repository;
        private readonly IRepository<QtiBank> _qtiBankRepository;
        private readonly IRepository<QtiBankDistrict> _qtiBankDistrictRepository;
        private readonly IRepository<School> _schoolRepository;
        private readonly IReadOnlyRepository<QtiBankSchool> _qtiBankSchoolRepository;
        private readonly IUserSchoolRepository<UserSchool> _userSchoolRepository;

        public QtiBankService(IQtiBankRepository repository, IRepository<QtiBank> qtiBankRepository, IRepository<QtiBankDistrict> qtiBankDistrictRepository, IRepository<School> schoolRepository, IReadOnlyRepository<QtiBankSchool> qtiBankSchoolRepository, IUserSchoolRepository<UserSchool> userSchoolRepository)
        {
            _repository = repository;
            _qtiBankRepository = qtiBankRepository;
            _qtiBankDistrictRepository = qtiBankDistrictRepository;
            _schoolRepository = schoolRepository;
            _qtiBankSchoolRepository = qtiBankSchoolRepository;
            _userSchoolRepository = userSchoolRepository;
        }

        public IQueryable<QtiBankList> GetQtiBankList(int userId, int districtId, string bankName, string author, string publishedTo)
        {
            return _repository
                .GetQtiBankList(userId, districtId, bankName, author, publishedTo)
                .Select(x => new QtiBankList
                                 {
                                      QtiBankId = x.QTIBankID,
                                      Name = x.Name,
                                      Author = x.Author,
                                      QtiGroupSet = x.QTIGroupSet,
                                      DistrictNames = x.DistrictName,
                                      SchoolNames = x.SchoolName
                                  }).ToList().AsQueryable();
        }

        public IQueryable<QtiBankPublishedDistrict> GetPublishedDistrict(int qtiBankId)
        {
            return _repository.GetPublishedDistrict().Where(x => x.QtiBankId == qtiBankId);
        }

        public IQueryable<QtiBankPublishedSchool> GetPublishedSchool(int qtiBankId)
        {
            return _repository.GetPublishedSchool().Where(x => x.QtiBankId == qtiBankId);
        }

        public QtiBank GetById(int qtiBankId)
        {
            return _qtiBankRepository.Select().FirstOrDefault(x => x.QtiBankId == qtiBankId);
        }

        public DateTime? GetQTIBankModifiedDate(int qtiBankId)
        {
            return _repository.GetQTIBankModifiedDate(qtiBankId);
        }

        public List<QtiBankCustom> GetItemBanks(int userId, int roleId, int districtId, bool? hideTeacherBanks, bool? hideOtherPeopleBanks)
        {
            return _repository.LoadQTIBanks(userId, roleId, districtId, hideTeacherBanks,hideOtherPeopleBanks);
        }
        public List<QtiBank> GetItemBanksPersonal(int userId, int districtId)
        {
            return _repository.LoadQTIBanksPersonal(userId, districtId);
        }

        public List<QtiBank> GetOwnerItemBanks(int userId)
        {
            return _repository.GetOwnerItemBanks(userId);
        }

        public bool Save(QtiBank obj)
        {
            if (obj != null)
            {
                if (obj.QtiBankId == 0)
                    obj.CreatedDate = DateTime.Now;
                obj.ModifiedDate = DateTime.Now;
                _qtiBankRepository.Save(obj);
                return true;
            }
            return false;
        }

        public bool Delete(int qitBankId)
        {
            if (qitBankId > 0)
            {
                var  obj = _qtiBankRepository.Select().FirstOrDefault(o => o.QtiBankId == qitBankId);
                _qtiBankRepository.Delete(obj);
                return true;
            }
            return false;
        }

        public bool CheckExistBankName(int? qtiBankId, string bankName, int userId)
        {
            if(qtiBankId.HasValue)
                return _qtiBankRepository.Select().Any(o => o.UserId == userId && o.Name == bankName && o.QtiBankId != qtiBankId.Value);
            
            return _qtiBankRepository.Select().Any(o => o.UserId == userId && o.Name == bankName);
        }

        public void UpdateAuthorGroupId(int itemBankId, int authorGroupId)
        {
            var obj = _qtiBankRepository.Select().FirstOrDefault(o => o.QtiBankId == itemBankId);
            if (obj != null && authorGroupId > 0)
            {
                obj.AuthorGroupId = authorGroupId;
                obj.ModifiedDate = DateTime.Now;
                _qtiBankRepository.Save(obj);
            }
        }
        public List<QtiBank>  GetQtiBankDistricts(int roleId,int userId, int districtId,List<int> memberDistrictIdList = null)
        {
            List<int> qtiBankIdList = _qtiBankDistrictRepository.Select().Where(x => x.DistrictId == districtId).Select(x=>x.QtiBankId) .ToList();
            
            //System should show the banks shared to user's school (in QTIBankSchool) as well 
            var schoolsOfDistrict = _schoolRepository.Select().Where(x => x.DistrictId == districtId).Select(x => x.Id).ToList();

            if (roleId == (int)Permissions.Teacher || roleId == (int)Permissions.SchoolAdmin)
            {
                //Only get schools user has access to
                List<int> rightAccessSchools =
                    _userSchoolRepository.Select().Where(x => x.UserId.Equals(userId) && x.UserSchoolId != null).Select(
                        x => x.SchoolId??0).ToList();
                List<int> validSchools = rightAccessSchools.Where(x => rightAccessSchools.Contains(x)).ToList();
                var banksSharedSchools =
                    _qtiBankSchoolRepository.Select().Where(x => validSchools.Contains(x.SchoolId)).ToList();
                //Add banks shared to schools
                qtiBankIdList.AddRange(banksSharedSchools.Select(x => x.QtiBankId).ToList());
            }
            else if(roleId ==(int)Permissions.NetworkAdmin)
            {
                if (memberDistrictIdList == null)
                {
                    memberDistrictIdList = new List<int>();
                }
                qtiBankIdList = _qtiBankDistrictRepository.Select().Where(x => memberDistrictIdList.Contains(x.DistrictId)).Select(x => x.QtiBankId).ToList();
                //banks shared
                schoolsOfDistrict = _schoolRepository.Select().Where(x => memberDistrictIdList.Contains(x.DistrictId)).Select(x => x.Id).ToList();
                //var banksSharedSchools = _qtiBankSchoolRepository.Select().Where(x => schoolsOfDistrict.Contains(x.SchoolId)).ToList();
                var banksSharedSchools = _repository.GetQtiBankSchoolOfSchools(schoolsOfDistrict);//change to use a stored proc
                //Add banks shared to schools
                qtiBankIdList.AddRange(banksSharedSchools.Select(x => x.QtiBankId).ToList());

            }
            else //District Admin
            {
                //var banksSharedSchools = _qtiBankSchoolRepository.Select().Where(x => schoolsOfDistrict.Contains(x.SchoolId)).ToList();
                var banksSharedSchools = _repository.GetQtiBankSchoolOfSchools(schoolsOfDistrict);//change to use a stored proc
                //Add banks shared to schools
                qtiBankIdList.AddRange(banksSharedSchools.Select(x => x.QtiBankId).ToList());
            }

            return _qtiBankRepository.Select().Where(x => qtiBankIdList.Contains(x.QtiBankId)).ToList();
        }

        public bool CheckBankSharedToDistrict(int qtiBankId, int districtId)
        {
            return _qtiBankDistrictRepository.Select().Any(x => x.DistrictId == districtId && x.QtiBankId == qtiBankId);
        }
        public IQueryable<QtiBank> GetAll()
        {
            return _qtiBankRepository.Select();
        }

        public QtiBank GetQTIBankByName(string bankName, int userId)
        {
            return _qtiBankRepository.Select().FirstOrDefault(o => o.UserId == userId && o.Name == bankName);
        }

        public QtiBank CreateQTIBankByUserName(string userName, int userId)
        {
            var vQTIBank = GetQTIBankByName(userName, userId);
            if (vQTIBank == null)
            {
                vQTIBank = new QtiBank()
                {
                    Name = userName,
                    AccessId = 3,
                    UserId = userId,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow
                };
                Save(vQTIBank);
            }
            return vQTIBank;
        }
        public QtiBank GetDefaultQTIBank(string name, int userId)
        {
            var qtiBank = this.GetQTIBankByName(name, userId);
            if (qtiBank == null)
            {
                qtiBank = new QtiBank
                {
                    Name = name,
                    AccessId = 3,
                    UserId = userId
                };

                _qtiBankRepository.Save(qtiBank);
            }

            return qtiBank;
        }
    }
}
