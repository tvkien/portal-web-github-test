using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.StudentOnlineTesting;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Security;
using Microsoft.IdentityModel.JsonWebTokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize]
    [VersionFilter]
    [SessionState(SessionStateBehavior.Required)]
    public class StudentOnlineTestingController : BaseController
    {
        private readonly QTITestClassAssignmentService _qTITestClassAssignmentServices;
        private readonly StudentMetaService _studentMetaServices;
        private readonly APIAccountService _apiAccountServices;
        private readonly DistrictDecodeService _districtDecodeService;
        private readonly TestAssignmentControllerParameters _parameters;

        public StudentOnlineTestingController(QTITestClassAssignmentService qTITestClassAssignmentServices,
            StudentMetaService studentMetaServices, APIAccountService apiAccountServices, DistrictDecodeService districtDecodeService, TestAssignmentControllerParameters parameters)
        {
            _qTITestClassAssignmentServices = qTITestClassAssignmentServices;
            _studentMetaServices = studentMetaServices;
            _apiAccountServices = apiAccountServices;
            _districtDecodeService = districtDecodeService;
            _parameters = parameters;
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.OnlineTesting)]
        public ActionResult Index()
        {
            if (!CurrentUser.IsStudent)
                return RedirectToAction("Index", "Student");

            var districtId = CurrentUser.DistrictId ?? 0;

            var districtDecode = _districtDecodeService.GetDistrictDecodeOfDistrictOrConfigurationByLabel(districtId,
                Util.DistrictDecode_OpenAllApplicationInSameTab);

            ViewBag.DistrictId = districtId;

            ViewBag.EnableSameTab = districtDecode != null ? districtDecode.Value : string.Empty;

            return View();
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult GetTestClassAssignmentsForStudent()
        {
            var parser = new DataTableParser<StudentOnlineTestingViewDto>();
            var result = new List<StudentOnlineTestingViewDto>();

            var studentId = _studentMetaServices.GetStudentIdViaStudentUser(CurrentUser.Id);
            if (studentId > 0)
            {
                IEnumerable<QTITestClassAssignmentForStudent> data =
                    _qTITestClassAssignmentServices.GetTestClassAssignmentsForStudent(studentId,
                        CurrentUser.DistrictId.GetValueOrDefault()).ToArray();

                string apiPrefix = LinkitConfigurationManager.Vault.DatabaseID;
                APIAccount apiAccount =
                    _apiAccountServices.GetAPIAccountByDistrictId(CurrentUser.DistrictId.GetValueOrDefault());
                if (data.Any() && apiAccount != null)
                {
                    var isNewSkin = _parameters.DistrictDecodeService.DistrictSupportTestTakerNewSkin(CurrentUser.DistrictId.GetValueOrDefault());
                    CheckTestAssignments(ref data);
                    var validDatas = data.Where(c => c.IsValid).ToArray();
                    foreach (var validData in validDatas)
                    {
                        var timestamp = DateTime.UtcNow.ToString();

                        if (!string.IsNullOrEmpty(validData.Code))
                        {
                            var redirectUrl = string.Format("{0}://{1}/{2}", HelperExtensions.GetHTTPProtocal(Request), Request.Url.Authority, "StudentOnlineTesting");
                            var privateKey = apiAccount.LinkitPrivateKey;
                            var rawDataObject = new
                            {
                                StudentID = studentId,
                                TestCode = validData.RawCode,
                                AssignmentGUID = validData.AssignmentGUID,
                                RedirectUrl = redirectUrl,
                                Timestamp = timestamp,
                                IsFromSPP = true,
                                LogoutUrl = string.Format("{0}://{1}.{2}{3}", HelperExtensions.GetHTTPProtocal(Request), HelperExtensions.GetSubdomain().ToLower(), ConfigurationManager.AppSettings["LinkItUrl"], ConfigurationManager.AppSettings["PortalLogOffUrl"]),
                                OpenFromReporting = false,
                                HasValidateAuthenticatorCode = true
                            };
                            string rawData = JsonConvert.SerializeObject(rawDataObject, Formatting.Indented);

                            string strData = Util.EncryptString(rawData, privateKey);
                            var url = ConfigurationManager.AppSettings["TestLaunchURL"];
                            var testTakerUrl = string.Format(url, HttpUtility.UrlEncode(strData), HttpUtility.UrlEncode(apiAccount.ClientAccessKeyID.ToString()), apiPrefix);
                            if (isNewSkin) testTakerUrl += "&new_skin";
                            validData.RedirectUrl = testTakerUrl;
                        }
                        result.Add(new StudentOnlineTestingViewDto()
                        {
                            AssignmentDate = validData.AssignmentDate,
                            AssignmentFirstName = validData.AssignmentFirstName,
                            AssignmentGUID = validData.AssignmentGUID,
                            AssignmentLastName = validData.AssignmentLastName,
                            AssignmentModifiedUserID = validData.AssignmentModifiedUserID,
                            ClassName = validData.ClassName,
                            Code = validData.Code,
                            DistrictTermDateEnd = validData.DistrictTermDateEnd,
                            DistrictTermDateStart = validData.DistrictTermDateStart,
                            ErrorMsg = validData.ErrorMsg,
                            IsValid = validData.IsValid,
                            QTIOnlineTestSessionID = validData.QTIOnlineTestSessionID,
                            QTITestClassAssignmentID = validData.QTITestClassAssignmentID,
                            RedirectUrl = validData.RedirectUrl,
                            StartDate = validData.StartDate,
                            Status = validData.Status,
                            TeacherName = validData.TeacherName,
                            TestName = validData.TestName,
                            IsTutorialMode = validData.IsTutorialMode
                        });
                    }
                }
            }
            return Json(parser.Parse(result.AsQueryable(), true), JsonRequestBehavior.AllowGet);
        }
        private void CheckTestAssignments(ref IEnumerable<QTITestClassAssignmentForStudent> qTITestClassAssignmentForStudent)
        {
            foreach (var assignment in qTITestClassAssignmentForStudent)
            {
                assignment.IsValid = true;
            }

            var testSessionIds = qTITestClassAssignmentForStudent
                .Select(c => c.QTIOnlineTestSessionID)
                .Where(c => c.HasValue).Select(c => c.Value)
                .Distinct()
                .ToArray();

            var assignmentHasNoAutoGradingQueue = GetAllTestSessionIdHasNoAutoGradingQueue(testSessionIds);

            var testClassIdOfAssignmentThatHasNoSessionId = qTITestClassAssignmentForStudent
                .Where(c => !c.QTIOnlineTestSessionID.HasValue)
                .Select(c => c.QTITestClassAssignmentID)
                .ToArray();

            var qTITestClassAssignmentIdHasNoAutoGrading = (from assignment in qTITestClassAssignmentForStudent
                                                            join id in assignmentHasNoAutoGradingQueue
                                                            on assignment.QTIOnlineTestSessionID equals id
                                                            select assignment.QTITestClassAssignmentID)
                                                            .ToArray()
                                                            .Concat(testClassIdOfAssignmentThatHasNoSessionId)
                                                            .Distinct()
                                                            .ToArray();

            var preferences = _parameters.QTITestClassAssignmentServices
                .GetPreferencesForOnlineTest(qTITestClassAssignmentIdHasNoAutoGrading);

            var preferencesWithAssignments = (from assignment in qTITestClassAssignmentForStudent
                                              join preference in preferences
                                              on assignment.QTITestClassAssignmentID equals preference.Key
                                              select new
                                              {
                                                  assignment,
                                                  preference = preference.Value
                                              }).ToArray();

            var nowInSchoolTimeZone = new Dictionary<int, DateTime>();
            foreach (var preferenceWithAssignment in preferencesWithAssignments)
            {
                var preference = preferenceWithAssignment.preference;
                var assignment = preferenceWithAssignment.assignment;

                var classDetail = _parameters.ClassServices.GetClassById(assignment.ClassId);
                DateTime schoolNowDateTime = DateTime.UtcNow;

                if (classDetail != null)
                {
                    var existing = nowInSchoolTimeZone.TryGetValue(classDetail.SchoolId.GetValueOrDefault(), out schoolNowDateTime);
                    if (!existing)
                    {
                        schoolNowDateTime = _parameters.SchoolService.GetCurrentDateTimeBySchoolId(classDetail.SchoolId.GetValueOrDefault());
                        nowInSchoolTimeZone.Add(classDetail.SchoolId.GetValueOrDefault(), schoolNowDateTime);
                    }
                }

                string errorMessage = GetErrorString(assignment, preference, schoolNowDateTime);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    assignment.IsValid = false;
                    assignment.ErrorMsg = errorMessage;
                }
            }
        }

        private string GetErrorString(QTITestClassAssignmentForStudent assignment, PreferenceOptions preference, DateTime schoolNowDateTime)
        {
            if (IsDistrictTermExpired(assignment.DistrictTermDateStart, assignment.DistrictTermDateEnd, DateTime.UtcNow, preference.TestScheduleTimezoneOffset.GetValueOrDefault()))
            {
                return "District term was expired";
            }
            if (preference != null)
            {
                double timeRemain = 0;
                //Check Time Limit
                if (preference.TimeLimit.HasValue && preference.TimeLimit.Value)
                {
                    string errorTimeLimit = "Test Period for this session has expired.";
                    if (preference.Deadline.HasValue && preference.Deadline > DateTime.MinValue)
                    {
                        timeRemain = preference.Deadline.Value.Subtract(schoolNowDateTime).TotalSeconds;
                        if (timeRemain < 0)
                        {
                            return errorTimeLimit;
                        }
                    }
                    else if (preference.Duration.HasValue && preference.Duration > 0 && assignment.QTIOnlineTestSessionID.HasValue)
                    {
                        int totalSpentTime = _qTITestClassAssignmentServices.GetTotalSpentTimeByQTIOnlineTestSessionID(assignment.QTIOnlineTestSessionID.Value);
                        if (totalSpentTime > 0)
                        {
                            timeRemain = (preference.Duration.Value * 60) - totalSpentTime;
                            if (timeRemain < 0)
                            {
                                return errorTimeLimit;
                            }
                        }
                    }
                }

                //Check Time TestSchedule
                if (!ValidTestSchedule(preference, schoolNowDateTime))
                {
                    return "This test is currently inactive. Please contact your teacher to activate the test.";
                }

                if (assignment.QTIOnlineTestSessionID.HasValue && preference.SectionAvailability)
                {
                    var sectionSubmiteds = _parameters.GetOnlineTestSessionDynamoService.GetSectionSubmiteds(assignment.QTIOnlineTestSessionID.Value);
                    if (!preference.OpenSections.Any() || !preference.OpenSections.Any(x => !sectionSubmiteds.Contains(x)))
                    {
                        return "No section available";
                    }
                }
            }
            return string.Empty;
        }

        private bool IsDistrictTermExpired(DateTime? districtTermDateStart, DateTime? districtTermDateEnd, DateTime utcNow, double timezoneOffset)
        {
            var localNow = utcNow.AddMinutes(timezoneOffset);
            if (districtTermDateStart.HasValue && districtTermDateStart.Value > localNow)
            {
                return true;
            }
            if (districtTermDateEnd.HasValue && districtTermDateEnd.Value < localNow)
            {
                return true;
            }

            return false;
        }

        private bool ValidTestSchedule(PreferenceOptions preference, DateTime schoolNowDateTime)
        {
            if (preference.TestSchedule != null
                && preference.TestScheduleFromDate != null
                && preference.TestScheduleToDate != null
                && preference.TestScheduleDayOfWeek != null
                && preference.TestScheduleTimezoneOffset != null
                && preference.TestSchedule.Value)
            {
                DateTime strStartDate = preference.TestScheduleFromDate.GetValueOrDefault();
                DateTime strEndDate = preference.TestScheduleToDate.GetValueOrDefault();
                string strDateOfWeek = preference.TestScheduleDayOfWeek;

                DateTime startDate = DateTime.Parse($"{strStartDate.ToShortDateString()} {preference.TestScheduleStartTime}");
                DateTime endDate = DateTime.Parse($"{strEndDate.ToShortDateString()} {preference.TestScheduleEndTime}");

                string[] strDateAllow = strDateOfWeek.Split('|');
                int currentIndex = (int)schoolNowDateTime.DayOfWeek;
                string[] dateOfWeek = { "sun", "mon", "tue", "wed", "thu", "fri", "sat" };

                if (schoolNowDateTime < startDate
                            || schoolNowDateTime.TimeOfDay < startDate.TimeOfDay
                            || schoolNowDateTime > endDate
                            || schoolNowDateTime.TimeOfDay > endDate.TimeOfDay
                            || !strDateAllow.Contains(dateOfWeek[currentIndex]))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// check test assign was reopened
        /// </summary>
        /// <param name="qTIOnlineTestSessionID"></param>
        /// <returns></returns>
        private bool CheckExistingAutoGradingQueue(int? qTIOnlineTestSessionID)
        {
            if (qTIOnlineTestSessionID.HasValue)
            {
                var autoGradingQueue = _parameters.QTIOnlineTestSessionService.GetAutoGradingQueueByQTOnlineTestSessionID(qTIOnlineTestSessionID.GetValueOrDefault());
                return autoGradingQueue != null ? true : false;
            }

            return false;
        }
        private IEnumerable<int> GetAllTestSessionIdHasNoAutoGradingQueue(IEnumerable<int> qTIOnlineTestSessionID)
        {
            var autoGradingQueues = _parameters.QTIOnlineTestSessionService.GetAutoGradingQueueByQTOnlineTestSessionID(qTIOnlineTestSessionID);


            var sessionIdsThatHasNoAutoGradingQueue = (from sessionId in qTIOnlineTestSessionID
                                                       join queue in autoGradingQueues
                                                       on sessionId equals queue.QTIOnlineTestSessionID
                                                       into joined
                                                       from j in joined.DefaultIfEmpty()
                                                       where j == null
                                                       select sessionId)
                                .ToArray();

            return sessionIdsThatHasNoAutoGradingQueue;
        }
    }
}
