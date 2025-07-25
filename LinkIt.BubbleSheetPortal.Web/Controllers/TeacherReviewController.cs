using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.Media;
using LinkIt.BubbleSheetPortal.Web.Models.TestAssignmentRegrader;
using LinkIt.BubbleSheetPortal.Web.Print;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels.TeacherReviewModel;
using LinkIt.BubbleSheetPortal.Models.Enum;
using S3Library;

//using RestSharp.Contrib;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize]
    [SessionState(System.Web.SessionState.SessionStateBehavior.Required)]
    [VersionFilter]
    public class TeacherReviewController : BaseController
    {
        private const int AutoGraderStatusNotProcess = 0;

        private readonly TestAssignmentControllerParameters _parameters;
        private readonly IS3Service _s3Service;

        public TeacherReviewController(TestAssignmentControllerParameters parameters, IS3Service s3Service)
        {
            _parameters = parameters;
            _s3Service = s3Service;
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.OnlineTestAssignmentReview)]
        public ActionResult Index(TeacherReviewIndexModel model)
        {
            if (model.QtiTestClassAssignmentID == null)
            {
                return RedirectToAction("Index", "TestAssignmentReview");
            }

            var mediaModel = new MediaModel();
            model.GetViewReferenceImgFullPath = mediaModel.TestMediaFolderPath;

            var user = _parameters.UserServices.GetUserById(CurrentUser.Id);
            if (user != null)
            {
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
            }

            model.QTITestClassAssignment = _parameters.QTITestClassAssignmentServices.GetQtiTestClassAssignment(model.QtiTestClassAssignmentID.Value);
            if (model.QTITestClassAssignment == null)
            {
                return RedirectToAction("Index", "TestAssignmentReview");
            }

            model.VirtualTest =
                _parameters.QTITestClassAssignmentServices.GetVirtualTestByID(model.QTITestClassAssignment.VirtualTestId);
            if (model.VirtualTest == null)
            {
                return RedirectToAction("Index", "TestAssignmentReview");
            }

            model.ClassName = model.QTITestClassAssignment.ClassName;
            model.TeacherName = model.QTITestClassAssignment.TeacherName;

            model.IsAllowToManualGrade = _parameters.RestrictionBO.IsAllowTo(new Models.RestrictionDTO.IsCheckRestrictionObjectDTO
            {
                DistrictId = model.DistrictId,
                BankId = model.VirtualTest.BankID,
                TestId = model.VirtualTest.VirtualTestID,
                RoleId = CurrentUser.RoleId,
                UserId = CurrentUser.Id,
                ModuleCode = RestrictionConstant.Module_Manual_Grade,
                ObjectType = BubbleSheetPortal.Models.RestrictionDTO.RestrictionObjectType.Test
            });

            model.IsAllowToPrint = _parameters.RestrictionBO.IsAllowTo(new Models.RestrictionDTO.IsCheckRestrictionObjectDTO
            {
                DistrictId = model.DistrictId,
                BankId = model.VirtualTest.BankID,
                TestId = model.VirtualTest.VirtualTestID,
                RoleId = CurrentUser.RoleId,
                UserId = CurrentUser.Id,
                ModuleCode = RestrictionConstant.Module_PrintTest,
                ObjectType = BubbleSheetPortal.Models.RestrictionDTO.RestrictionObjectType.Test
            });

            var gradingType = _parameters.DistrictDecodeService.GetDistrictDecodeOfDistrictOrConfigurationByLabel(model.DistrictId, DistrictDecodeLabelConstant.GradingType);
            model.GradingType = gradingType != null ? gradingType.Value : "student";
            return View("Index", model);
        }

        [HttpGet]
        public ActionResult GetStudentsForAssignment(int? qtiTestClassAssignmentID, int? qtiTestStudentAssignmentID)
        {
            var studentAssignments = _parameters.QTITestClassAssignmentServices.GetTestStudentAssignments(qtiTestClassAssignmentID, null);
            if (qtiTestStudentAssignmentID.HasValue)
                studentAssignments =
                    studentAssignments.Where(o => o.QTITestStudentAssignmentID == qtiTestStudentAssignmentID);

            var result = studentAssignments.ToList().Select(o => new StudentAssignment
            {
                QTIOnlineTestSessionID = o.QTIOnlineTestSessionID,
                StudentName = o.StudentName,
                QTOStatusID = o.QTIOnlineStatusID,
                TestName = o.TestName,
                TimeOver = o.TimeOver,
                StartDate = o.StartDate.HasValue ? o.StartDate.ToString() : string.Empty,
                CanBulkGrading = o.CanBulkGrading.HasValue && o.CanBulkGrading.Value
            }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Obsolete]
        [HttpGet]
        public ActionResult GetQuestionsForAssignment(int virtualTestID)
        {
            var virtualQuestions = _parameters.QTITestClassAssignmentServices.GetQTIVirtualTest(virtualTestID).ToList();
            var result = new List<QuestionAssignment>();

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
                    ResponseProcessingTypeID = virtuaQuestion.ResponseProcessingTypeID,
                    IsRubricBasedQuestion = virtuaQuestion.IsRubricBasedQuestion
                };

                question.XmlContent = Util.UpdateS3LinkForItemMedia(question.XmlContent);

                question.IsBaseVirtualQuestion = question.QTIItemSchemaID == (int)QtiSchemaEnum.ExtendedText &&
                                                 question.BaseVirtualQuestionID == null &&
                                                 virtualQuestions.Any(
                                                     o => o.BaseVirtualQuestionID == question.VirtualQuestionID);
                question.IsGhostVirtualQuestion = question.BaseVirtualQuestionID.HasValue &&
                                           virtualQuestions.Any(
                                               o => o.VirtualQuestionID == question.BaseVirtualQuestionID);

                result.Add(question);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private string DisplaySectionInstruction(QTIVirtualTest virtuaQuestion)
        {
            var result = string.Format("<div style='font-style:none;margin-bottom:5px;'><b>{0}</b><div>{1}</div></div>", virtuaQuestion.SectionTitle, virtuaQuestion.SectionInstruction);
            result = ReplaceWeirdCharacters(result);
            return result;
        }

        [HttpGet]
        public ActionResult GetTestOnlineSessionAnswers(int qtiOnlineTestSessionID, int qtiTestClassAssignmentID,
            string startDate)
        {
            var preference =
                _parameters.QTITestClassAssignmentServices.GetPreferencesForOnlineTest(qtiTestClassAssignmentID);
            var multipleChoiceClickMethod =
                _parameters.QTITestClassAssignmentServices.GetPreferenceOptionValue(qtiTestClassAssignmentID,
                    CurrentUser.Id, "multiplechoiceclickmethod");
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

            var data =
                _parameters.QTITestClassAssignmentServices.GetTestState(qtiOnlineTestSessionID).ToList();
            var result = data.Select(
                o => new TestOnlineSessionAnwer
                {
                    QTIOnlineTestSessionAnswerID = o.QTIOnlineTestSessionAnswerID,
                    AnswerID = o.AnswerID,
                    QTIOnlineTestSessionID = o.QTIOnlineTestSessionID,
                    VirtualQuestionID = o.VirtualQuestionID,
                    Answered = o.Answered.HasValue && o.Answered.Value,
                    AnswerChoice = o.AnswerChoice,
                    AnswerText = ReplaceWeirdCharacters(o.AnswerText) ?? string.Empty,
                    AnswerImage = ReformatXmlData(o.AnswerImage),
                    HighlightQuestion = ReplaceWeirdCharacters(o.HighlightQuestion) ?? string.Empty,
                    HighlightPassage = FormatHighlightPassage(o.HighlightPassage),
                    AnswerTemp = o.AnswerTemp,
                    PointsEarned = o.PointsEarned,
                    ResponseProcessingTypeID = o.ResponseProcessingTypeID,
                    QTISchemaID = o.QTISchemaID,
                    Overridden = o.Overridden,
                    UpdatedBy = o.UpdatedBy,
                    UpdatedDate = o.UpdatedDate.HasValue ? o.UpdatedDate.Value.DisplayDateWithFormat() : string.Empty,
                    TestOnlineSessionAnswerSubs = GetTestOnlineSessionAnswerSubs(o),
                    ItemFeedbackID = o.ItemFeedbackID ?? 0,
                    ItemAnswerID = o.ItemAnswerID ?? 0,
                    Feedback = o.Feedback,
                    UserUpdatedFeedback = o.UserUpdatedFeedback,
                    DateUpdatedFeedback = o.DateUpdatedFeedback == null ? string.Empty : o.DateUpdatedFeedback.Value.DisplayDateWithFormat(true)
                }).ToList();

            var scoreRaw = data.Select(o => o.ScoreRaw).FirstOrDefault();
            int subtractFrom100PointPossible = -1;
            var qtiOnlineTestSession =
                _parameters.QTIOnlineTestSessionService.Select()
                    .FirstOrDefault(x => x.QTIOnlineTestSessionId == qtiOnlineTestSessionID);
            if (qtiOnlineTestSession != null)
            {
                var testResult = _parameters.TestResultService.GetTestResults(qtiOnlineTestSession.VirtualTestId,
                    qtiOnlineTestSession.StudentId).FirstOrDefault();
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
                lastDateUpdatedFeedback = testfeedback.UpdatedDate.DisplayDateWithFormat(true);
            }

            return Json(new
            {
                data = result,
                expired = expired,
                scoreRaw = scoreRaw,
                subtractFrom100PointPossible = subtractFrom100PointPossible,
                testFeedbackId = testfeedback.TestFeedbackID,
                feedbackContent = testfeedback.Feedback,
                lastUserUpdatedFeedback = lastUserUpdatedFeedback,
                lastDateUpdatedFeedback = lastDateUpdatedFeedback
            },
                JsonRequestBehavior.AllowGet);
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
                onlineAnswerSub.AnswerText = ReplaceWeirdCharacters(GetStringValue(answersub.Element("AnswerText")));
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
                    ? updatedDate.Value.DisplayDateWithFormat()
                    : string.Empty;

                result.Add(onlineAnswerSub);
            }

            return result;
        }

        [HttpGet]
        public ActionResult GetBatchPrintingView(int? classTestAssignmentId)
        {
            if (classTestAssignmentId.HasValue)
            {
                ViewBag.QTITestClassAssignmentId = classTestAssignmentId.Value;
            }
            return PartialView("_ViewBatchPrinting");
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
                var medialModel = new MediaModel();
                if (string.IsNullOrWhiteSpace(refObjectID) && !string.IsNullOrWhiteSpace(data))
                {
                    using (var client = new HttpClient())
                    {
                        HttpResponseMessage response = client.GetAsync(data).Result;
                        ViewBag.Content = response.Content.ReadAsStringAsync().Result;
                        ViewBag.Content = Util.ReplaceTagListByTagOlForPassage(ViewBag.Content);

                        ViewBag.Content = PassageUtil.UpdateS3LinkForPassageMedia(ViewBag.Content, medialModel.S3Domain, medialModel.UpLoadBucketName, medialModel.AUVirtualTestFolder);
                        ViewBag.ReloadImg = false;
                        return PartialView("ViewReferenceContent");
                    }
                }

                var testItemMediaPath = ConfigurationManager.AppSettings["TestItemMediaPath"];
                if (string.IsNullOrWhiteSpace(testItemMediaPath))
                {
                    ViewBag.Content = string.Empty;
                    return PartialView("ViewReferenceContent");
                }

                var roFilePath = Path.Combine(testItemMediaPath, string.Format("RO\\RO_{0}.xml", refObjectID));
                if (!System.IO.File.Exists(roFilePath))
                {
                    ViewBag.Content = string.Empty;
                    return PartialView("ViewReferenceContent");
                }

                //ViewBag.Content = System.IO.File.ReadAllText(roFilePath);
                var passageHtmlContent = ItemSetPrinting.GetReferenceHtml(refObjectID, data, _s3Service);
                passageHtmlContent = ItemSetPrinting.AdjustXmlContentFloatImg(passageHtmlContent);
                passageHtmlContent = Util.ReplaceTagListByTagOlForPassage(passageHtmlContent, false);
                passageHtmlContent = PassageUtil.UpdateS3LinkForPassageMedia(passageHtmlContent, medialModel.S3Domain, medialModel.UpLoadBucketName, medialModel.AUVirtualTestFolder);
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

            //imgPath = imgPath.Replace("\\", HttpUtility.UrlEncode("\\"));
            //imgPath = imgPath.Replace("/", HttpUtility.UrlEncode("/"));

            var testItemMediaPath = ConfigurationManager.AppSettings["TestItemMediaPath"];
            if (string.IsNullOrWhiteSpace(testItemMediaPath))
            {
                return new FileContentResult(new byte[0], "image/jpeg");
            }

            imgPath = imgPath.Replace("/", "\\");
            if (imgPath.StartsWith("\\")) imgPath = imgPath.Substring(1);
            var roFilePath = Path.Combine(testItemMediaPath, imgPath);
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
            if (IsImgRelativePath(imgPath)) return GetViewReferenceImg(imgPath);
            if (!System.IO.File.Exists(imgPath)) return new FileContentResult(new byte[0], "image/jpeg");

            var byteArray = System.IO.File.ReadAllBytes(imgPath);
            return new FileContentResult(byteArray, "image/jpeg");
        }

        //[HttpPost]
        //public ActionResult UpdateAnswerPointsEarned(UpdateAnswerPointsEarnedDTO model)
        //{
        //    _parameters.QTITestClassAssignmentServices.UpdateAnswerPointsEarned(qtiOnlineTestSessionID, answerID, answerSubID, pointsEarned, CurrentUser.Id);

        //    return Json("success");
        //}

        [HttpPost]
        public ActionResult UpdateAnswerText(int? answerID, int? answerSubID, bool? saved)
        {
            _parameters.QTITestClassAssignmentServices.UpdateAnswerText(answerID, answerSubID, saved);

            return Json("success");
        }

        [HttpPost]
        public ActionResult SubmitTest(string qtiOnlineTestSessionIDs)
        {
            if (string.IsNullOrWhiteSpace(qtiOnlineTestSessionIDs)) return Json("success");
            var ids = qtiOnlineTestSessionIDs.Split(',');

            foreach (var id in ids)
            {
                var qtiOnlineTestSessionID = 0;
                if (!int.TryParse(id, out qtiOnlineTestSessionID) && qtiOnlineTestSessionID <= 0) continue;
                var autoGradingQueue = new AutoGradingQueueData
                {
                    QTIOnlineTestSessionID = qtiOnlineTestSessionID,
                    CreatedDate = DateTime.UtcNow,
                    Status = AutoGraderStatusNotProcess,
                    ForceGrading = true,
                    RequestUserId = CurrentUser.Id
                };

                _parameters.QTITestClassAssignmentServices.AddAutoGradingQueue(autoGradingQueue);
            }

            return Json("success");
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

        private bool IsImgRelativePath(string path)
        {
            var startOfImgRelativePaths = new List<string> { "RO", "/RO", "ItemSet", "/ItemSet" };
            var result =
                startOfImgRelativePaths.Any(
                    startOfImgRelativePath =>
                        path.StartsWith(startOfImgRelativePath, StringComparison.OrdinalIgnoreCase));
            return result;
        }

        private string FormatHighlightPassage(string passage)
        {
            passage = ReplaceWeirdCharacters(passage) ?? string.Empty;
            passage = ItemSetPrinting.AdjustXmlContentFloatImg(passage);
            passage = Util.ReplaceTagListByTagOlForPassage(passage, false);

            return passage;
        }

        #region Teacher Feedback

        public ActionResult SaveTestFeedback(int? testFeedbackId, int qtiOnlineTestSessionId, string feedbackContent)
        {
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
                catch (Exception ex)
                {
                    PortalAuditManager.LogException(ex);
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
                lastDateUpdatedFeedback = DateTime.UtcNow.DisplayDateWithFormat(true);
                return Json(new { success = true, hasChanged = true, testFeedbackID = testFeedback.TestFeedbackID, lastUserUpdatedFeedback = lastUserUpdatedFeedback, lastDateUpdatedFeedback = lastDateUpdatedFeedback }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = true, hasChanged = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SaveItemFeedback(int? itemFeedbackId, int qtiOnlineTestSessionAnswerId, int answerId, string feedbackContent)
        {
            if (feedbackContent == null)
            {
                feedbackContent = string.Empty;
            }
            feedbackContent = feedbackContent.Trim();
            feedbackContent = HttpUtility.UrlDecode(feedbackContent);

            var itemFeedback = new ItemFeedback();
            bool hasChanged = false;
            if (itemFeedbackId.HasValue && itemFeedbackId.Value > 0)
            {
                //get the feedback
                itemFeedback = _parameters.ItemFeedbackService.GetItemFeedbackById(itemFeedbackId.Value);
                if (itemFeedback.Feedback == null)
                {
                    itemFeedback.Feedback = string.Empty;
                }
                //check if there is any change in feedback
                if (!feedbackContent.Equals(itemFeedback.Feedback))
                {
                    hasChanged = true;
                }
            }
            else
            {
                if (answerId > 0) // complete test
                {
                    itemFeedback = _parameters.ItemFeedbackService.GetFeedbackOfAnswer(answerId);
                    if (itemFeedback == null || itemFeedback.ItemFeedbackID == 0)
                    {
                        itemFeedback = new ItemFeedback { AnswerID = answerId, QTIOnlineTestSessionAnswerID = 0 };
                        hasChanged = true;
                    }
                    else
                    {
                        //check if there is any change in feedback
                        if (!feedbackContent.Equals(itemFeedback.Feedback))
                        {
                            hasChanged = true;
                        }
                    }
                }
                else
                {
                    if (qtiOnlineTestSessionAnswerId > 0)
                    {
                        itemFeedback = _parameters.ItemFeedbackService.GetFeedbackOfOnlineSessionAnswer(qtiOnlineTestSessionAnswerId);
                        if (itemFeedback == null || itemFeedback.QTIOnlineTestSessionAnswerID == 0)
                        {
                            itemFeedback = new ItemFeedback { QTIOnlineTestSessionAnswerID = qtiOnlineTestSessionAnswerId, AnswerID = 0 };
                            hasChanged = true;
                        }
                        else
                        {
                            //check if there is any change in feedback
                            if (!feedbackContent.Equals(itemFeedback.Feedback))
                            {
                                hasChanged = true;
                            }
                        }
                    }
                }
            }
            if (hasChanged && (itemFeedback.QTIOnlineTestSessionAnswerID > 0 || itemFeedback.AnswerID.GetValueOrDefault() > 0))
            {
                itemFeedback.Feedback = feedbackContent;
                itemFeedback.UserID = CurrentUser.Id;
                itemFeedback.UpdatedDate = DateTime.UtcNow;
                try
                {
                    _parameters.ItemFeedbackService.Save(itemFeedback);
                }
                catch (Exception ex)
                {
                    PortalAuditManager.LogException(ex);
                    return Json(new { success = false, message = "Can not save item feedback right now.", }, JsonRequestBehavior.AllowGet);
                }
                string lastUserUpdatedFeedback = string.Empty;
                string lastDateUpdatedFeedback = string.Empty;
                var user = _parameters.UserServices.GetUserById(CurrentUser.Id);
                if (user != null)
                {
                    lastUserUpdatedFeedback = string.Format("{0}({1},{2})", user.UserName, user.LastName ?? string.Empty, user.FirstName ?? string.Empty);
                }
                lastDateUpdatedFeedback = DateTime.UtcNow.DisplayDateWithFormat(true);
                return Json(new { success = true, itemFeedbackID = itemFeedback.ItemFeedbackID, hasChanged = true, lastUserUpdatedFeedback = lastUserUpdatedFeedback, lastDateUpdatedFeedback = lastDateUpdatedFeedback }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = true, itemFeedbackID = itemFeedback.ItemFeedbackID, hasChanged = false }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion Teacher Feedback
    }
}
