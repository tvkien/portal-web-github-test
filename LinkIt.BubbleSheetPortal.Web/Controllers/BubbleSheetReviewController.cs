using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using LinkIt.BubbleSheetPortal.Web.Models.BubbleSheetReview;
using LinkIt.BubbleSheetPortal.Web.Models.DataTable;
using LinkIt.BubbleSheetPortal.Models.BubbleSheetReview;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.ReadOnly)]
    [AjaxAwareAuthorize]
    [VersionFilter]
    public class BubbleSheetReviewController : ReviewBubbleSheetBaseController
    {
        private readonly BubbleSheetService bubbleSheetService;
        private readonly BubbleSheetStudentResultsService bubbleSheetStudentResultsService;
        private readonly VulnerabilityService vulnerabilityService;

        public BubbleSheetReviewController(BubbleSheetListService bubbleSheetListService, BubbleSheetService bubbleSheetService, VulnerabilityService vulnerabilityService, UserService userService,
            BubbleSheetStudentResultsService bubbleSheetStudentResultsService, DistrictDecodeService districtDecodeService, SchoolService schoolService)
            : base(bubbleSheetListService, bubbleSheetService, vulnerabilityService, userService, districtDecodeService)
        {
            this.bubbleSheetService = bubbleSheetService;
            this.bubbleSheetStudentResultsService = bubbleSheetStudentResultsService;
            this.vulnerabilityService = vulnerabilityService;
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.ManagebubblesheetsReview)]
        public ActionResult Index()
        {
            ViewBag.IsPublisher = false;
            ViewBag.IsNetworkAdmin = false;
            ViewBag.ListDictrictIds = new List<int>();
            if(CurrentUser.IsPublisher())
                ViewBag.IsPublisher = true;
            if (CurrentUser.IsNetworkAdmin)
            {
                ViewBag.IsNetworkAdmin = true;
                ViewBag.ListDictrictIds = ConvertListIdToStringId(CurrentUser.GetMemberListDistrictId());
            }

            ViewBag.IsTeacher = CurrentUser.IsTeacher;
            return View();
        }

        [HttpGet]
        public ActionResult GetBubbleSheetReviewList(bool archived, int? districtId, int? schoolId, int? duration)
        {
            var date = duration.HasValue ? duration.Value : 0;
            var dateTimeFilter = GetDateTimeFromDayOffsetNew(date);

            var vDistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;
            if (districtId != null && districtId > 0)
                vDistrictId = districtId.Value;

            var schoolParameter = schoolId.HasValue ? schoolId.Value : 0;

            var data = GetBubbleSheetReviewListData(vDistrictId, schoolParameter, archived, dateTimeFilter);
            //if (!archived)
            //{
            //    data = data.Where(x => x.IsArchived.Equals(false));
            //}
            var parser = new DataTableParser<BubbleSheetReviewListViewModel>();
            return new JsonNetResult{Data = parser.Parse(data)};
        }

        public ActionResult GetBubbleSheetReviewList_IncludedStatus(GetBubbleSheetReviewCriteria bbsCriteria)
        {
            var result = new GenericDataTableResponse<BubbleSheetReviewListViewModel>()
            {
                sEcho = bbsCriteria.sEcho,
                sColumns = bbsCriteria.sColumns,
                aaData = new List<BubbleSheetReviewListViewModel>(),
                iTotalDisplayRecords = 0,
                iTotalRecords = 0
            };

            var request = new GetBubbleSheetReviewRequest();
            MappingGetBubbleSheetReviewCriteria(bbsCriteria, request);

            var response = GetBubbleSheetReviewListDataIncludedStatus(request);

            result.aaData = response.Data.Select(x => new BubbleSheetReviewListViewModel {
                BankID = x.BankID,
                Ticket = x.Ticket,
                Grade = x.GradeName,
                Subject = x.SubjectName,
                Bank = x.BankName,
                Test = x.TestName,
                Class = x.ClassName,
                ClassId = x.ClassId,
                Teacher = x.ClassId > 0 ? x.TeacherName : ParseGroupTeacherName(x.GroupTeacherName),
                DateCreated = x.DateCreated,
                DateUpdated = x.DateUpdated,
                GradedCount = x.GradedCount + " / " + x.TotalSheets,
                ClassIds = x.ClassIds,
                IsArchived = x.IsArchived,
                IsDownloadable = !x.IsManualEntry,
                UnmappedSheetCount = x.UnmappedCount,
                VirtualTestSubTypeID = x.VirtualTestSubTypeID ?? 1,
                Fini = x.Fini.HasValue ? x.Fini.Value.ToString() : "0",
                Review = x.Review.HasValue ? x.Review.Value.ToString() : "0",
                Ungraded = x.Ungraded.HasValue ? x.Ungraded.Value.ToString() : "0"
            }).ToList();

            result.iTotalRecords = response.TotalRecord;
            result.iTotalDisplayRecords = response.TotalRecord;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private void MappingGetBubbleSheetReviewCriteria(GetBubbleSheetReviewCriteria criteria, GetBubbleSheetReviewRequest request)
        {
            if (criteria == null || request == null) return;

            request.PageSize = criteria.iDisplayLength;
            request.StartRow = criteria.iDisplayStart;
            request.GeneralSearch = criteria.sSearch;

            if(!string.IsNullOrWhiteSpace(criteria.sColumns) && criteria.iSortCol_0.HasValue)
            {
                var columns = criteria.sColumns.Split(',');
                request.SortColumn = columns[criteria.iSortCol_0.Value];
                request.SortDirection = criteria.sSortDir_0.Equals("desc") ? "DESC" : "ASC";
            }

            var date = criteria.duration.HasValue ? criteria.duration.Value : 0;
            request.CreatedDate = GetDateTimeFromDayOffsetNew(date);

            request.DistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;
            if (criteria.districtId != null && criteria.districtId > 0)
                request.DistrictId = criteria.districtId.Value;

            request.RoleId = CurrentUser.RoleId;
            request.UserId = CurrentUser.Id;
            request.SchoolId = criteria.schoolId.HasValue ? criteria.schoolId.Value : 0;

            request.Archived = criteria.archived;
            request.BankName = criteria.BankName;
            request.ClassName = criteria.ClassName;
            request.GradeName = criteria.GradeName;
            request.TestName = criteria.TestName;
            request.TeacherName = criteria.TeacherName;
            request.SubjectName = criteria.SubjectName;
        }

        [HttpGet]
        public JsonResult GetStudentTest(string ticket, int classID)
        {
            if (CheckUserCanAccessClass(CurrentUser.Id, CurrentUser.RoleId, classID) == false)
            {
                return Json(new { Finish = 0, Review = 0, NotGraded = 0 }, JsonRequestBehavior.AllowGet);
            }

            var status = bubbleSheetStudentResultsService.GetBubbleSheetReviewStatusCount(ticket, classID);

            return Json(new {Finish = status.Finished, Review = status.Review, NotGraded = status.Ungraded},
                JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult ArchiveTest(string ticket, int? classId)
        {
            var sheets = bubbleSheetService.GetBubbleSheetsByTicketAndClass(ticket, classId.GetValueOrDefault()).ToList();
            if (sheets.Count.Equals(0))
            {
                return Json(false);
            }
            foreach (var bubbleSheet in sheets)
            {
                bubbleSheet.IsArchived = !bubbleSheet.IsArchived;
            }
            bubbleSheetService.ToggleArchivedSheets(sheets);
            return Json(true);
        }

        [HttpGet]
        public ActionResult GetDateTimeFromDayOffset(int days)
        {
            var date = DateTime.UtcNow.AddDays(-days).ToShortDateString();
            if (days == 0) { date = DateTime.MinValue.AddYears(1800).ToShortDateString(); }
            return Json(new { Date = date }, JsonRequestBehavior.AllowGet);
        }

        public string ConvertListIdToStringId(List<int> ListDistricIds)
        {
            var ids = string.Empty;
            if (!ListDistricIds.Any())
            {
                return ids;
            }
            ids = ListDistricIds.Aggregate(ids, (current, id) => current + (id + ","));
            return ids.TrimEnd(new[] { ',' });
            
        }
        private DateTime GetDateTimeFromDayOffsetNew(int days)
        {
            if (days == 0) 
                return DateTime.MinValue.AddYears(1800);
            return DateTime.UtcNow.AddDays(-days);
        }


    }
}
