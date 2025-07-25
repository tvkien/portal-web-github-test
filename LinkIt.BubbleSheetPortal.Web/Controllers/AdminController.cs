using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using AutoMapper;
using Envoc.Core.Shared.Extensions;
using FluentValidation.Results;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DistrictReferenceData;
using LinkIt.BubbleSheetPortal.Models.ETL;
using LinkIt.BubbleSheetPortal.Models.Requests;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Models;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using System.IO;
using LinkIt.BubbleSheetPortal.Web.Helpers.ETL;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using LinkIt.BubbleSheetPortal.Web.ViewModels.DistrictReferenceData;
using LinkIt.BubbleSheetPortal.Web.ViewModels.Validators;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using LinkIt.BubbleSheetPortal.Services.BusinessObjects;
using LinkIt.BubbleSheetPortal.Web.Models.DataTable;
using LinkIt.BubbleSheetPortal.Web.ViewModels.UserGroup;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Models.Constants;
using Newtonsoft.Json;
using LinkIt.BubbleSheetPortal.Web.Constant;
using LinkIt.BubbleSheetPortal.Models.Enum;
using LinkIt.BubbleSheetPortal.Models.Old.XpsQueue;
using LinkIt.BubbleSheetPortal.Common.ZipHelper.Models;
using LinkIt.BubbleSheetPortal.Common.ZipHelper;
using LinkIt.BubbleSheetPortal.Models.Old.XpsDistrictUpload;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.Required)]
    [AjaxAwareAuthorize(Order = 2)]
    [VersionFilter]
    public class AdminController : BaseController
    {
        private readonly AdminControllerParameters parameters;
        private readonly CategoriesService _categoriesService;
        private readonly EmailService _emailService;
        private readonly DistrictDecodeService _districtDecodeService;
        private readonly DistrictService _districtService;
        private readonly RosterValidationService _rosterValidationService;
        public string TempFolder
        {
            get
            {
                return HttpContext.Server?.MapPath("~/Content/Upload/UploadRoster/Temp") ?? "/";
            }
        }
        public AdminController(AdminControllerParameters parameters, CategoriesService categoriesService, EmailService emailService
            , DistrictDecodeService districtDecodeService, DistrictService districtService, RosterValidationService rosterValidationService)
        {
            this.parameters = parameters;
            _categoriesService = categoriesService;
            _emailService = emailService;
            _districtDecodeService = districtDecodeService;
            _districtService = districtService;
            _rosterValidationService = rosterValidationService;
        }

        [HttpGet, AdminOnly(Order = 3)]
        public ActionResult DataAdmin()
        {
            return View();
        }

        [HttpGet, AdminOnly(Order = 3)]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.DadManageuser)]
        public ActionResult ManageUsers()
        {
            var model = new ManageUser
            {
                CurrentUserId = CurrentUser.Id,
                RoleId = CurrentUser.RoleId,
                DistrictId = CurrentUser.DistrictId.GetValueOrDefault(),
                SchoolId = CurrentUser.SchoolId.GetValueOrDefault()
            };
            return View(model);
        }

        [HttpGet, AdminOnly(Order = 3)]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.DadManageParents)]
        public ActionResult ManageParents()
        {
            if (CurrentUser.IsDistrictAdminOrPublisher)
            {
                ViewBag.IsPublisher = CurrentUser.IsPublisher ? "true" : "false";
                ViewBag.DistrictID = CurrentUser.DistrictId ?? 0;
                return View();
            }
            return HttpNotFound();
        }

        [HttpGet, AdminOnly(Order = 3), AjaxOnly]
        public ActionResult GetUsers(bool inactive, int? districtId, int? schoolId, string staffName)
        {
            IQueryable<UserManage> userManages = new List<UserManage>().AsQueryable();

            if (districtId.HasValue && districtId.Value > 0 && Util.HasRightOnDistrict(CurrentUser, districtId.Value))
            {
                staffName = staffName.Replace(',', ' ');
                userManages = parameters.UserSchoolService.GetManageUserByRole(CurrentUser.Id,
                        districtId.Value, CurrentUser.RoleId, schoolId.GetValueOrDefault(), staffName, inactive)
                    .Where(c => c.RoleId != 26)
                    .ToArray()
                    .AsQueryable();
                var focusGroupConfig = parameters.AutoFocusGroupConfigService.GetConfigByDistrictID(districtId.Value);
                if (focusGroupConfig != null)
                {
                    var jsonConfig = JsonConvert.DeserializeObject<AutoFocusGroupJsonConfig>(focusGroupConfig.JSONConfig);
                    if (!String.IsNullOrEmpty(jsonConfig.PrimaryTeacher))
                    {
                        userManages = userManages.Where(x => x.UserId != int.Parse(jsonConfig.PrimaryTeacher));
                    }
                }
            }
            var data = BindUserManageToManageUsersViewModel(userManages);
            var parser = new DataTableParser<ManageUsersViewModel>();
            return Json(parser.Parse2018(data), JsonRequestBehavior.AllowGet);
        }

        [HttpGet, AdminOnly(Order = 3), AjaxOnly]
        public ActionResult GetStates()
        {
            var data = parameters.StateService.GetStates().Select(x => new ListItem { Name = x.Name, Id = x.Id }).ToList();
            if (CurrentUser.IsNetworkAdmin)
            {
                List<int> stateIdList = parameters.DistrictService.GetStateIdByDictricIds(CurrentUser.GetMemberListDistrictId());
                data = parameters.StateService.GetStates().Where(x => stateIdList.Contains(x.Id)).Select(x => new ListItem { Name = x.Name, Id = x.Id }).ToList();
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, AjaxOnly]
        public ActionResult GetStatesImpersonate()
        {
            var data = parameters.StateService.GetStates().Select(x => new ListItem { Name = x.Name, Id = x.Id }).ToList();

            if (CurrentUser.RoleId != (int)Permissions.Publisher)
            {
                if (CurrentUser.RoleId == (int)Permissions.NetworkAdmin)
                {
                    var dspDistricts = parameters.DspDistrictService.GetDistrictsByUserId(CurrentUser.Id);
                    List<int> stateIdList = parameters.DistrictService.GetStateIdByDictricIds(dspDistricts);
                    data = data.Where(x => stateIdList.Contains(x.Id)).ToList();
                }
                else
                {
                    data = data.Where(x => x.Id == CurrentUser.StateId).ToList();
                }
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, AdminOnly(Order = 3), AjaxOnly]
        public ActionResult GetDistricts(int? stateId)
        {
            if (stateId.HasValue && stateId.Value > 0 && !parameters.VulnerabilityService.HasRightOnState(CurrentUser, CurrentUser.GetMemberListDistrictId(),
                    stateId.Value))
            {
                return Json(new { error = "Has no right on state" }, JsonRequestBehavior.AllowGet);
            }
            var districts = stateId.HasValue
                                ? parameters.DistrictService.GetDistrictsByStateId(stateId.Value).ToList()
                                : parameters.DistrictService.GetDistrictsUserHasAccessTo(
                                    CurrentUser.DistrictId.GetValueOrDefault()).ToList();
            if (CurrentUser.IsNetworkAdmin)
            {
                districts = districts.Where(x => CurrentUser.GetMemberListDistrictId().Contains(x.Id)).ToList();
            }
            IOrderedQueryable<ListItem> data = districts.AsQueryable().Select(x => new ListItem { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, AjaxOnly]
        public ActionResult GetDistrictsImpersonate(int? stateId)
        {
            var districts = stateId.HasValue
                                ? parameters.DistrictService.GetDistrictsByStateId(stateId.Value).ToList()
                                : parameters.DistrictService.GetDistrictsUserHasAccessTo(
                                    CurrentUser.DistrictId.GetValueOrDefault()).ToList();

            if (CurrentUser.RoleId != (int)Permissions.Publisher)
            {
                if (CurrentUser.RoleId == (int)Permissions.NetworkAdmin)
                {
                    var dspDistricts = parameters.DspDistrictService.GetDistrictsByUserId(CurrentUser.Id);
                    districts = districts.Where(x => dspDistricts.Contains(x.Id)).ToList();
                }
                else
                {
                    districts = districts.Where(x => x.Id == CurrentUser.DistrictId).ToList();
                }
            }

            IOrderedQueryable<ListItem> data = districts.AsQueryable().Select(x => new ListItem { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, AdminOnly(Order = 3)]
        public ActionResult GetSchools(int? districtId)
        {
            if (districtId.HasValue && !Util.HasRightOnDistrict(CurrentUser, districtId.Value))
            {
                return Json(new { error = "Has no right on the district" }, JsonRequestBehavior.AllowGet);
            }

            var data = _categoriesService.GetSchoolByDistrictId(CurrentUser.Id, CurrentUser.RoleId, districtId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSchoolsByDistrictIdAndUserId(int districtId, int userId)
        {
            if (districtId > 0 && !Util.HasRightOnDistrict(CurrentUser, districtId))
            {
                return Json(new { error = "Has no right on the district" }, JsonRequestBehavior.AllowGet);
            }

            var data = _categoriesService.GetSchoolByDistrictId(CurrentUser.Id, CurrentUser.RoleId, districtId);
            //Excluse SchoolIds belong this User
            var userSchoolIds = parameters.UserSchoolService.GetListSchoolIdByUserId(userId);
            var listSchools = new List<BubbleSheetPortal.Models.DTOs.SelectListItemDTO>();
            foreach (var item in data)
            {
                if (userSchoolIds.Any(o => o == item.Id) == false)
                {
                    listSchools.Add(item);
                }
            }
            return Json(listSchools.AsQueryable(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet, AdminOnly(Order = 3)]
        public ActionResult GetSchoolsForDataAdmin()
        {
            var schools = GetSchoolsBasedOnPermissions().Select(x => new SchoolListViewModel
            {
                SchoolID = x.Id,
                SchoolName = x.Name
            }).AsQueryable();

            var parser = new DataTableParser<SchoolListViewModel>();
            return Json(parser.Parse(schools), JsonRequestBehavior.AllowGet);
        }

        [HttpGet, AdminOnly(Order = 3)]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.DadManageuser)]
        public ActionResult CreateUser()
        {
            var model = InitializeNewCreateUserViewModel();
            return View(model);
        }

        [HttpPost, AdminOnly(Order = 3)]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUser(CreateUserViewModel model)
        {
            model.CurrentUserRoleId = CurrentUser.RoleId;
            model.DistrictId = model.DistrictId ?? CurrentUser.DistrictId;
            model.SetValidator(parameters.CreateUserViewModelValidator);

            if (!model.IsValid)
            {
                return Json(new { Success = false, ErrorList = model.ValidationErrors });
            }

            var isSkipsRuleCheckLeadingZeros =
                parameters.DistrictDecodeService.IsPortalStaffSkipsRuleCheckLeadingZerosInUserCode(model.DistrictId.Value);

            if (!isSkipsRuleCheckLeadingZeros)
            {
                // Check Code start with rezo
                if (parameters.UserService.CheckUserCodeExistsStartWithRezoByDistrictID(model.DistrictId.GetValueOrDefault(), model.LocalCode.TrimStart('0'), 0))
                {
                    return Json(new { Success = false, ErrorList = new List<ValidationFailure>(1) { new ValidationFailure("customError", "USER CODE ALREADY EXISTS") } });
                }
            }

            //avoid ajax modifying parameter
            //check right on district
            if (model.DistrictId.HasValue && model.DistrictId.Value > 0 && !Util.HasRightOnDistrict(CurrentUser, model.DistrictId.GetValueOrDefault()))
            {
                return Json(new { Success = false, ErrorList = new List<ValidationFailure>(1) { new ValidationFailure("customError", "Has no right on this " + LabelHelper.DistrictLabel + ".") } });
            }
            //check role
            if (!CurrentUser.IsDistrictAdminOrPublisher && !CurrentUser.IsSchoolAdmin)
            {
                return Json(new { Success = false, ErrorList = new List<ValidationFailure>(1) { new ValidationFailure("customError", "Has no right to create user.") } });
            }
            if (model.RoleId == (int)Permissions.Publisher && !CurrentUser.IsPublisher)
            {
                return Json(new { Success = false, ErrorList = new List<ValidationFailure>(1) { new ValidationFailure("customError", "Has no right to create a Publisher.") } });
            }

            if (model.RoleId == (int)Permissions.NetworkAdmin && !CurrentUser.IsPublisher && !CurrentUser.IsNetworkAdmin)
            {
                return Json(new { Success = false, ErrorList = new List<ValidationFailure>(1) { new ValidationFailure("customError", "Has no right to create a Network Admin.") } });
            }

            if (model.RoleId == (int)Permissions.DistrictAdmin && !CurrentUser.IsDistrictAdminOrPublisher)
            {
                return Json(new { Success = false, ErrorList = new List<ValidationFailure>(1) { new ValidationFailure("customError", "Has no right to create a " + LabelHelper.DistrictLabel + " Admin.") } });
            }

            if (model.RoleId == (int)Permissions.Teacher && !CurrentUser.IsDistrictAdminOrPublisher && !CurrentUser.IsSchoolAdmin)
            {
                return Json(new { Success = false, ErrorList = new List<ValidationFailure>(1) { new ValidationFailure("customError", "Has no right to create a School Admin.") } });
            }

            if (RequiresUserSchool(model) && model.SchoolId > 0)
            {
                //check right to access the school
                var districtId = CurrentUser.DistrictId;
                if (model.CanSelectDistrict)
                {
                    districtId = model.DistrictId.Value;
                }
                if (districtId > 0)
                {
                    var authorizedSchoolIdList = _categoriesService.GetSchoolByDistrictId(CurrentUser.Id, CurrentUser.RoleId, districtId).Select(x => x.Id).ToList();
                    if (!authorizedSchoolIdList.Contains(model.SchoolId))
                    {
                        return Json(new { Success = false, ErrorList = new List<ValidationFailure>(1) { new ValidationFailure("customError", "Has no right on the school.") } });
                    }
                }
            }

            string messageInvalid = ValidateDataModel(model.DistrictId ?? 0, model.LocalCode, model.UserName);
            if (!string.IsNullOrEmpty(messageInvalid))
                return Json(new { Success = false, ErrorList = new List<ValidationFailure>() { new ValidationFailure("error", messageInvalid) } });

            try
            {
                CreateNewUser(model);
                return Json(new { Success = true });
            }
            catch (Exception e)
            {
                return ShowJsonResultException(model, e.Message);
            }
        }

        private string ValidateDataModel(int districtId, string localCode, string userName)
        {
            if (CurrentUser.RoleId != (int)Permissions.Publisher && CurrentUser.RoleId != (int)Permissions.NetworkAdmin)
            {
                districtId = CurrentUser.DistrictId ?? 0;
            }
            return parameters.DistrictDecodeService.GetDistrictDecodeValidations(districtId, GetValidateFields(localCode, userName));
        }

        public Dictionary<string, string> GetValidateFields(string localCode, string userName)
        {
            var dataFields = new Dictionary<string, string>();
            dataFields.Add(Constanst.USERNAME_VALIDATION, userName);
            dataFields.Add(Constanst.USER_CODE_VALIDATION, localCode);

            return dataFields;
        }

        private bool CurrentUserHasAccessToNewUser(CreateUserViewModel model)
        {
            return model.AvailableRoles.Any(x => x.Value == model.RoleId.ToString());
        }

        [HttpGet, AdminOnly(Order = 3)]
        public ActionResult EditUser(int id)
        {
            var user = parameters.UserService.GetUserById(id);
            if (user == null)
            {
                return RedirectToAction("ManageUsers");
            }
            //Check if user has right on this user
            if (!CurrentUser.IsPublisher)
            {
                var hasRight = parameters.VulnerabilityService.HasRightToUpdateUser(CurrentUser, user.Id, user.DistrictId.GetValueOrDefault());
                if (!hasRight)
                {
                    return RedirectToAction("ManageUsers");
                }
            }

            var model = new EditUserViewModel { CurrentUserRoleId = CurrentUser.RoleId };
            if (IsNotNullAndCanAccess(user))
            {
                Mapper.Map(user, model);
            }
            else
            {
                return RedirectToAction("ManageUsers");
            }
            return View(model);
        }

        [HttpPost, AdminOnly(Order = 3)]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(EditUserViewModel model)
        {
            model.CurrentUserRoleId = CurrentUser.RoleId;
            var user = parameters.UserService.GetUserById(model.UserId);
            if (!IsNotNullAndCanAccess(user))
            {
                ModelState.AddModelError("UserName", "User does not exist, please try again.");
                return Json(new { Success = false, ErrorList = model.ValidationErrors });
            }

            model.StateId = user.StateId.GetValueOrDefault();
            model.DistrictId = model.DistrictId ?? user.DistrictId.GetValueOrDefault();
            model.SetValidator(parameters.EditUserViewModelValidator);
            if (!IsValid(model))
            {
                return Json(new { Success = false, ErrorList = model.ValidationErrors });
            }

            var isSkipsRuleCheckLeadingZeros =
                parameters.DistrictDecodeService.IsPortalStaffSkipsRuleCheckLeadingZerosInUserCode(model.DistrictId.Value);

            if (!isSkipsRuleCheckLeadingZeros)
            {
                //Check Code start with rezo
                if (parameters.UserService.CheckUserCodeExistsStartWithRezoByDistrictID(model.DistrictId.GetValueOrDefault(), model.LocalCode.TrimStart('0'), model.UserId))
                {
                    return Json(new { Success = false, ErrorList = new List<ValidationFailure>(1) { new ValidationFailure("customError", "USER CODE ALREADY EXISTS") } });
                }
            }

            //avoid modify ajax parameters
            if (model.DistrictId.HasValue && model.DistrictId.Value > 0)
            {
                if (!Util.HasRightOnDistrict(CurrentUser, model.DistrictId.GetValueOrDefault()))
                {
                    return Json(new { Success = false, ErrorList = new List<ValidationFailure>(1) { new ValidationFailure("customError", "Has no right on this " + LabelHelper.DistrictLabel + ".") } });
                }
                if (user.DistrictId.HasValue && user.DistrictId.Value != model.DistrictId && user.RoleId == (int)Permissions.Teacher)
                {
                    return Json(new { Success = false, ErrorList = new List<ValidationFailure>(1) { new ValidationFailure("customError", "Has no right on this " + LabelHelper.DistrictLabel + ".") } });
                }
            }
            if (!Util.HasRightOnDistrict(CurrentUser, user.DistrictId.GetValueOrDefault()))
            {
                return Json(new { Success = false, ErrorList = new List<ValidationFailure>(1) { new ValidationFailure("customError", "Has no right on this " + LabelHelper.DistrictLabel + ".") } });
            }
            //Check if user has right on this user
            if (!CurrentUser.IsPublisher)
            {
                var hasRight = parameters.VulnerabilityService.HasRightToUpdateUser(CurrentUser, user.Id, model.DistrictId.GetValueOrDefault());
                if (!hasRight)
                {
                    return Json(new { Success = false, ErrorList = new List<ValidationFailure>(1) { new ValidationFailure("customError", "Has no right on this user.") } });
                }
            }
            //Check new role
            if (!CurrentUser.IsDistrictAdminOrPublisher && !CurrentUser.IsSchoolAdmin)
            {
                return Json(new { Success = false, ErrorList = new List<ValidationFailure>(1) { new ValidationFailure("customError", "Has no right to update user.") } });
            }
            if (model.RoleId == (int)Permissions.Publisher && !CurrentUser.IsPublisher)
            {
                return Json(new { Success = false, ErrorList = new List<ValidationFailure>(1) { new ValidationFailure("customError", "Has no right to update a Publisher.") } });
            }
            if (model.RoleId == (int)Permissions.NetworkAdmin && !CurrentUser.IsPublisher && !CurrentUser.IsNetworkAdmin)
            {
                return Json(new { Success = false, ErrorList = new List<ValidationFailure>(1) { new ValidationFailure("customError", "Has no right to update a Network Admin.") } });
            }

            if (model.RoleId == (int)Permissions.DistrictAdmin && !CurrentUser.IsDistrictAdminOrPublisher)
            {
                return Json(new { Success = false, ErrorList = new List<ValidationFailure>(1) { new ValidationFailure("customError", "Has no right to update a " + LabelHelper.DistrictLabel + " Admin.") } });
            }

            if (model.RoleId == (int)Permissions.SchoolAdmin && !CurrentUser.IsDistrictAdminOrPublisher && !CurrentUser.IsSchoolAdmin)
            {
                return Json(new { Success = false, ErrorList = new List<ValidationFailure>(1) { new ValidationFailure("customError", "Has no right to update a School Admin.") } });
            }

            string messageInvalid = ValidateDataModel(model.DistrictId ?? 0, model.LocalCode, model.UserName);
            if (!string.IsNullOrEmpty(messageInvalid))
                return Json(new { Success = false, ErrorList = new List<ValidationFailure>() { new ValidationFailure("error", messageInvalid) } });

            Mapper.Map(model, user);
            user.ModifiedDate = DateTime.UtcNow;
            if (string.IsNullOrEmpty(user.ModifiedBy))
                user.ModifiedBy = "Portal";
            if (!user.ModifiedUser.HasValue)
                user.ModifiedUser = CurrentUser.Id;

            parameters.UserService.SaveUser(user);
            return Json(new { Success = true });
        }

        [HttpPost, AdminOnly(Order = 3)]
        public ActionResult ChangeUserStatus(int userId, int operation)
        {
            var user = parameters.UserService.GetUserById(userId);
            if (!IsNotNullAndCanAccess(user))
            {
                return Json(false);
            }
            //avoid modify ajax request
            if (!Util.HasRightOnDistrict(CurrentUser, user.DistrictId.GetValueOrDefault()))
            {
                return Json(new { Success = false, Error = "Has no right on this " + LabelHelper.DistrictLabel + "." });
            }
            //Check if user has right on this user
            if (!CurrentUser.IsPublisher)
            {
                var hasRight = parameters.VulnerabilityService.HasRightToUpdateUser(CurrentUser, user.Id, user.DistrictId.GetValueOrDefault());
                if (!hasRight)
                {
                    return Json(new { Success = false, Error = "Has no right on this user." });
                }
            }
            if (operation == 0)
            {
                user.UserStatusId = (int)UserStatus.Active;
                user.DateConfirmedActive = DateTime.Today;
            }
            else if (operation == 1)
            {
                user.UserStatusId = (int)UserStatus.Inactive;
            }

            user.ModifiedDate = DateTime.UtcNow;
            parameters.UserService.SaveUser(user);
            return Json(true);
        }

        [HttpPost, AdminOnly(Order = 3)]
        public ActionResult GetSchoolsForUser(int userId)
        {
            var user = parameters.UserService.GetUserById(userId);
            if (!IsNotNullAndCanAccess(user))
            {
                return Json(false);
            }

            var schools = BindSchoolsForUser(user);
            return PartialView("_UsersSchools", schools);
        }

        private List<UserSchool> BindSchoolsForUser(User user)
        {
            var schools = parameters.UserSchoolService.GetSchoolsUserHasAccessTo(user.Id).ToList();
            foreach (var userSchool in schools)
            {
                var district = parameters.DistrictService.GetDistrictById(user.DistrictId.GetValueOrDefault());
                var state = parameters.StateService.GetStateById(user.StateId);
                userSchool.DistrictName = district.Name;
                userSchool.StateName = state.Name;
            }
            return schools.OrderBy(o => o.SchoolName).ToList();
        }

        [HttpPost, AdminOnly(Order = 3)]
        public ActionResult RemoveUserFromSchool(int userSchoolId, int userId, int schoolId)
        {
            if (schoolId > 0 && !parameters.VulnerabilityService.CheckUserCanAccessSchool(CurrentUser, schoolId))
            {
                return Json(new { error = "Has no right on school" }, JsonRequestBehavior.AllowGet);
            }

            if (userId > 0)
            {
                var user = parameters.UserService.GetUserById(userId);

                if (!IsNotNullAndCanAccess(user))
                {
                    return Json(false);
                }

                if (!parameters.VulnerabilityService.HasRightToUpdateUser(CurrentUser, userId, user.DistrictId.GetValueOrDefault()))
                {
                    return Json(new { error = "Has no right on user" }, JsonRequestBehavior.AllowGet);
                }
            }

            var userSchool = parameters.UserSchoolService.VerifyUserSchoolExists(userSchoolId, userId, schoolId);
            if (userSchool.IsNull())
            {
                return Json(false);
            }

            parameters.UserSchoolService.RemoveUserSchool(userSchool);
            return Json(true);
        }

        [HttpGet, AdminOnly(Order = 3)]
        public ActionResult AddUserSchool(int userId)
        {
            var user = parameters.UserService.GetUserById(userId);
            if (!IsNotNullAndCanAccess(user))
            {
                ModelState.AddModelError("UserName", "User does not exist, please try again.");
                return PartialView("_AddUserSchool", new AddUserSchoolViewModel { CurrentUserRoleId = CurrentUser.RoleId });
            }
            var model = new AddUserSchoolViewModel
            {
                CurrentUserRoleId = CurrentUser.RoleId,
                UserId = userId,
                RoleId = user.RoleId,
                DistrictId = user.DistrictId
            };

            return PartialView("_AddUserSchool", model);
        }

        [HttpPost, AdminOnly(Order = 3)]
        public ActionResult AddUserSchool(AddUserSchoolViewModel viewModel)
        {
            viewModel.CurrentUserRoleId = CurrentUser.RoleId;
            if (!IsValid(viewModel))
            {
                return Json(new { success = false, ErrorList = viewModel.ValidationErrors });
            }

            var user = parameters.UserService.GetUserById(viewModel.UserId);
            if (!IsNotNullAndCanAccess(user))
            {
                ModelState.AddModelError("UserName", "User does not exist, please try again.");
                return Json(new { success = false, ErrorList = viewModel.ValidationErrors });
            }
            if (viewModel.SchoolId > 0)
            {
                if (!parameters.VulnerabilityService.CheckUserCanAccessSchool(CurrentUser, viewModel.SchoolId))
                {
                    return Json(new { error = "Has no right on school" }, JsonRequestBehavior.AllowGet);
                }
                //not allow to add user to a school of another district
                var authorizedSchoolIdList = _categoriesService.GetSchoolByDistrictId(CurrentUser.Id, CurrentUser.RoleId, user.DistrictId).Select(x => x.Id).ToList();
                if (!authorizedSchoolIdList.Contains(viewModel.SchoolId))
                {
                    return Json(new { error = "User and school must belong to the same district" }, JsonRequestBehavior.AllowGet);
                }
            }
            if (user.Id > 0 && !parameters.VulnerabilityService.HasRightToUpdateUser(CurrentUser, user.Id, user.DistrictId.GetValueOrDefault()))
            {
                return Json(new { error = "Has no right on user", JsonRequestBehavior.AllowGet });
            }

            AssignUserSchool(user.Id, viewModel.SchoolId);

            return Json(new { success = true, ErrorList = viewModel.ValidationErrors });
        }

        [HttpGet, AdminOnly(Order = 3)]
        public ActionResult ResetPassword(int userId)
        {
            var user = parameters.UserService.GetUserById(userId);
            if (!IsNotNullAndCanAccess(user))
            {
                return Json(false);
            }

            if (!parameters.VulnerabilityService.HasRightToUpdateUser(CurrentUser, userId, user.DistrictId.GetValueOrDefault()))
            {
                return Json(new { error = "Has no right on user" }, JsonRequestBehavior.AllowGet);
            }

            var model = new ResetPassword { UserId = userId };
            return View("_ResetPassword", model);
        }

        [HttpPost, AdminOnly(Order = 3)]
        public ActionResult ResetPassword(ResetPassword model)
        {
            var user = parameters.UserService.GetUserById(model.UserId);
            if (!IsNotNullAndCanAccess(user))
            {
                ModelState.AddModelError("error", "User does not exist.  Please try again.");
            }
            if (string.IsNullOrEmpty(model.NewPassword) || string.IsNullOrEmpty(model.ConfirmNewPassword))
            {
                ModelState.AddModelError("error", ConfigurationManager.AppSettings["PasswordRequirements"]);
                return View("_ResetPasswordForm", model);
            }
            if (!Regex.IsMatch(model.NewPassword, ConfigurationManager.AppSettings["PasswordRegex"]))
            {
                ModelState.AddModelError("error", ConfigurationManager.AppSettings["PasswordRequirements"]);
            }

            if (!ModelState.IsValid)
            {
                return View("_ResetPasswordForm", model);
            }
            //avoid modify ajax request
            if (!Util.HasRightOnDistrict(CurrentUser, user.DistrictId.GetValueOrDefault()))
            {
                return Json(new { Success = false, Error = "Has no right on this " + LabelHelper.DistrictLabel + "." });
            }
            //Check if user has right on this user
            if (!CurrentUser.IsPublisher)
            {
                var hasRight = parameters.VulnerabilityService.HasRightToUpdateUser(CurrentUser, user.Id, user.DistrictId.GetValueOrDefault());
                if (!hasRight)
                {
                    return Json(new { Success = false, Error = "Has no right on this user." });
                }
            }
            parameters.UserService.ResetUsersPassword(user, model.NewPassword);
            return Json(true);
        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.TmgmtUploadassessmentresults)]
        public ActionResult UploadAssessmentResults(string id)
        {
            id = string.IsNullOrEmpty(id) ? string.Empty : id;
            return View(model: id);
        }

        [HttpGet, AdminOnly(Order = 3)]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.DadManageRosters)]
        public ActionResult ManageRosters()
        {
            var model = new UploadRosterViewModel();
            model.RequestTypes = GetRosterRequestTypes();
            return View(model);
        }

        [HttpGet, AdminOnly(Order = 3)]
        public ActionResult AddRoster()
        {
            var model = new UploadRosterViewModel
            {
                IsPublisherUploading = CurrentUser.RoleId.Equals((int)Permissions.Publisher)
            };
            ViewBag.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
            ViewBag.DistrictId = CurrentUser.DistrictId;
            ViewBag.StateId = CurrentUser.StateId;
            model.RequestTypes = GetRosterRequestTypes();
            if (model.RequestTypes.Any())
            {
                model.RequestTypeId = Convert.ToInt32(model.RequestTypes[0].Value);
            }
            return PartialView("_UploadRoster", model);
        }

        private List<SelectListItem> GetRosterRequestTypes()
        {
            var requestTypes = new List<SelectListItem>();

            try
            {
                var rosterOptions = GetRosterOptions();
                requestTypes.AddRange(rosterOptions.Select(x =>
                    new SelectListItem
                    {
                        Text = x.Value,
                        Value = x.Key.ToString()
                    }));
            }
            catch
            {
                //nothing
            }

            return requestTypes;
        }

        private List<KeyValuePair<int, string>> GetRosterOptions()
        {
            var districtId = CurrentUser.DistrictId;
            if (CurrentUser.IsPublisher || CurrentUser.IsNetworkAdmin)
            {
                var liCode = HttpContext.GetLICodeFromRequest();
                districtId = _districtService.GetLiCodeBySubDomain(liCode);
            }

            var districtRosterOptions = parameters.DistrictRosterOptionService.Select()
                .Where(x => x.DistrictId == districtId).ToList();
            if (!districtRosterOptions.Any())
            {
                districtRosterOptions = parameters.DistrictRosterOptionService.Select()
                    .Where(x => x.DistrictId == null).ToList();
            }
            if (!CurrentUser.IsDistrictAdmin && !CurrentUser.IsNetworkAdmin && !CurrentUser.IsPublisher)
            {
                var rosterTypeExclude = new List<int>
                {
                   (int)RequestType.PowerSchoolFullRefresh,
                   (int)RequestType.OneRosterFullRefresh,
                   (int)RequestType.SchoolTooleScholarFullRefresh
                };

                districtRosterOptions = districtRosterOptions.Where(m => !rosterTypeExclude.Contains(m.RosterTypeId)).ToList();
            }

            var rosterTypes = parameters.RosterTypeService.Select().ToList();

            var rosterOptions = districtRosterOptions.Where(x => x.IsEnabled == 1)
                .OrderBy(x => x.DisplayOrder)
                .Select(x => new KeyValuePair<int, string>(
                    x.RosterTypeId,
                    string.IsNullOrEmpty(x.DisplayName)
                        ? rosterTypes.FirstOrDefault(k => k.RosterTypeId == x.RosterTypeId).RosterTypeName
                        : x.DisplayName
                    )
                ).ToList();

            return rosterOptions;
        }

        [HttpGet, AdminOnly(Order = 3)]
        public ActionResult GetRosters()
        {
            var districtId = CurrentUser.DistrictId;
            if (CurrentUser.IsPublisher || CurrentUser.IsNetworkAdmin)
            {
                var liCode = HttpContext.GetLICodeFromRequest();
                districtId = _districtService.GetLiCodeBySubDomain(liCode);
            }
            var rosters =
                parameters.RosterRequestService.GetRequestsByUserId(CurrentUser.Id, districtId.GetValueOrDefault()).Select(x => new RequestViewModel
                {
                    Status = Enum.IsDefined(typeof(RequestStatus), x.RequestStatus) ? (int)x.RequestStatus : -1,
                    DistrictName = $"{x.DistrictName} ({x.DistrictId})",
                    FileName = x.ImportedFileName,
                    RosterType = x.RosterType,
                    DateUploaded = x.RequestTime,
                    IsDeleted = x.IsDeleted,
                    CanSubmit = parameters.RequestService.CanSubmitRoster(x),
                    IsSubmitted = parameters.RequestService.IsRosterSubmitted(x),
                    HasBeenMoved = x.HasBeenMoved,
                    HasEmailContent = x.HasEmailContent,
                    Id = x.Id,
                    Mode = Enum.IsDefined(typeof(RequestMode), x.RequestMode) ? (int)x.RequestMode : -1,
                    DataRequestTypeId = (int)x.DataRequestType
                });

            var parser = new DataTableParser<RequestViewModel>();

            return Json(parser.ParseForClientSide(rosters), JsonRequestBehavior.AllowGet);
        }

        [HttpGet, AdminOnly(Order = 3)]
        public ActionResult GetEmailContentForRoster(int requestId)
        {
            var request = parameters.RequestService.GetRequestByIdAndUserId(CurrentUser.Id, requestId);
            if (request.IsNull())
            {
                return HttpNotFound();
            }
            var emailContent = string.Format("<h2>Request: {0}</h2>", request.Id) +
                               parameters.RequestService.GetEmailContentForRequest(requestId);

            var district = parameters.DistrictService.GetDistrictById(request.DistrictId);
            var subDomain = HelperExtensions.GetSubdomain().ToLower();
            if (district != null && !string.Equals(district.LICode, subDomain, StringComparison.OrdinalIgnoreCase))
            {
                var hasPermission = false;
                if (CurrentUser.RoleId == (int)RoleEnum.NetworkAdmin)
                {
                    var user = parameters.UserService.GetUserById(CurrentUser.Id);
                    var districts = parameters.DspDistrictService.GetDistrictMembers(user.DistrictId.Value).ToList();
                    hasPermission = districts.Any(x => x.Id == request.DistrictId);
                }

                if (CurrentUser.RoleId == (int)RoleEnum.Publisher || hasPermission)
                {
                    var linkItUrl = ConfigurationManager.AppSettings["LinkItUrl"];
                    var requestUrl = $"{district.LICode}.{linkItUrl}";
                    var newUrl = $"{subDomain}.{linkItUrl}";
                    emailContent = Regex.Replace(emailContent, requestUrl, newUrl, RegexOptions.IgnoreCase);
                }
            }

            return Json(emailContent, JsonRequestBehavior.AllowGet);
        }

        [UploadifyPrincipal(Order = 1)]
        public ActionResult UploadRoster(HttpPostedFileBase postedFile, int? stateId, int? districtId, int? requestTypeId)
        {
            if (!IsValidPostedFile(postedFile))
            {
                return Json(new { message = "Invalid file, please try again.", success = false, type = "error" },
                            JsonRequestBehavior.AllowGet);
            }

            if (!requestTypeId.HasValue || !Enum.IsDefined(typeof(RequestType), requestTypeId))
            {
                return Json(new { message = "Invalid request type, please try again.", success = false, type = "error" },
                            JsonRequestBehavior.AllowGet);
            }

            if (!(CurrentUser.IsPublisher || CurrentUser.IsNetworkAdmin))
            {
                stateId = CurrentUser.StateId;
                districtId = CurrentUser.DistrictId;
            }
            return SaveRequestAndUploadFile(postedFile, requestTypeId.Value, districtId.Value);
        }

        [UploadifyPrincipal(Order = 1)]
        public ActionResult UploadRosterMultiple(IEnumerable<HttpPostedFileBase> postedFiles, int? stateId, int? districtId, int? requestTypeId)
        {
            if (!(CurrentUser.IsPublisher || CurrentUser.IsNetworkAdmin))
            {
                stateId = CurrentUser.StateId;
                districtId = CurrentUser.DistrictId;
            }

            var validation = _rosterValidationService.InitRosterValidationRequest(postedFiles, stateId, districtId,  requestTypeId, CurrentUser);
            if (!validation.Success)
                return Json(new { message = validation.Message, success = validation.Success, type = "error"}, JsonRequestBehavior.AllowGet);

            var extensions = postedFiles.Select(file => Path.GetExtension(file.FileName).ToLower()).ToList();

            var uploadFilePath = AssignUploadFilePath(validation.Request);
            var envID = LinkitConfigurationManager.Vault.DatabaseID;
            if (!string.IsNullOrEmpty(envID))
            {
                uploadFilePath = string.Format("{0}\\{1}", uploadFilePath, envID);
            }

            if (extensions.Count == 1 && extensions[0].Equals(".zip"))
            {
                parameters.RosterUploadService.UploadRosterFile(postedFiles.First(), validation.Request, uploadFilePath);
            }
            else
            {
                var modelZipHelper = postedFiles.Select(file => new ZipModel()
                {
                    FileData = GetFileDataContent(file),
                    FileRelativeName = file.FileName
                }).ToList();

                string filePath = ZipHelper.RosterZip(TempFolder, modelZipHelper, validation.Request.Id);

                var destinationPath = Path.Combine(uploadFilePath, $"{validation.Request.Id}.zip");
                if (System.IO.File.Exists(destinationPath))
                {
                    System.IO.File.Delete(destinationPath);
                }

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Move(filePath, destinationPath);

                    var folderPath = Path.GetDirectoryName(filePath);
                    if (Directory.Exists(folderPath))
                        Directory.Delete(folderPath, true);
                }
            }

            _rosterValidationService.MakeQueueCanStart(validation.XpsQueue);

            return Json(new { success = true });
        }

        private byte[] GetFileDataContent(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                byte[] fileBytes;
                using (var binaryReader = new BinaryReader(file.InputStream))
                {
                    fileBytes = binaryReader.ReadBytes(file.ContentLength);
                }
                return fileBytes;
            }
            return new byte[0];
        }

        private int? GetXpsUploadTypeByRequestType(int requestTypeId)
        {
            switch (requestTypeId)
            {
                case (int)RequestType.PowerSchoolFullRefresh:
                    return (int)XpsUploadType.PowerSchool;
                case (int)RequestType.SchoolTooleScholarFullRefresh:
                    return (int)XpsUploadType.EScholarTemplate;
                case (int)RequestType.OneRosterFullRefresh:
                    return (int)XpsUploadType.OneRoster;
                default:
                    return null;
            }
        }

        private ActionResult SaveRequestAndUploadFile(HttpPostedFileBase postedFile, int requestTypeId, int districtId)
        {
            var request = parameters.RequestService.CreateRequestWithFileName(CurrentUser, postedFile.FileName, (RequestType)requestTypeId, districtId);
            request.SetValidator(parameters.RequestValidator);
            if (!request.IsValid)
            {
                return
                    Json(
                        new
                        {
                            ErrorList = request.ValidationErrors,
                            message = "An error has occured, please try again.",
                            success = false,
                            type = "error"
                        }, JsonRequestBehavior.AllowGet);
            }

            parameters.RequestService.Insert(request);
            CreateRequestParameter(request, "FileName", postedFile.FileName);
            if (request.RequestType.Equals(RequestType.StudentFullRefresh) || request.RequestType.Equals(RequestType.StudentAddUpdate))
            {
                CreateRequestParameter(request, "UpdateStudentInfo", "1");

                var classNameType = _rosterValidationService.GetClassNameType(districtId, XpsUploadType.TemplateFormat);
                if (classNameType.HasValue)
                {
                    CreateRequestParameter(request, nameof(XpsDistrictUpload.ClassNameType), $"{classNameType}");
                }
            }
            var uploadFilePath = AssignUploadFilePath(request);
            // Upload correct folder by Identifier
            var envID = LinkitConfigurationManager.Vault.DatabaseID;
            if (!string.IsNullOrEmpty(envID))
            {
                uploadFilePath = string.Format("{0}\\{1}", uploadFilePath, envID);
            }

            parameters.RosterUploadService.UploadFile(postedFile, request, uploadFilePath);
            return Json(new { success = true });
        }

        [HttpGet, AdminOnly(Order = 3)]
        public ActionResult DeleteRequest(int? requestId)
        {
            var request = parameters.RequestService.GetRequestByIdAndUserId(CurrentUser.Id,
                                                                            requestId.GetValueOrDefault());
            if (request.IsNotNull())
            {
                request.IsDeleted = true;
                request.SetValidator(parameters.RequestValidator);
                parameters.RequestService.Update(request);
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { message = "An error has occurred.  Please try again.", success = false },
                        JsonRequestBehavior.AllowGet);
        }

        [HttpGet, AdminOnly(Order = 3), AjaxOnly]
        public ActionResult SubmitRequest(int? requestId)
        {
            var request = parameters.RequestService.GetRequestByIdAndUserId(CurrentUser.Id,
                                                                            requestId.GetValueOrDefault());
            if (request.IsNotNull())
            {
                request.HasBeenMoved = true;
                request.SetValidator(parameters.RequestValidator);
                parameters.RequestService.Update(request);

                var uploadFilePath = AssignUploadFilePath(request);
                var completedFilePath = AssignCompletedFilePath(request);

                //Upload correct folder by Identifier
                var envID = LinkitConfigurationManager.Vault.DatabaseID;
                if (!string.IsNullOrEmpty(envID))
                {
                    uploadFilePath = string.Format("{0}\\{1}", uploadFilePath, envID);
                    completedFilePath = string.Format("{0}\\{1}", completedFilePath, envID);
                }

                parameters.RosterUploadService.MoveRosterFileBackToUploadedFolder(request.Id, request.ImportedFileName, uploadFilePath,
                                                                                  completedFilePath, (message =>
                                                                                  {
                                                                                      PortalAuditManager.LogError(message);
                                                                                  }));
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { message = "An error has occurred.  Please try again.", success = false },
                        JsonRequestBehavior.AllowGet);
        }

        private string AssignUploadFilePath(Request request)
        {
            switch (request.DataRequestType)
            {
                case DataRequestType.StudentFullRefresh:
                    return ConfigurationManager.AppSettings["RosterUploadFilePath"];

                case DataRequestType.StudentAddUpdate:
                    return ConfigurationManager.AppSettings["RosterUploadFilePath"];

                case DataRequestType.Staff:
                    return ConfigurationManager.AppSettings["StaffUploadFilePath"];

                case DataRequestType.StaffFullRefresh:
                    return ConfigurationManager.AppSettings["StaffUploadFilePath"];

                case DataRequestType.StudentProgram:
                    return ConfigurationManager.AppSettings["ProgramStudentUploadFilePath"];

                case DataRequestType.TestDataUpload:
                    return ConfigurationManager.AppSettings["TestDataUploadFilePath"];

                case DataRequestType.Parent:
                    return ConfigurationManager.AppSettings["StudentParentUploadFilePath"];

                case DataRequestType.StudentMeta:
                    return ConfigurationManager.AppSettings["StudentMetaUploadFilePath"];
                case DataRequestType.OneRosterFullRefresh:
                case DataRequestType.SchoolTooleScholarFullRefresh:
                case DataRequestType.PowerSchoolFullRefresh:
                    return ConfigurationManager.AppSettings["RosterValidationUploadFilePath"];
                default:
                    return string.Empty;
            }
        }

        private string AssignCompletedFilePath(Request request)
        {
            switch (request.DataRequestType)
            {
                case DataRequestType.StudentFullRefresh:
                    return ConfigurationManager.AppSettings["RosterCompletedFilePath"];

                case DataRequestType.StudentAddUpdate:
                    return ConfigurationManager.AppSettings["RosterCompletedFilePath"];

                case DataRequestType.Staff:
                    return ConfigurationManager.AppSettings["StaffCompletedFilePath"];

                case DataRequestType.StaffFullRefresh:
                    return ConfigurationManager.AppSettings["StaffCompletedFilePath"];

                case DataRequestType.StudentProgram:
                    return ConfigurationManager.AppSettings["ProgramStudentCompletedFilePath"];

                case DataRequestType.Parent:
                    return ConfigurationManager.AppSettings["StudentParentProcessedFolder"];
                case DataRequestType.OneRosterFullRefresh:
                case DataRequestType.SchoolTooleScholarFullRefresh:
                case DataRequestType.PowerSchoolFullRefresh:
                    return ConfigurationManager.AppSettings["RosterValidationCompletedFilePath"];
                default:
                    return string.Empty;
            }
        }

        private IQueryable<ManageUsersViewModel> BindUserManageToManageUsersViewModel(
            IQueryable<UserManage> userSchools)
        {
            return userSchools.Select(x => new ManageUsersViewModel
            {
                UserId = x.UserId,
                UserName = x.UserName,
                FullName = $"{x.LastName}, {x.FirstName}",
                RoleName = x.RoleName,
                DistrictName = x.DistrictName,
                StateName = x.StateName,
                UserStatusId = x.UserStatusId.GetValueOrDefault(),
                FirstSchool = GetFirstSchoolItem(x.SchoolList),
                SchoolList = x.SchoolList ?? string.Empty
            }).Distinct();
        }

        private string GetFirstSchoolItem(string itemList)
        {
            try
            {
                if (!string.IsNullOrEmpty(itemList) && itemList.Contains("|"))
                {
                    string firstItem = itemList.Split('|')[0];
                    if (itemList.Substring(0, itemList.Length - 1) == firstItem)
                        return firstItem;

                    return firstItem + " ...";
                }
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return null;
            }

            return null;
        }

        private void CreateNewUser(CreateUserViewModel model)
        {
            if (!Enum.IsDefined(typeof(Permissions), model.RoleId))
            {
                throw new ArgumentException("Permission level does not exist.");
            }

            var user = InitializeNewUser(model);
            parameters.UserService.SaveUser(user);

            if (RequiresUserSchool(model))
            {
                AssignUserSchool(user.Id, model.SchoolId);
            }
        }

        private bool RequiresUserSchool(CreateUserViewModel model)
        {
            return model.RoleId == (int)Permissions.SchoolAdmin || model.RoleId == (int)Permissions.Teacher;
        }

        private void AssignUserSchool(int userId, int schoolId)
        {
            var newUserSchool = new UserSchool
            {
                SchoolId = schoolId,
                UserId = userId,
                DateActive = DateTime.Now
            };
            newUserSchool.SetValidator(parameters.UserSchoolValidator);
            if (!newUserSchool.IsValid)
            {
                throw new Exception("User school entry is not valid.");
            }
            parameters.UserSchoolService.InsertUserSchool(newUserSchool);
            parameters.UserService.UpdateDateConfirmActive(newUserSchool.UserId);
        }

        private User InitializeNewUser(CreateUserViewModel model)
        {
            return new User
            {
                UserName = model.UserName,
                EmailAddress = model.EmailAddress,
                HashedPassword = parameters.UserService.HashPassword(model.Password),
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                LocalCode = model.LocalCode,
                StateCode = model.StateCode,
                AddedByUserId = CurrentUser.Id,
                ApiAccess = false,
                DistrictGroupId = 0,
                SchoolId = model.SchoolId,
                UserStatusId = 1,
                RoleId = model.RoleId,
                StateId = AssignStateId(model),
                DistrictId = AssignDistrictId(model),
                DateConfirmedActive = DateTime.Today,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                ModifiedUser = CurrentUser.Id,
                ModifiedBy = "Portal",
            };
        }

        private int AssignStateId(CreateUserViewModel model)
        {
            return model.StateId == 0 ? CurrentUser.StateId.GetValueOrDefault() : model.StateId;
        }

        private int? AssignDistrictId(CreateUserViewModel model)
        {
            return !model.DistrictId.HasValue ? CurrentUser.DistrictId : model.DistrictId.GetValueOrDefault();
        }

        private IEnumerable<ListItem> GetSchoolsBasedOnPermissions()
        {
            if (CurrentUser.IsDistrictAdminOrPublisher)
            {
                return
                    parameters.SchoolService.GetSchoolsByDistrictId(CurrentUser.DistrictId.GetValueOrDefault()).Select(
                        x => new ListItem { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name);
            }

            return
                parameters.UserSchoolService.GetSchoolsUserHasAccessTo(CurrentUser.Id).Select(
                    x => new ListItem { Id = x.SchoolId.GetValueOrDefault(), Name = x.SchoolName }).OrderBy(x => x.Name);
        }

        private CreateUserViewModel InitializeNewCreateUserViewModel()
        {
            return new CreateUserViewModel { CurrentUserRoleId = CurrentUser.RoleId };
        }

        private bool IsNotNullAndCanAccess(User user)
        {
            if (user.IsNull())
            {
                return false;
            }

            if (CurrentUser.IsLinkItAdminOrPublisher() || CurrentUser.IsNetworkAdmin)
            {
                return true;
            }
            return parameters.UserSchoolService.CanAccessUser(CurrentUser, user);
        }

        private void CreateRequestParameter(Request request, string name, string value)
        {
            var requestParamter = new RequestParameter
            {
                RequestId = request.Id,
                Name = name,
                Value = value
            };

            requestParamter.SetValidator(parameters.RequestParameterValidator);
            if (requestParamter.IsValid)
            {
                parameters.RequestParameterService.Insert(requestParamter);
            }
        }

        private bool IsValidPostedFile(HttpPostedFileBase file)
        {
            if (file.IsNull())
            {
                return false;
            }
            return !string.IsNullOrEmpty(file.FileName) && file.InputStream.IsNotNull();
        }

        [HttpGet]
        public ActionResult AddSchool()
        {
            return View();
        }

        [HttpPost, AjaxOnly]
        public ActionResult AddSchool(School addedSchool)
        {
            addedSchool.DistrictId = CurrentUser.DistrictId.GetValueOrDefault();
            addedSchool.SetValidator(parameters.SchoolValidator);
            if (addedSchool.IsValid)
            {
                parameters.SchoolService.Save(addedSchool);
                return Json(new { Success = true });
            }

            return Json(new { Success = false, ErrorList = addedSchool.ValidationErrors });
        }

        [HttpPost, AjaxOnly]
        public ActionResult AddUserToSchool(int userId, int schoolId)
        {
            AssignUserSchool(userId, schoolId);
            return Json(true);
        }

        private string ValidateSourceFileUpload(HttpPostedFileBase postedFile)
        {
            if (!IsValidPostedFile(postedFile))
            {
                return "Invalid input data file. Please try again.";
            }
            return string.Empty;
        }

        private string ValidateSourceContentFormat(string content)
        {
            string errorMessage = string.Empty;
            if (string.IsNullOrEmpty(content))
            {
                errorMessage = "Input data file is empty.";
            }
            else
            {
                string[] lines = Regex.Split(content, Environment.NewLine);
                string headerLine = lines[0];
                Match match = Regex.Match(headerLine, @"^.*\t.*\t.*\t.*[\t.*]*$", RegexOptions.IgnoreCase);
                if (!match.Success)
                {
                    errorMessage = "Input data file has invalid format.";
                }
            }
            return errorMessage;
        }

        [HttpGet]
        [AdminOnly(Order = 3)]
        public ActionResult MappingDetail(int id)
        {
            var mapping = parameters.MappingInformationService.GetMappingById(id);
            if (mapping.IsNotNull())
            {
                var model = new MappingDetailViewModel
                {
                    SourceColumns = mapping.GetSourceColumnList(),
                    MapID = id
                };
                return View(model);
            }
            return RedirectToAction("ManageMapping");
        }

        [HttpGet]
        [AdminOnly(Order = 3)]
        public ActionResult GetSampleData(int mappingId, int columnId)
        {
            var mapping = parameters.MappingInformationService.GetMappingById(mappingId);
            if (mapping.IsNotNull())
            {
                var data = mapping.GetSampleData(columnId);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json(new List<string>(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AdminOnly(Order = 3)]
        public ActionResult GetFixedOrLookupValuesForColumn(int columnId)
        {
            var data = parameters.StandardColumnService.GetFixedOrLookupValueByColumn(columnId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AdminOnly(Order = 3)]
        public ActionResult GetUniqueValuesForSourceColumn(int mappingId, int columnId)
        {
            var mapping = parameters.MappingInformationService.GetMappingById(mappingId);
            if (mapping.IsNotNull())
            {
                var data = mapping.GetUniqueValues(columnId);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json(new List<string>(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AdminOnly(Order = 3)]
        public ActionResult GetStandardColumns(int loaderType, int columnType)
        {
            var data =
                parameters.StandardColumnService.GetAllColumnByLoaderTypeAndColumnType((MappingLoaderType)loaderType,
                                                                                       (StandardColumnTypes)columnType)
                    .ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private ActionResult CreateNewMappingRule(int phaseId, MappingInformation mapping, List<BaseMapping> mappings)
        {
            if (phaseId == 0)
            {
                mapping.XmlTransform = parameters.MappingRuleHelper.CreateNewMappingRuleAndReturnXmlTransform(mappings);
                parameters.MappingInformationService.SaveMapping(mapping);
                return Json(new { success = true });
            }
            return
                Json(
                    new
                    {
                        message = string.Format("Error. Requested phaseId ({0}) is not valid.", phaseId),
                        success = false,
                        type = "error"
                    });
        }

        private string CreateNewMappingRuleAndReturnErrorMessage(int phaseId, MappingInformation mapping,
                                                                 List<BaseMapping> mappings)
        {
            if (phaseId == 0)
            {
                mapping.XmlTransform = parameters.MappingRuleHelper.CreateNewMappingRuleAndReturnXmlTransform(mappings);
                parameters.MappingInformationService.SaveMapping(mapping);
                return string.Empty;
            }
            return string.Format("Requested phaseId ({0}) is not valid.", phaseId);
        }

        private ActionResult CreateNewOrUpdateTestPhaseMapping(int phaseId, MappingInformation mapping,
                                                               List<BaseMapping> mappings)
        {
            var xmlTransform = parameters.MappingRuleHelper.UpdatingTestPhaseAndReturnXmlTransform(
                mapping.XmlTransform, mappings, phaseId);
            if (!string.IsNullOrEmpty(xmlTransform))
            {
                mapping.XmlTransform = xmlTransform;
                parameters.MappingInformationService.SaveMapping(mapping);
                return Json(new { success = true });
            }
            xmlTransform = parameters.MappingRuleHelper.CreateNewTestPhaseAndReturnXmlTransform(mapping.XmlTransform,
                                                                                                mappings, phaseId);
            if (!string.IsNullOrEmpty(xmlTransform))
            {
                mapping.XmlTransform = xmlTransform;
                parameters.MappingInformationService.SaveMapping(mapping);
                return Json(new { success = true });
            }
            return
                Json(
                    new
                    {
                        message = string.Format("Error. Requested phaseId ({0}) is not valid.", phaseId),
                        success = false,
                        type = "error"
                    });
        }

        private string CreateNewOrUpdateTestPhaseMappingAndReturnErrorMessage(int phaseId, MappingInformation mapping,
                                                                              List<BaseMapping> mappings)
        {
            var xmlTransform = parameters.MappingRuleHelper.UpdatingTestPhaseAndReturnXmlTransform(
                mapping.XmlTransform, mappings, phaseId);
            if (!string.IsNullOrEmpty(xmlTransform))
            {
                mapping.XmlTransform = xmlTransform;
                parameters.MappingInformationService.SaveMapping(mapping);
                return string.Empty;
            }
            xmlTransform = parameters.MappingRuleHelper.CreateNewTestPhaseAndReturnXmlTransform(mapping.XmlTransform,
                                                                                                mappings, phaseId);
            if (!string.IsNullOrEmpty(xmlTransform))
            {
                mapping.XmlTransform = xmlTransform;
                parameters.MappingInformationService.SaveMapping(mapping);
                return string.Empty;
            }
            return string.Format("Error. Requested phaseId ({0}) is not valid.", phaseId);
        }

        private ActionResult UpdateCommonPhaseOrSavingTestPhaseMapping(int phaseId, MappingInformation mapping,
                                                                       List<BaseMapping> mappings)
        {
            if (phaseId == 0)
            {
                mapping.XmlTransform =
                    parameters.MappingRuleHelper.CreateNewOrUpdatingCommonPhaseAndReturnXmlTransform(
                        mapping.XmlTransform, mappings);
                parameters.MappingInformationService.SaveMapping(mapping);
                return Json(new { success = true });
            }
            return CreateNewOrUpdateTestPhaseMapping(phaseId, mapping, mappings);
        }

        private string UpdateCommonPhaseOrSavingTestPhaseMappingAndReturnErrorMessage(int phaseId,
                                                                                      MappingInformation mapping,
                                                                                      List<BaseMapping> mappings)
        {
            if (phaseId == 0)
            {
                mapping.XmlTransform =
                    parameters.MappingRuleHelper.CreateNewOrUpdatingCommonPhaseAndReturnXmlTransform(
                        mapping.XmlTransform, mappings);
                parameters.MappingInformationService.SaveMapping(mapping);
                return string.Empty;
            }
            return CreateNewOrUpdateTestPhaseMappingAndReturnErrorMessage(phaseId, mapping, mappings);
        }

        [HttpPost]
        [AdminOnly(Order = 3)]
        [ValidateInput(false)]
        [AjaxOnly]
        public ActionResult ValidateAndUpdateMappingPhase(int mappingId, int phaseId, string mappingPhaseXmlText)
        {
            var mapping = parameters.MappingInformationService.GetMappingById(mappingId);
            if (mapping != null)
            {
                var listMappingSerialization = new ETLXmlSerialization<ListMapping>();
                var listMapping = listMappingSerialization.DeserializeXmlToObject(mappingPhaseXmlText);
                if (listMapping.IsNull())
                {
                    return Json(new { message = "Error. Xml transform is empty.", success = false, type = "error" });
                }
                var errorMessage = MappingListWithRequiredColumnValidate(listMapping.Mappings, phaseId);
                return string.IsNullOrEmpty(errorMessage)
                           ? SavingMapping(mapping, phaseId, listMapping.Mappings)
                           : Json(
                               new
                               {
                                   message = string.Format("Error. {0}", errorMessage),
                                   success = false,
                                   type = "error"
                               });
            }
            return Json(new { message = "Error. Mapping does not exist.", success = false, type = "error" });
        }

        private ActionResult SavingMapping(MappingInformation mapping, int phaseId, List<BaseMapping> listMappings)
        {
            return string.IsNullOrEmpty(mapping.XmlTransform)
                       ? CreateNewMappingRule(phaseId, mapping, listMappings)
                       : UpdateCommonPhaseOrSavingTestPhaseMapping(phaseId, mapping, listMappings);
        }

        [HttpPost]
        [AdminOnly(Order = 3)]
        [ValidateInput(false)]
        [AjaxOnly]
        public ActionResult SavingMappingPhase(int mappingId, int phaseId, string mappingPhaseXmlText)
        {
            var mapping = parameters.MappingInformationService.GetMappingById(mappingId);
            if (mapping != null)
            {
                var listMappingSerialization = new ETLXmlSerialization<ListMapping>();
                var listMapping = listMappingSerialization.DeserializeXmlToObject(mappingPhaseXmlText);
                if (listMapping.IsNull())
                {
                    return Json(new { message = "Error. Xml transform is empty.", success = false, type = "error" });
                }
                return string.IsNullOrEmpty(mapping.XmlTransform)
                           ? CreateNewMappingRule(phaseId, mapping, listMapping.Mappings)
                           : UpdateCommonPhaseOrSavingTestPhaseMapping(phaseId, mapping, listMapping.Mappings);
            }
            return Json(new { message = "Error. Mapping does not exist.", success = false, type = "error" });
        }

        [HttpPost]
        [AdminOnly(Order = 3)]
        [ValidateInput(false)]
        [AjaxOnly]
        public ActionResult CompleteMappingPhase(int mappingId, int phaseId, string mappingPhaseXmlText)
        {
            var mapping = parameters.MappingInformationService.GetMappingById(mappingId);
            if (mapping != null)
            {
                var listMappingSerialization = new ETLXmlSerialization<ListMapping>();
                var listMapping = listMappingSerialization.DeserializeXmlToObject(mappingPhaseXmlText);
                if (listMapping.IsNull())
                    return Json(new { message = "Error. Xml transform is empty.", success = false, type = "error" });
                var errorMessage = parameters.MappingValidatingHelper.MappingListValidate(listMapping.Mappings);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    return Json(new { message = errorMessage, success = false, type = "error" });
                }
                errorMessage = SavingMappingAndReturnErrorMessage(mapping, phaseId, listMapping.Mappings);
                return !string.IsNullOrEmpty(errorMessage)
                           ? Json(
                               new
                               {
                                   message = string.Format("Error. {0}.", errorMessage),
                                   success = false,
                                   type = "error"
                               })
                           : ValidateMappingRule(mapping.XmlTransform);
            }
            return Json(new { message = "Error. Mapping does not exist.", success = false, type = "error" });
        }

        private string SavingMappingAndReturnErrorMessage(MappingInformation mapping, int phaseId,
                                                          List<BaseMapping> listMappings)
        {
            var errorMessage = string.IsNullOrEmpty(mapping.XmlTransform)
                                   ? CreateNewMappingRuleAndReturnErrorMessage(phaseId, mapping, listMappings)
                                   : UpdateCommonPhaseOrSavingTestPhaseMappingAndReturnErrorMessage(phaseId, mapping,
                                                                                                    listMappings);
            return errorMessage;
        }

        [HttpGet]
        [AdminOnly(Order = 3)]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.TechETLtool)]
        public ActionResult ManageMapping()
        {
            var currentMapping = parameters.MappingInformationService.GetFirstMappingByUserId(CurrentUser.Id);
            ViewBag.MappingID = currentMapping.IsNotNull()
                                    ? currentMapping.MapID.ToString(CultureInfo.InvariantCulture)
                                    : "0";
            return View();
        }

        [UploadifyPrincipal(Order = 1)]
        public ActionResult CreateMapping(HttpPostedFileBase postedFile)
        {
            var mappingName = DateTime.Now.ToString("dd/MM/yyyy-HHmmss");
            var errorMessage = ValidateSourceFileUpload(postedFile);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                return Json(new { message = errorMessage, success = false, type = "error" });
            }
            var content = GetInputDataFromPostedFile(postedFile);
            errorMessage = ValidateSourceContentFormat(content);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                return Json(new { message = errorMessage, success = false, type = "error" });
            }
            var newMapping = SaveMapping(mappingName, (int)MappingLoaderType.DataLoader, content);
            return !newMapping.IsNotNull()
                       ? Json(
                           new
                           {
                               message = "A system error has occurred.  Please try again.",
                               success = false,
                               type = "error"
                           })
                       : Json(new { success = true, mapId = newMapping.MapID });
        }

        private string GetInputDataFromPostedFile(HttpPostedFileBase postedFile)
        {
            var reader = new StreamReader(postedFile.InputStream);
            var content = reader.ReadToEnd();
            return content;
        }

        private MappingInformation SaveMapping(string mappingName, int loaderType, string content)
        {
            try
            {
                var newMapping = InitNewMapping(mappingName, loaderType, content);
                parameters.MappingInformationService.DeleteAllMappingBeUserId(CurrentUser.Id);
                parameters.MappingInformationService.InsertNewMapping(newMapping);
                return newMapping;
            }
            catch
            {
                return null;
            }
        }

        private MappingInformation InitNewMapping(string mappingName, int loaderType, string content)
        {
            var newMapping = new MappingInformation
            {
                Name = mappingName,
                LoaderType = (MappingLoaderType)loaderType,
                ProgressStatus = MappingProgressStatus.InProgress,
                SourceFileContent = content,
                UserID = CurrentUser.Id,
                XmlTransform = string.Empty
            };
            newMapping.CreatedDate = newMapping.LastestModifiedDate = DateTime.Now;

            return newMapping;
        }

        [HttpGet, AdminOnly(Order = 3)]
        public FileStreamResult CreateMappingOutputFile(int mapId)
        {
            var mapping = parameters.MappingInformationService.GetMappingById(mapId);
            var outputData = string.Empty;
            if (mapping != null)
            {
                var mappingRuleSerialization = new ETLXmlSerialization<MappingRule>();
                var predefinedMappingRule = mappingRuleSerialization.DeserializeXmlToObject(mapping.XmlTransform);
                var engine = new ETLTransformEngine();
                outputData = engine.Transform(predefinedMappingRule, mapping.SourceFileContent);
            }
            //add some data from your database into that string:
            var byteArray = Encoding.ASCII.GetBytes(outputData);
            var stream = new MemoryStream(byteArray);

            return File(stream, "text/plain", string.Format("{0}.txt", DateTime.Now.ToString("ddMMyyyy-HHmmss")));
        }

        private ActionResult ValidateMappingRule(string xmlTransform)
        {
            var mappingRuleSerialization = new ETLXmlSerialization<MappingRule>();
            var predefinedMappingRule = mappingRuleSerialization.DeserializeXmlToObject(xmlTransform);
            var commonRequiredColumns = GetRequiredColumnsByPhase(0);
            var testRequiredColumns = GetRequiredColumnsByPhase(1);
            parameters.MappingValidatingHelper.IsCheckUnknownValue = true;
            var errorMessage = parameters.MappingValidatingHelper.ValidateMappingRule(predefinedMappingRule,
                                                                                      commonRequiredColumns,
                                                                                      testRequiredColumns);
            return !string.IsNullOrEmpty(errorMessage)
                       ? Json(new { message = errorMessage, success = false, type = "error" })
                       : Json(new { success = true });
        }

        private string MappingListWithRequiredColumnValidate(List<BaseMapping> mappings, int phaseId)
        {
            var requiredColumns = GetRequiredColumnsByPhase(phaseId);
            return parameters.MappingValidatingHelper.MappingListWithRequiredColumnValidate(mappings, requiredColumns);
        }

        private List<StandardColumn> GetRequiredColumnsByPhase(int phaseId)
        {
            var columnType = phaseId > 0 ? StandardColumnTypes.TestField : StandardColumnTypes.CommonField;
            var requiredColumns =
                parameters.StandardColumnService.GetRequiredColumnsByLoaderTypeAndColumnType(
                    MappingLoaderType.DataLoader, columnType).ToList();
            return requiredColumns;
        }

        [HttpGet]
        [AdminOnly(Order = 3)]
        public ActionResult GetMappingPhase(int mappingId, int phaseId)
        {
            var mapping = parameters.MappingInformationService.GetMappingById(mappingId);
            if (mapping.IsNotNull())
            {
                var mappingRule = parameters.MappingRuleHelper.GetMappingRuleFromXmlTransform(mapping.XmlTransform);
                var data = parameters.MappingTransferHelper.GenerateMappingTransferByPhase(mappingRule, phaseId);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AdminOnly(Order = 3)]
        public ActionResult GetDefaultColumnForSpecifiedPhase(int loaderType, int columnType)
        {
            var data =
                parameters.StandardColumnService.GetDefaultColumnsByLoaderTypeAndColumnType(
                    (MappingLoaderType)loaderType, (StandardColumnTypes)columnType).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AdminOnly(Order = 3)]
        public ActionResult GetRequiredColumnForSpecifiedPhase(int loaderType, int columnType)
        {
            var data =
                parameters.StandardColumnService.GetRequiredColumnsByLoaderTypeAndColumnType(
                    (MappingLoaderType)loaderType, (StandardColumnTypes)columnType).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AdminOnly(Order = 3)]
        //[AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.DadDistrictReferencedata)]
        [NonAction]
        public ActionResult DistrictReferenceData()
        {
            var model = new DistrictReferenceDataViewModel { DistrictID = CurrentUser.DistrictId ?? 0 };
            var district = parameters.DistrictService.GetDistrictById(model.DistrictID);
            if (district.IsNotNull())
            {
                PopulateWebUtilityViewModelProperties(district, model);
            }
            ViewBag.Domain = GetUrldomain();
            ViewBag.RoleId = CurrentUser.RoleId;
            return View(model);
        }

        [HttpPost]
        [AdminOnly(Order = 3)]
        public ActionResult DistrictReferenceData(DistrictReferenceDataViewModel model)
        {
            ViewBag.Domain = GetUrldomain();
            ViewBag.RoleId = CurrentUser.RoleId;
            var id = 0;
            if (model.IsNull())
            {
                model = new DistrictReferenceDataViewModel { DistrictID = id };
            }
            id = model.DistrictID;
            var district = parameters.DistrictService.GetDistrictById(id);
            if (district.IsNotNull())
            {
                PopulateWebUtilityViewModelProperties(district, model);
            }
            return View(model);
        }

        private string GetUrldomain()
        {
            var host = Request.Url.Host.Equals("localhost") ? Request.Url.Authority : Request.Url.Host;
            if (host.EndsWith("/"))
                host = host.Substring(0, host.Length - 1);
            host = host + Request.ApplicationPath;
            if (host.EndsWith("/"))
                host = host.Substring(0, host.Length - 1);
            var temp = string.Format("{0}://{1}/", HelperExtensions.GetHTTPProtocal(Request), host);

            var isSsl = ConfigurationManager.AppSettings["ForceSSL"];
            bool SslRequired = true;
            if (isSsl != null)
            {
                bool.TryParse(isSsl, out SslRequired);
            }
            if (SslRequired)
            {
                temp = temp.Replace("http://", "https://");
            }
            return temp;
        }

        [HttpGet]
        [AdminOnly(Order = 3)]
        public ActionResult GetDistrictsByKeyWord(string keywords, int pageIndex, int pageSize)
        {
            var searchObject = new DistrictSearchingObject
            {
                PageSize = pageSize,
                PageIndex = pageIndex,
                KeyWords = keywords
            };
            var resultList = parameters.DistrictService.GetDistrictsByKeyWords(searchObject);
            return
                Json(
                    new { data = resultList, resultCnt = searchObject.TotalRecords, pageNum = pageIndex, errorCode = 0 },
                    JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AdminOnly(Order = 3)]
        //[AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.DadDistrictReferencedata)]
        [NonAction]
        public ActionResult DistrictReferenceDataPrinting(int id)
        {
            DistrictReferenceDataViewModel model = null;
            if (!id.Equals(0))
            {
                model = new DistrictReferenceDataViewModel { DistrictID = id };
                var district = parameters.DistrictService.GetDistrictById(id);
                if (district.IsNotNull())
                {
                    PopulateWebUtilityViewModelProperties(district, model);
                }
            }
            return View(model);
        }

        private void PopulateWebUtilityViewModelProperties(District district, DistrictReferenceDataViewModel model)
        {
            State state = parameters.StateService.GetStateById(district.StateId);
            model.DistrictName = district.Name;
            model.DistrictFullName = parameters.WebUtilityHelper.GetDistrictFullName(district);
            model.StateName = parameters.WebUtilityHelper.GetStateName(state);
            model.Shools = parameters.SchoolService.GetSchoolsByDistrictId(district.Id).ToList();
            model.Programs = parameters.ProgramService.GetProgramsByDistrictID(district.Id).ToList();
            model.Races = parameters.RaceService.GetRacesByDistrictID(district.Id).ToList();
            model.Grades = parameters.GradeDistrictService.GetGradeByDistrictID(district.Id).ToList();
            model.Genders = parameters.GenderStudentService.GetGenderByDistrictID(district.Id).ToList();
            model.Subjects = parameters.SubjectDistrictService.GetSubjectByDistrictID(district.Id).ToList();
            model.Clusters = parameters.ClusterDistrictService.GetClustersByDistrictID(district.Id);
            model.AchievementLevelSettings =
                parameters.AchievementLevelSettingService.GetAchievementByDistrictID(district.Id).ToList();
            model.DistrictTerms = parameters.DistrictTermService.GetDistrictTermByDistrictID(district.Id).ToList();
        }

        public ActionResult OpenMoveStudentsForm(MoveStudentModel model)
        {
            model.UserId = CurrentUser.Id;
            if (CurrentUser.DistrictId != null)
                model.DistrictId = CurrentUser.DistrictId.Value;

            if (CurrentUser.IsPublisher() || CurrentUser.IsNetworkAdmin)
            {
                var oldClass = parameters.ClassService.GetClassById(model.OldClassId);
                if (oldClass != null)
                {
                    if (oldClass.DistrictId.HasValue && oldClass.DistrictId.Value > 0)
                    {
                        model.DistrictId = oldClass.DistrictId.GetValueOrDefault();
                    }
                    else
                    {
                        var districtTermObj = parameters.DistrictTermService.GetDistrictTermById(oldClass.DistrictTermId.GetValueOrDefault());
                        if (districtTermObj != null)
                        {
                            model.DistrictId = districtTermObj.DistrictID;
                        }
                    }
                }
            }

            return PartialView("_MoveStudentForm", model);
        }

        public ActionResult GetTeachersInSchool(int schoolId)
        {
            var validUserSchoolRoleId = new[] { 2, 3, 5, 8, 27 };
            var teachersHasTerm = parameters.TeacherDistrictTermService.GetTeachersHasTerms(schoolId).Select(x => x.UserId).ToList();

            var data = parameters.UserSchoolService.GetSchoolsUserBySchoolId(schoolId).Where(x => validUserSchoolRoleId.Contains((int)(x.Role)))
                .Where(x => teachersHasTerm.Contains(x.UserId))
                .Select(x => new //Only Publisher, " + LabelHelper.DistrictLabel + " Admin, Shool Admin, Teacher
                {
                    Name = x.UserName,
                    x.FirstName,
                    x.LastName,
                    x.DisplayName,
                    Id = x.UserId
                }).OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //\ TestResoultRemover
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.TmgmtTestresultremover)]
        public ActionResult TestResultRemover()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetVirtualTestTestResultDistrict(int districtId, int classId, int studentId, int schoolId, int teacherId, bool isRegrader)
        {
            if (districtId > 0 && !Util.HasRightOnDistrict(CurrentUser, districtId))
            {
                return Json(new { error = "Has no right on the selected district." }, JsonRequestBehavior.AllowGet);
            }

            if (teacherId > 0 && !HasRightOnPrimaryTeacher(districtId, isRegrader, teacherId))
            {
                return Json(new { error = "Has no right on the selected teacher." }, JsonRequestBehavior.AllowGet);
            }
            if (classId > 0 && !HasRightOnClassDistrictByRole(districtId, isRegrader, classId))
            {
                return Json(new { error = "Has no right on the selected class." }, JsonRequestBehavior.AllowGet);
            }
            if (studentId > 0 && !HasRightOnStudentDistrictByRole(districtId, isRegrader, studentId))
            {
                return Json(new { error = "Has no right on the selected student." }, JsonRequestBehavior.AllowGet);
            }
            if (schoolId > 0 && !HasRightOnSchoolDistrictFilterByRole(districtId, isRegrader, teacherId, classId, studentId, 0,
                        schoolId))
            {
                return Json(new { error = "Has no right on the selected school." }, JsonRequestBehavior.AllowGet);
            }
            //Use store
            var data = parameters.VirtualTestDistrictServices.GetVirtualTestDistrictFilterByRole(districtId,
                CurrentUser.Id, CurrentUser.RoleId, isRegrader, schoolId, teacherId, classId, studentId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // Get Test For Regrader
        [HttpGet]
        public ActionResult GetVirtualTestTestResultForRegrader(int? districtId, int schoolId, int? termId)
        {
            if (!districtId.HasValue)
                districtId = CurrentUser.DistrictId;

            //Use store
            var data = parameters.VirtualTestDistrictServices.GetVirtualTestForRegrader(districtId.GetValueOrDefault(),
                CurrentUser.Id, CurrentUser.RoleId, schoolId, termId.GetValueOrDefault());
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetStudentTestResultDistrict(int districtId, int virtualTestId, int classId, int schoolId, int teacherId, bool isRegrader)
        {
            if (districtId > 0 && !Util.HasRightOnDistrict(CurrentUser, districtId))
            {
                return Json(new { error = "Has no right on the selected district." }, JsonRequestBehavior.AllowGet);
            }
            if (virtualTestId > 0 && !HasRightOnVirtualTestByRole(districtId, isRegrader, virtualTestId))
            {
                return Json(new { error = "Has no right on the selected virtual test." }, JsonRequestBehavior.AllowGet);
            }
            if (teacherId > 0 && !HasRightOnPrimaryTeacher(districtId, isRegrader, teacherId))
            {
                return Json(new { error = "Has no right on the selected teacher." }, JsonRequestBehavior.AllowGet);
            }
            if (classId > 0 && !HasRightOnClassDistrictByRole(districtId, isRegrader, classId))
            {
                return Json(new { error = "Has no right on the selected class." }, JsonRequestBehavior.AllowGet);
            }
            var data = parameters.VirtualTestDistrictServices.GetStudentDistrictFilterByRole(districtId, CurrentUser.Id,
                CurrentUser.RoleId, isRegrader, schoolId, teacherId, classId, virtualTestId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetTeacherTestResultDistrict(int districtId, int virtualTestId, int classId, int studentId,
                                                         int schoolId, bool isRegrader)
        {
            if (districtId > 0 && !Util.HasRightOnDistrict(CurrentUser, districtId))
            {
                return Json(new { error = "Has no right on the selected district." }, JsonRequestBehavior.AllowGet);
            }
            if (virtualTestId > 0 && !HasRightOnVirtualTestByRole(districtId, isRegrader, virtualTestId))
            {
                return Json(new { error = "Has no right on the selected virtual test." }, JsonRequestBehavior.AllowGet);
            }
            if (classId > 0 && !HasRightOnClassDistrictByRole(districtId, isRegrader, classId))
            {
                return Json(new { error = "Has no right on the selected class." }, JsonRequestBehavior.AllowGet);
            }
            if (studentId > 0 && !HasRightOnStudentDistrictByRole(districtId, isRegrader, studentId))
            {
                return Json(new { error = "Has no right on the selected student." }, JsonRequestBehavior.AllowGet);
            }
            if (schoolId > 0 && !HasRightOnSchoolDistrictFilterByRole(districtId, isRegrader, 0, classId, studentId, virtualTestId, schoolId))
            {
                return Json(new { error = "Has no right on the selected school." }, JsonRequestBehavior.AllowGet);
            }
            // Use store
            var data = parameters.VirtualTestDistrictServices.GetPrimaryTeacherDistrictFilterByRole(districtId,
                CurrentUser.Id, CurrentUser.RoleId, isRegrader, schoolId, classId, studentId, virtualTestId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetClassTestResultDistrict_(int districtId, int virtualTestId, int studentId, int schoolId, int teacherId, bool isRegrader)
        {
            if (districtId > 0 && !Util.HasRightOnDistrict(CurrentUser, districtId))
            {
                return Json(new { error = "Has no right on the selected district." }, JsonRequestBehavior.AllowGet);
            }
            if (virtualTestId > 0 && !HasRightOnVirtualTestByRole(districtId, isRegrader, virtualTestId))
            {
                return Json(new { error = "Has no right on the selected virtual test." }, JsonRequestBehavior.AllowGet);
            }
            if (teacherId > 0 && !HasRightOnPrimaryTeacher(districtId, isRegrader, teacherId))
            {
                return Json(new { error = "Has no right on the selected teacher." }, JsonRequestBehavior.AllowGet);
            }
            if (studentId > 0 && !HasRightOnStudentDistrictByRole(districtId, isRegrader, studentId))
            {
                return Json(new { error = "Has no right on the selected student." }, JsonRequestBehavior.AllowGet);
            }
            if (schoolId > 0 && !HasRightOnSchoolDistrictFilterByRole(districtId, isRegrader, teacherId, 0, studentId, virtualTestId, schoolId))
            {
                return Json(new { error = "Has no right on the selected school." }, JsonRequestBehavior.AllowGet);
            }
            var data = parameters.VirtualTestDistrictServices.GetClassDistrictFilterByRole(districtId, CurrentUser.Id,
                CurrentUser.RoleId, isRegrader, virtualTestId, studentId, schoolId, teacherId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetClassTestResultDistrict(int? districtId, int schoolId, int? termId)
        {
            if (!districtId.HasValue)
                districtId = CurrentUser.DistrictId;

            var data = parameters.VirtualTestDistrictServices.GetClassDistrictFilterByRoleHasTestResult(districtId.GetValueOrDefault(), CurrentUser.Id, CurrentUser.RoleId, schoolId, termId.GetValueOrDefault());
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetSchoolTestResultDistrict_(int districtId, int virtualTestId, int teacherId, int classId, int studentId, bool isRegrader)
        {
            if (districtId > 0 && !Util.HasRightOnDistrict(CurrentUser, districtId))
            {
                return Json(new { error = "Has no right on the selected district." }, JsonRequestBehavior.AllowGet);
            }
            if (virtualTestId > 0 && !HasRightOnVirtualTestByRole(districtId, isRegrader, virtualTestId))
            {
                return Json(new { error = "Has no right on the selected virtual test." }, JsonRequestBehavior.AllowGet);
            }
            if (teacherId > 0 && !HasRightOnPrimaryTeacher(districtId, isRegrader, teacherId))
            {
                return Json(new { error = "Has no right on the selected teacher." }, JsonRequestBehavior.AllowGet);
            }
            if (classId > 0 && !HasRightOnClassDistrictByRole(districtId, isRegrader, classId))
            {
                return Json(new { error = "Has no right on the selected class." }, JsonRequestBehavior.AllowGet);
            }
            if (studentId > 0 && !HasRightOnStudentDistrictByRole(districtId, isRegrader, studentId))
            {
                return Json(new { error = "Has no right on the selected student." }, JsonRequestBehavior.AllowGet);
            }
            //Use Store
            var data = GetSchoolDistrictFilterByRole(districtId, isRegrader, teacherId, classId, studentId, virtualTestId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetSchoolTestResultDistrict(int? districtId) //Regrader
        {
            if (districtId > 0 && !Util.HasRightOnDistrict(CurrentUser, districtId.GetValueOrDefault()))
            {
                return Json(new { error = "Has no right on the selected district." }, JsonRequestBehavior.AllowGet);
            }

            if (!districtId.HasValue)
                districtId = CurrentUser.DistrictId;

            var data = GetSchoolDistrictFilterByRoleRegrader(districtId.GetValueOrDefault());
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetTermTestResultDistrict(int districtId, int classId, int studentId,
            int virtualTestId, int schoolId, int teacherId, bool isRegrader)
        {
            if (districtId > 0 && !Util.HasRightOnDistrict(CurrentUser, districtId))
            {
                return Json(new { error = "Has no right on the selected district." }, JsonRequestBehavior.AllowGet);
            }
            if (virtualTestId > 0 && !HasRightOnVirtualTestByRole(districtId, isRegrader, virtualTestId))
            {
                return Json(new { error = "Has no right on the selected virtual test." }, JsonRequestBehavior.AllowGet);
            }
            if (teacherId > 0 && !HasRightOnPrimaryTeacher(districtId, isRegrader, teacherId))
            {
                return Json(new { error = "Has no right on the selected teacher." }, JsonRequestBehavior.AllowGet);
            }
            if (studentId > 0 && !HasRightOnStudentDistrictByRole(districtId, isRegrader, studentId))
            {
                return Json(new { error = "Has no right on the selected student." }, JsonRequestBehavior.AllowGet);
            }
            if (schoolId > 0 && !HasRightOnSchoolDistrictFilterByRole(districtId, isRegrader, 0, classId, studentId, virtualTestId,
                        schoolId))
            {
                return Json(new { error = "Has no right on the selected school." }, JsonRequestBehavior.AllowGet);
            }

            //Use store
            var data = GetTermsDistrictFilterByRole(districtId, isRegrader, teacherId, classId, studentId, virtualTestId, schoolId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetTermTestResutForRegader(int? districtId, int? classId, int schoolId)
        {
            if (!districtId.HasValue)
                districtId = CurrentUser.DistrictId;

            //Use store
            var data = parameters.VirtualTestDistrictServices.GetTermsDistrictFilterForRegrader(districtId.GetValueOrDefault(),
                CurrentUser.Id, CurrentUser.RoleId, classId, schoolId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetAllDistrict()
        {
            var data =
                parameters.DistrictStateServices.GetDistricts().Where(o => o.DistrictId == CurrentUser.DistrictId)
                    .Select(o => new ListItem { Id = o.DistrictId, Name = o.DistrictNameCustom });
            if (CurrentUser.IsPublisher())
            {
                data = parameters.DistrictStateServices.GetDistricts().Select(o => new ListItem { Id = o.DistrictId, Name = o.DistrictNameCustom });
            }
            if (CurrentUser.IsNetworkAdmin)
            {
                data = parameters.DistrictStateServices.GetDistricts().Where(x => CurrentUser.GetMemberListDistrictId().Contains(x.DistrictId)).Select(o => new ListItem { Id = o.DistrictId, Name = o.DistrictNameCustom });
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetTestResultsByDistrictId(int Id, bool isRegrader)
        {
            if (!Util.HasRightOnDistrict(CurrentUser, Id))
            {
                return Json(new { error = "Has no right on the selected district." }, JsonRequestBehavior.AllowGet);
            }
            var vData = GetVirtualTestByRole(Id, isRegrader);
            return Json(vData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetTestResultsAuthorByDistrictId(int Id, bool isRegrader)
        {
            if (!Util.HasRightOnDistrict(CurrentUser, Id))
            {
                return Json(new { error = "Has no right on the selected district." }, JsonRequestBehavior.AllowGet);
            }
            //User store [Teacher is a primanry teacher of class, not author test]
            var data = GetPrimaryTeacherDistrictByRole(Id, isRegrader);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetClassByDistrictId(int Id, bool isRegrader)
        {
            if (!Util.HasRightOnDistrict(CurrentUser, Id))
            {
                return Json(new { error = "Has no right on the selected district." }, JsonRequestBehavior.AllowGet);
            }
            //Using Store
            var vData = GetClassDistrictByRole(Id, isRegrader);
            return Json(vData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetStudentByDistrictId(int Id, bool isRegrader)
        {
            if (!Util.HasRightOnDistrict(CurrentUser, Id))
            {
                return Json(new { error = "Has no right on the selected district." }, JsonRequestBehavior.AllowGet);
            }
            //Using Store
            var vData = GetStudentDistrictByRole(Id, isRegrader);
            return Json(vData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTestResultToView(int districtId, int virtualTestId, int classId, string studentName, int schoolId, string teacherName, int termId, bool isRegrader, int timePeriod)
        {
            int isRegrade = 1;

            var parser = new DataTableParserProc<TestResultResultFilterViewModel>();
            int? totalRecords = 0;
            var sortColumns = parser.SortableColumns;
            var testPeriod = GetDateTime(timePeriod).ToShortDateString();
            var searchTerm = Request["sSearch"] ?? string.Empty;

            var testResults = parameters.VirtualTestDistrictServices.GetTestResultToView(districtId, CurrentUser.Id, CurrentUser.RoleId, schoolId, teacherName,
                classId, studentName, virtualTestId, termId, parser.StartIndex, parser.PageSize, ref totalRecords, sortColumns, isRegrade, testPeriod, searchTerm)
                .Select(x => new TestResultResultFilterViewModel()
                {
                    ID = x.TestResultId,
                    TestNameCustom = x.TestName,
                    SchoolName = x.SchoolName,
                    TeacherCustom = x.TeacherCustom,
                    ClassNameCustom = x.ClassNameCustom,
                    StudentCustom = x.StudentCustom,
                    ResultDate = x.ResultDate,
                    StudentID = x.StudentId
                }).AsQueryable();

            return Json(parser.Parse(testResults, totalRecords ?? 0), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult DeleteTestResultAndSubItems(string testresultIds)
        {
            if (parameters.VulnerabilityService.CheckUserCanAccessTestResult(CurrentUser,
                    testresultIds) == false)
            {
                return Json(new { Success = false, Message = "Do not have permission" }, JsonRequestBehavior.AllowGet);
            }

            List<int> lstTestResultIds = new List<int>();
            string[] arr = testresultIds.Split(',');
            if (arr?.Length > 0)
            {
                int item = 0;
                foreach (var s in arr)
                {
                    item = 0;
                    int.TryParse(s, out item);
                    if (item > 0)
                    {
                        lstTestResultIds.Add(item);
                    }
                }
            }
            //Build TestResultAudit
            var testResultAudits = BuildTestResultAudit(lstTestResultIds, ContaintUtil.Remover);

            //Build TestResult
            var strTestresultIds = string.Join(",", lstTestResultIds);
            var testResultLogs = parameters.VirtualTestDistrictServices.GetTestResultDetails(strTestresultIds).ToList();

            //Build TestResultScore
            var testResultScoreLogs = parameters.VirtualTestDistrictServices.GetTestResultScores(strTestresultIds);

            //Build TestResultSubScore
            var testResultScoreIds = testResultScoreLogs.Select(x => x.TestResultScoreID);
            var listTestResultScoreIds = string.Join(",", testResultScoreIds);
            var testResultSubScoreLogs = parameters.VirtualTestDistrictServices.GetTestResultSubScores(listTestResultScoreIds);

            //Build TestResultProgram
            var testResultProgamLogs = parameters.VirtualTestDistrictServices.GetTestResultProgram(strTestresultIds);

            //Build Answer
            var answers = parameters.VirtualTestDistrictServices.GetAnswersByTestResultId(strTestresultIds);

            //Build AnswerSub
            var answerIds = answers.Select(x => x.AnswerID);
            var listAnswerIds = string.Join(",", answerIds);
            var answerSubLogs = parameters.VirtualTestDistrictServices.GetAnswerSubsByAnswerId(listAnswerIds);
            parameters.RubricModuleCommandService.DeleteRubricTestResultScoreWhenDeleteTestResult(lstTestResultIds);
            if (parameters.ClassDistrictServices.DeleteTestResultAndSubItem(lstTestResultIds, CurrentUser.Id))
            {
                try
                {
                    parameters.TestResultLogServices.Save(testResultAudits);
                    parameters.TestResultLogServices.Save(testResultLogs);
                    parameters.TestResultLogServices.Save(testResultScoreLogs);
                    parameters.TestResultLogServices.Save(testResultSubScoreLogs);
                    parameters.TestResultLogServices.Save(testResultProgamLogs);
                    parameters.TestResultLogServices.Save(answers);
                    parameters.TestResultLogServices.Save(answerSubLogs);

                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    PortalAuditManager.LogException(ex);
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        [AjaxOnly]
        public ActionResult DeleteTestResultArtifacts(string testresultIds)
        {
            if (parameters.VulnerabilityService.CheckUserCanAccessTestResult(CurrentUser, testresultIds) == false)
            {
                return Json(new { Success = false, Message = "Do not have permission" }, JsonRequestBehavior.AllowGet);
            }

            var listTestResultIds = testresultIds.ToIntList(",");

            if(listTestResultIds.Count == 0)
            {
                return Json(new { Success = false, Message = "Do not have test results" }, JsonRequestBehavior.AllowGet);
            }

            var result = parameters.ClassDistrictServices.DeleteTestResultArtifacts(listTestResultIds, CurrentUser.Id);
            return Json(new { Success = result }, JsonRequestBehavior.AllowGet);
        }

        private List<TestResultAudit> BuildTestResultAudit(List<int> lstTestResultIds, string type)
        {
            var visitorsIPAddr = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrWhiteSpace(visitorsIPAddr))
            {
                visitorsIPAddr = Request.ServerVariables["REMOTE_ADDR"];
            }

            var testResultAudits = new List<TestResultAudit>();
            foreach (var testResultId in lstTestResultIds)
            {
                //log TestResultAudit
                var testResultAudit = new TestResultAudit()
                {
                    TestResultId = testResultId,
                    AuditDate = DateTime.Now,
                    IPAddress = visitorsIPAddr,
                    UserId = CurrentUser.Id,
                    Type = type
                };
                testResultAudits.Add(testResultAudit);
            }
            return testResultAudits;
        }

        [HttpGet]
        public ActionResult GetUsersByDistrictAndRole(int districtId, int roleId)
        {
            var data = parameters.UserService.GetUsersByDistrictAndRole(districtId, roleId).Where(x => x.UserStatusId == (int)UserStatus.Active).Select(x => new
            {
                Id = x.UserName,
                Name = x.UserName,
                FirstName = x.FirstName,
                LastName = x.LastName
            }).OrderBy(x => x.LastName).ToList();

            if ((roleId == (int)(Permissions.NetworkAdmin) && !parameters.DspDistrictService.HasMemberDistrict(districtId)) || !Util.HasPermissionImpersonateRole(CurrentUser.RoleId, roleId))
            {
                //if district has no " + LabelHelper.DistrictLabel + " member(s) in table DSPDistrict, no display user networkadmin of that district
                data.Clear();
            }

            var jsonResult = Json(new { Data = data });
            var js = new JavaScriptSerializer() { MaxJsonLength = int.MaxValue };
            var jsonStringResult = js.Serialize(jsonResult.Data);
            return Content(jsonStringResult, "application/json");
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.TechUserImpersonation)]
        public ActionResult UserImpersonation()
        {
            var model = new UserImpersonationViewModel();
            model.RoleId = CurrentUser.RoleId;
            return View(model);
        }

        [HttpPost]
        [AjaxOnly]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.TechUserImpersonation)]
        public ActionResult ImpersonateUser(UserImpersonationViewModel model)
        {
            model.SetValidator(new UserImpersonationViewModelValidator());

            if (!model.IsValid)
            {
                return Json(new { ErrorList = model.ValidationErrors, Success = false });
            }
            if (model.RoleId == (int)Permissions.NetworkAdmin)
            {
                var error = model.ValidationErrors.ToList();
                error.Add(new ValidationFailure("MemberDistrictId", "Please select a " + LabelHelper.DistrictLabel + " member."));
                if (!model.MemberDistrictId.HasValue || model.MemberDistrictId == 0)
                {
                    return Json(new { ErrorList = error, Success = false });
                }
            }
            if (model.RoleId == (int)Permissions.SchoolAdmin || model.RoleId == (int)Permissions.Teacher)
            {
                if (model.SchoolID == 0)
                {
                    var error = model.ValidationErrors.ToList();
                    error.Add(new ValidationFailure("SchoolID", "Please select a School."));
                    return Json(new { ErrorList = error, Success = false });
                }
            }
            if (string.IsNullOrEmpty(model.UserName) || model.UserName == "select")
            {
                var error = model.ValidationErrors.ToList();
                error.Add(new ValidationFailure("UserName", "Please select a UserName."));
                return Json(new { ErrorList = error, Success = false });
            }
            var district = parameters.DistrictService.GetDistrictById(model.DistrictId);
            var districtSubDomain = district.IsNull() ? "portal" : district.LICode.ToLower();
            int currentUserId = CurrentUser.Id;//for impersonate log
            var user = parameters.UserService.GetUserByUsernameAndDistrict(model.UserName, model.DistrictId, new List<int> { model.RoleId });
            if (user.IsNull())
            {
                return
                    Json(
                        new
                        {
                            ErrorList =
                            new List<ValidationFailure> { new ValidationFailure("error", "That user does not exist. Please try again.") },
                            Success = false
                        });
            }

            var dspDistrics = new List<int>();
            if (user.IsNetworkAdmin)
            {
                dspDistrics = parameters.DspDistrictService.GetDistrictsByUserId(user.Id);
                if (dspDistrics.IsNotNull() && dspDistrics.Any())
                {
                    //SessionManager.ListDistrictId = dspDistrics;//move setting Session under SignOut and SignIn
                }
            }
            //Impersonate Update, remember original user
            if (CurrentUser.OriginalID == 0)
            {
                user.OriginalID = CurrentUser.Id;
                user.OriginalDistrictId = CurrentUser.DistrictId ?? 0;
                user.OriginalEmailAddress = CurrentUser.EmailAddress;
                user.OriginalRoleId = CurrentUser.RoleId;
                user.OriginalName = CurrentUser.Name;
                user.OriginalStateId = CurrentUser.StateId ?? 0;
                user.OriginalUsername = CurrentUser.UserName;
                user.OriginalNetworkAdminDistrictId = CurrentUser.OriginalNetworkAdminDistrictId;
                var subDomain = Request.Headers["HOST"].Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0];
                user.OriginalDistrictLiCode = subDomain;
                user.ImpersonatedSubdomain = districtSubDomain;
            }
            else
            {
                //Rememebr the original user for the second and more impersonate
                user.OriginalID = CurrentUser.OriginalID;
                user.OriginalDistrictId = CurrentUser.OriginalDistrictId;
                user.OriginalEmailAddress = CurrentUser.OriginalEmailAddress;
                user.OriginalRoleId = CurrentUser.OriginalRoleId;
                user.OriginalName = CurrentUser.OriginalName;
                user.OriginalStateId = CurrentUser.OriginalStateId;
                user.OriginalUsername = CurrentUser.OriginalUsername;
                user.OriginalNetworkAdminDistrictId = CurrentUser.OriginalNetworkAdminDistrictId;
                user.OriginalDistrictLiCode = CurrentUser.OriginalDistrictLiCode;
                user.ImpersonatedSubdomain = CurrentUser.ImpersonatedSubdomain;
            }

            if (!this.CheckPermissionToImpersonate(CurrentUser.RoleId, user.RoleId, user))
            {
                var error = model.ValidationErrors.ToList();
                error.Add(new ValidationFailure("Permission", "You don't have permission to use function."));
                return Json(new { ErrorList = error, Success = false, IsReload = true });
            }

            //Set CurrentUser infomation from User has been Impersonate
            CurrentUser.Id = user.Id;
            CurrentUser.RoleId = user.RoleId;
            CurrentUser.StateId = user.StateId;
            CurrentUser.DistrictId = user.DistrictId;

            //remember SessionCookieGUID for impersonatelog
            if (CurrentUser.SessionCookieGUID == null)
            {
                CurrentUser.SessionCookieGUID = Guid.NewGuid().ToString();
            }
            user.SessionCookieGUID = CurrentUser.SessionCookieGUID;
            user.GUIDSession = CurrentUser.GUIDSession;
            user.ImpersonateLogActivity = ImpersonateLog.ActionTypeEnum.Impersonate;

            parameters.FormsAuthenticationService.SignOut(); //SignOut here will clear all session above, so all setting to Session variable should be done here

            if (!user.IsNull())
                LoadUserMetaData(user);
            parameters.FormsAuthenticationService.SignIn(user, false, true);
            SessionManager.ListDistrictId = dspDistrics; //move setting Session under SignOut and SignIn

            this.parameters.ImpersonateLogService.SaveImpersonateLog(user.SessionCookieGUID,
                                                                     ImpersonateLog.ActionTypeEnum.Impersonate,
                                                                     user.OriginalID, currentUserId, user.Id);

            var redirectUrl = HelperExtensions.GetStartUrlForAuthenticatedUser(HelperExtensions.GetHTTPProtocal(Request), districtSubDomain, user, isImpersonate: true);
            TempData["ListDistrictId"] = dspDistrics;

            return Json(new { Success = true, RedirectUrl = redirectUrl });
        }

        /// <summary>
        /// Check permission user impersonate
        /// </summary>
        /// <param name="currentUserRoleID"></param>
        /// <param name="accessRoleID"></param>
        /// <returns></returns>
        public bool CheckPermissionToImpersonate(int currentUserRoleID, int accessRoleID, User originalUser)
        {
            if (!Util.HasPermissionImpersonateRole(currentUserRoleID, accessRoleID))
            {
                var mailTo = ConfigurationManager.AppSettings["PortalWarningMail"];
                var subject = "Portal Warning - Unauthorized Use";
                var district = parameters.DistrictService.GetDistrictById(originalUser.OriginalDistrictId);
                var ipAddress = Util.GetIPAddressClient(ControllerContext.HttpContext.Request);

                var body = string.Format("An unauthorized user has attempted to use the \"User Impersonation\" function at {0}. See details below. " +
                    "<br>- User name: {1}" +
                    "<br>- Full name: {2}" +
                    "<br>- User ID: {3}" +
                    "<br>- District ID: {4}" +
                    "<br>- District name: {5}" +
                    "<br>- IP address: {6}",
                    DateTime.Now,
                    originalUser.OriginalUsername,
                    originalUser.OriginalName,
                    originalUser.OriginalID,
                    originalUser.OriginalDistrictId,
                    district != null ? district.Name : string.Empty,
                    ipAddress);
                var emailCredentialSetting = LinkitConfigurationManager.GetEmailCredentialSetting(EmailSetting.LinkItUseEmailCredentialKey);
                _emailService.SendPortalWarningEmail(mailTo, subject, body, emailCredentialSetting);

                parameters.FormsAuthenticationService.SignOut();

                return false;
            }
            return true;
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

        private void LoadUserMetaData(User currentUser)
        {
            var userMeta = parameters.UserMetaService.GetByUserId(currentUser.Id, UserMetaLabelConst.NOTIFICATION);
            if (userMeta == null)
            {
                userMeta = new UserMeta
                {
                    UserId = currentUser.Id,
                    MetaLabel = UserMetaLabelConst.NOTIFICATION,
                    UserMetaValue = new UserMetaValue
                    {
                        LatestNotificationClicked = DateTime.MinValue
                    }
                };
                parameters.UserMetaService.Save(userMeta);
            }
            currentUser.UserMetaValue = userMeta.UserMetaValue;
        }

        public PartialViewResult LoadTestRemoverInstruction()
        {
            return PartialView("_TestRemoverInstructions");
        }

        public ActionResult LoadTestRegaderResult(int districtId, int virtualTestId, int classId, string studentName, int schoolId, string teacherName, int termId, int timePeriod)
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
            return PartialView("_TestRegaderResult", model);
        }

        public ActionResult LoadTestResultByFilter(int districtId, int virtualTestId, int classId, string studentName, int schoolId, string teacherName, int termId)
        {
            DisplayTestResultFilterViewModel model = new DisplayTestResultFilterViewModel()
            {
                DistrictId = districtId,
                VirtualTestId = virtualTestId,
                ClassId = classId,
                StudentName = studentName,
                SchoolId = schoolId,
                TeacherName = teacherName,
                TermrId = termId
            };
            return PartialView("_TestResultByFilter", model);
        }

        //\ Test Regrader
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.TmgmtTestregrader)]
        public ActionResult TestRegrader()
        {
            DisplayTestResultFilterViewModel model = new DisplayTestResultFilterViewModel()
            {
                DistrictId = 0,
                VirtualTestId = 0,
                ClassId = 0,
                StudentName = "",
                SchoolId = 0,
                TeacherName = "",
                TermrId = 0,
                TimePeriod = 0
            };
            return View(model);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult RegradeResultAndSubItems(string testresultIds)
        {
            if (!parameters.VulnerabilityService.CheckUserCanAccessTestResult(CurrentUser,
                    testresultIds))
            {
                return Json(new { Success = false, Message = "Do not have permission" }, JsonRequestBehavior.AllowGet);
            }
            List<int> lstTestResultIds = new List<int>();
            string[] arr = testresultIds.Split(',');
            if (arr.Length > 0)
            {
                int item = 0;
                foreach (var s in arr)
                {
                    item = 0;
                    int.TryParse(s, out item);
                    if (item > 0)
                    {
                        lstTestResultIds.Add(item);
                    }
                }
            }
            //check IsExistAutoGradingQueueBeingGraded
            var isExist = parameters.VirtualTestDistrictServices.IsExistAutoGradingQueueBeingGraded(testresultIds);
            if (isExist)
                return Json(new { Success = false, IsExistAutoGradingQueueBeingGraded = true }, JsonRequestBehavior.AllowGet);

            //Build TestResultAudit
            var testResultAudits = BuildTestResultAudit(lstTestResultIds, ContaintUtil.Regrader);

            //Build TestResult
            var strTestresultIds = string.Join(",", lstTestResultIds);
            var testResultLogs = parameters.VirtualTestDistrictServices.GetTestResultDetails(strTestresultIds).ToList();

            //Build TestResultScore
            var testResultScoreLogs = parameters.VirtualTestDistrictServices.GetTestResultScores(strTestresultIds);

            //Build TestResultSubScore
            var testResultScoreIds = testResultScoreLogs.Select(x => x.TestResultScoreID);
            var listTestResultScoreIds = string.Join(",", testResultScoreIds);
            var testResultSubScoreLogs = parameters.VirtualTestDistrictServices.GetTestResultSubScores(listTestResultScoreIds);

            //Build TestResultProgram
            var testResultProgamLogs = parameters.VirtualTestDistrictServices.GetTestResultProgram(strTestresultIds);

            //Build Answer
            var answers = parameters.VirtualTestDistrictServices.GetAnswersByTestResultId(strTestresultIds);

            //Build AnswerSub
            var answerIds = answers.Select(x => x.AnswerID);
            var listAnswerIds = string.Join(",", answerIds);
            var answerSubLogs = parameters.VirtualTestDistrictServices.GetAnswerSubsByAnswerId(listAnswerIds);

            if (parameters.AnswerServices.RegradeAnswerByListTestResult(lstTestResultIds))
            {
                try
                {
                    parameters.TestResultLogServices.Save(testResultAudits);
                    parameters.TestResultLogServices.Save(testResultLogs);
                    parameters.TestResultLogServices.Save(testResultScoreLogs);
                    parameters.TestResultLogServices.Save(testResultSubScoreLogs);
                    parameters.TestResultLogServices.Save(testResultProgamLogs);
                    parameters.TestResultLogServices.Save(answers);
                    parameters.TestResultLogServices.Save(answerSubLogs);

                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    PortalAuditManager.LogException(ex);
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult LoadTestRegraderInstruction()
        {
            return PartialView("_TestRegraderInstructions");
        }

        // PurgeTest
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.TmgmtPurgetest)]
        public ActionResult PurgeTest()
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
            return View(model);
        }

        [HttpGet]
        public ActionResult GetAuthorTestForPurgeByDistrictId(int Id)
        {
            if (Id > 0 && !Util.HasRightOnDistrict(CurrentUser, Id))
            {
                return Json(new { error = "Has no right on district" }, JsonRequestBehavior.AllowGet);
            }
            var data = GetAuthorTest(Id);
            if (data != null)
            {
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetVirtualTestNoResult(int authorId, int districtid)
        {
            if (districtid > 0 && !Util.HasRightOnDistrict(CurrentUser, districtid))
            {
                return Json(new { error = "Has no right on district" }, JsonRequestBehavior.AllowGet);
            }

            var temp = parameters.VirtualTestWithOutTestResultServices.GetVirtualTestWithOutTestResult(authorId, districtid);
            var virtualTestsIncludeRetakeOrigin = temp.Where(x => x.OriginalTestID == null).ToArray();
            var virtualTestsHasRetake = temp.Where(x => x.OriginalTestID.HasValue)
                .GroupBy(x => x.OriginalTestID)
                .Select(x => x.OrderByDescending(y => y.VirtualTestId).FirstOrDefault()).ToArray();
            var virtualTestIDsRetakeOrigin = virtualTestsHasRetake.Select(x => x.OriginalTestID.Value).ToArray();
            var data = virtualTestsIncludeRetakeOrigin
                .Where(x => !virtualTestIDsRetakeOrigin.Contains(x.VirtualTestId))
                .Concat(virtualTestsHasRetake).ToArray();
            IEnumerable<ListItem> results = Enumerable.Empty<ListItem>();

            if (data.Count() > 0 && authorId > 0)
            {
                //check security
                var teachers = GetAuthorTest(districtid);
                if (!teachers.Any(x => x.Id == authorId))
                {
                    return Json(new { error = "Has no right" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    results = data.Select(o => new ListItem { Id = o.VirtualTestId, Name = o.Name });
                }
            }
            if (authorId == 0)
            {
                if (CurrentUser.IsTeacher())
                {
                    results = data.Where(o => o.AuthorUserId == CurrentUser.Id).Select(o => new ListItem { Id = o.VirtualTestId, Name = o.Name });
                }
                else if (CurrentUser.RoleId == (int)Permissions.SchoolAdmin)
                {
                    List<int> lstAuthor =
                        parameters.CustomAuthorTestServices.GetAuthorBySchoolAdmin(CurrentUser.Id).Select(o => o.UserID)
                            .ToList();
                    results = data.Where(o => lstAuthor.Contains(o.AuthorUserId)).Select(o => new ListItem { Id = o.VirtualTestId, Name = o.Name });
                }
                else if (CurrentUser.IsPublisher)
                {
                    results = parameters.VirtualTestWithOutTestResultServices.GetVirtualTestWithOutTestResultForPublisher(districtid);
                }
                else
                {
                    results = data.Select(o => new ListItem { Id = o.VirtualTestId, Name = o.Name });
                }
            }

            return new LargeJsonResult { Data = results, MaxJsonLength = int.MaxValue, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
        public ActionResult ConfirmPurgeTest()
        {
            return PartialView("_PurgeTestConfirm");
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult PurgeTestByTestId(int virtualTestId)
        {
            //check current user has permission to access virtualTest or not
            if (!parameters.VulnerabilityService.CheckUserCanPurgeTest(CurrentUser.Id, CurrentUser.RoleId,
                    virtualTestId))
            {
                return Json(new { Success = false, Message = "Do not have permission" });
            }
            parameters.RubricModuleCommandService.PurgeTestByVirtualTestId(virtualTestId);
            if (parameters.AnswerServices.purgeTestByVirtualTestId(virtualTestId))
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult LoadPurgeInstruction()
        {
            return PartialView("_PurgeInstructions");
        }

        //[AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.TmgmtCustomAssessments)]
        [NonAction]
        public ActionResult CustomAssessments()
        {
            return View();
        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.DadLumos)]
        public ActionResult Lumos()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetDistrictByDictricIds(string dictricIds)
        {
            var ids = ConvertStringIdsToListId(dictricIds);
            var data =
                parameters.DistrictStateServices.GetDistricts().Where(o => ids.Contains(o.DistrictId))
                    .Select(o => new ListItem { Id = o.DistrictId, Name = o.DistrictNameCustom });
            if (CurrentUser.IsPublisher())
            {
                data = parameters.DistrictStateServices.GetDistricts().Select(o => new ListItem { Id = o.DistrictId, Name = o.DistrictNameCustom });
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private List<int> ConvertStringIdsToListId(string strIds)
        {
            var arrIds = strIds.Split(new[] { ',' });//split string id to array id
            return arrIds.Select(int.Parse).ToList();//convert to list id type int
        }

        [HttpPost]
        [AjaxOnly]
        //[AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.TechUserImpersonation)]
        public ActionResult GoBackToOriginalUser(UserImpersonationViewModel model)
        {
            var user = parameters.UserService.GetUserById(CurrentUser.OriginalID);
            var dspDistrics = new List<int>();
            if (CurrentUser.IsOrginalNetworkAdmin)
            {
                dspDistrics = parameters.DspDistrictService.GetDistrictsByUserId(user.Id);
            }
            var originalDistrictLiCode = CurrentUser.OriginalDistrictLiCode;
            user.ImpersonateLogActivity = ImpersonateLog.ActionTypeEnum.GoBack;
            //keep the current SessionCookieGUID for impersonate log
            user.SessionCookieGUID = CurrentUser.SessionCookieGUID;
            user.GUIDSession = CurrentUser.GUIDSession;

            parameters.FormsAuthenticationService.SignOut();//SignOut here will clear all session above,  so all setting to Session variable should be done here

            if (!user.IsNull())
                LoadUserMetaData(user);
            parameters.FormsAuthenticationService.SignIn(user, false, true);
            if (dspDistrics.IsNotNull() && dspDistrics.Any())
            {
                SessionManager.ListDistrictId = dspDistrics;
            }
            this.parameters.ImpersonateLogService.SaveImpersonateLog(user.SessionCookieGUID, ImpersonateLog.ActionTypeEnum.GoBack, null, user.Id, null);

            var district = parameters.DistrictService.GetDistrictById(user.DistrictId ?? 0);
            var districtSubDomain = district.IsNull() ? "portal" : district.LICode.ToLower();
            if (!string.IsNullOrWhiteSpace(originalDistrictLiCode))
            {
                districtSubDomain = originalDistrictLiCode;
            }
            var redirectUrl = HelperExtensions.GetStartUrlForAuthenticatedUser(HelperExtensions.GetHTTPProtocal(Request), districtSubDomain, user);

            return Json(new { Success = true, RedirectUrl = redirectUrl });
        }

        private IQueryable<ListItem> GetVirtualTestByRole(int districtId, bool isRegrader)
        {
            //Using Store
            var vData = parameters.VirtualTestDistrictServices.GetVirtualTestByRole(districtId, CurrentUser.Id, CurrentUser.RoleId, isRegrader);
            return vData;
        }

        private bool HasRightOnVirtualTestByRole(int districtId, bool isRegrader, int virtualTestId)
        {
            var data = GetVirtualTestByRole(districtId, isRegrader);
            return data.Any(x => x.Id == virtualTestId);
        }

        private IQueryable<ListItem> GetPrimaryTeacherDistrictByRole(int districtId, bool isRegrader)
        {
            //User store [Teacher is a primanry teacher of class, not author test]
            var data = parameters.VirtualTestDistrictServices.GetPrimaryTeacherDistrictByRole(districtId, CurrentUser.Id,
                CurrentUser.RoleId, isRegrader);
            return data;
        }

        private bool HasRightOnPrimaryTeacher(int districtId, bool isRegrader, int teacherId)
        {
            //User store [Teacher is a primanry teacher of class, not author test]
            var data = GetPrimaryTeacherDistrictByRole(districtId, isRegrader);
            return data.Any(x => x.Id == teacherId);
        }

        private IQueryable<ListItem> GetClassDistrictByRole(int districtId, bool isRegrader)
        {
            //User store [Teacher is a primanry teacher of class, not author test]
            var data = parameters.VirtualTestDistrictServices.GetClassDistrictByRole(districtId, CurrentUser.Id, CurrentUser.RoleId, isRegrader);
            return data;
        }

        private bool HasRightOnClassDistrictByRole(int districtId, bool isRegrader, int classId)
        {
            //User store [Teacher is a primanry teacher of class, not author test]
            var data = GetClassDistrictByRole(districtId, isRegrader);
            return data.Any(x => x.Id == classId);
        }

        private IQueryable<ListItem> GetStudentDistrictByRole(int districtId, bool isRegrader)
        {
            var vData = parameters.VirtualTestDistrictServices.GetStudentDistrictByRole(districtId, CurrentUser.Id,
                CurrentUser.RoleId, isRegrader);
            return vData;
        }

        private bool HasRightOnStudentDistrictByRole(int districtId, bool isRegrader, int studentId)
        {
            //User store [Teacher is a primanry teacher of class, not author test]
            var data = GetStudentDistrictByRole(districtId, isRegrader);
            return data.Any(x => x.Id == studentId);
        }

        private IQueryable<ListItem> GetSchoolDistrictFilterByRoleRegrader(int districtId)
        {
            var data = parameters.VirtualTestDistrictServices.GetSchoolDistrictFilterByRoleRegrader(districtId, CurrentUser.Id,
             CurrentUser.RoleId);
            return data;
        }

        private IQueryable<ListItem> GetSchoolDistrictFilterByRole(int districtId, bool isRegrader, int teacherId, int classId, int studentId, int virtualTestId)
        {
            var data = parameters.VirtualTestDistrictServices.GetSchoolDistrictFilterByRole(districtId, CurrentUser.Id,
             CurrentUser.RoleId, isRegrader, teacherId, classId, studentId, virtualTestId);
            return data;
        }

        private bool HasRightOnSchoolDistrictFilterByRole(int districtId, bool isRegrader, int teacherId, int classId,
            int studentId, int virtualTestId, int schoolId)
        {
            var data = GetSchoolDistrictFilterByRole(districtId, isRegrader, teacherId, classId, studentId,
                virtualTestId);
            return data.Any(x => x.Id == schoolId);
        }

        private IQueryable<ListItem> GetTermsDistrictFilterByRole(int districtId, bool isRegrader, int teacherId,
            int classId, int studentId, int virtualTestId, int schoolId)
        {
            var data = parameters.VirtualTestDistrictServices.GetTermsDistrictFilterByRole(districtId, CurrentUser.Id, CurrentUser.RoleId, virtualTestId, studentId, classId, schoolId, teacherId, isRegrader);
            return data;
        }

        private bool HasRightOnTermsDistrictFilterByRole(int districtId, bool isRegrader, int teacherId,
            int classId, int studentId, int virtualTestId, int schoolId, int termId)
        {
            var data = GetTermsDistrictFilterByRole(districtId, isRegrader, teacherId, classId, studentId, virtualTestId,
                schoolId);
            return data.Any(x => x.Id == termId);
        }

        private List<ListItem> GetAuthorTest(int districtId)
        {
            if (CurrentUser.RoleId == (int)Permissions.SchoolAdmin)
            {
                var data = parameters.CustomAuthorTestServices.GetAuthorBySchoolAdmin(CurrentUser.Id);
                if (data.Any())
                {
                    var temp = data.Select(o => new ListItem { Id = o.UserID, Name = o.NameLast + ", " + o.NameFirst + " (" + o.UserName + ")" });
                    return temp.ToList();
                }
            }
            else
            {
                var tmp = parameters.AuthorTestWithoutTestResultServices.GetAuthorTestWithoutTestResult(districtId, CurrentUser.RoleId, CurrentUser.Id);
                if (tmp.Any())
                {
                    //miss case SchoolAdmin
                    var data = tmp.Select(o => new ListItem { Id = o.UserId, Name = o.NameLast + ", " + o.NameFirst + " (" + o.UserName + ")" }).OrderBy(o => o.Name);
                    return data.ToList();
                }
            }
            return new List<ListItem>();
        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.DadManageLAC)]
        public ActionResult ManageLAC()
        {
            var obj = new TestDataUploadViewModel()
            {
                IsPublisherUploading = CurrentUser.IsPublisher(),
            };
            var achievementLevelIds = new List<Select2ListItem>();
            var lstArchievementLevelIds = parameters.AchievementLevelSettingService.GetAllAchievementLevelSettings();
            if (lstArchievementLevelIds.Any())
            {
                achievementLevelIds = lstArchievementLevelIds.Select(o => new Select2ListItem()
                {
                    Id = o.AchievementLevelSettingID.ToString(),
                    Text = o.Name + " - " + o.AchievementLevelSettingID
                }).OrderBy(o => o.Text).ToList();
            }
            if (CurrentUser.IsNetworkAdmin)
            {
                ViewBag.IsNetworkAdmin = true;
            }
            string strListLACImportTypes = ConfigurationManager.AppSettings["LACSupportDataPBS"]?.ToString();
            obj.ListAttendance = _districtDecodeService.GetAllDistrictSuportAttendance(strListLACImportTypes);
            obj.DistrictId = CurrentUser.DistrictId.GetValueOrDefault();
            ViewBag.AchievementLevelIds = Newtonsoft.Json.JsonConvert.SerializeObject(achievementLevelIds);
            return View(obj);
        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.DadManageLAC)]
        public ActionResult UploadTestData(HttpPostedFileBase postedFile, int? stateId, int? districtId, int? achievementLevelId, int? categoryId, bool userProgram, string listProgramIds, bool? isExcludeSubject, bool isNotCreateNewStudent, bool isApplyPerformanceBankds)
        {
            if (!IsValidPostedFile(postedFile))
            {
                return Json(new { message = "Invalid file, please try again.", success = false, type = "error" }, JsonRequestBehavior.AllowGet);
            }
            if (achievementLevelId <= 0)
            {
                return Json(new { message = "Invalid request type, please try again.", success = false, type = "error" }, JsonRequestBehavior.AllowGet);
            }

            if (CurrentUser.IsPublisher)
            {
                CurrentUser.StateId = stateId;
                CurrentUser.DistrictId = districtId;
            }

            //Check Attendance Category
            //Check DistrictSupport Attendance via DistrictDataParam. ( DistrictID & ImportType='Attendance' )
            string strImportTypes = ConfigurationManager.AppSettings["LACSupportDataPBS"] == null ? string.Empty : ConfigurationManager.AppSettings["LACSupportDataPBS"].ToString();
            var objDistrictSupportDLPBS = _districtDecodeService.GetDistrictSuportAttendanceGradeByDistrictId(categoryId.GetValueOrDefault(), CurrentUser.DistrictId.GetValueOrDefault(), strImportTypes);
            string strImportType = string.Empty;
            if (objDistrictSupportDLPBS.Count > 0)
            {
                achievementLevelId = null;
                userProgram = false;
                listProgramIds = string.Empty;
                isExcludeSubject = true;
                strImportType = objDistrictSupportDLPBS[0].ImportType;
            }
            return SaveRequestAndUploadFileForTestData(postedFile, achievementLevelId, categoryId, userProgram, listProgramIds, isExcludeSubject, isNotCreateNewStudent, strImportType, isApplyPerformanceBankds);
        }

        private ActionResult SaveRequestAndUploadFileForTestData(HttpPostedFileBase postedFile, int? achievementLevelId, int? categoryId, bool userProgram, string listProgramIds, bool? isExcludeSubject, bool isNotCreateNewStudent, string strImportType, bool isApplyPerformanceBankds)
        {
            var request = parameters.RequestService.CreateRequestForTestDataUpload(CurrentUser, postedFile.FileName);
            request.SetValidator(parameters.RequestValidator);
            if (!request.IsValid)
            {
                return Json(new
                {
                    ErrorList = request.ValidationErrors,
                    message = "An error has occured, please try again.",
                    success = false,
                    type = "error"
                }, JsonRequestBehavior.AllowGet);
            }

            parameters.RequestService.Insert(request);
            CreateRequestParameter(request, "FileName", postedFile.FileName);
            CreateRequestParameter(request, "LoadTestData", "true");
            if (isExcludeSubject.HasValue && isExcludeSubject.Value)
            {
                CreateRequestParameter(request, "OmitSubjectFromTestName", "true");
            }

            if (!string.IsNullOrEmpty(listProgramIds))
            {
                CreateRequestParameter(request, "rosterprogramdata", listProgramIds);
            }

            if (userProgram)
            {
                CreateRequestParameter(request, "useprograms", "true");
            }
            else
            {
                CreateRequestParameter(request, "useprograms", "false");
            }
            if (achievementLevelId.HasValue)
            {
                CreateRequestParameter(request, "achievelevelsetID", achievementLevelId.ToString());
            }
            if (categoryId.HasValue)
            {
                CreateRequestParameter(request, "categoryId", categoryId.ToString());
            }
            if (!string.IsNullOrEmpty(strImportType))
            {
                CreateRequestParameter(request, "importtype", strImportType);
            }

            CreateRequestParameter(request, "DoNotCreateNewStudent", isNotCreateNewStudent.ToString().ToLower());
            CreateRequestParameter(request, "ApplyPerformanceBankds", isApplyPerformanceBankds.ToString().ToLower());

            var uploadFilePath = AssignUploadFilePath(request);
            //Upload correct folder by Identifier
            var envID = LinkitConfigurationManager.Vault.DatabaseID;
            if (!string.IsNullOrEmpty(envID))
            {
                uploadFilePath = string.Format("{0}\\{1}", uploadFilePath, envID);
            }
            parameters.RosterUploadService.UploadTestDataFile(postedFile, request, uploadFilePath);
            return Json(new { success = true });
        }

        [HttpGet]
        public ActionResult GetUsersInSchool(int schoolId, int roleId)
        {
            var validUserSchoolRoleId = new List<int>();
            validUserSchoolRoleId.Add(roleId);
            //The users don't need to have class
            var data = parameters.UserSchoolService.GetSchoolsUserBySchoolId(schoolId).Where(x => validUserSchoolRoleId.Contains((int)(x.Role)))
                .Select(x => new //Should return data as GetUsersByDistrictAndRole
                {
                    Name = x.UserName,
                    x.FirstName,
                    x.LastName,
                    x.DisplayName,
                    Id = x.UserName
                }).OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ToList();

            var jsonResult = Json(new { Data = data });
            var js = new JavaScriptSerializer() { MaxJsonLength = int.MaxValue };
            var jsonStringResult = js.Serialize(jsonResult.Data);
            return Content(jsonStringResult, "application/json");
        }

        [HttpGet]
        public ActionResult GetProgramLAC(int districtId)
        {
            if (!CurrentUser.IsNetworkAdmin && !CurrentUser.IsPublisher)
            {
                districtId = CurrentUser.DistrictId.GetValueOrDefault();
            }
            var lst = parameters.ProgramService.GetProgramInStudentProgramByDistrictId(districtId);
            var jsonResult = Json(new { Data = lst });
            var js = new JavaScriptSerializer() { MaxJsonLength = int.MaxValue };
            var jsonStringResult = js.Serialize(jsonResult.Data);
            return Content(jsonStringResult, "application/json");
        }

        public DateTime GetDateTime(int days)
        {
            if (days == 0) return DateTime.MinValue.AddYears(1800);
            return DateTime.UtcNow.AddDays(-days).Date;
        }

        #region Manage user group

        [HttpGet, AdminOnly(Order = 3)]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.DadManageUserGroup)]
        public ActionResult ManageUserGroups(bool isNewSession = false)
        {
            var model = new ManageUserGroupViewModel()
            {
                CurrentUserId = CurrentUser.Id,
                IsPublisher = CurrentUser.IsPublisher,
                IsNetworkAdmin = CurrentUser.IsNetworkAdmin,
                DistrictID = CurrentUser.DistrictId
            };
            ViewBag.IsNewSession = isNewSession;
            return View(model);
        }

        [HttpGet]
        public ActionResult GetXLIGroups(int? districtId)
        {
            List<ListItem> data = null;

            int currentDistrictID = districtId.HasValue ? districtId.Value : CurrentUser.DistrictId.Value;
            var groups = parameters.XLIGroupService.GetGroupByDistrict(currentDistrictID).OrderBy(x => x.Name).ToList();
            data = groups.Select(x => new ListItem { Id = x.XLIGroupID, Name = x.Name }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public JsonResult RemoveUserFromGroup(int userId, int groupId)
        {
            if (CurrentUser.IsPublisher || CurrentUser.IsNetworkAdmin || CurrentUser.IsDistrictAdmin)
            {
                parameters.XLIGroupService.RemoveUserFromGroup(userId, groupId);
                return Json(true);
            }
            return Json(false);
        }

        [HttpPost]
        [AjaxOnly]
        public JsonResult GetGroupUsers(GetGroupUserDataTableCriteria criteria)
        {
            var result = new GenericDataTableResponse<UserManage>()
            {
                sEcho = criteria.sEcho,
                sColumns = criteria.sColumns,
                aaData = new List<UserManage>(),
                iTotalDisplayRecords = 0,
                iTotalRecords = 0
            };

            var request = new GetGroupUserRequest();
            MappingGetGroupUserRequest(criteria, request);

            var response = parameters.UserSchoolService.GetUserManageByRoleInGroup(request);
            result.aaData = response.Data;
            result.iTotalDisplayRecords = response.TotalRecord;
            result.iTotalRecords = response.TotalRecord;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, AdminOnly(Order = 3)]
        public ActionResult GetAvailebleUsersToAddGroupView(int groupID)
        {
            var xliGroup = this.parameters.XLIGroupService.GetGroupByID(groupID);
            return PartialView("_AddUserToGroup", xliGroup);
        }

        [HttpPost]
        [AjaxOnly]
        public JsonResult GetUsersAddToGroup(GetUserAddToGroupDataTableCriteria criteria)
        {
            var result = new GenericDataTableResponse<UserManage>()
            {
                sEcho = criteria.sEcho,
                sColumns = criteria.sColumns,
                aaData = new List<UserManage>(),
                iTotalDisplayRecords = 0,
                iTotalRecords = 0
            };

            var request = new GetUserForAddGroupRequest();
            MappingGetUserAddToGroupRequest(criteria, request);

            var response = parameters.UserSchoolService.GetUserAvailableForAddGroup(request);
            result.aaData = response.Data;
            result.iTotalDisplayRecords = response.TotalRecord;
            result.iTotalRecords = response.TotalRecord;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AjaxOnly]
        public JsonResult GetManageRolesByCurrentUser(int xliGroupID)
        {
            var roleItems = parameters.UserSchoolService.GetRolesForManageUserGroup(CurrentUser.RoleId, xliGroupID);

            return Json(roleItems, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public JsonResult AddUsersToGroup(string userIDs, int groupID)
        {
            string[] arr = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(userIDs);
            string users = string.Join(";", arr);
            parameters.XLIGroupService.AddUsersToGroup(users, groupID);
            return Json(true);
        }

        [HttpGet]
        [AjaxOnly]
        public JsonResult GetSchoolsManageByCurrentUser(int? districtID)
        {
            int? criteriaDistrictID = CurrentUser.IsPublisher || CurrentUser.IsNetworkAdmin ? districtID : CurrentUser.DistrictId;
            var data = new List<ListItem>();
            if (CurrentUser.IsDistrictAdminOrPublisher)
            {
                data = parameters.SchoolService.GetSchoolsByDistrictId(criteriaDistrictID.GetValueOrDefault()).Select(
                        x => new ListItem { Id = x.Id, Name = x.Name }).ToList();
            }
            else
            {
                data = parameters.UserSchoolService.GetSchoolsUserHasAccessTo(CurrentUser.Id).Select(
                   x => new ListItem { Id = x.SchoolId.GetValueOrDefault(), Name = x.SchoolName })
                   .ToList();
            }

            data = data.OrderBy(x => x.Name).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private void MappingGetUserAddToGroupRequest(GetUserAddToGroupDataTableCriteria criteria, GetUserForAddGroupRequest request)
        {
            request.PageSize = criteria.iDisplayLength;
            request.StartRow = criteria.iDisplayStart;
            request.IsShowInactiveUser = criteria.IsShowInactiveUser;
            request.GeneralSearch = criteria.sSearch;
            request.XLIGroupID = criteria.GroupID;

            if (!string.IsNullOrWhiteSpace(criteria.sColumns) && criteria.iSortCol_0.HasValue)
            {
                var columns = criteria.sColumns.Split(',');
                request.SortColumn = columns[criteria.iSortCol_0.Value];
                request.SortDirection = criteria.sSortDir_0.Equals("desc") ? "DESC" : "ASC";
            }

            request.DistrictID = CurrentUser.IsPublisher || CurrentUser.IsNetworkAdmin ? criteria.DistrictID : CurrentUser.DistrictId;

            request.UserID = CurrentUser.Id;
            request.UserRoleID = CurrentUser.RoleId;
            request.SchoolID = criteria.SchoolID;
            request.RoleID = criteria.RoleID;
        }

        private void MappingGetGroupUserRequest(GetGroupUserDataTableCriteria criteria, GetGroupUserRequest request)
        {
            request.PageSize = criteria.iDisplayLength;
            request.StartRow = criteria.iDisplayStart;
            request.GeneralSearch = criteria.sSearch;
            request.IsShowInactiveUser = criteria.IsShowInactiveUser;

            if (!string.IsNullOrWhiteSpace(criteria.sColumns) && criteria.iSortCol_0.HasValue)
            {
                var columns = criteria.sColumns.Split(',');
                request.SortColumn = columns[criteria.iSortCol_0.Value];
                request.SortDirection = criteria.sSortDir_0.Equals("desc") ? "DESC" : "ASC";
            }

            request.DistrictID = CurrentUser.IsPublisher || CurrentUser.IsNetworkAdmin ? criteria.DistrictID : CurrentUser.DistrictId;
            if (request.DistrictID.HasValue && request.DistrictID.Value < 0)
                request.DistrictID = null;
            if (criteria.XLIGroupID.HasValue && criteria.XLIGroupID.Value > 0)
                request.XLIGroupID = criteria.XLIGroupID;
            request.UserID = CurrentUser.Id;
            request.RoleID = CurrentUser.RoleId;
        }

        #endregion Manage user group
    }
}
