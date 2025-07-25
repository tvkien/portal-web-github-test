using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class AblesReportService
    {
        private readonly IAblesReportRepository _repository;
        private readonly IRepository<AblesReportJob> _ablesReportJobRepository;
        private readonly IReadOnlyRepository<AblesVirtualTestMapping> _ablesMappingRepository;
        private readonly IReadOnlyRepository<AblesAssessmentRound> _ablesAssRoundRepository;

        public AblesReportService(IAblesReportRepository repository,
            IRepository<AblesReportJob> ablesReportJobRepository,
            IReadOnlyRepository<AblesVirtualTestMapping> ablesMappingRepository,
            IReadOnlyRepository<AblesAssessmentRound> ablesAssRoundRepository)
        {
            _repository = repository;
            _ablesReportJobRepository = ablesReportJobRepository;
            _ablesMappingRepository = ablesMappingRepository;
            _ablesAssRoundRepository = ablesAssRoundRepository;
        }

        public List<ReportType> GetAllReportTypes()
        {
            return _repository.GetAllReportTypes().ToList();
        }
        public List<AblesVirtualTestMapping> GetVirtuaTestMappings(int districtId)
        {
            return _ablesMappingRepository.Select().Where(x => x.DistrictID == districtId).ToList();
        }
        public AblesVirtualTestMapping GetVirtuaTestMappingByTestId(int districtId, int testId)
        {
            var result = _ablesMappingRepository.Select().Where(x => x.DistrictID == districtId && x.VirtualTestID == testId).FirstOrDefault();
            return result ?? new AblesVirtualTestMapping();
        }

        public AblesVirtualTestMapping GetVirtuaTestMapping(int districtId, string ablesTestName, string round)
        {
            var result = _ablesMappingRepository.Select()
                .Where(x => x.DistrictID == districtId && x.AblesTestName == ablesTestName && x.Round == round)
                .FirstOrDefault();
            return result ?? new AblesVirtualTestMapping();
        }

        public List<AblesVirtualTestMapping> GetVirtuaTestMappingByAblesTestName(int districtId, string ablesTestName)
        {
            return _ablesMappingRepository.Select().Where(x => x.DistrictID == districtId && x.AblesTestName == ablesTestName).ToList();
        }

        public List<AblesPointsEarnedResponseData> GetPointsEarnedResponsed(int districtId, string studentList, int schoolId,
            string termIdList, string testIdList, bool isASD)
        {
            return _repository.GetPointsEarnedResponsed(districtId, studentList, schoolId, termIdList, testIdList, isASD);
        }

        public List<AblesPointsEarnedResponseData> GetAblesDataForAdminReporting(int studentId, int testresultId)
        {
            return _repository.GetAblesDataForAdminReporting(studentId, testresultId);
        }

        public void SaveReportJob(AblesReportJob ablesReportJob)
        {
            _ablesReportJobRepository.Save(ablesReportJob);
        }
        public AblesReportJob GetReportJobById(int reportJobId)
        {
            return _ablesReportJobRepository.Select().Where(x=>x.AblesReportJobId == reportJobId).FirstOrDefault();
        }
        public List<AblesDataDropDown> GetAblesDataDropDown(int? districtId, int? schoolId, int? teacherId, int userId, int roleId,
            string districtTermIdStr, string testIdList, bool isASD, int year)
        {
            return _repository.GetAblesDataDropDown(districtId, schoolId, teacherId, userId, roleId, districtTermIdStr, testIdList, isASD, year);
        }

        public List<AblesReportJobData> GetAblesReportJobs(int districtId, int userId, DateTime fromDate, DateTime toDate)
        {
            return _repository.GetAblesReportJobs(districtId, userId, fromDate, toDate);
        }

        public List<AblesAssessmentRound> GetAssessmentRounds(int districtId)
        {
            return _ablesAssRoundRepository.Select().Where(x => x.DistrictId == districtId).ToList();
        }

        public List<AblesStudent> GetStudentHasData(int? districtId, int? classId, string districtTermIdStr,
            string testIdList, bool isASD)
        {
            return
                _repository.GetStudentHasData(districtId, classId, districtTermIdStr, testIdList, isASD)
                    .OrderBy(x => x.FullName).ToList();
        }

        public List<int> GetYearBySchool(int? schoolId)
        {
            var result = new List<int>();
            var currentYear = DateTime.UtcNow.Year;
            var minYear = _repository.GetMinResultYearBySchool(schoolId);
            if (!minYear.HasValue)
                minYear = currentYear;
            var rangeYear = currentYear - minYear.Value;
            for (var i = 0; i <= rangeYear; i++)
            {
                var year = currentYear - i;
                result.Add(year);
            }

            return result;
        }
    }
}
