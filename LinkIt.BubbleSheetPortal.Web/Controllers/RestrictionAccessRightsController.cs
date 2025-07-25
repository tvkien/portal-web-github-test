using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Services.BusinessObjects;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Security;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.SessionState;
using System.Linq;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using LinkIt.BubbleSheetPortal.Common;
using UTLI = LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Models.DataTable;
using LinkIt.BubbleSheetPortal.Models.Enum;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport;
using LinkIt.BubbleSheetPortal.Models.DTOs;
using LinkIt.BubbleSheetPortal.Models.DTOs.StudentPreference;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.ReadOnly)]
    [AjaxAwareAuthorize]
    [VersionFilter]
    public class RestrictionAccessRightsController : BaseController
    {
        private readonly RestrictionAccessRightsControllerParameters _parameters;
        private readonly CategoriesService _service;
        private readonly UserService _userService;

        public RestrictionAccessRightsController(RestrictionAccessRightsControllerParameters parameters, CategoriesService service, UserService userService)
        {
            this._parameters = parameters;
            this._service = service;
            this._userService = userService;
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.DataAccessManagement)]
        public ActionResult Index()
        {
            var model = new RestrictionAccessRightViewModel
            {
                IsDistrictAdmin = CurrentUser.RoleId.Equals((int)Permissions.DistrictAdmin),
                IsPublisher = CurrentUser.IsPublisher,
                DistrictId = CurrentUser.DistrictId.GetValueOrDefault(),
                StateId = CurrentUser.StateId.GetValueOrDefault(),
                CurrentUserId = CurrentUser.Id,
                IsNetworkAdmin = CurrentUser.IsNetworkAdmin,
                UserRoleId = CurrentUser.RoleId
            };
            return View("RestrictionAccessRights", model);
        }


        [HttpPost, AjaxOnly]
        public ActionResult SaveRestrictionCategoriesTests(string restrictionCategoriesTests)
        {            
            if(!string.IsNullOrEmpty(restrictionCategoriesTests))
            {
                var restrictionTestCategories = StringExtensions.DeserializeObject<List<CategoryTestRestrictionModuleMatrixDto>>(restrictionCategoriesTests);
                var data = restrictionTestCategories.FirstOrDefault();
                _parameters.TestRestrictionModuleService.SaveRestrictionCategoriesTests(new SaveCategoriesTestsRestrictionModuleRequestDto
                {
                    ModifiedUser = CurrentUser.Id,
                    ModifiedDate = DateTime.UtcNow,
                    PublishedLevelID = CurrentDistrict(data.DistrictId),
                    PublishedLevelName = ContaintUtil.TestPreferenceLevelDistrict,
                    RestrictionModuleId = 9,
                    RestrictedObjectName = data.RestrictionTypeName,
                    CategoriesTestsRestriction = restrictionTestCategories,
                    DistrictId = CurrentDistrict(data.DistrictId)
                });
            }
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetRestrictionCategories(int districtId)
        {
            var categoriesRestrict = new List<CategoryRestrictionModuleDto>();
            if (!CurrentUser.IsPublisher && !CurrentUser.IsNetworkAdmin)
            {
                districtId = CurrentUser.DistrictId.GetValueOrDefault();
            }                
            if (districtId > 0)
            {
                var criteria = new GetDatasetCatogoriesParams
                {
                    DistrictId = districtId,
                    UserId = CurrentUser.Id,
                    RoleId = CurrentUser.RoleId,
                    SchoolId = CurrentUser.SchoolId.GetValueOrDefault()
                };
                categoriesRestrict = this._parameters.TestRestrictionModuleService.GetCategoryRestrictions(criteria);
            }
            var parser = new DataTableParser<CategoryRestrictionModuleDto>();
            return Json(parser.Parse2018(categoriesRestrict.AsQueryable(), true), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetRestrictionTests(TestRestrictionFilter criteria)
        {
            var result = new GenericDataTableResponse<TestRestrictionDto>();
            result.aaData = new List<TestRestrictionDto>();
            result.sEcho = criteria.sEcho;
            result.sColumns = criteria.sColumns;
            
            TestRestrictionRequestDto request = MappingTestRestrictionRequest(criteria);

            request.UserID = CurrentUser.Id;
            request.RoleID = CurrentUser.RoleId;               
            if (criteria.Level == PreferenceLevel.DISTRICT)
            {
                var districtAdmin = _userService.GetUsersByDistrictAndRole(criteria.DistrictId, (int)RoleEnum.DistrictAdmin).FirstOrDefault();
                if (districtAdmin != null)
                {
                    request.UserID = districtAdmin.Id;
                    request.RoleID = (int)RoleEnum.DistrictAdmin;
                }
            }
            if (string.IsNullOrEmpty(criteria.GradeIds) && string.IsNullOrEmpty(criteria.Subjects) && string.IsNullOrEmpty(criteria.CategoryIds))
            {
                request.Subjects = "-1";
            }
            if ((!CurrentUser.IsPublisher))
            {
                request.DistrictID = CurrentUser.DistrictId.GetValueOrDefault();
            }
            var data = this._parameters.TestRestrictionModuleService.GetTestRestrictions(request);
            result.iTotalDisplayRecords = data.TotalRecord;
            result.iTotalRecords = data.TotalRecord;
            result.aaData = data.Data;
                         
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetTestTypeGradeAndSubject(SearchBankCriteria criteria)
        {
            RestrictInvalidParams(criteria);

            if (string.Compare(criteria.Level, "class", true) == 0 && string.IsNullOrEmpty(criteria.ClassIds))
            {
                return Json(BaseResponseModel<object>.InstanceSuccess(new
                {
                    TestTypes = new List<ListItemRestriction>(),
                    Grades = new List<ListItemRestriction>(),
                    Subjects = new List<ListItemRestriction>()
                }), JsonRequestBehavior.AllowGet);
            }

            var availableAttribute = _parameters.StudentPreferencesService.GetAvailableTestTypeGradeAndSubjectForStudentPreference(criteria);

            var validTestTypes = availableAttribute.Where(c => string.Compare(c.Kind, "TestType", true) == 0).Select(c => new SelectListItemDTO() { Id = c.Id, Name = c.Name }).DistinctBy(x => x.Id).OrderBy(x => x.Name).ToList();

            var validGrades = availableAttribute.Where(c => string.Compare(c.Kind, "Grade", true) == 0).OrderBy(x => x.Order).Select(c => new ListItemRestriction(c.Id, c.Name)).DistinctBy(x => x.Id).ToList();

            var validSubjects = availableAttribute.Where(c => string.Compare(c.Kind, "Subject", true) == 0).Select(c => new ListItemRestriction(c.Id, c.Name)).DistinctBy(x => x.Name).OrderBy(x => x.Name).ToList();

            var categoriesRestrict = _parameters.TestRestrictionModuleService.GetCategoriesRestriction(criteria.DistrictId, validTestTypes.ToList());
            return Json(BaseResponseModel<object>.InstanceSuccess(new
            {
                TestTypes = categoriesRestrict.Select(x => new ListItemRestriction(x.CategoryId, x.CategoryName) {
                    SchoolAdminRestriction = x.SchoolAdminRestriction,
                    TeacherRestriction = x.TeacherRestriction,
                    SchoolAdminRestrictionContent = x.SchoolAdminRestrictionContent,
                    TeacherRestrictionContent = x.TeacherRestrictionContent,
                }),
                Grades = validGrades,
                Subjects = validSubjects
            }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRestrictionByRestrictedObject(int categoryId, int virtualTestId, int districtId)
        {
            if (!CurrentUser.IsPublisher && !CurrentUser.IsNetworkAdmin)
            {
                districtId = CurrentUser.DistrictId.GetValueOrDefault();
            }
            var result = _parameters.TestRestrictionModuleService.GetRestrictionByRestrictedObject(categoryId, virtualTestId, districtId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private static TestRestrictionRequestDto MappingTestRestrictionRequest(TestRestrictionFilter criteria)
        {            
            var request = new TestRestrictionRequestDto();
            request.PageSize = criteria.iDisplayLength;
            request.StartRow = criteria.iDisplayStart;
            request.GeneralSearch = criteria.sSearch;            

            if (!string.IsNullOrWhiteSpace(criteria.sColumns) && criteria.iSortCol_0.HasValue)
            {                
                var columns = criteria.sColumns.Split(',');
                request.SortColumn = columns[criteria.iSortCol_0.Value];
                request.SortDirection = string.Compare(criteria.sSortDir_0, "desc") == 0 ? "DESC" : "ASC";
            }

            request.DistrictID = criteria.DistrictId;
            request.GradeIds = criteria.GradeIds;
            request.Level = criteria.Level;
            request.Subjects = criteria.Subjects;
            request.CategoryIds = criteria.CategoryIds;
            return request;
        }
        private void RestrictInvalidParams(SearchBankCriteria criteria)
        {
            criteria.UserId = CurrentUser.Id;
            criteria.UserRole = CurrentUser.RoleId;
            if (criteria.Level == PreferenceLevel.DISTRICT)
            {
                var districtAdmin = _userService.GetUsersByDistrictAndRole(criteria.DistrictId, (int)RoleEnum.DistrictAdmin).FirstOrDefault();
                if (districtAdmin != null)
                {
                    criteria.UserId = districtAdmin.Id;
                    criteria.UserRole = (int)RoleEnum.DistrictAdmin;
                }
            }
            else if (criteria.Level == PreferenceLevel.SCHOOL)
            {
                var schoolAdminIds = _userService.GetUsersByDistrictAndRole(criteria.DistrictId, (int)RoleEnum.SchoolAdmin).Select(m => m.Id).ToArray();
                var userIds = _parameters.UserSchoolService.GetListUserBySchoolId(criteria.SchoolId);
                var schoolAdminId = userIds.FirstOrDefault(m => schoolAdminIds.Contains(m));

                if (schoolAdminId > 0)
                {
                    criteria.UserId = schoolAdminId;
                    criteria.UserRole = (int)RoleEnum.SchoolAdmin;
                }
            }

            if ((!CurrentUser.IsPublisher && !CurrentUser.IsNetworkAdmin) || criteria.Level == PreferenceLevel.CLASS)
            {
                criteria.SchoolId = 0;
                criteria.DistrictId = CurrentUser.DistrictId.GetValueOrDefault();
            }
        }
    }
}
