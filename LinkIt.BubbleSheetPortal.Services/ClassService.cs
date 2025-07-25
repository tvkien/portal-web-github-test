using System;
using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Models.DTOs.Classes;
using Newtonsoft.Json;
using LinkIt.BubbleSheetPortal.Models.DTOs.AggregateSubjectMapping;
using System.Web.Helpers;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class ClassService
    {
        private readonly IRepository<Class> repository;
        private readonly IClassRepository classWithoutFilterByActiveTermRepository;
        private readonly IRepository<ClassUser> _classUserRepository;
        private readonly IReadOnlyRepository<District> districtRepository;
        private readonly IReadOnlyRepository<DistrictTerm> districtTermRepository;
        private readonly IRepository<ClassStudentData> _studentClassRepository;
        private readonly IRepository<Student> _studentRepository;
        public ClassService(IRepository<Class> repository,
            IClassRepository classWithoutFilterByActiveTermRepository,
            IRepository<ClassUser> classUserRepository,
            IReadOnlyRepository<District> districtRepository,
            IReadOnlyRepository<DistrictTerm> districtTermRepository,
            IRepository<ClassStudentData> studentClassRepository,
            IRepository<Student> studentRepository)
        {
            this.repository = repository;
            this.classWithoutFilterByActiveTermRepository = classWithoutFilterByActiveTermRepository;
            this._classUserRepository = classUserRepository;
            this.districtRepository = districtRepository;
            this.districtTermRepository = districtTermRepository;
            this._studentClassRepository = studentClassRepository;
            this._studentRepository = studentRepository;
        }
        public bool SaveClassMeta(List<CreateClassMetas> models)
        {
            return classWithoutFilterByActiveTermRepository.SaveClassMeta(models);
        }
        public List<ClassMetaDto> GetMetaClassByClassId(List<int> classIds, string jsonLabel)
        {
            var labels = new List<LabelConfigDto>();
            if (!string.IsNullOrEmpty(jsonLabel))
            {
                var jsonObject = JsonConvert.DeserializeObject<MetaConfigDto>(jsonLabel);
                labels = jsonObject.Fields.ToList();
            }
            var data = classWithoutFilterByActiveTermRepository.GetMetaClassByClassId(classIds).Select(s => new ClassMetaDto()
            {
                ClassMetaID = s.ClassMetaID,
                ClassID = s.ClassID,
                Data = s.Data,
                Name = s.Name,
                Label = labels.FirstOrDefault(f => f.Name == s.Name).Label,
            }).ToList();
            return data;
        }

        public Class GetClassById(int classId)
        {
            return repository.Select().FirstOrDefault(x => x.Id.Equals(classId));
        }
        public List<Class> GetClassByIdWithoutFilterTerm()
        {
            return classWithoutFilterByActiveTermRepository.SelectWithoutFilterByActiveTerm().ToList();
        }
        public Class GetClassByIdWithoutFilterByActiveTerm(int classId)
        {
            return
                classWithoutFilterByActiveTermRepository.SelectWithoutFilterByActiveTerm()
                    .FirstOrDefault(x => x.Id.Equals(classId));
        }

        public IQueryable<Class> GetClassesByDistrictTermIdAndUserId(int districtTermId, int userId)
        {
            return repository.Select().Where(x => x.DistrictTermId.Equals(districtTermId) && x.UserId.Equals(userId));
        }

        public IQueryable<Class> GetClassesBySchoolIdAndDistrictTermId(int schoolId, int districtTermId)
        {
            return repository.Select().Where(x => x.DistrictTermId.Equals(districtTermId) && x.SchoolId.Equals(schoolId));
        }

        public IQueryable<Class> GetClassesByUserId(int teacherId)
        {
            return repository.Select().Where(c => c.UserId.Equals(teacherId));
        }

        public void SaveClass(Class newClass)
        {
            repository.Save(newClass);
        }

        public void DeleteClass(int classId)
        {
            var classUsers = _classUserRepository.Select()
                .Where(c => c.ClassId == classId)
                .ToArray();
            if (classUsers?.Length > 0)
            {
                foreach (var classUser in classUsers)
                {
                    _classUserRepository.Delete(classUser);
                }
            }
            Class aClass = GetClassById(classId);
            if (aClass.IsNotNull())
            {
                repository.Delete(aClass);
            }
        }

        public IQueryable<Class> GetClassesBySchoolId(int schoolId)
        {
            return repository.Select().Where(c => c.SchoolId.Equals(schoolId));
        }

        public IQueryable<Class> GetClassesBySchoolAndTeacher(int schoolId, int teacherId)
        {
            return repository.Select().Where(c => c.SchoolId.Equals(schoolId) && c.UserId.Equals(teacherId));
        }

        public int GetClassIdByNameSchoolIdUserIdDistrictTermId(int schoolId, int userId, int districtTermId, string className)
        {
            var query = repository.Select().FirstOrDefault(o => o.SchoolId == schoolId && o.UserId == userId && o.DistrictTermId == districtTermId && o.Name == className);
            return query.IsNull() ? 0 : query.Id;
        }

        public Class GetClassesByDistrictTermIdAndClassId(int termId, int classId)
        {
            return repository.Select().FirstOrDefault(x => x.DistrictTermId.Equals(termId) && x.Id.Equals(classId));
        }

        public IQueryable<Class> GetClassesBySchoolIdAndTermIdAndUserId(int termId, int userId, int schoolId)
        {
            return
                repository.Select()
                    .Where(
                        o => o.SchoolId.Equals(schoolId) && o.DistrictTermId.Equals(termId) && o.UserId.Equals(userId));
        }

        public IQueryable<Class> GetClassesBySchoolIdAndTermIdsAndUserIds(List<int> termIds, List<int> userIds, int schoolId)
        {
            return
                repository.Select()
                    .Where(
                        o => o.SchoolId.Equals(schoolId) && o.DistrictTermId != null && termIds.Contains(o.DistrictTermId.Value) && o.UserId != null && userIds.Contains(o.UserId.Value));
        }

        public void UpdateUserId(int classId, int userId, int byUserId)
        {
            var obj = repository.Select().FirstOrDefault(o => o.Id == classId);
            if (obj != null)
            {
                obj.UserId = userId;
                obj.ModifiedDate = DateTime.UtcNow;
                obj.ModifiedUser = byUserId;
                repository.Save(obj);
            }
        }

        public IQueryable<Class> GetClassesByIds(IEnumerable<int> classIds)
        {
            return repository.Select().Where(x => classIds.Contains(x.Id));
        }

        public IQueryable<Class> GetClassesByDistrictTermId(int districtTermId)
        {
            return repository.Select().Where(x => x.DistrictTermId.Equals(districtTermId));
        }
        public IQueryable<Class> Select()
        {
            return repository.Select();
        }

        public int GetSurveyClassBySchoolDistrictTermAndName(int districtId, int schoolId, int districtTermId, string strClassName)
        {
            var objClass = classWithoutFilterByActiveTermRepository
                .GetClassBySchoolTermAndUser(districtId, schoolId, districtTermId, strClassName);

            if (objClass != null)
                return objClass.Id;

            return 0;
        }

        public int SurveyCreateClass(Class objClass)
        {
            repository.Save(objClass);
            return objClass.Id;
        }

        public Class SurveyGetClassByClassId(int classId)
        {
            return classWithoutFilterByActiveTermRepository.GetClassByID(classId);
        }

        public List<ListItem> GetClassBySchoolAndTerm(int districtId, int schoolId, int districtTermId, int userId,
            int roleId)
        {
            return classWithoutFilterByActiveTermRepository.GetClassBySchoolAndTerm(districtId, schoolId, districtTermId,
                userId, roleId);
        }

        public List<ListItem> GetClassBySchoolAndTermV2(int districtId, string schoolIds, int districtTermId, int userId,
            int roleId)
        {
            return classWithoutFilterByActiveTermRepository.GetClassBySchoolAndTermV2(districtId, schoolIds, districtTermId,
                userId, roleId);
        }

        public Class GetClassByClassName(string name, int districtId, int termId)
        {
            return repository.Select().FirstOrDefault(x => x.Name.Equals(name) && x.DistrictId == districtId && x.DistrictTermId == termId);
        }

        public Class GetSurveyClass(int districtId, int districtTermId, int schoolId, string schoolCode, string testName, int userId)
        {
            var district = districtRepository.Select().FirstOrDefault(x => x.Id == districtId);
            var term = districtTermRepository.Select().FirstOrDefault(x => x.DistrictTermID == districtTermId);
            string className = $"{testName} {term.Name}";
            var surveyClass = GetClassByClassName(className, districtId, districtTermId);
            if (surveyClass == null)
            {
                surveyClass = new Class
                {
                    DistrictId = district.Id,
                    Name = className,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow,
                    DistrictTermId = districtTermId,
                    SchoolId = schoolId,
                    ModifiedUser = userId,
                    ModifiedBy = "Portal",
                    UserId = userId,
                    ClassType = (int)ClassTypeEnum.Survey_Class
                };
                SaveClass(surveyClass);

                //Create PlaceHolder Teacher
            }
            return surveyClass;
        }

        public IEnumerable<ListItem> GetClassDistrictTermBySchool(int schoolId, int? userId, int? roleId)
        {
            return classWithoutFilterByActiveTermRepository.GetClassDistrictTermBySchool(schoolId, userId, roleId);
        }
        public IEnumerable<ClassStudentData> GetClassByStudentIds(int classId,List<int> studentIds)
        {
            return _studentClassRepository.Select().Where(x => studentIds.Contains(x.StudentID) && x.ClassID == classId);
        }
        public string GetStudentsName(IEnumerable<string> studentIds)
        {
            var studentList = _studentRepository.Select().Where(x => studentIds.Contains(x.Id.ToString())).Select(x => string.Format("{0}, {1}", x.LastName, x.FirstName)).ToList();
            return string.Join("; ", studentList);
        }
    }
}
