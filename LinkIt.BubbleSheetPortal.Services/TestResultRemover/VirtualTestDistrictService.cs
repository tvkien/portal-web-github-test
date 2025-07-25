using System.Collections.Generic;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories.PerformanceBandAutomations;
using LinkIt.BubbleSheetPortal.Data.Repositories.TestResultRemover;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs;
using LinkIt.BubbleSheetPortal.Models.DTOs.PerformanceBandAutomations;
using LinkIt.BubbleSheetPortal.Models.GenesisGradeBook;
using LinkIt.BubbleSheetPortal.Models.TestResultRemover;

namespace LinkIt.BubbleSheetPortal.Services.TestResultRemover
{
    public class VirtualTestDistrictService
    {
        private readonly IVirtualTestDistrictRepository repository;
        private readonly IDataSetCategoryRepository dataSetCategoryRepository;
        private readonly IPerformanceBandAutomationRepository performanceBandAutomationRepository;

        public VirtualTestDistrictService(
            IVirtualTestDistrictRepository repository,
            IDataSetCategoryRepository dataSetCategoryRepository,
            IPerformanceBandAutomationRepository performanceBandAutomationRepository)
        {
            this.repository = repository;
            this.dataSetCategoryRepository = dataSetCategoryRepository;
            this.performanceBandAutomationRepository = performanceBandAutomationRepository;
        }

        public IQueryable<VirtualTestDistrict> GetVirtualTestByDistrictId(int districtId, bool isRegrader)
        {
            if (isRegrader)
            {
                return repository.Select().Where(x => x.DistrictId.Equals(districtId) && x.VirtualTestSourceId != 3).OrderBy(o => o.Name);
            }
            return repository.Select().Where(x => x.DistrictId.Equals(districtId)).OrderBy(o => o.Name);
        }

        /// <summary>
        /// Get VirtualTest By District & UserRole
        /// </summary>
        /// <param name="districtId"></param>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <param name="isRegrader"></param>
        /// <returns></returns>
        public IQueryable<ListItem> GetVirtualTestByRole(int districtId, int userId, int roleId, bool isRegrader)
        {

            int regrader = 0;
            if (isRegrader)
                regrader = 1;

            var vResult = repository.GetVirtualTestDistricts(districtId, userId, roleId, 0, 0, 0, 0, regrader);

            var vListItem = vResult.Select(o => new ListItem()
            {
                Id = o.VirtualTestId,
                Name = o.Name
            });

            return vListItem;
        }

        /// <summary>
        /// Get VirtualTest By District & UserRole
        /// </summary>
        /// <param name="districtId"></param>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public IQueryable<ListItem> GetVirtualTestValideTermByRole(int districtId, int userId, int roleId)
        {
            var vResult = repository.GetVirtualTestValidTermByDistrict(districtId, userId, roleId, 0, 0, 0, 0);

            var vListItem = vResult.Select(o => new ListItem()
            {
                Id = o.VirtualTestId,
                Name = o.Name
            });

            return vListItem;

        }

        /// <summary>
        /// Get VirtualTest by District & Userrole with filter
        /// </summary>
        /// <param name="districtId"></param>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <param name="isRegrader"></param>
        /// <param name="schoolId"></param>
        /// <param name="teacherId"></param>
        /// <param name="classId"></param>
        /// <param name="studentId"></param>
        /// <returns></returns>
        public IQueryable<ListItem> GetVirtualTestDistrictFilterByRole(int districtId, int userId, int roleId, bool isRegrader, int schoolId, int teacherId, int classId, int studentId)
        {

            int regrader = 0;
            if (isRegrader)
                regrader = 1;

            var vResult = repository.GetVirtualTestDistricts(districtId, userId, roleId, schoolId, teacherId, classId, studentId, regrader);

            var vListItem = vResult.Select(o => new ListItem()
            {
                Id = o.VirtualTestId,
                Name = o.Name
            });

            return vListItem;

        }

        public IQueryable<ListItem> GetTestBySchoolAndDistrictTerm(int districtId, int schoolId, int districtTermId, int userId, int roleId)
        {
            return repository.GetTestBySchoolAndDistrictTerm(districtId, schoolId, districtTermId, userId, roleId);
        }

