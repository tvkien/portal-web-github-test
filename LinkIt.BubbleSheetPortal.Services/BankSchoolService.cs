using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class BankSchoolService
    {
        private readonly IRepository<BankSchool> _repository;
        private readonly IReadOnlyRepository<District> _districtRepository;
        private readonly IReadOnlyRepository<School> _schoolRepository;

        public BankSchoolService(IRepository<BankSchool> repository, IReadOnlyRepository<District> districtRepository,
                                 IReadOnlyRepository<School> schoolRepository)
        {
            this._repository = repository;
            this._districtRepository = districtRepository;
            this._schoolRepository = schoolRepository;
        }

        public BankSchool GetBankSchoolById(int bankSchoolId)
        {
            return _repository.Select().FirstOrDefault(x => x.BankSchoolId == bankSchoolId);
        }

        public IQueryable<BankSchool> GetBankSchoolByDistrictId(int districtId)
        {
            return _repository.Select().Where(x => x.DistrictId == districtId);
        }

        public IQueryable<BankSchool> GetBankSchoolBySchoolId(int schoolId)
        {
            return _repository.Select().Where(x => x.SchoolId == schoolId);
        }


        public void Save(BankSchool bankSchool)
        {
            _repository.Save(bankSchool);
        }

        public void Delete(int bankSchoolId)
        {
            BankSchool bd = GetBankSchoolById(bankSchoolId);
            if (bd.IsNotNull())
            {
                _repository.Delete(bd);
            }
        }

        public IQueryable<BankSchool> GetBankSchoolByBankId(int bankId)
        {
            return _repository.Select().Where(x => x.BankId == bankId);
        }

        public IQueryable<School> GetUnPublishedSchool(int districtId, int bankId)
        {
            var schools = _schoolRepository.Select().Where(x => x.DistrictId.Equals(districtId) && x.Status == 1).ToList();
            var publishedSchools = _repository.Select().Where(x => x.BankId.Equals(bankId)).ToList();

            var unpublishedSchoolIds = schools.Select(x => x.Id).Except(publishedSchools.Select(x => x.SchoolId));

            return schools.Where(x => unpublishedSchoolIds.Contains(x.Id)).AsQueryable();
        }

        public IQueryable<BankSchool> Select()
        {
            return _repository.Select();
        }

        public void Delete(BankSchool item)
        {
            _repository.Delete(item);
        }
    }
}
