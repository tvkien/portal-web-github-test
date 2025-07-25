using System;
using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Old.UnGroup;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class TeacherDistrictTermService
    {
        private readonly IReadOnlyRepository<TeacherDistrictTerm> repository;
        private readonly ITeacherDistrictTermRepository _teacherDistrictTermRepository;
        private readonly IReadOnlyRepository<TeacherTestDistrictTerm> teacherTestDistrictTermRepository;
        private readonly IStudentTestDistrictTermRepository studentTestDistrictTermRepository;

        private readonly IRepository<Class> classRepository;

        public TeacherDistrictTermService(IReadOnlyRepository<TeacherDistrictTerm> repository,
            ITeacherDistrictTermRepository teacherDistrictTermRepository,
            IReadOnlyRepository<TeacherTestDistrictTerm> teacherTestDistrictTermRepository,
            IRepository<Class> classRepository,
            IStudentTestDistrictTermRepository studentTestDistrictTermRepository)
        {
            this.repository = repository;
            this.teacherTestDistrictTermRepository = teacherTestDistrictTermRepository;
            this.classRepository = classRepository;
            this.studentTestDistrictTermRepository = studentTestDistrictTermRepository;
            this._teacherDistrictTermRepository = teacherDistrictTermRepository;
        }

        public IQueryable<TeacherDistrictTerm> GetTermsByUserIdAndSchoolId(int userId, int schoolId)
        {
            return repository.Select().Where(x => x.UserId.Equals(userId) && x.SchoolId.Equals(schoolId));
        }

        public IQueryable<TeacherDistrictTerm> GetTermsByUserIdsAndSchoolId(List<int> userIds, int schoolId)
        {
            return repository.Select().Where(x => userIds.Contains(x.UserId) && x.SchoolId.Equals(schoolId));
        }

        public IQueryable<TeacherTestDistrictTerm> GetTeacherTestDistrictTerm(int? districtId, int? schoolId, int? teacherId, int? districtTermId, int? virtualTestId,
            int? virtualTestSubTypeId)
        {
            var data = teacherTestDistrictTermRepository.Select();

            if (districtId.HasValue)
                data = data.Where(x => x.DistrictId.Equals(districtId));

            if (schoolId.HasValue)
                data = data.Where(x => x.SchoolId.Equals(schoolId));

            if (teacherId.HasValue)
                data = data.Where(x => x.UserId.Equals(teacherId));

            if (virtualTestId.HasValue)
                data = data.Where(x => x.VirtualTestId.Equals(virtualTestId));

            if (districtTermId.HasValue)
                data = data.Where(x => x.DistrictTermId.Equals(districtTermId));

            if (virtualTestSubTypeId.HasValue)
                data = data.Where(x => x.VirtualTestSubTypeId.Equals(virtualTestSubTypeId));

            return data;
        }

        public IQueryable<TeacherTestDistrictTerm> GetTeacherTestDistrictTerm(int? districtId, int? schoolId, int? teacherId, int? districtTermId, List<int> virtualTestIds,
            int? virtualTestSubTypeId, DateTime? resultDateFrom, DateTime? resultDateTo)
        {
            var data = teacherTestDistrictTermRepository.Select();

            if (districtId.HasValue)
                data = data.Where(x => x.DistrictId.Equals(districtId));

            if (schoolId.HasValue)
                data = data.Where(x => x.SchoolId.Equals(schoolId));

            if (teacherId.HasValue)
                data = data.Where(x => x.UserId.Equals(teacherId));

            if (virtualTestIds.Any())
                data = data.Where(x => virtualTestIds.Contains(x.VirtualTestId));

            if (districtTermId.HasValue)
                data = data.Where(x => x.DistrictTermId.Equals(districtTermId));

            if (virtualTestSubTypeId.HasValue)
                data = data.Where(x => x.VirtualTestSubTypeId.Equals(virtualTestSubTypeId));

            if (resultDateFrom.HasValue)
                data = data.Where(x => x.ResultDate >= resultDateFrom);

            if (resultDateTo.HasValue)
            {
                resultDateTo = Convert.ToDateTime(resultDateTo.Value.ToShortDateString()).AddDays(1).AddSeconds(-1);
                data = data.Where(x => x.ResultDate <= resultDateTo);
            }

            return data;
        }

        public IQueryable<StudentTestDistrictTerm> GetStudentTestDistrictTerm(int? districtId, int? schoolId, int? teacherId, int? districtTermId, int? classId, List<int> virtualTestIds,
            int? virtualTestSubTypeId, DateTime? resultDateFrom, DateTime? resultDateTo)
        {
            var data = studentTestDistrictTermRepository.Select();

            if (districtId.HasValue)
                data = data.Where(x => x.DistrictId.Equals(districtId));

            if (schoolId.HasValue)
                data = data.Where(x => x.SchoolId.Equals(schoolId));

            if (teacherId.HasValue)
                data = data.Where(x => x.UserId.Equals(teacherId));

            if (virtualTestIds.Any())
                data = data.Where(x => virtualTestIds.Contains(x.VirtualTestId));

            if (districtTermId.HasValue)
                data = data.Where(x => x.DistrictTermId.Equals(districtTermId));

            if (classId.HasValue)
                data = data.Where(x => x.ClassId.Equals(classId));

            if (virtualTestSubTypeId.HasValue)
                data = data.Where(x => x.VirtualTestSubTypeId.Equals(virtualTestSubTypeId));

            if (resultDateFrom.HasValue)
                data = data.Where(x => x.ResultDate >= resultDateFrom);

            if (resultDateTo.HasValue)
            {
                resultDateTo = Convert.ToDateTime(resultDateTo.Value.ToShortDateString()).AddDays(1).AddSeconds(-1);
                data = data.Where(x => x.ResultDate <= resultDateTo);
            }

            return data;
        }

        public IEnumerable<StudentTestDistrictTerm> GetStudentTestDistrictTerm_New(
            StudentTestDistrictTermParam param)
        {
            return studentTestDistrictTermRepository.GetStudentTestDistrictTerms(param);
        }

        public IQueryable<TeacherDistrictTerm> GetTeachersHasTerms(int schoolId)
        {
            return repository.Select().Where(x => x.SchoolId.Equals(schoolId) && x.UserStatusId == (int)UserStatus.Active);
        }
        public IQueryable<TeacherDistrictTerm> GetTeachersHasTermsInDistrict(int districtId)
        {
            return repository.Select().Where(x => x.DistrictID.Equals(districtId) && x.UserStatusId == (int)UserStatus.Active);
        }

        public IEnumerable<TeacherDistrictTerm> GetTermBySchool(int schoolId, int userId, int roleId)
        {
            return _teacherDistrictTermRepository.GetTermBySchool(schoolId, userId, roleId);
        }
    }
}
