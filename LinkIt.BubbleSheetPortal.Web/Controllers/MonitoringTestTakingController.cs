using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using DevExpress.Data.Linq;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Isolating;
using LinkIt.BubbleSheetPortal.Models.TestResultRemover;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Models.MonitoringTestTaking;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Web.Models.DataTable;
using LinkIt.BubbleSheetPortal.Models.Monitoring;
using LinkIt.BubbleSheetPortal.Models.DTOs.TestTaking;
using LinkIt.BubbleSheetPortal.Services;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize]
    [VersionFilter]
    public class MonitoringTestTakingController : BaseController
    {
        private readonly TestAssignmentControllerParameters _parameters;

        public MonitoringTestTakingController(TestAssignmentControllerParameters parameters)
        {
            _parameters = parameters;
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.OnlineTestMonitorTestTaking)]
        public ActionResult Index()
        {
            ViewBag.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
            ViewBag.IsPublisher = CurrentUser.IsPublisher();
            if (CurrentUser.IsNetworkAdmin)
            {
                ViewBag.ListDictrictIds = ConvertListIdToStringId(CurrentUser.GetMemberListDistrictId());
            }
            var refreshTime = _parameters.ConfigurationServices.GetConfigurationByKeyWithDefaultValue(
                "AutoRefreshTime", 180);
            ViewBag.RefreshTime = refreshTime * 1000;
            return View();
        }

        private string ConvertListIdToStringId(List<int> ListDistricIds)
        {
            var ids = string.Empty;
            if (!ListDistricIds.Any())
            {
                return ids;
            }
            ids = ListDistricIds.Aggregate(ids, (current, id) => current + (id + ","));
            return ids.TrimEnd(new[] { ',' });

        }

        private void MappingGetTestClassAssignmentsImproved(GetTestClassAssignmentCriteria criteria, GetTestClassAssignmentsOTTRequest request)
        {
            if (criteria == null || request == null) return;

            request.PageSize = criteria.iDisplayLength;
            request.StartRow = criteria.iDisplayStart;
            request.GeneralSearch = criteria.sSearch;

            if (!string.IsNullOrWhiteSpace(criteria.sColumns) && criteria.iSortCol_0.HasValue)
            {
                var columns = criteria.sColumns.Split(',');
                request.SortColumn = columns[criteria.iSortCol_0.Value];
                request.SortDirection = criteria.sSortDir_0.Equals("desc") ? "DESC" : "ASC";
            }

            request.AssignDate = GetDateTime(criteria.DateTime).ToString();

            request.DistrictID = CurrentUser.IsPublisher || CurrentUser.IsNetworkAdmin ? criteria.DistrictID : CurrentUser.DistrictId;
            request.SchoolID = criteria.SchoolID.HasValue && criteria.SchoolID.Value > 0 ? criteria.SchoolID.Value : 0;
            //request.OnlyShowPedingReview = criteria.OnlyShowPendingReview;
            request.ShowActiveClassTestAssignment = criteria.ShowActiveClassTestAssignment;
            request.UserID = CurrentUser.Id;

            request.AssignmentCodes = string.Format("{0};{1}", criteria.AssignmentCodes ?? string.Empty, criteria.Code ?? string.Empty);
            if (request.AssignmentCodes.Length == 1) request.AssignmentCodes = string.Empty;

            request.GradeName = criteria.GradeName;
            request.SubjectName = criteria.SubjectName;
            request.BankName = criteria.BankName;
            request.ClassName = criteria.ClassName;
            request.TeacherName = criteria.TeacherName;
            request.StudentName = criteria.StudentName;
            request.TestName = criteria.TestName;
        }

        [HttpPost]
        [AjaxOnly]
        public JsonResult GetTestClassAssignmentsImproved(GetTestClassAssignmentCriteria criteria)
        {
            var result = new GenericDataTableResponse<QTITestClassAssignmentOTT>()
            {
                sEcho = criteria.sEcho,
                sColumns = criteria.sColumns,
                aaData = new List<QTITestClassAssignmentOTT>(),
                iTotalDisplayRecords = 0,
                iTotalRecords = 0
            };

            if (criteria.PageLoad
                && (CurrentUser.IsPublisher || CurrentUser.IsNetworkAdmin || CurrentUser.IsDistrictAdmin))
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            var request = new GetTestClassAssignmentsOTTRequest();
            MappingGetTestClassAssignmentsImproved(criteria, request);

            var response = _parameters.QTITestClassAssignmentServices.GetTestClassAssignmentsOTT(request);
            var timeZoneId = _parameters.StateService.GetTimeZoneId(CurrentUser.StateId.GetValueOrDefault());
            var dateTimeFormat = _parameters.DistrictDecodeService.GetDateFormat(CurrentUser.DistrictId.GetValueOrDefault());
            response.Data.ForEach(x =>
            {
                if (x.AssignmentDate.HasValue)
                    x.Assigned = x.AssignmentDate.Value.ConvertTimeFromUtc(timeZoneId).DisplayDateWithFormat(dateTimeFormat.DateFormat, dateTimeFormat.TimeFormat, false);
            });
            result.aaData = response.Data;
            result.iTotalDisplayRecords = response.TotalRecord;
            result.iTotalRecords = response.TotalRecord;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult GetTestClassAssignments(TestAssignmentCriteria criteria)
        {
            var parser = new DataTableParser<QTITestClassAssignmentOTT>();

            var assignmentDateCompare = GetDateTime(criteria.DateTime);

            int? districtID = null;
            if (CurrentUser.IsPublisher || CurrentUser.IsNetworkAdmin)
            {
                districtID = criteria.DistrictID;
            }
            else if (CurrentUser.DistrictId.HasValue)
            {
                districtID = CurrentUser.DistrictId;
            }
            else
            {
                return Json(parser.Parse(new List<QTITestClassAssignmentOTT>().AsQueryable()),
                            JsonRequestBehavior.AllowGet);
            }

            var schoolID = criteria.SchoolID.HasValue && criteria.SchoolID.Value > 0 ? criteria.SchoolID.Value : 0;

            if ((!CurrentUser.IsPublisher && !CurrentUser.IsNetworkAdmin && !CurrentUser.IsDistrictAdmin)
                || !criteria.PageLoad.HasValue
                || !criteria.PageLoad.Value)
            {
                var data =
                _parameters.QTITestClassAssignmentServices.GetTestClassAssignmentsOTT(
                    assignmentDateCompare.ToString(), CurrentUser.Id, districtID, criteria.ShowActiveClassTestAssignment, schoolID);
                return Json(parser.Parse(data, true), JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = new List<QTITestClassAssignmentOTT>().AsQueryable();

                return Json(parser.Parse(data, true), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult GetTestClassAssignmentsRefresh(string qtiTestClassAssignmentIDs)
        {
            var data =
                _parameters.QTITestClassAssignmentServices.GetTestClassAssignmentsOTTRefresh(qtiTestClassAssignmentIDs).ToList();

            UpdateIsolatingData(data, qtiTestClassAssignmentIDs);

            GetTestClassAssignmentsRefresh_CombinePausedAndInactive(data);


            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private void GetTestClassAssignmentsRefresh_CombinePausedAndInactive(List<QTITestClassAssignmentOTTRefresh> data)
        {
            data.ForEach(c => c.Paused = (c.Paused ?? 0) + (c.NotActive ?? 0));
        }
        [HttpGet]
        [AjaxOnly]
        public ActionResult MonitoringTestGetPopupDetail(int qtiTestClassAssignmentId)
        {
            var isolatingDatas = GetSessionIdAndActiveStatus(qtiTestClassAssignmentId);
            IEnumerable<TestTakingPopupDetailDto> popupDetails = _parameters.QTITestClassAssignmentServices.MonitoringTestGetPopupDetail(qtiTestClassAssignmentId, isolatingDatas).ToList();

            return Json(popupDetails, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult ProctorTestView(ProctorViewModel model)
        {
            model.BranchingTest = CheckBranchingTest(model);

            return PartialView("ProctorTestView", model);
        }

        private bool CheckBranchingTest(ProctorViewModel model)
        {
            var preference =
                _parameters.QTITestClassAssignmentServices.GetPreferencesForOnlineTest(model.QTITestClassAssignmentID);
            var branchingTestPreference = preference == null ? null : preference.BranchingTest;
            var virtualTest = _parameters.QTITestClassAssignmentServices.GetVirtualTestByID(model.VirtualTestId);
            var branchingTest = virtualTest != null &&
                                virtualTest.NavigationMethodID == (int)NavigationMethodEnum.NORMAL_BRANCHING;
            if (branchingTestPreference != null) branchingTest = branchingTest && branchingTestPreference.Value;
            return branchingTest;
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult GetProctorTestViewData(ProctorViewModel model)
        {
            var data = _parameters.QTITestClassAssignmentServices.GetProctorTestViewData(model.QTITestClassAssignmentID);
            model.Students = new List<ProctorViewStudent>();
            if (data == null || data.Count == 0) return Json(model, JsonRequestBehavior.AllowGet);

            var onlineTestSessionIDs =
                data.Where(x => x.StatusID != null && (x.StatusID == 1 || x.StatusID == 2 || x.StatusID == 3 || x.StatusID == 5) && x.QTIOnlineTestSessionID != null)
                    .Select(x => x.QTIOnlineTestSessionID.Value.ToString()).ToList();

            IQueryable<OnlineTestSessionAnswerDTO> onlineTestSessionAnswers = null;
            var inactiveMinute = 10;

            if (onlineTestSessionIDs.Any())
            {
                onlineTestSessionAnswers =
                    _parameters.IsolatingTestTakerService.GetOnlineTestSessionAnswer(string.Join(",",
                        onlineTestSessionIDs.ToList()));

                var configuration = _parameters.ConfigurationServices.GetConfigurationByKey("InActiveTestTakingTime");
                if (configuration != null)
                {
                    int.TryParse(configuration.Value, out inactiveMinute);
                }
            }

            model.Students = Transform(data, onlineTestSessionAnswers, inactiveMinute);
            model.Students = model.Students.OrderBy(o => o.StudentName).ToList();

            var preference =
                _parameters.QTITestClassAssignmentServices.GetPreferencesForOnlineTest(model.QTITestClassAssignmentID);
            model.QuestionNumberLabel = preference.QuestionNumberLabel;

            var timeZoneId = _parameters.StateService.GetTimeZoneId(CurrentUser.StateId.GetValueOrDefault());
            model.LastUpdated = DateTime.UtcNow.ConvertTimeFromUtc(timeZoneId).DisplayDateWithFormat(true);

            var result = new LargeJsonResult
            {
                Data = model,
                MaxJsonLength = int.MaxValue,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

            return result;
        }

        private bool IsActiveSession(int inActiveMinutes, DateTime? lastLoginDate, DateTime? timeStamp)
        {
            var dtInactive = DateTime.UtcNow.AddMinutes(-inActiveMinutes);

            var result = lastLoginDate >= dtInactive || timeStamp >= dtInactive;

            return result;
        }

        private List<ProctorViewStudent> Transform(List<Data.Entities.GetProctorTestViewDataResult> data, IQueryable<OnlineTestSessionAnswerDTO> isolatingAnswers, int inactiveMinute)
        {
            var result = new List<ProctorViewStudent>();
            foreach (var obj in data)
            {
                var student = Transform(obj, isolatingAnswers, inactiveMinute);
                result.Add(student);
            }

            return result;
        }

        private ProctorViewStudent Transform(Data.Entities.GetProctorTestViewDataResult obj, IQueryable<OnlineTestSessionAnswerDTO> isolatingAnswers, int inactiveMinute)
        {
            var student = new ProctorViewStudent
            {
                StudentName = obj.StudentName,
                StudentID = obj.StudentID,
                Questions = ParseQuestions(obj.Questions),
                GradingProcessSuccess = true
            };

            AutoGradingQueueData autoGradingQueue = null;
            if (obj.QTIOnlineTestSessionID.HasValue)
            {
                student.QTIOnlineTestSessionID = obj.QTIOnlineTestSessionID.Value;
                student.TestStatus = obj.StatusID.ToString();
                student.Active = (obj.Active.HasValue && obj.Active.Value) ? 1 : 0;
                student.AutoGrading = (obj.AutoGrading.HasValue && obj.AutoGrading.Value) ? 1 : 0;
                autoGradingQueue =
                   _parameters.QTIOnlineTestSessionService.GetAutoGradingQueueByQTOnlineTestSessionID(
                       obj.QTIOnlineTestSessionID.Value);
                student.GradingProcessStatus = (int)_parameters.QTITestClassAssignmentServices.GetGradingProcessStatus(obj.QTIOnlineTestSessionID.Value);
            }

            if (isolatingAnswers != null && isolatingAnswers.Any())
            {
                student.GradingProcessSuccess = !IsGradingProcessFail(autoGradingQueue, obj.StatusID, true);
                var currentTestSessionAnswers =
                    isolatingAnswers.Where(x => x.QTIOnlineTestSessionID == obj.QTIOnlineTestSessionID);
                if (currentTestSessionAnswers.Any())
                {
                    var statusId = currentTestSessionAnswers.Max(x => x.StatusID);
                    if (statusId.HasValue && statusId.Value == 1)
                        student.Active = 0;
                    else if (statusId.HasValue && statusId.Value == 2)
                    {
                        var isActive = IsActiveSession(inactiveMinute,
                            currentTestSessionAnswers.Max(x => x.LastLoginDate),
                            currentTestSessionAnswers.Max(x => x.TimeStamp));
                        student.Active = isActive ? 1 : 0;
                    }
                    student.Questions = Transform(currentTestSessionAnswers, student.Questions);
                }
            }

            return student;
        }

        private bool IsGradingProcessFail(AutoGradingQueueData autoGradingQueue, int? testStatus, bool hasDataOnIsolatingDb)
        {
            if (!testStatus.HasValue || autoGradingQueue == null) return false;
            var result = testStatus == 5 && hasDataOnIsolatingDb && autoGradingQueue.Status == -1
                && !autoGradingQueue.IsAwaitingRetry;
            return result;
        }

        private List<ProctorViewQuestion> Transform(IQueryable<OnlineTestSessionAnswerDTO> answers, List<ProctorViewQuestion> questions)
        {
            var result = new List<ProctorViewQuestion>();
            if (answers == null) return result;

            foreach (var answer in answers)
            {
                var proctorViewQuestion = Transform(answer);
                var question = questions.Where(x => x.QuestionID == answer.VirtualQuestionID).FirstOrDefault();
                proctorViewQuestion.QuestionGroupId = question == null ? 0 : question.QuestionGroupId;
                result.Add(proctorViewQuestion);
            }

            return result.OrderBy(x => x.QuestionOrder).ToList();
        }

        private ProctorViewQuestion Transform(OnlineTestSessionAnswerDTO source)
        {
            if (source == null) return null;

            var proctorViewQuestion = new ProctorViewQuestion
            {
                QuestionID = source.VirtualQuestionID,
                QuestionOrder = source.QuestionOrder ?? 0,
                AnswerOrder = source.AnswerOrder ?? 0,
                Answered =
                                     source.Answered != null && source.Answered.Value,
                ManualReview =
                                     source.ManualReview != null &&
                                     source.ManualReview.Value,
                TimeSpent = source.TimeSpent ?? 0,
                TimesVisited = source.TimesVisited ?? 0
            };

            return proctorViewQuestion;
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult PauseOnlineTest(List<int?> qtiOnlineTestSessionIDs)
        {
            var pausedSuccess = _parameters.QTIOnlineTestSessionService.PauseOnlineTests(qtiOnlineTestSessionIDs);

            var onlineTestSessionIDs = string.Join(",",
                pausedSuccess.Select(x => x.ToString()));

            _parameters.IsolatingTestTakerService.PausedOnlineTest(onlineTestSessionIDs);

            return Json(new { Success = true, Data = new { Resquest = qtiOnlineTestSessionIDs, Response = pausedSuccess } }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult SubmitOnlineTest(List<int?> qtiOnlineTestSessionIDs)
        {
            //_parameters.QTIOnlineTestSessionService.SubmitOnlineTests(qtiOnlineTestSessionIDs);
            List<int> sessionIDSubmitted = _parameters.QTIOnlineTestSessionService.TeacherSubmitOnlineTests(qtiOnlineTestSessionIDs, CurrentUser.Id);

            var onlineTestSessionIDs = string.Join(",",
                sessionIDSubmitted.Select(x => x.ToString()));

            _parameters.IsolatingTestTakerService.SubmitOnlineTest(onlineTestSessionIDs);

            return Json(new { Success = true, Count = sessionIDSubmitted.Count }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult RedoFailedGradingProcess(List<int?> qtiOnlineTestSessionIDs)
        {
            if (qtiOnlineTestSessionIDs == null || qtiOnlineTestSessionIDs.Count == 0)
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            //check
            var isExist = _parameters.QTITestClassAssignmentServices.IsExistAutoGradingQueueBeingGraded(qtiOnlineTestSessionIDs);
            if (isExist)
                return Json(new { Success = false }, JsonRequestBehavior.AllowGet);

            _parameters.QTIOnlineTestSessionService.RedoFailedGradingProcess(qtiOnlineTestSessionIDs);

            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult ReopenFailedGradingProcess(List<int?> qtiOnlineTestSessionIDs)
        {
            _parameters.QTIOnlineTestSessionService.ReopenFailedGradingProcess(qtiOnlineTestSessionIDs);
            _parameters.IsolatingTestTakerService.IsolatingReopenFailedTestSessions(qtiOnlineTestSessionIDs);

            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        public List<ProctorViewQuestion> ParseQuestions(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return new List<ProctorViewQuestion>();
            var xdoc = XDocument.Parse(str);
            var result = new List<ProctorViewQuestion>();
            var questionsElement = xdoc.Element("Questions");
            if (questionsElement == null) return result;
            foreach (var item in questionsElement.Elements("Question"))
            {
                var dto = new ProctorViewQuestion
                {
                    QuestionID = GetIntValue(item.Element("VirtualQuestionID")),
                    QuestionOrder = GetIntValue(item.Element("QuestionOrder")),
                    AnswerOrder = GetIntValue(item.Element("AnswerOrder")),
                    Answered = GetBoolValue(item.Element("Answered")),
                    ManualReview = GetBoolValue(item.Element("ManualReview")),
                    QuestionGroupId = GetIntValue(item.Element("QuestionGroupID")),
                    TimeSpent = GetIntValue(item.Element("TimeSpent")),
                    TimesVisited = GetIntValue(item.Element("TimesVisited"))
                };

                result.Add(dto);
            }

            return result;
        }

        [HttpGet]
        public ActionResult GetDateTimeFromDayOffset(int days)
        {
            var date = DateTime.UtcNow.AddDays(-days).ToShortDateString();
            if (days == 0) { date = DateTime.MinValue.AddYears(1800).ToShortDateString(); }
            return Json(new { Date = date }, JsonRequestBehavior.AllowGet);
        }

        public DateTime GetDateTime(int days)
        {
            if (days == 0) return DateTime.MinValue.AddYears(1800);
            return DateTime.UtcNow.AddDays(-days).Date;
        }

        private bool GetBoolValue(XElement element)
        {
            if (element == null) return false;
            return element.Value == "1" || element.Value == "true";
        }

        private int GetIntValue(XElement element)
        {
            if (element == null) return 0;
            return Convert.ToInt32(element.Value);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult ReopenTest(ProctorViewModel model)
        {
            if (model.QTIOnlineTestSessionIDs == null) return Json(new { Success = true, TotalReopenTests = 0 }, JsonRequestBehavior.AllowGet);
            var branchingTest = CheckBranchingTest(model);
            if (branchingTest) return Json(new { Success = false, TotalReopenTests = 0 }, JsonRequestBehavior.AllowGet);

            //parse CompleteXmlcontent to detect drawable image
            var imageIndexs = GetImageIndexInComplex(model.VirtualTestId);

            var totalReopenTests = 0;
            foreach (var qtiOnlineTestSessionID in model.QTIOnlineTestSessionIDs)
            {
                if (qtiOnlineTestSessionID == null) continue;
                var canReopen = _parameters.QTIOnlineTestSessionService.CanReopenTest(qtiOnlineTestSessionID.Value);
                if (!canReopen) continue;

                var testResultAuditModel = new TestResultAuditModel
                {
                    QTIOnlineTestSessionID = qtiOnlineTestSessionID.Value,
                    UserID = CurrentUser.Id
                };

                var visitorsIPAddr = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrWhiteSpace(visitorsIPAddr))
                {
                    visitorsIPAddr = Request.ServerVariables["REMOTE_ADDR"];
                }
                testResultAuditModel.VisitorsIPAddr = visitorsIPAddr;

                _parameters.QTIOnlineTestSessionService.LogTestResultDataBeforeRemove(testResultAuditModel, ContaintUtil.Reopen);

                _parameters.QTIOnlineTestSessionService.ReopenTest(qtiOnlineTestSessionID.Value, imageIndexs);

                totalReopenTests++;
            }

            return Json(new { Success = true, TotalReopenTests = totalReopenTests }, JsonRequestBehavior.AllowGet);
        }

        private List<ImageIndexInQuestion> GetImageIndexInComplex(int virtualTestId)
        {
            var result = new List<ImageIndexInQuestion>();
            var complexs = _parameters.VirtualTestService.GetConstructedResponseQuestion(virtualTestId);
            XmlSpecialCharToken xmlSpecialCharToken = new XmlSpecialCharToken();
            foreach (var complex in complexs)
            {
                string strXmlContent = complex.XmlContent.RemoveLineBreaks().ReplaceWeirdCharacters();
                strXmlContent = strXmlContent.ReplaceXmlSpecialChars(xmlSpecialCharToken);
                strXmlContent = strXmlContent.Replace("&nbsp;", "&#160;");
                var imageIndexs = _parameters.XmlContentSerializer.GetImageIndexInQuestion(strXmlContent);
                result.AddRange(imageIndexs.Select(x => new ImageIndexInQuestion()
                {
                    VirtualQuestionId = complex.VirtualQuestionId,
                    ResponseIdentifier = x.Key,
                    Index = x.Value
                }));
            }
            return result;
        }

        private void UpdateIsolatingData(IEnumerable<QTITestClassAssignmentOTTRefresh> data, string qtiTestClassAssignmentIDs)
        {
            var onlineTestSessionIsolating =
                _parameters.IsolatingTestTakerService.GetOnlineTestSessionStatusIsolating(qtiTestClassAssignmentIDs).Where(x => x.StatusId == 1 || x.StatusId == 2);

            if (onlineTestSessionIsolating.Any())
            {
                var inactiveMinute = 10;
                var configuration = _parameters.ConfigurationServices.GetConfigurationByKey("InActiveTestTakingTime");
                if (configuration != null)
                {
                    int.TryParse(configuration.Value, out inactiveMinute);
                }

                foreach (var item in data)
                {
                    var testClassAssignmentId = item.QTITestClassAssignmentID;
                    var dtInactive = DateTime.UtcNow.AddMinutes(-inactiveMinute);

                    var assignmentQuery =
                        onlineTestSessionIsolating.Where(o => o.QTITestClassAssignmentId == testClassAssignmentId);
                    //get all TestSessionID
                    var assignment =
                        _parameters.QTITestClassAssignmentServices.GetQtiTestClassAssignment(testClassAssignmentId.Value);
                    var qtiOnlineTestSessionIDs =
                        _parameters.QTIOnlineTestSessionService.GetListQTIOnlineTestSession(assignment.AssignmentGuId).Where(x => x.StatusId == 1 || x.StatusId == 2);

                    var active = 0;
                    var notActive = 0;
                    foreach (var qtiOnlineTestSession in qtiOnlineTestSessionIDs)
                    {
                        var onlineSessionIsolating = assignmentQuery.FirstOrDefault(x => x.QTIOnlineTestSessionId == qtiOnlineTestSession.QTIOnlineTestSessionId);
                        if (onlineSessionIsolating == null || (onlineSessionIsolating != null && onlineSessionIsolating.StatusId == 1)) //case reopen or just click Begin started test
                        {
                            notActive += 1;
                        }
                        else //statusId = 2
                        {
                            if ((onlineSessionIsolating.LastLoginDate != null &&
                                 onlineSessionIsolating.LastLoginDate > dtInactive)
                                ||
                                (onlineSessionIsolating.Timestamp != null &&
                                 onlineSessionIsolating.Timestamp > dtInactive))
                                active += 1;
                            else
                            {
                                notActive += 1;
                            }
                        }
                    }
                    item.Started = active;
                    item.NotActive = notActive;
                }
            }
        }

        private IEnumerable<SessionIdAndActiveStatusDto> GetSessionIdAndActiveStatus(int qtiTestClassAssignmentId)
        {
            var onlineTestSessionIsolating =
                _parameters.IsolatingTestTakerService.GetOnlineTestSessionStatusIsolating(qtiTestClassAssignmentId.ToString()).Where(x => x.StatusId == 1 || x.StatusId == 2);

            if (!onlineTestSessionIsolating.Any())
            {
                return new SessionIdAndActiveStatusDto[0];
            }
            var assignmentQuery =
                onlineTestSessionIsolating.Where(o => o.QTITestClassAssignmentId == qtiTestClassAssignmentId)
                .Select(c => new SessionIdAndActiveStatusDto
                {
                    QTIOnlineTestSessionId = c.QTIOnlineTestSessionId,
                    LastLoginDate = c.LastLoginDate,
                    Timestamp = c.Timestamp,
                    StatusId = c.StatusId
                }).ToArray();

            return assignmentQuery;
        }
    }
}
