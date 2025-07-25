using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.SessionState;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Common;
using System.IO;
using System.Configuration;
using S3Library;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using LinkIt.BubbleSheetPortal.Web.Models.DataTable;
using LinkIt.BubbleSheetPortal.Web.Models.GetTestClassAssignment;
using Newtonsoft.Json;
using LinkIt.BubbleSheetPortal.Models.DTOs.StudentLookup;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Models.RestrictionDTO;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.Required)]
    [AjaxAwareAuthorize]
    [VersionFilter]
    public class TestAssignmentReviewController : BaseController
    {
        private readonly TestAssignmentControllerParameters _parameters;
        private readonly IS3Service _service;

        public TestAssignmentReviewController(TestAssignmentControllerParameters parameters, IS3Service service)
        {
            _parameters = parameters;
            _service = service;
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.OnlineTestAssignmentReview)]

        public ActionResult Index(string assignmentCodes, int? version)
        {
            ViewBag.Version = version;
            if (CurrentUser.IsPublisher())
                ViewBag.IsPublisher = true;
            ViewBag.IsNetworkAdmin = false;
            if (CurrentUser.IsNetworkAdmin)
            {
                ViewBag.IsNetworkAdmin = true;
                ViewBag.ListDictrictIds = ConvertListIdToStringId(CurrentUser.GetMemberListDistrictId());
            }

            ViewBag.AssignmentCodes = assignmentCodes;
            ViewBag.IsSingleAssignmentCodes = JsonConvert.SerializeObject(Util.SplitString(assignmentCodes, ';').Length == 1);
            if (Util.SplitString(assignmentCodes, ';').Length == 1)
            {
                var request = new GetTestClassAssignmentsRequest();
                request.UserID = CurrentUser.Id;
                request.DistrictID = CurrentUser.DistrictId;
                request.AssignmentCodes = assignmentCodes + ";";
                request.StartRow = 0;
                request.PageSize = 10;
                request.SortDirection = "DESC";
                request.SortColumn = "AssignmentDate";
                var assignmentDateCompare = CurrentUser.IsTeacher ? GetDateTime(180) : GetDateTime(30);
                request.AssignDate = assignmentDateCompare.ToString();
                var data = _parameters.QTITestClassAssignmentServices.GetTestClassAssignmentsRemoveTempTable(request);
                if (data.Data != null && data.Data.Count == 1)
                {
                    var teacherReviewerQtiTestClassAssignmentID = data.Data[0].QTITestClassAssignmentID;
                    var teacherReviewerVirtualTestID = data.Data[0].VirtualTestID;
                    var teacherReviewerSelectFirstStudent = "0";
                    var teacherReviewerClassName = data.Data[0].ClassName;
                    var teacherReviewerTeacherName = data.Data[0].TeacherName;
                    var url = Url.Action("Index", "TeacherReview");
                    url += "?qtiTestClassAssignmentID=" + teacherReviewerQtiTestClassAssignmentID;
                    url += "&virtualTestID=" + teacherReviewerVirtualTestID;
                    url += "&selectFirstStudentForReview=" + teacherReviewerSelectFirstStudent;
                    url += "&ClassName=" + teacherReviewerClassName;
                    url += "&TeacherName=" + teacherReviewerTeacherName;
                    url += "&IsPassThrough=true";
                    return RedirectToAction("Index", "TeacherReview", new
                    {
                        qtiTestClassAssignmentID = teacherReviewerQtiTestClassAssignmentID,
                        virtualTestID = teacherReviewerVirtualTestID,
                        selectFirstStudentForReview = teacherReviewerSelectFirstStudent,
                        ClassName = teacherReviewerSelectFirstStudent,
                        TeacherName = teacherReviewerTeacherName,
                        IsPassThrough = true
                    });
                }
            }
            ViewBag.TestTakerUrl = Util.GetConfigByKey("PortalhyperlinkTestCode", "");

            if (CurrentUser.IsTeacher)
            {
                ViewBag.IsTeacher = true;
            }

            ViewBag.IsShowUserCurrentDictrictStudentPortal = _parameters.XLIMenuPermissionService.IsAccessModuleByDistrict(CurrentUser.DistrictId.GetValueOrDefault(), ContaintUtil.OnlineTesting);
            ViewBag.DateFormat = _parameters.ConfigurationServices.GetConfigurationByKey(Constanst.JQueryDateFormat)?.Value;
            //Moduleid=104:student portal
            //ViewBag.IsShowUserCurrentDictrictStudentPortal = true;
            //ViewBag.IsShowUserCurrentDictrictStudentPortal = JsonConvert.SerializeObject( _parameters.XLIAreaDistrictModuleService.CheckDistrictRoleAccessModule(CurrentUser.Id, 104));
            var menuAccess = HelperExtensions.GetMenuForDistrict(CurrentUser);
            ViewBag.IsShowUserCurrentDictrictStudentPortal = menuAccess.IsDisplayOnlineTesting;
            return View();
        }

        [HttpGet]
        public ActionResult GetIsShowIsHideFunctionConfig(int? districtId)
        {
            var isShow = false;
            var districtDecode = _parameters.DistrictDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(districtId ?? 0,
                     "IsShow_IsHideFunction").FirstOrDefault();
            if (districtDecode != null)
            {
                bool value;
                bool.TryParse(districtDecode.Value, out value);
                isShow = value;
            }
            return Json(isShow, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult GetTestClassAssignments(TestAssignmentCriteria criteria)
        {
            var data = new List<QTITestClassAssignment>();
            var parser = new DataTableParser<QTITestClassAssignment>();

            var assignmentDateCompare = GetDateTime(criteria.DateTime);
            criteria.ProgramID = criteria.ProgramID == -1 ? null : criteria.ProgramID;

            int? districtID = null;
            if ((CurrentUser.IsPublisher || CurrentUser.IsNetworkAdmin))
            {
                districtID = criteria.DistrictID;
            }
            else
            {
                districtID = CurrentUser.DistrictId;
            }

            var schoolID = criteria.SchoolID.HasValue && criteria.SchoolID.Value > 0 ? criteria.SchoolID.Value : 0;

            if (!string.IsNullOrEmpty(criteria.AssignmentCodes))
            {
                data =
                _parameters.QTITestClassAssignmentServices.GetTestClassAssignmentsPassThrough(
                    criteria.AssignmentCodes, criteria.OnlyShowPendingReview,
                    criteria.ShowActiveClassTestAssignment, CurrentUser.Id, districtID ?? 0).ToList();
            }
            else if ((!CurrentUser.IsPublisher && !CurrentUser.IsNetworkAdmin && !CurrentUser.IsDistrictAdmin)
               || !criteria.PageLoad.HasValue
               || !criteria.PageLoad.Value)
            {
                data =
                _parameters.QTITestClassAssignmentServices.GetTestClassAssignments_V1(
                    assignmentDateCompare.ToString(), criteria.OnlyShowPendingReview,
                    criteria.ShowActiveClassTestAssignment, CurrentUser.Id, districtID ?? 0, null, schoolID).ToList();
            }

            // apply restriction 
            if (!string.IsNullOrEmpty(criteria.ModuleCode))
            {
                var bankIds = data.Select(m => m.BankId).Distinct();
                var allowTestIds = new List<int>();

                foreach (var item in bankIds)
                {
                    var testIds = data.Where(m => m.BankId == item)
                                        .Select(m => new ListItem { Id = m.VirtualTestID, Name = m.TestName }).ToList();
                    var allowTests = _parameters.RestrictionBO.FilterTests(new Models.RestrictionDTO.FilterTestQueryDTO
                    {
                        ModuleCode = criteria.ModuleCode,
                        DistrictId = districtID.HasValue ? districtID.Value : 0,
                        UserId = CurrentUser.Id,
                        RoleId = CurrentUser.RoleId,
                        Tests = testIds,
                        BankId = item
                    }).Select(m => m.Id).Distinct();

                    allowTestIds.AddRange(allowTests);
                }

                data.ForEach(m =>
                {
                    if (!allowTestIds.Contains(m.VirtualTestID))
                    {
                        m.IsAllowReview = false;
                    }
                });
            }

            return Json(parser.Parse(data.AsQueryable(), true), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetTestClassAssignmentsImproved(GetTestClassAssignmentCriteria criteria)
        {
            var result = new GenericDataTableResponse<GetTestClassAssignmentRow>();
            result.sEcho = criteria.sEcho;
            result.sColumns = criteria.sColumns;

            if (criteria.PageLoad
                && string.IsNullOrWhiteSpace(criteria.AssignmentCodes)
                && (CurrentUser.IsPublisher || CurrentUser.IsNetworkAdmin || CurrentUser.IsDistrictAdmin))
            {
                result.iTotalDisplayRecords = 0;
                result.iTotalRecords = 0;
                result.aaData = new List<GetTestClassAssignmentRow>();
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            var request = new GetTestClassAssignmentsRequest();
            Mapping(criteria, request);

            var data = _parameters.QTITestClassAssignmentServices.GetTestClassAssignmentsRemoveTempTable(request);

            // apply restriction 
            if (!string.IsNullOrEmpty(criteria.ModuleCode))
            {
                var bankIds = data.Data.Select(m => m.BankId).Distinct();
                var allowTestIds = new List<int>();
                var districtId = criteria.DistrictID.HasValue ? criteria.DistrictID.Value
                    : (CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0);

                var restrictionList = _parameters.RestrictionBO.GetRestrictionList(criteria.ModuleCode, CurrentUser.Id, CurrentUser.RoleId, PublishLevelTypeEnum.District, districtId);

                foreach (var item in bankIds)
                {
                    var testIds = data.Data.Where(m => m.BankId == item)
                                        .Select(m => new ListItem { Id = m.VirtualTestID, Name = m.TestName }).ToList();
                    var allowTests = _parameters.RestrictionBO.FilterTests(new Models.RestrictionDTO.FilterTestQueryDTO
                    {
                        ModuleCode = criteria.ModuleCode,
                        DistrictId = districtId,
                        UserId = CurrentUser.Id,
                        RoleId = CurrentUser.RoleId,
                        Tests = testIds,
                        BankId = item
                    }, restrictionList).Select(m => m.Id).Distinct();

                    allowTestIds.AddRange(allowTests);
                }
                var timeZoneId = _parameters.StateService.GetTimeZoneId(CurrentUser.StateId.GetValueOrDefault());
                var dateTimeFormat = _parameters.DistrictDecodeService.GetDateFormat(CurrentUser.DistrictId.GetValueOrDefault());
                data.Data.ForEach(m =>
                {
                    if (!allowTestIds.Contains(m.VirtualTestID))
                    {
                        m.IsAllowReview = false;
                    }
                    if (m.AssignmentDate.HasValue)
                        m.Assigned = m.AssignmentDate.Value.ConvertTimeFromUtc(timeZoneId).DisplayDateWithFormat(dateTimeFormat.DateFormat, dateTimeFormat.TimeFormat, false);
                });
            }

            result.iTotalDisplayRecords = data.TotalRecord;
            result.iTotalRecords = data.TotalRecord;
            result.aaData = data.Data.Select(o => Transform(o)).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private void Mapping(GetTestClassAssignmentCriteria criteria, GetTestClassAssignmentsRequest request)
        {
            if (criteria == null || request == null) return;

            request.PageSize = criteria.iDisplayLength;
            request.StartRow = criteria.iDisplayStart;
            request.GeneralSearch = criteria.sSearch;
            if (!string.IsNullOrWhiteSpace(criteria.sColumns) && criteria.iSortCol_0.HasValue)
            {
                var columns = criteria.sColumns.Split(',');
                request.SortColumn = columns[criteria.iSortCol_0.Value];
                request.SortDirection = string.Compare(criteria.sSortDir_0, "desc") == 0 ? "DESC" : "ASC";
            }

            var assignmentDateCompare = GetDateTime(criteria.DateTime);
            request.AssignDate = assignmentDateCompare.ToString();

            request.DistrictID = CurrentUser.IsPublisher || CurrentUser.IsNetworkAdmin ? criteria.DistrictID : CurrentUser.DistrictId;

            request.SchoolID = criteria.SchoolID.HasValue && criteria.SchoolID.Value > 0 ? criteria.SchoolID.Value : 0;
            request.OnlyShowPedingReview = criteria.OnlyShowPendingReview;
            request.ShowActiveClassTestAssignment = criteria.ShowActiveClassTestAssignment;
            request.UserID = CurrentUser.Id;
            request.AssignmentCodes = string.Format("{0};{1}", criteria.AssignmentCodes ?? string.Empty, criteria.Code ?? string.Empty);
            if (request.AssignmentCodes.Length == 1) request.AssignmentCodes = string.Empty;

            request.GradeName = criteria.GradeName;
            request.SubjectName = criteria.SubjectName;
            request.BankName = criteria.BankName;
            request.ClassName = criteria.ClassName;
            request.TeacherName = criteria.TeacherName;
            request.StudentName = criteria.StudentName;
            request.TestName = criteria.TestName;
        }

        private GetTestClassAssignmentRow Transform(QTITestClassAssignment x)
        {
            if (x == null) return null;

            var result = new GetTestClassAssignmentRow
            {
                AssignmentDate = x.AssignmentDate,
                ClassID = x.ClassID,
                ClassName = x.ClassName,
                TeacherName = x.TeacherName,
                Code = x.Code,
                CodeTime = x.CodeTime,
                QTITestClassAssignmentID = x.QTITestClassAssignmentID,
                Started = x.Started,
                NotStarted = x.NotStarted,
                WaitingForReview = x.WaitingForReview,
                Completed = x.Completed,
                TestName = x.TestName,
                VirtualTestID = x.VirtualTestID,
                DistrictID = x.DistrictID,
                BankName = x.BankName,
                GradeName = x.GradeName,
                SubjectName = x.SubjectName,
                Status = x.Status,
                StudentNames = x.StudentNames,
                AssignmentType = x.AssignmentType,
                BankIsLocked = x.BankIsLocked,
                IsTeacherLed = x.IsTeacherLed,
                IsHide = x.IsHide,
                AssignmentModifiedUserID = x.AssignmentModifiedUserID,
                AssignmentFirstName = x.AssignmentFirstName,
                AssignmentLastName = x.AssignmentLastName,
                IsAllowReview = x.IsAllowReview,
                IsVirtualTestRetake = x.IsVirtualTestRetake,
                IsVirtualTestPartialRetake = x.IsVirtualTestPartialRetake,
                AuthenticationCode = x.AuthenticationCode,
                IsAuthenticationCodeExpired = x.IsAuthenticationCodeExpired,
                Assigned = x.Assigned
            };

            return result;
        }

        [HttpGet]
        public ActionResult GetTestStudentAssignments(TestAssignmentCriteria criteria)
        {
            var parser = new DataTableParser<QTITestStudentAssignment>();

            criteria.ProgramID = criteria.ProgramID == -1 ? null : criteria.ProgramID;

            int? districtID = null;
            if (CurrentUser.IsPublisher && criteria.DistrictID.HasValue)
            {
                districtID = criteria.DistrictID;
            }
            else if (!CurrentUser.IsPublisher && CurrentUser.DistrictId.HasValue)
            {
                districtID = CurrentUser.DistrictId;
            }
            else if (!CurrentUser.IsPublisher)
            {
                return Json(parser.Parse(new List<QTITestStudentAssignment>().AsQueryable()),
                            JsonRequestBehavior.AllowGet);
            }

            if (criteria.QTITestClassAssignmentID.HasValue)
            {
                criteria.OnlyShowPendingReview = false;
            }

            var data =
            _parameters.QTITestClassAssignmentServices.GetTestStudentAssignments(criteria.QTITestClassAssignmentID, districtID);


            return Json(parser.Parse(data), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ExportTestAssignments(ExportTestAssignmentCriteria criteria)
        {
            if(string.IsNullOrEmpty(criteria.QTITestClassAssignmentIDs))
            { return Json(false, JsonRequestBehavior.AllowGet); }    
            var stringIds = criteria.QTITestClassAssignmentIDs.ToIntArray().ToList();
            criteria.AssignmentCodes = _parameters.QTITestClassAssignmentServices.GetTestCodeByTestClassIds(stringIds);
            var assignmentDateCompare = GetDateTime(criteria.DateTime);
            criteria.ProgramID = criteria.ProgramID == -1 ? null : criteria.ProgramID;

            int? districtID = null;
            if ((CurrentUser.IsPublisher || CurrentUser.IsNetworkAdmin))
            {
                districtID = criteria.DistrictID;
            }
            else
            {
                districtID = CurrentUser.DistrictId;
            }

            var data =
            _parameters.QTITestClassAssignmentServices.GetTestClassAssignmentsExport(criteria, assignmentDateCompare.ToString(), CurrentUser.Id, districtID ?? 0);
            var sb = new StringBuilder();
            //sb.AppendFormat("Assigned" + "\t" + "Assigned By" + "\t" + "Test" + "\t" + "School" + "\t" + "Teacher" + "\t" + "Class" + "\t" + "NS" + "\t" + "IP" + "\t" + "PR" + "\t" + "Fini" + "\t" + "Code" + "\t" + "Student {0}", System.Environment.NewLine);
            sb.AppendFormat(
                "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\" {12}",
                "Assigned Date", "Assigned By", "Test", "School", "Teacher", "Class", "NS", "IP", "PR", "Fini", "Code",
                "Student", System.Environment.NewLine);
            //Format DefaultDateFormat for Export
            var dateFormatModel = _parameters.DistrictDecodeServices.GetDateFormat(districtID.HasValue ? districtID.Value : 0);
            foreach (var item in data)
            {
                //sb.AppendFormat("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11} {12}",
                //     item.AssignmentDate, string.Format("{0},{1}",item.AssignmentLastName, item.AssignmentFirstName),item.TestName, item.SchoolName,
                //     item.TeacherName, item.ClassName, item.NotStarted, item.Started, item.WaitingForReview, item.Completed, item.Code, item.StudentNames,
                //     System.Environment.NewLine);
                var assignmentDate = string.Empty;
                var assignedBy = string.Empty;
                if (string.IsNullOrEmpty(item.AssignmentFirstName) || string.IsNullOrEmpty(item.AssignmentLastName))
                {
                    assignedBy = string.Format("{0}{1}", item.AssignmentLastName, item.AssignmentFirstName);
                }
                else
                {
                    assignedBy = string.Format("{0}, {1}", item.AssignmentLastName, item.AssignmentFirstName);
                }

                if (item.TeacherName.IndexOf(',') == item.TeacherName.Length - 2)
                    item.TeacherName = item.TeacherName.Substring(0, item.TeacherName.Length - 2);
                if (item.AssignmentDate.HasValue && dateFormatModel != null)
                {
                    if (criteria.UtcOffsetClient.HasValue)
                    {
                        item.AssignmentDate = LinkIt.BubbleSheetPortal.Common.ConvertValue.ConvertDateTimeByTimeZone(item.AssignmentDate.Value, criteria.UtcOffsetClient.Value);
                    }
                    assignmentDate = item.AssignmentDate.Value.ToString(dateFormatModel.DateFormat);
                }

                sb.AppendFormat("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\" {12}",
                    assignmentDate, assignedBy, item.TestName, item.SchoolName,
                    item.TeacherName, item.ClassName, item.NotStarted, item.Started, item.WaitingForReview, item.Completed, item.Code, item.StudentNames.Replace("&amp;", "&"),
                    System.Environment.NewLine);
            }

            byte[] byteArray = Encoding.UTF8.GetBytes(sb.ToString());
            var stream = new MemoryStream(byteArray);
            var folder = ConfigurationManager.AppSettings["ExportClassTestAssFolder"];
            var bucketName = LinkitConfigurationManager.GetS3Settings().ExportClassTestAssBucket;

            var fileName = string.Format("{0}_{1}_{2}.csv", "Export Test Assignments", DateTime.Now.ToString("yyMMddhhmmss"), Guid.NewGuid().ToString().Substring(0, 8));
            string s3FileName = string.Format("{0}/{1}", folder, fileName);
            var result = _service.UploadRubricFile(bucketName,
                s3FileName, stream);
            var url = _service.GetPublicUrl(bucketName, s3FileName);
            return Json(url, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ExportTestStudentSessions(ExportStudentSessionsCriteria criteria)
        {
            if (string.IsNullOrEmpty(criteria.QTITestClassAssignmentIDs))
            { return Json(false); }
            var stringIds = criteria.QTITestClassAssignmentIDs.ToIntArray().ToList();
            var assignmentCodes = _parameters.QTITestClassAssignmentServices.GetTestCodeByTestClassIds(stringIds);
            var request = new GetTestStudentSessionExportRequest()
            {
                AssignDate = GetDateTime(criteria.DateTime).ToString(),
                UserID = CurrentUser.Id,
                AssignmentCodes = assignmentCodes,
                DistrictID = CurrentUser.IsPublisher || CurrentUser.IsNetworkAdmin ? criteria.DistrictID : CurrentUser.DistrictId,
                SortColumn = "AssignmentDate",
                SortDirection = "DESC",
                StartRow = 0,
                PageSize = 1000000
            };

            var data = _parameters.QTIOnlineTestSessionService.GetTestStudentSessionsExport(request);
            data = data.OrderBy(x => x.SchoolName).ToList();

            var sb = new StringBuilder();
            sb.AppendFormat(
                "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\",\"{13}\",\"{14}\",\"{15}\",\"{16}\",\"{17}\",\"{18}\",\"{19}\" {20}",
                "School", "School Code", "User Name", "User Code", "Term", "Class Name", "Class Section", "Course Number", "First Name", "Last Name",
                "Middle Name", "Student Local ID", $"Student {LabelHelper.StudentStateID}", "Gender", "Race", "Grade", "Assignment Date","Test Name", "Test Code", "Test Session Status", Environment.NewLine);

            foreach (var item in data)
            {
                var status = item.TestSessionStatus;
                if (status.Trim().Equals("Created"))
                    status = "Paused";

                sb.AppendFormat("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\",\"{13}\",\"{14}\",\"{15}\",\"{16}\",\"{17}\",\"{18}\",\"{19}\" {20}",
                item.SchoolName, item.SchoolCode, item.UserName, item.UserCode, item.Term, item.ClassName,
                item.ClassSection, item.CourseNumber, item.FirstName, item.LastName, item.MiddleName, item.StudentLocalID,
                item.StudentStateID, item.Gender, item.Race, item.Grade, item.AssignmentShortDate,item.TestName, item.TestCode, status, Environment.NewLine);
            }

            byte[] byteArray = Encoding.UTF8.GetBytes(sb.ToString());
            var stream = new MemoryStream(byteArray);
            var folder = ConfigurationManager.AppSettings["ExportClassTestAssFolder"];
            var bucketName = LinkitConfigurationManager.GetS3Settings().ExportClassTestAssBucket;

            var fileName = string.Format("{0}_{1}_{2}.csv", "Export Student Session Status", DateTime.Now.ToString("yyMMddhhmmss"), Guid.NewGuid().ToString().Substring(0, 8));
            string s3FileName = string.Format("{0}/{1}", folder, fileName);
            var result = _service.UploadRubricFile(bucketName, s3FileName, stream);
            var url = _service.GetPublicUrl(bucketName, s3FileName);
            return Json(url, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ChangeStatus(int qtiTestClassAssignmentID, int operation, string studentStr = null, int type = 0, bool IsOldUI = false)
        {
            //avoid  modify ajax paramenters
            if (
                !_parameters.VulnerabilityService.HasRigtToEditQtiTestClassAssignment(CurrentUser,
                    qtiTestClassAssignmentID))
            {
                return Json(false);
            }
            var status = 1;
            if (operation == 1)
            {
                status = 0;
            }
            var studentList = new List<int>();
            if (!string.IsNullOrEmpty(studentStr))
            {
                studentList = studentStr.Split(',').Select(Int32.Parse).ToList();
            }
            _parameters.QTITestClassAssignmentServices.ChangeStatus(qtiTestClassAssignmentID, status, CurrentUser.Id, studentList, type, IsOldUI);            
            return Json(true);
        }

        [HttpPost]
        public ActionResult ChangeStatusMutipleAssignment(string qtiTestClassAssignmentIDs, int operationOption, bool IsOldUI = false)
        {
            if (string.IsNullOrEmpty(qtiTestClassAssignmentIDs))
            { return Json(false); }
            var stringIds = qtiTestClassAssignmentIDs.ToIntArray().ToList();
            //avoid  modify ajax paramenters
            if (
                !_parameters.VulnerabilityService.HasRigtToEditMultiQtiTestClassAssignment(CurrentUser,
                    stringIds))
            {
                return Json(false);
            }
            var status = 1;
            if (operationOption == 1)
            {
                status = 0;

            }
            var testsCanActive = _parameters.QTITestClassAssignmentServices.GetVirtualTestsCanActive(stringIds);
            if (testsCanActive.TestsAssignmentCanActiveIds != null)
                stringIds = testsCanActive.TestsAssignmentCanActiveIds;
            var virtualTestNames = new List<string>();
            if (testsCanActive.TestNameErrors != null && testsCanActive.TestNameErrors.Any())
                virtualTestNames.AddRange(testsCanActive.TestNameErrors);
            _parameters.QTITestClassAssignmentServices.ChangeStatusMultipleAssignment(stringIds, status, CurrentUser.Id, IsOldUI);
            
            return Json(new { virtualTestNames = virtualTestNames.AsQueryable(), success = true}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ChangeStatusShowHide(int qtiTestClassAssignmentID, bool isHide, string studentStr = null, int type = 0, bool IsOldUI = false)
        {
            //avoid  modify ajax paramenters
            if (
                !_parameters.VulnerabilityService.HasRigtToEditQtiTestClassAssignment(CurrentUser,
                    qtiTestClassAssignmentID))
            {
                return Json(false);
            }
            var studentList = new List<int>();
            if (!string.IsNullOrEmpty(studentStr))
            {
                studentList = studentStr.Split(',').Select(Int32.Parse).ToList();
            }
            var status = !isHide;
            _parameters.QTITestClassAssignmentServices.ChangeStatusShowHide(qtiTestClassAssignmentID, status, CurrentUser.Id, studentList, type, IsOldUI);

            return Json(true);
        }

        [HttpPost]
        public ActionResult ChangeStatusShowHideMutipleAssignment(string qtiTestClassAssignmentIDs, bool isHide, bool IsOldUI = false)
        {
            if (string.IsNullOrEmpty(qtiTestClassAssignmentIDs))
            { return Json(false); }
            //avoid  modify ajax paramenters
            var stringIds = qtiTestClassAssignmentIDs.ToIntArray().ToList();
            if (
                !_parameters.VulnerabilityService.HasRigtToEditMultiQtiTestClassAssignment(CurrentUser,
                    stringIds))
            {
                return Json(false);
            }
            var status = !isHide;
            _parameters.QTITestClassAssignmentServices.ChangeStatusShowHideMutipleAssignment(stringIds, status, CurrentUser.Id, IsOldUI);

            return Json(true);
        }

        public ActionResult GetPrograms(int? districtID)
        {
            int? districtIDToSearch = null;
            if (CurrentUser.IsPublisher && districtID.HasValue)
            {
                districtIDToSearch = districtID;
            }
            else if (!CurrentUser.IsPublisher && CurrentUser.DistrictId.HasValue)
            {
                districtIDToSearch = CurrentUser.DistrictId;
            }
            else if (!CurrentUser.IsPublisher)
            {
                return Json(new List<ListItem>().AsQueryable(), JsonRequestBehavior.AllowGet);
            }

            var programs = districtIDToSearch.HasValue
                                ? _parameters.ProgramServices.GetProgramsByDistrictID(districtIDToSearch.Value)
                                : new List<Program>().AsQueryable();
            if (!CurrentUser.IsPublisher)
            {
                programs = programs.Where(x => x.AccessLevelID != (int)AccessLevelEnum.LinkItOnly && x.AccessLevelID != (int)AccessLevelEnum.StateUsers);
            }
            if (CurrentUser.IsSchoolAdmin)
            {
                programs =
                    programs.Where(
                        x =>
                            x.AccessLevelID == (int)AccessLevelEnum.DistrictAndSchoolAdmins ||
                            x.AccessLevelID == (int)AccessLevelEnum.AllUsers);
            }
            if (CurrentUser.IsTeacher)
            {
                programs =
                    programs.Where(
                        x => x.AccessLevelID == (int)AccessLevelEnum.AllUsers);

            }
            var data = programs.Select(x => new ListItem { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeassignStudent(int? districtID, int qtiTestClassAssignmentID, int qtiTestStudentAssignmentID)
        {
            int? districtIDToSearch = null;
            if (CurrentUser.IsPublisher && districtID.HasValue)
            {
                districtIDToSearch = districtID;
            }
            else if (!CurrentUser.IsPublisher && CurrentUser.DistrictId.HasValue)
            {
                districtIDToSearch = CurrentUser.DistrictId;
            }

            var isNotStarted =
                _parameters.QTITestClassAssignmentServices.IsTestAssignmentInNotStartedStatus(districtIDToSearch, qtiTestClassAssignmentID,
                                                                                              qtiTestStudentAssignmentID);
            if (!isNotStarted) return Json("started");
            _parameters.QTITestClassAssignmentServices.DeassignStudent(qtiTestClassAssignmentID, qtiTestStudentAssignmentID);
            return Json(true);
        }

        [HttpGet]
        public ActionResult GetDateTimeFromDayOffset(int days)
        {
            var date = DateTime.UtcNow.AddDays(-days).ToShortDateString();
            if (days == 0) { date = DateTime.MinValue.AddYears(1800).ToShortDateString(); }
            return Json(new { Date = date }, JsonRequestBehavior.AllowGet);
        }

        public DateTime GetDateTime(int days)
        {
            if (days == 0) return DateTime.MinValue.AddYears(1800);
            return DateTime.UtcNow.AddDays(-days).Date;
        }

        public string ConvertListIdToStringId(List<int> ListDistricIds)
        {
            var ids = string.Empty;
            if (!ListDistricIds.Any())
            {
                return ids;
            }
            ids = ListDistricIds.Aggregate(ids, (current, id) => current + (id + ","));
            return ids.TrimEnd(new[] { ',' });

        }

        [HttpGet]
        public ActionResult GetStudentByCodeAndStatus(int qtiTestClassAssignmentId)
        {
            if (qtiTestClassAssignmentId > 0)
            {
                var lstStudentStatus = _parameters.QTITestClassAssignmentServices.GetStudentTestStatus(qtiTestClassAssignmentId);
                if (lstStudentStatus != null && lstStudentStatus.Count > 0)
                    return Json(new { success = true, liststudentstatus = lstStudentStatus }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetAllStudents(int qtiTestClassAssignmentId)
        {
            if (qtiTestClassAssignmentId > 0)
            {
                var lstStudentStatus = _parameters.QTITestClassAssignmentServices.GetAllStudents(qtiTestClassAssignmentId);
                if (lstStudentStatus != null)
                    return Json(new { success = true, liststudentstatus = lstStudentStatus }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CanActive(int qtiTestClassAssignmentID, int operation, string studentStr = null, int type = 0, bool IsOldUI = false)
        {
            if (_parameters.QTITestClassAssignmentServices.CanActive(qtiTestClassAssignmentID))
            {
                return ChangeStatus(qtiTestClassAssignmentID, operation, studentStr, type, IsOldUI);
            }

            return Json(false);
        }

        [HttpGet]
        public ActionResult LoadStudentStatusInClass(int qtiTestClassAssignmentId, int classId, int type, int status, bool isHide)
        {
            if (qtiTestClassAssignmentId > 0)
            {
                var listStudentStatus = new List<StudentTestStatus>();
                if (type == (int)AssignmentType.Roster)
                {
                    listStudentStatus = _parameters.QTITestClassAssignmentServices.GetListStudentTestStatus(qtiTestClassAssignmentId);
                    var listAllStudentInClass = _parameters.ParentConnectService.GetAllStudentInClass(classId).Select(s => new StudentTestStatus()
                    {
                        IsHide = SetIsHideForStudent(listStudentStatus, s.StudentID, isHide),
                        Status = SetStatusForStudent(listStudentStatus, s.StudentID, status),
                        StudentFullName = s.FullName,
                        StudentId = s.StudentID,
                    }).ToList();
                    if (listStudentStatus == null || (listStudentStatus != null && listStudentStatus.Count() == 0))
                    {
                        listStudentStatus.AddRange(listAllStudentInClass);
                    }
                    else
                    {
                        var studentIDs = listStudentStatus.Select(s => s.StudentId).ToList();
                        var studentNotList = listAllStudentInClass.Where(w => !studentIDs.Contains(w.StudentId)).ToList();
                        if (studentNotList != null && studentNotList.Count() > 0)
                        {                            
                            listStudentStatus.AddRange(studentNotList);
                        }
                    }
                }
                else
                {
                    listStudentStatus = _parameters.QTITestClassAssignmentServices.GetListStudentTestStatus(qtiTestClassAssignmentId);
                }
                if (listStudentStatus != null)
                {
                    listStudentStatus = listStudentStatus.Distinct().OrderBy(o => o.StudentFullName).ToList();
                    return Json(listStudentStatus, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }
        private int? SetStatusForStudent(List<StudentTestStatus> students, int studentId, int status)
        {
            var studentInfo = students.FirstOrDefault(f => f.StudentId == studentId);
            if (students != null && studentInfo != null)
            {
                return studentInfo.Status;
            }
            return status;
        }
        private bool? SetIsHideForStudent(List<StudentTestStatus> students, int studentId, bool? isHide)
        {
            var studentInfo = students.FirstOrDefault(f => f.StudentId == studentId);
            if (students != null && studentInfo != null)
            {
                return studentInfo.IsHide;
            }
            return isHide;
        }
    }
}
