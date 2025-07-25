using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Isolating;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.Media;
using LinkIt.BubbleSheetPortal.Web.Models;
using LinkIt.BubbleSheetPortal.Web.Models.PrintTestOfStudent;
using LinkIt.BubbleSheetPortal.Web.Models.TestAssignmentRegrader;
using LinkIt.BubbleSheetPortal.Web.Print;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using RestSharp.Extensions;
using HttpUtility = RestSharp.Contrib.HttpUtility;
using LinkIt.BubbleSheetPortal.Services.DownloadPdf;
using LinkIt.BubbleSheetPortal.Web.ServiceConsumer;
using Ionic.Zip;
using LinkIt.BubbleSheetPortal.Models.PDFGenerator;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Web.Helpers.Algorithmic;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using S3Library;
using LinkIt.BubbleSheetPortal.Models.Enum;
using LinkIt.BubbleSheetPortal.Models.Algorithmic;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.ReadOnly)]
    [AjaxAwareAuthorize]
    [VersionFilter]
    public class PrintTestController : BaseController
    {
        public string CssPath(string fileName)
        {
            var path = HttpContext.Server.MapPath("~/Content/themes/Print/VirtualTest/");
            var result = Path.Combine(path, fileName);
            return result;
        }

        public string JSPath(string fileName)
        {
            var path = HttpContext.Server.MapPath("~/Scripts/");
            var result = Path.Combine(path, fileName);
            return result;
        }

        private readonly PrintTestControllerParameters parameters;

        public PrintTestController(PrintTestControllerParameters parameters)
        {
            this.parameters = parameters;
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.TmgmtPrinttest)]
        public ActionResult Index()
        {
            var model = new PrintTestViewModel()
            {
                IsAdmin = IsUserAdmin(),
                CanSelectTeachers = CurrentUser.IsDistrictAdminOrPublisher,
                IsSchoolAdmin = CurrentUser.RoleId.Equals(8),
                IsTeacher = CurrentUser.IsTeacher,
                IsPublisher = CurrentUser.RoleId.Equals((int)Permissions.Publisher),
                DistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0,
                IsNetworkAdmin = false
            };
            if (CurrentUser.IsNetworkAdmin)
            {
                model.IsNetworkAdmin = true;
                model.ListDistricIds = CurrentUser.GetMemberListDistrictId();
            }
            return View(model);
        }

        public ActionResult PrintTest(int testId, int bankId, int districtId)
        {
            var districtBank = parameters.ListBankServices.GetBankDistrictByDistrictIdAndBankId(districtId, bankId);
            bool isLockedBank = districtBank != null && (districtBank.BankDistrictAccessId == (int)LockBankStatus.Restricted || districtBank.BankAccessId == (int)LockBankStatus.Restricted);

            var test = parameters.TestServices.GetTestById(testId);
            string answerKeyURL = ConfigurationManager.AppSettings["PrintAnswerKeyURL"];
            string testKeyURL = ConfigurationManager.AppSettings["PrintTestURL"];
            var vVirtualTestFile = parameters.VirtualTestFileService.GetFirstOrDefaultByVirtualTest(testId);
            string strRubrickkey = vVirtualTestFile != null ? vVirtualTestFile.FileKey : string.Empty;
            PrintTestRequest obj = new PrintTestRequest()
            {
                IsLockbank = isLockedBank,
                VirtualTestID = testId.ToString(),
                TestTitle = test.Name,
                PrintTestURL = testKeyURL,
                PrintAnswerKeyURL = answerKeyURL,
                RubricKey = strRubrickkey,
                RubricLink = string.Format("Notification/DownloadRubricFile/?key={0}", strRubrickkey)
            };
            return PartialView("_PrintTest", obj);
        }

        [CacheControl(HttpCacheability.NoCache)]
        public ActionResult PrintVirtualTest(int testId, int? districtID, bool? layoutV2 = false)
        {
            var virtualTest = parameters.VirtualTestService.GetTestById(testId);
            ViewBag.HasRightToAccess = true;
            if (virtualTest == null)
            {
                if (layoutV2 == true)
                {
                    return PartialView("v2/_PrintVirtualTest", new PrintTestRequest());
                }
                return PartialView("_PrintVirtualTest", new PrintTestRequest());
            }
 
            //check to avoid modifying ajax parameter
            var hasPermission = parameters.VulnerabilityService.HasRightToAccessTestBank(CurrentUser, virtualTest.BankID, CurrentUser.GetMemberListDistrictId());
            if (!hasPermission)
            {
                ViewBag.HasRightToAccess = false;
            }

            if (!CurrentUser.IsPublisher && CurrentUser.DistrictId.HasValue)
            {
                districtID = CurrentUser.DistrictId;
            }

            // apply restriction rule
            var allowToPrint = parameters.RestrictionBO.IsAllowTo(new Models.RestrictionDTO.IsCheckRestrictionObjectDTO
            {
                DistrictId = districtID.HasValue ? districtID.Value : 0,
                BankId = virtualTest.BankID,
                TestId = testId,
                UserId = CurrentUser.Id,
                RoleId = CurrentUser.RoleId,
                ModuleCode = RestrictionConstant.Module_PrintTest,
                ObjectType = BubbleSheetPortal.Models.RestrictionDTO.RestrictionObjectType.Test
            });

            if (allowToPrint == false)
            {
                throw new Exception { Source = "Infringe restriction rule" };
            }

            //isLockedBank = parameters.BankService.CheckBankLock(virtualTest.BankID, CurrentUser.Id, districtID??0);
            var obj = new PrintTestOptions();
            try
            {
                obj = parameters.BankService.CheckPermissionPrintQuestionAndAnswerkey(virtualTest.BankID, CurrentUser.Id, districtID ?? 0);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                ViewBag.ErrorMessage = ex.Message.ToString();
            }
            var m = virtualTest.BankID;
            var Id = CurrentUser.Id;
            var districtID1 = districtID;
            var answerKeyURL = ConfigurationManager.AppSettings["PrintAnswerKeyURL"];
            var testKeyURL = ConfigurationManager.AppSettings["PrintTestURL"];
            var vVirtualTestFile = parameters.VirtualTestFileService.GetFirstOrDefaultByVirtualTest(testId);
            var strRubrickkey = vVirtualTestFile != null ? vVirtualTestFile.FileKey : string.Empty;

            var testData = parameters.QTITestClassAssignmentServices.GetVirtualTestForPrinting(testId);

            var model = new PrintTestRequest
            {
                CanPrintQuestion = obj.CanPrintQuestion,
                CanPrintAnswerKey = obj.CanPrintAnswerKey,
                //IsLockbank = isLockedBank,
                VirtualTestID = testId.ToString(),
                TestTitle = testData.Select(o => o.TestName).FirstOrDefault(),
                TestInstructions = testData.Select(o => o.TestInstruction).FirstOrDefault().ReplaceWeirdCharacters(),
                PrintTestURL = testKeyURL,
                PrintAnswerKeyURL = answerKeyURL,
                RubricKey = strRubrickkey,
                RubricLink = string.Format("Notification/DownloadRubricFile/?key={0}", strRubrickkey)
            };
            ViewBag.BasicSciencePaletteSymbol = parameters.ConfigurationServices.GetConfigurationByKeyWithDefaultValue("BasicSciencePaletteSymbol", string.Empty);

            if (layoutV2 == true)
            {
                return PartialView("v2/_PrintVirtualTest", model);
            }
            return PartialView("_PrintVirtualTest", model);
        }

        [ValidateInput(false)]
        [HttpPost]
        [AjaxOnly]
        public ActionResult PrintVirtualTest(PrintVirtualTestModel printModel)
        {
            var testData = parameters.QTITestClassAssignmentServices.GetVirtualTestForPrinting(printModel.VirtualTestID.Value);
            var virtualTest = parameters.VirtualTestService.GetTestById(printModel.VirtualTestID.Value);
            if (virtualTest == null)
            {
                return Json(new { Success = false, ErrorMessage = "There is no virtual test!" });
            }
            //check to avoid modifying ajax parameter
            var hasPermission = parameters.VulnerabilityService.HasRightToAccessTestBank(CurrentUser, virtualTest.BankID, CurrentUser.GetMemberListDistrictId());
            if (!hasPermission)
            {
                return Json(new { Success = false, ErrorMessage = "Has no right to print virtual test!" });
            }

            var model = BuildVirtualTestPrintingModel(printModel, testData);
            var mapPath = HttpContext.Server.MapPath("~/");
            model.Css = new List<string>();
            model.Css.Add(System.IO.File.ReadAllText(string.Format("{0}/{1}", mapPath, "Content/themes/LinkitStyleSheet.css")));
            model.Css.Add(System.IO.File.ReadAllText(string.Format("{0}/{1}", mapPath, "Content/themes/Print/VirtualTest/VirtualTest.css")));
            var html = VirtualTestPrinting.GenerateHtml(this, model, "VirtualTestPDFTemplate", parameters.S3Service);
            var pdfGeneratorModel = new PdfGeneratorModel()
            {
                Html = html,
                FileName = model.TestTitle,
                Folder = "Tests"
            };
            var pdfData = InvokePdfGeneratorService(pdfGeneratorModel);

            return Json(pdfData);
        }

        [ValidateInput(false)]
        [HttpPost]
        [AjaxOnly]
        public ActionResult PrintVirtualTestAnswerKey(PrintVirtualTestModel printModel)
        {
            var virtualTest = parameters.VirtualTestService.GetTestById(printModel.VirtualTestID.Value);
            if (virtualTest == null)
            {
                return Json(new { Success = false, ErrorMessage = "There is no virtual test!" });
            }
            //check to avoid modifying ajax parameter
            var hasPermission = parameters.VulnerabilityService.HasRightToAccessTestBank(CurrentUser, virtualTest.BankID, CurrentUser.GetMemberListDistrictId());
            if (!hasPermission)
            {
                return Json(new { Success = false, ErrorMessage = "Has no right to print virtual test!" });
            }

            var testData = parameters.QTITestClassAssignmentServices.GetVirtualTestAnswerKeyForPrinting(printModel.VirtualTestID.Value, CurrentUser.Id);

            var model = BuildVirtualTestPrintingModel(printModel, testData);
            var mapPath = HttpContext.Server.MapPath("~/");
            model.Css = new List<string>();
            model.Css.Add(System.IO.File.ReadAllText(string.Format("{0}/{1}", mapPath, "Content/themes/LinkitStyleSheet.css")));
            model.Css.Add(System.IO.File.ReadAllText(string.Format("{0}/{1}", mapPath, "Content/themes/Print/AnswerKey/AnswerKey.css")));
            model.JS.Add(System.IO.File.ReadAllText(string.Format("{0}/{1}", mapPath, "Scripts/Lib/ramda.min.js")));
            model.ColumnCount = "1";
            var html = VirtualTestAnswerKeyPrinting.GenerateHtml(this, model, "VirtualTestAnswerKeyPDFTemplate");
            var pdfGeneratorModel = new PdfGeneratorModel()
            {
                Html = html,
                FileName = string.Format("{0}-ak", model.TestTitle),
                Folder = "Tests"
            };
            var pdfData = InvokePdfGeneratorService(pdfGeneratorModel);
            return Json(pdfData);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult PrintTestOfStudent(PrintTestOfStudentRequest printModel)
        {
            var qtiOnlineTestSessionIdList = new List<int>();
            if (string.IsNullOrEmpty(printModel.QTIOnlineTestSessionIDs))
            {
                return Json(new { Success = false, ErrorMessage = "Has any test of student to print!" });
            }
            var qtiOnlineTestSessionIdArray = printModel.QTIOnlineTestSessionIDs.Split(new char[] { ',' },
                    StringSplitOptions.RemoveEmptyEntries);
            foreach (var qtiOnlineTestSessionId in qtiOnlineTestSessionIdArray)
            {
                int id;
                if (int.TryParse(qtiOnlineTestSessionId, out id))
                {
                    qtiOnlineTestSessionIdList.Add(id);
                }
            }

            var hasRightToPrint = HasRightToPrintTestOfStudents(qtiOnlineTestSessionIdList);
            if (!hasRightToPrint)
                return Json(new { Success = false, ErrorMessage = "Has no right to print test of student!" });

            //user enter number of question
            if (printModel.QuestionType == 2)
            {
                if (string.IsNullOrWhiteSpace(printModel.PrintQuestionIDs))
                    return Json(new { Success = false, ErrorMessage = "Please enter number of question!" });
            }

            if (qtiOnlineTestSessionIdList.Count == 1 && string.IsNullOrWhiteSpace(printModel.StudentName))
            {
                var qtiOnlineTestSession =
                        parameters.QTIOnlineTestSessionService.GetQTIOnlineTestSessionByID(qtiOnlineTestSessionIdList[0]);
                var student = parameters.StudentServices.GetStudentById(qtiOnlineTestSession.StudentId);
                printModel.StudentName = string.Format("{0}, {1}", student.LastName, student.FirstName);
            }

            //save to batch printing queue
            var data = new BatchPrintingQueue()
            {
                QTITestClassAssignmentID = printModel.QTITestClassAssignmentID ?? 0,
                QTIOnlineTestSessionIDs = printModel.QTIOnlineTestSessionIDs,
                VirtualTestID = printModel.VirtualTestID ?? 0,

                ManuallyGradedOnly = printModel.ManuallyGradedOnly,
                ManuallyGradedOnlyQuestionIds = printModel.ManuallyGradedOnlyQuestionIds,
                TeacherFeedback = printModel.TeacherFeedback,
                TheCoverPage = printModel.TheCoverPage,
                TheCorrectAnswer = printModel.TheCorrectAnswer,
                Passages = printModel.Passages,
                GuidanceAndRationale = printModel.GuidanceAndRationale,
                TheQuestionContent = printModel.TheQuestionContent,
                NumberOfColumn = printModel.NumberOfColumn,
                ShowQuestionPrefix = printModel.ShowQuestionPrefix,
                ShowBorderAroundQuestions = printModel.ShowBorderAroundQuestions,
                ExcludeTestScore = printModel.ExcludeTestScore,
                IncorrectQuestionsOnly = printModel.IncorrectQuestionsOnly,
                IncludeAttachment = printModel.IncludeAttachment,

                StudentType = printModel.StudentType,
                QuestionType = printModel.QuestionType,
                PrintQuestionIDs = printModel.PrintQuestionIDs,
                UserID = CurrentUser.Id,
                UserName = CurrentUser.UserName,
                StudentName = string.IsNullOrWhiteSpace(printModel.StudentName) ? "Batch Printing" : printModel.StudentName,
                DistrictID = CurrentUser.DistrictId,
                ProcessingStatus = (int)BatchPrintingProcessingStatusEnum.Pending,
                CreatedDate = DateTime.UtcNow
            };

            parameters.BatchPrintingQueueService.SaveBatchPrintingQueue(data);
            return Json(new { Success = true });
            //var model = new TOSVirtualTest
            //{
            //    TestName = printModel.TestName,
            //    StudentName = printModel.StudentName,
            //    UserDistrictId = CurrentUser.DistrictId,
            //    JavaScripts = new List<string>(),
            //    Css = new List<string>(),
            //    TestFeedback = string.Empty,
            //    TotalPointsEarned = printModel.TotalPointsEarned,
            //    TotalPointsPossible = printModel.TotalPointsPossible,
            //    PrintGuidance = printModel.PrintGuidance ?? false,
            //    VirtualSections = new List<TOSVirtualSection>(),
            //    TeacherFeedback = printModel.TeacherFeedback,
            //    TheCoverPage = printModel.TheCoverPage,
            //    TheCorrectAnswer = printModel.TheCorrectAnswer,
            //    Passages = printModel.Passages,
            //    GuidanceAndRationale = printModel.GuidanceAndRationale,
            //    TheQuestionContent = printModel.TheQuestionContent,
            //    NumberOfColumn = printModel.NumberOfColumn,
            //    ShowQuestionPrefix = printModel.ShowQuestionPrefix,
            //    ShowBorderAroundQuestions = printModel.ShowBorderAroundQuestions,
            //    ExcludeTestScore = printModel.ExcludeTestScore
            //};

            //var mapPath = HttpContext.Server.MapPath("~/");
            //model.Css.Add(System.IO.File.ReadAllText(string.Format("{0}/{1}", mapPath, "Content/themes/LinkitStyleSheet.css")));
            //model.Css.Add(System.IO.File.ReadAllText(string.Format("{0}/{1}", mapPath, "Content/themes/Print/TestOfStudent/TestOfStudent.css")));

            //model.JavaScripts.Add(System.IO.File.ReadAllText(string.Format("{0}/{1}", mapPath, "Scripts/jquery-1.11.2.min.js")));
            //model.JavaScripts.Add(System.IO.File.ReadAllText(string.Format("{0}/{1}", mapPath, "Scripts/jquery.ui.widget.js")));
            //model.JavaScripts.Add(System.IO.File.ReadAllText(string.Format("{0}/{1}", mapPath, "Scripts/knockout-3.0.0.js")));
            //model.JavaScripts.Add(System.IO.File.ReadAllText(string.Format("{0}/{1}", mapPath, "Scripts/imagesloaded.pkgd.js")));
            //model.JavaScripts.Add(System.IO.File.ReadAllText(string.Format("{0}/{1}", mapPath, "Scripts/PrintTestOfStudent/Utils.js")));
            //model.JavaScripts.Add(System.IO.File.ReadAllText(string.Format("{0}/{1}", mapPath, "Scripts/PrintTestOfStudent/XmlContentProcessing.js")));
            //model.JavaScripts.Add(System.IO.File.ReadAllText(string.Format("{0}/{1}", mapPath, "Scripts/PrintTestOfStudent/Questions/QuestionRender.js")));
            //model.JavaScripts.Add(System.IO.File.ReadAllText(string.Format("{0}/{1}", mapPath, "Scripts/PrintTestOfStudent/Questions/OpenEnded.js")));
            //model.JavaScripts.Add(System.IO.File.ReadAllText(string.Format("{0}/{1}", mapPath, "Scripts/PrintTestOfStudent/Questions/TextEntry.js")));
            //model.JavaScripts.Add(System.IO.File.ReadAllText(string.Format("{0}/{1}", mapPath, "Scripts/PrintTestOfStudent/Questions/SimpleChoice.js")));
            //model.JavaScripts.Add(System.IO.File.ReadAllText(string.Format("{0}/{1}", mapPath, "Scripts/PrintTestOfStudent/Questions/MultipleChoice.js")));
            //model.JavaScripts.Add(System.IO.File.ReadAllText(string.Format("{0}/{1}", mapPath, "Scripts/PrintTestOfStudent/Questions/MultipleChoiceVariable.js")));
            //model.JavaScripts.Add(System.IO.File.ReadAllText(string.Format("{0}/{1}", mapPath, "Scripts/PrintTestOfStudent/Questions/InlineChoice.js")));
            //model.JavaScripts.Add(System.IO.File.ReadAllText(string.Format("{0}/{1}", mapPath, "Scripts/PrintTestOfStudent/Questions/ComplexItem.js")));
            //model.JavaScripts.Add(System.IO.File.ReadAllText(string.Format("{0}/{1}", mapPath, "Scripts/PrintTestOfStudent/Questions/DragDropStandard.js")));
            //model.JavaScripts.Add(System.IO.File.ReadAllText(string.Format("{0}/{1}", mapPath, "Scripts/PrintTestOfStudent/Questions/ImgHotspot.js")));
            //model.JavaScripts.Add(System.IO.File.ReadAllText(string.Format("{0}/{1}", mapPath, "Scripts/PrintTestOfStudent/Questions/TextHotspot.js")));
            //model.JavaScripts.Add(System.IO.File.ReadAllText(string.Format("{0}/{1}", mapPath, "Scripts/PrintTestOfStudent/Questions/DragDropNumerical.js")));
            //model.JavaScripts.Add(System.IO.File.ReadAllText(string.Format("{0}/{1}", mapPath, "Scripts/PrintTestOfStudent/Questions/DragDropSequence.js")));
            //model.JavaScripts.Add(System.IO.File.ReadAllText(string.Format("{0}/{1}", mapPath, "Scripts/PrintTestOfStudent/Questions/TableHotspot.js")));
            //model.JavaScripts.Add(System.IO.File.ReadAllText(string.Format("{0}/{1}", mapPath, "Scripts/PrintTestOfStudent/Questions/NumberLineHotspot.js")));

            //model.UseS3Content = true;
            ////parameters.DistrictDecodeService.UseS3Content(CurrentUser.DistrictId.GetValueOrDefault());

            //var virtualTestID = 0;
            //if (printModel.VirtualTestID.HasValue) virtualTestID = printModel.VirtualTestID.Value;
            //model.VirtualTest = parameters.VirtualTestService.GetTestById(virtualTestID);

            //if (!string.IsNullOrEmpty(model.TestName))
            //{
            //    model.TestName = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(model.TestName));
            //}

            //var dto = parameters.QTITestClassAssignmentServices.GetPrintTestOfStudent(virtualTestID, false);
            //var virtualTestForPrinting = dto.VirtualTestForPrinting;
            //var preference =
            //       parameters.QTITestClassAssignmentServices.GetPreferencesForOnlineTest(printModel.QTITestClassAssignmentID);
            //var branchingTest = preference != null && preference.BranchingTest != null && preference.BranchingTest.Value;
            //branchingTest = branchingTest && model.VirtualTest != null && model.VirtualTest.VirtualTestSubTypeID == 5 &&
            //                dto.VirtualTestForPrinting != null;
            //var totalPointsPossible = 0;
            //if (!branchingTest && (!printModel.TotalPointsPossible.HasValue || printModel.TotalPointsPossible.Value == 0))
            //{
            //    //Calc totalPointsPossible
            //    foreach (var questionDTO in dto.VirtualTestForPrinting)
            //    {
            //        totalPointsPossible += questionDTO.PointsPossible;
            //    }
            //    model.TotalPointsPossible = totalPointsPossible;
            //}

            //var pdfGeneratorModelList = new List<PdfGeneratorModel>();
            //var qtiTestClassAssignment =
            //    parameters.QTITestClassAssignmentServices.GetQtiTestClassAssignment(printModel.QTITestClassAssignmentID.GetValueOrDefault());
            //foreach (var qtiOnlineTestSessionId in qtiOnlineTestSessionIdList)
            //{
            //    MapAnswerToTOSVirtualTest(parameters.QTITestClassAssignmentServices.GetTestStateTOS(qtiOnlineTestSessionId)
            //    .ToList(), model);

            //    if (model.Answers.Count > 0)
            //    {
            //        var isolatingTestSessionAnswers =
            //            parameters.IsolatingTestTakerService.GetTestState(qtiOnlineTestSessionId).ToList();

            //        foreach (var o in model.Answers)
            //        {
            //            UpdateTestSessonAnserFromIsolatingDB(o, isolatingTestSessionAnswers);
            //        }
            //    }

            //    dto.VirtualTestForPrinting = virtualTestForPrinting; //reset list questions for next student
            //    var totalPointsEarned = 0;
            //    //Branching Test: remove unanswer questions
            //    if (branchingTest)
            //    {
            //        totalPointsPossible = 0;
            //        var branchingQuestions = new List<VirtualTestForPrinting>();
            //        foreach (var answer in model.Answers)
            //        {
            //            if (!answer.Answered) continue;
            //            var question =
            //                dto.VirtualTestForPrinting.FirstOrDefault(
            //                    o => o.VirtualQuestionID == answer.VirtualQuestionID);
            //            if (question == null) continue;
            //            question.QuestionOrder = answer.AnswerOrder;
            //            branchingQuestions.Add(question);
            //            totalPointsPossible += question.PointsPossible; //calc pointsPossible for branching test
            //            if (answer.PointsEarned.HasValue)
            //                totalPointsEarned += answer.PointsEarned.Value;
            //        }
            //        dto.VirtualTestForPrinting = branchingQuestions;
            //        model.TotalPointsPossible = totalPointsPossible;
            //    }
            //    else
            //    {
            //        //Update totalPointsEarned
            //        foreach (var answer in model.Answers)
            //        {
            //            if (answer.PointsEarned.HasValue)
            //                totalPointsEarned += answer.PointsEarned.Value;
            //        }
            //    }
            //    if (model.VirtualTest != null && model.VirtualTest.TestScoreMethodID == 2)
            //    {
            //        totalPointsEarned = totalPointsEarned + 100 - model.TotalPointsPossible.Value;
            //        totalPointsEarned = totalPointsEarned < 0 ? 0 : totalPointsEarned;
            //    }
            //    model.TotalPointsEarned = totalPointsEarned;

            //    if (model.TotalPointsPossible.HasValue && model.TotalPointsPossible.Value > 0)
            //    {
            //        model.Percent = (decimal)model.TotalPointsEarned.GetValueOrDefault() / (decimal)model.TotalPointsPossible.Value;
            //        model.Percent = model.Percent * 100;
            //    }

            //    if (printModel.ManuallyGradedOnly) FilterManualItems(printModel, dto);
            //    if (printModel.QuestionType == 2) FilterPrintQuestionItems(printModel, dto);
            //    if (printModel.IncorrectQuestionsOnly) FilterIncorrectQuestions(model, dto);

            //    MapQuestionsToTOSVirtualTest(dto, model);

            //    var testFeedback = parameters.TestFeedbackService.GetLasFeedback(qtiOnlineTestSessionId);
            //    if (testFeedback != null)
            //    {
            //        model.TestFeedbackId = testFeedback.TestFeedbackID;
            //        model.TestFeedback = testFeedback.Feedback;
            //    }

            //    var qtiOnlineTestSession =
            //       parameters.QTIOnlineTestSessionService.GetQTIOnlineTestSessionByID(qtiOnlineTestSessionId);
            //    var student = parameters.StudentServices.GetStudentById(qtiOnlineTestSession.StudentId);

            //    model.StudentName = string.Format("{0}, {1}", student.LastName, student.FirstName);
            //var html = this.RenderRazorViewToString("PrintTestOfStudentTemplate", model);
            //    var pdfName = string.Format("{0}-{1}-{2}-{3}", qtiTestClassAssignment.Code, student.LastName, student.FirstName, student.Code);
            //    var pdfGeneratorModel = new PdfGeneratorModel()
            //    {
            //        Html = html,
            //        FileName = pdfName,
            //        Folder = "StudentResponses"
            //    };
            //    if (qtiOnlineTestSessionIdList.Count == 1)
            //    {
            //        var pdfData = InvokePdfGeneratorService(pdfGeneratorModel);
            //        return Json(pdfData);
            //    }

            //    pdfGeneratorModelList.Add(pdfGeneratorModel);
            //}
            //var zipData = PipelineProcess(pdfGeneratorModelList, qtiTestClassAssignment.Code);
            //return Json(zipData);
        }

        [HttpPost]
        public ActionResult GetBatchPrintingData(int? classTestAssignmentId)
        {
            var data = new List<BatchPrintingViewModel>();
            if (classTestAssignmentId.HasValue)
            {
                data =
                    parameters.BatchPrintingQueueService.GetBatchPrintingByTestClassAssignmentId(classTestAssignmentId.Value, CurrentUser.Id).
                        Select(o => new BatchPrintingViewModel()
                        {
                            StudentName = o.StudentName,
                            PrintDate = o.CreatedDate ?? DateTime.UtcNow,
                            ProcessingStatus = Enum.GetName(typeof(BatchPrintingProcessingStatusEnum), o.ProcessingStatus),
                            DownloadPdfUrl = o.DownloadPdfID.HasValue ? Url.Action("Index", "DownloadPdf", new { pdfID = o.DownloadPdfID }, HelperExtensions.GetHTTPProtocal(Request)) : string.Empty
                        }).ToList();
            }
            var parser = new DataTableParser<BatchPrintingViewModel>();
            return Json(parser.Parse(data.AsQueryable(), true), JsonRequestBehavior.AllowGet);
        }

        private bool HasRightToPrintTestOfStudents(List<int> qtiOnlineTestSessionIdList)
        {
            foreach (var qtiOnlineTestSessionId in qtiOnlineTestSessionIdList)
            {
                if (qtiOnlineTestSessionId > 0 &&
                    !parameters.VulnerabilityService.HasRigtToEditQtiOnlineTestSession(CurrentUser,
                        qtiOnlineTestSessionId))
                    return false;

                var qtiOnlineTestSession =
                    parameters.QTIOnlineTestSessionService.GetQTIOnlineTestSessionByID(qtiOnlineTestSessionId);
                if (qtiOnlineTestSession == null)
                    return false;

                var student = parameters.StudentServices.GetStudentById(qtiOnlineTestSession.StudentId);
                if (student == null)
                    return false;
            }

            return true;
        }

        private static void FilterManualItems(PrintTestOfStudentRequest printModel, PrintTestOfStudentDTO dto)
        {
            var manuallyGradedOnlyQuestionIdList = new List<int>();
            if (!string.IsNullOrEmpty(printModel.ManuallyGradedOnlyQuestionIds))
            {
                var ids = printModel.ManuallyGradedOnlyQuestionIds.Split(',');
                foreach (var id in ids)
                {
                    int virtualQuestionId;
                    if (int.TryParse(id, out virtualQuestionId))
                    {
                        manuallyGradedOnlyQuestionIdList.Add(virtualQuestionId);
                    }
                }
            }
            dto.VirtualTestForPrinting =
                    dto.VirtualTestForPrinting.Where(
                        x => manuallyGradedOnlyQuestionIdList.Contains(x.VirtualQuestionID)).ToList();
        }

        private static void FilterPrintQuestionItems(PrintTestOfStudentRequest printModel, PrintTestOfStudentDTO dto)
        {
            var printQuestionIdList = new List<int>();
            if (!string.IsNullOrEmpty(printModel.PrintQuestionIDs))
            {
                var ids = printModel.PrintQuestionIDs.Split(',');
                foreach (var id in ids)
                {
                    int virtualQuestionId;
                    if (int.TryParse(id, out virtualQuestionId))
                    {
                        printQuestionIdList.Add(virtualQuestionId);
                    }
                }
            }
            dto.VirtualTestForPrinting =
                    dto.VirtualTestForPrinting.Where(
                        x => printQuestionIdList.Contains(x.VirtualQuestionID)).ToList();
        }

        private static void FilterIncorrectQuestions(TOSVirtualTest model, PrintTestOfStudentDTO dto)
        {
            var incorrectQuestionIds = new List<int>();
            foreach (var questionDTO in dto.VirtualTestForPrinting)
            {
                var answer =
                      model.Answers.FirstOrDefault(
                          x => x.Answered && x.IsReviewed && x.VirtualQuestionID == questionDTO.VirtualQuestionID);
                if (answer != null && answer.PointsEarned.HasValue && answer.PointsEarned.Value < questionDTO.PointsPossible)
                    incorrectQuestionIds.Add(questionDTO.VirtualQuestionID);
            }

            dto.VirtualTestForPrinting =
                    dto.VirtualTestForPrinting.Where(
                        x => incorrectQuestionIds.Contains(x.VirtualQuestionID)).ToList();

            model.Answers = model.Answers.Where(x => incorrectQuestionIds.Contains(x.VirtualQuestionID)).ToList();
        }

        private void UpdateTestSessonAnserFromIsolatingDB(TestOnlineSessionAnwer answer,
            List<IsolatingTestSessionAnswerDTO> isolatingAnswers)
        {
            if (answer == null || isolatingAnswers == null || isolatingAnswers.Count == 0) return;
            var isolatingAnswer =
                isolatingAnswers.FirstOrDefault(
                    o => o.QTIOnlineTestSessionAnswerID == answer.QTIOnlineTestSessionAnswerID);
            if (isolatingAnswer == null) return;
            answer.HighlightPassage = isolatingAnswer.HighlightPassage;
            answer.HighlightQuestion = isolatingAnswer.HighlightQuestion;
            answer.AnswerChoice = isolatingAnswer.AnswerChoice;
            answer.AnswerText = isolatingAnswer.AnswerText;
            answer.AnswerTemp = isolatingAnswer.AnswerTemp;
            answer.Answered = isolatingAnswer.Answered;
            answer.AnswerImage = ReformatXmlData(isolatingAnswer.AnswerImage);
            answer.AnswerOrder = isolatingAnswer.AnswerOrder ?? 0;
            if (answer.TestOnlineSessionAnswerSubs != null && answer.TestOnlineSessionAnswerSubs.Count > 0 &&
                isolatingAnswer.QTIOTSessionAnswerSubs != null && isolatingAnswer.QTIOTSessionAnswerSubs.Count > 0)
            {
                foreach (var answerSub in answer.TestOnlineSessionAnswerSubs)
                {
                    var isolatingAnswerSub =
                        isolatingAnswer.QTIOTSessionAnswerSubs.FirstOrDefault(
                            o => o.QTIOnlineTestSessionAnswerSubID == answerSub.QTIOnlineTestSessionAnswerSubID);
                    if (isolatingAnswerSub == null) continue;
                    answerSub.AnswerChoice = isolatingAnswerSub.AnswerChoice;
                    answerSub.AnswerText = isolatingAnswerSub.AnswerText;
                    answerSub.AnswerTemp = isolatingAnswerSub.AnswerTemp;
                    answerSub.Answered = isolatingAnswerSub.Answered;
                }
            }
        }

        public List<TestOnlineSessionAnswerSub> GetTestOnlineSessionAnswerSubs(QTITestState item)
        {
            if (string.IsNullOrWhiteSpace(item.AnswerSubs)) return new List<TestOnlineSessionAnswerSub>();
            var xdoc = XDocument.Parse(item.AnswerSubs);
            var result = new List<TestOnlineSessionAnswerSub>();
            foreach (var answersub in xdoc.Element("AnswerSubs").Elements("AnswerSub"))
            {
                var onlineAnswerSub = new TestOnlineSessionAnswerSub();
                onlineAnswerSub.AnswerChoice = XmlUtils.GetStringValue(answersub.Element("AnswerChoice"));
                onlineAnswerSub.AnswerText = XmlUtils.GetStringValue(answersub.Element("AnswerText")).ReplaceWeirdCharacters();
                onlineAnswerSub.PointsEarned = XmlUtils.GetInt(answersub.Element("PointsEarned"));
                onlineAnswerSub.QTIOnlineTestSessionAnswerSubID =
                    XmlUtils.GetIntValue(answersub.Element("QTIOnlineTestSessionAnswerSubID"));
                onlineAnswerSub.ResponseIdentifier = XmlUtils.GetStringValue(answersub.Element("ResponseIdentifier"));
                onlineAnswerSub.VirtualQuestionSubID = XmlUtils.GetIntValue(answersub.Element("VirtualQuestionSubID"));
                onlineAnswerSub.ResponseProcessingTypeID =
                    XmlUtils.GetInt(answersub.Element("ResponseProcessingTypeID"));
                result.Add(onlineAnswerSub);
            }

            return result;
        }

        private string FormatHighlightPassage(string passage)
        {
            passage = ReplaceWeirdCharacters(passage) ?? string.Empty;
            passage = ItemSetPrinting.AdjustXmlContentFloatImg(passage);
            passage = Util.ReplaceTagListByTagOlForPassage(passage, false);

            return passage;
        }

        private string ReformatXmlData(string source)
        {
            if (string.IsNullOrWhiteSpace(source)) return string.Empty;
            source = string.Format("<DrawAnswerXML>{0}</DrawAnswerXML>", source);
            try
            {
                var deSerializer = new XmlSerializer(typeof(DrawAnswerXML));
                DrawAnswerXML xml;
                using (TextReader reader = new StringReader(source))
                {
                    xml = deSerializer.Deserialize(reader) as DrawAnswerXML;
                }

                var dom = ConvertToDOM(xml);
                var settings = new XmlWriterSettings { OmitXmlDeclaration = true };
                var ms = new MemoryStream();
                var writer = XmlWriter.Create(ms, settings);
                var serializer = new XmlSerializer(typeof(DrawAnswerDOM));
                var ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                serializer.Serialize(writer, dom, ns);

                ms.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                var sr = new StreamReader(ms);
                var result = sr.ReadToEnd();

                result = VirtualTestPrinting.ConvertTags(result,
                    new List<string>
                    {
                        "DrawAnswer",
                        "DrawImgs",
                    }, "div", false);
                result = VirtualTestPrinting.ConvertTags(result,
                    new List<string>
                    {
                        "DrawImg",
                    }, "img", false);

                result = VirtualTestPrinting.ConvertTags(result,
                    new List<string>
                    {
                        "DrawAnswer",
                        "DrawImgs",
                    }, "div", false);
                result = VirtualTestPrinting.ConvertTags(result,
                   new List<string>
                   {
                        "DrawImg",
                   }, "img", false);

                return result;
            }
            catch (InvalidOperationException ex)
            {
                return string.Empty;
            }
        }

        private DrawAnswerDOM ConvertToDOM(DrawAnswerXML source)
        {
            var domFormat = new DrawAnswerDOM { DrawImgs = new List<DrawImg>() };
            if (source == null || source.Types == null) return domFormat;
            foreach (var type in source.Types)
            {
                if (type.Contents != null)
                {
                    foreach (var content in type.Contents)
                    {
                        var typeDOM = new DrawImg
                        {
                            Type = type.Name,
                            Source = content.Source,
                            Index = content.Index,
                            Data = content.Data
                        };

                        domFormat.DrawImgs.Add(typeDOM);
                    }
                }
                if (type.Passages != null)
                {
                    foreach (var passage in type.Passages)
                    {
                        foreach (var content in passage.Contents)
                        {
                            if (type.Contents == null) continue;
                            var typeDOM = new DrawImg
                            {
                                Type = type.Name,
                                Source = content.Source,
                                Index = content.Index,
                                Data = content.Data,
                                RefObjectID = passage.RefObjectID
                            };

                            domFormat.DrawImgs.Add(typeDOM);
                        }
                    }
                }
            }

            return domFormat;
        }

        private string ReplaceWeirdCharacters(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return string.Empty;

            var result = str.ReplaceWeirdCharacters();
            result =
                result.Replace("<p><span><list listStylePosition=\"outside\" listStyleType=\"decimal\" paragraphSpaceAfter=\"12\" styleName=\"passageNumbering\"><listMarkerFormat><ListMarkerFormat color=\"#aaaaaa\" paragraphEndIndent=\"20\"/></listMarkerFormat>", "<ol>")
                .Replace("<list ", "<ol ")
                .Replace("</list></span></p>", "</ol>")
                .Replace("</list>", "</ol>");
            result = Util.ReplaceVideoTag(result);

            return result;
        }

        private void MapQuestionsToTOSVirtualTest(PrintTestOfStudentDTO dto, TOSVirtualTest model)
        {
            if (dto == null || model == null || dto.VirtualTestForPrinting == null) return;
            model.VirtualSections = new List<TOSVirtualSection>();
            var sectionsDTO = dto.VirtualTestForPrinting.GroupBy(o => new { o.VirtualSectionID, o.SectionTitle, o.SectionOrder }).OrderBy(o => o.Key.SectionOrder).ToList();
            var currentPassageList = new List<string>();
            foreach (var sectionDTO in sectionsDTO)
            {
                var sectionModel = new TOSVirtualSection();
                sectionModel.Questions = new List<TOSVirtualQuestion>();
                sectionModel.SectionTitle = sectionDTO.Key.SectionTitle;
                sectionModel.SectionOrder = sectionDTO.Key.SectionOrder;
                sectionModel.VirtualSectionID = sectionDTO.Key.VirtualSectionID;
                var questionsDTO = sectionDTO.OrderBy(o => o.QuestionOrder).ToList();
                foreach (var questionDTO in questionsDTO)
                {
                    var questionModel = new TOSVirtualQuestion();
                    questionModel.VirtualQuestionID = questionDTO.VirtualQuestionID;
                    questionModel.QTISchemaID = questionDTO.QTISchemaID;
                    questionModel.XmlContent = ModifyXmlContent(questionDTO.XmlContent, false, model.NumberOfColumn);
                    questionModel.XmlContent = Util.ReplaceVideoTag(questionModel.XmlContent);
                    questionModel.PointsPossible = questionDTO.PointsPossible;
                    questionModel.QTIItemAnswerScoresDTO = questionDTO.QTIItemAnswerScoresDTO;
                    questionModel.VirtualQuestionAnswerScoresDTO = questionDTO.VirtualQuestionAnswerScoresDTO;
                    questionModel.QuestionOrder = questionDTO.QuestionOrder;
                    questionModel.XmlContent = VirtualTestPrinting.ConvertTags(questionModel.XmlContent,
                        new List<string>
                        {
                            "destinationObject",
                            "destinationItem",
                            "sourceObject",
                            "textHotSpot",
                            "imageHotSpot",
                            "numberLine",
                            "tableitem"
                        }, "div", false);
                    questionModel.XmlContent = VirtualTestPrinting.ConvertTags(questionModel.XmlContent,
                        new List<string>
                        {
                            "sourceText",
                            "sourceItem",
                            "numberLineItem"
                        }, "span", false);
                    questionModel.StartNewPassage = false;
                    var passageList = PassageUtil.GetReferenceContents(parameters.S3Service, questionModel.XmlContent,
                        LinkitConfigurationManager.GetS3Settings().S3Domain,
                        LinkitConfigurationManager.GetS3Settings().AUVirtualTestBucketName,
                        LinkitConfigurationManager.GetS3Settings().AUVirtualTestROFolder);

                    questionModel.PassageTexts = new List<string>();
                    if (passageList == null)
                    {
                        passageList = new List<string>();
                    }
                    foreach (string passage in passageList)
                    {
                        var adjustPassage = passage;
                        //if (model.UseS3Content)
                        adjustPassage = ReplaceWeirdCharacters(adjustPassage);

                        adjustPassage = PassageUtil.UpdateS3LinkForPassageMedia(passage,
                            LinkitConfigurationManager.GetS3Settings().S3Domain,
                        LinkitConfigurationManager.GetS3Settings().AUVirtualTestBucketName,
                        LinkitConfigurationManager.GetS3Settings().AUVirtualTestROFolder);

                        adjustPassage = ReplaceWeirdCharacters(adjustPassage);

                        var adjustPassageProcessing = new XmlContentProcessing(adjustPassage);

                        adjustPassageProcessing.ScaleTable(600);
                        adjustPassage = adjustPassageProcessing.GetXmlContent();

                        if (!currentPassageList.Contains(adjustPassage))
                        {
                            currentPassageList.Add(adjustPassage);
                            questionModel.PassageTexts.Add(adjustPassage);
                            questionModel.StartNewPassage = true;
                        }
                    }
                    var maxWith = model.NumberOfColumn == 2 ? 260 : 580;
                    var xmlContentProcessing = new XmlContentProcessing(questionModel.XmlContent);
                    xmlContentProcessing.ScaleTable(maxWith);

                    questionModel.XmlContent = xmlContentProcessing.GetXmlContent();

                    //parse responseRubric for text entry
                    questionModel.ResponseRubric = string.Empty;
                    if (questionModel.QTISchemaID == (int)QtiSchemaEnum.TextEntry)
                    {
                        try
                        {
                            xmlContentProcessing = new XmlContentProcessing(questionModel.XmlContent);
                            questionModel.ResponseRubric = xmlContentProcessing.GetResponseRubric();
                            questionModel.ResponseRubric = VirtualTestPrinting.ConvertTags(questionModel.ResponseRubric, new List<string> { "responseRubric", "value" }, "div");
                        }
                        catch
                        {
                        }
                    }
                    sectionModel.Questions.Add(questionModel);
                }

                model.VirtualSections.Add(sectionModel);
            }
        }

        private string ModifyXmlContent(string xmlContent, bool isHighlight, int numberOfColumn)
        {
            if (string.IsNullOrWhiteSpace(xmlContent)) return string.Empty;
            xmlContent = xmlContent.Replace("<br>", "<br/>");
            xmlContent = xmlContent.ReplaceWeirdCharacters();
            xmlContent = Util.UpdateS3LinkForItemMedia(xmlContent);
            xmlContent = Util.UpdateS3LinkForPassageLink(xmlContent);
            xmlContent = ConvertBoxedText(xmlContent);
            //darg and drop (must replace tag in C# so that it can be printed ,replacing in client will be unable to print :(
            xmlContent = VirtualTestPrinting.ConvertTags(xmlContent,
                new List<string>
                {
                    "destinationObject",
                    "destinationItem",
                    "sourceObject",
                    "textHotSpot",
                    "imageHotSpot",
                    "sourceItem",
                    "tableitem",
                    "numberLine",
                    "numberLineItem"
                },
                "div", isHighlight);
            xmlContent = VirtualTestPrinting.ConvertTags(xmlContent, new List<string> { "sourceText", "sourceItem" }, "span", isHighlight);
            //replace <br /> by <div/> to allow PDF break new line

            xmlContent =
                xmlContent.Replace("<p><span><list listStylePosition=\"outside\" listStyleType=\"decimal\" paragraphSpaceAfter=\"12\" styleName=\"passageNumbering\"><listMarkerFormat><ListMarkerFormat color=\"#aaaaaa\" paragraphEndIndent=\"20\"/></listMarkerFormat>", "<ol>")
                .Replace("<list ", "<ol ")
                .Replace("</list></span></p>", "</ol>")
                .Replace("</list>", "</ol>");
            xmlContent = Util.ReplaceVideoTag(xmlContent);

            var maxWith = numberOfColumn == 2 ? 260 : 580;
            var xmlContentProcessing = new XmlContentProcessing(xmlContent);
            xmlContentProcessing.ScaleTable(maxWith);

            xmlContent = xmlContentProcessing.GetXmlContent();

            return xmlContent;
        }

        public static string ConvertBoxedText(string data)
        {
            var xmlContentProcessing = new XmlContentProcessing(data);
            xmlContentProcessing.ConvertBoxedTextTag();
            var result = xmlContentProcessing.GetXmlContent();
            return result;
        }

        private void MapAnswerToTOSVirtualTest(List<QTITestState> dto, TOSVirtualTest model)
        {
            if (dto == null || model == null) return;
            model.Answers = dto.Select(
                o => new TestOnlineSessionAnwer
                {
                    QTIOnlineTestSessionAnswerID = o.QTIOnlineTestSessionAnswerID,
                    QTIOnlineTestSessionID = o.QTIOnlineTestSessionID,
                    VirtualQuestionID = o.VirtualQuestionID,
                    Answered = o.Answered.HasValue && o.Answered.Value,
                    AnswerOrder = o.AnswerOrder ?? 0,
                    AnswerChoice = o.AnswerChoice,
                    AnswerText = o.AnswerText.ReplaceWeirdCharacters(),
                    AnswerImage = ReformatXmlData(o.AnswerImage),
                    HighlightQuestion = ModifyXmlContent(o.HighlightQuestion, true, model.NumberOfColumn) ?? string.Empty,
                    HighlightPassage = FormatHighlightPassage(o.HighlightPassage),
                    PointsEarned = o.PointsEarned,
                    ResponseProcessingTypeID = o.ResponseProcessingTypeID,
                    Feedback = o.Feedback,
                    TestOnlineSessionAnswerSubs = GetTestOnlineSessionAnswerSubs(o)
                }).ToList();

            model.ScoreRaw = dto.Select(o => o.ScoreRaw).FirstOrDefault();
        }

        private VirtualTestPrintingModel BuildVirtualTestPrintingModel(PrintVirtualTestModel printModel, List<VirtualTestForPrinting> testData)
        {
            var maxWith = printModel.Columns == "single" ? 580 : 260;
            var model = new VirtualTestPrintingModel
            {
                Sections = TransformToModel(testData, maxWith),
                JS = new List<string>()
            };

            FillAlgorithmicQuestions(model.Sections, printModel.VirtualTestID);

            model.JS.Add(System.IO.File.ReadAllText(JSPath("jquery-1.7.1.js")));
            model.JS.Add(System.IO.File.ReadAllText(JSPath("imagesloaded.pkgd.js")));
            model.JS.Add(System.IO.File.ReadAllText(JSPath("PrintTest/PrintTest.js")));

            if (string.IsNullOrWhiteSpace(printModel.TestInstructions))
            {
                printModel.TestInstructions = testData.Select(o => o.TestInstruction).FirstOrDefault();
            }
            //for get media item from S3

            var mediaModel = new MediaModel();
            model.S3Domain = mediaModel.S3Domain;
            //if there is the last '/', remove it
            if (model.S3Domain[model.S3Domain.Length - 1] == '/')
            {
                model.S3Domain = model.S3Domain.Substring(0, model.S3Domain.Length - 1);
            }
            model.UpLoadBucketName = mediaModel.UpLoadBucketName;
            model.AUVirtualTestFolder = mediaModel.AUVirtualTestFolder;
            model.AUVirtualTestROFolder = mediaModel.AUVirtualTestROFolder;
            model.IsCustomItemNaming = parameters.VirtualTestService.IsCustomItemNaming(printModel.VirtualTestID.Value);
            model.IsNumberQuestions = testData.Select(x => x.IsNumberQuestions).FirstOrDefault();

            Transform(printModel, model);

            return model;
        }

        private string PipelineProcess(List<PdfGeneratorModel> models, string testClassAssignmentCode)
        {
            var limit = 10;
            var resultOfStartProcess = new BlockingCollection<PdfGeneratorModel>(limit);
            var resultOfInvokePdfGeneratorProcess = new BlockingCollection<string>(limit);
            //var resultOfStoreBubbleSheetFile = new BlockingCollection<BubbleSheetField>(limit);
            var zipFile = new ZipFile();

            var token = new CancellationToken();
            using (var cts = CancellationTokenSource.CreateLinkedTokenSource(token))
            {
                var f = new TaskFactory(TaskCreationOptions.LongRunning, TaskContinuationOptions.None);

                var stage1 = f.StartNew(() => StartProcess(models, resultOfStartProcess, cts));

                var stage2 = f.StartNew(() => InvokePdfsGeneratorProcess(resultOfStartProcess,
                                                                       resultOfInvokePdfGeneratorProcess,
                                                                       cts));

                var stage3 = f.StartNew(() => AddZipFileProcess(resultOfInvokePdfGeneratorProcess, zipFile,
                                                                   cts));
                Task.WaitAll(stage1, stage2, stage3);
            }
            var pdfGeneratorServerPath = ConfigurationManager.AppSettings["DownloadPdfFolderPath"];
            var relativeZipPath = string.Format(@"\PDF\{0}\{1}\{2}.zip", models[0].Folder, CurrentUser.UserName, testClassAssignmentCode);
            pdfGeneratorServerPath = string.Format("{0}{1}", pdfGeneratorServerPath.RemoveEndSlash(), relativeZipPath);
            zipFile.Save(pdfGeneratorServerPath);

            var downloadPdfData = new DownloadPdfData { FilePath = relativeZipPath, UserID = CurrentUser.Id, CreatedDate = DateTime.UtcNow };
            parameters.DownloadPdfService.SaveDownloadPdfData(downloadPdfData);
            var downLoadUrl = Url.Action("Index", "DownloadPdf", new { pdfID = downloadPdfData.DownloadPdfID }, HelperExtensions.GetHTTPProtocal(Request));

            return downLoadUrl;
        }

        private void StartProcess(List<PdfGeneratorModel> models, BlockingCollection<PdfGeneratorModel> output, CancellationTokenSource cts)
        {
            try
            {
                var token = cts.Token;
                foreach (var model in models)
                {
                    if (token.IsCancellationRequested) break;
                    output.Add(model);
                }
            }
            finally
            {
                output.CompleteAdding();
            }
        }

        private void InvokePdfsGeneratorProcess(BlockingCollection<PdfGeneratorModel> input, BlockingCollection<string> output, CancellationTokenSource cts)
        {
            try
            {
                var token = cts.Token;
                foreach (var model in input.GetConsumingEnumerable())
                {
                    if (token.IsCancellationRequested) break;
                    var pdfUrl = PdfGeneratorConsumer.InvokePdfGeneratorService(model.Html, model.FileName, model.Folder, CurrentUser.UserName);
                    if (!string.IsNullOrWhiteSpace(pdfUrl))
                        output.Add(pdfUrl);
                }
            }
            finally
            {
                output.CompleteAdding();
            }
        }

        private void AddZipFileProcess(BlockingCollection<string> input, ZipFile output, CancellationTokenSource cts)
        {
            var token = cts.Token;
            var pdfGeneratorServerPath = ConfigurationManager.AppSettings["DownloadPdfFolderPath"];
            foreach (var pdfUrl in input.GetConsumingEnumerable())
            {
                if (token.IsCancellationRequested) break;
                var absoluteFilePath = string.Format("{0}\\{1}", pdfGeneratorServerPath.RemoveEndSlash(), pdfUrl.RemoveStartSlash());
                output.AddFile(absoluteFilePath, "");
            }
        }

        private string InvokePdfGeneratorService(PdfGeneratorModel model)
        {
            var pdfUrl = PdfGeneratorConsumer.InvokePdfGeneratorService(model.Html, model.FileName, model.Folder, CurrentUser.UserName);

            if (string.IsNullOrWhiteSpace(pdfUrl)) return string.Empty;

            var downloadPdfData = new DownloadPdfData { FilePath = pdfUrl, UserID = CurrentUser.Id, CreatedDate = DateTime.UtcNow };
            parameters.DownloadPdfService.SaveDownloadPdfData(downloadPdfData);
            var downLoadUrl = Url.Action("Index", "DownloadPdf", new { pdfID = downloadPdfData.DownloadPdfID }, HelperExtensions.GetHTTPProtocal(Request));

            return downLoadUrl;
        }

        private string InvokePdfsGeneratorService(List<PdfGeneratorModel> models, string testClassAssignmentCode)
        {
            var zipFile = new ZipFile();
            var pdfGeneratorServerPath = ConfigurationManager.AppSettings["DownloadPdfFolderPath"];
            foreach (var model in models)
            {
                var pdfUrl = PdfGeneratorConsumer.InvokePdfGeneratorService(model.Html, model.FileName, model.Folder, CurrentUser.UserName);
                if (!string.IsNullOrWhiteSpace(pdfUrl))
                {
                    var absoluteFilePath = string.Format("{0}\\{1}", pdfGeneratorServerPath.RemoveEndSlash(), pdfUrl.RemoveStartSlash());
                    zipFile.AddFile(absoluteFilePath, "");
                }
            }

            var relativeZipPath = string.Format(@"\PDF\{0}\{1}\{2}.zip", models[0].Folder, CurrentUser.UserName, testClassAssignmentCode);
            pdfGeneratorServerPath = string.Format("{0}{1}", pdfGeneratorServerPath.RemoveEndSlash(), relativeZipPath);
            zipFile.Save(pdfGeneratorServerPath);

            var downloadPdfData = new DownloadPdfData { FilePath = relativeZipPath, UserID = CurrentUser.Id, CreatedDate = DateTime.UtcNow };
            parameters.DownloadPdfService.SaveDownloadPdfData(downloadPdfData);
            var downLoadUrl = Url.Action("Index", "DownloadPdf", new { pdfID = downloadPdfData.DownloadPdfID }, HelperExtensions.GetHTTPProtocal(Request));

            return downLoadUrl;
        }

        private void Transform(PrintVirtualTestModel printModel, VirtualTestPrintingModel model)
        {
            model.UserDistrictId = CurrentUser.DistrictId ?? 0;

            model.StartCountingOnCover = printModel.StartCountingOnCover;
            model.TestTitle = printModel.TestTitle;
            model.TeacherName = printModel.TeacherName;
            model.ClassName = printModel.ClassName;
            model.IncludePageNumbers = printModel.IncludePageNumbers;
            model.IncludeCoverPage = printModel.IncludeCoverPage;
            model.ColumnCount = printModel.Columns == "single" ? "1" : "2";
            model.AnswerLabelFormat = printModel.AnswerLabelFormat;
            model.ShowQuestionBorders = printModel.ShowQuestionBorders;
            model.TestInstruction = printModel.TestInstructions;
            model.ExtendedTextAreaShowLines = printModel.ExtendedTextAreaShowLines;
            model.ExtendedTextAreaAnswerOnSeparateSheet = printModel.ExtendedTextAreaAnswerOnSeparateSheet;
            model.ExtendedTextAreaNumberOfLines = printModel.ExtendedTextAreaNumberOfLines;
            model.DrawReferenceBackground = printModel.DrawReferenceBackground;
            model.ShowSectionHeadings = printModel.ShowSectionHeadings;
            model.QuestionPrefix = printModel.QuestionPrefix;
            model.IncludeStandards = printModel.IncludeStandards;
            model.IncludeTags = printModel.IncludeTags;
            model.IncludeRationale = printModel.IncludeRationale ?? false;
            model.IncludeGuidance = printModel.IncludeGuidance ?? false;
        }

        private List<VirtualSectionModel> TransformToModel(List<VirtualTestForPrinting> items, int maxWidth)
        {
            var result = items.GroupBy(o => new { o.VirtualSectionID, o.SectionOrder, o.SectionTitle, o.SectionInstruction }).Select(o => new VirtualSectionModel
            {
                SectionOrder = o.Key.SectionOrder,
                SectionTitle = o.Key.SectionTitle,
                SectionInstruction = o.Key.SectionInstruction,
                Items = o.Select(x => TransformToModel(x, maxWidth)).ToList()
            }).ToList();

            return result;
        }

        private VirtualQuestionModel TransformToModel(VirtualTestForPrinting item, int maxWidth)
        {
            if (item == null) return null;
            var result = new VirtualQuestionModel
            {
                PointsPossible = item.PointsPossible,
                QTIGroupID = item.QTIGroupID,
                QTIItemID = item.QTIItemID,
                QTISchemaID = item.QTISchemaID,
                QuestionOrder = item.QuestionOrder,
                SectionInstruction = item.SectionInstruction,
                SectionOrder = item.SectionOrder,
                SectionReferenceID = item.SectionReferenceID,
                SectionTitle = item.SectionTitle,
                TestInstruction = item.TestInstruction,
                TestName = item.TestName,
                Title = item.Title,
                VirtualQuestionID = item.VirtualQuestionID,
                VirtualSectionID = item.VirtualSectionID,
                XmlContent = item.XmlContent.ReplaceWeirdCharacters(),
                UrlPath = item.UrlPath,
                CorrectAnswer = item.CorrectAnswer,
                AnswerKeyXml = TransformToAnswerKeyXml(item.Answers),
                Skills = item.Skills,
                Topics = item.Topics,
                Other = item.Other,
                Standards = item.Standards.ReplaceWeirdCharacters(),
                DistrictTag = item.DistrictTag,
                QuestionGroupId = item.QuestionGroupId,
                GroupQuestionCommon = item.GroupQuestionCommon,
                GroupQuestionTitle = item.GroupQuestionTitle,
                ResponseProcessingTypeID = item.ResponseProcessingTypeID,
                QuestionLabel = item.QuestionLabel
            };

            var xmlContentProcessing = new XmlContentProcessing(result.XmlContent);
            xmlContentProcessing.ScaleTable(maxWidth);
            result.XmlContent = xmlContentProcessing.GetXmlContent();
            result.XmlContent = XmlUtils.RemoveAllNamespacesPrefix(result.XmlContent);

            if (!string.IsNullOrEmpty(result.GroupQuestionCommon))
            {
                result.GroupQuestionCommon = result.GroupQuestionCommon.ReplaceWeirdCharacters();

                var xmlGroupQuestionCommonProcessing = new XmlContentProcessing(result.GroupQuestionCommon);
                xmlGroupQuestionCommonProcessing.ScaleTable(maxWidth);
                result.GroupQuestionCommon = xmlGroupQuestionCommonProcessing.GetXmlContent();
            }

            ReorderAnswerKeyBaseOnXmlContent(result.AnswerKeyXml, result.XmlContent);

            return result;
        }

        private void FillAlgorithmicQuestions(List<VirtualSectionModel> virtualSections, int? virtualTestId)
        {
            if (virtualTestId.HasValue)
            {
                var algorithmicQuestionsData =
                    parameters.QTITestClassAssignmentServices.GetAlgorithmicQuestionExpressions(virtualTestId.Value);
                if (algorithmicQuestionsData == null || algorithmicQuestionsData.Count == 0)
                    return;

                foreach (var section in virtualSections)
                {
                    foreach (var virtualQuestion in section.Items)
                    {
                        if (virtualQuestion.ResponseProcessingTypeID == (int)ResponseProcessingTypeEnum.AlgorithmicScoring)
                        {
                            var algorithmicQuestionExpressions = algorithmicQuestionsData
                                .Where(x => x.VirtualQuestionID == virtualQuestion.VirtualQuestionID)
                                .OrderByDescending(x => x.PointsEarned).ToList();

                            ProccessTheAlgorithmicQuestion(virtualQuestion, algorithmicQuestionExpressions);
                        }
                    }
                }
            }
        }

        private void ProccessTheAlgorithmicQuestion(VirtualQuestionModel virtualQuestion, List<AlgorithmicQuestionExpression> algorithmicQuestionExpression)
        {
            if (virtualQuestion.ResponseProcessingTypeID != (int)ResponseProcessingTypeEnum.AlgorithmicScoring
                || algorithmicQuestionExpression == null
                || algorithmicQuestionExpression.Count == 0)
                return;

            var correctAnswers = AlgorithmicHelper.ConvertToAlgorithmicCorrectAnswers(virtualQuestion.QTISchemaID, algorithmicQuestionExpression);
            virtualQuestion.AlgorithmicCorrectAnswers = correctAnswers;
            virtualQuestion.PointsPossible = correctAnswers.Count > 0 ? correctAnswers.Max(x => x.PointsEarned) : 0;
        }

        private void ReorderAnswerKeyBaseOnXmlContent(AnswerKeyXml data, string xmlContent)
        {
            if (data == null || data.AnserKeyItems == null || string.IsNullOrWhiteSpace(xmlContent)) return;

            var items = data.AnserKeyItems;
            foreach (var item in items)
            {
                var responseIdentifier = string.Format("responseIdentifier=\"{0}\"", item.ResponseIdentifier);
                item.Index = xmlContent.IndexOf(responseIdentifier, System.StringComparison.Ordinal);
                item.Answer = item.Answer.ReplaceWeirdCharacters();
            }

            data.AnserKeyItems = items.OrderBy(o => o.Index).ToList();
        }

        private AnswerKeyXml TransformToAnswerKeyXml(string answers)
        {
            if (string.IsNullOrWhiteSpace(answers)) return null;
            var result = XmlUtils.Deserialize<AnswerKeyXml>("<AnswerKeyXml>" + answers + "</AnswerKeyXml>");
            return result;
        }

        #region Private Method

        private bool IsUserAdmin()
        {
            return parameters.UserServices.GetAdminByUsernameAndDistrict(CurrentUser.UserName, CurrentUser.DistrictId.GetValueOrDefault()).IsNotNull();
        }

        #endregion Private Method
    }
}
