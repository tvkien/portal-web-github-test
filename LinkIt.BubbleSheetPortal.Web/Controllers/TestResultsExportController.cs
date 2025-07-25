using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using LinkIt.BubbleSheetPortal.Models.Enum;
using LinkIt.BubbleSheetPortal.Models.GenesisGradeBook;
using LinkIt.BubbleSheetPortal.Common;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize(Order = 2)]
    [VersionFilter]
    public class TestResultsExportController : BaseController
    {
        private readonly TestResultsExportControllerParameters _parameters;

        public TestResultsExportController(TestResultsExportControllerParameters param)
        {
            _parameters = param;
        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.TmgmtTestResultExportGenesis)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult RetagTestResultsSelected(string lstTestResultIds, int districtId, bool? isGradebook, bool? isStudentRecord, bool? isCleverApi, bool? isExportRawScore)
        {
            //Check user permission to access those test result before transfer
            if (!_parameters.VulnerabilityService.CheckUserCanAccessTestResult(CurrentUser, lstTestResultIds))
            {
                return Json(new { Success = false, Error = "Do not have permission" });
            }

            var testResultIds = new List<int>();
            string[] arr = lstTestResultIds.Split(',');
            if (arr.Length > 0)
            {
                int item = 0;
                foreach (var s in arr)
                {
                    int.TryParse(s, out item);
                    if (item > 0)
                    {
                        testResultIds.Add(item);
                    }
                }
                if (testResultIds.Count > 0)
                {
                    _parameters.TestResultServices.RetagTestResults(new RetagTestResult()
                    {
                        ListTestResultIds = string.Join(",", testResultIds),
                        DistrictId = districtId,
                        UserId = CurrentUser.Id,
                        Gradebook = isGradebook.HasValue && isGradebook.Value,
                        StudentRecord = isStudentRecord.HasValue && isStudentRecord.Value,
                        CleverApi = isCleverApi.HasValue && isCleverApi.Value,
                        IsExportRawScore = isExportRawScore ?? false
                    });
                    return Json(new { Success = true, Data = testResultIds.Count }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { Success = false, Error = "No testresult selected" }, JsonRequestBehavior.AllowGet);
        }

        #region TestResult Filter

        public ActionResult LoadTestResultByFilter(int districtId, int virtualTestId, int classId, string studentName, int schoolId, string teacherName, int termId, int timePeriod)
        {
            DisplayTestResultFilterViewModel model = new DisplayTestResultFilterViewModel()
            {
                DistrictId = districtId,
                VirtualTestId = virtualTestId,
                ClassId = classId,
                StudentName = studentName,
                SchoolId = schoolId,
                TeacherName = teacherName,
                TermrId = termId,
                TimePeriod = timePeriod
            };

            var gradebookSISValue = _parameters.DistrictDecodeServices
                .GetDistrictDecodesOfSpecificDistrictByLabel(CurrentUser.DistrictId.GetValueOrDefault(), Util.DistrictDecode_SendTestResultToGenesis)
                .Select(c => c.Value)
                .FirstOrDefault();

            ViewBag.GradebookSIS = gradebookSISValue?.ToIntArray("|") ?? new int[0];
            return PartialView("_TestResultByFilter", model);
        }

        public ActionResult GetTestResultRetaggedToView(int districtId, int virtualTestId, int classId, string studentName, int schoolId, string teacherName, int termId, bool isShowExported, int timePeriod)
        {
            if (!Util.HasRightOnDistrict(CurrentUser, districtId) && districtId > 0)
            {
                return Json(new { error = "Has no right on the selected district." }, JsonRequestBehavior.AllowGet);
            }

            if (virtualTestId > 0 && !HasRightOnVirtualTestByRole(districtId, false, virtualTestId))
            {
                return Json(new { error = "Has no right on the selected virtual test." }, JsonRequestBehavior.AllowGet);
            }

            if (classId > 0 && !_parameters.VulnerabilityService.CheckUserCanAccessClass(CurrentUser, classId))
            {
                return Json(new { error = "Has no right on the selected class." }, JsonRequestBehavior.AllowGet);
            }

            if (schoolId > 0 && !_parameters.VulnerabilityService.CheckUserCanAccessSchool(CurrentUser, schoolId))
            {
                return Json(new { error = "Has no right on the selected school." }, JsonRequestBehavior.AllowGet);
            }

            var parser = new DataTableParserProc<TestResultExportViewModel>();
            int? totalRecords = 0;
            var sortColumns = parser.SortableColumns;

            var isStudentInformationSystem = false;
            var districtDecode =
                _parameters.DistrictDecodeServices.GetDistrictDecodesOfSpecificDistrictByLabel(districtId,
                    Util.IsStudentInformationSystem).FirstOrDefault();
            if (districtDecode != null)
                bool.TryParse(districtDecode.Value, out isStudentInformationSystem);

            var dateCompare = GetDateTime(timePeriod).ToString();
            var model = new TestResultDataModel();
            model.DistrictId = districtId;
            model.UserId = CurrentUser.Id;
            model.RoleId = CurrentUser.RoleId;
            model.TeacherName = teacherName;
            model.ClassId = classId;
            model.Schoolid = schoolId;
            model.StudentName = studentName;
            model.VirtualTestId = virtualTestId;
            model.TermId = termId;
            model.IsShowExported = isShowExported;
            model.PageIndex = parser.StartIndex;
            model.PageSize = parser.PageSize;
            model.SortColumns = sortColumns;
            model.IsStudentInformationSystem = isStudentInformationSystem;
            model.DateCompare = dateCompare;
            model.GeneralSearch = Request["sSearch"] ?? string.Empty;


            var testResults = _parameters.VirtualTestDistrictServices.GetTestResultRetagged(model, ref totalRecords)
                .Select(x => new TestResultExportViewModel()
                {
                    ID = x.TestResultId,
                    TestNameCustom = x.TestName,
                    SchoolName = x.SchoolName,
                    TeacherCustom = x.TeacherCustom,
                    ClassNameCustom = x.ClassNameCustom,
                    StudentCustom = x.StudentCustom,
                    IsExported = x.IsExported.GetValueOrDefault().ToString(),
                    ResultDate = x.ResultDate
                }).AsQueryable();

            return Json(parser.Parse(testResults, totalRecords ?? 0), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetTermTestResultDistrict(int districtId, int classId, int studentId, int virtualTestId, int schoolId, int teacherId)
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
            if (studentId > 0 && !HasRightOnStudentDistrictByRole(districtId, studentId))
            {
                return Json(new { error = "Has no right on the selected student." }, JsonRequestBehavior.AllowGet);
            }
            if (schoolId > 0 && !HasRightOnSchoolDistrictFilterByRole(districtId, 0, classId, studentId, virtualTestId, schoolId))
            {
                return Json(new { error = "Has no right on the selected school." }, JsonRequestBehavior.AllowGet);
            }
            var result = _parameters.VirtualTestDistrictServices.GetDistrictTermValidFilterByRole(districtId, CurrentUser.Id, CurrentUser.RoleId, virtualTestId, studentId, classId, schoolId, teacherId);
            return Json(result, JsonRequestBehavior.AllowGet);
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

        [HttpGet]
        public ActionResult GetTestResultsByDistrictId(int id)
        {
            if (!Util.HasRightOnDistrict(CurrentUser, id))
            {
                return Json(new { error = "Has no right on the selected district." }, JsonRequestBehavior.AllowGet);
            }
            var vData = _parameters.VirtualTestDistrictServices.GetVirtualTestValideTermByRole(id, CurrentUser.Id, CurrentUser.RoleId);
            return Json(vData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetTestResultsAuthorByDistrictId(int id)
        {
            if (!Util.HasRightOnDistrict(CurrentUser, id))
            {
                return Json(new { error = "Has no right on the selected district." }, JsonRequestBehavior.AllowGet);
            }
            //User store [Teacher is a primanry teacher of class, not author test]
            var data = _parameters.VirtualTestDistrictServices.GetPrimaryTeacherValidTermByDistrictAndRole(id, CurrentUser.Id, CurrentUser.RoleId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetClassByDistrictId(int id)
        {
            if (!Util.HasRightOnDistrict(CurrentUser, id))
            {
                return Json(new { error = "Has no right on the selected district." }, JsonRequestBehavior.AllowGet);
            }
            //Using Store
            var vData = _parameters.VirtualTestDistrictServices.GetClassValidTermByDistrictAndRole(id, CurrentUser.Id, CurrentUser.RoleId);
            return Json(vData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetStudentByDistrictId(int id)
        {
            if (!Util.HasRightOnDistrict(CurrentUser, id))
            {
                return Json(new { error = "Has no right on the selected district." }, JsonRequestBehavior.AllowGet);
            }
            //Using Store
            var vData = _parameters.VirtualTestDistrictServices.GetStudentValidTermByDistrictAndRole(id, CurrentUser.Id, CurrentUser.RoleId);
            return Json(vData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
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
            if (studentId > 0 && !HasRightOnStudentDistrictByRole(districtId, studentId))
            {
                return Json(new { error = "Has no right on the selected student." }, JsonRequestBehavior.AllowGet);
            }
            if (schoolId > 0 && !HasRightOnSchoolDistrictFilterByRole(districtId, teacherId, 0, studentId, virtualTestId, schoolId))
            {
                return Json(new { error = "Has no right on the selected school." }, JsonRequestBehavior.AllowGet);
            }
            var data = _parameters.VirtualTestDistrictServices.GetClassTestResultTermValidByDistrictAndRole(districtId, CurrentUser.Id,
                CurrentUser.RoleId, virtualTestId, studentId, schoolId, teacherId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTeacherTestResultDistrict(int districtId, int virtualTestId, int classId, int studentId, int schoolId)
        {
            if (districtId > 0 && !Util.HasRightOnDistrict(CurrentUser, districtId))
            {
                return Json(new { error = "Has no right on the selected district." }, JsonRequestBehavior.AllowGet);
            }
            if (virtualTestId > 0 && !HasRightOnVirtualTestByRole(districtId, false, virtualTestId))
            {
                return Json(new { error = "Has no right on the selected virtual test." }, JsonRequestBehavior.AllowGet);
            }
            if (classId > 0 && !HasRightOnClassDistrictByRole(districtId, false, classId))
            {
                return Json(new { error = "Has no right on the selected class." }, JsonRequestBehavior.AllowGet);
            }
            if (studentId > 0 && !HasRightOnStudentDistrictByRole(districtId, studentId))
            {
                return Json(new { error = "Has no right on the selected student." }, JsonRequestBehavior.AllowGet);
            }
            if (schoolId > 0 && !HasRightOnSchoolDistrictFilterByRole(districtId, 0, classId, studentId, virtualTestId, schoolId))
            {
                return Json(new { error = "Has no right on the selected school." }, JsonRequestBehavior.AllowGet);
            }
            // Use store
            var data = _parameters.VirtualTestDistrictServices.GetPrimaryTeacherValidTermByDistrictAndRole(districtId,
                CurrentUser.Id, CurrentUser.RoleId, schoolId, classId, studentId, virtualTestId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetStudentTestResultDistrict(int districtId, int virtualTestId, int classId, int schoolId, int teacherId)
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
            if (classId > 0 && !HasRightOnClassDistrictByRole(districtId, false, classId))
            {
                return Json(new { error = "Has no right on the selected class." }, JsonRequestBehavior.AllowGet);
            }
            var data = _parameters.VirtualTestDistrictServices.GetStudentValidTermByDistrictAndRole(districtId, CurrentUser.Id,
                CurrentUser.RoleId, schoolId, teacherId, classId, virtualTestId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetVirtualTestTestResultDistrict(int districtId, int classId, int studentId, int schoolId, int teacherId)
        {
            if (districtId > 0 && !Util.HasRightOnDistrict(CurrentUser, districtId))
            {
                return Json(new { error = "Has no right on the selected district." }, JsonRequestBehavior.AllowGet);
            }
            if (teacherId > 0 && !HasRightOnPrimaryTeacher(districtId, false, teacherId))
            {
                return Json(new { error = "Has no right on the selected teacher." }, JsonRequestBehavior.AllowGet);
            }
            if (classId > 0 && !HasRightOnClassDistrictByRole(districtId, false, classId))
            {
                return Json(new { error = "Has no right on the selected class." }, JsonRequestBehavior.AllowGet);
            }
            if (studentId > 0 && !_parameters.VulnerabilityService.CheckUserPermissionOnStudent(CurrentUser, studentId.ToString()))
            {
                return Json(new { error = "Has no right on the selected student." }, JsonRequestBehavior.AllowGet);
            }
            if (schoolId > 0 && !_parameters.VulnerabilityService.CheckUserCanAccessSchool(CurrentUser, schoolId))
            {
                return Json(new { error = "Has no right on the selected school." }, JsonRequestBehavior.AllowGet);
            }
            //Use store
            var data = _parameters.VirtualTestDistrictServices.GetVirtualTestDistrictFilterByRole(districtId,
                CurrentUser.Id, CurrentUser.RoleId, false, schoolId, teacherId, classId, studentId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetSchoolTestResultDistrict(int districtId, int virtualTestId, int teacherId, int classId, int studentId)
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
            if (classId > 0 && !HasRightOnClassDistrictByRole(districtId, false, classId))
            {
                return Json(new { error = "Has no right on the selected class." }, JsonRequestBehavior.AllowGet);
            }
            if (studentId > 0 && !HasRightOnStudentDistrictByRole(districtId, studentId))
            {
                return Json(new { error = "Has no right on the selected student." }, JsonRequestBehavior.AllowGet);
            }
            //Use Store
            var data = GetSchoolDistrictFilterByRole(districtId, teacherId, classId, studentId, virtualTestId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region private Method

        private bool HasRightOnStudentDistrictByRole(int districtId, int studentId)
        {
            //User store [Teacher is a primanry teacher of class, not author test]
            var data = _parameters.VirtualTestDistrictServices.GetStudentDistrictByRole(districtId, CurrentUser.Id, CurrentUser.RoleId, false);
            return data.Any(x => x.Id == studentId);
        }

        private bool HasRightOnVirtualTestByRole(int districtId, bool isRegrader, int virtualTestId)
        {
            var data = GetVirtualTestByRole(districtId, isRegrader);
            return data.Any(x => x.Id == virtualTestId);
        }

        private IQueryable<ListItem> GetVirtualTestByRole(int districtId, bool isRegrader)
        {
            //Using Store
            var vData = _parameters.VirtualTestDistrictServices.GetVirtualTestByRole(districtId, CurrentUser.Id, CurrentUser.RoleId, isRegrader);
            return vData;

        }

        private bool HasRightOnClassDistrictByRole(int districtId, bool isRegrader, int classId)
        {
            //User store [Teacher is a primanry teacher of class, not author test]
            var data = GetClassDistrictByRole(districtId, isRegrader);
            return data.Any(x => x.Id == classId);
        }

        private IQueryable<ListItem> GetClassDistrictByRole(int districtId, bool isRegrader)
        {
            //User store [Teacher is a primanry teacher of class, not author test]
            var data = _parameters.VirtualTestDistrictServices.GetClassDistrictByRole(districtId, CurrentUser.Id, CurrentUser.RoleId, isRegrader);
            return data;
        }

        private bool HasRightOnPrimaryTeacher(int districtId, bool isRegrader, int teacherId)
        {
            //User store [Teacher is a primanry teacher of class, not author test]
            var data = GetPrimaryTeacherDistrictByRole(districtId, isRegrader);
            return data.Any(x => x.Id == teacherId);
        }

        private IQueryable<ListItem> GetPrimaryTeacherDistrictByRole(int districtId, bool isRegrader)
        {
            //User store [Teacher is a primanry teacher of class, not author test]
            var data = _parameters.VirtualTestDistrictServices.GetPrimaryTeacherDistrictByRole(districtId, CurrentUser.Id,
                CurrentUser.RoleId, isRegrader);
            return data;
        }

        private bool HasRightOnSchoolDistrictFilterByRole(int districtId, int teacherId, int classId, int studentId, int virtualTestId, int schoolId)
        {
            var data = GetSchoolDistrictFilterByRole(districtId, teacherId, classId, studentId, virtualTestId);
            return data.Any(x => x.Id == schoolId);
        }

        private IQueryable<ListItem> GetSchoolDistrictFilterByRole(int districtId, int teacherId, int classId, int studentId, int virtualTestId)
        {
            var data = _parameters.VirtualTestDistrictServices.GetSchoolValidTermByDistrictAndRole(districtId, CurrentUser.Id, CurrentUser.RoleId, teacherId, classId, studentId, virtualTestId);
            return data;
        }
        // Functon Covert to int to DateTime. 
        private DateTime GetDateTime(int days)
        {
            if (days == 0) return DateTime.MinValue.AddYears(1800);
            return DateTime.UtcNow.AddDays(-days).Date;
        }

        #endregion
    }
}
