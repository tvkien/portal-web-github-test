using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Old.UserGroup;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Models.DataTable;
using LinkIt.BubbleSheetPortal.Web.Models.UserGroup;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels.UserGroup;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.Required)]
    [VersionFilter]
    [AjaxAwareAuthorize(Order = 2)]
    public class UserGroupManagementController : BaseController
    {
        private readonly UserGroupManagementControllerParameters _parameters;

        public UserGroupManagementController(UserGroupManagementControllerParameters parameters)
        {
            _parameters = parameters;
        }

        [HttpGet, AdminOnly(Order = 3)]
        public ActionResult GetCreateUserGroupView(Boolean layoutV2 = false)
        {
            if (layoutV2)
            {
                return PartialView("v2/_CreateUserGroup");
            }
            return PartialView("_CreateUserGroup");
        }

        [HttpGet, AdminOnly(Order = 3)]
        public ActionResult GetAddModulePermissionView(int xliGroupId, int xliAreaId, int xliModuleId)
        {
            var model = _parameters.XLIGroupService.GetAreaModuleName(xliAreaId, xliModuleId);
            model.XLIGroupId = xliGroupId;

            return PartialView("_AddModulePermission", model);
        }

        [HttpGet, AdminOnly(Order = 3)]
        public ActionResult ViewSchoolAccess(int xliModuleId, int? districtId)
        {
            ViewBag.DistrictID = CurrentDistrict(districtId);
            ViewBag.MouduleID = xliModuleId;
            return PartialView("_SchoolAccessView");
        }

        [HttpGet, AdminOnly(Order = 3)]
        [AjaxOnly]
        public ActionResult GetViewSchoolAccessData(GetSchoolAccessRequest request)
        {
            var data = _parameters.XLIGroupService.GetSchoolAccessData(request.XLIModuleID, request.DistrictID).ToList();
            var result = new GenericDataTableResponse<SchoolAccess>()
            {
                sEcho = request.sEcho,
                sColumns = request.sColumns,
                aaData = data
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public JsonResult AddModulePermission(AddModulePermissionRequest request)
        {
            _parameters.XLIGroupService.AddModulePermission(request);
            return Json(new { success = true, message = "Add module permission succeed." }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public JsonResult RemoveModulePermission(RemoveModulePermissionRequest request)
        {
            _parameters.XLIGroupService.RemoveModulePermission(request);
            return Json(new { success = true, message = $"Remove module permission succeed.", JsonRequestBehavior.AllowGet });
        }

        [HttpGet, AdminOnly(Order = 3)]
        public ActionResult GetEditUserGroupView(int xliGroupId, int? districtId)
        {
            var dto = _parameters.XLIGroupService.GetById(xliGroupId, CurrentDistrict(districtId));
            var model = new EditUserGroupDto
            {
                XLIGroupID = xliGroupId,
                Name = dto.Name,
                DistrictID = dto.DistrictID,
                InheritRoleFunctionality = dto.InheritRoleFunctionality.HasValue && dto.InheritRoleFunctionality.Value

            };
            return PartialView("_EditUserGroup", model);
        }

        [HttpGet, AdminOnly(Order = 3)]
        public ActionResult GetUserInGroupView(int xliGroupId, int districtId, Boolean layoutV2 = false)
        {
            if (!CurrentUser.IsNetworkAdmin && !CurrentUser.IsPublisher && !CurrentUser.IsDistrictAdmin)
            {
                return View("NotFound");
            }

            var xliGroup = _parameters.XLIGroupService.GetGroupByID(xliGroupId);
            var model = new ManageUserGroupViewModel()
            {
                CurrentUserId = CurrentUser.Id,
                IsPublisher = CurrentUser.IsPublisher,
                IsNetworkAdmin = CurrentUser.IsNetworkAdmin,
                DistrictID = districtId,
                Name = xliGroup.Name,
                XLIGroupID = xliGroup.XLIGroupID,
                InheritRoleFunctionality = xliGroup.InheritRoleFunctionality
            };

            if (layoutV2)
            {
                return View("v2/UserInGroup", model);
            }

            return View("UserInGroup", model);
        }

        [HttpGet, AdminOnly(Order = 3)]
        public ActionResult GetModuleAccessView(int? xliGroupId, int? districtId)
        {
            if (!CurrentUser.IsNetworkAdmin && !CurrentUser.IsPublisher && !CurrentUser.IsDistrictAdmin)
            {
                return View("NotFound");
            }

            var currentDistrictID = CurrentDistrict(districtId);
            var group = _parameters.XLIGroupService.GetById(xliGroupId.Value, currentDistrictID);

            if (group == null)
            {
                return View("NotFound");
            }

            var model = new ModuleAccessViewModel
            {
                DistrictID = group.DistrictID.Value,
                XLIGroupID = group.XLIGroupID,
                GroupName = group.Name,
                InheritRoleFunctionality = group.InheritRoleFunctionality == true ? "ON" : "OFF"
            };
            ViewBag.PreviousUrl = Request?.UrlReferrer?.ToString().Contains("ManageUserGroups") ?? true
                ? Url.Action("ManageUserGroups", "Admin")
                : Request.UrlReferrer.ToString();
            return View("ModuleAccess", model);
        }

        [HttpGet, AdminOnly(Order = 3)]
        public ActionResult GetModuleAccessSummaryView(int? districtId)
        {
            if (!CurrentUser.IsNetworkAdmin && !CurrentUser.IsPublisher && !CurrentUser.IsDistrictAdmin)
            {
                return View("NotFound");
            }

            var currentDistrictID = CurrentDistrict(districtId);
            var model = new ModuleAccessSumaryViewModel
            {
                DistrictID = currentDistrictID
            };

            return View("ModuleAccessSummary", model);
        }

        [HttpGet, AdminOnly(Order = 3)]
        public JsonResult GetModuleAccessSummaryData(GetModuleAccessRequest request)
        {
            var paginationRequest = MappingGetModuleAccessRequest(request);
            var response = _parameters.XLIGroupService.GetModuleAccessSumary(paginationRequest);
            var result = new GenericDataTableResponse<XLIModuleAccessSummaryDto>()
            {
                sEcho = request.sEcho,
                sColumns = request.sColumns,
                aaData = response.Data.ToList(),
                iTotalDisplayRecords = response.TotalRecord,
                iTotalRecords = response.TotalRecord
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, AdminOnly(Order = 3)]
        public JsonResult GetAllUsersInGroup(int xliGroupId, int? districtId)
        {
            try
            {
                var request = new GetGroupUserRequest
                {
                    UserID = CurrentUser.Id,
                    RoleID = CurrentUser.RoleId,
                    XLIGroupID = xliGroupId,
                    DistrictID = CurrentDistrict(districtId),
                    IsShowInactiveUser = true,
                    StartRow = 0,
                    PageSize = 1000,
                    SortColumn = "LastName",
                    SortDirection = "ASC"
                };
                var result = _parameters.XLIGroupService.GetAllUsersInGroup(request);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { success = false, message = "An error has occurred.  Please try again.", error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            
        }

        [HttpGet, AdminOnly(Order = 3)]
        public JsonResult GetModuleAccessData(GetModuleAccessRequest request)
        {
            var paginationRequest = MappingGetModuleAccessRequest(request);
            var response = _parameters.XLIGroupService.GetModuleAccessByUser(paginationRequest);

            var result = new GenericDataTableResponse<GetModuleAccessDataDto>()
            {
                sEcho = request.sEcho,
                sColumns = request.sColumns,
                aaData = response.Data.ToList(),
                iTotalDisplayRecords = response.TotalRecord,
                iTotalRecords = response.TotalRecord
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAreasByUser(int? districtId)
        {
            var currentDistrictID = CurrentDistrict(districtId);
            var areas = _parameters.XLIGroupService.GetAreasByUser(CurrentUser, currentDistrictID);
            IEnumerable<ListItem> data = areas.Select(x => new ListItem { Id = x.XliAreaId, Name = x.DisplayName }).OrderBy(x => x.Name);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetModulesByArea(int areaId)
        {
            var areas = _parameters.XLIGroupService.GetModulesByArea(CurrentUser, areaId);
            IEnumerable<ListItem> data = areas.Select(x => new ListItem { Id = x.XliModuleId, Name = x.DisplayName }).OrderBy(x => x.Name);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AjaxOnly]
        public JsonResult GetXLIGroupUsers(GetUserGroupManagementRequest request)
        {
            var paginationRequest =  MappingGetGroupUserRequest(request);
            var response = _parameters.XLIGroupService.GetUserGroupsByDistrict(paginationRequest);

            var result = new GenericDataTableResponse<XLIGroupDto>()
            {
                sEcho = request.sEcho,
                sColumns = request.sColumns,
                aaData = response.Data.Select(x => new XLIGroupDto
                {
                    XLIGroupID = x.XLIGroupID,
                    DistrictID = x.DistrictID,
                    Name = x.Name,
                    InheritRoleFunctionality = x.InheritRoleFunctionality.HasValue && x.InheritRoleFunctionality.Value
                }).ToList(),
                iTotalDisplayRecords = response.TotalRecord,
                iTotalRecords = response.TotalRecord
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public JsonResult CreateXLIGroup(CreateUserGroupDto request)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                return Json(new { success = false, message = "Invalid request. Please input group name." }, JsonRequestBehavior.AllowGet);
            }

            request.DistrictID = CurrentDistrict(request.DistrictID);
            var isExistGroupName = _parameters.XLIGroupService.IsExistGroupName(request.Name, request.DistrictID.Value);

            if (isExistGroupName)
            {
                return Json(new { success = false, message = "Duplicated group name." }, JsonRequestBehavior.AllowGet);
            }

            _parameters.XLIGroupService.AddXLIGroup(new XLIGroupDto
            {
                Name = request.Name,
                DistrictID = request.DistrictID,
                InheritRoleFunctionality = request.InheritRoleFunctionality
            });
            return Json(new { success = true, message = "Create Group succeed." }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public JsonResult UpdateXLIGroup(EditUserGroupDto request)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                return Json(new { success = false, message = "Invalid request. Please input group name." }, JsonRequestBehavior.AllowGet);
            }

            request.DistrictID = CurrentDistrict(request.DistrictID);

            var existGroup = _parameters.XLIGroupService.GetById(request.XLIGroupID, request.DistrictID.Value);

            if (existGroup == null)
            {
                return Json(new { success = false, message = "Invalid request. Does not exist group." }, JsonRequestBehavior.AllowGet);
            }

            if (existGroup.Name != request.Name)
            {
                var isExistGroupName = _parameters.XLIGroupService.IsExistGroupName(request.Name, request.DistrictID.Value, request.XLIGroupID);

                if (isExistGroupName)
                {
                    return Json(new { success = false, message = "Duplicated group name." }, JsonRequestBehavior.AllowGet);
                }
            }

            var isUpdated = _parameters.XLIGroupService.UpdateXLIGroup(new XLIGroupDto
            {
                XLIGroupID = request.XLIGroupID,
                InheritRoleFunctionality = request.InheritRoleFunctionality,
                DistrictID = request.DistrictID,
                Name = request.Name
            });

            return Json(new { success = isUpdated }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public JsonResult DeleteXLIGroup(int xliGroupId, int? districtId)
        {
            try
            {
                int currentDistrictID = CurrentDistrict(districtId);
                var isDeleted = _parameters.XLIGroupService.DeleteXLIGroup(xliGroupId, currentDistrictID);
                return Json(new { success = isDeleted }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { success = false, message = "An error has occurred.  Please try again.", error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private GetUserGroupManagementPaginationRequest MappingGetGroupUserRequest(GetUserGroupManagementRequest request)
        {
            var currentDistrict = CurrentDistrict(request.DistrictID);
            var paginationRequest = new GetUserGroupManagementPaginationRequest
            {
                PageSize = request.iDisplayLength,
                StartRow = request.iDisplayStart,
                GeneralSearch = request.sSearch,
                DistrictID = currentDistrict > 0 ? currentDistrict : 0,
                UserID = CurrentUser.Id,
                RoleID = CurrentUser.RoleId
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

        private GetModuleAccessPaginationRequest MappingGetModuleAccessRequest(GetModuleAccessRequest request)
        {
            var currentDistrict = CurrentDistrict(request.DistrictID);
            var paginationRequest = new GetModuleAccessPaginationRequest
            {
                PageSize = request.iDisplayLength,
                StartRow = request.iDisplayStart,
                DistrictID = currentDistrict > 0 ? currentDistrict : 0,
                UserID = CurrentUser.Id,
                RoleID = CurrentUser.RoleId,
                XLIAreaID = request.XLIAreaID,
                XLIGroupID = request.XLIGroupID,
                XLIModuleID = request.XLIModuleID
            };

            if (!string.IsNullOrWhiteSpace(request.sColumns) && request.iSortCol_0.HasValue)
            {
                var columns = request.sColumns.Split(',');
                paginationRequest.SortColumn = columns[request.iSortCol_0.Value];
                paginationRequest.SortDirection = request.sSortDir_0.Equals("desc") ? "DESC" : "ASC";
            }

            if (!string.IsNullOrEmpty(request.sSearch))
            {
                paginationRequest.SearchString = request.sSearch.Trim();
            }

            return paginationRequest;
        }
    }
}
