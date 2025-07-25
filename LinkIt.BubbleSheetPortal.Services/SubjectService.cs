using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.SGO;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class SubjectService
    {
        private readonly ISubjectRepository repository;
        private readonly IReadOnlyRepository<District> _districtRepository;
        private readonly ConfigurationService _configurationService;
        

        public SubjectService(ISubjectRepository repository, IReadOnlyRepository<District> districtRepository, ConfigurationService configurationService)
        {
            this.repository = repository;
            _districtRepository = districtRepository;
            _configurationService = configurationService;
        }

        public Subject GetSubjectById(int subjectId)
        {
            return repository.Select().FirstOrDefault(x => x.Id.Equals(subjectId));
        }

        public IQueryable<Subject> GetSubjectsByGradeAndState(int gradeId, int stateId)
        {
            return repository.Select().Where(x => x.GradeId.Equals(gradeId) && x.StateId.Equals(stateId));
        }

        public List<SubjectOrder> GetSubjectsFormBankByGradeAndDistrictId(int gradeId, int districtId, int userId, int userRole, bool? isFromMultiDate, bool usingMultiDate)
        {
            return repository.GetSubjectsFormBankByGradeId(gradeId, districtId, userId, userRole, isFromMultiDate ?? false, usingMultiDate);
        }

        public List<Subject> GetSubjectsByGradeId(int gradeId, int districtId, int userId, int userRole)
        {
            return repository.GetSubjectSByGradeId(gradeId, districtId, userId, userRole);
        }

        public List<Subject> GetSubjectsByGradeId(int gradeId)
        {
            return repository.Select().Where(x => x.GradeId == gradeId).ToList();
        }

        public IQueryable<Subject> GetSubjectsByListSubjectId(List<int> listSubjectIds)
        {
            return repository.Select().Where(s => listSubjectIds.Contains(s.Id));
        }

        public List<Subject> ACTGetSubjectsByGradeId(int gradeId, int districtId, int userId, int userRole)
        {
            return repository.ACTGetSubjectSByGradeId(gradeId, districtId, userId, userRole);
        }

        public List<Subject> GetSubjectsForItemSetSaveTest(int gradeId, int districtId, int userId, int userRole)
        {
            return repository.GetSubjectsForItemSetSaveTest(gradeId, districtId, userId, userRole);
        }
        public List<Subject> GetSubjectsByGradeIdAndAuthorOfAllTestType(SearchBankCriteria criteria)
        {
            return repository.GetSubjectsByGradeIdAndAuthorOfAllTestType(criteria);
        }
        public List<Subject> GetSubjectsByGradeIdAndAuthor(SearchBankCriteria criteria)
        {
            return repository.GetSubjectSByGradeIdAndAuthor(criteria);
        }

        public IQueryable<Subject> GetSubjectsByState(int stateId)
        {
            return repository.Select().Where(x => x.StateId.Equals(stateId));
        }

        public List<Subject> SATGetSubjectsByGradeId(int gradeId, int districtId, int userId, int userRole)
        {
            return repository.SATGetSubjectSByGradeId(gradeId, districtId, userId, userRole);
        }

        public IQueryable<Subject> SGOGetSubjectsForCreateExternalTest(SGOObject sgo)
        {
            var emptySubjects = new List<Subject>();
            if (sgo == null) return emptySubjects.AsQueryable();

            var district = _districtRepository.Select().FirstOrDefault(o => o.Id == sgo.DistrictID);
            if (district == null) return emptySubjects.AsQueryable();

            var result = repository.Select().Where(o => o.StateId == district.StateId);

            return result;
        }

        public IQueryable<SubjectOrderDistrict> GetSubjectOrders(int districtId)
        {
            return repository.GetSubjectOrders(districtId);
        }

        public Subject GetSubjectByShortName(string shortName, int stateId, string subjectName, string gradeName)
        {
            return repository.GetSubjectByShortName(shortName, stateId, subjectName, gradeName);
        }

    }
}
