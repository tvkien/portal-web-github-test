using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using LinkIt.BubbleSheetPortal.Models.ManageTest;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Services.Survey;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Models;
using System;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Models.Enum;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.ViewModels.ReviewSurvey;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize]
    public class ReviewSurveyController : BaseController
    {
        private readonly ManageSurveyService _manageSurveyService;
        private readonly ReviewSurveyService _reviewSurveyService;
        private readonly DistrictService _districtService;
        private readonly DistrictTermService _districtTermService;
        private readonly VirtualTestService _virtualTestService;

        public ReviewSurveyController(
            ManageSurveyService manageSurveyService,
            ReviewSurveyService reviewSurveyService,
            DistrictService districtService,
            DistrictTermService districtTermService,
            VirtualTestService virtualTestService)
        {
            _manageSurveyService = manageSurveyService;
            _reviewSurveyService = reviewSurveyService;
            _districtService = districtService;
            _districtTermService = districtTermService;
            _virtualTestService = virtualTestService;
        }

        public ActionResult Index()
        {
            if (!CurrentUser.IsNetworkAdmin)
            {
                ViewBag.StateId = CurrentUser.StateId;
            }

            ViewBag.DistrictId = CurrentUser.DistrictId;
            ViewBag.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
            ViewBag.IsPublisher = CurrentUser.IsPublisher;

            return View();
        }

        public ActionResult GetSurveyAssignmentTypes()
        {
            var surveyAssignmentType = ((SurveyAssignmentTypeEnum[])Enum
                .GetValues(typeof(SurveyAssignmentTypeEnum)))
                .Where(o => o != SurveyAssignmentTypeEnum.Preview)
                .Select(o => new ListItem()
                {
                    Id = (int)o,
                    Name = EnumExtension.Description(o)
                }).ToList();

            return Json(surveyAssignmentType, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSurveyAssignments(ReviewSurveyRequestViewModel request)
        {
            var reviewSurveys = new List<ReviewSurveyViewModel>();

            var parser = new DataTableParserProc<ReviewSurveyViewModel>();

            if (request.DistrictId == null)
            {
                return Json(parser.Parse(new List<ReviewSurveyViewModel>().AsQueryable(), 0), JsonRequestBehavior.AllowGet);
            }

            var searchText = Request["sSearch"] ?? string.Empty;

            if (CurrentUser.RoleId != (int)Permissions.Publisher && CurrentUser.RoleId != (int)Permissions.NetworkAdmin || request.DistrictId == 0)
            {
                request.DistrictId = CurrentUser.DistrictId.GetValueOrDefault();
            }

            var results = _manageSurveyService
                .GetReviewSurveys(CurrentUser.Id, CurrentUser.RoleId, request.DistrictId, request.TermId, (int?)request.SurveyAssignmentType, request.SurveyBankId, request.SurveyId, request.ShowActiveAssignment, parser.SortableColumns, searchText, parser.StartIndex, parser.PageSize).ToList();
            
            int? totalRecords = 0;
            if (results != null && results.Count > 0)
            {
                totalRecords = results[0].TotalRecords;
            }

            var data = results.Select(x => new ReviewSurveyViewModel
            {
                VirtualTestId = x.VirtualTestId,
                SurveyName = x.SurveyName,
                TermName = x.TermName,
                SchoolName = x.SchoolName,
                AssignmentType = x.AssignmentType != null ? EnumExtension.Description((SurveyAssignmentTypeEnum)x.AssignmentType) : string.Empty,
                Assignments = x.Assignments,
                MostRecentResponse = x.MostRecentResponse,
                BankId = x.BankId,
                TermID = x.TermID,
                AssignmentTypeRawValue = x.AssignmentType
            }).AsQueryable();

            return Json(parser.Parse(data, totalRecords ?? 0), JsonRequestBehavior.AllowGet);
        }
    }
}