        public List<DisplayTestResultDistrictCustom> GetRemoveTestResultFilterByRole(RemoveTestResultsDetail input, ref int? totalRecords)
        {
            var vResult = repository.GetRemoveTestResutProcByRole(input, ref totalRecords);
            return vResult.ToList();
        }

        public List<DisplayTestResultDistrictCustomV2> GetRemoveTestResultFilterByRoleV2(RemoveTestResultsDetailV2 input)
        {
            var vResult = repository.GetRemoveTestResutProcByRoleV2(input);
            return vResult.ToList();
        }

        public List<DisplayVirtualTestDistrictCustomV2> GetRemoveVirtualTestFilterByRoleV2(RemoveVirtualTestDetailV2 input, ref int? totalRecords, ref int? totalStudents, ref int? totalTestResults)
        {
            return repository.GetRemoveVirtualTestProcByRoleV2(input, ref totalRecords, ref totalStudents, ref totalTestResults).ToList();
        }

        public IQueryable<ListItem> GetVirtualTestForRegrader(int districtId, int userId, int roleId, int schoolId, int termId)
        {
            var vResult = repository.GetVirtualTestDistrictsForRegrader(districtId, userId, roleId, schoolId, termId);

            var vListItem = vResult.Select(o => new ListItem()
            {
                Id = o.VirtualTestId,
                Name = o.Name
            });

            return vListItem.OrderBy(o => o.Name);
        }
        /// <summary>
        /// Get Class Have TestResult By District & UserRole
        /// </summary>
        /// <param name="districtId"></param>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <param name="isRegrader"></param>
        /// <returns></returns>
        public IQueryable<ListItem> GetClassDistrictByRole(int districtId, int userId, int roleId, bool isRegrader)
        {
            int regrader = 0;
            if (isRegrader)
                regrader = 1;
            var vResult = repository.GetClassDistrictByRole(districtId, userId, roleId, 0, 0, 0, 0, regrader);
            var vListItem2 = vResult.Select(o => new ListItem()
            {
                Id = o.ClassId,
                Name = o.Name
            });
            return vListItem2;
        }

        public IQueryable<ListItem> GetClassValidTermByDistrictAndRole(int districtId, int userId, int roleId)
        {
            var vResult = repository.GetClassValidTermByDistrictRole(districtId, userId, roleId, 0, 0, 0, 0);
            var vListItem2 = vResult.Select(o => new ListItem()
            {
                Id = o.ClassId,
                Name = o.Name
            });
            return vListItem2.AsQueryable().OrderBy(o => o.Name);
        }

        /// <summary>
        /// Get Class have TestResult by district & userrole with filter
        /// </summary>
        /// <param name="districtId"></param>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <param name="isRegrader"></param>
        /// <param name="virtualTestId"></param>
        /// <param name="studentId"></param>
        /// <param name="schoolId"></param>
        /// <param name="teacherId"></param>
        /// <returns></returns>
        public IQueryable<ListItem> GetClassDistrictFilterByRole(int districtId, int userId, int roleId, bool isRegrader, int virtualTestId, int studentId, int schoolId, int teacherId)
        {
            int regrader = 0;
            if (isRegrader)
                regrader = 1;

            var vResult = repository.GetClassDistrictByRole(districtId, userId, roleId, virtualTestId, studentId, schoolId, teacherId, regrader);

            var vListItem = vResult.Select(o => new ListItem()
            {
                Id = o.ClassId,
                Name = o.Name
            }).ToList();
            return vListItem.AsQueryable().OrderBy(o => o.Name);
        }
        public IQueryable<ListItem> GetClassDistrictFilterByRoleHasTestResult(int districtId, int userId, int roleId, int schoolId, int termId)
        {
            var vResult = repository.GetClassDistrictByRoleHasTestResult(districtId, userId, roleId, schoolId, termId);

            var vListItem = vResult.Select(o => new ListItem()
            {
                Id = o.ClassId,
                Name = o.Name
            }).ToList();
            return vListItem.AsQueryable().OrderBy(o => o.Name);
        }

