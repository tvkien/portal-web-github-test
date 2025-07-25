using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport;
using LinkIt.BubbleSheetPortal.Models.DTOs.StudentPreference;
using LinkIt.BubbleSheetPortal.Models.Enum;
using LinkIt.BubbleSheetPortal.Models.Old.UnGroup;
using LinkIt.BubbleSheetPortal.Models.StudentPreferenceDTOs;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Models.DataTable;
using LinkIt.BubbleSheetPortal.Web.Models.StudentPreferences;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels.StudentPreference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.ReadOnly)]
    [AjaxAwareAuthorize]
    [VersionFilter]
    public class StudentPreferenceController : BaseController
    {
        private readonly StudentPreferencesService _preferenceService;
        private readonly StateService _stateService;
        private readonly DistrictService _districtService;
        private readonly SchoolService _schoolService;
        private readonly UserSchoolService _userSchoolService;
        private readonly SubjectService _subjectService;
        private readonly UserService _userService;
        private readonly ManageTestService _manageTestService;

        public StudentPreferenceController(StudentPreferencesService pPreferenceService, StateService stateService, DistrictService districtService,
            SchoolService schoolService, UserSchoolService userSchoolService,
            SubjectService subjectService, UserService userService, ManageTestService manageTestService)
        {
            _preferenceService = pPreferenceService;
            _stateService = stateService;
            _districtService = districtService;
            _schoolService = schoolService;
            _userSchoolService = userSchoolService;
            _subjectService = subjectService;
            _userService = userService;
            _manageTestService = manageTestService;
        }

        public ActionResult FullOptionIncludeDependency(string level, string virtualTestIds, int levelId = 0, int districtId = 0, int schoolId = 0, int dataSetCategoryID = 0, string tabActive = "", string classIds = "")
        {
            InitDataByUserRole(level, ref levelId, ref schoolId, ref districtId);

            var virtualTestId = 0;
            var viewModel = new StudentPreferenceTableViewModel();

            if (!string.IsNullOrEmpty(virtualTestIds))
            {
                int.TryParse(virtualTestIds.Split(',')[0], out virtualTestId);
            }
            var matrix = _preferenceService.GetFullOptionIncludeDependency(level, districtId, schoolId, CurrentUser.Id, dataSetCategoryID, virtualTestId, tabActive, (classIds ?? "").ToIntArray(","));
            viewModel.StudentPreferenceMatrix = matrix.StudentPreferenceMatrix.OrderBy(x => x.Order).ToList();
            var html = RazorViewToString.RenderRazorViewToString(this, "_FullOptionIncludeDependency", viewModel);
            return Json(html, JsonRequestBehavior.AllowGet);
        }


        public ActionResult FullOptionIncludeDependencyJson(string level, int districtId = 0, int schoolId = 0, int userId = 0, int testType = 0, int virtualTestId = 0)
        {
            var matrix = _preferenceService.GetFullOptionIncludeDependency(level, districtId, schoolId, userId, testType, virtualTestId);
            return Json(matrix, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.OnlinetestStudentPreference)]
        public ActionResult Index()
        {
            return View("StudentPreferences", CurrentUser.RoleId);
        }

        public ActionResult GetTestTypeGradeAndSubject(SearchBankCriteria criteria)
        {
            var userId = CurrentUser.Id;
            var districtID = criteria.DistrictId;
            var schoolID = criteria.SchoolId;
            var roleId = CurrentUser.RoleId;

            RestrictInvalidParams(ref districtID, schoolID, ref userId, ref roleId);

            if (string.Compare(criteria.Level, "class", true) == 0 && string.IsNullOrEmpty(criteria.ClassIds))
            {
                return Json(BaseResponseModel<object>.InstanceSuccess(new
                {
                    TestTypes = new List<TestTypeSelectItem>(),
                    Grades = new List<ListItem>(),
                    Subjects = new List<ListSubjectItem>()
                }), JsonRequestBehavior.AllowGet);
            }
            RestrictInvalidParams(criteria);

            var availableAttribute = _preferenceService.GetAvailableTestTypeGradeAndSubjectForStudentPreference(criteria);

            var validTestTypes = availableAttribute.Where(c => string.Compare(c.Kind, "TestType", true) == 0).Select(c => new { c.Id, c.Name, c.IsShow, c.Tooltip }).DistinctBy(x => x.Id).OrderBy(x => x.Name).ToArray();

            var validGrades = availableAttribute.Where(c => string.Compare(c.Kind, "Grade", true) == 0).OrderBy(x => x.Order).Select(c => new { c.Id, c.Name, c.IsShow, c.Tooltip }).DistinctBy(x => x.Id).ToArray();

            var validSubjects = availableAttribute.Where(c => string.Compare(c.Kind, "Subject", true) == 0).Select(c => new { c.Id, c.Name, c.IsShow, c.Tooltip }).DistinctBy(x => x.Name).OrderBy(x => x.Name).ToArray();

            return Json(BaseResponseModel<object>.InstanceSuccess(new
            {
                TestTypes = validTestTypes,
                Grades = validGrades,
                Subjects = validSubjects
            }), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetAssociatedClasses()
        {
            try
            {

                var associatedClassDetails = _preferenceService.GetAssociatedClassesThatHasTestResult(this.CurrentUser.Id, this.CurrentUser.DistrictId, this.CurrentUser.RoleId);
                return Json(BaseResponseModel<IEnumerable<ClassDetailDto>>.InstanceSuccess(associatedClassDetails), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(BaseResponseModel<IEnumerable<ClassDetailDto>>.InstanceError(ex.Message, "error", null), JsonRequestBehavior.AllowGet);
            }
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
                var userIds = _userSchoolService.GetListUserBySchoolId(criteria.SchoolId);
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

        public ActionResult GetListTestType(int districtID = 0, int schoolID = 0)
        {
            var userId = CurrentUser.Id;
            var roleId = CurrentUser.RoleId;
            RestrictInvalidParams(ref districtID, schoolID, ref userId, ref roleId);
            List<SelectListItem> list = GetTestTypes(districtID, schoolID, userId, roleId);


            return Json(list, JsonRequestBehavior.AllowGet);
        }

        private List<SelectListItem> GetTestTypes(int districtID, int schoolID, int userId, int roleId)
        {
            var selectedItems = new List<SelectListItem>();
            var types = _preferenceService.GetListTestType(districtID, userId, roleId, schoolID);

            var others = types.Where(m => (m.DataSetOriginID != (int)DataSetOriginEnum.External_Data_Entry_Deprecated
                                            || m.DataSetOriginID != (int)DataSetOriginEnum.Custom_Deprecated
                                            || m.DataSetOriginID != (int)DataSetOriginEnum.Item_Based_Score_Deprecated)
                                        && !string.IsNullOrEmpty(m.DataSetCategoryName))
                                .Select(m => new SelectListItem
                                {
                                    Value = m.DataSetCategoryID.ToString(),
                                    Text = m.DataSetCategoryName
                                }).ToList();

            selectedItems.AddRange(others);
            selectedItems = selectedItems.OrderBy(m => m.Text).ToList();
            return selectedItems;
        }

        private void RestrictInvalidParams(ref int districtID, int schoolID, ref int userId, ref int roleId)
        {
            if (districtID == 0 || (!CurrentUser.IsPublisher() && !CurrentUser.IsNetworkAdmin()))
            {
                districtID = CurrentUser.DistrictId.GetValueOrDefault();
            }


            if (schoolID == 0)
            {
                var districtAdmin = _userService.GetUsersByDistrictAndRole(districtID, (int)RoleEnum.DistrictAdmin).FirstOrDefault();
                if (districtAdmin != null)
                {
                    userId = districtAdmin.Id;
                    roleId = (int)RoleEnum.DistrictAdmin;
                }
            }
            else
            {
                var schoolAdminIds = _userService.GetUsersByDistrictAndRole(districtID, (int)RoleEnum.SchoolAdmin).Select(m => m.Id).ToArray();
                var userIds = _userSchoolService.GetListUserBySchoolId(schoolID);
                var schoolAdminId = userIds.FirstOrDefault(m => schoolAdminIds.Contains(m));

                if (schoolAdminId > 0)
                {
                    userId = schoolAdminId;
                    roleId = (int)RoleEnum.SchoolAdmin;
                }
            }
        }

        public ActionResult GetStudentTestPreferences(TestForStudentPreferenceFilter criteria)
        {
            var result = new GenericDataTableResponse<TestForStudentPreferenceDto>();
            result.sEcho = criteria.sEcho;
            result.sColumns = criteria.sColumns;
            StudentPreferenceRequestDto request = MappingStudentPreferenceRequest(criteria);

            request.UserID = CurrentUser.Id;
            request.RoleID = CurrentUser.RoleId;
            if (criteria.Level == PreferenceLevel.DISTRICT)
            {
                var districtAdmin = _userService.GetUsersByDistrictAndRole(criteria.DistrictID, (int)RoleEnum.DistrictAdmin).FirstOrDefault();
                if (districtAdmin != null)
                {
                    request.UserID = districtAdmin.Id;
                    request.RoleID = (int)RoleEnum.DistrictAdmin;
                }
            }
            else if (criteria.Level == PreferenceLevel.SCHOOL)
            {
                var schoolAdminIds = _userService.GetUsersByDistrictAndRole(criteria.DistrictID, (int)RoleEnum.SchoolAdmin).Select(m => m.Id).ToList();
                var userIds = _userSchoolService.GetListUserBySchoolId(criteria.SchoolID);
                var schoolAdminId = userIds.FirstOrDefault(m => schoolAdminIds.Contains(m));

                if (schoolAdminId > 0)
                {
                    request.UserID = schoolAdminId;
                    request.RoleID = (int)RoleEnum.SchoolAdmin;
                }
            }

            if (string.IsNullOrEmpty(criteria.GradeIDs) && string.IsNullOrEmpty(criteria.SubjectIDs) && string.IsNullOrEmpty(criteria.VirtualTestTypeIds))
            {
                request.SubjectIDs = "-1";
            }

            if ((!CurrentUser.IsPublisher && !CurrentUser.IsNetworkAdmin) || criteria.Level == PreferenceLevel.CLASS)
            {
                request.SchoolID = 0;
                request.DistrictID = CurrentUser.DistrictId.GetValueOrDefault();
            }

            var data = _preferenceService.GetTestForStudentPreferences(request);

            result.iTotalDisplayRecords = data.TotalRecord;
            result.iTotalRecords = data.TotalRecord;
            result.aaData = data.Data.ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DefaultPreferences()
        {
            if (!CurrentUser.IsPublisher)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        private static StudentPreferenceRequestDto MappingStudentPreferenceRequest(TestForStudentPreferenceFilter criteria)
        {
            if (criteria.Level == PreferenceLevel.DISTRICT)
            {
                criteria.SchoolID = 0;
            }

            var request = new StudentPreferenceRequestDto();
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
            request.SchoolID = criteria.SchoolID;
            request.GradeIDs = criteria.GradeIDs;
            request.Level = criteria.Level;
            request.SubjectIDs = criteria.SubjectIDs;
            request.VirtualTestTypeIds = criteria.VirtualTestTypeIds;
            request.Visibilities = criteria.Visibilities;
            request.ClassIds = criteria.ClassIds;
            request.ExcludeTestTypes = criteria.ExcludeTestTypes;

            return request;
        }

        [CacheControl(HttpCacheability.NoCache), HttpGet]
        public ActionResult GetSchools(int? districtId)
        {
            IEnumerable<ListItem> data = null;
            if (CurrentUser.IsPublisher || CurrentUser.IsNetworkAdmin)
            {
                if (!districtId.HasValue) districtId = CurrentUser.DistrictId;
                data = _schoolService.GetSchoolsByDistrictId(districtId.GetValueOrDefault())
                    .Select(x => new ListItem { Id = x.Id, Name = x.Name })
                    .OrderBy(x => x.Name);
                return Json(data, JsonRequestBehavior.AllowGet);
            }

            if (CurrentUser.IsDistrictAdmin)
            {
                data = _schoolService.GetSchoolsByDistrictId(CurrentUser.DistrictId.GetValueOrDefault())
                    .Select(x => new ListItem { Id = x.Id, Name = x.Name })
                    .OrderBy(x => x.Name);
                return Json(data, JsonRequestBehavior.AllowGet);
            }

            if (CurrentUser.IsSchoolAdmin || CurrentUser.IsTeacher)
            {
                data = _userSchoolService.GetSchoolsUserHasAccessTo(CurrentUser.Id)
                    .Select(x => new ListItem { Id = x.SchoolId.GetValueOrDefault(), Name = x.SchoolName })
                    .OrderBy(x => x.Name);
                return Json(data, JsonRequestBehavior.AllowGet);
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [CacheControl(HttpCacheability.NoCache), HttpGet]
        public ActionResult GetStates()
        {
            IQueryable<State> data = _stateService.GetStates();

            var districtIDs = CurrentUser.GetMemberListDistrictId();
            if (CurrentUser.IsNetworkAdmin && districtIDs != null && districtIDs.Count > 0)
            {
                var stateIDs =
                    _districtService.GetDistricts()
                        .Where(o => districtIDs.Contains(o.Id))
                        .Select(o => o.StateId)
                        .ToList();
                data = data.Where(o => stateIDs.Contains(o.Id));
            }

            data = data.OrderBy(x => x.Name);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [CacheControl(HttpCacheability.NoCache), HttpGet]
        public ActionResult GetDistricts(int? stateId)
        {
            IQueryable<District> districts = stateId.HasValue
                ? _districtService.GetDistrictsByStateId(stateId.GetValueOrDefault())
                : _districtService.GetDistricts();
            var districtIDs = CurrentUser.GetMemberListDistrictId();
            if (CurrentUser.IsNetworkAdmin && districtIDs != null && districtIDs.Count > 0)
            {
                districts = districts.Where(o => districtIDs.Contains(o.Id));
            }

            IOrderedQueryable<ListItem> data =
                districts.Select(x => new ListItem { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetStudentPreference(string virtualTestIds, string level, int levelID = 0, int dataSetCategoryID = 0, int schoolID = 0, int districtID = 0, string classIds = "")
        {
            InitDataByUserRole(level, ref levelID, ref schoolID, ref districtID);

            if (!IsCanAccess(level, levelID))
            {
                return Json(new { Status = "error", Message = "Access denied!" }, JsonRequestBehavior.AllowGet);
            }

            var perference = _preferenceService.GetStudentPreferences(level, levelID, dataSetCategoryID, schoolID, districtID, virtualTestIds, CurrentUser.Id, classIds);

            return Json(perference, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetSettingForReporting(string virtualTestIds, string level, int levelID = 0, string classId = "")
        {
            var user = _userService.GetUserById(levelID);
            var districtID = user.DistrictId.GetValueOrDefault();
            var schoolID = (_userSchoolService.GetListSchoolIdByUserId(user.Id)).FirstOrDefault();

            if (!IsCanAccess(level, levelID))
            {
                return Json(new { Status = "error", Message = "Access denied!" }, JsonRequestBehavior.AllowGet);
            }

            var perference = _preferenceService.GetStudentPreferences(level, levelID, 0, schoolID, districtID, virtualTestIds, levelID, classId);

            var setting = new Setting(perference.Details);

            return Json(new { settings = setting }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetTestsBeShowedForAdminreporting(List<StudentPreferenceParamDTO> studentPreferenceParams, string level)
        {
            var result = new List<StudentPreferenceParamDTO>();
            foreach (var param in studentPreferenceParams)
            {
                var user = _userService.GetUserById(param.LevelId);
                var districtId = user.DistrictId.GetValueOrDefault();
                var schoolId = (_userSchoolService.GetListSchoolIdByUserId(user.Id)).FirstOrDefault();

                var perference = _preferenceService.GetStudentPreferences(level, param.LevelId, 0, schoolId, districtId, param.VirtualTestId.ToString(), param.LevelId);
                var setting = new Setting(perference.Details);
                if (setting.showTest == 1)
                    result.Add(new StudentPreferenceParamDTO() { VirtualTestId = param.VirtualTestId, IsShowReview = setting.reviewTest });
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetSettingForReportingDev(string virtualTestIds, string level, int levelID = 0)
        {
            var user = _userService.GetUserById(levelID);
            var districtID = user.DistrictId.GetValueOrDefault();
            var schoolID = (_userSchoolService.GetListSchoolIdByUserId(user.Id)).FirstOrDefault();

            if (!IsCanAccess(level, levelID))
            {
                return Json(new { Status = "error", Message = "Access denied!" }, JsonRequestBehavior.AllowGet);
            }

            var perference = _preferenceService.GetStudentPreferences(level, levelID, 0, schoolID, districtID, virtualTestIds, levelID);

            var setting = new Setting(perference.Details);

            return Json(new { settings = setting }, JsonRequestBehavior.AllowGet);
        }

        private void InitDataByUserRole(string level, ref int levelID, ref int schoolID, ref int districtID)
        {
            if (level == PreferenceLevel.SCHOOL)
            {
                schoolID = levelID;
                var school = _schoolService.GetSchoolById(levelID);
                if (school != null)
                {
                    districtID = school.DistrictId;
                }
            }
            else
            {
                if (level == PreferenceLevel.DISTRICT)
                {
                    districtID = levelID;
                }
                if (!CurrentUser.IsPublisher && !CurrentUser.IsNetworkAdmin)
                {
                    districtID = CurrentUser.DistrictId.GetValueOrDefault();

                    if (!CurrentUser.IsDistrictAdmin)
                    {
                        schoolID = (_userSchoolService.GetListSchoolIdByUserId(CurrentUser.Id)).FirstOrDefault();
                    }

                    if (levelID == 0)
                    {
                        if (level == PreferenceLevel.DISTRICT)
                        {
                            levelID = districtID;
                        }
                        else if (level == PreferenceLevel.CLASS && levelID == 0)
                        {
                            levelID = CurrentUser.Id;
                        }
                    }
                }
            }

            if (districtID == 0)
            {
                districtID = CurrentUser.DistrictId.GetValueOrDefault();
            }

            if (schoolID == 0)
            {
                schoolID = (_userSchoolService.GetListSchoolIdByUserId(CurrentUser.Id)).FirstOrDefault();
            }
        }

        [HttpPost]
        public ActionResult SetStudentPreference(StudentPreferenceDto model)
        {
            if (!IsCanAccess(model.Level, model.LevelID))
            {
                return Json(new { Status = "error", Message = "Access denied!" }, JsonRequestBehavior.AllowGet);
            }

            model.LastModifiedBy = CurrentUser.Id;
            model.LastModifiedDate = DateTime.UtcNow;

            if (model.VirtualTestID.HasValue || !string.IsNullOrEmpty(model.VirtualTestIDs))
            {
                model.DataSetCategoryID = null;
            }

            if (model.Level == PreferenceLevel.CLASS)
            {
                model.LevelID = CurrentUser.Id;
            }

            _preferenceService.SetDefaultOption(model, userId: CurrentUser.Id, CurrentUser.DistrictId, CurrentUser.RoleId);

            return Json(new { Status = "success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetSubjectByUserId(int districtId = 0, int schoolId = 0)
        {
            if (!CurrentUser.IsPublisher && !CurrentUser.IsNetworkAdmin)
            {
                districtId = CurrentUser.DistrictId.GetValueOrDefault();
            }

            var subjects = _preferenceService.GetSubjectByUserId(districtId, CurrentUser.Id, CurrentUser.RoleId, schoolId);
            return Json(subjects, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetGradeByUserId(int districtId = 0, int schoolId = 0)
        {
            var userId = CurrentUser.Id;
            var roleId = CurrentUser.RoleId;

            RestrictInvalidParams(ref districtId, schoolId, ref userId, ref roleId);

            var grades = _preferenceService.GetGradeByUserId(districtId, userId, roleId, schoolId);
            return Json(grades, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetSubjectsByGradeIdAndAuthor(SearchBankCriteria criteria)
        {
            var listSubjects = GetSubjectsByGradeIdAndAuthorAsAList(criteria);
            return Json(listSubjects, JsonRequestBehavior.AllowGet);
        }
        public List<ListSubjectItem> GetSubjectsByGradeIdAndAuthorAsAList(SearchBankCriteria criteria)
        {
            criteria.UserId = CurrentUser.Id;
            criteria.UserRole = CurrentUser.RoleId;


            if (criteria.DistrictId == 0 || (!CurrentUser.IsPublisher() && !CurrentUser.IsNetworkAdmin()))
            {
                criteria.DistrictId = CurrentUser.DistrictId.GetValueOrDefault();
            }


            if (criteria.SchoolId == 0)
            {
                var districtAdmin = _userService.GetUsersByDistrictAndRole(criteria.DistrictId, (int)RoleEnum.DistrictAdmin).FirstOrDefault();
                if (districtAdmin != null)
                {
                    criteria.UserId = districtAdmin.Id;
                    criteria.UserRole = (int)RoleEnum.DistrictAdmin;
                }
            }
            else
            {
                var schoolAdminIds = _userService.GetUsersByDistrictAndRole(criteria.DistrictId, (int)RoleEnum.SchoolAdmin).Select(m => m.Id).ToArray();
                var userIds = _userSchoolService.GetListUserBySchoolId(criteria.SchoolId);
                var schoolAdminId = userIds.FirstOrDefault(m => schoolAdminIds.Contains(m));

                if (schoolAdminId > 0)
                {
                    criteria.UserId = schoolAdminId;
                    criteria.UserRole = (int)RoleEnum.SchoolAdmin;
                }
            }

            var subjects = _subjectService.GetSubjectsByGradeIdAndAuthorOfAllTestType(criteria);
            if (subjects != null)
            {
                var subjectIncludes = _manageTestService.GetSubjectIncludes(CurrentUser.Id);
                subjects.AddRange(subjectIncludes);
                subjects = subjects.DistinctBy(m => m.Id).ToList();
                var subjectsAndIncludes = subjects.GroupBy(s => s.Name).Select(x => new ListSubjectItem { Id = x.Key, Name = x.Key }).OrderBy(x => x.Name).ToList();
                return subjectsAndIncludes;
            }
            return new List<ListSubjectItem>();
        }
        private bool IsCanAccess(string level, int levelID)
        {
            var canAccess = true;

            if (level == PreferenceLevel.ENTERPRISE && !CurrentUser.IsPublisher)
            {
                canAccess = false;
            }
            else if (level == PreferenceLevel.DISTRICT)
            {
                if (CurrentUser.IsSchoolAdmin || CurrentUser.IsTeacher)
                {
                    canAccess = false;
                }
                else if (CurrentUser.IsDistrictAdmin && CurrentUser.DistrictId.Value != levelID)
                {
                    canAccess = false;
                }
                else if (CurrentUser.IsNetworkAdmin && !CurrentUser.GetMemberListDistrictId().Any(m => m == levelID))
                {
                    canAccess = false;
                }
            }
            else if (level == PreferenceLevel.SCHOOL)
            {
                if (CurrentUser.IsTeacher)
                {
                    canAccess = false;
                }
            }
            return canAccess;
        }
    }
}
