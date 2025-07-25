using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using dotless.Core.Parser.Tree;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Models.RemoveTestResults;
using LinkIt.BubbleSheetPortal.Web.Resolver;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using RestSharp.Extensions;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize(Order = 2)]
    public class RemoveTestResultsController : BaseController
    {
        private readonly RemoveTestResultsControllerParameters _parameters;

        public RemoveTestResultsController(RemoveTestResultsControllerParameters param)
        {
            _parameters = param;
        }

        //
        // GET: /TestResultTransfer/
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.TmgmtTestResultTransfer)]
        public ActionResult Index()
        {
            var model = new TestResultTransferViewModel()
            {
                IsAdmin = _parameters.UserServices.GetAdminByUsernameAndDistrict(CurrentUser.UserName, CurrentUser.DistrictId.GetValueOrDefault()).IsNotNull(),
                CanSelectTeachers = CurrentUser.IsDistrictAdminOrPublisher,
                IsSchoolAdmin = CurrentUser.RoleId.Equals(8),
                IsTeacher = CurrentUser.RoleId.Equals(2),
                IsPublisher = CurrentUser.RoleId.Equals((int)Permissions.Publisher),
                DistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0,
                IsNetworkAdmin = CurrentUser.IsNetworkAdmin
            };
            return View(model);
        }

        public ActionResult LoadTestFilter()
        {
            TestFilterViewModel model = new TestFilterViewModel();
            model.DistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;
            model.IsPublisher = CurrentUser.IsPublisher();
            model.IsTeacher = CurrentUser.IsTeacher();
            model.UserID = CurrentUser.Id;
            if (CurrentUser.IsNetworkAdmin)
            {
                model.IsNetworkAdmin = true;
                model.ListDistricIds = CurrentUser.GetMemberListDistrictId();
            }

            return PartialView("_TestFilter", model);
        }

         public ActionResult LoadTestFilterV2()
         {
            TestFilterViewModel model = new TestFilterViewModel();
            model.DistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;
            model.IsPublisher = CurrentUser.IsPublisher();
            model.IsTeacher = CurrentUser.IsTeacher();
            model.UserID = CurrentUser.Id;
            if (CurrentUser.IsNetworkAdmin)
            {
                model.IsNetworkAdmin = true;
                model.ListDistricIds = CurrentUser.GetMemberListDistrictId();
            }

            return PartialView("v2/_TestFilter", model);
         }

        public ActionResult LoadTestResultByFilter(int districtId, int virtualTestId, int classId,
                                               string studentName, int schoolId, string teacherName, int termId, int timePeriod)
        {
            DisplayTestResultFilterViewModel model = new DisplayTestResultFilterViewModel()
            {
                DistrictId = CurrentDistrict(districtId),
                VirtualTestId = virtualTestId,
                ClassId = classId,
                StudentName = studentName,
                SchoolId = schoolId,
                TeacherName = teacherName,
                TermrId = termId,
                TimePeriod = timePeriod

            };
            return PartialView("_TestResultByFilter", model);
        }

        public ActionResult LoadTestResultByFilterV2(DisplayTestResultFilterV2ViewModel model)
        {
            return PartialView("v2/_TestResultByFilter", model);
        }

        [HttpGet]
        public ActionResult GetSchoolTestResultDistrict(int districtId, int virtualTestId, int teacherId, int classId, int studentId)
        {
            //Use Store
            var data = GetSchoolDistrictFilterByRole(CurrentDistrict(districtId), teacherId, classId, studentId, virtualTestId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetClassTestResultDistrict(int districtId, int virtualTestId, int studentId, int schoolId, int teacherId)
        {
            if (districtId > 0 && !Util.HasRightOnDistrict(CurrentUser, districtId))
            {
                return Json(new { error = "Has no right on the selected district." }, JsonRequestBehavior.AllowGet);
            }

            if (virtualTestId > 0 && !HasRightOnVirtualTestByRole(districtId, false, virtualTestId))
            {
                return Json(new { error = "Has no right on the selected virtual test." }, JsonRequestBehavior.AllowGet);
            }

            if (teacherId > 0 && !HasRightOnPrimaryTeacher(districtId, false, teacherId))
            {
                return Json(new { error = "Has no right on the selected teacher." }, JsonRequestBehavior.AllowGet);
            }

            if (schoolId > 0 && !HasRightOnSchoolDistrictFilterByRole(districtId, teacherId, 0, studentId, virtualTestId, schoolId))
            {
                return Json(new { error = "Has no right on the selected school." }, JsonRequestBehavior.AllowGet);
            }

            var data = _parameters.VirtualTestDistrictServices.GetClassTestResultTermValidByDistrictAndRole(districtId, CurrentUser.Id,
                CurrentUser.RoleId, virtualTestId, studentId, schoolId, teacherId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetClassBySchoolAndTerm(int? districtId, int? schoolId, int? districtTermId)
        {
            var data = _parameters.ClassServices.GetClassBySchoolAndTerm(CurrentDistrict(districtId), schoolId.GetValueOrDefault(), districtTermId.GetValueOrDefault(), CurrentUser.Id, CurrentUser.RoleId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetClassBySchoolAndTermV2(int? districtId, string schoolIds, int districtTermId)
        {
            var data = _parameters.ClassServices.GetClassBySchoolAndTermV2(CurrentDistrict(districtId), schoolIds, districtTermId, CurrentUser.Id, CurrentUser.RoleId);
            return new JsonResult() { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = int.MaxValue };
        }

        [HttpGet]
        public ActionResult GetVirtualTestTestResultDistrict(int districtId, int classId, int studentId, int schoolId, int teacherId)
        {
            //Use store
            var data = _parameters.VirtualTestDistrictServices.GetVirtualTestDistrictFilterByRole(CurrentDistrict(districtId),
                CurrentUser.Id, CurrentUser.RoleId, false, schoolId, teacherId, classId, studentId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetTestBySchoolAndDistrictTerm(int? districtId, int? schoolId, int? districtTermId)
        {
            var data = _parameters.VirtualTestDistrictServices.GetTestBySchoolAndDistrictTerm(CurrentDistrict(districtId), schoolId.GetValueOrDefault(), districtTermId.GetValueOrDefault(), CurrentUser.Id, CurrentUser.RoleId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetTermTestResultDistrict(int? districtId, int classId, int studentId, int virtualTestId, int schoolId, int teacherId)
        {
            var result = _parameters.VirtualTestDistrictServices.GetFullDistrictTerm(CurrentDistrict(districtId), CurrentUser.Id, CurrentUser.RoleId, virtualTestId, studentId, classId, schoolId, teacherId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetTermTestResultDistrictV2(int? districtId, int classId, int studentId, int virtualTestId, string schoolIds, int teacherId)
        {
            var result = _parameters.VirtualTestDistrictServices.GetFullDistrictTermV2(CurrentDistrict(districtId), CurrentUser.Id, CurrentUser.RoleId, virtualTestId, studentId, classId, schoolIds, teacherId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetCategoriesGradesAndSubjects(int districtId, string schoolIds)
        {
            var result = _parameters.VirtualTestDistrictServices.GetCategoriesGradesAndSubjects(CurrentDistrict(districtId), schoolIds.ToIntCommaSeparatedString());
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTestResultToView(int districtId, int virtualTestId, int classId, string studentName, int schoolId, string teacherName, int termId, int timePeriod)
        {
            var parser = new DataTableParserProc<RemoveTestResultsViewModel>();
            int? totalRecords = 0;

            var isStudentInformationSystem = false;
            var districtDecode =
                _parameters.DistrictDecodeServices.GetDistrictDecodesOfSpecificDistrictByLabel(CurrentDistrict(districtId),
                    Util.IsStudentInformationSystem).FirstOrDefault();
            if (districtDecode != null)
                bool.TryParse(districtDecode.Value, out isStudentInformationSystem);

            var generalSearch = Request["sSearch"] ?? string.Empty;

            var input = new RemoveTestResultsDetail()
            {
                DistrictId = CurrentDistrict(districtId),
                UserId = CurrentUser.Id,
                RoleId = CurrentUser.RoleId,
                SchoolId = schoolId,
                TeacherName = teacherName,
                ClassId = classId,
                StudentName = studentName,
                VirtualTestId = virtualTestId,
                TermId = termId,
                DateCompare = GetDateTime(timePeriod).ToShortDateString(),
                StartIndex = parser.StartIndex,
                PageSize = parser.PageSize,
                SortColumns = parser.SortableColumns,
                GeneralSearch = generalSearch
            };

            var testResults = _parameters.VirtualTestDistrictServices.GetRemoveTestResultFilterByRole(input, ref totalRecords)
                .Select(x => new RemoveTestResultsViewModel()
                {
                    ID = x.TestResultId,
                    TestNameCustom = x.TestName,
                    SchoolName = x.SchoolName,
                    TeacherCustom = x.TeacherCustom,
                    ClassNameCustom = x.ClassNameCustom,
                    StudentCustom = x.StudentCustom,
                    ResultDate = x.ResultDate.GetValueOrDefault()
                }).AsQueryable();

            return Json(parser.Parse(testResults, totalRecords ?? 0), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetTestResultToViewV2(int districtId, string virtualTestName, int classId, string studentName, string schoolIds, string teacherName, int termId,
            string categoryIds, string gradeIds, string subjectNames, DateTime? fromResultDate, DateTime? toResultDate, DateTime? fromCreatedDate, DateTime? toCreatedDate,
            DateTime? fromUpdatedDate, DateTime? toUpdatedDate, bool hasStudentGeneralSearch = true)
        {
            var parser = new DataTableParserProc<RemoveTestResultsV2ViewModel>();

            var isStudentInformationSystem = false;
            var districtDecode =
                _parameters.DistrictDecodeServices.GetDistrictDecodesOfSpecificDistrictByLabel(CurrentDistrict(districtId),
                    Util.IsStudentInformationSystem).FirstOrDefault();
            if (districtDecode != null)
                bool.TryParse(districtDecode.Value, out isStudentInformationSystem);

            var generalSearch = Request["sSearch"] ?? string.Empty;

            var input = new RemoveTestResultsDetailV2()
            {
                DistrictId = CurrentDistrict(districtId),
                SchoolIds = schoolIds.ToIntCommaSeparatedString(),
                CategoryIds = categoryIds.ToIntCommaSeparatedString(),
                GradeIds = gradeIds.ToIntCommaSeparatedString(),
                SubjectNames = subjectNames,
                TermId = termId,
                ClassId = classId,
                TeacherName = teacherName?.Trim(),
                StudentName = studentName?.Trim(),
                FromResultDate = fromResultDate,
                ToResultDate = toResultDate,
                FromCreatedDate = fromCreatedDate,
                ToCreatedDate = toCreatedDate,
                FromUpdatedDate = fromUpdatedDate,
                ToUpdatedDate = toUpdatedDate,
                VirtualTestName = virtualTestName?.Trim(),
                UserId = CurrentUser.Id,
                RoleId = CurrentUser.RoleId,
                StartIndex = parser.StartIndex,
                PageSize = parser.PageSize,
                SortColumns = parser.SortableColumns,
                GeneralSearch = generalSearch?.Trim(),
                HasStudentGeneralSearch = hasStudentGeneralSearch
            };

            var testResults = _parameters.VirtualTestDistrictServices.GetRemoveTestResultFilterByRoleV2(input);
            var removeTestResultsV2 = testResults.Select(x => new RemoveTestResultsV2ViewModel()
                {
                    ID = x.TestResultID,
                    VirtualTestName = x.VirtualTestName,
                    StudentName = x.StudentName,
                    ClassTermName = x.ClassTermName,
                    ResultDate = x.ResultDate.GetValueOrDefault(),
                    CategoryName = x.CategoryName,
                    GradeName = x.GradeName,
                    SubjectName = x.SubjectName
                }).AsQueryable();

            int totalRecords = testResults.FirstOrDefault()?.TotalRecords ?? 0;
            int totalStudents = testResults.FirstOrDefault()?.TotalStudents ?? 0;
            int totalVirtualTests = testResults.FirstOrDefault()?.TotalVirtualTests ?? 0;

            var data = new RemoveTestResultReturnedModel(parser.Parse(removeTestResultsV2, totalRecords), totalStudents, totalVirtualTests, totalRecords);
            return new JsonResult() { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = int.MaxValue };
        }

        [HttpGet]
        public ActionResult GetVirtualTestToViewV2(int districtId, string virtualTestName, int classId, string studentName, string schoolIds, string teacherName, int termId,
            string categoryIds, string gradeIds, string subjectNames, DateTime? fromResultDate, DateTime? toResultDate, DateTime? fromCreatedDate, DateTime? toCreatedDate, DateTime? fromUpdatedDate, DateTime? toUpdatedDate)
        {
            var parser = new DataTableParserProc<RemoveVirtualTestV2ViewModel>();
            int? totalRecords = 0;
            int? totalStudents = 0;
            int? totalTestResults = 0;

            var isStudentInformationSystem = false;
            var districtDecode =
                _parameters.DistrictDecodeServices.GetDistrictDecodesOfSpecificDistrictByLabel(CurrentDistrict(districtId),
                    Util.IsStudentInformationSystem).FirstOrDefault();
            if (districtDecode != null)
                bool.TryParse(districtDecode.Value, out isStudentInformationSystem);

            var generalSearch = Request["sSearch"] ?? string.Empty;

            var input = new RemoveVirtualTestDetailV2()
            {
                DistrictId = CurrentDistrict(districtId),
                SchoolIds = schoolIds.ToIntCommaSeparatedString(),
                CategoryIds = categoryIds.ToIntCommaSeparatedString(),
                GradeIds = gradeIds.ToIntCommaSeparatedString(),
                SubjectNames = subjectNames,
                TermId = termId,
                ClassId = classId,
                TeacherName = teacherName?.Trim(),
                StudentName = studentName?.Trim(),
                FromResultDate = fromResultDate,
                ToResultDate = toResultDate,
                FromCreatedDate = fromCreatedDate,
                ToCreatedDate = toCreatedDate,
                FromUpdatedDate = fromUpdatedDate,
                ToUpdatedDate = toUpdatedDate,
                VirtualTestName = virtualTestName?.Trim(),
                UserId = CurrentUser.Id,
                RoleId = CurrentUser.RoleId,
                StartIndex = parser.StartIndex,
                PageSize = parser.PageSize,
                SortColumns = parser.SortableColumns,
                GeneralSearch = generalSearch?.Trim()
            };

            var testResults = _parameters.VirtualTestDistrictServices.GetRemoveVirtualTestFilterByRoleV2(input, ref totalRecords, ref totalStudents, ref totalTestResults)
                .Select(x => new RemoveVirtualTestV2ViewModel()
                {
                    VirtualTestID = x.VirtualTestID,
                    VirtualTestName = x.VirtualTestName,
                    CategoryName = x.CategoryName,
                    GradeName = x.GradeName,
                    SubjectName = x.SubjectName,
                    ResultCount = x.ResultCount,
                    StudentNameList = x.StudentNameList,
                    StudentIDList = x.StudentIDList,
                    TestResultIDList = x.TestResultIDList,
                }).AsQueryable();

            var data = new RemoveTestResultReturnedModel(parser.Parse(testResults, totalRecords ?? 0), totalStudents ?? 0, totalRecords ?? 0, totalTestResults ?? 0);
            return new JsonResult() { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = int.MaxValue };
        }

        private DateTime GetDateTime(int days)
        {
            if (days == 0) return DateTime.MinValue.AddYears(1800);
            return DateTime.UtcNow.AddDays(-days).Date;
        }

        private bool HasRightOnSchoolDistrictFilterByRole(int districtId, int teacherId, int classId, int studentId, int virtualTestId, int schoolId)
        {
            var data = GetSchoolDistrictFilterByRole(districtId, teacherId, classId, studentId, virtualTestId);
            return data.Any(x => x.Id == schoolId);
        }

        private bool HasRightOnVirtualTestByRole(int districtId, bool isRegrader, int virtualTestId)
        {
            var data = GetVirtualTestByRole(districtId, isRegrader);
            return data.Any(x => x.Id == virtualTestId);
        }

        private bool HasRightOnPrimaryTeacher(int districtId, bool isRegrader, int teacherId)
        {
            //User store [Teacher is a primanry teacher of class, not author test]
            var data = GetPrimaryTeacherDistrictByRole(districtId, isRegrader);
            return data.Any(x => x.Id == teacherId);
        }

        private IQueryable<ListItem> GetVirtualTestByRole(int districtId, bool isRegrader)
        {
            //Using Store
            var vData = _parameters.VirtualTestDistrictServices.GetVirtualTestByRole(districtId, CurrentUser.Id, CurrentUser.RoleId, isRegrader);
            return vData;
        }

        private IQueryable<ListItem> GetPrimaryTeacherDistrictByRole(int districtId, bool isRegrader)
        {
            //User store [Teacher is a primary teacher of class, not author test]
            var data = _parameters.VirtualTestDistrictServices.GetPrimaryTeacherDistrictByRole(districtId, CurrentUser.Id, CurrentUser.RoleId, isRegrader);
            return data;
        }

        private IQueryable<ListItem> GetSchoolDistrictFilterByRole(int? districtId, int teacherId, int classId, int studentId, int virtualTestId)
        {
            var data = _parameters.VirtualTestDistrictServices.GetSchoolValidTermByDistrictForRemoveTestResult(CurrentDistrict(districtId), CurrentUser.Id, CurrentUser.RoleId, teacherId, classId, studentId, virtualTestId);
            return data;
        }

        public ActionResult LoadProgressRemove()
        {
            int batchConfig;
            if (!int.TryParse(ConfigurationManager.AppSettings["NumberOfTestResultRemover"], out batchConfig))
            {
                batchConfig = 1000;
            }
            ViewBag.BatchConfig = batchConfig;

            return PartialView("v2/_ProgressRemove");
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult DeleteTestResultAndSubItemsV2(string testResultIds)
        {
            try
            {
                if (_parameters.VulnerabilityService.CheckUserCanAccessTestResult(CurrentUser, testResultIds) == false)
                {
                    return Json(new { Success = false, Message = "Do not have permission" }, JsonRequestBehavior.AllowGet);
                }

                _parameters.ClassDistrictServices.DeleteTestResultAndSubItemV2(testResultIds.ToEnumerable());

                LogTestResultRemover(testResultIds);

                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public void LogTestResultRemover(string testResultIds)
        {
            // Log this action to AdminReportingLog
            try
            {
                var visitorsIPAddr = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrWhiteSpace(visitorsIPAddr))
                {
                    visitorsIPAddr = Request.ServerVariables["REMOTE_ADDR"];
                }

                _parameters.TestResultLogServices.SaveTestResultRemoverLog(new TestResultAudit
                {
                    UserId = CurrentUser.Id,
                    AuditDate = DateTime.UtcNow,
                    IPAddress = visitorsIPAddr,
                    TestResultIDs = testResultIds,
                    Type = ContaintUtil.Remover
                });
            }
            catch
            {
                //Non complain
            }
        }
    }
}
