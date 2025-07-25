using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using S3Library;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize]
    [VersionFilter]
    public class TestExtractController : BaseController
    {
        private readonly TestExtractControllerParameters parameters;

        public TestExtractController(TestExtractControllerParameters parameters)
        {
            this.parameters = parameters;
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.TmgmtExtractTestResult)]
        public ActionResult Index()
        {
            if (CurrentUser.IsPublisher)
            {
                ViewBag.IsPublisher = true;
                ViewBag.DistrictID = -1;
            }
            else
            {
                ViewBag.IsPublisher = false;
                ViewBag.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
                ViewBag.StateId = CurrentUser.StateId;
                ViewBag.DistrictID = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;
            }
            return View();
        }

        private bool CheckSettingHideStudentName(int districtID)
        {
            var setting = parameters.DistrictDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(districtID, Util.DistrictDecode_HideStudentName)
                .FirstOrDefault();
            if (setting != null && !string.IsNullOrEmpty(setting.Value))
            {
                bool hideStudentName;
                bool.TryParse(setting.Value, out hideStudentName);
                return hideStudentName;
            }
            return false;
        }

        public ActionResult GetExtractTestResult(ExtractLocalCustom obj)
        {
            var parser = new DataTableParserProc<ExtractTestResultViewModel>();
            DateTime dtStartDate, dtEndDate;
            //obj.StartDate = DateTime.TryParse(obj.StrStartDate, out dtStartDate) ? dtStartDate : DateTime.UtcNow;
            //obj.EndDate = DateTime.TryParse(obj.StrEndDate, out dtEndDate) ? dtEndDate : DateTime.UtcNow;
            if (obj.StrStartDate.TryParseDateWithFormat(out dtStartDate))
            {
                obj.StartDate = dtStartDate;
            }
            else
            {
                obj.StartDate = DateTime.UtcNow;
            }
            if (obj.StrEndDate.TryParseDateWithFormat(out dtEndDate))
            {
                obj.EndDate = dtEndDate;
            }
            else
            {
                obj.EndDate = DateTime.UtcNow;
            }

            obj.UserId = CurrentUser.Id;
            obj.UserRoleId = CurrentUser.RoleId;

            int? totalRecords = 0;
            var sortColumns = Util.ProcessWildCharacters(parser.SortableColumns);

            bool isHideStudentName = CheckSettingHideStudentName(obj.DistrictId);
            obj.IsHideStudentName = isHideStudentName;
            obj.SSearch = Util.ProcessWildCharacters(obj.SSearch);
            obj.ListTestIDs = obj.ListTestIDs.ToIntCommaSeparatedString();
            var data = parameters.TestResultServices.GetExtractTestResults(obj, parser.StartIndex, parser.PageSize, ref totalRecords, sortColumns)
                .Select(o => new ExtractTestResultViewModel
                {
                    TestResultId = o.TestResultId,
                    TestNameCustom = o.TestNameCustom,
                    SchoolName = o.SchoolName,
                    TeacherCustom = o.TeacherCustom,
                    ClassNameCustom = o.ClassNameCustom,
                    StudentCustom = isHideStudentName ? o.StudentCodeCustom : o.StudentCustom,
                    ResultDate = o.ResultDate
                }).AsQueryable();
            //return new JsonNetResult { Data = parser.Parse(data) };
            return Json(parser.Parse(data, totalRecords ?? 0), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetExtractUser(ExtractUserCustom obj)
        {
            var parser = new DataTableParser<ExtractUserViewModel>();
            DateTime dtStartDate, dtEndDate;
            if (obj.StrStartDate.TryParseDateWithFormat(out dtStartDate))
            {
                obj.StartDate = dtStartDate;
            }
            else
            {
                obj.StartDate = DateTime.UtcNow;
            }
            if (obj.StrEndDate.TryParseDateWithFormat(out dtEndDate))
            {
                obj.EndDate = dtEndDate;
            }
            else
            {
                obj.EndDate = DateTime.UtcNow;
            }

            obj.UserId = CurrentUser.Id;
            obj.UserRoleId = CurrentUser.RoleId;

            var data = parameters.TestResultServices.GetExtractUser(obj).Select(o => new ExtractUserViewModel
            {
                UserId = o.UserId,
                UserName = o.UserName,
                FirstName = o.FirstName,
                LastName = o.LastName,
                SchoolName = o.SchoolName,
                CreatedDate = o.CreatedDate,
                ModifiedDate = o.ModifiedDate,
                SchoolId = o.SchoolId

            }).AsQueryable();
            return new JsonNetResult { Data = parser.Parse(data) };
        }
        public ActionResult GetExtractTest(ExtractLocalCustom obj)
        {
            var parser = new DataTableParser<ExtractTestViewModel>();
            DateTime dtStartDate, dtEndDate;
            //obj.StartDate = DateTime.TryParse(obj.StrStartDate, out dtStartDate) ? dtStartDate : DateTime.UtcNow;
            //obj.EndDate = DateTime.TryParse(obj.StrEndDate, out dtEndDate) ? dtEndDate : DateTime.UtcNow;
            if (obj.StrStartDate.TryParseDateWithFormat(out dtStartDate))
            {
                obj.StartDate = dtStartDate;
            }
            else
            {
                obj.StartDate = DateTime.UtcNow;
            }
            if (obj.StrEndDate.TryParseDateWithFormat(out dtEndDate))
            {
                obj.EndDate = dtEndDate;
            }
            else
            {
                obj.EndDate = DateTime.UtcNow;
            }
            obj.UserId = CurrentUser.Id;
            obj.UserRoleId = CurrentUser.RoleId;
            if (obj.SubjectName == "-1") obj.SubjectName = null;

            var data = parameters.TestResultServices.GetExtractTest(obj).Select(o => new ExtractTestViewModel
            {
                VirtualTestId = o.VirtualTestId,
                BankName = o.BankName,
                TestName = o.TestName,
                Grade = o.Grade,
                Subject = o.Subject

            }).AsQueryable();
            return new JsonNetResult { Data = parser.Parse(data) };
        }

        public ActionResult GetExtractTestAssignment(ExtractLocalCustom obj)
        {
            var parser = new DataTableParser<ExtractTestAssignmentViewModel>();
            DateTime dtStartDate, dtEndDate;
            if (obj.StrStartDate.TryParseDateWithFormat(out dtStartDate))
            {
                obj.StartDate = dtStartDate;
            }
            else
            {
                obj.StartDate = DateTime.UtcNow;
            }
            if (obj.StrEndDate.TryParseDateWithFormat(out dtEndDate))
            {
                obj.EndDate = dtEndDate;
            }
            else
            {
                obj.EndDate = DateTime.UtcNow;
            }

            obj.UserId = CurrentUser.Id;
            obj.UserRoleId = CurrentUser.RoleId;
            if (obj.SubjectName == "-1") obj.SubjectName = null;

            var data = parameters.TestResultServices.GetExtractTestAssignment(obj).Select(o => new ExtractTestAssignmentViewModel
            {
                QTITestClassAssignmentId = o.QTITestClassAssignmentID,
                Assigned = o.AssignmentDate,
                TestName = o.TestName,
                Teacher = o.TeacherName,
                Class = o.ClassName,
                Code = o.Code

            }).AsQueryable();
            return new JsonNetResult { Data = parser.Parse(data) };
        }

        public ActionResult GetExtractRoster(ExtractRosterCustom obj)
        {
            var parser = new DataTableParser<ExtractRosterViewModel>();
            DateTime dtStartDate, dtEndDate;
            //obj.StartDate = DateTime.TryParse(obj.StrStartDate, out dtStartDate) ? dtStartDate : DateTime.UtcNow;
            //obj.EndDate = DateTime.TryParse(obj.StrEndDate, out dtEndDate) ? dtEndDate : DateTime.UtcNow;
            if (obj.StrStartDate.TryParseDateWithFormat(out dtStartDate))
            {
                obj.StartDate = dtStartDate;
            }
            else
            {
                obj.StartDate = DateTime.UtcNow;
            }
            if (obj.StrEndDate.TryParseDateWithFormat(out dtEndDate))
            {
                obj.EndDate = dtEndDate;
            }
            else
            {
                obj.EndDate = DateTime.UtcNow;
            }
            obj.UserId = CurrentUser.Id;
            obj.UserRoleId = CurrentUser.RoleId;

            bool isHideStudentName = CheckSettingHideStudentName(obj.DistrictId);
            var data = parameters.TestResultServices.GetExtractRoster(obj).Select(o => new ExtractRosterViewModel()
            {
                ClassId = o.ClassID,
                ClassName = o.ClassName,
                SchoolName = o.SchoolName,
                DistrictTerm = o.Term,
                UserName = o.Username,
                ClassStudentID = o.ClassStudentID,
                Student = isHideStudentName ? o.StudentCode : o.StudentNameCustom,
            }).AsQueryable();
            return new JsonNetResult { Data = parser.Parse(data) };
        }
        public ActionResult GetListTestResultIds(ExtractLocalCustom obj)
        {
            DateTime dtStartDate, dtEndDate;
            if (obj.StrStartDate.TryParseDateWithFormat(out dtStartDate))
            {
                obj.StartDate = dtStartDate;
            }
            else
            {
                obj.StartDate = DateTime.UtcNow;
            }

            if (obj.StrEndDate.TryParseDateWithFormat(out dtEndDate))
            {
                obj.EndDate = dtEndDate;
            }
            else
            {
                obj.EndDate = DateTime.UtcNow;
            }

            obj.UserId = CurrentUser.Id;
            obj.UserRoleId = CurrentUser.RoleId;
            obj.ListTestIDs = obj.ListTestIDs.ToIntCommaSeparatedString();

            var data = parameters.TestResultServices.GetExtractTestResultStudentIDs(obj);
            string strResult = string.Empty;
            if (data.Count > 0)
            {
                strResult = data.Aggregate(strResult, (current, i) => current + (";_" + i + "_"));
            }
            return Json(new { success = true, data = strResult }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetListUserIds(ExtractUserCustom obj)
        {
            DateTime dtStartDate, dtEndDate;
            //obj.StartDate = DateTime.TryParse(obj.StrStartDate, out dtStartDate) ? dtStartDate : DateTime.UtcNow;
            //obj.EndDate = DateTime.TryParse(obj.StrEndDate, out dtEndDate) ? dtEndDate : DateTime.UtcNow;
            if (obj.StrStartDate.TryParseDateWithFormat(out dtStartDate))
            {
                obj.StartDate = dtStartDate;
            }
            else
            {
                obj.StartDate = DateTime.UtcNow;
            }
            if (obj.StrEndDate.TryParseDateWithFormat(out dtEndDate))
            {
                obj.EndDate = dtEndDate;
            }
            else
            {
                obj.EndDate = DateTime.UtcNow;
            }

            obj.UserId = CurrentUser.Id;
            obj.UserRoleId = CurrentUser.RoleId;

            var data = parameters.TestResultServices.GetExtractUser(obj).Select(o => new ExtractUserViewModel
            {
                UserId = o.UserId,
                UserName = o.UserName,
                FirstName = o.FirstName,
                LastName = o.LastName,
                SchoolName = o.SchoolName,
                CreatedDate = o.CreatedDate,
                ModifiedDate = o.ModifiedDate,
                SchoolId = o.SchoolId

            }).AsQueryable();
            var parser = new DataTableParser<ExtractUserViewModel>();
            var userFilters = parser.ParseNotPaging(data, false, obj.SearchBox, obj.ColumnSearchs.Split(',')?.Select(int.Parse)?.ToList());
            string strResult = string.Empty;
            if (userFilters != null && userFilters.aaData != null && userFilters.aaData.Any())
            {
                var properties = typeof(ExtractUserViewModel).GetProperties().ToList();
                var userIdIndex = properties.FindIndex(x => x.Name == nameof(ExtractUserViewModel.UserId));
                var schoolIdIndex = properties.FindIndex(x => x.Name == nameof(ExtractUserViewModel.SchoolId));
                var userIds = userFilters.aaData
                    .Select(m => $"{m.ElementAt(userIdIndex)}-{m.ElementAt(schoolIdIndex)}")
                    .ToList();
                strResult = userIds.Aggregate(strResult, (current, i) => current + (";_" + i + "_"));
            }
            return Json(new { success = true, data = strResult }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetListTestIds(ExtractLocalCustom obj)
        {
            DateTime dtStartDate, dtEndDate;
            if (obj.StrStartDate.TryParseDateWithFormat(out dtStartDate))
            {
                obj.StartDate = dtStartDate;
            }
            else
            {
                obj.StartDate = DateTime.UtcNow;
            }
            if (obj.StrEndDate.TryParseDateWithFormat(out dtEndDate))
            {
                obj.EndDate = dtEndDate;
            }
            else
            {
                obj.EndDate = DateTime.UtcNow;
            }

            obj.UserId = CurrentUser.Id;
            obj.UserRoleId = CurrentUser.RoleId;
            if (obj.SubjectName == "-1") obj.SubjectName = null;

            var testResults = parameters.TestResultServices.GetExtractTest(obj).Select(o => new ExtractTestViewModel
            {
                VirtualTestId = o.VirtualTestId,
                BankName = o.BankName,
                TestName = o.TestName,
                Grade = o.Grade,
                Subject = o.Subject

            }).AsQueryable();
            var parser = new DataTableParser<ExtractTestViewModel>();
            var testResultFilters = parser.ParseNotPaging(testResults, false, obj.SearchBox, obj.ColumnSearchs.Split(',')?.Select(int.Parse)?.ToList());
            string strResult = string.Empty;
            if (testResultFilters != null && testResultFilters.aaData != null && testResultFilters.aaData.Any())
            {
                var properties = typeof(ExtractTestViewModel).GetProperties().ToList();
                var virtualTestIdIndex = properties.FindIndex(x => x.Name == nameof(ExtractTestViewModel.VirtualTestId));
                var virtualTestIds = testResultFilters.aaData.Select(m => m.ElementAt(virtualTestIdIndex)).Distinct().ToList();
                strResult = virtualTestIds.Aggregate(strResult, (current, i) => current + (";_" + i + "_"));
            }
            return Json(new { success = true, data = strResult }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetListTesAssignmentIds(ExtractLocalCustom obj)
        {
            DateTime dtStartDate, dtEndDate;
            if (obj.StrStartDate.TryParseDateWithFormat(out dtStartDate))
            {
                obj.StartDate = dtStartDate;
            }
            else
            {
                obj.StartDate = DateTime.UtcNow;
            }
            if (obj.StrEndDate.TryParseDateWithFormat(out dtEndDate))
            {
                obj.EndDate = dtEndDate;
            }
            else
            {
                obj.EndDate = DateTime.UtcNow;
            }
            obj.UserId = CurrentUser.Id;
            obj.UserRoleId = CurrentUser.RoleId;
            if (obj.SubjectName == "-1") obj.SubjectName = null;

            var data = parameters.TestResultServices.GetExtractTestAssignment(obj).Select(o => new ExtractTestAssignmentViewModel
            {
                QTITestClassAssignmentId = o.QTITestClassAssignmentID,
                Assigned = o.AssignmentDate,
                TestName = o.TestName,
                Teacher = o.TeacherName,
                Class = o.ClassName,
                Code = o.Code

            }).AsQueryable();
            var parser = new DataTableParser<ExtractTestAssignmentViewModel>();
            var testAssignmentFilters = parser.ParseNotPaging(data, false, obj.SearchBox, obj.ColumnSearchs.Split(',')?.Select(int.Parse)?.ToList());
            string strResult = string.Empty;
            if (testAssignmentFilters != null && testAssignmentFilters.aaData != null && testAssignmentFilters.aaData.Any())
            {
                var properties = typeof(ExtractTestAssignmentViewModel).GetProperties().ToList();
                var qtiTestClassAssignmentIdIndex = properties.FindIndex(x => x.Name == nameof(ExtractTestAssignmentViewModel.QTITestClassAssignmentId));
                var qtiTestClassAssignmentIds = testAssignmentFilters.aaData.Select(m => m.ElementAt(qtiTestClassAssignmentIdIndex)).Distinct().ToList();
                strResult = qtiTestClassAssignmentIds.Aggregate(strResult, (current, i) => current + (";_" + i + "_"));
            }
            return Json(new { success = true, data = strResult }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AddExtractionToQueue(int? district, string lstIds, string lstTemplates, int timezoneOffset, string type, string lstSchoolIds)
        {
            int currentDistrict = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;
            if (district.HasValue && (CurrentUser.IsPublisher || CurrentUser.IsNetworkAdmin)) currentDistrict = district.Value;
            if (string.IsNullOrEmpty(lstIds))
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            string strListResult = lstIds.Replace("_", "");
            var typeExtract = (int)ExtractTypeEnum.TestResults;
            if (!string.IsNullOrEmpty(type))
            {
                if (type == "users")
                    typeExtract = (int)ExtractTypeEnum.Users;
                if (type == "tests")
                    typeExtract = (int)ExtractTypeEnum.Tests;
                if (type == "testAssignments")
                    typeExtract = (int)ExtractTypeEnum.TestAssignments;
            }
            var queueEntity = new ExtractLocalTestResultsQueue()
            {
                DistrictId = currentDistrict,
                UserId = CurrentUser.Id,
                UserTimeZoneOffset = timezoneOffset,
                ExportTemplates = lstTemplates,
                ListIDsInput = strListResult,
                ExtractType = typeExtract,
                ListSchoolIds = lstSchoolIds,
                Status = (int)ExtractLocalTestStatusEnum.NotProcess,
                CreatedDate = DateTime.UtcNow,
                BaseHostURL = string.Format("{0}://{1}", HelperExtensions.GetHTTPProtocal(Request), Request.Url.Authority)
            };
            parameters.ExtractQueueService.Save(queueEntity);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddExtractionToQueueTestResult(ExtractLocalCustom obj, string lstIds, string lstTemplates, int timezoneOffset, bool isCheckAll, string lstIdsUncheck)
        {
            if (string.IsNullOrEmpty(lstIds) && isCheckAll == false)
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }

            int currentDistrict = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;
            if (!CurrentUser.IsPublisher && !CurrentUser.IsNetworkAdmin) obj.DistrictId = currentDistrict;
            obj.UserId = CurrentUser.Id;
            obj.UserRoleId = CurrentUser.RoleId;
            obj.GeneralSearch = Util.ProcessWildCharacters(obj.GeneralSearch);
            var baseHostUrl = string.Format("{0}://{1}", HelperExtensions.GetHTTPProtocal(Request), Request.Url.Authority);
            var param = new ExtractTestResultParamCustom()
            {
                ExtractLocalCustom = obj,
                BaseHostUrl = baseHostUrl,
                IsCheckAll = isCheckAll,
                ListIdsUncheck = lstIdsUncheck,
                ListId = lstIds,
                TimeZoneOffset = timezoneOffset,
                ListTemplates = lstTemplates
            };
            var queueId = parameters.ExtractQueueService.AddExtractionToQueueTestResult(param);
            if (queueId > 0)
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddExtractionToQueueRoster(ExtractRosterCustom obj, string lstIds, string lstTemplates, int timezoneOffset, bool isCheckAll)
        {
            int currentDistrict = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;
            if (CurrentUser.IsPublisher || CurrentUser.IsNetworkAdmin) currentDistrict = obj.DistrictId;
            if (string.IsNullOrEmpty(lstIds) && isCheckAll == false)
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }

            if (isCheckAll)
            {
                DateTime dtStartDate, dtEndDate;
                if (obj.StrStartDate.TryParseDateWithFormat(out dtStartDate))
                {
                    obj.StartDate = dtStartDate;
                }
                else
                {
                    obj.StartDate = DateTime.UtcNow;
                }
                if (obj.StrEndDate.TryParseDateWithFormat(out dtEndDate))
                {
                    obj.EndDate = dtEndDate;
                }
                else
                {
                    obj.EndDate = DateTime.UtcNow;
                }

                obj.UserId = CurrentUser.Id;
                obj.UserRoleId = CurrentUser.RoleId;

                bool isHideStudentName = CheckSettingHideStudentName(obj.DistrictId);
                var extractRosters = parameters.TestResultServices.GetExtractRoster(obj).Select(o => new ExtractRosterViewModel()
                {
                    ClassId = o.ClassID,
                    ClassName = o.ClassName,
                    SchoolName = o.SchoolName,
                    DistrictTerm = o.Term,
                    UserName = o.Username,
                    ClassStudentID = o.ClassStudentID,
                    Student = isHideStudentName ? o.StudentCode : o.StudentNameCustom,
                }).AsQueryable();
                var parser = new DataTableParser<ExtractRosterViewModel>();
                var extractRosterFilters = parser.ParseNotPaging(extractRosters, false, obj.SearchBox, obj.ColumnSearchs.Split(',')?.Select(int.Parse)?.ToList());
                if (extractRosterFilters != null && extractRosterFilters.aaData != null && extractRosterFilters.aaData.Any())
                {
                    var properties = typeof(ExtractRosterViewModel).GetProperties().ToList();
                    var classStudentIdIndex = properties.FindIndex(x => x.Name == nameof(ExtractRosterViewModel.ClassStudentID));
                    var studentIds = extractRosterFilters.aaData.Select(m => m.ElementAt(classStudentIdIndex)).Distinct().ToList();
                    lstIds = studentIds.Aggregate(lstIds, (current, i) => current + (";_" + i + "_"));

                    var classIdIndex = properties.FindIndex(x => x.Name == nameof(ExtractRosterViewModel.ClassId));
                    var classIds = extractRosterFilters.aaData.Select(m => m.ElementAt(classIdIndex)).Distinct().ToList();
                    obj.ListClassIDs = classIds.Aggregate(obj.ListClassIDs, (current, i) => current + (";" + i));
                }
            }

            string strListId = lstIds != null ? lstIds.Replace("_", "") : null;
            var queueEntity = new ExtractLocalTestResultsQueue()
            {
                DistrictId = currentDistrict,
                UserId = CurrentUser.Id,
                UserTimeZoneOffset = timezoneOffset,
                ExportTemplates = lstTemplates,
                ListIDsInput = strListId,
                ListClassIds = obj.ListClassIDs,
                ExtractType = (int)ExtractTypeEnum.Rosters,
                Status = (int)ExtractLocalTestStatusEnum.NotProcess,
                CreatedDate = DateTime.UtcNow,
                BaseHostURL = string.Format("{0}://{1}", HelperExtensions.GetHTTPProtocal(Request), Request.Url.Authority)
            };
            parameters.ExtractQueueService.Save(queueEntity);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        //\ Update Extract Local Test Result
        public ActionResult GetGradeHaveTestResult(ExtractLocalCustom filter)
        {
            DateTime dtStartDate, dtEndDate;
            if (filter.StrStartDate.TryParseDateWithFormat(out dtStartDate))
            {
                filter.StartDate = dtStartDate;
            }
            else
            {
                filter.StartDate = DateTime.UtcNow;
            }
            if (filter.StrEndDate.TryParseDateWithFormat(out dtEndDate))
            {
                filter.EndDate = dtEndDate;
            }
            else
            {
                filter.EndDate = DateTime.UtcNow;
            }

            filter.UserId = CurrentUser.Id;
            filter.UserRoleId = CurrentUser.RoleId;

            var data = parameters.TestExtractTemplateServices.GetGradeHaveTestResult(filter)
                .Select(x => new ListItem { Id = x.Id, Name = x.Name });

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSubjectHaveTestResult(ExtractLocalCustom filter)
        {
            DateTime dtStartDate, dtEndDate;
            if (filter.StrStartDate.TryParseDateWithFormat(out dtStartDate))
            {
                filter.StartDate = dtStartDate;
            }
            else
            {
                filter.StartDate = DateTime.UtcNow;
            }
            if (filter.StrEndDate.TryParseDateWithFormat(out dtEndDate))
            {
                filter.EndDate = dtEndDate;
            }
            else
            {
                filter.EndDate = DateTime.UtcNow;
            }
            filter.UserId = CurrentUser.Id;
            filter.UserRoleId = CurrentUser.RoleId;

            var data = parameters.TestExtractTemplateServices.GetSubjectHaveTestResult(filter).OrderBy(x => x.Name);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBankHaveTestResult(ExtractLocalCustom filter)
        {
            DateTime dtStartDate, dtEndDate;
            if (filter.StrStartDate.TryParseDateWithFormat(out dtStartDate))
            {
                filter.StartDate = dtStartDate;
            }
            else
            {
                filter.StartDate = DateTime.UtcNow;
            }
            if (filter.StrEndDate.TryParseDateWithFormat(out dtEndDate))
            {
                filter.EndDate = dtEndDate;
            }
            else
            {
                filter.EndDate = DateTime.UtcNow;
            }
            filter.UserId = CurrentUser.Id;
            filter.UserRoleId = CurrentUser.RoleId;

            var data = parameters.TestExtractTemplateServices.GetBankHaveTestResult(filter)
                .Select(x => new ListItem { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTestHaveTestResult(ExtractLocalCustom filter)
        {
            DateTime dtStartDate, dtEndDate;
            if (filter.StrStartDate.TryParseDateWithFormat(out dtStartDate))
            {
                filter.StartDate = dtStartDate;
            }
            else
            {
                filter.StartDate = DateTime.UtcNow;
            }
            if (filter.StrEndDate.TryParseDateWithFormat(out dtEndDate))
            {
                filter.EndDate = dtEndDate;
            }
            else
            {
                filter.EndDate = DateTime.UtcNow;
            }
            filter.UserId = CurrentUser.Id;
            filter.UserRoleId = CurrentUser.RoleId;

            var data = parameters.TestExtractTemplateServices.GetTestHaveTestResult(filter)
                .Select(x => new ListItem { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSchoolHaveTestResult(ExtractLocalCustom filter)
        {
            DateTime dtStartDate, dtEndDate;
            if (filter.StrStartDate.TryParseDateWithFormat(out dtStartDate))
            {
                filter.StartDate = dtStartDate;
            }
            else
            {
                filter.StartDate = DateTime.UtcNow;
            }
            if (filter.StrEndDate.TryParseDateWithFormat(out dtEndDate))
            {
                filter.EndDate = dtEndDate;
            }
            else
            {
                filter.EndDate = DateTime.UtcNow;
            }
            filter.UserId = CurrentUser.Id;
            filter.UserRoleId = CurrentUser.RoleId;

            var data = parameters.TestExtractTemplateServices.GetSchoolHaveTestResult(filter)
                .Select(x => new ListItem { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSchoolHaveUser(int districtId)
        {
            //DateTime dtStartDate, dtEndDate;
            //filter.StartDate = DateTime.TryParse(filter.StrStartDate, out dtStartDate) ? dtStartDate : DateTime.UtcNow;
            //filter.EndDate = DateTime.TryParse(filter.StrEndDate, out dtEndDate) ? dtEndDate : DateTime.UtcNow;

            var data = parameters.TestExtractTemplateServices.GetSchoolHaveUser(districtId, CurrentUser.Id, CurrentUser.RoleId)
                .Select(x => new ListItem { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTeacherHaveTestResult(ExtractLocalCustom filter)
        {
            DateTime dtStartDate, dtEndDate;
            if (filter.StrStartDate.TryParseDateWithFormat(out dtStartDate))
            {
                filter.StartDate = dtStartDate;
            }
            else
            {
                filter.StartDate = DateTime.UtcNow;
            }
            if (filter.StrEndDate.TryParseDateWithFormat(out dtEndDate))
            {
                filter.EndDate = dtEndDate;
            }
            else
            {
                filter.EndDate = DateTime.UtcNow;
            }
            filter.UserId = CurrentUser.Id;
            filter.UserRoleId = CurrentUser.RoleId;

            var data = parameters.TestExtractTemplateServices.GetTeacherHaveTestResult(filter)
                .Select(x => new ListItem { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetClassHaveTestResult(ExtractLocalCustom filter)
        {
            DateTime dtStartDate, dtEndDate;
            if (filter.StrStartDate.TryParseDateWithFormat(out dtStartDate))
            {
                filter.StartDate = dtStartDate;
            }
            else
            {
                filter.StartDate = DateTime.UtcNow;
            }
            if (filter.StrEndDate.TryParseDateWithFormat(out dtEndDate))
            {
                filter.EndDate = dtEndDate;
            }
            else
            {
                filter.EndDate = DateTime.UtcNow;
            }
            filter.UserId = CurrentUser.Id;
            filter.UserRoleId = CurrentUser.RoleId;

            var data = parameters.TestExtractTemplateServices.GetClassHaveTestResult(filter)
                .Select(x => new ListItem { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetStudentHaveTestResult(ExtractLocalCustom filter)
        {
            DateTime dtStartDate, dtEndDate;
            if (filter.StrStartDate.TryParseDateWithFormat(out dtStartDate))
            {
                filter.StartDate = dtStartDate;
            }
            else
            {
                filter.StartDate = DateTime.UtcNow;
            }
            if (filter.StrEndDate.TryParseDateWithFormat(out dtEndDate))
            {
                filter.EndDate = dtEndDate;
            }
            else
            {
                filter.EndDate = DateTime.UtcNow;
            }
            filter.UserId = CurrentUser.Id;
            filter.UserRoleId = CurrentUser.RoleId;

            bool isHideStudentName = CheckSettingHideStudentName(filter.DistrictId);

            var data = parameters.TestExtractTemplateServices.GetStudentHaveTestResult(filter)
                .Select(x => new ListItem
                {
                    Id = x.StudentID,
                    Name = isHideStudentName ? x.Code : x.Name + " (" + x.Code + ")"
                }).OrderBy(x => x.Name);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetGradeHaveTest(ExtractLocalCustom filter)
        {
            DateTime dtStartDate, dtEndDate;
            if (filter.StrStartDate.TryParseDateWithFormat(out dtStartDate))
            {
                filter.StartDate = dtStartDate;
            }
            else
            {
                filter.StartDate = DateTime.UtcNow;
            }
            if (filter.StrEndDate.TryParseDateWithFormat(out dtEndDate))
            {
                filter.EndDate = dtEndDate;
            }
            else
            {
                filter.EndDate = DateTime.UtcNow;
            }
            filter.UserId = CurrentUser.Id;
            filter.UserRoleId = CurrentUser.RoleId;

            var data = parameters.TestExtractTemplateServices.GetGradeHaveTest(filter)
                .Select(x => new ListItem { Id = x.Id, Name = x.Name });

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSubjectHaveTest(ExtractLocalCustom filter)
        {
            DateTime dtStartDate, dtEndDate;
            if (filter.StrStartDate.TryParseDateWithFormat(out dtStartDate))
            {
                filter.StartDate = dtStartDate;
            }
            else
            {
                filter.StartDate = DateTime.UtcNow;
            }
            if (filter.StrEndDate.TryParseDateWithFormat(out dtEndDate))
            {
                filter.EndDate = dtEndDate;
            }
            else
            {
                filter.EndDate = DateTime.UtcNow;
            }
            filter.UserId = CurrentUser.Id;
            filter.UserRoleId = CurrentUser.RoleId;

            var data = parameters.TestExtractTemplateServices.GetSubjectHaveTest(filter).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBankHaveTest(ExtractLocalCustom filter)
        {
            DateTime dtStartDate, dtEndDate;
            if (filter.StrStartDate.TryParseDateWithFormat(out dtStartDate))
            {
                filter.StartDate = dtStartDate;
            }
            else
            {
                filter.StartDate = DateTime.UtcNow;
            }
            if (filter.StrEndDate.TryParseDateWithFormat(out dtEndDate))
            {
                filter.EndDate = dtEndDate;
            }
            else
            {
                filter.EndDate = DateTime.UtcNow;
            }
            filter.UserId = CurrentUser.Id;
            filter.UserRoleId = CurrentUser.RoleId;

            var data = parameters.TestExtractTemplateServices.GetBankHaveTest(filter)
                .Select(x => new ListItem { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetVirtualTests(ExtractLocalCustom filter)
        {
            DateTime dtStartDate, dtEndDate;
            if (filter.StrStartDate.TryParseDateWithFormat(out dtStartDate))
            {
                filter.StartDate = dtStartDate;
            }
            else
            {
                filter.StartDate = DateTime.UtcNow;
            }
            if (filter.StrEndDate.TryParseDateWithFormat(out dtEndDate))
            {
                filter.EndDate = dtEndDate;
            }
            else
            {
                filter.EndDate = DateTime.UtcNow;
            }

            var data = parameters.TestExtractTemplateServices.GetVirtualTests(filter)
                .Select(x => new ListItem { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAllTemplate()
        {
            var data = parameters.TestExtractTemplateServices.GetAllTemplates()
                .Select(x => new ListItem { Id = x.ID, Name = x.Name });

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDefaultSelect(int? districtId)
        {
            var arr = GetDefaultTemplate(districtId);
            var data = new List<ListItem>();
            var templates = parameters.TestExtractTemplateServices.GetAllTemplates().ToList();
            foreach (var idstr in arr)
            {
                int id;
                var result = int.TryParse(idstr, out id);
                if (result)
                {
                    var template = templates.FirstOrDefault(x => x.ID == id);
                    if (template != null)
                        data.Add(new ListItem() { Id = template.ID, Name = template.Name });
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUserTemplates(int? districtId)
        {
            var arr = GetDefaultTemplate(districtId);
            var data = parameters.TestExtractTemplateServices.GetAllTemplates().Where(x => (x.Name == "USER") && arr.Contains(x.ID.ToString()));
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetRosterTemplates(int? districtId)
        {
            var arr = GetDefaultTemplate(districtId);
            var data = parameters.TestExtractTemplateServices.GetAllTemplates().Where(x => (x.Name == "ROSTER") && arr.Contains(x.ID.ToString()));
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTestTemplate(int? districtId)
        {
            var arr = GetDefaultTemplate(districtId);
            var data = new List<ListItem>();
            var templates = parameters.TestExtractTemplateServices.GetAllTemplates().ToList();
            foreach (var idstr in arr)
            {
                int id;
                var result = int.TryParse(idstr, out id);
                if (result)
                {
                    var template = templates.FirstOrDefault(x => x.ID == id && (x.Name == "TEST" || x.Name == "QUESTION"));
                    if (template != null)
                        data.Add(new ListItem() { Id = template.ID, Name = template.Name });
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetTestAssignmentTemplate(int? districtId)
        {
            var arr = GetDefaultTemplate(districtId);
            var data = parameters.TestExtractTemplateServices.GetAllTemplates().Where(x => x.Name == "CLASS TEST ASSIGNMENT" && arr.Contains(x.ID.ToString()));
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        private string[] GetDefaultTemplate(int? districtId)
        {
            string[] arr = { };
            if (districtId.HasValue)
            {
                var v = parameters.DistrictDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(districtId.Value, Util.DistrictDecode_DefaultTemplates).FirstOrDefault();
                if (v != null)
                {
                    arr = v.Value.Split(',');
                }
            }
            return arr;
        }
        public ActionResult LoadTestResultFilter(bool isPublisher, int districtId)
        {
            ViewBag.IsPublisher = isPublisher;
            ViewBag.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
            ViewBag.DistrictID = districtId;
            ViewBag.StateId = CurrentUser.StateId;
            return PartialView("_FilterExtractTestResult");
        }
        public ActionResult LoadUserFilter(bool isPublisher, int districtId)
        {
            ViewBag.IsPublisher = isPublisher;
            ViewBag.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
            ViewBag.DistrictID = districtId;
            ViewBag.StateId = CurrentUser.StateId;
            return PartialView("_FilterExtractUser");
        }
        public ActionResult LoadRosterFilter(bool isPublisher, int districtId)
        {
            ViewBag.IsPublisher = isPublisher;
            ViewBag.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
            ViewBag.DistrictID = districtId;
            ViewBag.StateId = CurrentUser.StateId;
            return PartialView("_FilterExtractRoster");
        }
        public ActionResult LoadTestFilter(bool isPublisher, int districtId)
        {
            ViewBag.IsPublisher = isPublisher;
            ViewBag.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
            ViewBag.DistrictID = districtId;
            ViewBag.StateId = CurrentUser.StateId;
            return PartialView("_FilterExtractTest");
        }
        public ActionResult LoadTestAssignmentFilter(bool isPublisher, int districtId)
        {
            ViewBag.IsPublisher = isPublisher;
            ViewBag.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
            ViewBag.DistrictID = districtId;
            ViewBag.StateId = CurrentUser.StateId;
            return PartialView("_FilterExtractTestAssignment");
        }
        public ActionResult GetGradeHaveTestAssignment(ExtractLocalCustom filter)
        {
            DateTime dtStartDate, dtEndDate;
            //filter.StartDate = DateTime.TryParse(filter.StrStartDate, out dtStartDate) ? dtStartDate : DateTime.UtcNow;
            //filter.EndDate = DateTime.TryParse(filter.StrEndDate, out dtEndDate) ? dtEndDate : DateTime.UtcNow;
            if (filter.StrStartDate.TryParseDateWithFormat(out dtStartDate))
            {
                filter.StartDate = dtStartDate;
            }
            else
            {
                filter.StartDate = DateTime.UtcNow;
            }
            if (filter.StrEndDate.TryParseDateWithFormat(out dtEndDate))
            {
                filter.EndDate = dtEndDate;
            }
            else
            {
                filter.EndDate = DateTime.UtcNow;
            }
            filter.UserId = CurrentUser.Id;
            filter.UserRoleId = CurrentUser.RoleId;

            var data = parameters.TestExtractTemplateServices.GetGradeHaveTestAssignment(filter)
                .Select(x => new ListItem { Id = x.Id, Name = x.Name });

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSubjectHaveTestAssignment(ExtractLocalCustom filter)
        {
            DateTime dtStartDate, dtEndDate;
            if (filter.StrStartDate.TryParseDateWithFormat(out dtStartDate))
            {
                filter.StartDate = dtStartDate;
            }
            else
            {
                filter.StartDate = DateTime.UtcNow;
            }
            if (filter.StrEndDate.TryParseDateWithFormat(out dtEndDate))
            {
                filter.EndDate = dtEndDate;
            }
            else
            {
                filter.EndDate = DateTime.UtcNow;
            }
            filter.UserId = CurrentUser.Id;
            filter.UserRoleId = CurrentUser.RoleId;

            var data = parameters.TestExtractTemplateServices.GetSubjectHaveTestAssignment(filter).OrderBy(x => x.Name);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBankHaveTestAssignment(ExtractLocalCustom filter)
        {
            DateTime dtStartDate, dtEndDate;
            if (filter.StrStartDate.TryParseDateWithFormat(out dtStartDate))
            {
                filter.StartDate = dtStartDate;
            }
            else
            {
                filter.StartDate = DateTime.UtcNow;
            }
            if (filter.StrEndDate.TryParseDateWithFormat(out dtEndDate))
            {
                filter.EndDate = dtEndDate;
            }
            else
            {
                filter.EndDate = DateTime.UtcNow;
            }
            filter.UserId = CurrentUser.Id;
            filter.UserRoleId = CurrentUser.RoleId;

            var data = parameters.TestExtractTemplateServices.GetBankHaveTestAssignment(filter)
                .Select(x => new ListItem { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTestHaveTestAssignment(ExtractLocalCustom filter)
        {
            DateTime dtStartDate, dtEndDate;
            if (filter.StrStartDate.TryParseDateWithFormat(out dtStartDate))
            {
                filter.StartDate = dtStartDate;
            }
            else
            {
                filter.StartDate = DateTime.UtcNow;
            }
            if (filter.StrEndDate.TryParseDateWithFormat(out dtEndDate))
            {
                filter.EndDate = dtEndDate;
            }
            else
            {
                filter.EndDate = DateTime.UtcNow;
            }
            filter.UserId = CurrentUser.Id;
            filter.UserRoleId = CurrentUser.RoleId;

            var data = parameters.TestExtractTemplateServices.GetVirtualTestsHaveTestAssignment(filter)
                .Select(x => new ListItem { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSchoolHaveTestAssignment(ExtractLocalCustom filter)
        {
            DateTime dtStartDate, dtEndDate;
            if (filter.StrStartDate.TryParseDateWithFormat(out dtStartDate))
            {
                filter.StartDate = dtStartDate;
            }
            else
            {
                filter.StartDate = DateTime.UtcNow;
            }
            if (filter.StrEndDate.TryParseDateWithFormat(out dtEndDate))
            {
                filter.EndDate = dtEndDate;
            }
            else
            {
                filter.EndDate = DateTime.UtcNow;
            }
            filter.UserId = CurrentUser.Id;
            filter.UserRoleId = CurrentUser.RoleId;

            var data = parameters.TestExtractTemplateServices.GetSchoolHaveTestAssignment(filter)
                .Select(x => new ListItem { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTeacherHaveTestAssignment(ExtractLocalCustom filter)
        {
            DateTime dtStartDate, dtEndDate;
            if (filter.StrStartDate.TryParseDateWithFormat(out dtStartDate))
            {
                filter.StartDate = dtStartDate;
            }
            else
            {
                filter.StartDate = DateTime.UtcNow;
            }
            if (filter.StrEndDate.TryParseDateWithFormat(out dtEndDate))
            {
                filter.EndDate = dtEndDate;
            }
            else
            {
                filter.EndDate = DateTime.UtcNow;
            }
            filter.UserId = CurrentUser.Id;
            filter.UserRoleId = CurrentUser.RoleId;

            var data = parameters.TestExtractTemplateServices.GetTeacherHaveTestAssignment(filter)
                .Select(x => new ListItem { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetClassHaveTestAssignment(ExtractLocalCustom filter)
        {
            DateTime dtStartDate, dtEndDate;
            if (filter.StrStartDate.TryParseDateWithFormat(out dtStartDate))
            {
                filter.StartDate = dtStartDate;
            }
            else
            {
                filter.StartDate = DateTime.UtcNow;
            }
            if (filter.StrEndDate.TryParseDateWithFormat(out dtEndDate))
            {
                filter.EndDate = dtEndDate;
            }
            else
            {
                filter.EndDate = DateTime.UtcNow;
            }
            filter.UserId = CurrentUser.Id;
            filter.UserRoleId = CurrentUser.RoleId;

            var data = parameters.TestExtractTemplateServices.GetClassHaveTestAssignment(filter)
                .Select(x => new ListItem { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetTermHaveStudent(int districtId)
        {
            var data = parameters.TestExtractTemplateServices.GetTermHaveStudent(districtId, CurrentUser.Id, CurrentUser.RoleId)
                .Select(x => new ListItem { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSchoolHaveStudent(ExtractRosterCustom filter)
        {
            DateTime dtStartDate, dtEndDate;
            if (filter.StrStartDate.TryParseDateWithFormat(out dtStartDate))
            {
                filter.StartDate = dtStartDate;
            }
            else
            {
                filter.StartDate = DateTime.UtcNow;
            }
            if (filter.StrEndDate.TryParseDateWithFormat(out dtEndDate))
            {
                filter.EndDate = dtEndDate;
            }
            else
            {
                filter.EndDate = DateTime.UtcNow;
            }
            filter.UserId = CurrentUser.Id;
            filter.UserRoleId = CurrentUser.RoleId;

            var data = parameters.TestExtractTemplateServices.GetSchoolHaveStudent(filter)
                .Select(x => new ListItem { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTeacherHaveStudent(ExtractRosterCustom filter)
        {
            DateTime dtStartDate, dtEndDate;
            if (filter.StrStartDate.TryParseDateWithFormat(out dtStartDate))
            {
                filter.StartDate = dtStartDate;
            }
            else
            {
                filter.StartDate = DateTime.UtcNow;
            }
            if (filter.StrEndDate.TryParseDateWithFormat(out dtEndDate))
            {
                filter.EndDate = dtEndDate;
            }
            else
            {
                filter.EndDate = DateTime.UtcNow;
            }
            filter.UserId = CurrentUser.Id;
            filter.UserRoleId = CurrentUser.RoleId;

            var data = parameters.TestExtractTemplateServices.GetTeacherHaveStudent(filter)
                .Select(x => new ListItem { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetClassHaveStudent(ExtractRosterCustom filter)
        {
            DateTime dtStartDate, dtEndDate;
            if (filter.StrStartDate.TryParseDateWithFormat(out dtStartDate))
            {
                filter.StartDate = dtStartDate;
            }
            else
            {
                filter.StartDate = DateTime.UtcNow;
            }
            if (filter.StrEndDate.TryParseDateWithFormat(out dtEndDate))
            {
                filter.EndDate = dtEndDate;
            }
            else
            {
                filter.EndDate = DateTime.UtcNow;
            }
            filter.UserId = CurrentUser.Id;
            filter.UserRoleId = CurrentUser.RoleId;

            var data = parameters.TestExtractTemplateServices.GetClassHaveStudent(filter)
                .Select(x => new ListItem { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
