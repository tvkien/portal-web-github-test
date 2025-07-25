using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;
using Envoc.Core.Shared.Extensions;
using FluentValidation.Results;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Models.DTOs.ManageParent;
using LinkIt.BubbleSheetPortal.Web.Models.DataTable;
using LinkIt.BubbleSheetPortal.Models.Enum;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport;
using System.Diagnostics;
using S3Library;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using System.IO;
using LinkIt.BubbleSheetPortal.Web.ViewModels.ACTReport;
using LinkIt.BubbleSheetPortal.Models.Constants;
using LinkIt.BubbleSheetPortal.Models.DTOs.Commons;
using LinkIt.BubbleSheetPortal.Services;
using System.Configuration;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.Required)]
    [VersionFilter]
    public class ManageParentController : BaseController
    {
        private readonly ManageParentControllerParameters _parameters;

        private readonly DistrictDecodeService _districtDecodeService;
        private readonly IS3Service _s3Service;

        public ManageParentController(ManageParentControllerParameters parameters, IS3Service s3Service, DistrictDecodeService districtDecodeService)
        {
            this._parameters = parameters;
            _s3Service = s3Service;
            _districtDecodeService = districtDecodeService;

        }
        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.ManageParent)]
        [AjaxAwareAuthorize]
        public ActionResult Index()
        {
            ViewBag.IsPublisherOrNetworkAdmin = false;
            if (CurrentUser.IsPublisher() || CurrentUser.IsNetworkAdmin())
                ViewBag.IsPublisherOrNetworkAdmin = true;
            ViewBag.IsPubliser = CurrentUser.IsPublisher;
            ViewBag.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
            return View("ManageParents");
        }
        [AjaxAwareAuthorize]
        public ActionResult GetParentList(FilterParentRequestModel criteria)
        {
            if (!CurrentUser.IsPublisher && !CurrentUser.IsNetworkAdmin() && (!criteria.DistrictId.HasValue || criteria.DistrictId == 0))
                criteria.DistrictId = CurrentUser.DistrictId.Value;


            if (criteria.DistrictId == -1)
            {
                if (criteria.SchoolId.HasValue && criteria.SchoolId.Value > 0)
                    criteria.DistrictId = CurrentUser.DistrictId.Value;
                else
                    return Json(
                       new GenericDataTableResponse<ParentGridViewModel>()
                       {
                           sEcho = criteria.sEcho,
                           sColumns = criteria.sColumns,
                           aaData = new List<ParentGridViewModel>(),
                           iTotalDisplayRecords = 0,
                           iTotalRecords = 0
                       }, JsonRequestBehavior.AllowGet);
            }

            if (!CurrentUser.IsPublisher && !CurrentUser.IsNetworkAdmin() && (!criteria.DistrictId.HasValue || criteria.DistrictId == 0))
            {
                criteria.DistrictId = CurrentUser.DistrictId.Value;
            }

            criteria.UserId = CurrentUser.Id;
            criteria.RoleId = CurrentUser.RoleId;
            criteria.DateTimeUTC = DateTime.UtcNow;

            var parentList = _parameters.ManageParentService.GetParentList(criteria);

            var result = new GenericDataTableResponse<ParentGridViewModel>()
            {
                sEcho = criteria.sEcho,
                sColumns = criteria.sColumns,
                aaData = parentList.Data.ToList(),
                iTotalDisplayRecords = parentList.TotalRecord,
                iTotalRecords = parentList.TotalRecord
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.ManageParent)]
        [AjaxAwareAuthorize]
        public ActionResult Edit(int parentUserId = 0)
        {
            if (!_parameters.ManageParentService.CanEditParent(CurrentUser.Id, CurrentUser.RoleId, CurrentUser.DistrictId, parentUserId))
            {
                return RedirectToAction("Add");
            }
            if (parentUserId == 0)
            {
                return RedirectToAction("Index");
            }
            if (!_parameters.ManageParentService.IsParentUser(parentUserId))
            {
                return RedirectToAction("Add");
            }
            AddOrEditViewBag(userId: parentUserId);
            return View("AddOrEditParent");
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.ManageParent)]
        [AjaxAwareAuthorize]
        public ActionResult Add()
        {
            AddOrEditViewBag();
            return View("AddOrEditParent");
        }
        [HttpPost]
        [AjaxOnly]
        public ActionResult DeactivateParent(int parentUserId)
        {
            if (!_parameters.VulnerabilityService.HasRightToAcessUserWithoutCheckStatus(CurrentUser, parentUserId, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            var listError = new List<ValidationFailure>();
            var user = _parameters.UserService.GetUserById(parentUserId);
            if (user.IsNull())
            {
                listError.Add(new ValidationFailure("", "Cannot deactivate because the parent's info did not exist."));
                return Json(new { Success = false, ErrorList = listError }, JsonRequestBehavior.AllowGet);
            }

            user.UserStatusId = (int)UserStatus.Inactive;
            _parameters.UserService.SaveUser(user);

            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult CreateNewParent(CreateParentRequestModel parentModel)
        {
            parentModel.CurrentUserRoleId = CurrentUser.RoleId;
            if (!new int[] { (int)RoleEnum.Publisher, (int)RoleEnum.NetworkAdmin }.Contains(CurrentUser.RoleId))
            {
                parentModel.DistrictId = CurrentUser.DistrictId ?? 0;
                parentModel.StateId = CurrentUser.StateId ?? 0;
            }
            parentModel.SetValidator(_parameters.CreateParentModelValidator);
            parentModel.RoleId = (int)RoleEnum.Parent;

            if (!parentModel.IsValid)
            {
                return Json(new { Success = false, ErrorList = parentModel.ValidationErrors });
            }
            //avoid ajax modifying parameter
            //check right on district
            if (parentModel.DistrictId > 0 && !Util.HasRightOnDistrict(CurrentUser, parentModel.DistrictId))
            {
                return Json(new { Success = false, ErrorList = new List<ValidationFailure>(1) { new ValidationFailure("customError", "Has no right on this " + LabelHelper.DistrictLabel + ".") } });
            }

            try
            {
                var createdParentUser = CreateNewUser(parentModel);                 
                return Json(new
                {
                    Success = true,
                    CreatedParentUserId = createdParentUser.Id
                });
            }
            catch (Exception e)
            {
                return ShowJsonResultException(parentModel, e.Message);
            }

        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult UpdateParentInfo(UpdateParentRequestModel parentModel)
        {
            parentModel.CurrentUserRoleId = CurrentUser.RoleId;
            parentModel.SetValidator(_parameters.UpdateParentModelValidator);
            if (!parentModel.IsValid)
            {
                return Json(new { Success = false, ErrorList = parentModel.ValidationErrors });
            }

            try
            {
                _parameters.ManageParentService.UpdateParentUserInfo(parentModel);
                if(parentModel.ParentId > 0 && parentModel.ParentMetaDatas.Count > 0)
                {
                    _parameters.ParentService.SaveParentMetas(parentModel.ParentId, parentModel.ParentMetaDatas);
                }
                if (parentModel.ParentId > 0 && parentModel.StudentParents != null && parentModel.StudentParents.Count > 0)
                {
                    _parameters.ManageParentService.UpdateStudentParents(parentModel.ParentId, parentModel.StudentParents);
                }
                return Json(new
                {
                    Success = true
                });
            }
            catch (Exception e)
            {
                return ShowJsonResultException(parentModel, e.Message);
            }

        }
        private User CreateNewUser(CreateParentRequestModel parentModel)
        {
            if (!Enum.IsDefined(typeof(Permissions), Permissions.Parent))
            {
                throw new ArgumentException("Permission level does not exist.");
            }

            var user = InitializeNewUser(parentModel);
            _parameters.UserService.SaveUser(user);
            
            var parent = InitializeNewParent(user);
            _parameters.ParentService.SaveParent(parent);
            
            _parameters.ParentService.SaveParentMetas(parent.ParentID, parentModel.ParentMetaDatas);
                        
            if (parentModel.StudentParents != null && parentModel.StudentParents.Count > 0)
            {
                _parameters.ManageParentService.UpdateStudentParents(parent.ParentID, parentModel.StudentParents);
            }
            return user;
        }
        private User InitializeNewUser(CreateParentRequestModel model)
        {
            return new User
            {
                UserName = model.UserName,
                EmailAddress = model.UserName,
                HashedPassword = string.Empty,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.Phone,
                LocalCode = model.UserName,
                AddedByUserId = CurrentUser.Id,
                ApiAccess = false,
                DistrictGroupId = 0,
                UserStatusId = model.StudentIdsThatBeAddedOnCommit?.Length > 0 ? (int)UserStatus.Active : (int)UserStatus.Inactive,
                RoleId = model.RoleId,
                StateId = model.StateId == 0 ? CurrentUser.StateId.GetValueOrDefault() : model.StateId,
                DistrictId = model.DistrictId,
                DateConfirmedActive = DateTime.Today,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                ModifiedUser = CurrentUser.Id,
                ModifiedBy = "Portal",
            };
        }

        private ParentDto InitializeNewParent(User user)
        {
            return new ParentDto
            {
                Code = user.LocalCode,
                DistrictID = user.DistrictId.GetValueOrDefault(),
                Email = user.EmailAddress,
                UserID = user.Id,
                CreatedBy = CurrentUser.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,                
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,                
                ModifiedBy = "Portal",
            };
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult ActivateParent(int? parentUserId)
        {
            if (parentUserId.HasValue && !_parameters.VulnerabilityService.HasRightToAcessUserWithoutCheckStatus(CurrentUser, parentUserId.Value, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            User user = _parameters.UserService.GetUserById(parentUserId.GetValueOrDefault());
            if (user.IsNotNull())
            {
                user.UserStatusId = (int)UserStatus.Active;
                _parameters.UserService.SaveUser(user);
            }
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        private void AddOrEditViewBag(int userId = 0)
        {
            ViewBag.IsPublisherOrNetworkAdmin = false;
            if (CurrentUser.IsPublisher() || CurrentUser.IsNetworkAdmin())
                ViewBag.IsPublisherOrNetworkAdmin = true;
            ViewBag.IsPubliser = CurrentUser.IsPublisher;
            ViewBag.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
            ViewBag.roleId = CurrentUser.RoleId;
            ViewBag.parentUserId = userId;
            var title = userId > 0 ? "Edit Parent Information" : "Add New Parent";
            ViewBag.Title = HelperExtensions.FormatPageTitle(ContaintUtil.DataAdmin, title);

        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.ManageParent)]
        [AjaxAwareAuthorize]
        public ActionResult GetParentInfo(int parentUserId)
        {
            AddOrEditParentViewModelDto parentUserInfo = new AddOrEditParentViewModelDto();
            if (!CurrentUser.IsPublisher && !CurrentUser.IsNetworkAdmin)
            {
                parentUserInfo.StateId = CurrentUser.StateId ?? 0;
                parentUserInfo.DistrictId = CurrentUser.DistrictId ?? 0;
            };
            if (parentUserId > 0)
            {
                parentUserInfo = _parameters.ManageParentService.GetParentUserInfo(parentUserId);                 
            }
            parentUserInfo.ParentMetaDatas = GetParentMetaDataByParentId(parentUserInfo.ParentId, parentUserInfo.DistrictId);
            return Json(parentUserInfo, JsonRequestBehavior.AllowGet);
        }
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.ManageParent)]
        [AjaxAwareAuthorize]
        public ActionResult GetChildrenList(GetChildrenListRequestModel criteria)
        {

            var result = _parameters.ManageParentService.GetChildrenList(criteria, CurrentUser.Id, CurrentUser.RoleId, CurrentUser.DistrictId);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAvailableProgramAndGrade(int parentUserId, int districtId)
        {
            var selectedDistrictID = districtId;
            int parentId = 0;
            int userId = parentUserId;
            var parent = _parameters.ParentService.GetParentViaUserId(parentUserId);
            if (parent != null)
                parentId = parent.ParentID;

            if (!CurrentUser.IsPublisher() && !CurrentUser.IsNetworkAdmin)
            {
                var parentUserInfo = _parameters.ManageParentService.GetParentUserInfo(userId);
                if (parentUserInfo != null)
                {
                    selectedDistrictID = parentUserInfo.DistrictId;
                }
                else
                {
                    selectedDistrictID = CurrentUser.DistrictId.GetValueOrDefault();
                }
            }
            else
            {
                selectedDistrictID = CurrentUser.DistrictId.GetValueOrDefault();
            }

            //get setup hide or show "Add New Student" in configuration table

            IEnumerable<int> studentIdList = _parameters.ManageParentService.GetChildrenStudentIdList(parentId);

            var programs = _parameters
                .StudentService
                .GetProgramsStudent(selectedDistrictID, CurrentUser.Id, CurrentUser.RoleId)
                .ToList();

            var programItems = programs.Where(x => !studentIdList.Contains(x.StudentID)).Select(x => new ListItem() { Id = x.ProgramID, Name = x.ProgramName }).DistinctBy(x => x.Id).OrderBy(x => x.Name).ToList();

            var grades = _parameters.StudentService.GetGradesStudent(selectedDistrictID, CurrentUser.Id, CurrentUser.RoleId).ToList();
            var gradeItems = grades.Where(x => !studentIdList.Contains(x.StudentID))
                .DistinctBy(x => x.GradeID)
                .OrderBy(x => x.Order)
                .Select(x => new ListItem() { Id = x.GradeID, Name = x.GradeName })
            .ToList();

            var isShowAddNewStudent = Convert.ToBoolean(_parameters.ConfigurationService.GetConfigurationByKeyWithDefaultValue(Util.IsShow_AddNewStudentButton, "true")) && CurrentUser.IsAdmin();
            var model = new AssignStudentToParentFilterModelDto() { Programs = programItems, Grades = gradeItems, IsShowAddNewStudent = isShowAddNewStudent };
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListAvailableStudent(int parentUserId, int districtId, string programIdList, string gradeIdList, bool showInactive, string temporaryAddedStudentIds = "")
        {
            var parser = new DataTableParser<AvailableStudentViewModel>();
            int currentParentId = 0;
            var parent = _parameters.ParentService.GetParentViaUserId(parentUserId);
            if(parent != null)
            {
                currentParentId = parent.ParentID;
            }

            var studentIdList = _parameters.ManageParentService.GetChildrenStudentIdList(currentParentId);
            if (!string.IsNullOrEmpty(temporaryAddedStudentIds))
            {
                studentIdList = (studentIdList ?? new int[0]).Concat(temporaryAddedStudentIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(c => int.Parse(c))).ToArray();
            }
            if (!string.IsNullOrEmpty(programIdList))
                programIdList = string.Format(",{0},", programIdList);
            if (!string.IsNullOrEmpty(gradeIdList))
                gradeIdList = string.Format(",{0},", gradeIdList);

            var students = _parameters.StudentService.ManageParentGetStudentsAvailableByFilter(districtId, programIdList, gradeIdList,
                showInactive, CurrentUser.Id, CurrentUser.RoleId);
            if (studentIdList.Any())
            {
                students = students.Where(x => !studentIdList.Contains(x.StudentId)).ToList();
            }
            var studentList = students.Select(x => new AvailableStudentViewModel
            {
                StudentId = x.StudentId,
                FullName = $"{x.LastName}, {x.FirstName}",
                Code = x.Code,
                Gender = x.Gender,
                Grade = x.Grade
            });
            
            return Json(parser.Parse(studentList.AsQueryable(), true), JsonRequestBehavior.AllowGet);
        }

        [AjaxAwareAuthorize]
        public ActionResult GenRCode(string idList)
        {
            if (!string.IsNullOrEmpty(idList))
            {
                var registrationCodeLength = _parameters.StudentService.GetStudentRegistrationCodeLength();

                var parentIds = idList.Split(',').Select(s => Int32.TryParse(s, out int n) ? n : (int?)null)
                                .Where(n => n.HasValue && n.Value > 0)
                                .Select(n => n.Value)
                                .ToList();
                _parameters.ManageParentService.GenerateRegistrationCode(parentIds, CurrentUser.Id, registrationCodeLength);

            }
            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.ManageParent)]
        [AjaxAwareAuthorize]
        public ActionResult AddStudentsToParent(AddStudentToParentRequestModel requestModel)
        {
            BaseResponseModel<bool> unassignResult = _parameters.ManageParentService.AddStudentsToParent(requestModel.StudentIds, requestModel.ParentUserId, requestModel.Relationship, requestModel.StudentDataAccess);
            return Json(unassignResult, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.ManageParent)]
        [AjaxAwareAuthorize]
        public ActionResult UnassignStudent(UnassignStudentRequestModel requestModel)
        {
            BaseResponseModel<bool> unassignResult = _parameters.ManageParentService.UnassignStudent(requestModel.ParentUserId, requestModel.StudentId);
            return Json(unassignResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DistributeByEmail(FilterParentRequestModel criteria)
        {
            RestrictInvalidParameters(criteria);
            var parentList = _parameters.ManageParentService.GetParentListForDistributing(criteria);
            parentList = RemoveUnselectedParents(criteria, parentList).Cast<ParentListForDistributingDto>();

            var parentUsersInfo = parentList
                .Where(c => !c.TheUserLogedIn && (c.RegistrationCode ?? "").Length > 0)
                .Select(c => new { c.UserId, c.RegistrationCode }).ToArray();
            if (parentUsersInfo.Length <= 0)
            {
                return Json(BaseResponseModel<int>.InstanceSuccess(0), JsonRequestBehavior.AllowGet);
            }

            ParentStudentEmailModel objParentStudentEmail = new ParentStudentEmailModel()
            {
                HTTPProtocal = HelperExtensions.GetHTTPProtocal(Request),
                ListUserRegistrationCode = new List<UserRegistrationCodeDto>()
            };

            objParentStudentEmail.ListUserRegistrationCode = parentUsersInfo
                .Select(c => new UserRegistrationCodeDto()
                {
                    RegistrationCode = c.RegistrationCode,
                    UserId = c.UserId
                }).ToList();
            var emailCredentialSetting = LinkitConfigurationManager.GetEmailCredentialSetting(EmailSetting.RegistrationUseEmailCredentialKey);

            foreach (var userId in _parameters.EmailService.SendMailWhenRegistrationCode(objParentStudentEmail, emailCredentialSetting))
            {
                _parameters.ManageParentService.TrackingLastSendDistributeEmail(userId);
            }

            return Json(BaseResponseModel<int>.InstanceSuccess(objParentStudentEmail.ListUserRegistrationCode.Count), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Generate(FilterParentRequestModel model)
        {
            string url = string.Empty;
            var pdf = Print(model, out url);

            var folder = LinkitConfigurationManager.GetS3Settings().ReportPrintingFolder;
            var bucketName = LinkitConfigurationManager.GetS3Settings().ReportPrintingBucketName;

            _s3Service.UploadRubricFile(bucketName, folder + "/" + model.ParentDetailPrintingFileName, new MemoryStream(pdf));

            var s3Url = _s3Service.GetPublicUrl(bucketName, folder + "/" + model.ParentDetailPrintingFileName);
            return Json(new { IsSuccess = true, Url = s3Url });
        }
        private byte[] Print(FilterParentRequestModel model, out string url)
        {
            if (!CurrentUser.IsPublisher && !CurrentUser.IsNetworkAdmin())
            {
                model.DistrictId = CurrentUser.DistrictId.Value;
            }

            model.UserId = CurrentUser.Id;
            model.RoleId = CurrentUser.RoleId;

            url = Url.Action("ReportPrinting", "ManageParent", model, HelperExtensions.GetHTTPProtocal(Request));
            var pdf = ExportToPDF(url, model.TimezoneOffset);
            return pdf;
        }

        private byte[] ExportToPDF(string url, int timezoneOffset)
        {
            DateTime dt = DateTime.UtcNow.AddMinutes(timezoneOffset * (-1));

            string footerUrl = Url.Action("RenderFooter", "ManageParent", null, HelperExtensions.GetHTTPProtocal(Request));
            string headerUrl = Url.Action("RenderHeader", "ManageParent", new
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

        [HttpGet]
        public ActionResult ReportPrinting(FilterParentRequestModel criteria)
        {
            var parentList = GetParentListByFilterCriteria(criteria);
            return View(parentList);

        }
        private IEnumerable<string> GetParentListByFilterCriteria(FilterParentRequestModel criteria)
        {
            var body = new List<string>();

            RestrictInvalidParameters(criteria);
            var parentList = _parameters.ManageParentService.GetParentListForDistributing(criteria);
            parentList = RemoveUnselectedParents(criteria, parentList).Cast<ParentListForDistributingDto>();

            var listUserRegistrationCode = parentList
                .Where(c => !c.TheUserLogedIn && !string.IsNullOrEmpty(c.RegistrationCode))
                .Select(c => new UserRegistrationCodeDto() { UserId = c.UserId, RegistrationCode = c.RegistrationCode }).ToArray();

            var template = _districtDecodeService.GetDistrictDecodeOfDistrictOrConfigurationByLabel(criteria.DistrictId ?? 0, Constanst.TemplateStudentParentRegistrationCode);
            if (template == null)
                return body;

            foreach (var userRegistration in listUserRegistrationCode)
            {
                var macros = _parameters.ManageParentService.GetParentsInformationForDistributeRegistrationCode(userRegistration.UserId);
                string strParentLoginUrl = string.Format("{0}://{1}.{2}/Parent", HelperExtensions.GetHTTPProtocal(Request), macros.LICode.ToLower(), ConfigurationManager.AppSettings["LinkItUrl"]);

                if (macros != null)
                {
                    var keyPairValues = new Dictionary<string, string>()
                        {
                            { "[StudentFirstName]",macros.StudentFirstName },
                            { "[DistrictName]",macros.DistrictName},
                            { "[ParentPortalURL]",strParentLoginUrl },
                            { "[RegistrationCode]",macros.RegistrationCode },
                            { "[StudentCurrentSchoolName]",macros.StudentCurrentSchoolName },
                            { "[UserFirstName]",macros.UserFirstName },
                            { "[UserLastName]",macros.UserLastName },
                        };
                    var strBody = template.Value;
                    foreach (var keyPairValue in keyPairValues)
                    {
                        strBody = strBody.Replace(keyPairValue.Key, keyPairValue.Value);
                    }
                    body.Add(strBody);
                }
            }

            return body;
        }

        private static IEnumerable<ParentGridViewModel> RemoveUnselectedParents(FilterParentRequestModel criteria, IEnumerable<ParentGridViewModel> parentList)
        {
            var filteredParentUserIds = (criteria.SelectedUserIds ?? "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(c =>
                            {
                                int.TryParse(c, out int _userid);
                                return _userid;
                            }).Where(c => c > 0).ToArray();

            if (filteredParentUserIds?.Length > 0)
            {
                parentList = parentList.Where(c => filteredParentUserIds.Contains(c.UserId)).ToList();
            }
            else
            {
                parentList = new List<ParentGridViewModel>();
            }

            return parentList;
        }

        private void RestrictInvalidParameters(FilterParentRequestModel criteria)
        {
            if (!CurrentUser.IsPublisher && !CurrentUser.IsNetworkAdmin() && (!criteria.DistrictId.HasValue || criteria.DistrictId == 0))
                criteria.DistrictId = CurrentUser.DistrictId.Value;

            if (criteria.DistrictId == -1 && criteria.SchoolId.HasValue && criteria.SchoolId.Value > 0)
            {
                criteria.DistrictId = CurrentUser.DistrictId.Value;
            }
            criteria.iDisplayStart = 0;
            criteria.iDisplayLength = 1000000;
            criteria.UserId = CurrentUser.Id;
            criteria.RoleId = CurrentUser.RoleId;
        }

        public ActionResult GoogleCallback(string code, string state)
        {
            return RedirectToAction("GoogleCallback", "Account", new { code = code, state = state, loginType = TextConstants.LOGIN_ROLE_PARENT });
        }

        public ActionResult MicrosoftCallback(string code, string state)
        {
            return RedirectToAction("MicrosoftCallback", "Account", new { code = code, state = state, loginType = TextConstants.LOGIN_ROLE_PARENT });
        }

        public ActionResult GetParentMetaData(int parentId, int districtId)
        {
            var parentMetaDatas = GetParentMetaDataByParentId(parentId, districtId);
            if(parentMetaDatas == null)
            {
                return Json(new { data = parentMetaDatas.AsQueryable() }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { message = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        private List<MetaDataKeyValueDto> GetParentMetaDataByParentId(int parentId, int districtId)
        {
            try
            {
                var parentMetaDataLabels = _parameters.DistrictDecodeService.GetParentMetaLabel(districtId);
                var parentMetaValues = _parameters.ParentService.GetParentMetasByParentId(parentId);
                if (parentMetaValues != null && parentMetaValues.Count > 0)
                {
                    foreach (var parentMetaDataLabel in parentMetaDataLabels)
                    {
                        var parentMetaValue = parentMetaValues.FirstOrDefault(x => x.Name.Equals(parentMetaDataLabel.Name, StringComparison.OrdinalIgnoreCase));
                        if (parentMetaValue != null)
                        {
                            parentMetaDataLabel.Value = parentMetaValue.Data;
                        }
                    }
                }
                return parentMetaDataLabels;                
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return null;                
            }
        }
    }
}
