using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class QtiBankSchoolService
    {
        private readonly IReadOnlyRepository<QtiBankSchool> _repository;
        private readonly IInsertDeleteRepository<QtiBankSchool> _insertDeleteRepository;
        private readonly IReadOnlyRepository<School> _schoolRepository;
        private readonly IQtiBankRepository _qtiBankRepository;

        public QtiBankSchoolService(IReadOnlyRepository<QtiBankSchool> repository, 
            IInsertDeleteRepository<QtiBankSchool> insertDeleteRepository,
            IReadOnlyRepository<School> schoolRepository,
            IQtiBankRepository qtiBankRepository)
        {
            _repository = repository;
            _insertDeleteRepository = insertDeleteRepository;
            _schoolRepository = schoolRepository;
            _qtiBankRepository = qtiBankRepository;
        }

        public QtiBankSchool GetById(int qtiBankSchoolId)
        {
            return _repository.Select().FirstOrDefault(x => x.QtiBankSchoolId == qtiBankSchoolId);
        }
        
        public void Delete(QtiBankSchool qtiBankSchool)
        {
            _insertDeleteRepository.Delete(qtiBankSchool);
        }

        public IQueryable<School> GetUnPublishedSchool(int districtId, int qtiBankId)
        {
            var schools = _schoolRepository.Select().Where(x => x.DistrictId.Equals(districtId) && x.Status == 1).ToList();
            var publishedSchools = _repository.Select().Where(x => x.QtiBankId.Equals(qtiBankId)).ToList();

            var unpublishedSchoolIds = schools.Select(x => x.Id).Except(publishedSchools.Select(x => x.SchoolId));

            return schools.Where(x => unpublishedSchoolIds.Contains(x.Id)).AsQueryable();
        }

        public IQueryable<QtiBankSchool> Select()
        {
            return _repository.Select();
        }

        public void Save(QtiBankSchool qtiBankSchool)
        {
            _insertDeleteRepository.Save(qtiBankSchool);
        }

        public List<QtiBankSchool> GetQtiBankSchoolOfSchools(string schoolIdString)
        {
            return _qtiBankRepository.GetQtiBankSchoolOfSchools(schoolIdString);
        }
    }
}
