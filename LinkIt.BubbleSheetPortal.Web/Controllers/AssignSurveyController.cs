using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.ViewModels.AssignSurvey;
using System;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Web.Models.Survey;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Services.Survey;
using LinkIt.BubbleSheetPortal.Models.DTOs.Survey;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Common.Enum;
using System.Configuration;
using System.Threading.Tasks;
using LinkIt.BubbleSheetPortal.Web.Resolver;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.Required)]
    [AjaxAwareAuthorize]
    public class AssignSurveyController : BaseController
    {
        private readonly AssignSurveyService _assignSurveyService;
        private readonly DistrictTermService _districtTermService;
        private readonly ConfigurationService _configurationService;
        private readonly QTITestClassAssignmentService _qTITestClassAssignmentService;
        private readonly UserService _userService;
        private readonly ProgramService _programService;
        public AssignSurveyController(
            AssignSurveyService assignSurveyService,
            DistrictTermService districtTermService,
            ConfigurationService configurationService,
            QTITestClassAssignmentService qTITestClassAssignmentService,
            UserService userService,
            ProgramService programService)
        {
            _assignSurveyService = assignSurveyService;
            this._districtTermService = districtTermService;
            this._configurationService = configurationService;
            this._qTITestClassAssignmentService = qTITestClassAssignmentService;
            this._userService = userService;
            this._programService = programService;
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.AssignSurveys)]
        public ActionResult Index(int? surveyBankId, int? surveyId, int? termId, int? surveyAssignmentType, int? stateId, int? districtId)
        {
            var model = new SurveyAssignmentViewModel()
            {
                IsAdmin = _userService.GetAdminByUsernameAndDistrict(CurrentUser.UserName, CurrentUser.DistrictId.GetValueOrDefault()).IsNotNull(),
                CanSelectTeachers = CurrentUser.IsDistrictAdminOrPublisher,
                IsSchoolAdmin = CurrentUser.RoleId.Equals(8),
                IsPublisher = CurrentUser.RoleId.Equals((int)Permissions.Publisher),
                DistrictId = districtId.HasValue ? districtId.Value : CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0,
                StateId = stateId.HasValue ? stateId.Value : 0,
                SurveyBankId = surveyBankId,
                SurveyId = surveyId,
                TermId = termId,
                SurveyAssignmentType = surveyAssignmentType
            };

            model.IsNetworkAdmin = false;
            if (CurrentUser.IsNetworkAdmin)
            {
                model.IsNetworkAdmin = true;
                model.ListDistricIds = CurrentUser.GetMemberListDistrictId();
            }

            model.IsDistrictAdmin = CurrentUser.IsDistrictAdmin;

            ViewBag.DateFormat = _configurationService.GetConfigurationByKey(Constanst.JQueryDateFormat)?.Value;

            return View(model);
        }
        [HttpGet]
        public ActionResult GetTermsByDistrict(int districtId)
        {
            if (CurrentUser.RoleId != (int)Permissions.Publisher && CurrentUser.RoleId != (int)Permissions.NetworkAdmin || districtId == 0)
            {
                districtId = CurrentUser.DistrictId.GetValueOrDefault();
            }

            var terms = _districtTermService.GetDistrictTermByDistrictID(districtId).OrderBy(x => x.Name).Select(x => new ListItem
            {
                Id = x.DistrictTermID,
                Name = x.Name
            });
            return Json(terms, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult AssignPublicAnonymous(SurveyAssignmentData data)
        {
            try
            {
                if (CurrentUser.RoleId != (int)Permissions.Publisher && CurrentUser.RoleId != (int)Permissions.NetworkAdmin)
                {
                    data.DistrictId = CurrentUser.DistrictId.GetValueOrDefault();
                }
                data.UserId = CurrentUser.Id;
                _assignSurveyService.AssignPublicAnonymous(data, LinkitConfigurationManager.GetLinkitSettings().DatabaseID);                
                return Json(new { Success = true });
            }
            catch (ArgumentException exp)
            {                
                return Json(new { Success = false, Message = exp.Message });
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { Success = false, Message = "Internal error. Please try again." });
            }
        }
        [HttpPost]
        public ActionResult AssignPublicIndividualized(SurveyAssignmentData data)
        {
            try
            {
                if (CurrentUser.RoleId != (int)Permissions.Publisher && CurrentUser.RoleId != (int)Permissions.NetworkAdmin)
                {
                    data.DistrictId = CurrentUser.DistrictId.GetValueOrDefault();
                }
                data.UserId = CurrentUser.Id;
                _assignSurveyService.AssignExtraCodePublicIndividualized(data, LinkitConfigurationManager.GetLinkitSettings().DatabaseID);

                return Json(new { Success = true });
            }
            catch (Exception exp)
            {
                PortalAuditManager.LogException(exp);
                return Json(new { Success = false, Message = exp.Message });
            }
        }
        [HttpPost]
        public ActionResult AssignAndDistributePublicIndividualized(SurveyAssignmentData data)
        {

            try
            {
                if (CurrentUser.RoleId != (int)Permissions.Publisher && CurrentUser.RoleId != (int)Permissions.NetworkAdmin)
                {
                    data.DistrictId = CurrentUser.DistrictId.GetValueOrDefault();
                }
                data.UserId = CurrentUser.Id;
                var distributeSetting = new DistributeSetting()
                {
                    TestCodePrefix = LinkitConfigurationManager.GetLinkitSettings().DatabaseID,
                    DistributedBy = CurrentUser.Name
                };
                _assignSurveyService.AssignAndDistributePublicIndividualized(data, distributeSetting);

                return Json(new { Success = true });
            }
            catch (Exception exp)
            {
                PortalAuditManager.LogException(exp);
                return Json(new { Success = false, Message = exp.Message });
            }
        }
        [HttpPost]
        public ActionResult AssignPrivate(SurveyAssignmentData data)
        {
            try
            {
                if (CurrentUser.RoleId != (int)Permissions.Publisher && CurrentUser.RoleId != (int)Permissions.NetworkAdmin)
                {
                    data.DistrictId = CurrentUser.DistrictId.GetValueOrDefault();
                }
                data.UserId = CurrentUser.Id;
                _assignSurveyService.AssignPrivate(data, LinkitConfigurationManager.GetLinkitSettings().DatabaseID);

                return Json(new { Success = true });
            }
            catch (Exception exp)
            {
                PortalAuditManager.LogException(exp);
                return Json(new { Success = false, Message = exp.Message });
            }
        }
        [HttpPost]
        public ActionResult AssignAndDistributePrivate(SurveyAssignmentData data)
        {
            try
            {
                if (CurrentUser.RoleId != (int)Permissions.Publisher && CurrentUser.RoleId != (int)Permissions.NetworkAdmin)
                {
                    data.DistrictId = CurrentUser.DistrictId.GetValueOrDefault();
                }
                data.UserId = CurrentUser.Id;
                var distributeSetting = new DistributeSetting()
                {
                    TestCodePrefix = LinkitConfigurationManager.GetLinkitSettings().DatabaseID,
                    DistributedBy = CurrentUser.Name,
                    HTTPProtocol = HelperExtensions.GetHTTPProtocal(Request)

                };
                _assignSurveyService.AssignAndDistributePrivate(data, distributeSetting);

                return Json(new { Success = true });
            }
            catch (Exception exp)
            {
                PortalAuditManager.LogException(exp);
                return Json(new { Success = false, Message = exp.Message });
            }
        }

        [HttpPost]
        public ActionResult Distribute(SurveyDistributeNotifyDto distributeModel)
        {
            if (!distributeModel.DistrictId.HasValue || distributeModel.DistrictId <= 0)
            {
                distributeModel.DistrictId = CurrentUser.DistrictId ?? 0;
            }
            distributeModel.PublishedBy = CurrentUser.Name;
            var httpProtocol = HelperExtensions.GetHTTPProtocal(Request);
            _assignSurveyService.Distribute(distributeModel, httpProtocol);

            return Json(new { Success = true });
        }

        public ActionResult GetSurveyAssignmentsResult(GetSurveyAssignmentsResultCriteria criteria)
        {
            if (CurrentUser.RoleId != (int)Permissions.Publisher && CurrentUser.RoleId != (int)Permissions.NetworkAdmin)
            {
                criteria.DistrictId = CurrentUser.DistrictId.GetValueOrDefault();
            }
            var parser = new DataTableParserProc<SurveyAssignResultViewModel>();
            if (!criteria.DistrictId.HasValue || !criteria.DistrictTermId.HasValue || !criteria.SurveyId.HasValue || !criteria.iDisplayLength.HasValue)
            {
                var empResult = new List<SurveyAssignResultViewModel>();
                return Json(parser.Parse(empResult.AsQueryable(), 0), JsonRequestBehavior.AllowGet);

            }
            int? totalRecords = 0;
            var searchText = Request["sSearch"] ?? string.Empty;
            var result = _qTITestClassAssignmentService
                .GetSurveyAssignmentResult(criteria.DistrictId.Value, criteria.DistrictTermId.Value, criteria.SurveyId.Value, (int)criteria.Type, parser.SortableColumns, searchText, parser.StartIndex, parser.PageSize, criteria.ShowInActive).ToList();
            if (result != null && result.Count > 0)
            {
                totalRecords = result[0].TotalRecords;
            }
            var data = result.Select(o => new SurveyAssignResultViewModel()
            {
                AssignmentDate = o.AssignmentDate,
                Id = o.Id,
                Code = o.Code,
                ShortLink = o.ShortLink,
                Status = o.Status,
                Email = o.Email,
                ResponseDate = o.ResponseDate,
                Respondent = o.Respondent,
                StudentId = o.StudentId
            }).AsQueryable();
            return Json(parser.Parse(data, totalRecords ?? 0), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult ChangeStatus(int districtId, int districtTermId, int surveyId, int surveyAssignmentType, int assignmentId, int status)
        {
            try
            {
                if (status < 0 || status > 1)
                {
                    return Json(new { Success = false, Message = "Status incorrect" });
                }
                bool allowAssign = true;
                if (status == 1 && surveyAssignmentType == (int)SurveyAssignmentTypeEnum.PublicAnonymous)
                {
                    allowAssign = _qTITestClassAssignmentService.CheckAllowAssignSurvey(districtId, districtTermId, surveyId, surveyAssignmentType);
                }
                if (allowAssign)
                {
                    _qTITestClassAssignmentService.ChangeStatus(assignmentId, status, CurrentUser.Id);
                    return Json(new { Success = true });
                }
                else
                {
                    return Json(new { Success = false, Message = "There should only be one active Public Anonymous Assignment link for each Survey on a specific Term." });
                }
                
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { Success = false, Message = ex.Message });
            }
        }
        [HttpGet]
        public ActionResult CheckMatchEmail(string emails, int? districtId, int surveyId, int termId, int assignmentType)
        {
            if (!districtId.HasValue || districtId <= 0)
            {
                districtId = CurrentUser.DistrictId;
            }

            var result = new List<CheckMatchEmailDto>();

            if (!string.IsNullOrEmpty(emails))
            {
                result = _assignSurveyService.CheckMatchEmails(emails, districtId ?? 0, surveyId, termId, assignmentType);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AjaxAwareAuthorize]
        public ActionResult GetUsersByRoles(GetUserResultsCriteria criteria)
        {
            if (CurrentUser.RoleId != (int)Permissions.Publisher && CurrentUser.RoleId != (int)Permissions.NetworkAdmin)
            {
                criteria.DistrictId = CurrentUser.DistrictId.GetValueOrDefault();
            }
            var parser = new DataTableParserProc<FilterUserResultsViewModel>();
            if (!criteria.DistrictId.HasValue || !criteria.iDisplayLength.HasValue || string.IsNullOrEmpty(criteria.Roles))
            {
                var empResult = new List<FilterUserResultsViewModel>();
                return Json(parser.Parse(empResult.AsQueryable(), 0), JsonRequestBehavior.AllowGet);

            }
            var request = new GetUserResultsRequest()
            {
                DistrictId = criteria.DistrictId,
                ClassId = criteria.ClassId,
                SchoolId = criteria.SchoolId,
                TermId = criteria.TermId,
                Type = criteria.Type,
                TeacherId = criteria.TeacherId,
                GradeIds = criteria.GradeIds ?? string.Empty,
                ProgramIds = criteria.ProgramIds ?? string.Empty,
                Roles = criteria.Roles ?? string.Empty,
                PageSize = parser.PageSize,
                StartIndex = parser.StartIndex,
                SearchText = Request["sSearch"],
                SortBy = parser.SortableColumns,
                RoleId = CurrentUser.RoleId,
                UserId = CurrentUser.Id
            };
            var results = _assignSurveyService.GetSurveyListUsersByRoles(request).ToList();
            int? totalRecords = 0;
            if (results.Count > 0)
            {
                totalRecords = results[0].TotalRecords;
            }
            var users = results.Select(x => new FilterUserResultsViewModel() {
                FullName = x.FullName,
                RoleId = x.RoleId,
                RoleName = x.RoleName,
                SchoolName = x.SchoolName,
                UserId = x.UserId,
                UserName = x.UserName
            }).ToList();
            return Json(parser.Parse(users.AsQueryable(), totalRecords ?? 0), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AjaxAwareAuthorize]
        public ActionResult GetSurveyProgram(int? districtId)
        {
            if (CurrentUser.RoleId != (int)Permissions.Publisher && CurrentUser.RoleId != (int)Permissions.NetworkAdmin || !districtId.HasValue)
            {
                districtId = CurrentUser.DistrictId.GetValueOrDefault();
            }
            var result = _programService.GetSurveyProgramByRole(districtId.GetValueOrDefault(), CurrentUser.Id, CurrentUser.RoleId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        [AjaxAwareAuthorize]
        public ActionResult GetAssignSurveyBanks(int? districtId)
        {
            if (!districtId.HasValue || districtId <= 0)
                districtId = CurrentUser.DistrictId;
            var result = _assignSurveyService.GetAssignSurveyBanksByUserID(districtId, CurrentUser.RoleId, CurrentUser.Id).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
