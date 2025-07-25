using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Models.BubbleSheetReview;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.BubbleSheetAws;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using Newtonsoft.Json;
using LinkIt.BubbleSheetPortal.Common.Enum;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize]
    [VersionFilter]
    public class BubbleSheetClassViewController : ReviewBubbleSheetBaseController
    {
        private readonly BubbleSheetReviewDetailsService bubbleSheetReviewDetailsService;
        private readonly BubbleSheetStudentResultsService bubbleSheetStudentResultsService;
        private readonly BubbleSheetFileService bubbleSheetFileService;
        private readonly BubbleSheetService bubbleSheetService;
        private readonly TestResubmissionService testResubmissionService;
        private readonly SchoolService schoolService;
        private readonly StudentRosterService studentRosterService;
        private readonly ReadResultService readResultService;
        private readonly ConfigurationService configurationService;
        private readonly VirtualQuestionService virtualQuestionService;
        private readonly UnansweredQuestionService unansweredQuestionService;

        public BubbleSheetClassViewController(
            BubbleSheetReviewDetailsService bubbleSheetReviewDetailsService,
            BubbleSheetListService bubbleSheetListService,
            BubbleSheetService bubbleSheetService,
            BubbleSheetFileService bubbleSheetFileService,
            BubbleSheetStudentResultsService bubbleSheetStudentResultsService,
            UserService userService,
            VulnerabilityService vulnerabilityService,
            DistrictDecodeService districtDecodeService,
            TestResubmissionService testResubmissionService,
            SchoolService schoolService,
            StudentRosterService studentRosterService,
            ReadResultService readResultService,
            ConfigurationService configurationService,
            VirtualQuestionService virtualQuestionService,
            UnansweredQuestionService unansweredQuestionService)
            : base(bubbleSheetListService, bubbleSheetService, vulnerabilityService, userService, districtDecodeService)
        {
            this.bubbleSheetReviewDetailsService = bubbleSheetReviewDetailsService;
            this.bubbleSheetStudentResultsService = bubbleSheetStudentResultsService;
            this.bubbleSheetFileService = bubbleSheetFileService;
            this.bubbleSheetService = bubbleSheetService;
            this.testResubmissionService = testResubmissionService;
            this.schoolService = schoolService;
            this.studentRosterService = studentRosterService;
            this.readResultService = readResultService;
            this.configurationService = configurationService;
            this.virtualQuestionService = virtualQuestionService;
            this.unansweredQuestionService = unansweredQuestionService;
        }

        [HttpGet]
        [UrlReturnDecode]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.ManagebubblesheetsReview)]
        public ActionResult ClassViewPage(string id, int? classId, string test)
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

            var model = new BubbleSheetClassViewViewModel()
            {
                Ticket = id,
                ClassId = classId ?? 0,
                TestName = test
            };

            //get autoSaved data
            var autoSavedData = bubbleSheetReviewDetailsService.GetAutoSavedData(id, classId.GetValueOrDefault(),
                CurrentUser.Id);
            if (autoSavedData != null && !string.IsNullOrEmpty(autoSavedData.Data))
            {
                model.BubbleSheetStudentDatas =
                    JsonConvert.DeserializeObject<List<BubbleSheetClassViewStudentViewModel>>(autoSavedData.Data);
                if (!string.IsNullOrEmpty(autoSavedData.ActualData))
                {
                    var actualData = JsonConvert.DeserializeObject<BubbleSheetClassViewViewModel>(autoSavedData.ActualData);
                    ViewBag.ActualData = BBSClassViewSplitModel.Split(actualData);
                }
            }
            else
            {
                model.BubbleSheetStudentDatas = GetStudentAnswersByTicketAndClass(id, classId, totalPage).OrderBy(x => x.Name).ToList();
            }
            ViewBag.IntervalAutoSave =
                configurationService.GetConfigurationByKeyWithDefaultValue(Util.BubbleSheetIntervalAutoSave, "300000");

            var splitModel = BBSClassViewSplitModel.Split(model);
            return View(splitModel);
        }

        [HttpPost]
        public ActionResult AutoSaved(string ticket, int? classId, string bubbleSheetStudentDatas, string actualData)
        {
            if (!classId.HasValue)
            {
                return Json(new { IsSuccess = false, Error = "Please input classid." }, JsonRequestBehavior.AllowGet);
            }

            var obj = new BubbleSheetClassViewAutoSave()
            {
                Ticket = ticket,
                ClassId = classId ?? 0,
                Data = bubbleSheetStudentDatas,
                ActualData = actualData,
                UserId = CurrentUser.Id
            };
            try
            {
                bubbleSheetReviewDetailsService.AutoSavedClassView(obj);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { IsSuccess = false, Error = "Can not save data." }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { IsSuccess = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteAutoSavedData(string ticket, int? classId)
        {
            if (!classId.HasValue)
            {
                return Json(new { IsSuccess = false, Error = "Please input classid." }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                DeleteAutoSaveData(ticket, classId);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { IsSuccess = false, Error = "Can not delete data." }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { IsSuccess = true }, JsonRequestBehavior.AllowGet);
        }

        private void DeleteAutoSaveData(string ticket, int? classId)
        {
            var obj = new BubbleSheetClassViewAutoSave()
            {
                Ticket = ticket,
                ClassId = classId ?? 0,
                UserId = CurrentUser.Id
            };
            bubbleSheetReviewDetailsService.DeleteAutoSavedData(obj);
        }

        [HttpGet]
        public ActionResult GetBubbleSheetFileByBubbleSheetId(int bubbleSheetId, int studentId)
        {
            var lst = new BubbleSheetFileSubListViewModel();
            var vSubs = bubbleSheetFileService.GetLastestBubbleSheetFileById(bubbleSheetId, studentId)
                .OrderBy(o => o.PageNumber).ToList();

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
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SubmitTestQuestions(BubbleSheetClassViewViewModel model)
        {
            if (CheckUserCanAccessClass(CurrentUser.Id, CurrentUser.RoleId, model.ClassId) == false)
            {
                return HttpNotFound();
            }
            try
            {
                // valid bubblesheet
                var bbsIds = model.BubbleSheetStudentDatas.Select(m => m.BubbleSheetId).ToList();
                var isValid = bubbleSheetFileService.ValidListBubblesheetIDs(bbsIds);
                if (isValid == false)
                {
                    return HttpNotFound();
                }

                var districtID = 0;
                if (model.BubbleSheetStudentDatas.Count > 0)
                {
                    districtID = schoolService.GetSchoolById(bubbleSheetService.GetBubbleSheetById(model.BubbleSheetStudentDatas[0].BubbleSheetId)
                        .SchoolId.GetValueOrDefault()).DistrictId;
                }

                var bubbleSheetFileCorrections = new List<int>();
                var bubbleSheetFileCloudEntities = new List<ReadResult>();

                foreach (var item in model.BubbleSheetStudentDatas)
                {
                    var testQuestionModel = new TestQuestionsViewModel()
                    {
                        BubbleSheetId = item.BubbleSheetId,
                        RosterPosition = item.RosterPosition,
                        BubbleSheetFileId = item.BubbleSheetFileViewModel.BubbleSheetFileId,
                        StudentId = item.StudentId,
                        ClassId = model.ClassId,
                        DistrictID = districtID
                    };

                    BubbleSheetFile bubbleSheetFile = null;
                    if (item.BubbleSheetFileViewModel.BubbleSheetFileId.Equals(0))
                    {
                        bubbleSheetFile = CreateBubbleSheetFileForBlankSheet(testQuestionModel);
                        bubbleSheetFile.OutputFileName = "new";
                        bubbleSheetFile.InputFileName = "new";
                        bubbleSheetFile.InputFilePath = "new";
                        bubbleSheetFileService.SaveBubbleSheetFile(bubbleSheetFile);
                        //item.BubbleSheetFileViewModel.BubbleSheetFileId = bubbleSheetFileService.GetNewlyCreatedBubbleSheetFileId(bubbleSheetFile);
                        //testQuestionModel.BubbleSheetFileId = item.BubbleSheetFileViewModel.BubbleSheetFileId;
                        item.BubbleSheetFileViewModel.BubbleSheetFileId = bubbleSheetFile.BubbleSheetFileId;
                        testQuestionModel.BubbleSheetFileId = bubbleSheetFile.BubbleSheetFileId;
                    }

                    if (bubbleSheetFile == null)
                    {
                        bubbleSheetFile = AssignBubbleSheetFile(testQuestionModel);
                    }

                    var bubbleSheetFileCloudEntity = GetBubbleSheetFileAwsEntity(bubbleSheetFile, item.BubbleSheetAnswers.Count);

                    var answerModel = item.BubbleSheetAnswers.Select(x => new UnansweredQuestionAnswer()
                    {
                        SelectedAnswer = x.AnswerLetter,
                        QuestionOrder = x.QuestionOrder
                    });

                    testResubmissionService.AssignNewQuestions(answerModel, bubbleSheetFileCloudEntity);
                    bubbleSheetFileCloudEntities.Add(bubbleSheetFileCloudEntity);
                    //Only Change Status when submit Success
                    bubbleSheetFileCorrections.Add(item.BubbleSheetFileViewModel.BubbleSheetFileId);
                }

                var response = BubbleSheetWsHelper.SendListGradeRequest(bubbleSheetFileCloudEntities);
                if (response == null || response.IsSuccess == false)
                {
                    return Json(false);
                }

                bubbleSheetFileService.SaveBubbleSheetFileCorrections(bubbleSheetFileCorrections);
                //delete all autosave draft data
                bubbleSheetReviewDetailsService.DeleteAllAutoSavedData(model.Ticket, model.ClassId);
                return Json(true);
            }
            catch (GetRequestTimeoutException e)
            {
                return Json(false);
            }
        }

        [HttpPost]
        public ActionResult MarkStudentAsAbsent(List<TestQuestionsViewModel> model)
        {
            try
            {
                foreach (var student in model)
                {
                    var newFile = CreateBubbleSheetFileForBlankSheet(student);
                    newFile.IsConfirmed = true;
                    bubbleSheetFileService.SaveBubbleSheetFile(newFile);
                }
                return Json(true);
            }
            catch
            {
                return Json(false);
            }
        }

        [HttpPost]
        public ActionResult GetStudentsRefreshDetail(string id, int? classId)
        {
            var data = bubbleSheetStudentResultsService.GetStudentsOfBubbleSheetByTicketWithStatusInBatch(id, classId.GetValueOrDefault());
            var bbsfile = bubbleSheetFileService.GetBubbleSheetLatestFile(id, classId.GetValueOrDefault());

            var totalPage = BubbleSheetWsHelper.GetTotalPageOfTestForOneStudent(id,
                    ConfigurationManager.AppSettings["ApiKey"]);
            var result = data.Select(x => new BubbleSheetClassViewStudentViewModel
            {
                Name = x.StudentName,
                Status = x.Status,
                Graded = BubbleSheetWsHelper.GetGradedCountForStudent(x, totalPage),
                PointsEarned = x.PointsEarned + "/" + x.PointsPossible,
                StudentId = x.StudentId,
                BubbleSheetId = x.BubbleSheetId,
                ArtifactFileName = x.ArtifactFileName,
                BubbleSheetFileViewModel = GetBubbleSheetFile(bbsfile, x.StudentId)
            }).OrderBy(x => x.Name).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetImageUrl(List<BubbleSheetFileSubViewModel> files)
        {
            if (files != null && files.Count > 0)
            {
                foreach (var item in files)
                {
                    item.ImageUrl = BubbleSheetWsHelper.GetTestImageUrl(item.SubFileName, ConfigurationManager.AppSettings["ApiKey"]);
                }
            }
            return Json(new { IsSuccess = true, Data = files }, JsonRequestBehavior.AllowGet);
        }

        private BubbleSheetFileViewModel GetBubbleSheetFile(List<BubbleSheetFileSub> bbsfile, int studentId)
        {
            var model = new BubbleSheetFileViewModel();
            var files = bbsfile.Where(m => m.StudentID == studentId).ToList();
            if (files.Count > 0)
            {
                model.BubbleSheetFileId = files[0].BubbleSheetFileId;
                model.ListFileSubViewModels = files.OrderBy(x => x.PageNumber >= 0 ? 0 : 1).ThenBy(x => x.PageNumber).ToList().Select(m => new BubbleSheetFileSubViewModel
                {
                    SubFileName = m.OutputFileName,
                    PageType = m.PageType
                }).ToList();
            }
            return model;
        }

        private List<BubbleSheetClassViewStudentViewModel> GetStudentAnswersByTicketAndClass(string id, int? classId, int totalPage)
        {           
            var data = bubbleSheetStudentResultsService.GetStudentsOfBubbleSheetByTicketWithStatusInBatch(id, classId.GetValueOrDefault());
            var bbsfile = bubbleSheetFileService.GetBubbleSheetLatestFile(id, classId.GetValueOrDefault());
            var studentIds = data.Select(x => x.StudentId).Distinct().ToList();
            var studentIdStr = string.Join(";", studentIds);

            var answers = FillMaxChoiceToAnswer(id, classId, studentIdStr);

            var result = data.Select(x => new BubbleSheetClassViewStudentViewModel
            {
                Name = x.StudentName,
                Status = x.Status,
                Graded = BubbleSheetWsHelper.GetGradedCountForStudent(x, totalPage),
                PointsEarned = x.PointsEarned + "/" + x.PointsPossible,
                StudentId = x.StudentId,
                BubbleSheetId = x.BubbleSheetId,
                RosterPosition = x.RosterPosition,
                ArtifactFileName = x.ArtifactFileName,
                BubbleSheetFileViewModel = GetBubbleSheetFile(bbsfile, x.StudentId),
                BubbleSheetAnswers = answers.Where(o => o.StudentId == x.StudentId).OrderBy(o => o.QuestionOrder).ToList()
            }).ToList();

            return result;
        }

        private List<BubbleSheetClassViewAnswer> FillMaxChoiceToAnswer(string id, int? classId, string studentIdStr)
        {
            var answers =
                bubbleSheetReviewDetailsService.GetBubbleSheetClassViewAnswerData(studentIdStr, id,
                    classId.GetValueOrDefault());

            var bbs = bubbleSheetService.GetBubbleSheetsByTicketAndClass(id, classId.GetValueOrDefault()).FirstOrDefault();
            var virtualTestID = bbs != null ? bbs.TestId.GetValueOrDefault() : 0;
            var qtiitems = virtualQuestionService.GetXMLContent(virtualTestID);

            var questions = answers.Where(
                x =>
                    x.QTISchemaId == (int)QtiSchemaEnum.ChoiceMultipleVariable ||
                    x.QTISchemaId == (int)QtiSchemaEnum.ChoiceMultiple)
                .Select(
                    x =>
                        new QuestionClassView
                        {
                            VirtualQuestionId = x.VirtualQuestionId,
                            XmlContent = qtiitems.FirstOrDefault(m => m.VirtualQuestionID == x.VirtualQuestionId).XMLContent,
                            CorrectAnswer = x.CorrectAnswer
                        }).DistinctBy(o => o.VirtualQuestionId).ToList();

            foreach (var question in questions)
            {
                var xmlContent = Util.BindQtiItemXmlContentFromXml(question.XmlContent);
                if (xmlContent != null)
                    question.MaxChoice = xmlContent.MaxChoices;

                // Get top greatest score answer only
                if (question.MaxChoice > 0)
                {
                    var correctLetters = question.CorrectAnswer.Split(',').ToList();
                    if (correctLetters.Count() > question.MaxChoice)
                    {
                        question.CorrectAnswer = string.Join(",", correctLetters.Take(question.MaxChoice));
                    }
                }
            }

            var finalAnswers =
                from a in answers
                join q in questions on a.VirtualQuestionId equals q.VirtualQuestionId into aq
                from q in aq.DefaultIfEmpty()
                select new BubbleSheetClassViewAnswer()
                {
                    BubbleSheetId = a.BubbleSheetId,
                    StudentId = a.StudentId,
                    VirtualQuestionId = a.VirtualQuestionId,
                    AnswerIdentifiers = a.AnswerIdentifiers,
                    CorrectAnswer = q != null ? q.CorrectAnswer : a.CorrectAnswer,
                    PointsPossible = a.PointsPossible,
                    QTISchemaId = a.QTISchemaId,
                    QuestionOrder = a.QuestionOrder,
                    Status = a.Status,
                    AnswerLetter = a.AnswerLetter,
                    WasAnswered = a.WasAnswered,
                    MaxChoice = q != null ? q.MaxChoice : 0
                };
            return finalAnswers.ToList();
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
                DistrictId = model.DistrictID > 0 ? model.DistrictID : schoolService.GetSchoolById(bubbleSheetService.GetBubbleSheetById(model.BubbleSheetId).SchoolId.GetValueOrDefault()).DistrictId
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

        private ReadResult GetBubbleSheetFileAwsEntity(BubbleSheetFile bubbleSheetFile, int questionCount)
        {
            ReadResult bubbleSheetFileCloudEntity;
            try
            {
                bubbleSheetFileCloudEntity = bubbleSheetService.GetBubbleSheetProcessingReadResult(bubbleSheetFile.InputFilePath,
                bubbleSheetFile.UrlSafeOutputFileName);
            }
            catch (GetRequestTimeoutException e)
            {
                throw e;
            }

            if (bubbleSheetFileCloudEntity.IsNull())
            {
                bubbleSheetFileCloudEntity = CreateNewBubbleSheetFileReadResultForMissingSheets(bubbleSheetFile, questionCount);
            }
            else
            {
                bubbleSheetFileCloudEntity.InputPath = bubbleSheetFile.InputFilePath;
                bubbleSheetFileCloudEntity.InputFileName = bubbleSheetFile.InputFileName;
                bubbleSheetFileCloudEntity.OutputFile = bubbleSheetFile.OutputFileName;
            }

            return bubbleSheetFileCloudEntity;
        }

        private ReadResult CreateNewBubbleSheetFileReadResultForMissingSheets(BubbleSheetFile bubbleSheetFile, int questionCount)
        {
            var bubbleSheetFileCloudEntity = readResultService.CreateNewBubbleSheetFileReadResult(bubbleSheetFile);
            readResultService.SetBubbleSheetReadResultQuestions(bubbleSheetFileCloudEntity, questionCount);
            return bubbleSheetFileCloudEntity;
        }
    }
}