        public IQueryable<ListItem> GetClassTestResultTermValidByDistrictAndRole(int districtId, int userId, int roleId, int virtualTestId, int studentId, int schoolId, int teacherId)
        {
            var vResult = repository.GetClassValidTermByDistrictRole(districtId, userId, roleId, virtualTestId, studentId, schoolId, teacherId);
            var vListItem = vResult.Select(o => new ListItem()
            {
                Id = o.ClassId,
                Name = o.Name
            }).ToList();
            return vListItem.AsQueryable().OrderBy(o => o.Name);
        }

        /// <summary>
        /// Get Student By District & UserRole with custom Student Name
        /// </summary>
        /// <param name="districtId"></param>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <param name="isRegrader"></param>
        /// <returns></returns>
        public IQueryable<ListItem> GetStudentDistrictByRole(int districtId, int userId, int roleId, bool isRegrader)
        {
            int regrader = 0;
            if (isRegrader)
                regrader = 1;

            var vResult = repository.GetStudentDistrictByRole(districtId, userId, roleId, 0, 0, 0, 0, regrader);

            var vListItem = vResult.Select(o => new ListItem()
            {
                Id = o.StudentId,
                Name = o.StudentCustom
            });
            return vListItem;
        }

        public IQueryable<ListItem> GetStudentValidTermByDistrictAndRole(int districtId, int userId, int roleId)
        {
            var vResult = repository.GetStudentValidTermByDistrictAndRole(districtId, userId, roleId, 0, 0, 0, 0);

            var vListItem = vResult.Select(o => new ListItem()
            {
                Id = o.StudentId,
                Name = o.StudentCustom
            });
            return vListItem.AsQueryable().OrderBy(o => o.Name);
        }

        public IQueryable<ListItem> GetStudentValidTermByDistrictAndRole(int districtId, int userId, int roleId, int schoolId, int teacherId, int classId, int virtualTestId)
        {
            var vResult = repository.GetStudentValidTermByDistrictAndRole(districtId, userId, roleId, schoolId, teacherId, classId, virtualTestId);

            var vListItem = vResult.Select(o => new ListItem()
            {
                Id = o.StudentId,
                Name = o.StudentCustom
            }).ToList();
            return vListItem.AsQueryable().OrderBy(o => o.Name);
        }

        /// <summary>
        /// Get student by district & user role with filter
        /// </summary>
        /// <param name="districtId"></param>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <param name="isRegrader"></param>
        /// <param name="schoolId"></param>
        /// <param name="teacherId"></param>
        /// <param name="classId"></param>
        /// <param name="virtualTestId"></param>
        /// <returns></returns>
        public IQueryable<ListItem> GetStudentDistrictFilterByRole(int districtId, int userId, int roleId, bool isRegrader, int schoolId, int teacherId, int classId, int virtualTestId)
        {
            var regrader = isRegrader ? 1 : 0;
         
            var vResult = repository.GetStudentDistrictByRole(districtId, userId, roleId, schoolId, teacherId, classId, virtualTestId, regrader);

            var vListItem = vResult.Select(o => new ListItem()
            {
                Id = o.StudentId,
                Name = o.StudentCustom
            }).ToList();
            return vListItem.AsQueryable().OrderBy(o => o.Name);
        }

