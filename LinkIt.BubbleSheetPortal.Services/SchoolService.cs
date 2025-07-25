using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using System.Collections.Generic;
using System;
using LinkIt.BubbleSheetPortal.Models.DTOs.Commons;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class SchoolService
    {
        private readonly IRepository<School> repository;
        private readonly IRepository<AuthorGroupSchool> authorGroupSchoolRepository;
        private readonly ISchoolRepository schoolRepository;

        public SchoolService(IRepository<School> repository,
            IRepository<AuthorGroupSchool> authorGroupSchoolRepository,
            ISchoolRepository schoolRepository)
        {
            this.repository = repository;
            this.authorGroupSchoolRepository = authorGroupSchoolRepository;
            this.schoolRepository = schoolRepository;
        }

        public School GetSchoolById(int schoolId)
        {
            return repository.Select().FirstOrDefault(x => x.Id.Equals(schoolId));
        }

        public IQueryable<School> GetSchoolsByDistrictId(int districtId)
        {
            return repository.Select().Where(x => x.DistrictId.Equals(districtId) && (!x.Status.HasValue || x.Status != 3)).OrderBy(x => x.Name).ThenBy(x => x.Code).ThenBy(x => x.StateCode);
        }

        public void Save(School item)
        {
            repository.Save(item);
        }

        public void Delete(School item)
        {
            repository.Delete(item);
        }

        public List<School> GetSchoolByNames(IEnumerable<string> lstSchoolName, int districtId)
        {
            var query = repository.Select().Where(o => o.DistrictId == districtId && lstSchoolName.Contains(o.Name));
            return query.ToList();
        }

        public IQueryable<School> GetSchoolByAuthorGroupId(int authorGroupId)
        {
            var schoolIds =
                authorGroupSchoolRepository.Select()
                    .Where(x => x.AuthorGroupId == authorGroupId)
                    .Select(x => x.SchoolId)
                    .Distinct()
                    .ToList();
            var query = repository.Select().Where(x => schoolIds.Contains(x.Id));
            return query;
        }
        public IQueryable<School> GetAll()
        {
            return repository.Select();
        }

        public bool CheckUserCanAccessSchool(int userId, int roleId, int schoolId)
        {
            return schoolRepository.CheckUserCanAccessSchool(userId, roleId, schoolId);
        }

        public DateTime GetCurrentDateTimeBySchoolId(int schoolId)
        {
            DateTime dt = DateTime.UtcNow;
            var schoolTimeZone = GetSchoolTimeZone(schoolId);
            var timezoneInfo = TimeZoneInfo.FindSystemTimeZoneById(schoolTimeZone.TimeZoneId);
            dt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timezoneInfo);
            return dt;
        }

        public TimeZoneDto GetSchoolTimeZone(int schoolId)
        {
            return schoolRepository.GetSchoolTimeZone(schoolId);
        }

        public ItemValue GetSurveySchoolClass(int districtId, int termId, string surveyName, int userId)
        {
            return schoolRepository.CreateSurveySchoolClass(districtId, termId, surveyName, userId);
        }

        public int GetDistrictIdBySchoolId(int schoolId)
        {
            return repository.Select()
                .Where(x => x.Id == schoolId)
                .Select(x => x.DistrictId)
                .FirstOrDefault();
        }

        public List<School> GetSchoolsByDistrictV2(GetSchoolRequestModel input, ref int? totalRecords)
        {
            return schoolRepository.GetSchoolsByDistrictV2(input, ref totalRecords).ToList();
        }

        public List<School> GetTLDSSchoolsByDistrictId(int districtId)
        {
            return schoolRepository.GetTLDSSchoolsByDistrict(districtId);
        }
    }
}
