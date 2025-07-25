using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using LinkIt.BubbleSheetPortal.Models.EInstructionImport;
using System.IO;
using LinkIt.BubbleSheetPortal.Data.Entities;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.ReadOnly)]
    [AjaxAwareAuthorize(Order = 2)]
    public class EInstructionController : Controller
    {
        private readonly EInstructionService eInstructionService;
        private readonly UserService userService;
        private readonly IncorrectQuestionOrderService incorrectQuestionOrderService;
        private readonly ClassStudentService classStudentService;
        private readonly TeacherDistrictTermService teacherDistrictTermService;

        private readonly string strSelected = "1";
        private readonly string strNotSelected = "0";


        public EInstructionController(EInstructionService eInstructionService, UserService userService,
            IncorrectQuestionOrderService incorrectQuestionOrderService, ClassStudentService classStudentService, TeacherDistrictTermService teacherDistrictTermService)
        {
            this.eInstructionService = eInstructionService;
            this.userService = userService;
            this.incorrectQuestionOrderService = incorrectQuestionOrderService;
            this.classStudentService = classStudentService;
            this.teacherDistrictTermService = teacherDistrictTermService;
        }

        public UserPrincipal CurrentUser
        {
            get { return ((UserPrincipal)User); }
        }

        //[HttpGet]
        //[AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.TmgmtEinstructionimport)]
        [NonAction]
        public ActionResult Import()
        {   
            var model = new EInstructionImportViewModel
            {
                IsAdmin = IsUserAdmin(),
                CanSelectTeachers = CurrentUser.IsDistrictAdminOrPublisher,
                IsSchoolAdmin = CurrentUser.RoleId.Equals((int)Permissions.SchoolAdmin),
                IsPublisher = CurrentUser.IsPublisher
            };
            model.IsNetworkAdmin = false;
            if (CurrentUser.IsNetworkAdmin)
            {
                model.IsNetworkAdmin = true;
                model.ListDistricIds = CurrentUser.GetMemberListDistrictId();
            }
            return View(model);

        }

        [UploadifyPrincipal(Order = 1)]
        public ActionResult UploadCSV(HttpPostedFileBase postedFile, int? districtId, int? classID, int? schoolID)
        {
            var text = string.Empty;
            var totalRows = 0;
            var resResultCount = 0;
            var requestID = 0;
            var canGradeTest = true;
            var lstStudent = new List<EInstructionStudent>();
            try
            {
                if (!CurrentUser.IsPublisher)
                {
                    districtId = CurrentUser.DistrictId.Value;
                }
                classID = !classID.HasValue ? 0 : classID.GetValueOrDefault();
                schoolID = !schoolID.HasValue ? 0 : schoolID.GetValueOrDefault();
                var requestParameter = string.Format("ClassID|{0}|SchoolID|{1}", classID, schoolID);
                requestID = eInstructionService.RITCreateRequest(CurrentUser.Id, districtId.Value, postedFile.FileName,
                                                      requestParameter);

                using (var reader = new StreamReader(postedFile.InputStream))
                {
                    text = reader.ReadToEnd();
                }

                totalRows = GetTotalRows(text);
                totalRows = totalRows > 0 ? totalRows - 1 : 0;
                var resResult = eInstructionService.RITCreateRequestTestResponse(requestID, text);

                var lstTemp = new List<EInstructionStudent>();
                resResultCount = resResult.Count;
                for (var i = 0; i < resResultCount; i++)
                {
                    lstStudent.Add(BuildEIstructionStudent(resResult[i], lstTemp));
                }

                canGradeTest = lstStudent.Any(x => string.Equals(x.IsSelected, strSelected));

                return Json(new
                {
                    success = true,
                    data = lstStudent,
                    requestID = requestID,
                    canGradeTest = canGradeTest,
                    totalRows = totalRows,
                    resResultCount = resResultCount
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new
                {
                    success = false,
                    totalRows = totalRows,
                    resResultCount = resResultCount
                }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public ActionResult GetStudents(int classId)
        {
            var data =
                classStudentService.GetClassStudentsByClassId(classId)
                                   .ToList()
                                   .Select(x => new { x.Code, x.FullName })
                                   .OrderBy(x => x.FullName);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //[HttpGet]
        //public ActionResult CheckIfTestRequiresCorrection(int testId)
        //{
        //    var requiresCorrection = incorrectQuestionOrderService.CheckIfTestRequiresCorrection(testId);
        //    return Json(new { Success = requiresCorrection }, JsonRequestBehavior.AllowGet);
        //}

        [HttpPost]
        [ValidateInput(false)]
        [AjaxOnly]
        public ActionResult UpdateStudentLocalCode(int resId, string selectedVal, int? foundId)
        {
            var sucess = eInstructionService.UpdateStudentLocalCode(resId, selectedVal, foundId);
            if (sucess)
            {
                return Json(new { message = "", success = true, type = "" });
            }

            return Json(new { message = "Error when updating student local code.", success = false, type = "error" });
        }

        [HttpPost]
        [ValidateInput(false)]
        [AjaxOnly]
        public ActionResult GradeTest(int requestId, int virtualTestId, string approveStudent)
        {
            if (virtualTestId > 0)
            {
                var result = eInstructionService.RITGradeTestResponse(requestId, virtualTestId, approveStudent);
                var message = BuildMessage(result);

                return Json(new { message = message, success = true, type = "" });
            }
            return Json(new { message = "Grade Test not success.", success = false, type = "error" });
        }

        [HttpGet]
        public ActionResult GetTeachers(int schoolId)
        {
            var validUserSchoolRoleId = new[] { 2, 3, 5, 8,27 };//Only Publisher, " + LabelHelper.DistrictLabel + " Admin, Shool Admin, Teacher
            var data = eInstructionService.GetTeachersBySchoolId(schoolId).Where(x=> validUserSchoolRoleId.Contains(x.RoleId)) 
                .Select(x => new
            {
                Id = x.UserId,
                Name = x.UserName,
                FirstName = x.FirstName,
                LastName = x.LastName
            }).OrderBy(x => x.LastName).ToList();
            var teachersHasTerm = teacherDistrictTermService.GetTeachersHasTerms(schoolId).Select(x => x.UserId).ToList();
            data = data.Where(x => teachersHasTerm.Contains(x.Id)).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetTerms(int? userId, int schoolId)
        {
            userId = userId ?? CurrentUser.Id;
            var data = GetDistrictTermsByUserId(userId, schoolId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetClasses(int termId, int? userId, int schoolId)
        {
            userId = userId ?? CurrentUser.Id;
            var data = GetClassesByDistrictTermIdAndUserId(termId, userId.GetValueOrDefault(), schoolId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private EInstructionStudent BuildEIstructionStudent(RITCreateRequestTestResponseResult item, List<EInstructionStudent> lstTemp)
        {
            var rs = new EInstructionStudent();

            var inputNameTemplate = "{0} {1} ({2})";
            rs.ID = item.RequestStudentResponseID;
            rs.LocalCode = item.StudentLocalCode;

            var arrSessionData = item.SessionData.Split(',');
            var count = arrSessionData.Length;
            if (count >= 8)
            {
                rs.ClassName = arrSessionData[0];
                rs.SectionName = arrSessionData[1];
                rs.InputName = string.Format(inputNameTemplate, arrSessionData[5], arrSessionData[7], item.StudentLocalCode);
            }

            var arrSummary = item.StudentResponseSummary.Split(',');
            if (arrSummary.Length > 0)
            {
                rs.Score = arrSummary[arrSummary.Length - 1];
            }
            if (lstTemp.Any(x => string.Equals(x.LocalCode, rs.LocalCode)))
            {
                rs.IsSelected = strNotSelected;
            }
            else
            {
                rs.IsSelected = item.StudentExist ? strSelected : strNotSelected;
            }
            lstTemp.Add(rs);

            return rs;
        }

        private bool IsUserAdmin()
        {
            return userService.GetAdminByUsernameAndDistrict(CurrentUser.UserName, CurrentUser.DistrictId.GetValueOrDefault()).IsNotNull();
        }

        private static int GetTotalRows(string fileContents)
        {
            if (string.IsNullOrWhiteSpace(fileContents)) return 0;
            var rows = Regex.Split(fileContents, Environment.NewLine);
            return rows.Count(o => !string.IsNullOrWhiteSpace(o));
        }

        private IEnumerable<ListItem> GetDistrictTermsByUserId(int? userId, int? schoolId)
        {
            var teacher = userService.GetUserById(userId.GetValueOrDefault());
            if (teacher.IsNull())
            {
                return new List<ListItem>();
            }
            return eInstructionService.GetTermsByUserIdAndSchoolId(userId.GetValueOrDefault(), schoolId.GetValueOrDefault())
                    .Select(x => new ListItem
                            {
                                Id = x.DistrictTermId,
                                Name = x.DistrictName
                            }).OrderBy(x => x.Name);
        }

        public IEnumerable<ListItem> GetClassesByDistrictTermIdAndUserId(int termId, int userId, int schoolId)
        {
            var teacher = userService.GetUserById(userId);
            if (teacher.IsNull())
            {
                return new List<ListItem>();
            }
            return eInstructionService.GetClassesByDistrictTermIdAndUserId(termId, userId, schoolId)
                    .Select(x => new ListItem
                    {
                        Id = x.Id,
                        Name = x.Name
                    }).OrderBy(x => x.Name);
        }

        public string BuildMessage(List<RITGradeTestResponseResult> result)
        {
            if (result == null || result.Count == 0) return string.Empty;
            var message = new StringBuilder();

            // # of tests successfully graded: <number>
            var successfullGradedLog =
                result.FirstOrDefault(o => o.DomainID == (int)EInstructionDomainCode.eI_SuccessFullGraded);
            var successfullGraded = successfullGradedLog != null ? successfullGradedLog.LogData : string.Empty;
            message.AppendFormat("<b># of tests successfully graded:</b> {0}<br/>", successfullGraded);

            // # of tests not successfully graded: <number>
            var notSuccessfullGradedLog =
                result.FirstOrDefault(o => o.DomainID == (int)EInstructionDomainCode.eI_NotSuccessFullGraded);
            var notSuccessfullGraded = notSuccessfullGradedLog != null
                                           ? notSuccessfullGradedLog.LogData
                                           : string.Empty;
            message.AppendFormat("<b># of tests not successfully graded:</b> {0}<br/>", notSuccessfullGraded);

            var totalQuestionLog =
                    result.FirstOrDefault(o => o.DomainID == (int)EInstructionDomainCode.eI_TotalQuestions);
            var totalQuestion = totalQuestionLog != null ? totalQuestionLog.LogData : string.Empty;

            var lineNumbers = result.Where(o => o.DomainID == (int)EInstructionDomainCode.eI_LineNumber).OrderBy(o => Convert.ToInt32(o.LogData)).ToList();
            foreach (var lineNumber in lineNumbers)
            {
                GetLineDataMsg(result, lineNumber, message);
                GetNotMatchAnswersMsg(result, lineNumber, message);
                GetLessThanNumberMsg(result, lineNumber, message, totalQuestion);
                GetMoreThanMsg(result, lineNumber, message, totalQuestion);
            }

            return message.ToString();
        }

        // Line 1: <line data>
        private static void GetLineDataMsg(List<RITGradeTestResponseResult> result, RITGradeTestResponseResult lineNumber, StringBuilder message)
        {
            var lineDataLog = result.FirstOrDefault(o => o.DomainID == (int) EInstructionDomainCode.eI_LineData &&
                                                         o.ParentStatisticID == lineNumber.StatisticID);
            var lineData = lineDataLog != null ? lineDataLog.LogData.Replace(",", ", ") : string.Empty;
            message.AppendFormat("<br/><b>Line {0}:</b> {1}<br/>", lineNumber.LogData, lineData);
        }

        // Issue: Number of input answers (4) is less than number of test questions (5)
        private static void GetMoreThanMsg(List<RITGradeTestResponseResult> result, RITGradeTestResponseResult lineNumber, StringBuilder message,
                                           string totalQuestion)
        {
            var numberInputAnswerMoreThanTestQuestionsLog =
                result.FirstOrDefault(
                    o =>
                    o.DomainID == (int) EInstructionDomainCode.eI_NumberInputAnswerMoreThanQuestionNumber &&
                    o.ParentStatisticID == lineNumber.StatisticID);
            var numberInputAnswerMoreThanTestQuestions = numberInputAnswerMoreThanTestQuestionsLog != null
                                                             ? numberInputAnswerMoreThanTestQuestionsLog.LogData
                                                             : string.Empty;
            if (!string.IsNullOrWhiteSpace(numberInputAnswerMoreThanTestQuestions))
            {
                message.AppendFormat(
                    "<b>Issue:</b> Number of input answers ({0}) is more than number of test questions ({1})<br/>",
                    numberInputAnswerMoreThanTestQuestions, totalQuestion);
            }
        }

        // Issue: Number of input answers (4) is less than number of test questions (5)
        private static void GetLessThanNumberMsg(List<RITGradeTestResponseResult> result, RITGradeTestResponseResult lineNumber, StringBuilder message,
                                                 string totalQuestion)
        {
            var numberInputAnswerLessThanTestQuestionsLog =
                result.FirstOrDefault(
                    o =>
                    o.DomainID == (int) EInstructionDomainCode.eI_NumberInputAnswerLessThanQuestionNumber &&
                    o.ParentStatisticID == lineNumber.StatisticID);
            var numberInputAnswerLessThanTestQuestions = numberInputAnswerLessThanTestQuestionsLog != null
                                                             ? numberInputAnswerLessThanTestQuestionsLog.LogData
                                                             : string.Empty;
            if (!string.IsNullOrWhiteSpace(numberInputAnswerLessThanTestQuestions))
            {
                message.AppendFormat(
                    "<b>Issue:</b> Number of input answers ({0}) is less than number of test questions ({1})<br/>",
                    numberInputAnswerLessThanTestQuestions, totalQuestion);
            }
        }

        // Issue: Answer not matching possible answers
        private static void GetNotMatchAnswersMsg(List<RITGradeTestResponseResult> result, RITGradeTestResponseResult lineNumber, StringBuilder message)
        {
            var notMatchAnswersLog =
                result.FirstOrDefault(
                    o =>
                    o.DomainID == (int) EInstructionDomainCode.eI_NotMatchAnswers &&
                    o.ParentStatisticID == lineNumber.StatisticID);
            var notMatchAnswers = notMatchAnswersLog != null
                                      ? notMatchAnswersLog.LogData.Replace(",", ", ")
                                      : string.Empty;
            if (!string.IsNullOrWhiteSpace(notMatchAnswers))
            {
                message.AppendFormat("<b>Issue:</b> Answer ({0}) not matching possible answers<br/>", notMatchAnswers);
            }
        }
    }
}