        /// <summary>
        /// Get School by District & user role with filter
        /// </summary>
        /// <param name="districtId"></param>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <param name="isRegrader"></param>
        /// <param name="teacherId"></param>
        /// <param name="classId"></param>
        /// <param name="studentId"></param>
        /// <param name="virtualTestId"></param>
        /// <returns></returns>
        public IQueryable<ListItem> GetSchoolDistrictFilterByRole(int districtId, int userId, int roleId, bool isRegrader, int teacherId, int classId, int studentId, int virtualTestId)
        {
            int regrader = 0;
            if (isRegrader)
                regrader = 1;

            var vResult = repository.GetSchoolDistrictByRole(districtId, userId, roleId, teacherId, classId, studentId, virtualTestId, regrader);


            var vListItem = vResult.Select(o => new ListItem()
            {
                Id = o.SchoolId,
                Name = o.Name
            });

            return vListItem;
        }
        public IQueryable<ListItem> GetSchoolDistrictFilterByRoleRegrader(int districtId, int userId, int roleId)
        {
            var vResult = repository.GetSchoolDistrictByRoleReGrader(districtId, userId, roleId);
            var vListItem = vResult.Select(o => new ListItem()
            {
                Id = o.SchoolId,
                Name = o.Name
            }).OrderBy(x => x.Name);
            return vListItem;
        }
        public IQueryable<ListItem> GetSchoolValidTermByDistrictAndRole(int districtId, int userId, int roleId, int teacherId, int classId, int studentId, int virtualTestId)
        {
            var vResult = repository.GetSchoolValidTermByDistrictAndRole(districtId, userId, roleId, teacherId, classId, studentId, virtualTestId);

            var vListItem = vResult.Select(o => new ListItem()
            {
                Id = o.SchoolId,
                Name = o.Name
            });
            return vListItem.AsQueryable().OrderBy(o => o.Name);
        }

        public IQueryable<ListItem> GetSchoolValidTermByDistrictForRemoveTestResult(int districtId, int userId, int roleId, int teacherId, int classId, int studentId, int virtualTestId)
        {
            var vResult = repository.GetSchoolValidTermByDistrictForRemoveTestResults(districtId, userId, roleId, teacherId, classId, studentId, virtualTestId);

            var vListItem = vResult.Select(o => new ListItem()
            {
                Id = o.SchoolId,
                Name = o.Name
            });
            return vListItem.AsQueryable().OrderBy(o => o.Name);
        }

        public IQueryable<ListItem> GetAllSubjects()
        {
            return repository.GetAllSubjects();
        }

        /// <summary>
        /// Get Primary teacher by district & user role with filter
        /// </summary>
        /// <param name="districtId"></param>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <param name="isRegrader"></param>
        /// <param name="schoolId"></param>
        /// <param name="classId"></param>
        /// <param name="studentId"></param>
        /// <param name="virtualTestId"></param>
        /// <returns></returns>
        public IQueryable<ListItem> GetPrimaryTeacherDistrictFilterByRole(int districtId, int userId, int roleId, bool isRegrader, int schoolId, int classId, int studentId, int virtualTestId)
        {
            int regrader = 0;
            if (isRegrader)
                regrader = 1;
            var vResult = repository.GetPrimaryTeacherDistrictByRole(districtId, userId, roleId, schoolId, classId, studentId, virtualTestId, regrader);

            var vListItem = vResult.Select(o => new ListItem()
            {
                Id = o.UserId,
                Name = o.UserName
            });
            return vListItem;
        }
        public IQueryable<ListItem> GetPrimaryTeacherValidTermByDistrictAndRole(int districtId, int userId, int roleId, int schoolId, int classId, int studentId, int virtualTestId)
        {
            var vResult = repository.GetPrimaryTeacherValidTermByDistrictAndRole(districtId, userId, roleId, schoolId, classId, studentId, virtualTestId);
            var vListItem = vResult.Select(o => new ListItem()
            {
                Id = o.UserId,
                Name = o.UserName
            });
            return vListItem.AsQueryable().OrderBy(o => o.Name);
        }

        /// <summary>
        /// Get Primary Teacher by district & User role 
        /// </summary>
        /// <param name="districtId"></param>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <param name="isRegrader"></param>
        /// <returns></returns>
        public IQueryable<ListItem> GetPrimaryTeacherDistrictByRole(int districtId, int userId, int roleId, bool isRegrader)
        {
            int regrader = 0;
            if (isRegrader)
                regrader = 1;

            var vResult = repository.GetPrimaryTeacherDistrictByRole(districtId, userId, roleId, 0, 0, 0, 0, regrader);


            var vListItem = vResult.Select(o => new ListItem()
            {
                Id = o.UserId,
                Name = o.UserName
            });
            return vListItem;
        }

