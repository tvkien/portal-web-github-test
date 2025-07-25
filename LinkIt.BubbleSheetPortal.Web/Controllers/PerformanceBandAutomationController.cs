using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport;
using LinkIt.BubbleSheetPortal.Models.DTOs.PerformanceBandAutomations;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Models.DataTable;
using LinkIt.BubbleSheetPortal.Web.Models.PerformanceBandAutomations;
using LinkIt.BubbleSheetPortal.Web.Resolver;
using LinkIt.BubbleSheetPortal.Web.Security;
using System;
using System.Linq;
using System.Web.Mvc;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize]
    [VersionFilter]
    public class PerformanceBandAutomationController : BaseController
    {        
        private readonly UserService _userService;
        private readonly PerformanceBandAutomationService _performanceBandAutomationService;

        public PerformanceBandAutomationController(
            UserService userService,
            PerformanceBandAutomationService performanceBandAutomationService)
        {            
            _userService = userService;
            _performanceBandAutomationService = performanceBandAutomationService;
        }

        [HttpGet]        
        public ActionResult Index()
        {
            if(CurrentUser.IsPublisher == false)
                return RedirectToAction("Index", "Home");

            return View(CurrentUser.RoleId);
        }

        [HttpGet]
        public ActionResult GetPerformanceBandAutomations(PerformanceBandAutomationFilter criteria)
        {
            if (CurrentUser.IsPublisher == false)
                return RedirectToAction("Index", "Home");

            var result = new GenericDataTableResponse<PerformanceBandAutomationDto>();
            result.sEcho = criteria.sEcho;
            result.sColumns = criteria.sColumns;

            var request = MappingRequest(criteria);
            var data = _performanceBandAutomationService.GetTestForPBS(request);

            var totalRecord = data.FirstOrDefault()?.TotalCount ?? 0;
            result.iTotalDisplayRecords = totalRecord;
            result.iTotalRecords = totalRecord;
            result.aaData = data.Select(x => new PerformanceBandAutomationDto
            {
                VirtualTestID = x.VirtualTestID,
                ResultDate = x.ResultDate?.ToString("yyyy/MM/dd"),
                TestName = x.Name,
                DataSetCategory = x.DataSetCategoryName,
                Subject = x.SubjectName,
                Grade = x.GradeName,
                PBSInEffect = x.PBSInEffect,
                PerformanceBandGroupList = x.PerformanceBandGroupList,
            }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetTestTypeGradeAndSubject(TestTypeGradeAndSubjectForPBSFilter criteria)
        {
            if (CurrentUser.IsPublisher == false)
                return RedirectToAction("Index", "Home");

            var districtID = criteria.DistrictId;

            var availableAttribute = _performanceBandAutomationService.GetTestTypeGradeAndSubject(criteria);

            var validTestTypes = availableAttribute
                .Where(c => string.Equals(c.Kind, "TestType"))
                .OrderBy(x => x.Name)
                .Select(c => new { c.Id, c.Name })
                .ToList();

            var validGrades = availableAttribute
                .Where(c => string.Equals(c.Kind, "Grade"))
                .OrderBy(x => x.Order)
                .Select(c => new { c.Id, c.Name })
                .ToList();

            var validSubjects = availableAttribute
                .Where(c => string.Equals(c.Kind, "Subject"))
                .OrderBy(x => x.Name)
                .Select(c => new { c.Id, c.Name })
                .ToList();

            return Json(BaseResponseModel<object>.InstanceSuccess(new
            {
                TestTypes = validTestTypes,
                Grades = validGrades,
                Subjects = validSubjects
            }), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ApplySetting(ApplySettingForPBSPayload payload)
        {
            if (CurrentUser.IsPublisher == false)
                return RedirectToAction("Index", "Home");

            var testResult = _performanceBandAutomationService.ApplySetting(payload);
            return Json(BaseResponseModel<object>.InstanceSuccess(testResult), JsonRequestBehavior.AllowGet);
        }

        private static TestForPBSFilter MappingRequest(PerformanceBandAutomationFilter criteria)
        {
            var request = new TestForPBSFilter();
            request.PageSize = criteria.iDisplayLength;
            request.StartRow = criteria.iDisplayStart;
            request.GeneralSearch = criteria.sSearch;

            if (!string.IsNullOrWhiteSpace(criteria.sColumns) && criteria.iSortCol_0.HasValue)
            {
                var columns = criteria.sColumns.Split(',');
                request.SortColumn = columns[criteria.iSortCol_0.Value];
                request.SortDirection = string.Compare(criteria.sSortDir_0, "desc") == 0 ? "DESC" : "ASC";
            }

            request.DistrictID = criteria.DistrictID;
            request.GradeIDs = criteria.GradeIDs;
            request.SubjectNames = criteria.SubjectNames;
            request.VirtualTestTypeIds = criteria.VirtualTestTypeIds;
            request.PBSInEffect = criteria.PBSInEffect;
            request.PBSGroup = criteria.PBSGroup;

            return request;
        }

        [HttpGet]
        public ActionResult GetPBSInEffect()
        {
            var list = _performanceBandAutomationService.GetPBSInEffect();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetPBSGroup(int districtId)
        {
            var list = _performanceBandAutomationService.GetPBSGroup(CurrentDistrict(districtId));
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RemoveSetting(ApplySettingForPBSPayload payload)
        {
            try
            {
                var result = _performanceBandAutomationService.RemoveSetting(payload);
                return Json(BaseResponseModel<object>.InstanceSuccess(result), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            
        }

    }
}
