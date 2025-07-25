using System;
using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Old.UnGroup;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class PopulateReportingService
    {
        private readonly IPopulateReportingRepository _populateReportingRepository;
        private readonly IRepository<SpecializedReportJob> _specializedReportJobRepository;
        private readonly TeacherDistrictTermService _teacherDistrictTermService;
        private readonly UserSchoolService _userSchoolService;

        public PopulateReportingService(IPopulateReportingRepository populateReportingRepository
            , IRepository<SpecializedReportJob> specializedReportJobRepository
            , TeacherDistrictTermService teacherDistrictTermService
            , UserSchoolService userSchoolService)
        {
            _populateReportingRepository = populateReportingRepository;
            _specializedReportJobRepository = specializedReportJobRepository;
            _teacherDistrictTermService = teacherDistrictTermService;
            _userSchoolService = userSchoolService;
        }

        public List<Grade> ReportingGetGrades(int userId, int districtId, int roleId, int virtualTestSubTypeId, DateTime? resultDateFrom, DateTime? resultDateTo, bool? isGetAllClass)
        {
            return _populateReportingRepository.ReportingGetGrades(userId, districtId, roleId, virtualTestSubTypeId, resultDateFrom, resultDateTo, isGetAllClass);
        }

        public List<Subject> ReportingGetSubjects(int gradeId, int districtId, int userId, int userRole, int virtualTestSubTypeId, DateTime? resultDateFrom, DateTime? resultDateTo, bool? isGetAllClass)
        {
            return _populateReportingRepository.ReportingGetSubjects(gradeId, districtId, userId, userRole,
                virtualTestSubTypeId, resultDateFrom, resultDateTo, isGetAllClass);
        }

        public List<ListItem> ReportingGetBanks(int subjectId, int districtId, int userId, int userRole, int virtualTestSubTypeId, DateTime? resultDateFrom, DateTime? resultDateTo, bool? isGetAllClass)
        {
            return _populateReportingRepository.ReportingGetBanks(subjectId, districtId, userId, userRole,
                virtualTestSubTypeId, resultDateFrom, resultDateTo, isGetAllClass);
        }

        public List<ListItem> ReportingGetTests(int? gradeId, int? subjectId, int? bankId, int districtId, int userId, int userRole, int virtualTestSubTypeId, DateTime? resultDateFrom, DateTime? resultDateTo, bool? isGetAllClass)
        {
            return _populateReportingRepository.ReportingGetTests(gradeId, subjectId, bankId, districtId, userId, userRole,
                virtualTestSubTypeId, resultDateFrom, resultDateTo, isGetAllClass);
        }

        public List<SpecializedReportJob> GetSpecializedReportJobs(int userId)
        {
            return _specializedReportJobRepository.Select().Where(x => x.UserId == userId).ToList();
        }

        public void SaveSpecializedReportJob(SpecializedReportJob job)
        {
            _specializedReportJobRepository.Save(job);
        }

        public SpecializedReportJob GetSpecializedReportJob(int id)
        {
            return _specializedReportJobRepository.Select().FirstOrDefault(x => x.SpecializedReportJobId == id);
        }

        public List<StudentTestDistrictTerm> ReportingGetSchools(int userId, int userRoleId, int districtId, string virtualTestIds
            , string virtualTestSubTypeIds, DateTime? resultDateFrom, DateTime? resultDateTo, bool isGetAllClass = false)
        {
            var virtualTestIdsList = virtualTestIds.ToIntList(",");
            var virtualTestSubTypeIdsList = virtualTestSubTypeIds.ToIntList(";");

            var data = new List<StudentTestDistrictTerm>();

            if (isGetAllClass)
            {
                var param = new StudentTestDistrictTermParam
                {
                    DistrictId = districtId,
                    VirtualTestIds = virtualTestIdsList,
                    VirtualTestSubTypeIds = virtualTestSubTypeIdsList,
                    ResultDateFrom = resultDateFrom,
                    ResultDateTo = resultDateTo
                };

                data = _teacherDistrictTermService.GetStudentTestDistrictTerm_New(param).ToList();
            }
            else
            {
                foreach (var subTypeId in virtualTestSubTypeIdsList)
                {
                    var tempData = _teacherDistrictTermService.GetTeacherTestDistrictTerm(districtId,
                    null, null, null, virtualTestIdsList, subTypeId, resultDateFrom, resultDateTo)
                    .Select(x => new StudentTestDistrictTerm
                    {
                        ClassId = x.ClassId,
                        ClassName = x.ClassName,
                        DateEnd = x.DateEnd,
                        DateStart = x.DateStart,
                        DistrictId = x.DistrictId,
                        DistrictTermId = x.DistrictTermId,
                        DistrictTermName = x.DistrictTermName,
                        NameFirst = x.NameFirst,
                        NameLast = x.NameLast,
                        ResultDate = x.ResultDate,
                        SchoolId = x.SchoolId,
                        SchoolName = x.SchoolName,
                        UserId = x.UserId,
                        UserName = x.UserName,
                        VirtualTestId = x.VirtualTestId,
                        VirtualTestSubTypeId = x.VirtualTestSubTypeId
                    }).ToList();

                    if (tempData.Any())
                    {
                        data = data.Union(tempData).ToList();
                    }
                }
            }

            var currentUser = new User
            {
                Id = userId,
                RoleId = userRoleId
            };

            if (currentUser.IsTeacher || currentUser.IsSchoolAdmin)
            {
                var accessSchools = _userSchoolService.GetSchoolsUserHasAccessTo(currentUser.Id).ToList();
                data = data.Where(x => accessSchools.Any(s => s.SchoolId == x.SchoolId)).ToList();
            }

            return data;
        }

        public List<StudentTestDistrictTerm> ReportingGetTeachers(
            int userId,
            int userRoleId,
            int districtId,
            int schoolId,
            string virtualTestIds,
            string virtualTestSubTypeIds,
            DateTime? resultDateFrom,
            DateTime? resultDateTo,
            bool isGetAllClass = false)
        {
            var virtualTestIdsList = virtualTestIds.ToIntList(",");
            var virtualTestSubTypeIdsList = virtualTestSubTypeIds.ToIntList(";");
            var data = new List<StudentTestDistrictTerm>();

            if (isGetAllClass)
            {
                var param = new StudentTestDistrictTermParam
                {
                    DistrictId = districtId,
                    SchoolId = schoolId,
                    VirtualTestIds = virtualTestIdsList,
                    VirtualTestSubTypeIds = virtualTestSubTypeIdsList,
                    ResultDateFrom = resultDateFrom,
                    ResultDateTo = resultDateTo
                };

                data = _teacherDistrictTermService.GetStudentTestDistrictTerm_New(param).ToList();
            }
            else
            {
                foreach (var subTypeId in virtualTestSubTypeIdsList)
                {
                    var tempData = _teacherDistrictTermService.GetTeacherTestDistrictTerm(districtId,
                        schoolId, null, null, virtualTestIdsList, subTypeId, resultDateFrom, resultDateTo)
                        .Select(x => new StudentTestDistrictTerm
                        {
                            ClassId = x.ClassId,
                            ClassName = x.ClassName,
                            DateEnd = x.DateEnd,
                            DateStart = x.DateStart,
                            DistrictId = x.DistrictId,
                            DistrictTermId = x.DistrictTermId,
                            DistrictTermName = x.DistrictTermName,
                            NameFirst = x.NameFirst,
                            NameLast = x.NameLast,
                            ResultDate = x.ResultDate,
                            SchoolId = x.SchoolId,
                            SchoolName = x.SchoolName,
                            UserId = x.UserId,
                            UserName = x.UserName,
                            VirtualTestId = x.VirtualTestId,
                            VirtualTestSubTypeId = x.VirtualTestSubTypeId
                        }).ToList();

                    if (tempData.Any())
                    {
                        data = data.Union(tempData).ToList();
                    }
                }
            }

            var currentUser = new User
            {
                Id = userId,
                RoleId = userRoleId
            };

            if (currentUser.IsTeacher)
            {
                // Return current user only
                data = data.Where(x => x.UserId == currentUser.Id).ToList();
            }

            return data;
        }

        public List<StudentTestDistrictTerm> ReportingGetClasses(int userId, int userRoleId, int districtId,
            int schoolId, int teacherId, int? termId, string virtualTestIds, string virtualTestSubTypeIds, DateTime? resultDateFrom,
            DateTime? resultDateTo, bool isGetAllClass = false)
        {
            var virtualTestIdsList = virtualTestIds.ToIntList(",");
            var virtualTestSubTypeIdsList = virtualTestSubTypeIds.ToIntList(";");
            var data = new List<StudentTestDistrictTerm>();

            if (isGetAllClass)
            {
                var param = new StudentTestDistrictTermParam
                {
                    DistrictId = districtId,
                    SchoolId = schoolId,
                    TeacherId = teacherId,
                    DistrictTermId = termId,
                    VirtualTestIds = virtualTestIdsList,
                    VirtualTestSubTypeIds = virtualTestSubTypeIdsList,
                    ResultDateFrom = resultDateFrom,
                    ResultDateTo = resultDateTo
                };
                data = _teacherDistrictTermService.GetStudentTestDistrictTerm_New(param).ToList();
            }
            else
            {
                foreach (var subTypeId in virtualTestSubTypeIdsList)
                {
                    var tempData = _teacherDistrictTermService
                        .GetTeacherTestDistrictTerm(districtId, schoolId, teacherId, termId, virtualTestIdsList, subTypeId, resultDateFrom, resultDateTo)
                        .Select(x => new StudentTestDistrictTerm
                        {
                            ClassId = x.ClassId,
                            ClassName = x.ClassName,
                            DateEnd = x.DateEnd,
                            DateStart = x.DateStart,
                            DistrictId = x.DistrictId,
                            DistrictTermId = x.DistrictTermId,
                            DistrictTermName = x.DistrictTermName,
                            NameFirst = x.NameFirst,
                            NameLast = x.NameLast,
                            ResultDate = x.ResultDate,
                            SchoolId = x.SchoolId,
                            SchoolName = x.SchoolName,
                            UserId = x.UserId,
                            UserName = x.UserName,
                            VirtualTestId = x.VirtualTestId,
                            VirtualTestSubTypeId = x.VirtualTestSubTypeId
                        }).ToList();

                    if (tempData.Any())
                    {
                        data = data.Union(tempData).ToList();
                    }
                }
            }

            return data;
        }

        public List<TeacherTestDistrictTerm> ReportingGetAllClasses(int userId, int userRoleId, int districtId, 
            string virtualTestIds, string virtualTestSubTypeIds, DateTime? resultDateFrom, DateTime? resultDateTo)
        {
            var listIds = ConvertListIdFromString(virtualTestSubTypeIds);

            var data = new List<TeacherTestDistrictTerm>();
            foreach (var subTypeId in listIds)
            {
                var virtualTestIdsList = virtualTestIds.ToIntList(",");
                var tempData = _teacherDistrictTermService.GetTeacherTestDistrictTerm(
                districtId, null, null, null, virtualTestIdsList, subTypeId, resultDateFrom, resultDateTo).ToList();
                if (tempData.Any())
                {
                    data = data.Union(tempData).ToList();
                }
            }

            return data;
        }

        public List<StudentTestDistrictTerm> ReportingGetTerms(
            int userId,
            int userRoleId,
            int districtId,
            int schoolId,
            int teacherId,
            string virtualTestIds,
            string virtualTestSubTypeIds,
            DateTime? resultDateFrom,
            DateTime? resultDateTo,
            bool isGetAllClass = false)
        {
            var virtualTestIdsList = virtualTestIds.ToIntList(",");
            var virtualTestSubTypeIdsList = virtualTestSubTypeIds.ToIntList(";");
            var data = new List<StudentTestDistrictTerm>();

            if (isGetAllClass)
            {
                var param = new StudentTestDistrictTermParam
                {
                    DistrictId = districtId,
                    SchoolId = schoolId,
                    TeacherId = teacherId,
                    VirtualTestIds = virtualTestIdsList,
                    VirtualTestSubTypeIds = virtualTestSubTypeIdsList,
                    ResultDateFrom = resultDateFrom,
                    ResultDateTo = resultDateTo
                };
                data = _teacherDistrictTermService.GetStudentTestDistrictTerm_New(param).ToList();
            }
            else
            {
                foreach (var subTypeId in virtualTestSubTypeIdsList)
                {
                    var tempData = new List<StudentTestDistrictTerm>();

                    if (!isGetAllClass)
                    {
                        tempData = _teacherDistrictTermService.GetTeacherTestDistrictTerm(districtId,
                        schoolId, teacherId, null, virtualTestIdsList, subTypeId, resultDateFrom, resultDateTo)
                        .Select(x => new StudentTestDistrictTerm
                        {
                            ClassId = x.ClassId,
                            ClassName = x.ClassName,
                            DateEnd = x.DateEnd,
                            DateStart = x.DateStart,
                            DistrictId = x.DistrictId,
                            DistrictTermId = x.DistrictTermId,
                            DistrictTermName = x.DistrictTermName,
                            NameFirst = x.NameFirst,
                            NameLast = x.NameLast,
                            ResultDate = x.ResultDate,
                            SchoolId = x.SchoolId,
                            SchoolName = x.SchoolName,
                            UserId = x.UserId,
                            UserName = x.UserName,
                            VirtualTestId = x.VirtualTestId,
                            VirtualTestSubTypeId = x.VirtualTestSubTypeId
                        }).ToList();
                    }

                    if (tempData.Any())
                    {
                        data = data.Union(tempData).ToList();
                    }
                }
            }

            return data;
        }

        private List<int> ConvertListIdFromString(string ids)
        {
            string[] idStrings = ids.Split(new char[] { ';' },
                StringSplitOptions.RemoveEmptyEntries);
            var listIds = new List<int>();
            foreach (var idString in idStrings)
            {
                int temp;
                if (int.TryParse(idString, out temp))
                {
                    listIds.Add(temp);
                }
            }
            return listIds;
        }
    }
}
