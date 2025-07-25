using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using DevExpress.Office.Utils;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleService.Shared.Data;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.BubbleSheetAws;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using Lokad.Cloud.Storage;
using LinkIt.BubbleSheetPortal.Models.BubbleSheetReview;
using System.Web.Script.Serialization;
using LinkIt.BubbleSheetPortal.Models.Enum;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize]
    [VersionFilter]
    public class BubbleSheetReviewDetailsController : ReviewBubbleSheetBaseController
    {
        private readonly BubbleSheetReviewDetailsService bubbleSheetReviewDetailsService;
        private readonly BubbleSheetStudentResultsService bubbleSheetStudentResultsService;
        private readonly BubbleSheetService bubbleSheetService;
        private readonly BubbleSheetFileService bubbleSheetFileService;
        private readonly TestResubmissionService testResubmissionService;
        private readonly PreviouslyAnsweredQuestionsService previouslyAnsweredQuestionsService;
        private readonly StudentRosterService studentRosterService;
        private readonly UnansweredQuestionService unansweredQuestionService;
        private readonly ReadResultService readResultService;
        private readonly AlreadyAnsweredQuestionService alreadyAnsweredQuestionService;
        private readonly SchoolService schoolService;
        private readonly DistrictConfigurationService districtConfigurationService;
        private readonly VirtualTestFileService virtualTestFileService;
        private readonly StudentService studentService;
        private readonly StateService stateService;

        //\
        private readonly ACTAnswerQuestionService actAnswerQuestionService;

        private readonly VirtualTestService virtualTestService;
        private readonly BubbleSheetPrintingService bubbleSheetPrintingService;
        private readonly DistrictTermService districtTermService;
        private readonly VulnerabilityService _vulnerabilityService;
        private readonly DistrictDecodeService _districtDecodeService;
        private readonly VirtualQuestionService _virtualQuestionService;

        public BubbleSheetReviewDetailsController(
            BubbleSheetReviewDetailsService bubbleSheetReviewDetailsService,
            BubbleSheetListService bubbleSheetListService,
            BubbleSheetService bubbleSheetService,
            BubbleSheetFileService bubbleSheetFileService,
            TestResubmissionService testResubmissionService,
            PreviouslyAnsweredQuestionsService previouslyAnsweredQuestionsService,
            StudentRosterService studentRosterService,
            BubbleSheetStudentResultsService bubbleSheetStudentResultsService,
            UnansweredQuestionService unansweredQuestionService,
            ReadResultService readResultService,
            AlreadyAnsweredQuestionService alreadyAnsweredQuestionService,
            SchoolService schoolService,
            DistrictConfigurationService districtConfigurationService,
            VirtualTestFileService virtualTestFileService,
            StudentService studentService,
            StateService stateService,
            ACTAnswerQuestionService actAnswerQuestionService, VirtualTestService virtualTestService, BubbleSheetPrintingService bubbleSheetPrintingService, DistrictTermService districtTermService,
            ClassService classService, UserService userService,
            VulnerabilityService vulnerabilityService,
            DistrictDecodeService districtDecodeService,
            VirtualQuestionService virtualQuestionService)
            : base(bubbleSheetListService, bubbleSheetService, vulnerabilityService, userService, districtDecodeService)
        {
            this.bubbleSheetReviewDetailsService = bubbleSheetReviewDetailsService;
            this.bubbleSheetStudentResultsService = bubbleSheetStudentResultsService;
            this.bubbleSheetService = bubbleSheetService;
            this.bubbleSheetFileService = bubbleSheetFileService;
            this.testResubmissionService = testResubmissionService;
            this.previouslyAnsweredQuestionsService = previouslyAnsweredQuestionsService;
            this.studentRosterService = studentRosterService;
            this.unansweredQuestionService = unansweredQuestionService;
            this.readResultService = readResultService;
            this.alreadyAnsweredQuestionService = alreadyAnsweredQuestionService;
            this.schoolService = schoolService;
            this.districtConfigurationService = districtConfigurationService;
            this.virtualTestFileService = virtualTestFileService;
            this.studentService = studentService;
            this.stateService = stateService;
            this.actAnswerQuestionService = actAnswerQuestionService;
            this.virtualTestService = virtualTestService;
            this.bubbleSheetPrintingService = bubbleSheetPrintingService;
            this.districtTermService = districtTermService;
            this._vulnerabilityService = vulnerabilityService;
            this._districtDecodeService = districtDecodeService;
            _virtualQuestionService = virtualQuestionService;
        }

        [HttpGet]
        [UrlReturnDecode]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.ManagebubblesheetsReview)]
        public ActionResult Index(string id, int? classId)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }

            if (CheckUserCanAccessClass(CurrentUser.Id, CurrentUser.RoleId, classId.GetValueOrDefault()) == false)
            {
                return RedirectToAction("Index", "BubbleSheetReview");
            }

            var totalPage = BubbleSheetWsHelper.GetTotalPageOfTestForOneStudent(id,
                    ConfigurationManager.AppSettings["ApiKey"]);

            if (bubbleSheetFileService.IsFileGenericWithNoMappings(id, classId.GetValueOrDefault()))
            {
                if (totalPage > 1)
                {
                    return RedirectToAction("AssignStudentsToTestMultiPage", "GenericSheet", new { id, classId });
                }
                return RedirectToAction("AssignStudentsToTest", "GenericSheet", new { id, classId });
            }

            var bubbleSheetId = bubbleSheetService.GetBubbleSheetByTicket(id).FirstOrDefault()?.TestId ?? 0;
            var virtualTestName = virtualTestService.GetVirtualTestById(bubbleSheetId)?.Name ?? string.Empty;

            var model = new BubbleSheetReviewDetailsViewModel
            {
                Ticket = id,
                CanAccess = CanUserAccessBubbleSheet(id),
                ClassId = classId.GetValueOrDefault(),
                HasGenericSheet = bubbleSheetFileService.HasGenericSheets(id),
                IsMultipage = totalPage > 1,
                TestName = virtualTestName
            };

            return View(model);
        }

        [HttpGet]
        public ActionResult GetStudentsForTest(string id, int? classId)
        {
            var parser = new DataTableParser<BubbleSheetDetailsStudentListViewModel>();

            if (CheckUserCanAccessClass(CurrentUser.Id, CurrentUser.RoleId, classId.GetValueOrDefault()) == false)
            {
                var emptyStudent = new List<BubbleSheetDetailsStudentListViewModel>().AsQueryable();
                return Json(parser.ParseForClientSide(emptyStudent), JsonRequestBehavior.AllowGet);
            }

            var students = GetStudentsByTicketAndClass(id, classId);
            var returnData = parser.ParseForClientSide(students);
            return Json(returnData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetBubbleSheetStatus(string studentIdList, string ticket, int classId)
        {
            var data = bubbleSheetStudentResultsService.GetBubbleSheetStudentStatus(studentIdList, ticket, classId);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetBubbleSheetDetails(string ticket, int studentId, int? classId
            , int applyAllCorrectAnswer, int applyFullCreditAnswer, int applyZeroCreditAnswer, string answerData)
        {
            if (CheckUserCanAccessClass(CurrentUser.Id, CurrentUser.RoleId, classId.GetValueOrDefault()) == false)
            {
                return RedirectToAction("Index", "BubbleSheetReview");
            }
            //order by BubbleSheetFileID
            var allStudentDetails =
                bubbleSheetReviewDetailsService.GetAllBubbleSheetReviewDetailsForStudentByTicketAndClassId(studentId,
                    ticket, classId.GetValueOrDefault()).OrderByDescending(x => x.BubbleSheetFileId).ToList();

            if (allStudentDetails.Any() == false)
            {
                return RedirectToAction("ReviewDetails", ticket);
            }

            var timeZoneId = stateService.GetTimeZoneId(CurrentUser.StateId.GetValueOrDefault());
            var zn = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            foreach (var studentDetail in allStudentDetails)
            {
                if(studentDetail.UploadedDate.HasValue)
                    studentDetail.UploadedDate = TimeZoneInfo.ConvertTimeFromUtc(studentDetail.UploadedDate.Value, zn);
            }

            var studentArtifact = allStudentDetails.FirstOrDefault(x => x.PageNumber == -1);
            var studentDetails = allStudentDetails.FirstOrDefault(x => x.PageNumber != -1) ?? studentArtifact;

            if (studentDetails.OutputFileName.Equals("temp"))
            {
                var lastestBubblesheetFileHasImage =
                    allStudentDetails.FirstOrDefault(x => x.OutputFileName != string.Empty && x.OutputFileName != "temp");
                if (lastestBubblesheetFileHasImage != null)
                {
                    studentDetails.OutputFileName = lastestBubblesheetFileHasImage.OutputFileName;
                }
            }

            var model = BindStudentDetailsToViewModel(studentDetails);

            if (studentDetails.HasBubbleSheetFile)
            {
                //var imageUrl = BubbleSheetWsHelper.GetTestImageUrl(studentDetails.OutputFileName, ConfigurationManager.AppSettings["ApiKey"]);
                //model.ImageUrl = imageUrl;
                string strImg = string.Empty;
                model.OnlyOnePage = CheckOnlyOneBubbleSheetFile(model.BubbleSheetId, studentId, out strImg);
                model.ImageUrl = strImg;
                model.PreviousBubbleSheets = GetPreviousBubbleSheetsForStudent(allStudentDetails).ToList();
            }

            //model.UnansweredQuestions = GetUnansweredQuestions(studentDetails).OrderBy(x => x.QuestionOrder).ToList();
            model.UnansweredQuestions = unansweredQuestionService.GetAllQuestionOfBubbleSheet(studentId, ticket, classId.GetValueOrDefault()).ToList();
            model.ArtifactFileName = studentArtifact?.InputFileName;

            AutoApplyAnswerValueForUnAnsweredQuestions(model, applyAllCorrectAnswer, applyFullCreditAnswer, applyZeroCreditAnswer, answerData);

            model.AnsweredQuestions = GetAnsweredQuestions(studentDetails).OrderBy(x => x.QuestionOrder).ToList();

            model.UnansweredQuestions =
                model.UnansweredQuestions.Where(
                    x => model.AnsweredQuestions.Any(y => y.QuestionId == x.QuestionId) == false).ToList();

            GetBubbleSheetDisplayLable(model, studentId); // Assign layout custom label
            if (studentDetails.VirtualTestId.HasValue)
            {
                var virtualTestFile =
                    virtualTestFileService.GetFirstOrDefaultByVirtualTest(studentDetails.VirtualTestId.Value);

                if (virtualTestFile != null)
                {
                    model.VirtualTestFileKey = virtualTestFile.FileKey;
                    model.VirtualTestFileName = Helpers.Util.FormatRubricFileName(virtualTestFile.FileName);
                }
            }
            FillMaxChoiceToUnanswerQuestion(model);

            model.IsApplyFullCreditAnswer = applyFullCreditAnswer == 1;
            model.IsApplyZeroCreditAnswer = applyZeroCreditAnswer == 1;
            return PartialView("_BubbleSheetDetails", model);
        }

        [UploadifyPrincipal(Order = 1)]
        [HttpPost]
        public ActionResult UploadArtifactFile(BubbleSheetReviewUploadArtifactFileRequest request)
        {
            var bucketName = LinkitConfigurationManager.GetS3Settings().BubbleSheetBucketName;
            var bubbleSheetFolder = LinkitConfigurationManager.GetS3Settings().BubbleSheetFolder;
            var model = new BubbleSheetReviewUploadArtifactFileModel
            {
                CurrentUserID = CurrentUser.Id,
                BucketName = bucketName,
                BubbleSheetFolder = bubbleSheetFolder,
                DistrictID = CurrentDistrict(request.DistrictID),
                BubbleSheetID = request.BubbleSheetID,
                PostedFile = request.PostedFile,
                StudentID = request.StudentID,
                Ticket = request.Ticket
            };
            var response = bubbleSheetReviewDetailsService.UploadArtifactFile(model, PdfHelper.ConvertToPng);
            return Json(response);
        }

        [HttpPost]
        public ActionResult DeleteArtifactFile(BubbleSheetReviewDeleteArtifactFileRequest request)
        {
            var response = bubbleSheetReviewDetailsService.DeleteArtifactFile(request);
            return Json(response);
        }

        private void AutoApplyAnswerValueForUnAnsweredQuestions(BubbleSheetStudentDetailsViewModel model
            , int applyAllCorrectAnswer, int applyFullCreditAnswer, int applyZeroCreditAnswer, string answerData)
        {
            var submittedQuestionAnswers = (new JavaScriptSerializer()).Deserialize<List<UnansweredQuestionAnswer>>(answerData);
            if (submittedQuestionAnswers == null)
            {
                submittedQuestionAnswers = new List<UnansweredQuestionAnswer>();
            }

            if (applyAllCorrectAnswer == 0) // Clear Correct answer information when apply all correct answer option is not submit
            {
                foreach (var item in model.UnansweredQuestions)
                {
                    if (!item.IsOpenEndedQuestion)
                    {
                        var submittedItem = submittedQuestionAnswers.FirstOrDefault(x => x.QuestionId == item.QuestionId);
                        if (submittedItem != null)
                        {
                            item.CorrectLetter = submittedItem.SelectedAnswer;
                        }
                        else
                        {
                            item.CorrectLetter = "";
                        }
                    }
                }
            }

            if (applyFullCreditAnswer == 1)
            {
                foreach (var item in model.UnansweredQuestions)
                {
                    if (item.IsOpenEndedQuestion)
                    {
                        item.CorrectLetter = item.PointsPossible.ToString();
                    }
                }
            }
            else if (applyZeroCreditAnswer == 1)
            {
                foreach (var item in model.UnansweredQuestions)
                {
                    if (item.IsOpenEndedQuestion)
                    {
                        item.CorrectLetter = "0";
                    }
                }
            }
            else
            {
                // set user-submitted data in case CreditAnswer option is not set
                foreach (var item in model.UnansweredQuestions)
                {
                    if (item.IsOpenEndedQuestion)
                    {
                        var submittedItem = submittedQuestionAnswers.FirstOrDefault(x => x.QuestionId == item.QuestionId);
                        if (submittedItem != null)
                        {
                            item.CorrectLetter = submittedItem.SelectedAnswer;
                        }
                    }
                }
            }
        }

        private BubbleSheetStudentDetailsViewModel GetBubbleSheetDisplayLable(BubbleSheetStudentDetailsViewModel model, int studentId)
        {
            var student = studentService.GetStudentById(studentId);
            if (student != null)
            {
                var unansweredQuestionLableConfiguration =
                    districtConfigurationService.GetDistrictConfigurationByKey(student.DistrictId, DistrictConfigurationKey.BsUnansweredQuestionLabelKey);
                var answeredQuestionLableConfiguration =
                    districtConfigurationService.GetDistrictConfigurationByKey(student.DistrictId, DistrictConfigurationKey.BsAnsweredQuestionLabelKey);
                var questionLableConfiguration =
                    districtConfigurationService.GetDistrictConfigurationByKey(student.DistrictId, DistrictConfigurationKey.BsQuestionLabelKey);
                var answerChoicesLableConfiguration =
                    districtConfigurationService.GetDistrictConfigurationByKey(student.DistrictId, DistrictConfigurationKey.BsAnswerChoicesLabelKey);

                if (unansweredQuestionLableConfiguration != null)
                    model.UnansweredQuestionLable = unansweredQuestionLableConfiguration.Value;
                if (answeredQuestionLableConfiguration != null)
                    model.AnsweredQuestionLable = answeredQuestionLableConfiguration.Value;
                if (questionLableConfiguration != null)
                    model.QuestionLable = questionLableConfiguration.Value;
                if (answerChoicesLableConfiguration != null)
                    model.AnswerChoicesLable = answerChoicesLableConfiguration.Value;
            }

            return model;
        }

        public IQueryable<BubbleSheetDetailsStudentListViewModel> GetStudentsByTicketAndClass(string id, int? classId)
        {
            var totalPage = BubbleSheetWsHelper.GetTotalPageOfTestForOneStudent(id,
                ConfigurationManager.AppSettings["ApiKey"]);
            var data = bubbleSheetStudentResultsService.GetStudentsOfBubbleSheetByTicketWithStatusInBatch(id, classId.GetValueOrDefault());

            var result = data.Select(x => new BubbleSheetDetailsStudentListViewModel
            {
                Name = x.StudentName,
                Status = x.Status,
                Graded = BubbleSheetWsHelper.GetGradedCountForStudent(x, totalPage),
                PointsEarned = x.PointsEarned + "/" + x.PointsPossible,
                Id = x.StudentId,
                BubbleSheetId = x.BubbleSheetId,
                BubbleSheetFinalStatus = x.BubbleSheetFinalStatus,
                ArtifactFileName = x.ArtifactFileName,
            });

            return result.AsQueryable();
        }       

        [HttpPost]
        [AjaxOnly]
        public ActionResult SubmitTestQuestions(TestQuestionsViewModel model)
        {
            if (model.BubbleSheetId.Equals(0)) { return HttpNotFound(); }
            var bubbleSheet = bubbleSheetService.GetBubbleSheetById(model.BubbleSheetId);

            if (bubbleSheet == null)
            {
                return HttpNotFound();
            }

            if (CheckUserCanAccessClass(CurrentUser.Id, CurrentUser.RoleId, bubbleSheet.ClassId.GetValueOrDefault()) ==
                false)
            {
                return HttpNotFound();
            }

            if (model.BubbleSheetFileId.Equals(0))
            {
                var newFile = CreateBubbleSheetFileForBlankSheet(model);
                newFile.OutputFileName = "new";
                newFile.InputFileName = "new";
                newFile.InputFilePath = "new";
                bubbleSheetFileService.SaveBubbleSheetFile(newFile);
                model.BubbleSheetFileId = bubbleSheetFileService.GetNewlyCreatedBubbleSheetFileId(newFile);
            }

            //Move this line to after call API, because we dont' want change status if API Error
            //bubbleSheetFileService.SaveBubbleSheetFileCorrection(model.BubbleSheetFileId);

            var bubbleSheetFile = AssignBubbleSheetFile(model);
            bubbleSheetFile.UserId = CurrentUser.Id;
            //var bubbleSheetFileCloudEntity = GetBubbleSheetFileCloudEntity(model, bubbleSheetFile);
            try
            {
                var bubbleSheetFileCloudEntity = GetBubbleSheetFileAwsEntity(model, bubbleSheetFile);
                if (bubbleSheetFileCloudEntity == null)
                {
                    return Json(false);
                }
                testResubmissionService.AssignNewQuestions(GetCombinedTestQuestions(model), bubbleSheetFileCloudEntity);

                //var gradeRequest = readResultService.CreateGradeRequest(bubbleSheetFileCloudEntity);
                //gradeRequestService.SendGradeRequest(gradeRequest);
                var response = BubbleSheetWsHelper.SendGradeRequest(bubbleSheetFileCloudEntity);
                if (response == null || response.IsSuccess == false)
                {
                    return Json(false);
                }
                //Only Change Status when submit Success
                bubbleSheetFileService.SaveBubbleSheetFileCorrection(model.BubbleSheetFileId);
                return Json(true);
            }
            catch (GetRequestTimeoutException e)
            {
                return Json(false);
            }
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult MarkStudentAsAbsent(TestQuestionsViewModel model)
        {
            var newFile = CreateBubbleSheetFileForBlankSheet(model);
            newFile.IsConfirmed = true;
            bubbleSheetFileService.SaveBubbleSheetFile(newFile);
            return Json(true);
        }

        private IEnumerable<UnansweredQuestionAnswer> GetCombinedTestQuestions(TestQuestionsViewModel model)
        {
            var actuallyAnsweredQuestionsForTest =
                previouslyAnsweredQuestionsService.GetPreviouslyAnsweredQuestionsByStudentIdAndBubbleSheetId(
                    model.StudentId, model.BubbleSheetId).ToList();

            return
                testResubmissionService.CombinePreviouslyAnsweredQuestionsWithNewlyAnsweredQuestions(
                    actuallyAnsweredQuestionsForTest, model.Answers).AsEnumerable();
        }

        private BubbleSheetFile CreateBubbleSheetFileForBlankSheet(TestQuestionsViewModel model)
        {
            return new BubbleSheetFile
            {
                BubbleSheetId = model.BubbleSheetId,
                Date = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow,
                Barcode1 = string.Format("10060000{0}", model.BubbleSheetId),
                Barcode2 = string.Format("10001{0}", DateTime.UtcNow.ToString("yyMMdd")),
                InputFileName = string.Empty,
                InputFilePath = string.Empty,
                Resolution = "300",
                PageNumber = 1,
                FileDisposition = "Created",
                OutputFileName = string.Empty,
                RosterPosition = model.RosterPosition,
                ResultString = string.Empty,
                UserId = CurrentUser.Id,
                DistrictId = schoolService.GetSchoolById(bubbleSheetService.GetBubbleSheetById(model.BubbleSheetId).SchoolId.GetValueOrDefault()).DistrictId
            };
        }

        private BubbleSheetFile AssignBubbleSheetFile(TestQuestionsViewModel model)
        {
            if (unansweredQuestionService.VerifyUnansweredQuestionsExistByStudentIdAndBubbleSheetId(model.StudentId, model.BubbleSheetId, model.ClassId))
            {
                return bubbleSheetFileService.GetBubbleSheetFileById(model.BubbleSheetFileId);
            }

            var bubbleSheet = bubbleSheetService.GetBubbleSheetById(model.BubbleSheetId);
            var districtId = bubbleSheetService.GetDistrictIdFromSchoolBySchoolId(bubbleSheet.SchoolId);
            var bubbleSheetFile = bubbleSheetFileService.InitializeNewBubbleSheetFile(districtId, model.BubbleSheetId, CurrentUser);

            if (bubbleSheet.StudentId.Equals(0))
            {
                bubbleSheetFile.RosterPosition = studentRosterService.GetStudentRosterPositionByTicketAndStudentId(bubbleSheet.Ticket, model.StudentId, model.ClassId);
            }

            return bubbleSheetFile;
        }

        private ReadResult GetBubbleSheetFileAwsEntity(TestQuestionsViewModel model, BubbleSheetFile bubbleSheetFile)
        {
            //var bubbleSheetFileCloudEntity = testResultService.Select(bubbleSheetFile.InputFilePath, bubbleSheetFile.UrlSafeOutputFileName);
            ReadResult bubbleSheetFileCloudEntity;
            try
            {
                bubbleSheetFileCloudEntity = BubbleSheetWsHelper.GetReadResult(bubbleSheetFile.InputFilePath,
                bubbleSheetFile.UrlSafeOutputFileName);
            }
            catch (GetRequestTimeoutException e)
            {
                throw e;
            }

            if (bubbleSheetFileCloudEntity.IsNull())
            {
                bubbleSheetFileCloudEntity = CreateNewBubbleSheetFileReadResultForMissingSheets(model, bubbleSheetFile);
            }
            else
            {
                bubbleSheetFileCloudEntity.InputPath = bubbleSheetFile.InputFilePath;
                bubbleSheetFileCloudEntity.InputFileName = bubbleSheetFile.InputFileName;
                bubbleSheetFileCloudEntity.OutputFile = bubbleSheetFile.OutputFileName;
            }

            return bubbleSheetFileCloudEntity;
        }

        private CloudEntity<ReadResult> CreateNewBubbleSheetFileCloudEntityForMissingSheets(TestQuestionsViewModel model, BubbleSheetFile bubbleSheetFile)
        {
            var bubbleSheetFileCloudEntity = readResultService.CreateNewBubbleSheetFileCloudEntity(bubbleSheetFile);
            var ticket = bubbleSheetService.GetBubbleSheetById(bubbleSheetFile.BubbleSheetId).Ticket;
            var questions = unansweredQuestionService.GetQuestionsWithAnswerChoicesForMissingSheets(ticket, model.StudentId, model.ClassId);
            var questionAnswers = questions.Select(unansweredQuestion => new UnansweredQuestionAnswer { QuestionOrder = unansweredQuestion.QuestionOrder }).ToList();
            readResultService.SetBubbleSheetCloudEntityQuestions(bubbleSheetFileCloudEntity, questionAnswers.Count);
            return bubbleSheetFileCloudEntity;
        }

        private ReadResult CreateNewBubbleSheetFileReadResultForMissingSheets(TestQuestionsViewModel model, BubbleSheetFile bubbleSheetFile)
        {
            //var bubbleSheetFileCloudEntity = readResultService.CreateNewBubbleSheetFileCloudEntity(bubbleSheetFile);
            var bubbleSheetFileCloudEntity = readResultService.CreateNewBubbleSheetFileReadResult(bubbleSheetFile);
            var ticket = bubbleSheetService.GetBubbleSheetById(bubbleSheetFile.BubbleSheetId).Ticket;
            var questions = unansweredQuestionService.GetQuestionsWithAnswerChoicesForMissingSheets(ticket, model.StudentId, model.ClassId);
            var questionAnswers = questions.Select(unansweredQuestion => new UnansweredQuestionAnswer { QuestionOrder = unansweredQuestion.QuestionOrder }).ToList();
            readResultService.SetBubbleSheetReadResultQuestions(bubbleSheetFileCloudEntity, questionAnswers.Count);
            return bubbleSheetFileCloudEntity;
        }

        private bool CanUserAccessBubbleSheet(string id)
        {
            return true;
            //return GetBubbleSheetReviewListData().FirstOrDefault(x => x.Ticket.Equals(id)).IsNotNull();
        }

        private IEnumerable<UnansweredQuestion> GetUnansweredQuestions(BubbleSheetReviewDetails studentDetails)
        {
            if (studentDetails.HasBubbleSheetFile && !studentDetails.HasCreatedBubbleSheetFile && studentDetails.ResultDate.HasValue)
            {
                return unansweredQuestionService.GetUnansweredQuestionsByTicketAndStudentId(studentDetails.Ticket, studentDetails.StudentId, studentDetails.ClassId);
            }

            return unansweredQuestionService.GetQuestionsWithAnswerChoicesForMissingSheets(studentDetails.Ticket, studentDetails.StudentId, studentDetails.ClassId);
        }

        private IEnumerable<AlreadyAnsweredQuestion> GetAnsweredQuestions(BubbleSheetReviewDetails studentDetails)
        {
            return alreadyAnsweredQuestionService.GetAlreadyAnsweredQuestionsForStudentBubbleSheet(studentDetails.StudentId, studentDetails.BubbleSheetId);
        }

        private static object lockItem = new object();

        private IEnumerable<PreviousBubbleSheetDetailsViewModel> GetPreviousBubbleSheetsForStudent(List<BubbleSheetReviewDetails> studentDetails)
        {
            //var sheets = bubbleSheetReviewDetailsService.GetAllBubbleSheetReviewDetailsForStudentByTicketAndClassId(studentDetails.StudentId, studentDetails.Ticket, studentDetails.ClassId).ToList();
            var results = new List<PreviousBubbleSheetDetailsViewModel>();

            foreach (var bubbleSheetReviewDetail in studentDetails)
            {
                //var imageUrl = !bubbleSheetReviewDetail.OutputFileName.Equals("temp")
                //    ? BubbleSheetWsHelper.GetTestImageUrl(bubbleSheetReviewDetail.OutputFileName,
                //        ConfigurationManager.AppSettings["ApiKey"])
                //    : string.Empty;
                //if (string.IsNullOrEmpty(imageUrl) && !bubbleSheetReviewDetail.OutputFileName.Equals("temp"))
                //{
                //    imageUrl = GetImageUrl<TestImage>(bubbleSheetReviewDetail.OutputFileName);
                //}
                string imageUrl = string.Empty;
                bool isOnlyOnePage = false;
                if (!bubbleSheetReviewDetail.OutputFileName.Equals("temp"))
                {
                    isOnlyOnePage = CheckOnlyOneBubbleSheetFileByBubbleSheetFileId(bubbleSheetReviewDetail.BubbleSheetFileId.GetValueOrDefault(), out imageUrl);
                }
                var viewModel = new PreviousBubbleSheetDetailsViewModel
                {
                    BubbleSheetFileId = bubbleSheetReviewDetail.BubbleSheetFileId,
                    FileName = bubbleSheetReviewDetail.InputFileName,
                    ImageUrl = imageUrl,
                    UploadedBy = bubbleSheetReviewDetail.UploadedBy,
                    UploadedDate = bubbleSheetReviewDetail.UploadedDate,
                    OnlyOnePage = isOnlyOnePage
                };
                results.Add(viewModel);
            }

            return results;
        }

        private BubbleSheetStudentDetailsViewModel BindStudentDetailsToViewModel(BubbleSheetReviewDetails studentDetails)
        {
            return new BubbleSheetStudentDetailsViewModel
            {
                BubbleSheetFileId = studentDetails.BubbleSheetFileId,
                BubbleSheetId = studentDetails.BubbleSheetId,
                StudentId = studentDetails.StudentId,
                ClassId = studentDetails.ClassId,
                Ticket = studentDetails.Ticket,
                StudentName = studentDetails.StudentName,
                TeacherName = studentDetails.TeacherName,
                ClassName = studentDetails.ClassName,
                SchoolName = studentDetails.SchoolName,
                UploadedDate = studentDetails.UploadedDate,
                UploadedBy = studentDetails.UploadedBy,
                FileName = studentDetails.PageNumber == -1 ? null : studentDetails.InputFileName,
                HasBubbleSheetFile = studentDetails.HasBubbleSheetFile,
                RosterPosition = studentDetails.RosterPosition,
                IsManualEntry = studentDetails.IsManualEntry
            };
        }

        private void FillMaxChoiceToUnanswerQuestion(BubbleSheetStudentDetailsViewModel model)
        {
            if (model.UnansweredQuestions.Count > 0)
            {
                for (int i = 0; i < model.UnansweredQuestions.Count(); i++)
                {
                    if (model.UnansweredQuestions[i].QTISchemaId == (int)QtiSchemaEnum.ChoiceMultiple
                        || model.UnansweredQuestions[i].QTISchemaId == (int)QtiSchemaEnum.ChoiceMultipleVariable)
                    {
                        var xmlContent = Util.BindQtiItemXmlContentFromXml(model.UnansweredQuestions[i].XmlContent);
                        if (xmlContent != null)
                        {
                            model.UnansweredQuestions[i].MaxChoice = xmlContent.MaxChoices;
                            model.UnansweredQuestions[i].Cardinality = xmlContent.Cardinality;
                        }

                        // Get top greatest score answer only
                        if (model.UnansweredQuestions[i].MaxChoice > 0)
                        {
                            var correctLetters = model.UnansweredQuestions[i].CorrectLetter.Split(',');
                            if (correctLetters.Count() > model.UnansweredQuestions[i].MaxChoice)
                            {
                                model.UnansweredQuestions[i].CorrectLetter = string.Join(",", correctLetters.Take(model.UnansweredQuestions[i].MaxChoice));
                            }
                        }
                    }
                }
            }
            if (model.AnsweredQuestions.Count > 0)
            {
                for (int i = 0; i < model.AnsweredQuestions.Count(); i++)
                {
                    if (model.AnsweredQuestions[i].QTISchemaId == (int)QtiSchemaEnum.ChoiceMultiple
                        || model.AnsweredQuestions[i].QTISchemaId == (int)QtiSchemaEnum.ChoiceMultipleVariable)
                    {
                        var xmlContent = Util.BindQtiItemXmlContentFromXml(model.AnsweredQuestions[i].XmlContent);
                        if (xmlContent != null)
                        {
                            model.AnsweredQuestions[i].MaxChoice = xmlContent.MaxChoices;
                            model.AnsweredQuestions[i].Cardinality = xmlContent.Cardinality;
                        }
                    }
                }
            }
        }

        //\
        public ActionResult ACTPage(string id, int? classId, string test)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            if (bubbleSheetFileService.IsGenericGroup(id, classId.GetValueOrDefault())
                && CheckUserCanAccessClass(CurrentUser.Id, CurrentUser.RoleId, id))
            {
                return RedirectToAction("AssignStudentsToTestActSat", "GenericSheet", new { qticket = id, qclassId = classId.GetValueOrDefault() });
            }

            if (CheckUserCanAccessClass(CurrentUser.Id, CurrentUser.RoleId, classId.GetValueOrDefault()) == false)
            {
                return RedirectToAction("Index", "BubbleSheetReview");
            }

            if (bubbleSheetFileService.IsFileGenericWithNoMappings(id, classId.GetValueOrDefault()))
            {
                return RedirectToAction("AssignStudentsToTestActSat", "GenericSheet", new { qticket = id, qclassId = classId.GetValueOrDefault() });
            }

            var model = new BubbleSheetReviewDetailsViewModel
            {
                Ticket = id,
                CanAccess = CanUserAccessBubbleSheet(id),
                ClassId = classId.GetValueOrDefault(),
                HasGenericSheet = bubbleSheetFileService.HasGenericSheets(id),
                TestName = test
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult GetACTBubbleSheetDetails(string ticket, int studentId, int? classId)
        {
            if (CheckUserCanAccessClass(CurrentUser.Id, CurrentUser.RoleId, classId.GetValueOrDefault()) == false)
            {
                return RedirectToAction("Index", "BubbleSheetReview");
            }
            var allStudentDetails =
                bubbleSheetReviewDetailsService.GetAllBubbleSheetReviewDetailsForStudentByTicketAndClassId(studentId,
                    ticket, classId.GetValueOrDefault()).OrderByDescending(x => x.BubbleSheetFileId).ToList();

            if (allStudentDetails.Any() == false)
            {
                //return RedirectToAction("ReviewDetails", ticket);
                return Json(true);
            }

            var timeZoneId = stateService.GetTimeZoneId(CurrentUser.StateId.GetValueOrDefault());
            var zn = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            foreach (var studentDetail in allStudentDetails)
            {
                if (studentDetail.UploadedDate.HasValue)
                    studentDetail.UploadedDate = TimeZoneInfo.ConvertTimeFromUtc(studentDetail.UploadedDate.Value, zn);
            }

            var studentArtifact = allStudentDetails.FirstOrDefault(x => x.PageNumber == -1);
            var studentDetails = allStudentDetails.FirstOrDefault(x => x.PageNumber != -1) ?? studentArtifact;

            if (studentDetails.OutputFileName.Equals("temp"))
            {
                var lastestBubblesheetFileHasImage =
                    allStudentDetails.FirstOrDefault(x => x.OutputFileName != string.Empty && x.OutputFileName != "temp");
                if (lastestBubblesheetFileHasImage != null)
                {
                    studentDetails.OutputFileName = lastestBubblesheetFileHasImage.OutputFileName;
                }
            }

            var model = ACTBindStudentDetailsToViewModel(studentDetails);
            model.ArtifactFileName = studentArtifact?.InputFileName;
            if (studentDetails.HasBubbleSheetFile)
            {
                int bubbleSheetFileId = studentDetails.BubbleSheetFileId.HasValue
                    ? studentDetails.BubbleSheetFileId.Value
                    : 0;

                var vSubs = bubbleSheetFileService.GetLastestBubbleSheetFileSubByBubbleSheetId(model.BubbleSheetId);
                if (vSubs.Any())
                {
                    var defaultSubFile = GetDefaultSubFile(vSubs);
                    model.OnlyOnePage = vSubs.Count == 1;
                    var imageUrl = BubbleSheetWsHelper.GetTestImageUrl(defaultSubFile.OutputFileName, ConfigurationManager.AppSettings["ApiKey"]);
                    model.ImageUrl = imageUrl;
                }
                model.PreviousBubbleSheets = GetPreviousBubbleSheetsForStudent(allStudentDetails).ToList();
            }

            GetACTBubbleSheetDisplayLable(model, studentId); // Assign layout custom label s
            if (studentDetails.VirtualTestId.HasValue)
            {
                var virtualTestFile =
                    virtualTestFileService.GetFirstOrDefaultByVirtualTest(studentDetails.VirtualTestId.Value);

                if (virtualTestFile != null)
                {
                    model.VirtualTestFileKey = virtualTestFile.FileKey;
                    model.VirtualTestFileName = Helpers.Util.FormatRubricFileName(virtualTestFile.FileName);
                }
                //Get Unanswered Question
                model.UnansweredQuestions = actAnswerQuestionService.GetUnansweredQuestions(studentDetails.VirtualTestId.Value, studentDetails.BubbleSheetId, studentDetails.StudentId);
                //Get Answered Question
                model.AnsweredQuestions = actAnswerQuestionService.GetAnsweredQuestions(studentDetails.VirtualTestId.Value, studentDetails.BubbleSheetId, studentDetails.StudentId);

                var virtualTest = virtualTestService.GetTestById(studentDetails.VirtualTestId.Value);
                model.IsRemoveZeroPointForEssay = virtualTest.VirtualTestSubTypeID.GetValueOrDefault() ==
                                                  (int)VirtualTestSubType.NewACT;

                var essayQuestionWithTag =
                    bubbleSheetPrintingService.GetDomainTagForEssaySection(
                        studentDetails.VirtualTestId.GetValueOrDefault());
                foreach (var actUnansweredQuestion in model.UnansweredQuestions)
                {
                    var tagInfo =
                        essayQuestionWithTag.FirstOrDefault(x => x.Index == actUnansweredQuestion.OrderSectionQuestionIndex - 1 && actUnansweredQuestion.OrderSectionIndex == x.SectionID);
                    if (tagInfo != null)
                    {
                        actUnansweredQuestion.DomainTag = tagInfo.TagName;
                    }
                }
                foreach (var actUnansweredQuestion in model.AnsweredQuestions)
                {
                    var tagInfo =
                        essayQuestionWithTag.FirstOrDefault(x => x.Index == actUnansweredQuestion.OrderSectionQuestionIndex - 1 && actUnansweredQuestion.OrderSectionIndex == x.SectionID);
                    if (tagInfo != null)
                    {
                        actUnansweredQuestion.DomainTag = tagInfo.TagName;
                    }
                }
            }
            ACTFillMaxChoiceToUnanswerQuestion(model);
            return PartialView("_ACTBubbleSheetDetails", model);
        }

        private BubbleSheetFileSub GetDefaultSubFile(List<BubbleSheetFileSub> subFiles)
        {
            return subFiles.FirstOrDefault(o => o.PageNumber == 2 && o.OutputFileName != "temp")
                ?? subFiles.FirstOrDefault(o => o.PageNumber == -1)
                ?? subFiles.First();
        }

        [HttpGet]
        public ActionResult GetBubbleSheetFileSubById(int Id)
        {
            var lst = new BubbleSheetFileSubListViewModel();
            var vSubs = bubbleSheetFileService.GetBubbleSheetFileSubs(Id).OrderBy(x => x.PageNumber >= 0 ? 0 : 1).ThenBy(x => x.PageNumber).ToList();
            if (vSubs.Count > 0)
            {
                foreach (BubbleSheetFileSub bubbleSheetFileSub in vSubs)
                {
                    lst.ListFileSubViewModels.Add(new BubbleSheetFileSubViewModel()
                    {
                        ImageUrl = BubbleSheetWsHelper.GetTestImageUrl(bubbleSheetFileSub.OutputFileName, ConfigurationManager.AppSettings["ApiKey"]),
                        SubFileName = bubbleSheetFileSub.InputFileName
                    });
                }
                return PartialView("_DisplaySubFile", lst);
            }
            var bubblesheetFile = bubbleSheetFileService.GetBubbleSheetFileById(Id);
            if (bubblesheetFile != null)
            {
                lst.ListFileSubViewModels.Add(new BubbleSheetFileSubViewModel()
                {
                    ImageUrl = BubbleSheetWsHelper.GetTestImageUrl(bubblesheetFile.OutputFileName, ConfigurationManager.AppSettings["ApiKey"]),
                    SubFileName = bubblesheetFile.InputFileName
                });
            }
            else
            {
                lst.ListFileSubViewModels.Add(new BubbleSheetFileSubViewModel()
                {
                    ImageUrl = Url.Content("~/Content/images/noimage.png"),
                    SubFileName = string.Empty
                });
            }
            return PartialView("_DisplaySubFile", lst);
        }

        public ActionResult GetBubbleSheetFileSubByBubbleSheetID(int Id)
        {
            var vSubs = bubbleSheetFileService.GetLastestBubbleSheetFileSubByBubbleSheetId(Id)
                .OrderBy(x => x.PageNumber >= 0 ? 0 : 1)
                .ThenBy(o => o.PageNumber)
                .ToList();
            var lstOutputFileName = new List<string>();
            if (vSubs.Count > 0)
            {
                var lst = new BubbleSheetFileSubListViewModel();
                foreach (BubbleSheetFileSub bubbleSheetFileSub in vSubs)
                {
                    if (lstOutputFileName.Any(x => x.Equals(bubbleSheetFileSub.OutputFileName))
                        || bubbleSheetFileSub.OutputFileName == "temp") continue;
                    lstOutputFileName.Add(bubbleSheetFileSub.OutputFileName);
                    lst.ListFileSubViewModels.Add(new BubbleSheetFileSubViewModel()
                    {
                        ImageUrl = BubbleSheetWsHelper.GetTestImageUrl(bubbleSheetFileSub.OutputFileName, ConfigurationManager.AppSettings["ApiKey"]),
                        SubFileName = bubbleSheetFileSub.InputFileName
                    });
                }
                return PartialView("_DisplaySubFile", lst);
            }
            return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult SubmitACTTestQuestions(TestQuestionsViewModel model)
        {
            if (model.BubbleSheetId.Equals(0)) { return HttpNotFound(); }

            var bubbleSheet = bubbleSheetService.GetBubbleSheetById(model.BubbleSheetId);

            if (bubbleSheet == null)
            {
                return HttpNotFound();
            }

            if (CheckUserCanAccessClass(CurrentUser.Id, CurrentUser.RoleId, bubbleSheet.ClassId.GetValueOrDefault()) ==
                false)
            {
                return HttpNotFound();
            }

            if (model.BubbleSheetFileId.Equals(0))
            {
                var newFile = CreateBubbleSheetFileForBlankSheet(model);
                newFile.OutputFileName = "new";
                newFile.InputFileName = "new";
                newFile.InputFilePath = "new";
                bubbleSheetFileService.SaveBubbleSheetFile(newFile);
                model.BubbleSheetFileId = bubbleSheetFileService.GetNewlyCreatedBubbleSheetFileId(newFile);
            }
            //Move this line to after call API, because we dont' want change status if API Error
            //bubbleSheetFileService.SaveBubbleSheetFileCorrection(model.BubbleSheetFileId);

            var bubbleSheetFile = AssignBubbleSheetFile(model);
            bubbleSheetFile.UserId = CurrentUser.Id;

            //split submitted questions to 2 page (cover page and bubblesheet page)
            var bubbleSheetFileSub =
                bubbleSheetFileService.GetBubbleSheetFileSubs(model.BubbleSheetFileId)
                    .Where(x => x.PageNumber == 1 || x.PageNumber == 2)
                    .ToList();

            var finalAnswerList = ACTGetCombinedTestQuestions(model).ToList();
            var coverPageAnswerList = finalAnswerList.Where(x => x.SectionIndex == 0 || x.SectionIndex == 5)
                .OrderBy(x => x.SectionIndex).ThenBy(x => x.SectionQuestionIndex).ToList();
            var bubbleSheetPageAnswerList = finalAnswerList.Where(x => x.SectionIndex > 0 && x.SectionIndex < 5).ToList();
            var isNewACT = IsNewACTTest(model.BubbleSheetId);
            try
            {
                var bubbleSheetFileCloudEntity = GetBubbleSheetFileAwsEntity(model, bubbleSheetFile);
                if (bubbleSheetFileCloudEntity == null)
                {
                    return Json(false);
                }
                bubbleSheetFileCloudEntity.TestType = isNewACT ? 5 : 3;
                bubbleSheetFileCloudEntity.ACTPageIndex = 2;
                bubbleSheetFileCloudEntity.RosterPosition = "0";
                bubbleSheetFileCloudEntity.PageNumber = 0; //identify resubmit
                if (bubbleSheetFileSub.Any(x => x.PageNumber == 2))
                {
                    var sub = bubbleSheetFileSub.First(x => x.PageNumber == 2);
                    bubbleSheetFileCloudEntity.InputPath = sub.InputFilePath;
                    bubbleSheetFileCloudEntity.OutputFile = sub.OutputFileName;
                    bubbleSheetFileCloudEntity.InputFileName = sub.InputFileName;
                }
                testResubmissionService.ACTAssignNewQuestions(bubbleSheetPageAnswerList, bubbleSheetFileCloudEntity);

                var bubbleSheetFileForCoverPage = GetBubbleSheetFileAwsEntity(model, bubbleSheetFile);
                if (bubbleSheetFileForCoverPage == null)
                {
                    return Json(false);
                }
                bubbleSheetFileForCoverPage.TestType = isNewACT ? 5 : 3;
                bubbleSheetFileForCoverPage.ACTPageIndex = 1;
                bubbleSheetFileForCoverPage.RosterPosition = "0";
                bubbleSheetFileForCoverPage.PageNumber = 0; //identify resubmit
                if (bubbleSheetFileSub.Any(x => x.PageNumber == 1))
                {
                    var sub = bubbleSheetFileSub.First(x => x.PageNumber == 1);
                    bubbleSheetFileForCoverPage.InputPath = sub.InputFilePath;
                    bubbleSheetFileForCoverPage.OutputFile = sub.OutputFileName;
                    bubbleSheetFileForCoverPage.InputFileName = sub.InputFileName;
                }
                testResubmissionService.ACTAssignNewQuestions(coverPageAnswerList, bubbleSheetFileForCoverPage);
                if (!bubbleSheetFile.BubbleSheetId.ToString().Equals(bubbleSheetFileForCoverPage.Barcode1))
                {
                    bubbleSheetFileCloudEntity.Barcode1 = model.BubbleSheetId.ToString();
                    bubbleSheetFileForCoverPage.Barcode1 = model.BubbleSheetId.ToString();
                }
                var response = BubbleSheetWsHelper.SendGradeRequestBatch(new List<ReadResult>
                                                                         {
                                                                             bubbleSheetFileCloudEntity,
                                                                             bubbleSheetFileForCoverPage
                                                                         });
                if (response == null || response.IsSuccess == false)
                {
                    return Json(false);
                }
                //Only Change Status when submit Success
                bubbleSheetFileService.SaveBubbleSheetFileCorrection(model.BubbleSheetFileId);
                return Json(true);
            }
            catch (GetRequestTimeoutException e)
            {
                return Json(false);
            }
        }

        private ACTBubbleSheetStudentDetailsViewModel ACTBindStudentDetailsToViewModel(BubbleSheetReviewDetails studentDetails)
        {
            return new ACTBubbleSheetStudentDetailsViewModel
            {
                BubbleSheetFileId = studentDetails.BubbleSheetFileId,
                BubbleSheetId = studentDetails.BubbleSheetId,
                StudentId = studentDetails.StudentId,
                ClassId = studentDetails.ClassId,
                Ticket = studentDetails.Ticket,
                StudentName = studentDetails.StudentName,
                TeacherName = studentDetails.TeacherName,
                ClassName = studentDetails.ClassName,
                SchoolName = studentDetails.SchoolName,
                UploadedDate = studentDetails.UploadedDate,
                UploadedBy = studentDetails.UploadedBy,
                FileName = studentDetails.InputFileName,
                HasBubbleSheetFile = studentDetails.HasBubbleSheetFile,
                RosterPosition = studentDetails.RosterPosition,
                IsManualEntry = studentDetails.IsManualEntry,
                ResultDate = studentDetails.ResultDate
            };
        }

        private void GetACTBubbleSheetDisplayLable(ACTBubbleSheetStudentDetailsViewModel model, int studentId)
        {
            var student = studentService.GetStudentById(studentId);
            if (student != null)
            {
                var unansweredQuestionLableConfiguration =
                    districtConfigurationService.GetDistrictConfigurationByKey(student.DistrictId, DistrictConfigurationKey.BsUnansweredQuestionLabelKey);
                var answeredQuestionLableConfiguration =
                    districtConfigurationService.GetDistrictConfigurationByKey(student.DistrictId, DistrictConfigurationKey.BsAnsweredQuestionLabelKey);
                var questionLableConfiguration =
                    districtConfigurationService.GetDistrictConfigurationByKey(student.DistrictId, DistrictConfigurationKey.BsQuestionLabelKey);
                var answerChoicesLableConfiguration =
                    districtConfigurationService.GetDistrictConfigurationByKey(student.DistrictId, DistrictConfigurationKey.BsAnswerChoicesLabelKey);

                if (unansweredQuestionLableConfiguration != null)
                    model.UnansweredQuestionLable = unansweredQuestionLableConfiguration.Value;
                if (answeredQuestionLableConfiguration != null)
                    model.AnsweredQuestionLable = answeredQuestionLableConfiguration.Value;
                if (questionLableConfiguration != null)
                    model.QuestionLable = questionLableConfiguration.Value;
                if (answerChoicesLableConfiguration != null)
                    model.AnswerChoicesLable = answerChoicesLableConfiguration.Value;
            }

            //return model;
        }

        private void ACTFillMaxChoiceToUnanswerQuestion(ACTBubbleSheetStudentDetailsViewModel model)
        {
            List<int> lst = new List<int>();
            if (model.UnansweredQuestions.Count > 0)
            {
                for (int i = 0; i < model.UnansweredQuestions.Count(); i++)
                {
                    if (model.UnansweredQuestions[i].QTISchemaId == (int)QtiSchemaEnum.ChoiceMultiple)
                    {
                        var xmlContent = Util.BindQtiItemXmlContentFromXml(model.UnansweredQuestions[i].XmlContent);
                        if (xmlContent != null)
                            model.UnansweredQuestions[i].MaxChoice = xmlContent.MaxChoices;
                    }
                }
            }
            if (model.AnsweredQuestions.Count > 0)
            {
                for (int i = 0; i < model.AnsweredQuestions.Count(); i++)
                {
                    if (model.AnsweredQuestions[i].QTISchemaId == (int)QtiSchemaEnum.ChoiceMultiple)
                    {
                        var xmlContent = Util.BindQtiItemXmlContentFromXml(model.AnsweredQuestions[i].XmlContent);
                        if (xmlContent != null)
                            model.AnsweredQuestions[i].MaxChoice = xmlContent.MaxChoices;
                    }
                }
            }
        }

        private IEnumerable<UnansweredQuestionAnswer> ACTGetCombinedTestQuestions(TestQuestionsViewModel model)
        {
            int virtualTestId = 0;
            var bubbleSheet = bubbleSheetService.GetBubbleSheetById(model.BubbleSheetId);
            if (bubbleSheet != null && bubbleSheet.TestId.HasValue)
                virtualTestId = bubbleSheet.TestId.Value;
            var allQuestion = actAnswerQuestionService.GetAllQuestions(virtualTestId);
            var actuallyAnsweredQuestionsForTest = actAnswerQuestionService.GetExistAnswerForResubmit(virtualTestId, model.BubbleSheetId, model.StudentId);
            foreach (ACTAlreadyAnsweredQuestion question in actuallyAnsweredQuestionsForTest)
            {
                allQuestion.RemoveAll(o => o.QuestionId == question.QuestionId);
            }
            actuallyAnsweredQuestionsForTest.AddRange(allQuestion);

            return testResubmissionService.ACTCombinePreviouslyAnsweredQuestionsWithNewlyAnsweredQuestions(actuallyAnsweredQuestionsForTest, model.Answers).AsEnumerable();
        }

        [HttpGet]
        public ActionResult ACTGetStudentsForTest(string id, int? classId)
        {
            var parser = new DataTableParser<BubbleSheetDetailsStudentListViewModel>();
            if (CheckUserCanAccessClass(CurrentUser.Id, CurrentUser.RoleId, classId.GetValueOrDefault()) == false)
            {
                var emptyStudent = new List<BubbleSheetDetailsStudentListViewModel>().AsQueryable();
                return Json(parser.ParseForClientSide(emptyStudent), JsonRequestBehavior.AllowGet);
            }
            var students = GetStudentsByTicketAndClass(id, classId).ToList();
            return Json(parser.ParseForClientSide(students.AsQueryable()), JsonRequestBehavior.AllowGet);
        }

        public ActionResult SATGetStudentsForTest(string id, int? classId)
        {
            var parser = new DataTableParser<BubbleSheetDetailsStudentListViewModel>();
            if (CheckUserCanAccessClass(CurrentUser.Id, CurrentUser.RoleId, classId.GetValueOrDefault()) == false)
            {
                var emptyStudent = new List<BubbleSheetDetailsStudentListViewModel>().AsQueryable();
                return Json(parser.ParseForClientSide(emptyStudent), JsonRequestBehavior.AllowGet);
            }

            var students = GetStudentsByTicketAndClass(id, classId).ToList();
            return Json(parser.ParseForClientSide(students.AsQueryable()), JsonRequestBehavior.AllowGet);
        }

        private void SATCheckInvalidStatus(BubbleSheet bs, List<BubbleSheetDetailsStudentListViewModel> students, VirtualSection essaySection)
        {
            var lstStudentStatus = actAnswerQuestionService.GetSATStudentStatuses(bs.TestId.Value, bs.Ticket, essaySection.VirtualSectionId);
            foreach (BubbleSheetDetailsStudentListViewModel student in students)
            {
                if (student.Id > 0)
                {
                    var lstAnswer = lstStudentStatus.Where(o => o.StudentId == student.Id).ToList();
                    if (lstAnswer.Count == 2)
                    {
                        bool isDistanceBetweenTwoPointGreaterThanOne =
                            Math.Abs(lstAnswer[0].PointsEarned - lstAnswer[1].PointsEarned) > 1;
                        bool isOnePointIsZero = (lstAnswer[0].PointsEarned == 0 || lstAnswer[1].PointsEarned == 0) &&
                                                lstAnswer[0].PointsEarned != lstAnswer[1].PointsEarned;
                        if (isDistanceBetweenTwoPointGreaterThanOne || isOnePointIsZero)
                        {
                            student.Status = "Invalid";
                        }
                    }
                }
            }
        }

        private void SATCheckIncompleteStatus(List<BubbleSheetDetailsStudentListViewModel> students, string ticket, int? classId)
        {
            var studentIdList = students.Select(x => x.Id).ToList();
            var listBubbleSheetDetailViewModel = bubbleSheetReviewDetailsService
                .GetAllBubbleSheetReviewDetailsForStudentListByTicketAndClassId(studentIdList, ticket,
                    classId.GetValueOrDefault());
            foreach (var student in students)
            {
                if (student.Id <= 0) continue;

                var bubbleSheetDetail =
                    listBubbleSheetDetailViewModel.Where(x => x.StudentId == student.Id && x.BubbleSheetFileId != null)
                        .ToList();

                if (bubbleSheetDetail.Any() == false) continue;

                var essayPageCount =
                    bubbleSheetFileService.GetBubbleSheetFileSubsByListId(bubbleSheetDetail.Select(x => x.BubbleSheetFileId.Value).ToList())
                        .Count(x => x.PageType.HasValue && (x.PageType == (int)SATPageType.EssayPage));

                if (essayPageCount < 2)
                {
                    student.Status = "Incomplete";
                }
            }
        }

        //\SAT
        public ActionResult SATPage(string id, int? classId, string test)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            if (bubbleSheetFileService.IsGenericGroup(id, classId.GetValueOrDefault())
               && CheckUserCanAccessClass(CurrentUser.Id, CurrentUser.RoleId, id))
            {
                return RedirectToAction("AssignStudentsToTestActSat", "GenericSheet", new { qticket = id, qclassId = classId.GetValueOrDefault() });
            }

            if (CheckUserCanAccessClass(CurrentUser.Id, CurrentUser.RoleId, classId.GetValueOrDefault()) == false)
            {
                return RedirectToAction("Index", "BubbleSheetReview");
            }

            if (bubbleSheetFileService.IsFileGenericWithNoMappings(id, classId.GetValueOrDefault()))
            {
                return RedirectToAction("AssignStudentsToTestActSat", "GenericSheet", new { qticket = id, qclassId = classId.GetValueOrDefault() });
            }

            var model = new BubbleSheetReviewDetailsViewModel
            {
                Ticket = id,
                CanAccess = CanUserAccessBubbleSheet(id),
                ClassId = classId.GetValueOrDefault(),
                HasGenericSheet = bubbleSheetFileService.HasGenericSheets(id),
                TestName = test
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult GetSATBubbleSheetDetails(string ticket, int studentId, int? classId)
        {
            if (CheckUserCanAccessClass(CurrentUser.Id, CurrentUser.RoleId, classId.GetValueOrDefault()) == false)
            {
                return RedirectToAction("Index", "BubbleSheetReview");
            }
            var allStudentDetails =
                bubbleSheetReviewDetailsService.GetAllBubbleSheetReviewDetailsForStudentByTicketAndClassId(studentId,
                    ticket, classId.GetValueOrDefault()).OrderByDescending(x => x.BubbleSheetFileId).ToList();

            if (allStudentDetails.Any() == false)
            {
                //return RedirectToAction("ReviewDetails", ticket);
                return Json(true);
            }

            var timeZoneId = stateService.GetTimeZoneId(CurrentUser.StateId.GetValueOrDefault());
            var zn = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            foreach (var studentDetail in allStudentDetails)
            {
                if (studentDetail.UploadedDate.HasValue)
                    studentDetail.UploadedDate = TimeZoneInfo.ConvertTimeFromUtc(studentDetail.UploadedDate.Value, zn);
            }

            var studentArtifact = allStudentDetails.FirstOrDefault(x => x.PageNumber == -1);
            var studentDetails = allStudentDetails.FirstOrDefault(x => x.PageNumber != -1) ?? studentArtifact;

            if (studentDetails.OutputFileName.Equals("temp"))
            {
                var lastestBubblesheetFileHasImage =
                    allStudentDetails.FirstOrDefault(x => x.OutputFileName != string.Empty && x.OutputFileName != "temp");
                if (lastestBubblesheetFileHasImage != null)
                {
                    studentDetails.OutputFileName = lastestBubblesheetFileHasImage.OutputFileName;
                }
            }

            var model = SATBindStudentDetailsToViewModel(studentDetails);
            model.ArtifactFileName = studentArtifact?.InputFileName;
            if (studentDetails.HasBubbleSheetFile)
            {
                int bubbleSheetFileId = studentDetails.BubbleSheetFileId.HasValue
                    ? studentDetails.BubbleSheetFileId.Value
                    : 0;

                var vSubs = bubbleSheetFileService.GetLastestBubbleSheetFileSubByBubbleSheetId(model.BubbleSheetId);
                if (vSubs.Any())
                {
                    var defaultSubFile = GetDefaultSubFile(vSubs);
                    model.OnlyOnePage = vSubs.Count == 1;
                    var imageUrl = BubbleSheetWsHelper.GetTestImageUrl(defaultSubFile.OutputFileName, ConfigurationManager.AppSettings["ApiKey"]);
                    model.ImageUrl = imageUrl;
                }
                model.PreviousBubbleSheets = GetPreviousBubbleSheetsForStudent(allStudentDetails).ToList();
            }

            GetSATBubbleSheetDisplayLable(model, studentId); // Assign layout custom label s
            if (studentDetails.VirtualTestId.HasValue)
            {
                var virtualTestFile =
                    virtualTestFileService.GetFirstOrDefaultByVirtualTest(studentDetails.VirtualTestId.Value);

                if (virtualTestFile != null)
                {
                    model.VirtualTestFileKey = virtualTestFile.FileKey;
                    model.VirtualTestFileName = Helpers.Util.FormatRubricFileName(virtualTestFile.FileName);
                }
                //Get Unanswered Question
                model.UnansweredQuestions = actAnswerQuestionService.GetUnansweredQuestions(studentDetails.VirtualTestId.Value, studentDetails.BubbleSheetId, studentDetails.StudentId);
                //Get Answered Question
                model.AnsweredQuestions = actAnswerQuestionService.GetAnsweredQuestions(studentDetails.VirtualTestId.Value, studentDetails.BubbleSheetId, studentDetails.StudentId);
                // Get List Section
                model.ListSection = model.UnansweredQuestions.Select(o => o.VirtualSectionId).Distinct().ToList();

                var essayQuestionWithTag =
                    bubbleSheetPrintingService.GetDomainTagForEssaySection(
                        studentDetails.VirtualTestId.GetValueOrDefault());

                foreach (var actUnansweredQuestion in model.UnansweredQuestions)
                {
                    var tagInfo =
                        essayQuestionWithTag.FirstOrDefault(x => x.Index == actUnansweredQuestion.OrderSectionQuestionIndex - 1 && actUnansweredQuestion.OrderSectionIndex == x.SectionID);
                    if (tagInfo != null)
                    {
                        actUnansweredQuestion.DomainTag = tagInfo.TagName;
                    }
                }
                foreach (var actUnansweredQuestion in model.AnsweredQuestions)
                {
                    var tagInfo =
                        essayQuestionWithTag.FirstOrDefault(x => x.Index == actUnansweredQuestion.OrderSectionQuestionIndex - 1 && actUnansweredQuestion.OrderSectionIndex == x.SectionID);
                    if (tagInfo != null)
                    {
                        actUnansweredQuestion.DomainTag = tagInfo.TagName;
                    }
                }

                BuildListSectionID(model);
            }
            SATFillMaxChoiceToUnanswerQuestion(model);
            return PartialView("_SATBubbleSheetDetails", model);
        }

        private SATBubbleSheetStudentDetailsViewModel SATBindStudentDetailsToViewModel(BubbleSheetReviewDetails studentDetails)
        {
            return new SATBubbleSheetStudentDetailsViewModel
            {
                BubbleSheetFileId = studentDetails.BubbleSheetFileId,
                BubbleSheetId = studentDetails.BubbleSheetId,
                StudentId = studentDetails.StudentId,
                ClassId = studentDetails.ClassId,
                Ticket = studentDetails.Ticket,
                StudentName = studentDetails.StudentName,
                TeacherName = studentDetails.TeacherName,
                ClassName = studentDetails.ClassName,
                SchoolName = studentDetails.SchoolName,
                UploadedDate = studentDetails.UploadedDate,
                UploadedBy = studentDetails.UploadedBy,
                FileName = studentDetails.InputFileName,
                HasBubbleSheetFile = studentDetails.HasBubbleSheetFile,
                RosterPosition = studentDetails.RosterPosition,
                IsManualEntry = studentDetails.IsManualEntry,
                ResultDate = studentDetails.ResultDate
            };
        }

        private void GetSATBubbleSheetDisplayLable(SATBubbleSheetStudentDetailsViewModel model, int studentId)
        {
            var student = studentService.GetStudentById(studentId);
            if (student != null)
            {
                var unansweredQuestionLableConfiguration =
                    districtConfigurationService.GetDistrictConfigurationByKey(student.DistrictId, DistrictConfigurationKey.BsUnansweredQuestionLabelKey);
                var answeredQuestionLableConfiguration =
                    districtConfigurationService.GetDistrictConfigurationByKey(student.DistrictId, DistrictConfigurationKey.BsAnsweredQuestionLabelKey);
                var questionLableConfiguration =
                    districtConfigurationService.GetDistrictConfigurationByKey(student.DistrictId, DistrictConfigurationKey.BsQuestionLabelKey);
                var answerChoicesLableConfiguration =
                    districtConfigurationService.GetDistrictConfigurationByKey(student.DistrictId, DistrictConfigurationKey.BsAnswerChoicesLabelKey);

                if (unansweredQuestionLableConfiguration != null)
                    model.UnansweredQuestionLable = unansweredQuestionLableConfiguration.Value;
                if (answeredQuestionLableConfiguration != null)
                    model.AnsweredQuestionLable = answeredQuestionLableConfiguration.Value;
                if (questionLableConfiguration != null)
                    model.QuestionLable = questionLableConfiguration.Value;
                if (answerChoicesLableConfiguration != null)
                    model.AnswerChoicesLable = answerChoicesLableConfiguration.Value;
            }

            //return model;
        }

        private void SATFillMaxChoiceToUnanswerQuestion(SATBubbleSheetStudentDetailsViewModel model)
        {
            List<int> lst = new List<int>();
            if (model.UnansweredQuestions.Count > 0)
            {
                for (int i = 0; i < model.UnansweredQuestions.Count; i++)
                {
                    if (model.UnansweredQuestions[i].QTISchemaId == (int)QtiSchemaEnum.ChoiceMultiple)
                    {
                        var xmlContent = Util.BindQtiItemXmlContentFromXml(model.UnansweredQuestions[i].XmlContent);
                        if (xmlContent != null)
                            model.UnansweredQuestions[i].MaxChoice = xmlContent.MaxChoices;
                    }
                }
            }
            if (model.AnsweredQuestions.Count > 0)
            {
                for (int i = 0; i < model.AnsweredQuestions.Count; i++)
                {
                    if (model.AnsweredQuestions[i].QTISchemaId == (int)QtiSchemaEnum.ChoiceMultiple)
                    {
                        var xmlContent = Util.BindQtiItemXmlContentFromXml(model.AnsweredQuestions[i].XmlContent);
                        if (xmlContent != null)
                            model.AnsweredQuestions[i].MaxChoice = xmlContent.MaxChoices;
                    }
                }
            }
        }

        private void BuildListSectionID(SATBubbleSheetStudentDetailsViewModel model)
        {
            List<int> lstSectionIdsAnswered = model.AnsweredQuestions.Select(o => o.VirtualSectionId).Distinct().ToList();
            List<int> lstSectionIdsUnAnswered = model.UnansweredQuestions.Select(o => o.VirtualSectionId).Distinct().ToList();
            model.ListSection = lstSectionIdsAnswered.Union(lstSectionIdsUnAnswered).ToList();
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult SubmitSATTestQuestions(TestQuestionsViewModel model)
        {
            if (model.BubbleSheetId.Equals(0)) { return HttpNotFound(); }

            var bubbleSheet = bubbleSheetService.GetBubbleSheetById(model.BubbleSheetId);

            if (bubbleSheet == null)
            {
                return HttpNotFound();
            }

            if (CheckUserCanAccessClass(CurrentUser.Id, CurrentUser.RoleId, bubbleSheet.ClassId.GetValueOrDefault()) ==
                false)
            {
                return HttpNotFound();
            }

            if (model.BubbleSheetFileId.Equals(0))
            {
                var newFile = CreateBubbleSheetFileForBlankSheet(model);
                newFile.OutputFileName = "new";
                newFile.InputFileName = "new";
                newFile.InputFilePath = "new";
                bubbleSheetFileService.SaveBubbleSheetFile(newFile);
                model.BubbleSheetFileId = bubbleSheetFileService.GetNewlyCreatedBubbleSheetFileId(newFile);
            }
            //Move this line to after call API, because we dont' want change status if API Error
            //bubbleSheetFileService.SaveBubbleSheetFileCorrection(model.BubbleSheetFileId);

            //var bubbleSheet = bubbleSheetService.GetBubbleSheetById(model.BubbleSheetId);
            bool isNewSAT = false;
            if (bubbleSheet != null)
            {
                var virtualTest = virtualTestService.GetTestById(bubbleSheet.TestId.GetValueOrDefault());
                if (virtualTest != null)
                {
                    isNewSAT = virtualTest.VirtualTestSubTypeID == (int)VirtualTestSubType.NewSAT;
                }
            }

            var bubbleSheetFile = AssignBubbleSheetFile(model);
            bubbleSheetFile.UserId = CurrentUser.Id;

            //split submitted questions to 2 page (cover page and bubblesheet page)
            var bubbleSheetFileSub =
                bubbleSheetFileService.GetBubbleSheetFileSubs(model.BubbleSheetFileId)
                    .Where(x => x.PageNumber == 1 || x.PageNumber == 4)
                    .ToList();

            var finalAnswerList = ACTGetCombinedTestQuestions(model).ToList();

            var coverPageAnswerList = finalAnswerList.Where(x => x.SectionIndex == 0)
                .OrderBy(x => x.SectionIndex).ThenBy(x => x.SectionQuestionIndex).ToList();
            var bubbleSheetPageAnswerList = finalAnswerList.Where(x => x.SectionIndex > 0).ToList();

            try
            {
                var bubbleSheetFileCloudEntity = GetBubbleSheetFileAwsEntity(model, bubbleSheetFile);
                if (bubbleSheetFileCloudEntity == null)
                {
                    return Json(false);
                }
                bubbleSheetFileCloudEntity.TestType = isNewSAT ? 6 : 4;
                bubbleSheetFileCloudEntity.PageNumber = 4;
                bubbleSheetFileCloudEntity.ACTPageIndex = 4;
                bubbleSheetFileCloudEntity.PageType = 2;
                bubbleSheetFileCloudEntity.RosterPosition = "0";
                if (bubbleSheetFileSub.Any(x => x.PageNumber == 4))
                {
                    var sub = bubbleSheetFileSub.First(x => x.PageNumber == 4);
                    bubbleSheetFileCloudEntity.InputPath = sub.InputFilePath;
                    bubbleSheetFileCloudEntity.OutputFile = sub.OutputFileName;
                    bubbleSheetFileCloudEntity.InputFileName = sub.InputFileName;
                }
                testResubmissionService.SATAssignNewQuestions(bubbleSheetPageAnswerList, bubbleSheetFileCloudEntity);

                //TODO: Create CoverPage without Quesiton Essay
                var bubbleSheetFileForCoverPage = GetBubbleSheetFileAwsEntity(model, bubbleSheetFile);
                if (bubbleSheetFileForCoverPage == null)
                {
                    return Json(false);
                }
                bubbleSheetFileForCoverPage.TestType = isNewSAT ? 6 : 4; //SAT
                bubbleSheetFileForCoverPage.ACTPageIndex = 1;
                bubbleSheetFileForCoverPage.PageType = 1;
                bubbleSheetFileForCoverPage.RosterPosition = "0";
                bubbleSheetFileForCoverPage.PageNumber = 1;
                if (bubbleSheetFileSub.Any(x => x.PageNumber == 1))
                {
                    var sub = bubbleSheetFileSub.First(x => x.PageNumber == 1);
                    bubbleSheetFileForCoverPage.InputPath = sub.InputFilePath;
                    bubbleSheetFileForCoverPage.OutputFile = sub.OutputFileName;
                    bubbleSheetFileForCoverPage.InputFileName = sub.InputFileName;
                }
                testResubmissionService.SATAssignNewQuestions(coverPageAnswerList, bubbleSheetFileForCoverPage);

                if (!bubbleSheetFile.BubbleSheetId.ToString().Equals(bubbleSheetFileForCoverPage.Barcode1))
                {
                    bubbleSheetFileCloudEntity.Barcode1 = model.BubbleSheetId.ToString();
                    bubbleSheetFileForCoverPage.Barcode1 = model.BubbleSheetId.ToString();
                }

                var response = BubbleSheetWsHelper.SendGradeRequestBatch(new List<ReadResult>
                                                                         {
                                                                             bubbleSheetFileCloudEntity,
                                                                             bubbleSheetFileForCoverPage
                                                                         });
                if (response == null || response.IsSuccess == false)
                {
                    return Json(false);
                }
                //Only Change Status when submit Success
                bubbleSheetFileService.SaveBubbleSheetFileCorrection(model.BubbleSheetFileId);
                return Json(true);
            }
            catch (GetRequestTimeoutException e)
            {
                return Json(false);
            }
        }

        public ActionResult GetBubbleSheetFileSubNormalByBubbleSheetID(int Id, int studentID)
        {
            var lst = new BubbleSheetFileSubListViewModel();
            var vSubs = bubbleSheetFileService.GetLastestBubbleSheetFileById(Id, studentID);
            var orderedSubs = vSubs.OrderBy(x => x.PageNumber >= 0 ? 0 : 1)
                                   .ThenBy(x => x.PageNumber)
                                   .ToList();

            if (orderedSubs.Count > 0)
            {
                foreach (BubbleSheetFileSub bubbleSheetFileSub in orderedSubs)
                {
                    lst.ListFileSubViewModels.Add(new BubbleSheetFileSubViewModel()
                    {
                        ImageUrl = BubbleSheetWsHelper.GetTestImageUrl(bubbleSheetFileSub.OutputFileName, ConfigurationManager.AppSettings["ApiKey"]),
                        SubFileName = bubbleSheetFileSub.InputFileName
                    });
                }
                return PartialView("_DisplaySubFile", lst);
            }
            var bubblesheetFile = bubbleSheetFileService.GetLastestBubbleSheetFileByBubbleSheetId(Id);
            if (bubblesheetFile != null)
            {
                lst.ListFileSubViewModels.Add(new BubbleSheetFileSubViewModel()
                {
                    ImageUrl = BubbleSheetWsHelper.GetTestImageUrl(bubblesheetFile.OutputFileName, ConfigurationManager.AppSettings["ApiKey"]),
                    SubFileName = bubblesheetFile.InputFileName
                });
            }
            else
            {
                lst.ListFileSubViewModels.Add(new BubbleSheetFileSubViewModel()
                {
                    ImageUrl = Url.Content("~/Content/images/noimage.png"),
                    SubFileName = string.Empty
                });
            }
            return PartialView("_DisplaySubFile", lst);
        }

        [HttpGet]
        public ActionResult GetBubbleSheetFileByBubbleSheetId(int bubbleSheetId, int studentId)
        {
            var lst = new BubbleSheetFileSubListViewModel();
            var vSubs = bubbleSheetFileService.GetLastestBubbleSheetFileById(bubbleSheetId, studentId)
                .OrderBy(x => x.PageNumber >= 0 ? 0 : 1)
                .ThenBy(o => o.PageNumber).ToList();

            if (vSubs.Count > 0)
            {
                foreach (BubbleSheetFileSub bubbleSheetFileSub in vSubs)
                {
                    lst.ListFileSubViewModels.Add(new BubbleSheetFileSubViewModel()
                    {
                        ImageUrl = BubbleSheetWsHelper.GetTestImageUrl(bubbleSheetFileSub.OutputFileName, ConfigurationManager.AppSettings["ApiKey"]),
                        SubFileName = bubbleSheetFileSub.InputFileName
                    });
                }
                return Json(lst, JsonRequestBehavior.AllowGet);
            }
            var bubblesheetFile = bubbleSheetFileService.GetLastestBubbleSheetFileByBubbleSheetId(bubbleSheetId);
            if (bubblesheetFile != null)
            {
                lst.ListFileSubViewModels.Add(new BubbleSheetFileSubViewModel()
                {
                    ImageUrl = BubbleSheetWsHelper.GetTestImageUrl(bubblesheetFile.OutputFileName, ConfigurationManager.AppSettings["ApiKey"]),
                    SubFileName = bubblesheetFile.InputFileName
                });
            }
            else
            {
                lst.ListFileSubViewModels.Add(new BubbleSheetFileSubViewModel()
                {
                    ImageUrl = Url.Content("~/Content/images/noimage.png"),
                    SubFileName = string.Empty
                });
            }
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        private bool IsNewACTTest(int bubbleSheetID)
        {
            bool result = false;

            var bubbleSheet = bubbleSheetService.GetBubbleSheetById(bubbleSheetID);
            if (bubbleSheet != null)
            {
                var virtualTest = virtualTestService.GetTestById(bubbleSheet.TestId.GetValueOrDefault());
                if (virtualTest != null && virtualTest.VirtualTestSubTypeID.HasValue)
                {
                    result = virtualTest.VirtualTestSubTypeID.Value == (int)VirtualTestSubType.NewACT;
                }
            }

            return result;
        }

        private bool CheckOnlyOneBubbleSheetFile(int id, int studentID, out string strImg)
        {
            strImg = string.Empty;

            var vSubs = bubbleSheetFileService.GetLastestBubbleSheetFileById(id, studentID);
            if (vSubs.Count > 0) //have multiple files
            {
                vSubs = vSubs.OrderBy(x => x.PageNumber >= 0 ? 0 : 1).ThenBy(x => x.PageNumber).ToList();
                strImg = BubbleSheetWsHelper.GetTestImageUrl(vSubs.First().OutputFileName,
                    ConfigurationManager.AppSettings["ApiKey"]);

                return vSubs.Count == 1;
            }
            var bubblesheetFile = bubbleSheetFileService.GetLastestBubbleSheetFileByBubbleSheetId(id);
            if (bubblesheetFile != null)
            {
                strImg = BubbleSheetWsHelper.GetTestImageUrl(bubblesheetFile.OutputFileName,
                    ConfigurationManager.AppSettings["ApiKey"]);
                return true;
            }
            return false;
        }

        private bool CheckOnlyOneBubbleSheetFileByBubbleSheetFileId(int Id, out string strImg)
        {
            strImg = string.Empty;
            var vSubs = bubbleSheetFileService.GetBubbleSheetFileSubs(Id).OrderBy(x => x.PageNumber >= 0 ? 0 : 1).ThenBy(x => x.PageNumber).ToList();
            if (vSubs.Count > 0)
            {
                strImg = BubbleSheetWsHelper.GetTestImageUrl(vSubs.First().OutputFileName,
                    ConfigurationManager.AppSettings["ApiKey"]);

                return vSubs.Count == 1;
            }
            var bubblesheetFile = bubbleSheetFileService.GetBubbleSheetFileById(Id);
            if (bubblesheetFile != null)
            {
                strImg = BubbleSheetWsHelper.GetTestImageUrl(bubblesheetFile.OutputFileName,
                     ConfigurationManager.AppSettings["ApiKey"]);
                return true;
            }
            return false;
        }
    }
}
