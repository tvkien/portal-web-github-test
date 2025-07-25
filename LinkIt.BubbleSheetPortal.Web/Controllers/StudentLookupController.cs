using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using System.IO;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using S3Library;
using LinkIt.BubbleSheetPortal.Common;
using System.Diagnostics;
using LinkIt.BubbleSheetPortal.Web.ViewModels.ACTReport;
using LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport;
using LinkIt.BubbleSheetPortal.Models.DTOs.StudentLookup;
using LinkIt.BubbleSheetPortal.Models.Constants;
using LinkIt.BubbleSheetPortal.Services;
using System.Configuration;
using LinkIt.BubbleSheetPortal.Web.Models.DataTable;
using LinkIt.BubbleSheetPortal.Web.ServiceConsumer;
using LinkIt.BubbleSheetPortal.Models.PDFGenerator;
using System.Text.RegularExpressions;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.Required)]
    [VersionFilter]
    public class StudentLookupController : BaseController
    {
        private readonly StudentLookupControllerParameters _parameters;
        private readonly DistrictService _districtService;
        private readonly DistrictDecodeService _districtDecodeService;
        private readonly IS3Service _s3Service;

        private int SelectedDistrictID
        {
            get { return GetSessionValue("SelectedDistrictID"); }
            set { System.Web.HttpContext.Current.Session["SelectedDistrictID"] = value; }
        }

        private int SelectedSchoolID
        {
            get { return GetSessionValue("SelectedSchoolID"); }
            set { System.Web.HttpContext.Current.Session["SelectedSchoolID"] = value; }
        }

        private int GetSessionValue(string valueName)
        {
            if (System.Web.HttpContext.Current.Session[valueName].IsNull())
            {
                return 0;
            }

            int value;
            if (int.TryParse(System.Web.HttpContext.Current.Session[valueName].ToString(), out value))
            {
                return value;
            }
            return 0;
        }
        public ActionResult DistributeByEmail(LookupStudentCustom model)
        {
            if (!CurrentUser.IsPublisher && !CurrentUser.IsNetworkAdmin())
            {
                model.DistrictId = CurrentUser.DistrictId.Value;
            }

            model.UserId = CurrentUser.Id;
            model.RoleId = CurrentUser.RoleId;
            var parser = new DataTableParserProc<LookupStudentViewModel>();
            var studentList = new List<LookupStudent>();
            int? totalRecords = 0;

            if (model.DistrictId > 0)
            {
                var sortColumns = "LastName asc, FirstName asc, Code asc";

                model.FirstName = model.FirstName == null ? null : model.FirstName.Trim();
                model.LastName = model.LastName == null ? null : model.LastName.Trim();
                model.Code = model.Code == null ? null : model.Code.Trim();
                model.StateCode = model.StateCode == null ? null : model.StateCode.Trim();

                studentList = _parameters.StudentServices.LookupStudents(model, 0, 1000000, ref totalRecords, sortColumns, model.SelectedUserIds)
                   .ToList();

            }

            var studentInfor = studentList
                .Where(c => (!c.TheUserLogedIn || !c.HasPassword)
                && (c.RegistrationCode ?? "").Length > 0
                && c.HasEmailAddress
                )
                .Select(c => new { c.StudentId, c.RegistrationCode, c.UserName }).ToArray();
            if (studentInfor.Length <= 0)
            {
                return Json(BaseResponseModel<int>.InstanceSuccess(0), JsonRequestBehavior.AllowGet);
            }

            StudentRegistrationCodeEmailModel objStudentEmail = new StudentRegistrationCodeEmailModel()
            {
                HTTPProtocal = HelperExtensions.GetHTTPProtocal(Request),
                StudentEmailModels = new List<StudentAndRegistrationCode>()
            };

            objStudentEmail.StudentEmailModels = studentInfor
                .Select(c => new StudentAndRegistrationCode()
                {
                    StudentId = c.StudentId,
                    RegistrationCode = c.RegistrationCode,
                    UserName = c.UserName
                }).ToArray();
            var emailCredentialSetting = LinkitConfigurationManager.GetEmailCredentialSetting(EmailSetting.RegistrationUseEmailCredentialKey);

            foreach (var studentId in _parameters.EmailService.SendMailRegistrationCodeToStudent(model.DistrictId.Value, objStudentEmail, emailCredentialSetting))
            {
                _parameters.StudentServices.TrackingLastSendDistributeEmail(studentId);
            }

            return Json(BaseResponseModel<int>.InstanceSuccess(objStudentEmail.StudentEmailModels.Count()), JsonRequestBehavior.AllowGet);
        }


        public StudentLookupController(StudentLookupControllerParameters parameters
            , IS3Service s3Service, DistrictService districtService
            , DistrictDecodeService districtDecodeService)
        {
            _parameters = parameters;
            _s3Service = s3Service;
            _districtService = districtService;
            _districtDecodeService = districtDecodeService;
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.StudentLookup)]
        [AjaxAwareAuthorize]
        public ActionResult Index()
        {
            ViewBag.IsPublisherOrNetworkAdmin = false;
            if (CurrentUser.IsPublisher() || CurrentUser.IsNetworkAdmin())
                ViewBag.IsPublisherOrNetworkAdmin = true;
            ViewBag.IsPubliser = CurrentUser.IsPublisher;
            ViewBag.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
            ViewBag.HasAddNewStudent = CurrentUser.IsAdmin();
            ViewBag.HasGenerateLogin = false;

            if (!CurrentUser.IsPublisherOrNetworkAdmin)
            {
                ViewBag.HasGenerateLogin = _districtDecodeService.GetDistrictDecodeOrConfigurationByLabel(CurrentUser.DistrictId.Value, Constanst.ALLOW_STUDENT_USER_GENERATION);
            }

            return View();
        }

        [HttpGet]
        [AjaxAwareAuthorize]
        public ActionResult GetRacesByDistrict(int? districtId)
        {
            if ((!districtId.HasValue || districtId.Value <= 0) && !CurrentUser.IsPublisher && !CurrentUser.IsNetworkAdmin)
            {
                districtId = CurrentUser.DistrictId;
            }

            if (!Util.HasRightOnDistrict(CurrentUser, districtId.GetValueOrDefault()))
            {
                return Json(new { error = "Has no right on district" }, JsonRequestBehavior.AllowGet);
            }

            var races = LookupStudentGetRace(districtId);
            if (races != null && races.Any())
            {
                var data =
                    races.Select(x => x.Name)
                        .Distinct()
                        .OrderBy(x => x)
                        .Select(x => new ListItemStr { Id = x, Name = x })
                        .ToList();

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json(new List<ListItem>(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AjaxAwareAuthorize]
        public ActionResult GetAdminSchoolsByDistrict(int? districtId)
        {
            if ((!districtId.HasValue || districtId.Value <= 0) && !CurrentUser.IsPublisher && !CurrentUser.IsNetworkAdmin)
            {
                districtId = CurrentUser.DistrictId;
            }

            if (!Util.HasRightOnDistrict(CurrentUser, districtId.GetValueOrDefault()))
            {
                return Json(new { error = "Has no right on district" }, JsonRequestBehavior.AllowGet);
            }
            var schools = LookupStudentGetAdminSchool(districtId);

            if (schools != null && schools.Any())
            {
                var data = schools.Select(x => new ListItem() { Id = x.Id, Name = x.Name }).ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json(new List<ListItem>(), JsonRequestBehavior.AllowGet);
        }

        [AjaxAwareAuthorize]
        public ActionResult GetStudentResult(LookupStudentCustom model)
        {
            if (!CurrentUser.IsPublisher && !CurrentUser.IsNetworkAdmin() && (!model.DistrictId.HasValue || model.DistrictId == 0))
                model.DistrictId = CurrentUser.DistrictId.Value;

            var parser = new DataTableParserProc<GetStudentResultViewModel>();
            var data = new List<GetStudentResultViewModel>().AsQueryable();

            if (model.DistrictId == -1)
            {
                if (model.SchoolId.HasValue && model.SchoolId.Value > 0)
                    model.DistrictId = CurrentUser.DistrictId.Value;
                else
                    return Json(parser.Parse(data, 0), JsonRequestBehavior.AllowGet);
            }

            model.LastName = string.Empty;

            if (!string.IsNullOrEmpty(model.FirstName))
            {
                model.FirstName = model.FirstName.Replace(',', ' ');
            }

            if (model.DistrictId.HasValue && model.DistrictId.Value > 0)
            {
                if (!Util.HasRightOnDistrict(CurrentUser, model.DistrictId.Value))
                {
                    return Json(new { error = "Has no right on district" }, JsonRequestBehavior.AllowGet);
                }
            }
            if (model.SchoolId.HasValue && model.SchoolId.Value > 0)
            {
                var authorizedSchool = LookupStudentGetAdminSchool(model.DistrictId);
                if (!authorizedSchool.Any(x => x.Id == model.SchoolId.Value))
                {
                    return Json(new { error = "Has no right on school" }, JsonRequestBehavior.AllowGet);
                }

            }

            int? totalRecords = 0;

            if (!CurrentUser.IsPublisher && !CurrentUser.IsNetworkAdmin() && (!model.DistrictId.HasValue || model.DistrictId == 0))
                model.DistrictId = CurrentUser.DistrictId.Value;

            if (model.DistrictId > 0)
            {
                SelectedDistrictID = model.DistrictId.Value; // This session value is used in EditStudent page
                SelectedSchoolID = model.SchoolId.HasValue ? model.SchoolId.Value : 0;

                model.UserId = CurrentUser.Id;
                model.RoleId = CurrentUser.RoleId;

                var sortColumns = parser.SortableColumns;

                model.FirstName = model.FirstName?.Trim();
                model.LastName = model.LastName?.Trim();
                model.Code = model.Code?.Trim();
                model.StateCode = model.StateCode?.Trim();

                var userSchools = _parameters.UserSchoolServices.GetSchoolsUserHasAccessTo(CurrentUser.Id).ToList();
                var allowStudentUserGeneration = _districtDecodeService.GetDistrictDecodeOrConfigurationByLabel(model.DistrictId.Value, Constanst.ALLOW_STUDENT_USER_GENERATION);

                data = _parameters.StudentServices.LookupStudents(model, parser.StartIndex, parser.PageSize, ref totalRecords, sortColumns)
                    .Select(x => new GetStudentResultViewModel
                    {
                        Code = x.Code,
                        FullName = x.FullName,
                        GenderCode = x.GenderCode,
                        GradeName = x.GradeName,
                        RaceName = x.RaceName,
                        SchoolName = x.SchoolName,
                        StateCode = x.StateCode,
                        StudentId = x.StudentId,
                        Status = x.Status,
                        CanAccess = CanAccessStudentByAdminSchool(x.AdminSchoolId, x.DistrictId, userSchools),
                        RegistrationCode = x.RegistrationCode,
                        Email = x.Email,
                        RegistrationCodeEmailLastSent = x.RegistrationCodeEmailLastSent,
                        UserName = x.UserName,
                        SharedSecret = allowStudentUserGeneration ? x.SharedSecret : null,
                        HasEdit = CurrentUser.IsAdmin(),
                        HasDelete = CurrentUser.IsAdmin(),
                        HasView = CurrentUser.IsTeacher() || CurrentUser.IsAdmin(),
                        HasResetPassword = CurrentUser.IsTeacher() || CurrentUser.IsAdmin(),
                        HasGenerateLogin = allowStudentUserGeneration,
                        Classes = x.Classes
                    }).AsQueryable();
            }

            return Json(parser.Parse(data, totalRecords ?? 0), JsonRequestBehavior.AllowGet);
        }

        private bool CanAccessStudentByAdminSchool(int? adminSchoolId, int? districtId, List<UserSchool> userSchools)
        {
            if (adminSchoolId.HasValue
                && !CurrentUser.IsLinkItAdminOrPublisher()
                && !CurrentUser.IsNetworkAdmin
                && (!CurrentUser.IsDistrictAdminOrPublisher || (CurrentUser.IsDistrictAdminOrPublisher && CurrentUser.DistrictId != districtId))
                )
            {
                if (userSchools.All(en => en.SchoolId != adminSchoolId))
                {
                    return false;
                }
            }

            return true;
        }

        [AjaxAwareAuthorize]
        public ActionResult ActivateStudent(int studentId)
        {
            if (_parameters.VulnerabilityService.CheckUserPermissionOnStudent(CurrentUser, studentId.ToString()))
            {
                var student = _parameters.StudentServices.GetStudentById(studentId);

                if (student != null)
                {
                    student.Status = 1;
                    _parameters.StudentServices.Save(student);
                    return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { Result = false }, JsonRequestBehavior.AllowGet);
        }

        [AjaxAwareAuthorize]
        public ActionResult DeactivateStudent(int studentId)
        {
            if (_parameters.VulnerabilityService.CheckUserPermissionOnStudent(CurrentUser, studentId.ToString()))
            {
                var student = _parameters.StudentServices.GetStudentById(studentId);

                if (student != null)
                {
                    student.Status = 2;
                    _parameters.StudentServices.Save(student);
                    return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { Result = false }, JsonRequestBehavior.AllowGet);
        }

        private List<Race> LookupStudentGetRace(int? districtId)
        {
            int currentDistrictId = districtId ?? (CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0);
            var races = _parameters.StudentServices.LookupStudentGetRace(currentDistrictId, CurrentUser.Id,
                CurrentUser.RoleId);
            return races;
        }

        private List<School> LookupStudentGetAdminSchool(int? districtId)
        {
            int currentDistrictId = districtId ?? (CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0);
            var schools = _parameters.StudentServices.LookupStudentGetAdminSchool(currentDistrictId, CurrentUser.Id,
                CurrentUser.RoleId);
            return schools;
        }

        [AjaxAwareAuthorize]
        public ActionResult GenRCode(GenerateStudentLoginViewModel model)
        {
            if (!CurrentUser.IsPublisherOrNetworkAdmin)
                model.DistrictId = CurrentUser.DistrictId;

            var studentIds = model.StudentIds;
            var allowStudentUserGeneration = _districtDecodeService.GetDistrictDecodeOrConfigurationByLabel(model.DistrictId.Value, Constanst.ALLOW_STUDENT_USER_GENERATION);
            if (allowStudentUserGeneration)
            {
                var students = _parameters.StudentServices.GetStudents(model.StudentIds);
                studentIds = students.Where(x => !string.IsNullOrEmpty(x.Email)).Select(x => x.Id).ToList();
                _parameters.StudentServices.GenerateStudentLogin(students);
            }

            if (studentIds.Count > 0)
            {
                var studentRegistrationCodeLength = _parameters.StudentServices.GetStudentRegistrationCodeLength();
                _parameters.StudentServices.GenerateRegistrationCode(studentIds, CurrentUser.Id, studentRegistrationCodeLength);
            }
            
            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }

        private byte[] Print(LookupStudentCustom model, out string url)
        {
            if (!CurrentUser.IsPublisher && !CurrentUser.IsNetworkAdmin())
            {
                model.DistrictId = CurrentUser.DistrictId.Value;
            }

            model.UserId = CurrentUser.Id;
            model.RoleId = CurrentUser.RoleId;

            url = Url.Action("ReportPrinting", "StudentLookup", model, HelperExtensions.GetHTTPProtocal(Request));
            var pdf = ExportToPDF(url, model.TimezoneOffset);
            return pdf;
        }

        private byte[] ExportToPDF(string url, int timezoneOffset)
        {
            DateTime dt = DateTime.UtcNow.AddMinutes(timezoneOffset * (-1));

            string footerUrl = Url.Action("RenderFooter", "StudentLookup", null, HelperExtensions.GetHTTPProtocal(Request));
            string headerUrl = Url.Action("RenderHeader", "StudentLookup", new
            {
                leftLine1 = "Generated: " + dt.DisplayDateWithFormat(true)// String.Format("{0:g}", dt)
            }, HelperExtensions.GetHTTPProtocal(Request));

            string args =
                string.Format("--footer-html \"{0}\" --header-html \"{2}\" --header-spacing 5 \"{1}\" - "
                    , footerUrl
                    , url
                    , headerUrl
                    );

            var startInfo = new ProcessStartInfo(Server.MapPath("~/PDFTool/wkhtmltopdf.exe"), args)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
            };
            var proc = new Process { StartInfo = startInfo };
            proc.Start();

            string output = proc.StandardOutput.ReadToEnd();
            byte[] buffer = proc.StandardOutput.CurrentEncoding.GetBytes(output);
            proc.WaitForExit();
            proc.Close();

            return buffer;
        }

        public ActionResult RenderFooter()
        {
            return PartialView("_Footer");
        }

        public ActionResult RenderHeader(string leftLine1)
        {
            var obj = new FooterData { LeftLine1 = leftLine1 };
            return PartialView("_Header", obj);
        }

        [AjaxAwareAuthorize]
        public ActionResult Generate(LookupStudentCustom model)
        {
            if (!CurrentUser.IsPublisherOrNetworkAdmin)
                model.DistrictId = CurrentUser.DistrictId;

            string url = string.Empty;
            var pdf = Print(model, out url);

            var folder = LinkitConfigurationManager.GetS3Settings().ReportPrintingFolder;
            var bucketName = LinkitConfigurationManager.GetS3Settings().ReportPrintingBucketName;

            _s3Service.UploadRubricFile(bucketName, folder + "/" + model.StudentDetailPrintingFileName, new MemoryStream(pdf));

            var s3Url = _s3Service.GetPublicUrl(bucketName, folder + "/" + model.StudentDetailPrintingFileName);
            return Json(new { IsSuccess = true, Url = s3Url });
        }

        [HttpGet]
        public ActionResult ReportPrinting(LookupStudentCustom model)
        {
            var parser = new DataTableParserProc<LookupStudentViewModel>();
            var data = new List<string>();
            int? totalRecords = 0;

            if (model.DistrictId > 0)
            {
                var districtInfo = _districtService.GetDistrictById(model.DistrictId.Value);
                var objTemplateEmail = _districtDecodeService.GetDistrictDecodeOfDistrictOrConfigurationByLabel(model.DistrictId.Value, Constanst.TemplateStudentRegistrationCode);
                string studentPortalUrl = string.Format("{0}://{1}.{2}/Student", HelperExtensions.GetHTTPProtocal(Request), districtInfo.LICode.ToLower(), ConfigurationManager.AppSettings["LinkItUrl"]);

                var sortColumns = "LastName asc, FirstName asc, Code asc";

                model.FirstName = model.FirstName == null ? null : model.FirstName.Trim();
                model.LastName = model.LastName == null ? null : model.LastName.Trim();
                model.Code = model.Code == null ? null : model.Code.Trim();
                model.StateCode = model.StateCode == null ? null : model.StateCode.Trim();

                var students = _parameters.StudentServices.LookupStudents(model, 0, 1000000, ref totalRecords, sortColumns, model.SelectedUserIds)
                    .Where(c => (!c.TheUserLogedIn || !c.HasPassword) && !string.IsNullOrEmpty(c.RegistrationCode))
                    .Select(x => new LookupStudentViewModel
                    {
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        SchoolName = x.SchoolName, 
                        RegistrationCode = x.RegistrationCode,
                        UserName = x.UserName
                    }).ToList();

                var allowStudentUserGeneration = _districtDecodeService.GetDistrictDecodeOrConfigurationByLabel(model.DistrictId.Value, Constanst.ALLOW_STUDENT_USER_GENERATION);

                foreach (var student in students)
                {
                    var body = objTemplateEmail.Value;
                    var userDesc = allowStudentUserGeneration && !string.IsNullOrEmpty(student.UserName)
                        ? $" and your username is <b>{student.UserName}</b>" : string.Empty;

                    var keyPairValues = new Dictionary<string, string>()
                        {
                            { "[StudentFirstName]",student.FirstName },
                            { "[DistrictName]",districtInfo.Name},
                            { "[StudentPortalURL]",studentPortalUrl },
                            { "[RegistrationCode]",student.RegistrationCode },
                            { "[StudentCurrentSchoolName]", student.SchoolName },
                            { "[UserFirstName]",student.FirstName },
                            { "[UserLastName]",student.LastName },
                            { "[UserDesc]", userDesc}
                        };
                    foreach (var keyPairValue in keyPairValues)
                    {
                        body = body.Replace(keyPairValue.Key, keyPairValue.Value);
                    }
                    data.Add(body);
                }
            }
            return View(data);
        }

        [HttpGet]
        [AjaxAwareAuthorize]
        public ActionResult StudentLookupGetGradesFilter(int? districtId)
        {
            if ((!districtId.HasValue || districtId.Value <= 0) && !CurrentUser.IsPublisher && !CurrentUser.IsNetworkAdmin)
            {
                districtId = CurrentUser.DistrictId;
            }

            var tmp = _parameters.GradeService.StudentLookupGetGradesFilter(CurrentUser.Id, districtId.GetValueOrDefault(),
                CurrentUser.RoleId);
            if (tmp != null && tmp.Any())
            {
                var data = tmp.Select(x => new ListItem() { Id = x.Id, Name = x.Name }).ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json(new List<ListItem>(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetStudentSession(StudentSessionFilterRequestDto request)
        {
            var studentSessions = _parameters.StudentServices.GetStudentSession(request.StudentId, CurrentUser.DistrictId ?? 0, CurrentUser.StateId ?? 0);

            if (!string.IsNullOrEmpty(request.sSearch))
            {
                studentSessions = studentSessions.Where(x => x.TimeStamp.ToLower().Contains(request.sSearch.ToLower().Trim())
                                       || x.PointOfEntryDisplay.ToLower().Contains(request.sSearch.Trim().ToLower())
                                       || x.BrowserNameDisplay.ToLower().Contains(request.sSearch.Trim().ToLower())
                                       || x.StudentWANIP.ToLower().Contains(request.sSearch.Trim().ToLower())).ToList();
            }

            if (!string.IsNullOrWhiteSpace(request.sColumns) && request.iSortCol_0.HasValue)
            {
                var columns = request.sColumns.Split(',');
                var sortColumn = columns[request.iSortCol_0.Value];
                var sortDirection = request.sSortDir_0.Equals("desc") ? "DESC" : "ASC";
                switch (sortColumn)
                {
                    case "TimeStamp":
                        studentSessions = sortDirection.Equals("ASC")
                            ? studentSessions.OrderBy(x => x.TimeStampDate).ToList()
                            : studentSessions.OrderByDescending(x => x.TimeStampDate).ToList();
                        break;
                    case "PointOfEntryDisplay":
                        studentSessions = sortDirection.Equals("ASC")
                            ? studentSessions.OrderBy(x => x.PointOfEntryDisplay).ToList()
                            : studentSessions.OrderByDescending(x => x.PointOfEntryDisplay).ToList();
                        break;
                    case "BrowserNameDisplay":
                        studentSessions = sortDirection.Equals("ASC")
                            ? studentSessions.OrderBy(x => x.BrowserNameDisplay).ToList()
                            : studentSessions.OrderByDescending(x => x.BrowserNameDisplay).ToList();
                        break;
                    case "StudentWANIP":
                        studentSessions = sortDirection.Equals("ASC")
                            ? studentSessions.OrderBy(x => x.StudentWANIP).ToList()
                            : studentSessions.OrderByDescending(x => x.StudentWANIP).ToList();
                        break;
                }
            }

            var result = new GenericDataTableResponse<StudentSessionDto>()
            {
                sEcho = request.sEcho,
                sColumns = request.sColumns,
                aaData = studentSessions.Skip(request.iDisplayStart).Take(request.iDisplayLength).ToList(),
                iTotalDisplayRecords = studentSessions.Count,
                iTotalRecords = studentSessions.Count
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxAwareAuthorize]
        public ActionResult ExportStudentLogin(GenerateStudentLoginViewModel model)
        {
            if (!CurrentUser.IsPublisherOrNetworkAdmin)
                model.DistrictId = CurrentUser.DistrictId;

            var url = Url.Action("ReportStudentLoginPrinting", "StudentLookup",
                new {
                    studentIds = model.StudentIds.ConvertToString(),
                    districtId = model.DistrictId,
                    schoolId = model.SchoolId
                },
                HelperExtensions.GetHTTPProtocal(Request));

            var pdfBytes = ExportToPDF(url, 0);

            var folder = LinkitConfigurationManager.GetS3Settings().ReportPrintingFolder;
            var bucketName = LinkitConfigurationManager.GetS3Settings().ReportPrintingBucketName;
            var keyName = $"{folder}/StudentLogin_{DateTime.UtcNow:yyyyMMddHHmmssffff}.pdf";

            _s3Service.UploadRubricFile(bucketName, keyName, new MemoryStream(pdfBytes));

            var s3Url = _s3Service.GetPublicUrl(bucketName, keyName);
            return Json(new { Success = true, Url = s3Url });
        }

        [HttpGet]
        public ActionResult ReportStudentLoginPrinting(string studentIds, int? districtId, int? schoolId)
        {
            var req = new LookupStudentCustom
            {
                DistrictId = districtId ?? CurrentUser.DistrictId,
                UserId = CurrentUser.Id,
                RoleId = CurrentUser.RoleId,
                ShowInactiveStudent = true,
                SchoolId = schoolId
            };
            int? totalRecords = 0;
            var items = _parameters.StudentServices.LookupStudents(req, 0, int.MaxValue, ref totalRecords, nameof(LookupStudent.FullName), studentIds);
            var students = studentIds.ToIntList().Select(id => items.FirstOrDefault(x => x.StudentId == id)).Where(x => x != null).ToList();

            return View(new PrintStudentLoginViewModel { Students = students });
        }

        #region Student login slip
        public ActionResult GeneratePDFStudentLogin(LookupStudentCustom model)
        {
            if (!CurrentUser.IsPublisherOrNetworkAdmin)
                model.DistrictId = CurrentUser.DistrictId;

            if (string.IsNullOrEmpty(model.SelectedUserIds)) return Json(new { IsSuccess = false });

            var url = $"{HelperExtensions.GetHTTPProtocal(Request)}://{HttpContext.Request.Url.Host}/student";

            string s3CSSKey = LinkitConfigurationManager.GetS3Settings()?.S3CSSKey;
            var districtLogo = string.Format("{0}{1}-logo.png", s3CSSKey, model.DistrictId);
            UrlUtil.CheckUrlStatus(districtLogo);

            var studentInfo = _parameters.StudentServices.GetStudentLoginSlip(model.SelectedUserIds, url, districtLogo);
            if (studentInfo == null || studentInfo.Count < 1) return Json(new { IsSuccess = false });

            var fileName = string.Empty;
            var htmlTemplate = string.Empty;
            var template = string.Empty;
            try
            {
                if (model.SingleTemplate)
                {
                    template = _districtDecodeService.GetDistrictDecodeOfDistrictOrConfigurationByLabel(model.DistrictId ?? 0, ContaintUtil.SlipSingleTemplate)?.Value;
                    fileName = $"One_Page_Per_Student";
                    htmlTemplate = GenerateHtmlStudentLoginSlipSingle(this, studentInfo, template);
                }
                else
                {
                    template = _districtDecodeService.GetDistrictDecodeOfDistrictOrConfigurationByLabel(model.DistrictId ?? 0, ContaintUtil.SlipMultipleTemplate)?.Value;
                    fileName = $"Multiple_Students";
                    htmlTemplate = GenerateHtmlStudentLoginSlipMultiple(this, studentInfo, template);
                }
                var pdfGeneratorModel = new PdfGeneratorModel()
                {
                    Html = htmlTemplate,
                    FileName = $"{fileName}_{DateTime.UtcNow:yyyyMMdd_HHmmss}",
                    Folder = "StudentLookup"
                };
                var pdfData = InvokePdfGeneratorService(pdfGeneratorModel);
                return Json(new { IsSuccess = true, Url = pdfData });
            }
            catch
            {
                return Json(new { IsSuccess = false, Message = "Please check the template again." });
            }
        }
        private static string GenerateHtmlStudentLoginSlipSingle(Controller controller, List<StudentLoginSlipDto> models, string template)
        {
            var content = controller.RenderRazorViewToString("v2/SlipSingleTemplate", null);

            var dataBindings = new Dictionary<string, string>();
            var dataTemplate = string.Empty;

            var count = models.Count;
            for (int i = 0; i < count; i++)
            {
                var studentTemplate = string.Empty;
                dataBindings = BindingDataStudentSlipTemplate(models[i], dataBindings);
                studentTemplate = ReplacePlaceholders(template, dataBindings);
                dataTemplate += studentTemplate;

                if (i < count - 1)
                {
                    dataTemplate += "<div class=\"page-break\"></div>";
                }
            }

            return content.Replace("{content}", dataTemplate.Replace("''", "'"));
        }
        private static string GenerateHtmlStudentLoginSlipMultiple(Controller controller, List<StudentLoginSlipDto> models, string template)
        {
            var content = controller.RenderRazorViewToString("v2/SlipMultipleTemplate", null);

            var dataBindings = new Dictionary<string, string>();

            var dataTemplate = string.Empty;

            for (int i = 0; i < models.Count; i++)
            {
                var studentTemplate = string.Empty;
                dataBindings = BindingDataStudentSlipTemplate(models[i], dataBindings);

                dataTemplate += "<div class='row' style='display: flow-root;'>";
                studentTemplate = ReplacePlaceholders(template, dataBindings);
                dataTemplate += studentTemplate;

                if (models.Count > 1 && i < models.Count - 1)
                {
                    i++;
                    dataBindings = BindingDataStudentSlipTemplate(models[i], dataBindings);
                    studentTemplate = ReplacePlaceholders(template, dataBindings);
                    dataTemplate += studentTemplate;
                }
                dataTemplate += "</div>";
            }

            return content.Replace("{content}", dataTemplate.Replace("''", "'"));
        }
        private string InvokePdfGeneratorService(PdfGeneratorModel model)
        {
            var pdfUrl = PdfGeneratorConsumer.InvokePdfGeneratorService(model.Html, model.FileName, model.Folder, CurrentUser.UserName);

            if (string.IsNullOrWhiteSpace(pdfUrl)) return string.Empty;

            var downloadPdfData = new DownloadPdfData { FilePath = pdfUrl, UserID = CurrentUser.Id, CreatedDate = DateTime.UtcNow };

            _parameters.DownloadPdfService.SaveDownloadPdfData(downloadPdfData);
            var downLoadUrl = Url.Action("Index", "DownloadPdf", new { pdfID = downloadPdfData.DownloadPdfID }, HelperExtensions.GetHTTPProtocal(Request));

            return downLoadUrl;
        }
        private static Dictionary<string, string> BindingDataStudentSlipTemplate(StudentLoginSlipDto model, Dictionary<string, string> dataBindings)
        {
            dataBindings = new Dictionary<string, string>();
            dataBindings.Add("{url}", model.Url);
            dataBindings.Add("{district_logo}", model.DistrictLogo);
            dataBindings.Add("{first_name}", model.FirstName);
            dataBindings.Add("{last_name}", model.LastName);
            dataBindings.Add("{school_name}", model.SchoolName);
            dataBindings.Add("{local_code}", model.LocalCode);
            dataBindings.Add("{alt_code}", model.AltCode);
            dataBindings.Add("{email}", model.Email);
            dataBindings.Add("{phone_number}", model.PhoneNumber);
            dataBindings.Add("{registration_code}", model.RegistrationCode);
            dataBindings.Add("{year_level}", model.YearLevel);
            dataBindings.Add("{user_name}", model.UserName);
            dataBindings.Add("{pass_code}", model.PassCode);
            dataBindings.Add("{class}", model.Class);
            dataBindings.Add("{classes}", model.Classes);
            return dataBindings;
        }

        private static string ReplacePlaceholders(string template, Dictionary<string, string> dataBindings)
        {
            if (string.IsNullOrEmpty(template) || dataBindings == null || dataBindings.Count == 0)
                return template;

            string pattern = string.Join("|", dataBindings.Keys
                .Select(Regex.Escape));

            var regex = new Regex(pattern, RegexOptions.Compiled);

            return regex.Replace(template, match =>
            {
                var key = match.Value;
                return dataBindings.TryGetValue(key, out var value) ? value ?? string.Empty : match.Value;
            });
        }
        #endregion

    }
}
