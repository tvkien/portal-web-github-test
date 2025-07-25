using LinkIt.BubbleSheetPortal.Web.Security;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.SessionState;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Web.App_Start;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Services.BusinessObjects;
using LinkIt.BubbleSheetPortal.Models.DTOs.Users;
using System;
using System.Linq;
using LinkIt.BubbleSheetPortal.Models.DTOs;
using LinkIt.BubbleSheetPortal.Models.DTOs.StudentPreference;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.Required)]
    [AjaxAwareAuthorize]
    [JsonNetFilter]
    public class CategoriesAPIController : BaseController
    {
        private readonly CategoriesService _service;

        public CategoriesAPIController(CategoriesService service)
        {
            _service = service;
        }

        public ActionResult GetStates()
        {
            try
            {
                var networkDistricts = CurrentUser.GetMemberListDistrictId();
                var data = _service.GetStates(CurrentUser.RoleId, networkDistricts);
                return Json(Util.SuccessFormat(data), JsonRequestBehavior.AllowGet);
            }
            catch (System.Exception ex)
            {
                return Json(Util.ErrorFormat(ex.Message, null), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetDistrictByStateId(int stateId)
        {
            try
            {
                var networkDistricts = CurrentUser.GetMemberListDistrictId();
                var data = _service.GetDistrictByStateId(CurrentUser.RoleId, stateId, networkDistricts);
                return Json(Util.SuccessFormat(data), JsonRequestBehavior.AllowGet);
            }
            catch (System.Exception ex)
            {
                return Json(Util.ErrorFormat(ex.Message, null), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetSchoolByDistrictId(int? districtId)
        {
            try
            {
                if (districtId > 0 && !Util.HasRightOnDistrict(CurrentUser, districtId.GetValueOrDefault()))
                {
                    return Json(new List<ListItem>(), JsonRequestBehavior.AllowGet);
                }

                if (CurrentUser.IsDistrictAdmin)
                    districtId = CurrentUser.DistrictId;

                var data = _service.GetSchoolByDistrictId(CurrentUser.Id, CurrentUser.RoleId, districtId);
                return Json(Util.SuccessFormat(data), JsonRequestBehavior.AllowGet);
            }
            catch (System.Exception ex)
            {
                return Json(Util.ErrorFormat(ex.Message, null), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetCurrentUser()
        {
            try
            {
                return Json(Util.SuccessFormat(new CurrentUserDTO
                {
                    Name = CurrentUser.Name,
                    Id = CurrentUser.Id,
                    DistrictGroupId = CurrentUser.DistrictGroupId,
                    DistrictId = CurrentUser.DistrictId,
                    ListDistrictId = CurrentUser.GetMemberListDistrictId(),
                    Active = CurrentUser.Active,
                    EmailAddress = CurrentUser.EmailAddress,
                    FirstName = CurrentUser.FirstName,
                    LastName = CurrentUser.LastName,
                    PhoneNumber = CurrentUser.PhoneNumber,
                    RoleId = CurrentUser.RoleId,
                    SchoolId = CurrentUser.SchoolId,
                    StateId = CurrentUser.StateId,
                    UserName = CurrentUser.UserName
                }), JsonRequestBehavior.AllowGet);
            }
            catch (System.Exception ex)
            {
                return Json(Util.ErrorFormat(ex.Message, null), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public ActionResult GetGenders()
        {
            var genderList = _service
                .GetAllGenders()
                .Select(c => new ListItem()
                {
                    Id = c.GenderID,
                    Name = c.Name
                }).OrderBy(c => c.Name);
            return Json(genderList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetDataSetCategories()
        {
            try
            {
                var result = _service.GetDataSetCategoriesAddListItem();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(Util.ErrorFormat(ex.Message, null), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetDataSetCategoriesToComboTreeByDistrictId(int? districtid)
        {
            try
            {
                if (!CurrentUser.IsPublisher && !CurrentUser.IsNetworkAdmin)
                {
                    districtid = CurrentUser.DistrictId.GetValueOrDefault();
                }
                if (districtid == null || districtid.Value == 0)
                {
                    districtid = CurrentUser.DistrictId.GetValueOrDefault();
                }
                var dataSources = _service.GetDataSetCategoriesToComboTree(null, CurrentUser.Id, default, districtid ?? 0);
                return Json(dataSources, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(Util.ErrorFormat(ex.Message, null), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetDataSetCategoriesToComboTree(int? categoryId)
        {
            try
            {
                var dataSources = _service.GetDataSetCategoriesToComboTree(categoryId, CurrentUser.Id, CurrentUser.StateId.GetValueOrDefault(), CurrentUser.DistrictId.GetValueOrDefault());
                return Json(dataSources, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(Util.ErrorFormat(ex.Message, null), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetDataSetCategoriesForStudentPreference(int districtId = 0)
        {
            if (!CurrentUser.IsPublisher && !CurrentUser.IsNetworkAdmin)
            {
                districtId = CurrentUser.DistrictId.GetValueOrDefault();
            }
            var criteria = new GetDatasetCatogoriesParams
            {
                DistrictId = districtId,
                UserId = CurrentUser.Id,
                RoleId = CurrentUser.RoleId,
                SchoolId = CurrentUser.SchoolId.GetValueOrDefault()
            };
            var categories = _service.GetDataSetCategories(criteria);
            return Json(categories, JsonRequestBehavior.AllowGet);
        }
    }
}
