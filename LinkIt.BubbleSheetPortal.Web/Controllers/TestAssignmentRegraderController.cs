using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Common.DataFileUpload;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs;
using LinkIt.BubbleSheetPortal.Models.DTOs.TeacherReview;
using LinkIt.BubbleSheetPortal.Models.Enum;
using LinkIt.BubbleSheetPortal.Models.Enums;
using LinkIt.BubbleSheetPortal.Models.GradingShorcuts;
using LinkIt.BubbleSheetPortal.Models.Isolating;
using LinkIt.BubbleSheetPortal.Models.RestrictionDTO;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.Algorithmic;
using LinkIt.BubbleSheetPortal.Web.Helpers.Media;
using LinkIt.BubbleSheetPortal.Web.Models;
using LinkIt.BubbleSheetPortal.Web.Models.CustomModelBinder;
using LinkIt.BubbleSheetPortal.Web.Models.TestAssignmentRegrader;
using LinkIt.BubbleSheetPortal.Web.Print;
using LinkIt.BubbleSheetPortal.Web.Security;
using Newtonsoft.Json;
using S3Library;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize]
    public class TestAssignmentRegraderController : BaseController
    {
        private const int AutoGraderStatusNotProcess = 0;

        private readonly TestAssignmentControllerParameters _parameters;
        private readonly IAnswerAttachmentService _answerAttachmentService;
        private readonly IS3Service _s3Service;

        public TestAssignmentRegraderController(
            TestAssignmentControllerParameters parameters,
            IAnswerAttachmentService answerAttachmentService, IS3Service s3Service)
        {
            _parameters = parameters;
            _answerAttachmentService = answerAttachmentService;
            _s3Service = s3Service;
        }

        [HttpGet]
        public ActionResult Index(int? qtiTestClassAssignmentID, int? qtiTestStudentAssignmentID, int? virtualTestID,
            int? selectFirstStudentForReview)
        {
            if (qtiTestClassAssignmentID != null &&
                !_parameters.VulnerabilityService.HasRigtToEditQtiTestClassAssignment(CurrentUser,
                    qtiTestClassAssignmentID.Value))
            {
                return RedirectToAction("Index");
            }

            var model = new TeacherReviewerIndexModel
            {
                QTITestClassAssignmentID = qtiTestClassAssignmentID,
                QTITestStudentAssignmentID = qtiTestStudentAssignmentID,
            };
            ViewBag.SelectFirstStudentForReview = selectFirstStudentForReview.HasValue &&
                                                  selectFirstStudentForReview.Value == 1;
            VirtualTestData virtualTest = null;
            if (virtualTestID.HasValue)
            {
                model.VirtualTestID = virtualTestID.Value;
                virtualTest = _parameters.QTITestClassAssignmentServices.GetVirtualTestByID(virtualTestID.Value);
                if (virtualTest != null)
                {
                    model.VirtualTestName = virtualTest.Name;
                    model.VirtualTestSubtypeID = virtualTest.VirtualTestSubTypeID;
                    model.DataSetCategoryID = virtualTest.DatasetCategoryID;
                }
            }

            if (qtiTestClassAssignmentID != null)
            {
                var qtiTestClassAssignment =
                    _parameters.QTITestClassAssignmentServices.GetQtiTestClassAssignment(qtiTestClassAssignmentID.Value);
                model.TutorialMode = qtiTestClassAssignment == null ? 1 : qtiTestClassAssignment.TutorialMode;
            }

            model.TestScoreMethodID = virtualTest == null || virtualTest.TestScoreMethodID == null
                ? 1
                : virtualTest.TestScoreMethodID;

            return PartialView("Index", model);
        }

        [HttpGet]
        public ActionResult GetStudentsForAssignment(int? qtiTestClassAssignmentID, int? qtiTestStudentAssignmentID, int? studentId)
        {
            if (qtiTestClassAssignmentID != null &&
                !_parameters.VulnerabilityService.HasRigtToEditQtiTestClassAssignment(CurrentUser,
                    qtiTestClassAssignmentID.Value))
            {
                return Json(new { success = false, message = "Has no right", },
                    JsonRequestBehavior.AllowGet);
            }

            var studentAssignments = _parameters.QTITestClassAssignmentServices.GetTestStudentAssignments(qtiTestClassAssignmentID, null);
            var assignmentNotCompleted = studentAssignments.Any(x => !x.QTIOnlineTestSessionID.HasValue || x.QTIOnlineStatusID != 4);

            if (qtiTestStudentAssignmentID.HasValue)
                studentAssignments = studentAssignments.Where(o => o.QTITestStudentAssignmentID == qtiTestStudentAssignmentID);

            if (studentId.HasValue)
                studentAssignments = studentAssignments.Where(o => o.StudentID == studentId);
            
            var result = studentAssignments.Select((o, index) => new StudentAssignment
            {
                QTIOnlineTestSessionID = o.QTIOnlineTestSessionID,
                StudentName = o.StudentName,
                RealStudentName = o.StudentName,
                StudentID = o.StudentID,
                QTOStatusID = o.QTIOnlineStatusID,
                TestName = o.TestName,
                TimeOver = o.TimeOver,
                StartDate = o.StartDate.HasValue ? o.StartDate.ToString() : string.Empty,
                CanBulkGrading = o.CanBulkGrading.HasValue && o.CanBulkGrading.Value,
                StudentOrder = index
            }).ToList();

            var isAnonymousStudent = false;
            if (assignmentNotCompleted)
            {
                //check test has has manually questions or not
                var isTestHasManuallyQuestion = _parameters.QTITestClassAssignmentServices.IsTestHasManuallyQuestion(qtiTestClassAssignmentID ?? 0);
                if (isTestHasManuallyQuestion)
                {
                    isAnonymousStudent = _parameters.PreferencesServices.IsAnonymizedScoring(qtiTestClassAssignmentID ?? 0);
                    if (isAnonymousStudent)
                    {
                        result.ForEach(x => x.StudentName = Anonymous.GetAnonFullName(x.StudentID));
                        result = result.OrderBy(x => x.StudentName).ToList();
                    }
                }
            }
            return Json(new { IsAnonymizedScoring = isAnonymousStudent, Result = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [UrlReturnDecode]
        public ActionResult GetQuestionsForAssignment(int virtualTestID, int qtiTestClassAssignmentID, string moduleCode = "", int districtId = 0)
        {
            if (
                !_parameters.VulnerabilityService.HasRigtToEditQtiTestClassAssignment(CurrentUser,
                    qtiTestClassAssignmentID))
            {
                return Json(new { error = "Has no right" }, JsonRequestBehavior.AllowGet);
            }
            var virtualQuestions = _parameters.QTITestClassAssignmentServices.GetQTIVirtualTest(virtualTestID).Where(x => x.VirtualSectionMode == VirtualSectionModeConstant.Normal);
            var virtualTest = _parameters.VirtualTestService.GetTestById(virtualTestID);

            var isRestrictedManualGrade = false;
            // apply restriction manual grade
            if (!string.IsNullOrEmpty(moduleCode) && virtualTest != null)
            {
                if (districtId == 0)
                {
                    districtId = CurrentUser.DistrictId.GetValueOrDefault();
                }
                var restrictionList = _parameters.RestrictionBO.GetRestrictionList(moduleCode, CurrentUser.Id, CurrentUser.RoleId, PublishLevelTypeEnum.District, districtId)
                                                .Where(m => (m.RestrictionObjectType == RestrictionObjectType.Test && m.RestrictionObjectId == virtualTestID)
                                                        || (m.RestrictionObjectType == RestrictionObjectType.Bank && m.RestrictionObjectId == virtualTest.BankID));

                if (restrictionList.Any())
                {
                    isRestrictedManualGrade = true;
                }
            }

            var result = new List<QuestionAssignment>();
            var findVirtualQuestionIdsOfRubricBase = virtualQuestions.Where(x => x.IsRubricBasedQuestion == true).Select(x => x.VirtualQuestionID).ToArray();
            var rubricQuestionCategories = new List<RubricQuestionCategoryDto>();
            if (findVirtualQuestionIdsOfRubricBase?.Length > 0)
            {
                rubricQuestionCategories = _parameters.RubricQuestionCategoryService.GetRubicQuestionCategoriesByVirtualQuestionIds(findVirtualQuestionIdsOfRubricBase).ToList();
            }
            foreach (var virtuaQuestion in virtualQuestions)
            {
                var question = new QuestionAssignment
                {
                    QTIItemID = virtuaQuestion.QTIItemID,
                    VirtualQuestionID = virtuaQuestion.VirtualQuestionID,
                    QuestionOrder = virtuaQuestion.QuestionOrder,
                    QTIItemSchemaID = virtuaQuestion.QTIItemSchemaID,
                    PointsPossible = virtuaQuestion.PointsPossible,
                    SectionInstruction = DisplaySectionInstruction(virtuaQuestion),
                    XmlContent = ReplaceWeirdCharacters(virtuaQuestion.XmlContent),
                    BaseVirtualQuestionID = virtuaQuestion.BaseVirtualQuestionID,
                    CorrectAnswer = virtuaQuestion.CorrectAnswer,
                    ResponseProcessing = virtuaQuestion.ResponseProcessing,
                    ResponseProcessingTypeID = virtuaQuestion.ResponseProcessingTypeID,
                    IsRestrictedManualGrade = isRestrictedManualGrade,
                    IsRubricBasedQuestion = virtuaQuestion.IsRubricBasedQuestion,
                };

                question.XmlContent = Util.ReplaceVideoTag(question.XmlContent);
                {
                    question.XmlContent = Util.UpdateS3LinkForItemMedia(question.XmlContent);
                    question.XmlContent = Util.UpdateS3LinkForPassageLink(question.XmlContent);
                }

                question.IsBaseVirtualQuestion = question.QTIItemSchemaID == (int)QtiSchemaEnum.ExtendedText &&
                                                 question.BaseVirtualQuestionID == null &&
                                                 virtualQuestions.Any(
                                                     o => o.BaseVirtualQuestionID == question.VirtualQuestionID);
                question.IsGhostVirtualQuestion = question.BaseVirtualQuestionID.HasValue &&
                                           virtualQuestions.Any(
                                               o => o.VirtualQuestionID == question.BaseVirtualQuestionID);
                if (question.IsRubricBasedQuestion.HasValue && question.IsRubricBasedQuestion == true)
                {
                    question.RubricQuestionCategories = rubricQuestionCategories.Where(x => x.VirtualQuestionID == question.VirtualQuestionID).ToList();
                    question.PointsPossible = (int)question.RubricQuestionCategories.Sum(x => x.PointsPossible);
                }
                result.Add(question);
            }

            FillAlgorithmicQuestions(result, virtualTestID);
            SetQuestionGroupInfo(virtualTestID, result);

            return new LargeJsonResult {
                Data = result,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = int.MaxValue
            };
        }

        private void FillAlgorithmicQuestions(List<QuestionAssignment> virtualQuestions, int virtualTestID)
        {
            var algorithmicQuestionsData = _parameters.QTITestClassAssignmentServices.GetAlgorithmicQuestionExpressions(virtualTestID);
            if (algorithmicQuestionsData == null || algorithmicQuestionsData.Count == 0)
                return;

            foreach (var virtualQuestion in virtualQuestions)
            {
                if (virtualQuestion.IsApplyAlgorithmicScoring)
                {
                    virtualQuestion.AlgorithmicQuestionExpressions = algorithmicQuestionsData
                                                                            .Where(x => x.VirtualQuestionID == virtualQuestion.VirtualQuestionID)
                                                                            .OrderByDescending(x => x.PointsEarned).ToList();

                    ProccessTheAlgorithmicQuestion(virtualQuestion);
                }
            }
        }

        private void ProccessTheAlgorithmicQuestion(QuestionAssignment questionAssignment)
        {
            if (!questionAssignment.IsApplyAlgorithmicScoring
                || questionAssignment.AlgorithmicQuestionExpressions == null
                || questionAssignment.AlgorithmicQuestionExpressions.Count == 0)
                return;

            var correctAnswers = AlgorithmicHelper.ConvertToAlgorithmicCorrectAnswers(questionAssignment.QTIItemSchemaID, questionAssignment.AlgorithmicQuestionExpressions);
            questionAssignment.AlgorithmicCorrectAnswers = correctAnswers;
            questionAssignment.PointsPossible = correctAnswers.Count > 0 ? correctAnswers.Max(x => x.PointsEarned) : 0;
        }

        private string DisplaySectionInstruction(QTIVirtualTest virtuaQuestion)
        {
            var result = string.Format("<div style='font-style:none;margin-bottom:5px;'><b>{0}</b><div>{1}</div></div>", virtuaQuestion.SectionTitle, virtuaQuestion.SectionInstruction);
            result = result.ReplaceWeirdCharacters();

            return result;
        }

        private void SetQuestionGroupInfo(int virtualTestId, List<QuestionAssignment> questionAssignments)
        {
            var virtualQuestionGroups = _parameters.QuestionGroupService.GetListVirtualQuestionGroupByVirtualTestId(virtualTestId);

            foreach (var questionAssignment in questionAssignments)
            {
                var virtualQuestionGroup = virtualQuestionGroups.FirstOrDefault(o => o.VirtualQuestionID == questionAssignment.VirtualQuestionID);
                if (virtualQuestionGroup != null)
                {
                    questionAssignment.QuestionGroupID = virtualQuestionGroup.QuestionGroupID;
                }
            }
        }

        [HttpGet]
        public ActionResult GetTestOnlineSessionAnswers(int qtiOnlineTestSessionID, int qtiTestClassAssignmentID, string startDate)
        {
            if (!_parameters.VulnerabilityService.HasRigtToEditQtiTestClassAssignment(CurrentUser, qtiTestClassAssignmentID))
            {
                return Json(new { success = false, message = "Has no right", }, JsonRequestBehavior.AllowGet);
            }

            if (!_parameters.VulnerabilityService.HasRigtToEditQtiOnlineTestSession(CurrentUser, qtiOnlineTestSessionID))
            {
                return Json(new { success = false, message = "Has no right", }, JsonRequestBehavior.AllowGet);
            }

            var preference = _parameters.QTITestClassAssignmentServices.GetPreferencesForOnlineTest(qtiTestClassAssignmentID);

            var expired = CheckExpired(startDate, preference);

            var qtiOnlineTestSession =
                _parameters.QTIOnlineTestSessionService.Select()
                    .FirstOrDefault(x => x.QTIOnlineTestSessionId == qtiOnlineTestSessionID);
            var data =
                _parameters.QTITestClassAssignmentServices.GetTestState(qtiOnlineTestSessionID)
                    .Where(x => x.VirtualSectionMode == VirtualSectionModeConstant.Normal);

            var testSessionAnswers = new List<TestOnlineSessionAnwer>();

            var gradingProcessStatus = _parameters.QTITestClassAssignmentServices.GetGradingProcessStatus(qtiOnlineTestSessionID);
            if (qtiOnlineTestSession != null)
            {
                var allPostAnswerLogs = GetAllPostAnswerLogs(qtiOnlineTestSession.QTIOnlineTestSessionId);

                List<IsolatingTestSessionAnswerDTO> isolatingTestSessionAnswers = null;
                var isDataInIsolatingDB = qtiOnlineTestSession.StatusId == 1
                    || qtiOnlineTestSession.StatusId == 2
                    || qtiOnlineTestSession.StatusId == 3
                    || (qtiOnlineTestSession.StatusId == 5
                        && (gradingProcessStatus == GradingProcessStatusEnum.NotStartedHaveSubmitedTest
                            || gradingProcessStatus == GradingProcessStatusEnum.FailedAndWaitingRetry
                            || gradingProcessStatus == GradingProcessStatusEnum.FailedAndNotWaitingRetry));
                if (isDataInIsolatingDB)
                    isolatingTestSessionAnswers =
                        _parameters.IsolatingTestTakerService.GetTestState(qtiOnlineTestSessionID).ToList();

                var findVirtualQuestionIdsOfRubricBase = _parameters.QTITestClassAssignmentServices.GetQTIVirtualTest(qtiOnlineTestSession.VirtualTestId).Where(x => x.VirtualSectionMode == VirtualSectionModeConstant.Normal && x.IsRubricBasedQuestion == true).Select(x => x.VirtualQuestionID).ToArray();
                var rubricQuestionCategories = new List<RubricQuestionCategoryDto>();
                if (findVirtualQuestionIdsOfRubricBase?.Length > 0)
                {
                    rubricQuestionCategories = _parameters.RubricQuestionCategoryService.GetRubicQuestionCategoriesIncludeTestResultScoreByQTIOnlineTestSessionID(findVirtualQuestionIdsOfRubricBase, qtiOnlineTestSessionID).ToList();
                }

                var answerAttachments = _parameters.QTITestClassAssignmentServices.GetAnswerAttachments(qtiOnlineTestSessionID);

                foreach (var o in data)
                {
                    var answer = new TestOnlineSessionAnwer();
                    Mapping(o, answer);
                    FillPostAnswerLogs(allPostAnswerLogs, answer, qtiOnlineTestSessionID);
                    if (isDataInIsolatingDB) UpdateTestSessonAnserFromIsolatingDB(answer, isolatingTestSessionAnswers);

                    answer.RubricQuestionCategories = rubricQuestionCategories.Where(x => x.VirtualQuestionID == o.VirtualQuestionID).ToList();

                    answer.AnswerAttachments = GetAnswerAttachments(answerAttachments, answer.QTIOnlineTestSessionAnswerID, AttachmentTypeEnum.StudentAttachment);
                    answer.TeacherFeebackAttachment = GetAnswerAttachments(answerAttachments, answer.QTIOnlineTestSessionAnswerID, AttachmentTypeEnum.TeacherFeedback).FirstOrDefault();
                    answer.Answered = answer.Answered || (answer.IsRequiredAttachment && answer.AnswerAttachments.Any());
                    testSessionAnswers.Add(answer);
                }
            }

            var scoreRaw = data.Select(o => o.ScoreRaw).FirstOrDefault();
            int subtractFrom100PointPossible = -1;

            if (qtiOnlineTestSession != null)
            {
                var testResult = _parameters.TestResultService.GetTestResultByTestSessionId(qtiOnlineTestSessionID);
                if (testResult != null)
                {
                    var testResultScore = _parameters.TestResultScoreService.GetAll()
                                            .Where(x => x.TestResultID == testResult.TestResultId)
                                            .FirstOrDefault();
                    if (testResultScore != null)
                    {
                        subtractFrom100PointPossible = testResultScore.PointsPossible.GetValueOrDefault();
                    }
                }
            }

            var testfeedback = _parameters.TestFeedbackService.GetLasFeedback(qtiOnlineTestSessionID);
            string lastUserUpdatedFeedback = string.Empty;
            string lastDateUpdatedFeedback = string.Empty;
            if (testfeedback == null)
            {
                testfeedback = new TestFeedback { TestFeedbackID = 0, Feedback = string.Empty };
            }
            else
            {
                var user = _parameters.UserServices.GetUserById(testfeedback.UserID);
                if (user != null)
                {
                    lastUserUpdatedFeedback = string.Format("{0}({1},{2})", user.UserName, user.LastName ?? string.Empty, user.FirstName ?? string.Empty);
                }
                lastDateUpdatedFeedback = ConvertValue.ToZFormat(testfeedback.UpdatedDate).ToString();
            }

            var isBranchingTest = preference == null ? null : preference.BranchingTest;
            if (isBranchingTest.HasValue && isBranchingTest.Value)
                testSessionAnswers = testSessionAnswers.OrderBy(x => x.AnswerOrder).ToList();

            var result = new LargeJsonResult
            {
                Data = new
                {
                    data = testSessionAnswers,
                    expired = expired,
                    scoreRaw = scoreRaw,
                    subtractFrom100PointPossible = subtractFrom100PointPossible,
                    testFeedbackId = testfeedback.TestFeedbackID,
                    feedbackContent = testfeedback.Feedback,
                    lastUserUpdatedFeedback = lastUserUpdatedFeedback,
                    lastDateUpdatedFeedback = lastDateUpdatedFeedback,
                    branchingTest = isBranchingTest,
                    gradingProcessStatus = (int)gradingProcessStatus,
                    teacherFeedbackAttachmentSetting = GetTeacherFeedbackAttachmentSetting()
                },
                MaxJsonLength = int.MaxValue,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

            return result;
        }

        private List<AnswerAttachment> GetAnswerAttachments(
            IEnumerable<AnswerAttachmentDto> answerAttachments,
            int qTIOnlineTestSessionAnswerID,
            AttachmentTypeEnum attachmentType)
        {
            return answerAttachments
                .Where(a => a.QTIOnlineTestSessionAnswerID == qTIOnlineTestSessionAnswerID && a.AttachmentType == (int)attachmentType)
                .Select(x =>
                {
                    var answerAttachment = new AnswerAttachment
                    {
                        DocumentGuid = x.DocumentGuid,
                        FileName = x.FileName,
                        FilePath = x.FilePath,
                        FileType = x.FileType
                    };

                    if (attachmentType == AttachmentTypeEnum.TeacherFeedback)
                    {
                        var s3Settings = LinkitConfigurationManager.GetS3Settings();
                        answerAttachment.FileContent = _answerAttachmentService.DownloadFile(s3Settings, x.FilePath);
                    }

                    return answerAttachment;
                })
                .ToList();
        }

        private void FillPostAnswerLogs(List<PostAnswerLogModel> allPostAnswerLogs, TestOnlineSessionAnwer answer, int qtiOnlineTestSessionID)
        {
            var postAnswerLogs = allPostAnswerLogs.Where(x => x.QTIOnlineTestSessionID == qtiOnlineTestSessionID
                                                           && x.VirtualQuestionID == answer.VirtualQuestionID).OrderByDescending(x => x.Timestamp).ToList();
            if (postAnswerLogs != null && postAnswerLogs.Count > 0)
            {
                answer.PostAnswerLogs.AddRange(postAnswerLogs);
            }
        }

        private List<PostAnswerLogModel> GetAllPostAnswerLogs(int qtiOnlineTestSessionID)
        {
            var result = new List<PostAnswerLogModel>();
            var isolatingPostAnswerLogs = _parameters.IsolatingTestTakerService.GetPostAnswerLogs(qtiOnlineTestSessionID).ToList();

            foreach (var item in isolatingPostAnswerLogs)
            {
                result.Add(new PostAnswerLogModel()
                {
                    Answer = item.Answer,
                    AnswerImage = item.AnswerImage,
                    DumpCol = item.DumpCol,
                    QTIOnlineTestSessionID = qtiOnlineTestSessionID,
                    Timestamp = item.Timestamp,
                    VirtualQuestionID = item.VirtualQuestionID
                });
            }

            return result;
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult RecoverAnswerFromPostAnswerLog(RecoverPostAnswerLogModel model)
        {
            if (!_parameters.VulnerabilityService.HasRigtToEditQtiOnlineTestSession(CurrentUser, model.QtiOnlineTestSessionID))
            {
                return Json(new { success = false, message = "Has no right", },
                    JsonRequestBehavior.AllowGet);
            }

            var qtiOnlineTestSession = _parameters.QTIOnlineTestSessionService.Select()
                                              .FirstOrDefault(x => x.QTIOnlineTestSessionId == model.QtiOnlineTestSessionID);
            if (qtiOnlineTestSession == null
                || qtiOnlineTestSession.StatusId == (int)QTIOnlineTestSessionStatusEnum.Created
                || qtiOnlineTestSession.StatusId == (int)QTIOnlineTestSessionStatusEnum.InProgress)
            {
                return Json(new { success = false, message = "Has no right", },
                 JsonRequestBehavior.AllowGet);
            }

            model.AnswerText = string.IsNullOrEmpty(model.AnswerText) ? "" : model.AnswerText;
            switch (qtiOnlineTestSession.StatusId)
            {
                case (int)QTIOnlineTestSessionStatusEnum.Paused:
                    _parameters.IsolatingTestTakerService.UpdateAnswerText(model.AnswerText, model.AnswerId, model.AnswerSubId);
                    break;

                case (int)QTIOnlineTestSessionStatusEnum.Completed:
                    _parameters.QTITestClassAssignmentServices.UpdateAnswerTextInCompletedStatus(model.AnswerText, model.AnswerId, model.AnswerSubId);
                    break;

                case (int)QTIOnlineTestSessionStatusEnum.PendingReview:
                    _parameters.QTITestClassAssignmentServices.UpdateAnswerTextInPendingReviewStatus(model.AnswerText, model.AnswerId, model.AnswerSubId);
                    break;

                default:
                    break;
            }

            return Json(new { success = true, message = string.Empty },
                  JsonRequestBehavior.AllowGet);
        }

        private bool IsExistBeingGradedQueue(int qtiOnlineTestSessionID)
        {
            var gradingQueue = _parameters.QTITestClassAssignmentServices.GetGradingQueue(qtiOnlineTestSessionID);
            if (gradingQueue != null && (gradingQueue.Status == 0 ||
                                         (gradingQueue.Status == -1 && gradingQueue.IsAwaitingRetry)))
                return true;

            return false;
        }

        [HttpGet]
        public ActionResult GetAnswerForStudent(AnswerViewerModel model)
        {
            if (!model.VirtualQuestionID.HasValue || !model.TestResultID.HasValue)
            {
                return Json(new { IsSucess = false, ErrorMessage = "Detailed student response is not available." }, JsonRequestBehavior.AllowGet);
            }

            var testSessionAnswer = new TestOnlineSessionAnwerViewer();

            var answer = _parameters.QTITestClassAssignmentServices.GetAnswerOfStudent(model.TestResultID.Value, model.VirtualQuestionID.Value);
            if (answer != null)
            {
                Mapping(answer, testSessionAnswer);

                testSessionAnswer.XmlContent = Util.ReplaceWeirdCharactersCommon(testSessionAnswer.XmlContent);
                testSessionAnswer.XmlContent = Util.ReplaceVideoTag(testSessionAnswer.XmlContent);
                testSessionAnswer.XmlContent = Util.UpdateS3LinkForItemMedia(testSessionAnswer.XmlContent);
                testSessionAnswer.XmlContent = Util.UpdateS3LinkForPassageLink(testSessionAnswer.XmlContent);

                var passageList = Util.GetPassageList(testSessionAnswer.XmlContent, false);

                return Json(new { IsSucess = true, TestSessionAnswer = testSessionAnswer, PassageList = passageList }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { IsSucess = false, ErrorMessage = "Detailed student response is not available." }, JsonRequestBehavior.AllowGet);
        }

        private bool CheckExpired(string startDate, PreferenceOptions preference)
        {
            var expired = false;
            var startDateTime = GetDateTime(startDate);
            if (preference != null && preference.TimeLimit.HasValue && preference.TimeLimit.Value)
            {
                if (startDateTime.HasValue && preference.Duration.HasValue && preference.Duration.Value > 0)
                {
                    var endDate = startDateTime.Value.AddMinutes(preference.Duration.Value);
                    expired = DateTime.Compare(DateTime.UtcNow, endDate) > 0;
                }
                else if (preference.Deadline.HasValue)
                {
                    expired = DateTime.Compare(DateTime.UtcNow, preference.Deadline.Value) > 0;
                }
            }
            return expired;
        }

        private void UpdateTestSessonAnserFromIsolatingDB(TestOnlineSessionAnwer answer,
            List<IsolatingTestSessionAnswerDTO> isolatingAnswers)
        {
            if (answer == null || isolatingAnswers == null || isolatingAnswers.Count == 0) return;
            var isolatingAnswer =
                isolatingAnswers.FirstOrDefault(
                    o => o.QTIOnlineTestSessionAnswerID == answer.QTIOnlineTestSessionAnswerID);
            if (isolatingAnswer == null) return;
            answer.HighlightPassage = FormatHighlightPassage(isolatingAnswer.HighlightPassage);
            answer.HighlightQuestion = ReplaceWeirdCharacters(isolatingAnswer.HighlightQuestion) ?? string.Empty;
            answer.HighlightQuestion = Util.ReplaceVideoTag(isolatingAnswer.HighlightQuestion);
            answer.HighlightQuestion = Util.UpdateS3LinkForItemMedia(isolatingAnswer.HighlightQuestion);
            answer.AnswerChoice = isolatingAnswer.AnswerChoice;
            answer.AnswerText = ReplaceWeirdCharacters(isolatingAnswer.AnswerText) ?? string.Empty;
            answer.AnswerTemp = isolatingAnswer.AnswerTemp;
            answer.Answered = isolatingAnswer.Answered;
            answer.AnswerImage = ReformatXmlData(isolatingAnswer.AnswerImage);
            answer.AnswerOrder = isolatingAnswer.AnswerOrder.HasValue ? isolatingAnswer.AnswerOrder.Value : 0;
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
                    answerSub.AnswerText = isolatingAnswerSub.AnswerText.ReplaceWeirdCharacters();
                    answerSub.AnswerTemp = isolatingAnswerSub.AnswerTemp;
                    answerSub.Answered = isolatingAnswerSub.Answered;
                }
            }

            answer.TimeSpent = isolatingAnswer.TimeSpent ?? 0;
            answer.TimesVisited = isolatingAnswer.TimesVisited ?? 0;
            if (string.IsNullOrEmpty(answer.DrawingContent))
            {
                answer.DrawingContent = isolatingAnswer.DrawingContent;
            }
        }

        private void Mapping(QTITestState src, TestOnlineSessionAnwer dest)
        {
            if (src == null || dest == null) return;
            dest.QTIOnlineTestSessionAnswerID = src.QTIOnlineTestSessionAnswerID;
            dest.AnswerID = src.AnswerID;
            dest.QTIOnlineTestSessionID = src.QTIOnlineTestSessionID;
            dest.VirtualQuestionID = src.VirtualQuestionID;
            dest.Answered = src.Answered.HasValue && src.Answered.Value;
            dest.AnswerChoice = src.AnswerChoice;
            dest.AnswerText = ReplaceWeirdCharacters(src.AnswerText) ?? string.Empty;
            dest.AnswerImage = ReformatXmlData(src.AnswerImage);
            dest.HighlightQuestion = Util.RepairUncloseXmlTags(src.HighlightQuestion);
            dest.HighlightQuestion = ReplaceWeirdCharacters(dest.HighlightQuestion) ?? string.Empty;
            dest.HighlightQuestion = Util.ReplaceVideoTag(dest.HighlightQuestion);
            dest.HighlightQuestion = Util.UpdateS3LinkForItemMedia(dest.HighlightQuestion);
            dest.HighlightPassage = FormatHighlightPassage(src.HighlightPassage);
            dest.AnswerTemp = src.AnswerTemp;
            dest.PointsEarned = src.PointsEarned;
            dest.ResponseProcessingTypeID = src.ResponseProcessingTypeID;
            dest.Overridden = src.Overridden;
            dest.UpdatedBy = src.UpdatedBy;
            dest.UpdatedDate = src.UpdatedDate.HasValue ? ConvertValue.ToZFormat(src.UpdatedDate.Value).ToString() : string.Empty;
            dest.TestOnlineSessionAnswerSubs = GetTestOnlineSessionAnswerSubs(src);
            dest.ItemFeedbackID = src.ItemFeedbackID ?? 0;
            dest.ItemAnswerID = src.ItemAnswerID ?? 0;
            dest.Feedback = src.Feedback;
            dest.UserUpdatedFeedback = src.UserUpdatedFeedback;
            dest.DateUpdatedFeedback =
                src.DateUpdatedFeedback == null
                    ? string.Empty
                    : ConvertValue.ToZFormat(src.DateUpdatedFeedback.Value).ToString();
            dest.AnswerOrder = src.AnswerOrder ?? 0;
            dest.TimesVisited = src.TimesVisited;
            dest.TimeSpent = src.TimeSpent;
            dest.DrawingContent = src.DrawingContent;
            dest.IsRequiredAttachment = IsRequiredAttachment(src.XmlContent);
        }

        public List<TestOnlineSessionAnswerSub> GetTestOnlineSessionAnswerSubs(QTITestState item)
        {
            if (string.IsNullOrWhiteSpace(item.AnswerSubs)) return new List<TestOnlineSessionAnswerSub>();
            var xdoc = XDocument.Parse(item.AnswerSubs);
            var result = new List<TestOnlineSessionAnswerSub>();
            foreach (var answersub in xdoc.Element("AnswerSubs").Elements("AnswerSub"))
            {
                var onlineAnswerSub = new TestOnlineSessionAnswerSub();
                onlineAnswerSub.AnswerChoice = GetStringValue(answersub.Element("AnswerChoice"));
                onlineAnswerSub.Answered = GetBoolValue(answersub.Element("Answered"));
                onlineAnswerSub.AnswerText = GetStringValue(answersub.Element("AnswerText")).ReplaceWeirdCharacters();
                onlineAnswerSub.PointsEarned = GetIntNullableValue(answersub.Element("PointsEarned"));
                onlineAnswerSub.QTIOnlineTestSessionAnswerID = item.QTIOnlineTestSessionAnswerID;
                onlineAnswerSub.QTIOnlineTestSessionAnswerSubID =
                    GetIntValue(answersub.Element("QTIOnlineTestSessionAnswerSubID"));
                onlineAnswerSub.ResponseIdentifier = GetStringValue(answersub.Element("ResponseIdentifier"));
                onlineAnswerSub.VirtualQuestionSubID = GetIntValue(answersub.Element("VirtualQuestionSubID"));
                onlineAnswerSub.QTISchemaID = GetIntValue(answersub.Element("QTISchemaID"));
                onlineAnswerSub.PointsPossible = GetIntNullableValue(answersub.Element("PointsPossible"));
                onlineAnswerSub.ResponseProcessingTypeID =
                    GetIntNullableValue(answersub.Element("ResponseProcessingTypeID"));
                onlineAnswerSub.AnswerTemp = GetStringValue(answersub.Element("AnswerTemp"));
                onlineAnswerSub.Overridden = GetBoolValue(answersub.Element("Overridden"));
                onlineAnswerSub.UpdatedBy = GetStringValue(answersub.Element("UpdatedBy"));

                var updatedDate = GetDateTimeNullableValue(answersub.Element("UpdatedDate"));
                onlineAnswerSub.UpdatedDate = updatedDate.HasValue
                    ? ConvertValue.ToZFormat(updatedDate.Value).ToString()
                    : string.Empty;

                result.Add(onlineAnswerSub);
            }

            return result;
        }

        private void Mapping(AnswerViewer src, TestOnlineSessionAnwerViewer dest)
        {
            if (src == null || dest == null) return;
            dest.QTIOnlineTestSessionAnswerID = src.QTIOnlineTestSessionAnswerID;
            dest.AnswerID = src.AnswerID;
            dest.QTIOnlineTestSessionID = src.QTIOnlineTestSessionID.GetValueOrDefault();
            dest.VirtualQuestionID = src.VirtualQuestionID;
            dest.Answered = src.Answered.HasValue && src.Answered.Value;
            dest.AnswerChoice = src.AnswerChoice;
            dest.AnswerText = ReplaceWeirdCharacters(src.AnswerText) ?? string.Empty;
            dest.AnswerImage = ReformatXmlData(src.AnswerImage);
            dest.PointsEarned = src.PointsEarned;
            dest.TestOnlineSessionAnswerSubs = GetTestOnlineSessionAnswerSubs(src);
            dest.XmlContent = src.XMLContent;
            dest.CorrectAnswer = src.CorrectAnswer;
            dest.QTISchemaID = src.QTISchemaID;
            dest.PointsPossible = src.PointsPossible;
        }

        public List<TestOnlineSessionAnswerSub> GetTestOnlineSessionAnswerSubs(AnswerViewer item)
        {
            if (string.IsNullOrWhiteSpace(item.AnswerSubs)) return new List<TestOnlineSessionAnswerSub>();
            var xdoc = XDocument.Parse(item.AnswerSubs);
            var result = new List<TestOnlineSessionAnswerSub>();
            foreach (var answersub in xdoc.Element("AnswerSubs").Elements("AnswerSub"))
            {
                var onlineAnswerSub = new TestOnlineSessionAnswerSub();
                onlineAnswerSub.AnswerChoice = GetStringValue(answersub.Element("AnswerChoice"));
                onlineAnswerSub.Answered = GetBoolValue(answersub.Element("Answered"));
                onlineAnswerSub.AnswerText = (GetStringValue(answersub.Element("AnswerText"))).ReplaceWeirdCharacters();
                onlineAnswerSub.PointsEarned = GetIntNullableValue(answersub.Element("PointsEarned"));
                onlineAnswerSub.QTIOnlineTestSessionAnswerID = item.QTIOnlineTestSessionAnswerID;
                onlineAnswerSub.QTIOnlineTestSessionAnswerSubID =
                    GetIntValue(answersub.Element("QTIOnlineTestSessionAnswerSubID"));
                onlineAnswerSub.ResponseIdentifier = GetStringValue(answersub.Element("ResponseIdentifier"));
                onlineAnswerSub.VirtualQuestionSubID = GetIntValue(answersub.Element("VirtualQuestionSubID"));
                onlineAnswerSub.QTISchemaID = GetIntValue(answersub.Element("QTISchemaID"));
                onlineAnswerSub.PointsPossible = GetIntNullableValue(answersub.Element("PointsPossible"));
                onlineAnswerSub.ResponseProcessingTypeID =
                    GetIntNullableValue(answersub.Element("ResponseProcessingTypeID"));
                onlineAnswerSub.AnswerTemp = GetStringValue(answersub.Element("AnswerTemp"));
                onlineAnswerSub.Overridden = GetBoolValue(answersub.Element("Overridden"));
                onlineAnswerSub.UpdatedBy = GetStringValue(answersub.Element("UpdatedBy"));

                var updatedDate = GetDateTimeNullableValue(answersub.Element("UpdatedDate"));
                onlineAnswerSub.UpdatedDate = updatedDate.HasValue
                    ? ConvertValue.ToZFormat(updatedDate.Value).ToString()
                    : string.Empty;

                result.Add(onlineAnswerSub);
            }

            return result;
        }

        private bool GetBoolValue(XElement element)
        {
            if (element == null) return false;
            return element.Value == "1" || element.Value == "true";
        }

        private string GetStringValue(XElement element)
        {
            if (element == null) return string.Empty;
            return element.Value;
        }

        private int GetIntValue(XElement element)
        {
            if (element == null) return 0;
            return Convert.ToInt32(element.Value);
        }

        private int? GetIntNullableValue(XElement element)
        {
            try
            {
                return Convert.ToInt32(element.Value);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private DateTime? GetDateTimeNullableValue(XElement element)
        {
            try
            {
                return Convert.ToDateTime(element.Value);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpGet]
        public ActionResult GetViewReferenceContent(string refObjectID, string data)
        {
            try
            {
                var mediaModel = new MediaModel();

                if (string.IsNullOrWhiteSpace(refObjectID) && !string.IsNullOrWhiteSpace(data))
                {
                    using (var client = new HttpClient())
                    {
                        HttpResponseMessage response = client.GetAsync(data).Result;
                        ViewBag.Content = response.Content.ReadAsStringAsync().Result;
                        ViewBag.Content = Util.ReplaceTagListByTagOlForPassage(ViewBag.Content);
                        ViewBag.Content = PassageUtil.UpdateS3LinkForPassageMedia(ViewBag.Content, mediaModel.S3Domain, mediaModel.UpLoadBucketName, mediaModel.AUVirtualTestFolder);

                        ViewBag.ReloadImg = false;
                        return PartialView("ViewReferenceContent");
                    }
                }

                var passageHtmlContent = ItemSetPrinting.GetReferenceHtml(refObjectID, data, _s3Service);
                passageHtmlContent = ItemSetPrinting.AdjustXmlContentFloatImg(passageHtmlContent);
                passageHtmlContent = Util.ReplaceTagListByTagOlForPassage(passageHtmlContent, false);
                passageHtmlContent = PassageUtil.UpdateS3LinkForPassageMedia(passageHtmlContent, mediaModel.S3Domain, mediaModel.UpLoadBucketName, mediaModel.AUVirtualTestFolder);

                ViewBag.Content = passageHtmlContent;
                ViewBag.ReloadImg = true;

                return PartialView("ViewReferenceContent");
            }
            catch (Exception)
            {
                ViewBag.Content = string.Empty;
                return PartialView("ViewReferenceContent");
            }
        }

        [HttpGet]
        public ActionResult GetViewReferenceImg(string imgPath)
        {
            imgPath = Util.CorrectImgSrc(imgPath);
            if (!string.IsNullOrWhiteSpace(imgPath))
            {
                if (imgPath.Contains("\""))
                    imgPath = imgPath.Replace("\"", HttpUtility.UrlEncode("\""));
                if (imgPath.Contains("*"))
                    imgPath = imgPath.Replace("*", HttpUtility.UrlEncode("*"));
                if (imgPath.Contains("?"))
                    imgPath = imgPath.Replace("?", HttpUtility.UrlEncode("?"));
                if (imgPath.Contains("<"))
                    imgPath = imgPath.Replace("<", HttpUtility.UrlEncode("<"));
                if (imgPath.Contains(">"))
                    imgPath = imgPath.Replace(">", HttpUtility.UrlEncode(">"));
                if (imgPath.Contains("|"))
                    imgPath = imgPath.Replace("|", HttpUtility.UrlEncode("|"));
            }

            var testItemMediaPath = string.Empty;
            imgPath = imgPath.Replace("/", "\\");
            if (imgPath.StartsWith("\\")) imgPath = imgPath.Substring(1);
            var roFilePath = Path.Combine(testItemMediaPath, imgPath);
            if (imgPath.StartsWith(DataFileUploadConstant.QTI3pSourceUploadSubFolder))
            {
                var thirtParty = ConfigHelper.ThirdPartyItemMediaPath;
                roFilePath = Path.Combine(thirtParty, imgPath);
            }

            var folderName = Path.GetDirectoryName(roFilePath);
            var fileName = LinkitPath.GetFileName(roFilePath);
            roFilePath = Path.Combine(folderName, fileName);
            roFilePath = Path.GetFullPath(roFilePath);
            if (!System.IO.File.Exists(roFilePath))
            {
                return new FileContentResult(new byte[0], "image/jpeg");
            }

            var extension = System.IO.Path.GetExtension(roFilePath);
            var contentType = Util.GetContentType(extension);

            var byteArray = System.IO.File.ReadAllBytes(roFilePath);
            return new FileContentResult(byteArray, contentType);
        }

        [HttpGet]
        public ActionResult GetViewReferenceImgFullPath(string imgPath)
        {
            if (string.IsNullOrWhiteSpace(imgPath)) return new FileContentResult(new byte[0], "image/jpeg");
            if (imgPath.ToLower().StartsWith("http"))
            {
                return Content(imgPath);
            }
            if (!System.IO.File.Exists(imgPath)) return new FileContentResult(new byte[0], "image/jpeg");

            var byteArray = System.IO.File.ReadAllBytes(imgPath);
            return new FileContentResult(byteArray, "image/jpeg");
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult UpdateAnswerPointsEarned(UpdateAnswerPointsEarnedDto model)
        {
            if (!_parameters.VulnerabilityService.HasRigtToEditQtiOnlineTestSession(CurrentUser, model.QTIOnlineTestSessionID))
            {
                return Json(new { success = false, message = "Has no right", },
                    JsonRequestBehavior.AllowGet);
            }

            var canGradingModel = new CanGradingModel
            {
                AnswerID = model.AnswerID,
                AnswerSubID = model.AnswerSubID,
                RoleID = CurrentUser.RoleId,
                QTIOnlineTestSessionID = model.QTIOnlineTestSessionID,
                PointsEarned = model.PointsEarned
            };
            var canGrading = _parameters.TeacherReviewerService.CanGrading(canGradingModel);
            if (!canGrading)
            {
                return Json(new { success = false, message = "Has no right", },
                    JsonRequestBehavior.AllowGet);
            }

            model.UserID = CurrentUser.Id;
            _parameters.QTITestClassAssignmentServices.UpdateAnswerPointsEarned(model);

            if (model.QTIOnlineTestSessionID > 0 && model.VirtualQuestionID > 0 && model.RubricTestResultScores != null && model.RubricTestResultScores.Any())
            {
                _parameters.RubricModuleCommandService.SaveRubricTestResultScores(model.RubricTestResultScores, model.QTIOnlineTestSessionID, model.VirtualQuestionID, CurrentUser.Id);
            }
            var updatedDate = ConvertValue.ToZFormat(DateTime.UtcNow).ToString();

            return Json(new { success = true, message = string.Empty, updatedDate },
                    JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetNextApplicableStudent(int qtiTestClassAssignmentId, int virtualQuestionId, bool isManuallyGradedOnly, int studentId, string pendingStudentIDs = "")
        {
            if (
                !_parameters.VulnerabilityService.HasRigtToEditQtiTestClassAssignment(CurrentUser,
                    qtiTestClassAssignmentId))
            {
                return Json(new { success = false, error = "Has no right" }, JsonRequestBehavior.AllowGet);
            }

            var ignoreCheckOverrideAutoGraded = CurrentUser.IsPublisherOrNetworkAdmin || CurrentUser.IsDistrictAdmin;
            var nextStudent =
                _parameters.QTITestClassAssignmentServices.GetNextApplicableStudent(qtiTestClassAssignmentId,
                    virtualQuestionId, isManuallyGradedOnly, studentId, ignoreCheckOverrideAutoGraded, pendingStudentIDs);
            return Json(nextStudent, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetNextApplicableQuestion(int qtiTestClassAssignmentId, int studentId, bool isManuallyGradedOnly)
        {
            if (!_parameters.VulnerabilityService.HasRigtToEditQtiTestClassAssignment(CurrentUser,
                    qtiTestClassAssignmentId))
            {
                return Json(new { success = false, error = "Has no right" }, JsonRequestBehavior.AllowGet);
            }

            var nextQuestion = _parameters.QTITestClassAssignmentServices.GetNextApplicableQuestion(qtiTestClassAssignmentId,
                studentId, isManuallyGradedOnly, CurrentUser.IsPublisherOrNetworkAdmin || CurrentUser.IsDistrictAdmin);
            return Json(new { success = true, data = nextQuestion }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult UpdateAnswerText(int? answerID, int? answerSubID, bool? saved)
        {
            _parameters.IsolatingTestTakerService.UpdateAnswerText(answerID, answerSubID, saved);

            return Json("success");
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult SubmitTest(string qtiOnlineTestSessionIDs)
        {
            if (string.IsNullOrWhiteSpace(qtiOnlineTestSessionIDs)) return Json("success");
            var ids = qtiOnlineTestSessionIDs.ParseIdsFromString();
            //check modify ajax parameters
            foreach (var id in ids)
            {
                if (!_parameters.VulnerabilityService.HasRigtToEditQtiOnlineTestSession(CurrentUser, id))
                {
                    return Json("error: has no right to submit");
                }
            }
            var qtiOnlineTestSessionIds = new List<int>();
            foreach (var id in ids)
            {
                if (id <= 0) continue;
                var isExist = IsExistBeingGradedQueue(id);
                if (isExist)
                {
                    qtiOnlineTestSessionIds.Add(id);
                }
                else
                {
                    var autoGradingQueue = new AutoGradingQueueData
                    {
                        QTIOnlineTestSessionID = id,
                        CreatedDate = DateTime.UtcNow,
                        Status = AutoGraderStatusNotProcess,
                        ForceGrading = true,
                        RequestUserId = CurrentUser.Id
                    };

                    _parameters.QTITestClassAssignmentServices.AddAutoGradingQueue(autoGradingQueue);
                }
            }
            if (qtiOnlineTestSessionIds.Count > 0)
            {
                return Json(new { QTIOnlineTestSessionIds = qtiOnlineTestSessionIds });
            }
            return Json("success");
        }

        [HttpPost]
        public ActionResult GradingShortcuts(GradingShortcutsRequest request)
        {
            _parameters.QTITestClassAssignmentServices.GradingShortcuts(request, CurrentUser.Id);
            return Json("success");
        }

        [HttpGet]
        [AjaxOnly]
        public ActionResult ViewAttachment(Guid documentGuid, int qtiOnlineTestSessionId)
        {
            var attachmentResult = _parameters.QTITestClassAssignmentServices.ViewAttachment(documentGuid, qtiOnlineTestSessionId, CurrentUser.Id, (RoleEnum)CurrentUser.RoleId);
            var result = new LargeJsonResult
            {
                Data = attachmentResult,
                MaxJsonLength = int.MaxValue,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

            return result;
        }

        private string ReplaceWeirdCharacters(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return string.Empty;
            var result = str.ReplaceWeirdCharacters();

            return result;
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

                return result;
            }
            catch (InvalidOperationException)
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

        private DateTime? GetDateTime(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            DateTime result;
            if (DateTime.TryParse(value, out result))
            {
                return result;
            }

            return null;
        }

        private string FormatHighlightPassage(string passage)
        {
            passage = ReplaceWeirdCharacters(passage) ?? string.Empty;
            passage = Util.ReplaceVideoTag(passage);
            passage = ItemSetPrinting.AdjustXmlContentFloatImg(passage);
            passage = Util.ReplaceTagListByTagOlForPassage(passage, false);

            return passage;
        }

        private static bool IsRequiredAttachment(string xmlContent)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(xmlContent))
                {
                    return false;
                }

                var xmlContentProcessing = new XmlContentProcessing(xmlContent);
                var attachmentSetting = xmlContentProcessing.XmlDocument.GetElementsByTagName("attachmentSetting");
                var node = attachmentSetting.Cast<XmlNode>().FirstOrDefault();

                return node != null ? bool.Parse(node.Attributes["requireAttachment"].Value) : false;
            }
            catch
            {
                return false;
            }
        }

        #region Teacher Feedback

        [HttpPost]
        [AjaxOnly]
        public ActionResult SaveTestFeedback(int? testFeedbackId, int qtiOnlineTestSessionId, string feedbackContent)
        {
            //check to avoid modify ajax parameter
            if (!_parameters.VulnerabilityService.HasRigtToEditQtiOnlineTestSession(CurrentUser, qtiOnlineTestSessionId))
            {
                return Json(new { success = false, message = "Has no right to update Test feedback.", },
                    JsonRequestBehavior.AllowGet);
            }

            if (feedbackContent == null)
            {
                feedbackContent = string.Empty;
            }
            feedbackContent = feedbackContent.Trim();
            feedbackContent = HttpUtility.UrlDecode(feedbackContent);
            var testFeedback = new TestFeedback();
            bool hasChanged = false;
            if (testFeedbackId.HasValue && testFeedbackId.Value > 0)
            {
                //get the feedback
                testFeedback = _parameters.TestFeedbackService.GetTestFeedbackById(testFeedbackId.Value);
                if (testFeedback.Feedback == null)
                {
                    testFeedback.Feedback = string.Empty;
                }
                //check if there is any change in feedback
                if (!feedbackContent.Equals(testFeedback.Feedback))
                {
                    hasChanged = true;
                }
            }
            else
            {
                if (qtiOnlineTestSessionId > 0)
                {
                    testFeedback = _parameters.TestFeedbackService.GetLasFeedback(qtiOnlineTestSessionId);

                    if (testFeedback == null || testFeedback.QtiOnlineTestSessionID == 0)
                    {
                        testFeedback = new TestFeedback { QtiOnlineTestSessionID = qtiOnlineTestSessionId };
                        hasChanged = true;
                    }
                    else
                    {
                        //check if there is any change in feedback
                        if (!feedbackContent.Equals(testFeedback.Feedback))
                        {
                            hasChanged = true;
                        }
                    }
                }
            }
            if (hasChanged && testFeedback.QtiOnlineTestSessionID > 0)
            {
                testFeedback.Feedback = feedbackContent;
                testFeedback.UserID = CurrentUser.Id;
                testFeedback.UpdatedDate = DateTime.UtcNow;
                try
                {
                    _parameters.TestFeedbackService.Save(testFeedback);
                }
                catch (Exception)
                {
                    return Json(new { success = false, message = "Can not save test feedback right now.", },
                        JsonRequestBehavior.AllowGet);
                }
                string lastUserUpdatedFeedback = string.Empty;
                string lastDateUpdatedFeedback = string.Empty;
                var user = _parameters.UserServices.GetUserById(CurrentUser.Id);
                if (user != null)
                {
                    lastUserUpdatedFeedback = string.Format("{0}({1},{2})", user.UserName, user.LastName ?? string.Empty, user.FirstName ?? string.Empty);
                }
                lastDateUpdatedFeedback = ConvertValue.ToZFormat(DateTime.UtcNow).ToString();
                return Json(new { success = true, hasChanged = true, testFeedbackID = testFeedback.TestFeedbackID, lastUserUpdatedFeedback = lastUserUpdatedFeedback, lastDateUpdatedFeedback = lastDateUpdatedFeedback }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = true, hasChanged = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult SaveItemFeedback([ModelBinder(typeof(JsonModelBinder))] TeacherFeedbackRequest request)
        {
            try
            {
                var errorMessage = ValidateTeacherFeedbackRequest(request);

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    return Json(new { success = false, message = errorMessage, }, JsonRequestBehavior.AllowGet);
                }

                request.UserID = CurrentUser.Id;
                request.DistrictID = CurrentUser.DistrictId.Value;
                request.FeedbackContent = HttpUtility.UrlDecode(request.FeedbackContent);

                var files = Request.Files;
                var audioFile = Enumerable.Range(0, files.Count)
                       .Select(x => files[x])
                       .FirstOrDefault(x => x.ContentLength > 0);

                var s3Settings = LinkitConfigurationManager.GetS3Settings();
                var response = _answerAttachmentService.SaveTeacherFeedback(s3Settings, request, audioFile);
                var user = _parameters.UserServices.GetUserById(CurrentUser.Id);

                if (user != null)
                {
                    response.LastUserUpdatedFeedback = $"{user.UserName}({user.LastName},{user.FirstName})";
                }

                response.LastDateUpdatedFeedback = ConvertValue.ToZFormat(DateTime.UtcNow).ToString();
                return Json(new { success = true, data = response, JsonRequestBehavior.AllowGet });
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { success = false, message = ex.Message, JsonRequestBehavior.AllowGet });
            }
        }

        private string ValidateTeacherFeedbackRequest(TeacherFeedbackRequest request)
        {
            if (request == null || request.QTIOnlineTestSessionID == 0 || request.VirtualQuestionID == 0)
            {
                return "Invalid request";
            }

            if(request.AnswerId > 0)
            {
                var answer = _parameters.AnswerService.GetAnswerById(request.AnswerId);

                if (answer == null)
                {
                    return "Invalid item feedback.";
                }

                var testResult = _parameters.TestResultService.GetTestResultById(answer.TestResultID);

                if (testResult == null)
                {
                    return "Invalid item feedback.";
                }

                if (testResult.QTIOnlineTestSessionID.HasValue && !_parameters.VulnerabilityService.HasRigtToEditQtiOnlineTestSession(CurrentUser, testResult.QTIOnlineTestSessionID.Value))
                {
                    return "Has no right to save item feedback.";
                }

                if (!_parameters.VulnerabilityService.CheckUserCanAccessClass(CurrentUser, testResult.ClassId))
                {
                    return "Has no right to save item feedback.";
                }

                var hasRight = _parameters.VulnerabilityService.HasRigtToEditQtiOnlineTestSessionAnswer(
                    CurrentUser,
                    0,
                    request.AnswerId,
                    request.QTIOnlineTestSessionID,
                    request.VirtualQuestionID);

                if (!hasRight)
                {
                    return "Has no right to save item feedback.";
                }
            }
            else if (request.QTIOnlineTestSessionAnswerID > 0)
            {
                var hasRight = _parameters.VulnerabilityService.HasRigtToEditQtiOnlineTestSessionAnswer(
                    CurrentUser,
                    request.QTIOnlineTestSessionAnswerID,
                    0,
                    request.QTIOnlineTestSessionID,
                    request.VirtualQuestionID);

                if (!hasRight)
                {
                    return "Has no right to save item feedback.";
                }
            }

            return string.Empty;
        }

        private static TeacherFeedbackAttachmentSetting GetTeacherFeedbackAttachmentSetting()
        {
            try
            {
                var setting = ConfigurationManager.AppSettings["TeacherFeedbackAudioSetting"];
                return JsonConvert.DeserializeObject<TeacherFeedbackAttachmentSetting>(setting);
            }
            catch
            {
                return null;
            }
        }

        #endregion Teacher Feedback
    }
}