        public IQueryable<ListItem> GetPrimaryTeacherValidTermByDistrictAndRole(int districtId, int userId, int roleId)
        {
            var vResult = repository.GetPrimaryTeacherValidTermByDistrictAndRole(districtId, userId, roleId, 0, 0, 0, 0);


            var vListItem = vResult.Select(o => new ListItem()
            {
                Id = o.UserId,
                Name = o.UserName
            });
            return vListItem.AsQueryable().OrderBy(o => o.Name);
        }

        /// <summary>
        /// Get Test Result custom by Filter [Test Remover, Test Regrade]
        /// </summary>
        /// <param name="districtId"></param>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <param name="schoolId"></param>
        /// <param name="teacherName"></param>
        /// <param name="classId"></param>
        /// <param name="studentName"></param>
        /// <param name="virtualTestId"></param>
        /// <param name="termId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalRecords"></param>
        /// <param name="sortColumns"></param>
        /// <param name="isRegrader"></param>
        /// <param name="testPeriod"></param>
        /// <returns></returns>
        public List<DisplayTestResultDistrict> GetTestResultToView(int districtId, int userId, int roleId, int schoolId, string teacherName,
            int classId, string studentName, int virtualTestId, int termId, int pageIndex, int pageSize, ref int? totalRecords, string sortColumns,
            int isRegrader, string testPeriod, string generalSearch)
        {
            var vResult = repository.GetTestResutDistrictProcByRole(districtId, userId, roleId, schoolId, teacherName, classId, studentName,
                virtualTestId, termId, testPeriod, pageIndex, pageSize, ref totalRecords, sortColumns, isRegrader, generalSearch);

            return vResult.ToList();
        }

        public IQueryable<ListItem> GetTermsDistrictFilterByRole(int districtId, int userId, int roleId, int virtualTestId, int studentId, int classId, int schoolId, int teacherId, bool isRegrader)
        {
            int regrader = 0;
            if (isRegrader)
                regrader = 1;

            var vResult = repository.GetTermDistrictByRole(districtId, userId, roleId, virtualTestId, studentId, classId, schoolId, teacherId, regrader);

            var vListItem = vResult.Select(o => new ListItem()
            {
                Id = o.DistrictTermId,
                Name = o.Name
            }).ToList();
            return vListItem.AsQueryable().OrderBy(o => o.Name);
        }

        public IQueryable<ListItem> GetDistrictTermValidFilterByRole(int districtId, int userId, int roleId, int virtualTestId, int studentId, int classId, int schoolId, int teacherId)
        {
            var vResult = repository.GetDistrictTermValidFilterByRole(districtId, userId, roleId, virtualTestId, studentId, classId, schoolId, teacherId);

            var vListItem = vResult.Select(o => new ListItem()
            {
                Id = o.DistrictTermId,
                Name = o.Name
            }).ToList();
            return vListItem.AsQueryable().OrderBy(o => o.Name);
        }
        public IQueryable<ListItem> GetTermsDistrictFilterForRegrader(int districtId, int userId, int roleId, int? classId, int schoolId)
        {
            var vResult = repository.GetTermDistrictByRoleForRegrader(districtId, userId, roleId, classId, schoolId);

            var vListItem = vResult.Select(o => new ListItem()
            {
                Id = o.DistrictTermId,
                Name = o.Name
            }).ToList();
            return vListItem.AsQueryable().OrderBy(o => o.Name);
        }

        public IQueryable<ListItem> GetFullDistrictTermValidFilterByRole(int districtId, int userId, int roleId, int virtualTestId, int studentId, int classId, int schoolId, int teacherId)
        {
            var vResult = repository.GetFullDistrictTermValidFilterByRole(districtId, userId, roleId, virtualTestId, studentId, classId, schoolId, teacherId);

            var vListItem = vResult.Select(o => new ListItem()
            {
                Id = o.DistrictTermId,
                Name = o.Name
            }).ToList();
            return vListItem.AsQueryable().OrderBy(o => o.Name);
        }

