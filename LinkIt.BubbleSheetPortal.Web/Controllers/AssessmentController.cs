using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using Envoc.Core.Shared.Extensions;
using FluentValidation;
using FluentValidation.Results;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels.AuthorGroup;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.ReadOnly)]
    [AjaxAwareAuthorize(Order = 2)]
    [VersionFilter]
    public class AssessmentController : BaseController
    {
        private readonly UserService userService;
        private readonly AuthorGroupService authorGroupService;
        private readonly StateService stateService;
        private readonly IValidator<AddAuthorGroupViewModel> addAuthorGroupViewModelValidator;
        private readonly SchoolService schoolService;
        private readonly DistrictService districtService;
        private readonly UserSchoolService userSchoolService;
        private readonly VulnerabilityService vulnerabilityService;

        public AssessmentController(UserService userService, AuthorGroupService authorGroupService,
            StateService stateService, IValidator<AddAuthorGroupViewModel> addAuthorGroupViewModelValidator, SchoolService schoolService, DistrictService districtService,
            UserSchoolService userSchoolService, VulnerabilityService vulnerabilityService)
        {
            this.userService = userService;
            this.authorGroupService = authorGroupService;
            this.stateService = stateService;
            this.addAuthorGroupViewModelValidator = addAuthorGroupViewModelValidator;
            this.schoolService = schoolService;
            this.districtService = districtService;
            this.userSchoolService = userSchoolService;
            this.vulnerabilityService = vulnerabilityService;
        }

        public ActionResult Index()
        {
            return View();
        }

        #region Author Group

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.TestdesignManageAuthorGroup)] 
        public ActionResult AuthorGroupList()
        {
            var user = userService.GetUserById(CurrentUser.Id);
            if (user != null)
            {
                CurrentUser.SchoolId = user.SchoolId;
            }
            var model = new AuthorGroupListViewModel
                        {
                            IsDistrictAdmin = CurrentUser.RoleId.Equals((int)Permissions.DistrictAdmin),
                            IsSchoolAdmin = CurrentUser.IsSchoolAdmin,
                            IsPublisher = CurrentUser.IsPublisher,
                            IsTeacher = CurrentUser.IsTeacher,
                            DistrictId = CurrentUser.DistrictId.GetValueOrDefault(),
                            StateId = CurrentUser.StateId.GetValueOrDefault(),
                            SchoolId = CurrentUser.SchoolId.GetValueOrDefault(),
                            CurrentUserId = CurrentUser.Id,
                            IsNetworkAdmin = CurrentUser.IsNetworkAdmin,
                            ListDistricIds = CurrentUser.IsNetworkAdmin ? CurrentUser.GetMemberListDistrictId() : null
                        };
            return View(model);
        }

        [HttpGet, AjaxOnly]
        public ActionResult GetAuthorGroupListHasAccessTo(int stateId, int districtId, int schoolId)
        {
            if (districtId <= 0 && CurrentUser.IsPublisher == false && CurrentUser.IsNetworkAdmin==false)
            {
                districtId = CurrentUser.DistrictId.GetValueOrDefault();
            }

            var authorGroups = authorGroupService.GetAuthorGroupListHasAccessTo(CurrentUser.Id, stateId, districtId, schoolId);
            var data = authorGroups.Select(x => new AuthorGroupListDataViewModel
                                                {
                                                    AuthorGroupId = x.AuthorGroupId,
                                                    DistrictList = Server.HtmlEncode(x.DistrictName ?? string.Empty),
                                                    SchoolList = Server.HtmlEncode(x.SchoolName ?? string.Empty),
                                                    UserList = Server.HtmlEncode(x.UserNameList ?? string.Empty),
                                                    Name = Server.HtmlEncode(x.Name ?? string.Empty),
                                                    UserId = x.UserId
                                                });
            var parser = new DataTableParser<AuthorGroupListDataViewModel>();
            return Json(parser.Parse(data), JsonRequestBehavior.AllowGet);
        }

        [HttpGet, AjaxOnly]
        public ActionResult GetAuthorGroupList(int stateId, int districtId, int schoolId)
        {
            if (districtId <= 0 && CurrentUser.IsPublisher == false)
            {
                districtId = CurrentUser.DistrictId.GetValueOrDefault();
            }

            var authorGroups = authorGroupService.GetAuthorGroupList(stateId, districtId, schoolId);
            var data = authorGroups.Select(x => new AuthorGroupListDataViewModel
            {
                AuthorGroupId = x.AuthorGroupId,
                DistrictList = x.DistrictName ?? string.Empty,
                SchoolList = x.SchoolName ?? string.Empty,
                UserList = x.UserNameList ?? string.Empty,
                Name = x.Name ?? string.Empty,
                UserId = x.UserId
            });
            var parser = new DataTableParser<AuthorGroupListDataViewModel>();
            return Json(parser.Parse(data), JsonRequestBehavior.AllowGet);
        }

        [HttpGet, AjaxOnly]
        public ActionResult GetAuthorGroupUsers(int authorGroupId)
        {
            var userList = userService.GetUserByAuthorGroupId(authorGroupId);
            var data = userList.Select(x => new AuthorGroupUserListViewModel
                                            {
                                                UserId = x.Id,
                                                FirstName = x.FirstName,
                                                LastName = x.LastName,
                                                UserName = x.UserName
                                            });

            var parser = new DataTableParser<AuthorGroupUserListViewModel>();
            return Json(parser.Parse(data), JsonRequestBehavior.AllowGet);
        }

        [HttpGet, AjaxOnly]
        public ActionResult GetAuthorGroupDistricts(int authorGroupId)
        {
            var districtList = districtService.GetDistrictsByAuthorGroupId(authorGroupId);
            if(CurrentUser.IsNetworkAdmin)
            {
                districtList = districtList.Where(x => CurrentUser.GetMemberListDistrictId().Contains(x.Id));
            }
            var data = districtList.Select(x => new AuthorGroupDistrictListViewModel
            {
                DistrictId = x.Id,
                DistrictName = x.Name
            });

            var parser = new DataTableParser<AuthorGroupDistrictListViewModel>();
            return Json(parser.Parse(data), JsonRequestBehavior.AllowGet);
        }

        [HttpGet, AjaxOnly]
        public ActionResult GetAuthorGroupSchools(int authorGroupId)
        {
            var schoolList = schoolService.GetSchoolByAuthorGroupId(authorGroupId);
            var data = schoolList.Select(x => new AuthorGroupSchoolListViewModel
                                              {
                                                  SchoolId = x.Id,
                                                  SchoolName = x.Name
                                              });

            var parser = new DataTableParser<AuthorGroupSchoolListViewModel>();
            return Json(parser.Parse(data), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ManageAuthorGroupUser(int authorGroupId)
        {            
            return PartialView("_ManageAuthorGroupUser",authorGroupId);
        }

        public ActionResult LoadListAuthorGroupUser(int authorGroupId)
        {
            //TODO: need to validate authorGroupId with current user
            var viewModel = BuildAddAuthorGroupUserSchoolDistrictViewModel(authorGroupId);
            return PartialView("_ListAuthorGroupUser", viewModel);
        }

        public ActionResult LoadListAuthorGroupUserList(int authorGroupId)
        {
            //TODO: need to validate authorGroupId with current user
            var viewModel = BuildAddAuthorGroupUserSchoolDistrictViewModel(authorGroupId);
            return PartialView("_ListAuthorGroupUserEditPage", viewModel);
        }

        public ActionResult LoadListAuthorGroupSchool(int authorGroupId)
        {
            //TODO: need to validate authorGroupId with current user
            var viewModel = BuildAddAuthorGroupUserSchoolDistrictViewModel(authorGroupId);
            return PartialView("_ListAuthorGroupSchool", viewModel);
        }

        public ActionResult LoadListAuthorGroupSchoolList(int authorGroupId)
        {
            //TODO: need to validate authorGroupId with current user
            var viewModel = BuildAddAuthorGroupUserSchoolDistrictViewModel(authorGroupId);
            return PartialView("_ListAuthorGroupSchoolEditPage", viewModel);
        }

        public ActionResult LoadListAuthorGroupDistrict(int authorGroupId)
        {
            //TODO: need to validate authorGroupId with current user
            var viewModel = BuildAddAuthorGroupUserSchoolDistrictViewModel(authorGroupId);
            return PartialView("_ListAuthorGroupDistrict", viewModel);
        }

        public ActionResult LoadListAuthorGroupDistrictList(int authorGroupId)
        {
            //TODO: need to validate authorGroupId with current user
            var viewModel = BuildAddAuthorGroupUserSchoolDistrictViewModel(authorGroupId);
            return PartialView("_ListAuthorGroupDistrictEditPage", viewModel);
        }

        [HttpGet]
        public ActionResult ManageAuthorGroupSchool(int authorGroupId)
        {            
            return PartialView("_ManageAuthorGroupSchool",authorGroupId);

            //if (CurrentUser.IsDistrictAdminOrPublisher)
            //{
            //    var viewModel = BuildAddAuthorGroupUserSchoolDistrictViewModel(authorGroupId);
            //    viewModel.GroupName = HttpUtility.UrlDecode(authorGroupName);
            //    return PartialView(viewModel);
            //}
            //return RedirectToAction("AuthorGroupList");
        }

        [HttpGet]
        public ActionResult ManageAuthorGroupDistrict(int authorGroupId,string authorGroupName)
        {
            return PartialView("_ManageAuthorGroupDistrict", authorGroupId);

            //var viewModel = BuildAddAuthorGroupUserSchoolDistrictViewModel(authorGroupId);
            //if (viewModel.IsDistrictAdmin || viewModel.IsPublisher || viewModel.IsSchoolAdmin)
            //{
            //    viewModel.GroupName = HttpUtility.UrlDecode(authorGroupName);
            //    return View(viewModel);
            //}
            //else
            //{
            //    return RedirectToAction("AuthorGroupList");
            //}
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult RemoveUserFromAuthorGroup(int authorGroupId, int userId)
        {
            if (!vulnerabilityService.HasRigtToAccessAuthorGroup(CurrentUser, authorGroupId))
            {
                return Json(new { Success = false,error="Has no right to work with this author group" });
            }
            if (!vulnerabilityService.HasRightToAcessUser(CurrentUser, userId, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(new { Success = false, error = "Has no right to remove this user from the group." });
            }

            authorGroupService.DeleteAuthorGroupUser(authorGroupId, userId);
            return Json(new {Success = true});
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult RemoveDistrictFromAuthorGroup(int authorGroupId, int districtId)
        {
            if (!vulnerabilityService.HasRigtToAccessAuthorGroup(CurrentUser, authorGroupId))
            {
                return Json(new { Success = false, error = "Has no right to work with this author group" });
            }
            if (!Util.HasRightOnDistrict(CurrentUser, districtId))
            {
                return Json(new { Success = false, error = "Has no right to remove this " + LabelHelper.DistrictLabel + " from the group." });
            }
            authorGroupService.DeleteAuthorGroupDistrict(authorGroupId, districtId);
            return Json(new {Success = true});
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult RemoveSchoolFromAuthorGroup(int authorGroupId, int schoolId)
        {
            if (!vulnerabilityService.HasRigtToAccessAuthorGroup(CurrentUser, authorGroupId))
            {
                return Json(new { Success = false, error = "Has no right to work with this author group" });
            }
            if (!vulnerabilityService.CheckUserCanAccessSchool(CurrentUser, schoolId))
            {
                return Json(new { Success = false, error = "Has no right to remove this school from the group." });
            }

            authorGroupService.DeleteAuthorGroupSchool(authorGroupId, schoolId);
            return Json(new {Success = true});
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult DeleteAuthorGroup(int authorGroupId)
        {
            bool hasPermissionToDelete = false;
            var authorGroup = authorGroupService.GetAuthorGroupById(authorGroupId);

            if (authorGroup == null)
            {
                return Json(false);
            }
            
            if (CurrentUser.IsPublisher)
            {
                hasPermissionToDelete = true;
            }
            else if (authorGroup.UserId == CurrentUser.Id)
            {
                hasPermissionToDelete = true;
            }
            else if (authorGroupService.IsUserInAuthorGroup(CurrentUser.Id, authorGroupId))
            {
                hasPermissionToDelete = true;
            }
            
            if (hasPermissionToDelete)
            {
                authorGroupService.DeleteAuthorGroup(authorGroupId);
                return Json(true);
            }
            return Json(false);

        }

        [HttpGet]
        public ActionResult GetUserForAuthorGroup(int authorGroupId)
        {
            var viewModel = BuildAddAuthorGroupUserSchoolDistrictViewModel(authorGroupId);
            
            return PartialView("_AddAuthorGroupUser", viewModel);
        }

        private AddAuthorGroupUserSchoolDistrictViewModel BuildAddAuthorGroupUserSchoolDistrictViewModel(int authorGroupId)
        {
            var viewModel = new AddAuthorGroupUserSchoolDistrictViewModel
            {
                IsDistrictAdmin = CurrentUser.RoleId.Equals((int)Permissions.DistrictAdmin),
                IsSchoolAdmin = CurrentUser.IsSchoolAdmin,
                IsPublisher = CurrentUser.IsPublisher,
                IsTeacher = CurrentUser.IsTeacher,
                DistrictId = CurrentUser.DistrictId.GetValueOrDefault(),
                StateId = CurrentUser.StateId.GetValueOrDefault(),
                AuthorGroupId = authorGroupId,
                SchoolId = CurrentUser.SchoolId.GetValueOrDefault(),
                //CanEditGroup = IsCanEditGroup(authorGroupId)
                CanEditGroup = true,
                IsNetworkAdmin = CurrentUser.IsNetworkAdmin
            };
            return viewModel;
        }

        [HttpGet]
        public ActionResult GetDistrictForAuthorGroup(int authorGroupId)
        {
            var viewModel = BuildAddAuthorGroupUserSchoolDistrictViewModel(authorGroupId);
            return PartialView("_AddAuthorGroupDistrict", viewModel);
        }

        [HttpGet]
        public ActionResult GetSchoolForAuthorGroup(int authorGroupId)
        {
            var viewModel = BuildAddAuthorGroupUserSchoolDistrictViewModel(authorGroupId);
            return PartialView("_AddAuthorGroupSchool", viewModel);
        }

        [HttpGet]
        public ActionResult GetUserList(int stateId, int districtId, int schoolId, int authorGroupId)
        {
            IQueryable<User> userList;

            if (CurrentUser.IsSchoolAdmin || CurrentUser.IsTeacher)
            {
                userList = authorGroupService.GetUsersUserHasAccessTo(CurrentUser.Id, stateId, districtId, schoolId).Where(en => en.UserStatusId == 1);
            }
            else
            {
                userList = stateId > 0
                               ? userService.GetActiveUserByStateIdAndDistrictIdAndSchoolId(stateId, districtId,
                                                                                            schoolId)
                               : new EnumerableQuery<User>(new List<User>());
            }

            var existedUserList = userService.GetUserByAuthorGroupId(authorGroupId).Select(x => x.Id).ToList();

            if (existedUserList.Count > 0)
            {
                userList = userList.Where(u => existedUserList.Contains(u.Id) == false);
            }

            var data = userList.Select(x => new AuthorGroupUserListViewModel
                                            {
                                                UserId = x.Id,
                                                FirstName = x.FirstName,
                                                LastName = x.LastName,
                                                UserName = x.UserName
                                            });
            var parser = new DataTableParser<AuthorGroupUserListViewModel>();
            return Json(parser.Parse(data), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetDistrictList(int stateId, int authorGroupId)
        {
            IQueryable<District> districtList = new EnumerableQuery<District>(new List<District>());
            if (CurrentUser.IsPublisher)
            {
                districtList = stateId > 0
                    ? districtService.GetDistrictsByStateId(stateId)
                    : new EnumerableQuery<District>(new List<District>());
            }
            else if(CurrentUser.IsNetworkAdmin)
            {
                districtList = stateId > 0
                    ? districtService.GetDistrictsByStateId(stateId)
                    : new EnumerableQuery<District>(new List<District>());
                districtList = districtList.Where(x => CurrentUser.GetMemberListDistrictId().Contains(x.Id));
            }
            else if (CurrentUser.RoleId.Equals((int) Permissions.DistrictAdmin))
            {
                var district = districtService.GetDistrictById(CurrentUser.DistrictId.GetValueOrDefault());
                if (district != null)
                {
                    var listDistrict = new List<District> {district};
                    districtList  = new EnumerableQuery<District>(listDistrict);
                }
            }

            var existedDistrictList =
                districtService.GetDistrictsByAuthorGroupId(authorGroupId).Select(x => x.Id).ToList();

            if (existedDistrictList.Count > 0)
            {
                districtList = districtList.Where(u => existedDistrictList.Contains(u.Id) == false);
            }

            var data = districtList.Select(x => new AuthorGroupDistrictListViewModel
            {
                DistrictId = x.Id,
                DistrictName = x.Name
            });
            var parser = new DataTableParser<AuthorGroupDistrictListViewModel>();
            return Json(parser.Parse(data), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetSchoolList(int districtId, int authorGroupId)
        {            
            IQueryable<School> schoolList = districtId > 0
                ? schoolService.GetSchoolsByDistrictId(districtId)
                : new EnumerableQuery<School>(new List<School>());

            if(CurrentUser.IsSchoolAdmin || CurrentUser.IsTeacher)
            {
                var SchoolsUserHasAccessTo = userSchoolService.GetSchoolsUserHasAccessTo(CurrentUser.Id).Select(x=>x.SchoolId).ToList();
                schoolList = schoolList.Where(x => SchoolsUserHasAccessTo.Contains(x.Id));
            }

            var existedSchoolList = schoolService.GetSchoolByAuthorGroupId(authorGroupId).Select(x => x.Id).ToList();
            if (existedSchoolList.Count > 0)
            {
                schoolList = schoolList.Where(x => existedSchoolList.Contains(x.Id) == false);
            }

            var data = schoolList.Select(x => new AuthorGroupSchoolListViewModel
            {
                SchoolId = x.Id,
                SchoolName = x.Name
            });
            var parser = new DataTableParser<AuthorGroupSchoolListViewModel>();
            return Json(parser.Parse(data), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [AjaxOnly]
        public ActionResult AddUserToAuthorGroup(int userId, int authorGroupId)
        {
            if (userId > 0 && authorGroupId > 0)
            {
                //avoid modify ajax parameters

                if (!vulnerabilityService.HasRigtToAccessAuthorGroup(CurrentUser, authorGroupId))
                {
                    return
                        Json(
                            new
                            {
                                ErrorList = new[] { new { ErrorMessage = "Has no right to update this author group!" } },
                                success = false
                            }, JsonRequestBehavior.AllowGet);
                }
                if (!vulnerabilityService.HasRightToAcessUser(CurrentUser, userId, CurrentUser.GetMemberListDistrictId()))
                {
                    return
                        Json(
                            new
                            {
                                ErrorList = new[] { new { ErrorMessage = "Has no right to assign this user!" } },
                                success = false
                            }, JsonRequestBehavior.AllowGet);
                }
				
				if (CurrentUser.Id == userId)
                {
                    var authorGroup = authorGroupService.GetAuthorGroupById(authorGroupId);
                    if (authorGroup != null && CurrentUser.Id != authorGroup.UserId)
                    {
                        return
                       Json(
                           new
                           {
                               ErrorList = new[] { new { ErrorMessage = "Cannot assign yourself into this group" } },
                               success = false
                           }, JsonRequestBehavior.AllowGet);
                    }
                }

                authorGroupService.AddAuthorGroupUser(authorGroupId, userId);
                return Json(true);
            }
            return Json(false);
        }
        [HttpPost]
        [AjaxOnly]
        public ActionResult AddDistrictToAuthorGroup(int districtId, int authorGroupId)
        {
            if (districtId > 0 && authorGroupId > 0)
            {
                if (!vulnerabilityService.HasRigtToAccessAuthorGroup(CurrentUser, authorGroupId))
                {
                    return Json(new { Success = false, error = "Has no right to work with this author group" });
                }
                if (!Util.HasRightOnDistrict(CurrentUser, districtId))
                {
                    return Json(new { Success = false, error = "Has no right to assign this " + LabelHelper.DistrictLabel + " to the group." });
                }

                authorGroupService.AddAuthorGroupDistrict(authorGroupId, districtId);
                return Json(true);
            }
            return Json(false);
        }
        [HttpPost]
        [AjaxOnly]
        public ActionResult AddSchoolToAuthorGroup(int schoolId, int authorGroupId)
        {
            if (schoolId > 0 && authorGroupId > 0)
            {
                if (!vulnerabilityService.HasRigtToAccessAuthorGroup(CurrentUser, authorGroupId))
                {
                    return Json(new { Success = false, error = "Has no right to work with this author group" });
                }
                if (!vulnerabilityService.CheckUserCanAccessSchool(CurrentUser, schoolId))
                {
                    return Json(new { Success = false, error = "Has no right to assign this school to the group." });
                }
                authorGroupService.AddAuthorGroupSchool(authorGroupId, schoolId);
                return Json(true);
            }
            return Json(false);
        }

        [HttpGet]
        public ActionResult AddAuthorGroup()
        {
            var user = userService.GetUserById(CurrentUser.Id);
            if (user != null)
            {
                CurrentUser.SchoolId = user.SchoolId;
            }
            var viewModel = new AddAuthorGroupViewModel
                            {
                                IsDistrictAdmin = CurrentUser.RoleId.Equals((int) Permissions.DistrictAdmin),
                                IsSchoolAdmin = CurrentUser.IsSchoolAdmin,
                                IsPublisher = CurrentUser.IsPublisher,
                                IsTeacher = CurrentUser.IsTeacher,
                                States = stateService.GetStates().Select(s => new SelectListItem
                                                                              {
                                                                                  Value = s.Id.ToString(),
                                                                                  Text = s.Name
                                                                              }).ToList(),
                                UserDistrictId = CurrentUser.DistrictId.GetValueOrDefault(),
                                UserStateId = CurrentUser.StateId.GetValueOrDefault(),
                                UserSchoolId = CurrentUser.SchoolId.GetValueOrDefault(),
                                IsNetworkAdmin = CurrentUser.IsNetworkAdmin,
                                ListDistricIds = CurrentUser.IsNetworkAdmin ? CurrentUser.GetMemberListDistrictId() : null
                            };
            return View(viewModel);
        }

        [HttpGet]
        public ActionResult EditAuthorGroup(int id)
        {
            if (!vulnerabilityService.HasRigtToAccessAuthorGroup(CurrentUser, id))
            {
                return RedirectToAction("AuthorGroupList");
            }
            var obj = authorGroupService.GetAuthorGroupById(id);
            var user = userService.GetUserById(CurrentUser.Id);

            if (user != null)
            {
                CurrentUser.SchoolId = user.SchoolId;
            }

            var viewModel = new AddAuthorGroupViewModel
                                {
                                    Id = obj.Id,
                                    IsDistrictAdmin = CurrentUser.RoleId.Equals((int) Permissions.DistrictAdmin),
                                    IsSchoolAdmin = CurrentUser.IsSchoolAdmin,
                                    IsPublisher = CurrentUser.IsPublisher,
                                    IsTeacher = CurrentUser.IsTeacher,
                                    States = stateService.GetStates().Select(s => new SelectListItem
                                                                                      {
                                                                                          Value = s.Id.ToString(),
                                                                                          Text = s.Name
                                                                                      }).ToList(),                                    
                                    UserDistrictId = CurrentUser.DistrictId.GetValueOrDefault(),
                                    UserStateId = CurrentUser.StateId.GetValueOrDefault(),
                                    UserSchoolId = CurrentUser.SchoolId.GetValueOrDefault(),
                                    StateId = obj.StateId,
                                    DistrictId = obj.DistrictId,
                                    SchoolId = obj.SchoolId,
                                    Name = obj.Name,
                                    IsNetworkAdmin = CurrentUser.IsNetworkAdmin
                                };
            if(CurrentUser.IsNetworkAdmin)
            {
                List<int> stateIdList = districtService.GetStateIdByDictricIds(CurrentUser.GetMemberListDistrictId());
                viewModel.States = stateService.GetStatesByIds(stateIdList).Select(s => new SelectListItem
                                                                                            {
                                                                                                Value = s.Id.ToString(),
                                                                                                Text = s.Name
                                                                                            }).ToList();
            }
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAuthorGroup(AddAuthorGroupViewModel model)
        {
            var user = userService.GetUserById(CurrentUser.Id);
            if (user != null)
            {
                CurrentUser.SchoolId = user.SchoolId;
            }
            model.SetValidator(addAuthorGroupViewModelValidator);

            model.IsDistrictAdmin = CurrentUser.RoleId.Equals((int) Permissions.DistrictAdmin);
            model.IsSchoolAdmin = CurrentUser.IsSchoolAdmin;
            model.IsPublisher = CurrentUser.IsPublisher;
            model.IsTeacher = CurrentUser.IsTeacher;

            if (CurrentUser.IsDistrictAdmin)
            {
                model.StateId = CurrentUser.StateId;
                model.DistrictId = CurrentUser.DistrictId;
            }
            if (CurrentUser.IsSchoolAdmin || CurrentUser.IsTeacher)
            {
                model.StateId = CurrentUser.StateId;
                model.DistrictId = CurrentUser.DistrictId;
                //model.SchoolId = CurrentUser.SchoolId;
            }
           
            if (!model.IsValid)
            {
                var listError = new List<ValidationFailure>();
                foreach (var validationFailure in model.ValidationErrors)
                {
                    if (!listError.Any(x => x.ErrorMessage.Equals(validationFailure.ErrorMessage)))
                    {
                        listError.Add(validationFailure);
                    }
                }
                return Json(new { Success = false, ErrorList = listError });
            }
            //avoid modifying ajax parameters
            if (model.DistrictId.HasValue && model.DistrictId.Value > 0 
                &&!Util.HasRightOnDistrict(CurrentUser, model.DistrictId.GetValueOrDefault()))
            {
                var listError = new List<ValidationFailure>();
                listError.Add(new ValidationFailure("DistrictId", "Has no right on this distict."));
                return Json(new { Success = false, ErrorList = listError });
            }
            //get district

            if (model.DistrictId.HasValue && model.DistrictId.Value > 0)
            {
                var district = districtService.GetDistrictById(model.DistrictId.GetValueOrDefault());
                if (district == null)
                {
                    var listError = new List<ValidationFailure>();
                    listError.Add(new ValidationFailure("DistrictId", "" + LabelHelper.DistrictLabel + " does not exist."));
                    return Json(new { Success = false, ErrorList = listError });
                }
                else
                {
                    //check state
                    if (district.StateId != model.StateId)
                    {
                        var listError = new List<ValidationFailure>();
                        listError.Add(new ValidationFailure("DistrictId", "" + LabelHelper.DistrictLabel + " does not belong to the state."));
                        return Json(new { Success = false, ErrorList = listError });
                    }

                }
            }

            //check school
            if (model.SchoolId.HasValue && model.SchoolId.Value > 0
                && !vulnerabilityService.CheckUserCanAccessSchool(CurrentUser, model.SchoolId.GetValueOrDefault()))
            {
                var listError = new List<ValidationFailure>();
                listError.Add(new ValidationFailure("DistrictId", "Has no right on school."));
                return Json(new { Success = false, ErrorList = listError });
            }

            var newAuthorGroup = new AuthorGroup
                                 {
                                     Level = 3,
                                     DistrictId = model.DistrictId,
                                     Name = model.Name,
                                     SchoolId = model.SchoolId,
                                     StateId = model.StateId,
                                     UserId = CurrentUser.Id
                                 };
            authorGroupService.AddAuthorGroup(newAuthorGroup);

            authorGroupService.AddAuthorGroupUser(newAuthorGroup.Id, CurrentUser.Id);

            return Json(new {Success = true, RedirectUrl = Url.Action("EditAuthorGroup", new {id = newAuthorGroup.Id})});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateAuthorGroup(AddAuthorGroupViewModel model)
        {
            if (model != null)
            {
                model.SetValidator(addAuthorGroupViewModelValidator);
                if (!model.IsValid)
                {
                    var listError = new List<ValidationFailure>();
                    foreach (var validationFailure in model.ValidationErrors)
                    {
                        if (!listError.Any(x => x.ErrorMessage.Equals(validationFailure.ErrorMessage)))
                        {
                            listError.Add(validationFailure);
                        }
                    }
                    return Json(new { Success = false, ErrorList = listError });
                }
                //avoid modifying ajax parameters
                if (model.DistrictId.HasValue && model.DistrictId.Value > 0
                    && !Util.HasRightOnDistrict(CurrentUser, model.DistrictId.GetValueOrDefault()))
                {
                    return Json(new { ErrorList = new[] { new { ErrorMessage = "Has no right on this distict." } }, success = false }, JsonRequestBehavior.AllowGet);
                }
                //get district

                if (model.DistrictId.HasValue && model.DistrictId.Value > 0)
                {
                    var district = districtService.GetDistrictById(model.DistrictId.GetValueOrDefault());
                    if (district == null)
                    {
                        return Json(new { ErrorList = new[] { new { ErrorMessage = "" + LabelHelper.DistrictLabel + " does not exist." } }, success = false }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        //check state
                        if (district.StateId != model.StateId)
                        {
                            return Json(new { ErrorList = new[] { new { ErrorMessage = "" + LabelHelper.DistrictLabel + " does not belong to the state." } }, success = false }, JsonRequestBehavior.AllowGet);
                        }

                    }
                }

                //check school
                if (model.SchoolId.HasValue && model.SchoolId.Value > 0
                    && !vulnerabilityService.CheckUserCanAccessSchool(CurrentUser, model.SchoolId.GetValueOrDefault()))
                {
                    return Json(new { ErrorList = new[] { new { ErrorMessage = "Has no right on school." } }, success = false }, JsonRequestBehavior.AllowGet);
                }
               
                var obj = authorGroupService.GetAuthorGroupById(model.Id);
                if (obj != null)
                {
                    if (!vulnerabilityService.HasRigtToAccessAuthorGroup(CurrentUser, obj.Id))
                    {
                        return
                            Json(
                                new
                                {
                                    ErrorList = new[] { new { ErrorMessage = "Has no right to update this author group!" } },
                                    success = false
                                }, JsonRequestBehavior.AllowGet);
                    }
                    obj.Name = model.Name;
                    obj.StateId = model.StateId;
                    obj.DistrictId = model.DistrictId;
                    obj.SchoolId = model.SchoolId;
                    authorGroupService.Save(obj);
                    return Json(true);
                }
            }

            return Json(new { ErrorList = new[] { new { ErrorMessage = "invalid author group info!" } }, success = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult UpdateAuthorGroupOnTop(AuthorGroup model)
        {
            if (model != null)
            {
                var obj = authorGroupService.GetAuthorGroupById(model.Id);
                if (obj != null)
                {
                    if (!vulnerabilityService.HasRigtToAccessAuthorGroup(CurrentUser, obj.Id))
                    {
                        return
                            Json(
                                new
                                {
                                    ErrorList = new[] {new {ErrorMessage = "Has no right to update this author group!"}},
                                    success = false
                                }, JsonRequestBehavior.AllowGet);
                    }
                    obj.Name = model.Name;
                    authorGroupService.Save(obj);
                    return Json(true);
                }
            }

            return Json(new { ErrorList = new[] { new { ErrorMessage = "invalid author group info!" } }, success = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadAuthorGroupListForItemBank(int itemBankId)
        {
            ViewBag.ItemBankId = itemBankId;
            ViewBag.AddFor = Util.ItemBankConstant; // "itembank";
            var user = userService.GetUserById(CurrentUser.Id);
            if (user != null)
            {
                CurrentUser.SchoolId = user.SchoolId;
            }
            var model = new AuthorGroupListViewModel
            {
                IsDistrictAdmin = CurrentUser.RoleId.Equals((int)Permissions.DistrictAdmin),
                IsSchoolAdmin = CurrentUser.IsSchoolAdmin,
                IsPublisher = CurrentUser.IsPublisher,
                IsTeacher = CurrentUser.IsTeacher,
                IsNetworkAdmin = CurrentUser.IsNetworkAdmin,
                DistrictId = CurrentUser.DistrictId.GetValueOrDefault(),
                StateId = CurrentUser.StateId.GetValueOrDefault(),
                SchoolId = CurrentUser.SchoolId.GetValueOrDefault()
            };
            return PartialView("_AuthorGroupList", model);
        }

        public ActionResult LoadAuthorGroupListForItemBankV2(int itemBankId)
        {
            ViewBag.ItemBankId = itemBankId;
            ViewBag.AddFor = Util.ItemBankConstant; // "itembank";
            var user = userService.GetUserById(CurrentUser.Id);
            if (user != null)
            {
                CurrentUser.SchoolId = user.SchoolId;
            }
            var model = new AuthorGroupListViewModel
            {
                IsDistrictAdmin = CurrentUser.RoleId.Equals((int)Permissions.DistrictAdmin),
                IsSchoolAdmin = CurrentUser.IsSchoolAdmin,
                IsPublisher = CurrentUser.IsPublisher,
                IsTeacher = CurrentUser.IsTeacher,
                IsNetworkAdmin = CurrentUser.IsNetworkAdmin,
                DistrictId = CurrentUser.DistrictId.GetValueOrDefault(),
                StateId = CurrentUser.StateId.GetValueOrDefault(),
                SchoolId = CurrentUser.SchoolId.GetValueOrDefault()
            };
            return PartialView("v2/_AuthorGroupList", model);
        }

        public ActionResult LoadAuthorGroupListForItemSet(int itemSetId)
        {
            ViewBag.ItemSetId = itemSetId;
            ViewBag.AddFor = Util.ItemSetConstant;// "itemset";
            var user = userService.GetUserById(CurrentUser.Id);
            if (user != null)
            {
                CurrentUser.SchoolId = user.SchoolId;
            }
            var model = new AuthorGroupListViewModel
            {
                IsDistrictAdmin = CurrentUser.RoleId.Equals((int)Permissions.DistrictAdmin),
                IsSchoolAdmin = CurrentUser.IsSchoolAdmin,
                IsPublisher = CurrentUser.IsPublisher,
                IsTeacher = CurrentUser.IsTeacher,
                DistrictId = CurrentUser.DistrictId.GetValueOrDefault(),
                StateId = CurrentUser.StateId.GetValueOrDefault(),
                SchoolId = CurrentUser.SchoolId.GetValueOrDefault()
            };
            return PartialView("_AuthorGroupList", model);
        }

        public ActionResult EditAuthorGroupOnTop(int authorGroupId)
        {
            var obj = authorGroupService.GetAuthorGroupById(authorGroupId);
            return PartialView("_EditAuthorGroup", obj);
        }

        [HttpPost, AjaxOnly]
        public ActionResult GetAuthorGroupBanks(int bankID)
        {
            var authorGroups = authorGroupService.GetAuthorGroupBanks(bankID,CurrentUser.Id);
            var data = authorGroups.Select(x => new AuthorGroupListDataViewModel
            {
                AuthorGroupId = x.AuthorGroupId,
                DistrictList = x.DistrictName ?? string.Empty,
                SchoolList = x.SchoolName ?? string.Empty,
                UserList = x.UserNameList ?? string.Empty,
                Name = x.Name ?? string.Empty,
                UserId = x.UserId
            });

            var parser = new DataTableParser<AuthorGroupListDataViewModel>();

            return Json(parser.Parse(data), JsonRequestBehavior.AllowGet);
        }

        [HttpGet, AjaxOnly]
        public ActionResult GetAuthorGroupsNotInBank(int stateId, int districtId, int schoolId, int bankID)
        {
            if (districtId <= 0 && !CurrentUser.IsPublisher && !CurrentUser.IsNetworkAdmin )
            {
                districtId = CurrentUser.DistrictId.GetValueOrDefault();
            }

            var authorGroups = authorGroupService.GetAuthorGroupNotInBank(CurrentUser.Id, stateId, districtId, schoolId, bankID);
            var data = authorGroups.Select(x => new AuthorGroupListDataViewModel
            {
                AuthorGroupId = x.AuthorGroupId,
                DistrictList = x.DistrictName ?? string.Empty,
                SchoolList = x.SchoolName ?? string.Empty,
                UserList = x.UserNameList ?? string.Empty,
                Name = x.Name ?? string.Empty,
                UserId = x.UserId
            });
            var parser = new DataTableParser<AuthorGroupListDataViewModel>();
            return Json(parser.Parse(data), JsonRequestBehavior.AllowGet);
        }

        #endregion

        private bool IsUserAdmin()
        {
            return userService.GetAdminByUsernameAndDistrict(CurrentUser.UserName, CurrentUser.DistrictId.GetValueOrDefault()).IsNotNull();
        }

    }
}
