using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Old.SharingGroup;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Services.SharingGroup;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Models.DataTable;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels.SharingGroup;
using LinkIt.BubbleSheetPortal.Web.ViewModels.UserGroup;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.Required)]
    [VersionFilter]
    [AjaxAwareAuthorize(Order = 2)]
    public class SharingGroupController : BaseController
    {
        private SharingGroupService _sharingGroupService;
        private readonly SchoolService _schoolService;

        public SharingGroupController(SharingGroupService sharingGroupService,
            SchoolService schoolService)
        {
            this._sharingGroupService = sharingGroupService;
            this._schoolService= schoolService;
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.DadManageSharingGroups)]
        public ActionResult Index(bool? showInactiveSharingGroup, int? tabActive = 1)
        {
            var model = new SharingGroupViewModel()
            {
                CurrentUserId = CurrentUser.Id,
                IsPublisher = CurrentUser.IsPublisher,
                IsNetworkAdmin = CurrentUser.IsNetworkAdmin,
                DistrictID = CurrentUser.DistrictId,
                IsSchoolAdminOrTeacher = CurrentUser.IsSchoolAdmin || CurrentUser.IsTeacher,
                ShowInactiveSharingGroup = showInactiveSharingGroup ?? false,
                TabActive = tabActive
            };
            return View(model);
        }

        [HttpGet]
        [AjaxOnly]
        public JsonResult GetSharingGroups(GetSharingGroupRequest request)
        {
            var paginationRequest = MappingGetSharingGroupUserRequest(request);
            var response = _sharingGroupService.GetSharingGroups(paginationRequest);

            var result = new GenericDataTableResponse<SharingGroupDto>()
            {
                sEcho = request.sEcho,
                sColumns = request.sColumns,
                aaData = response.Data,
                iTotalDisplayRecords = response.TotalRecord,
                iTotalRecords = response.TotalRecord
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        private GetSharingGroupPaginationRequest MappingGetSharingGroupUserRequest(GetSharingGroupRequest request)
        {
            var currentDistrict = CurrentDistrict(request.DistrictID);
            var paginationRequest = new GetSharingGroupPaginationRequest
            {
                PageSize = request.iDisplayLength,
                StartRow = request.iDisplayStart,
                GeneralSearch = request.sSearch ?? "",
                DistrictID = currentDistrict > 0 ? currentDistrict : 0,
                UserID = CurrentUser.Id,
                RoleID = CurrentUser.RoleId,
                ShowInactiveSharingGroup = request.ShowInactiveSharingGroup,
                ShowCreatedByOtherSharingGroup = request.ShowCreatedByOtherSharingGroup,
            };

            if (!string.IsNullOrWhiteSpace(request.sColumns) && request.iSortCol_0.HasValue)
            {
                var columns = request.sColumns.Split(',');
                paginationRequest.SortColumn = columns[request.iSortCol_0.Value].Replace(" ", "");
                paginationRequest.SortDirection = request.sSortDir_0.Equals("desc") ? "DESC" : "ASC";
            }

            if (!string.IsNullOrEmpty(request.sSearch))
            {
                paginationRequest.GeneralSearch = request.sSearch.Trim();
            }
            return paginationRequest;
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult DeleteSharingGroup(int sharingGroupId)
        {
            var result = _sharingGroupService.DeleteSharingGroup(sharingGroupId);
            return Json(new { Success = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult ChangeStatusSharingGroup(int sharingGroupId)
        {
            var result = _sharingGroupService.ChangeStatusSharingGroup(sharingGroupId);
            return Json(new { Success = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult PublishOrUnpublishSharingGroup(int sharingGroupId, bool isPublished)
        {
            var result = _sharingGroupService.PublishOrUnpublishSharingGroup(CurrentUser.Id, sharingGroupId, !isPublished);
            return Json(new { Success = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int sharingGroupId, int districtId)
        {
            var model = _sharingGroupService.GetDetailSharingGroup(sharingGroupId);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.IsPublisher = CurrentUser.IsPublisher;
            ViewBag.IsDistrictAdmin = CurrentUser.IsDistrictAdmin;
            ViewBag.IsSchoolAdmin = CurrentUser.IsSchoolAdmin;
            return PartialView("_EditSharingGroup", model);
        }

        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult SaveEdit(SharingGroupDto model)
        {            
            try
            {
                var result = _sharingGroupService.EditSharingGroup(CurrentUser.Id, model);
                if (result != null &&  result.Success == false)
                {
                    return Json(new { Success = false, ErrorMessage = result.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(new { Success = false, ErrorMessage = "There was some error happend. Please contact admin." }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Add(int districtId)
        {
            var model = new SharingGroupDto();
            model.DistrictID = districtId;
            return PartialView("_AddSharingGroup", model);
        }

        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult SaveAdd(SharingGroupDto model)
        {
            try
            {
                var result = _sharingGroupService.AddSharingGroup(CurrentUser.Id, model);

                if (result != null && result.Success == false)
                {
                    return Json(new { Success = false, ErrorMessage = result.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(new { Success = false, ErrorMessage = "There was some error happend. Please contact admin." }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Clone(int sharingGroupId)
        {
            var model = _sharingGroupService.GetDetailSharingGroup(sharingGroupId);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            model.Name = string.Format("{0}_Clone", model.Name);
            ViewBag.IsPublisher = CurrentUser.IsPublisher;
            ViewBag.IsDistrictAdmin = CurrentUser.IsDistrictAdmin;
            ViewBag.IsSchoolAdmin = CurrentUser.IsSchoolAdmin;
            return PartialView("_CloneSharingGroup", model);
        }

        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult SaveClone(SharingGroupDto model)
        {
            try
            {
                var result = _sharingGroupService.CloneSharingGroup(model, CurrentUser.Id);

                if (result != null && result.Success == false)
                {
                    return Json(new { Success = false, ErrorMessage = result.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(new { Success = false, ErrorMessage = "There was some error happend. Please contact admin." }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        #region User In Sharing Group
        [HttpGet]
        public ActionResult EditUserInSharingGroup(int sharingGroupId, int districtId, bool? showInactiveSharingGroup, int? tabActive = 1)
        {
            var sharingGroup = _sharingGroupService.GetDetailSharingGroup(sharingGroupId);
            if (sharingGroup == null)
            {
                return RedirectToAction("Index");
            }
            if ((CurrentUser.IsSchoolAdmin || CurrentUser.IsTeacher) && sharingGroup.CreatedBy != CurrentUser.Id && (sharingGroup.IsPublished == null || (sharingGroup.IsPublished.HasValue && sharingGroup.IsPublished.Value == false)))
            {
                return RedirectToAction("NotFound", "Error");
            }
            var model = new UserInSharingGroupViewModel()
            {
                CurrentUserId = CurrentUser.Id,
                IsPublisher = CurrentUser.IsPublisher,
                IsNetworkAdmin = CurrentUser.IsNetworkAdmin,
                DistrictID = CurrentDistrict(districtId),
                Name = sharingGroup.Name,
                SharingGroupID = sharingGroupId,
                CreatedBySharingGroup = sharingGroup.CreatedBy,
                IsOwner = CurrentUser.IsSchoolAdmin ? (CurrentUser.Id == sharingGroup.CreatedBy ? true : false) : true,
                ShowInactiveSharingGroup = showInactiveSharingGroup ?? false,
                TabActive = tabActive
            };
            return View(model);
        }

        [HttpGet]
        [AjaxOnly]
        public JsonResult GetUserInSharingGroup(GetUserInSharingGroupRequest request)
        {
            var paginationRequest = MappingGetUserInSharingGroupRequest(request);
            var response = _sharingGroupService.GetUserInSharingGroup(paginationRequest);
            var result = new GenericDataTableResponse<UserInSharingGroupDto>()
            {
                sEcho = request.sEcho,
                sColumns = request.sColumns,
                aaData = response.Data,
                iTotalDisplayRecords = response.TotalRecord,
                iTotalRecords = response.TotalRecord
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        private GetUserInSharingGroupPaginationRequest MappingGetUserInSharingGroupRequest(GetUserInSharingGroupRequest request)
        {
            var paginationRequest = new GetUserInSharingGroupPaginationRequest
            {
                PageSize = request.iDisplayLength,
                StartRow = request.iDisplayStart,
                GeneralSearch = request.sSearch ?? "",
                DistrictID = CurrentDistrict(request.DistrictID),
                UserID = CurrentUser.Id,
                RoleID = CurrentUser.RoleId,
                IsShowInactiveUser = request.IsShowInactiveUser,
                SharingGroupID = request.SharingGroupID
            };

            if (!string.IsNullOrWhiteSpace(request.sColumns) && request.iSortCol_0.HasValue)
            {
                var columns = request.sColumns.Split(',');
                paginationRequest.SortColumn = columns[request.iSortCol_0.Value].Replace(" ", "");
                paginationRequest.SortDirection = request.sSortDir_0.Equals("desc") ? "DESC" : "ASC";
            }

            if (!string.IsNullOrEmpty(request.sSearch))
            {
                paginationRequest.GeneralSearch = request.sSearch.Trim();
            }
            return paginationRequest;
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult RemoveUserFromSharingGroup(int sharingGroupId, int userId)
        {
            var result = _sharingGroupService.RemoveUserFromSharingGroup(sharingGroupId, userId);
            return Json(new { Success = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAssignUserInSharingGroup(int sharingGroupId, int districtId)
        {
            try
            {
                var sharingGroup = _sharingGroupService.GetDetailSharingGroup(sharingGroupId);
                var roles = new List<ListItem>()
                {
                    new ListItem(){ Id = (int)Permissions.Teacher, Name = "Teacher"},
                    new ListItem(){ Id = (int)Permissions.SchoolAdmin, Name = "School Admin"},
                    new ListItem(){ Id = (int)Permissions.DistrictAdmin, Name = "District Admin"},
                    new ListItem(){ Id = (int)Permissions.NetworkAdmin, Name = "Network Admin"}
                };
                var schools = _schoolService.GetSchoolsByDistrictId(districtId).Select(x => new ListItem { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name).ToList();
                var model = new AssignUserSharingGroupViewModel() { SharingGroup = sharingGroup, Roles = roles, Schools = schools };
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [AjaxOnly]
        public JsonResult GetUserAvailableAddSharingGroupByFilter(GetUserAvailableAddSharingGroupRequest request)
        {
            var paginationRequest = MappingGetUserAvailableAddSharingGroupRequest(request);
            var response = _sharingGroupService.GetUserUserAvailableAddSharingGroup(paginationRequest);
            var result = new GenericDataTableResponse<UserInSharingGroupDto>()
            {
                sEcho = request.sEcho,
                sColumns = request.sColumns,
                aaData = response.Data,
                iTotalDisplayRecords = response.TotalRecord,
                iTotalRecords = response.TotalRecord
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        private GetUserAvailableAddSharingGroupPaginationRequest MappingGetUserAvailableAddSharingGroupRequest(GetUserAvailableAddSharingGroupRequest request)
        {
            var paginationRequest = new GetUserAvailableAddSharingGroupPaginationRequest
            {
                PageSize = request.iDisplayLength,
                StartRow = request.iDisplayStart,
                GeneralSearch = request.sSearch ?? "",
                DistrictID = CurrentDistrict(request.DistrictID),
                UserID = CurrentUser.Id,
                RoleID = CurrentUser.RoleId,
                ShowInactiveUser = request.ShowInactiveUser,
                SharingGroupID = request.SharingGroupID,
                RoleIdList = request.RoleIdList,
                SchoolIdList = request.SchoolIdList
            };

            if (!string.IsNullOrWhiteSpace(request.sColumns) && request.iSortCol_0.HasValue)
            {
                var columns = request.sColumns.Split(',');
                paginationRequest.SortColumn = columns[request.iSortCol_0.Value].Replace(" ", "");
                paginationRequest.SortDirection = request.sSortDir_0.Equals("desc") ? "DESC" : "ASC";
            }

            if (!string.IsNullOrEmpty(request.sSearch))
            {
                paginationRequest.GeneralSearch = request.sSearch.Trim();
            }
            return paginationRequest;
        }

        public ActionResult AddUserToSharingGroup(int sharingGroupId, string userIdsStr)
        {
            var userIds = userIdsStr.Split(',').Select(s => int.Parse(s)).ToList();
            var sharingGroup = _sharingGroupService.AddUserToSharingGroup(sharingGroupId, userIds);
            if (!sharingGroup.Success)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