        public List<TermDistrictWithStatus> GetFullDistrictTerm(int districtId, int userId, int roleId, int virtualTestId, int studentId, int classId, int schoolId, int teacherId)
        {
            return
                repository.GetFullDistrictTermValidFilterByRole(districtId, userId, roleId, virtualTestId, studentId,
                    classId, schoolId, teacherId).Select(x => new TermDistrictWithStatus()
                    {
                        Id = x.DistrictTermId,
                        Name = x.Name
                    }).ToList();
        }

        public List<TermDistrictWithStatus> GetFullDistrictTermV2(int districtId, int userId, int roleId, int virtualTestId, int studentId, int classId, string schoolIds, int teacherId)
        {
            return
                repository.GetFullDistrictTermValidFilterByRoleV2(districtId, userId, roleId, virtualTestId, studentId,
                    classId, schoolIds, teacherId).Select(x => new TermDistrictWithStatus()
                    {
                        Id = x.DistrictTermId,
                        Name = x.Name
                    }).ToList();
        }

        public List<TestResultLog> GetTestResultDetails(string testresultIds)
        {
            return repository.GetTestResultDetails(testresultIds).OrderBy(x => x.TestResultID).ToList();
        }

        public List<TestResultScoreLog> GetTestResultScores(string testresultIds)
        {
            return repository.GetTestResultScores(testresultIds).OrderBy(x => x.TestResultID).ThenBy(x => x.TestResultScoreID).ToList();
        }
        public List<TestResultSubScoreLog> GetTestResultSubScores(string listTestResultScoreId)
        {
            return repository.GetTestResultSubScores(listTestResultScoreId).OrderBy(x => x.TestResultScoreID).ThenBy(x => x.TestResultSubScoreID).ToList();
        }

        public List<TestResultProgramLog> GetTestResultProgram(string testresultIds)
        {
            return repository.GetTestResultProgram(testresultIds).OrderBy(x => x.TestResultID).ThenBy(x => x.TestResultProgramID).ToList();
        }

        public List<AnswerLog> GetAnswersByTestResultId(string testresultIds)
        {
            return repository.GetAnswersByTestResultId(testresultIds).OrderBy(x => x.TestResultID).ThenBy(x => x.AnswerID).ToList();
        }

        public List<AnswerSubLog> GetAnswerSubsByAnswerId(string listAnswerId)
        {
            return repository.GetAnswerSubsByAnswerId(listAnswerId).OrderBy(x => x.AnswerID).ThenBy(x => x.AnswerSubID).ToList();
        }

        public List<DisplayTestResultDistrictCustom> GetTestResultRetagged(TestResultDataModel model, ref int? totalRecords)
        {
            var vResult = repository.GetTestResutRetaggedProcByRole(model, ref totalRecords);
            return vResult.ToList();

        }

        public bool IsExistAutoGradingQueueBeingGraded(string testResultIds)
        {
            var isExist = repository.IsExistAutoGradingQueueBeingGraded(testResultIds);
            if (isExist.HasValue)
                return isExist.Value;

            return false;
        }

        public CategoriesGradesAndSubjectsModel GetCategoriesGradesAndSubjects(int districtId, string schoolIds)
        {
            var result = new CategoriesGradesAndSubjectsModel();
            var list = performanceBandAutomationRepository.GetTestTypeGradeAndSubject(new TestTypeGradeAndSubjectForPBSFilter { DistrictId = districtId, SchoolIds = schoolIds });
            result.Categories = list
                .Where(c => string.Equals(c.Kind, "TestType"))
                .OrderBy(x => x.Name)
                .Select(c => new SelectListItemDTO { Id = c.ID ?? 0, Name = c.Name })
                .ToList();
            result.Grades = list
                .Where(c => string.Equals(c.Kind, "Grade"))
                .OrderBy(x => x.Order)
                .Select(c => new SelectListItemDTO { Id = c.ID ?? 0, Name = c.Name })
                .ToList();
            result.Subjects = list
                .Where(c => string.Equals(c.Kind, "Subject"))
                .OrderBy(x => x.Name)
                .Select(c => new SelectListItemDTO { Id = c.ID ?? 0, Name = c.Name })
                .ToList();
            return result;
        }
    }
}
