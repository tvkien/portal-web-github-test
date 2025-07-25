using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using DevExpress.Office.Utils;
using FluentValidation;
using LinkIt.BubbleSheetPortal.Common;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.ACTReport;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.BubbleSheetAws;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using LinkIt.BubbleSheetPortal.Web.ViewModels.ACTReport;
using LinkIt.BubbleSheetPortal.Web.ViewModels.SATReport;
using RestSharp.Extensions;
using S3Library;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class KnowsysSATReportController : BaseController
    {
        private readonly ACTReportService actReportService;
        private readonly UserService userService;
        private readonly SchoolService schoolService;
        private readonly UserSchoolService userSchoolService;
        private readonly TestResultService testResultService;
        private readonly TeacherDistrictTermService teacherDistrictTermService;
        private readonly ClassUserService classUserService;
        private readonly ClassService classService;
        private readonly ClassStudentService classStudentService;
        private readonly DistrictService districtService;
        private readonly StudentService studentService;
        private readonly VirtualTestService virtualTestService;
        private readonly IS3Service _s3Service;
        private readonly IValidator<ACTReportData> actReportDataValidator;
        private readonly BubbleSheetPrintingService bubbleSheetPrintingService;
        private readonly DistrictDecodeService districtDecodeService;
        private readonly BubbleSheetFileService bubbleSheetFileService;
        private int _districtID;
        private const string compositeName = "Composite";

        public KnowsysSATReportController(ACTReportService actReportService, UserService userService,
                                   SchoolService schoolService, UserSchoolService userSchoolService,
                                   TestResultService testResultService,
                                   TeacherDistrictTermService teacherDistrictTermService,
                                   ClassUserService classUserService,
                                   ClassService classService,
                                   ClassStudentService classStudentService, DistrictService districtService,
                                   StudentService studentService,
                                   VirtualTestService virtualTestService, IValidator<ACTReportData> actReportDataValidator, BubbleSheetPrintingService bubbleSheetPrintingService, DistrictDecodeService districtDecodeService,
            BubbleSheetFileService bubbleSheetFileService, IS3Service s3Service)
        {
            this.actReportService = actReportService;
            this.userService = userService;
            this.schoolService = schoolService;
            this.userSchoolService = userSchoolService;
            this.testResultService = testResultService;
            this.teacherDistrictTermService = teacherDistrictTermService;
            this.classUserService = classUserService;
            this.classService = classService;
            this.classStudentService = classStudentService;
            this.districtService = districtService;
            this.studentService = studentService;
            this.virtualTestService = virtualTestService;
            this.actReportDataValidator = actReportDataValidator;
            this.bubbleSheetPrintingService = bubbleSheetPrintingService;
            this.districtDecodeService = districtDecodeService;
            this.bubbleSheetFileService = bubbleSheetFileService;
            _s3Service = s3Service;
        }

        private void BuildStudentInformationData(SATReportMasterViewModel masterModel, int studentID,
                                                 int generatedUserId)
        {
            //var testResultID = masterModel.DiagnosticHistoryViewModel.TestAndScoreViewModels[0].TestResultID;
            if (masterModel.DiagnosticHistoryViewModel.TestAndScoreViewModels.Any() == false)
            {
                masterModel.StudentInformation = new ACTReportStudentInformation();
                return;
            }
            var testResultID =
                masterModel.DiagnosticHistoryViewModel.TestAndScoreViewModels.FirstOrDefault(en => en.IsSelected).
                    TestResultID;

            var studentInfo = actReportService.GetSATStudentInformation(studentID, testResultID);

            var generatedUser = userService.GetUserById(generatedUserId);

            masterModel.StudentInformation = new ACTReportStudentInformation
            {
                DistrictTermName = studentInfo.DistrictTermName,
                StudentID = studentInfo.StudentCode,
                ReportClassName =
                    studentInfo.ClassName + " (" + studentInfo.DistrictTermName +
                    ")",
                ReportTeacherName =
                    Util.FormatFullname(generatedUser.LastName + ", " +
                                        generatedUser.FirstName),
                StudentName =
                    Util.FormatFullname(string.Format("{0}, {1}", studentInfo.StudentLastName,
                                  studentInfo.StudentFirstName)),
                TestResultClassName =
                    studentInfo.ClassName + " (" + studentInfo.DistrictTermName +
                    ")",
                TestResultTeacherName =
                    Util.FormatFullname(studentInfo.TeacherName),
                TestDate = studentInfo.TestDate,
                TestName = studentInfo.TestName
            };
        }

        private void BuildDiagnosticHistoryData(SATReportMasterViewModel masterModel, int studentID, int testID,
                                                int teacherID, int classID)
        {
            masterModel.DiagnosticHistoryViewModel = new SATDiagnosticHistoryViewModel
            {
                TestAndScoreViewModels = new List<SATTestAndScoreViewModel>()
            };
            var diagnotisHistoryData = actReportService.GetSATTestHistoryData(studentID, (int)VirtualTestSubType.SAT);
            if (diagnotisHistoryData.Any())
            {
                var listTestResult = diagnotisHistoryData
                    .OrderByDescending(x => x.UpdatedDate)
                    .Select(x => new
                    {
                        x.TestResultID,
                        x.UpdatedDate
                    }).Distinct().ToList();

                var selectedTestResult =
                    diagnotisHistoryData.Where(en => en.ClassID == classID && en.VirtualTestID == testID).
                        OrderByDescending(en => en.UpdatedDate).FirstOrDefault();
                var selectedTestResultId = selectedTestResult == null ? 0 : selectedTestResult.TestResultID;

                foreach (var testResultData in listTestResult)
                {
                    var data = diagnotisHistoryData.Where(x => x.TestResultID == testResultData.TestResultID).ToList();
                    var subScores = data.Select(en => new SATSubScore
                    {
                        Score = en.SectionScore,
                        ScoreRaw = en.SectionScoreRaw,
                        SectionName = en.SectionName
                    }).ToList();

                    var compositeScore = data.First().CompositeScore;
                    var testScore = new SATTestAndScoreViewModel
                    {
                        TestResultID = testResultData.TestResultID,
                        TestDate = testResultData.UpdatedDate,
                        SubScores = subScores,
                        CompositeScore = compositeScore,
                        IsSelected = testResultData.TestResultID == selectedTestResultId,
                        TestName =
                            diagnotisHistoryData.Any(x => x.TestResultID == testResultData.TestResultID)
                                ? diagnotisHistoryData.First(
                                    x => x.TestResultID == testResultData.TestResultID).TestName
                                : string.Empty
                    };

                    masterModel.DiagnosticHistoryViewModel.TestAndScoreViewModels.Add(testScore);
                }
            }
        }

        private void BuildSessionTagData(SATReportMasterViewModel masterModel, int studentID)
        {
            var subjectTagData = actReportService.GetSATSubjectTagData(studentID, (int)VirtualTestSubType.SAT);

            if (masterModel.DiagnosticHistoryViewModel.TestAndScoreViewModels.Any(en => en.IsSelected))
            {
                var testResultID =
                    masterModel.DiagnosticHistoryViewModel.TestAndScoreViewModels.FirstOrDefault(en => en.IsSelected).
                        TestResultID;
                var lastestSubjectTag = subjectTagData.Where(x => x.TestResultID == testResultID).ToList();
                var listSubject = lastestSubjectTag.Select(x => x.SubjectID).Distinct().ToList();

                var virtualTest = virtualTestService.GetTestByTestResultID(testResultID);

                VirtualSection essaySection = null;
                if (virtualTest != null)
                {
                    essaySection = bubbleSheetPrintingService.GetSATEssaySection(virtualTest, _districtID);
                }

                masterModel.EssaySectionId = essaySection != null ? essaySection.VirtualSectionId : -1;

                //Get Lastest essay pages
                var bubbleSheetId = testResultService.GetTestResultById(testResultID).BubbleSheetId;
                //var bubbleSheetFileSubs = bubbleSheetFileService.GetLastestBubbleSheetFileById(bubbleSheetId).Where(x => x.PageNumber == 3 || x.PageNumber == 4 || x.PageNumber == 5 || x.PageNumber == 6).OrderBy(p => p.PageNumber).ToList();
                var bubbleSheetFileSubs = bubbleSheetFileService.GetLastestBubbleSheetFileSubByBubbleSheetId(bubbleSheetId).OrderBy(p => p.PageNumber).ToList();
                masterModel.BubbleSheetFileSubViewModels = new List<BubbleSheetFileSubViewModel>();
                if (bubbleSheetFileSubs.Any())
                {
                    masterModel.BubbleSheetFileSubViewModels =
                        bubbleSheetFileSubs.Select(x => new BubbleSheetFileSubViewModel()
                        {
                            ImageUrl =
                                BubbleSheetWsHelper.GetTestImageUrl(x.OutputFileName,
                                    ConfigurationManager.AppSettings["ApiKey"]),
                            SubFileName = x.InputFileName,
                            PageType = x.PageType
                        }).ToList();
                }

                //get answer data for each section
                var answerSectionData = new List<ACTAnswerSectionData>();
                if (lastestSubjectTag.Any())
                    answerSectionData = actReportService.GetSATAnswerSectionData(lastestSubjectTag.First().TestResultID)
                        //.Where(x => x.SubjectID != essaySectionID)
                        .ToList();

                foreach (var subjectId in listSubject)
                {
                    //if (essaySectionID == sectionID) continue;
                    var sectionTagModel = new SectionTagViewModel();
                    sectionTagModel.SectionName = lastestSubjectTag.First(x => x.SubjectID == subjectId).SubjectName;
                    sectionTagModel.TagCategoryReportViewModels = new List<TagCategoryReportViewModel>();

                    var tagCategoryData = lastestSubjectTag.Where(x => x.SubjectID == subjectId).ToList();
                    BuildTagCategoryData(sectionTagModel, tagCategoryData, answerSectionData);

                    sectionTagModel.AnswerSectionViewModels = answerSectionData.Where(x => x.SubjectID == subjectId)
                    .GroupBy(x => new
                    {
                        x.WasAnswered,
                        x.PointsEarned,
                        x.PointsPossible,
                        x.CorrectAnswer,
                        x.AnswerLetter,
                        x.AnswerID,
                        x.QuestionOrder,
                        x.QTISchemaID,
                        x.SectionID,
                        x.SectionName,
                        x.VirtualQuestionID
                    })
                    .Select(g => new AnswerSectionViewModel
                    {
                        WasAnswered = g.Key.WasAnswered,
                        PointsEarned = g.Key.PointsEarned,
                        PointsPossible = g.Key.PointsPossible,
                        CorrectAnswer = ParseCorrectAnswer(g.Key.CorrectAnswer),
                        AnswerLetter = g.Key.AnswerLetter,
                        AnswerID = g.Key.AnswerID,
                        QuestionOrder = g.Key.QuestionOrder,
                        QTISchemaID = g.Key.QTISchemaID,
                        SectionID = g.Key.SectionID,
                        SectionName = g.Key.SectionName,
                        VirtualQuestionID = g.Key.VirtualQuestionID,
                        TagNames = string.Join(", ", g.Select(x => x.TagName))
                    }).ToList();

                    masterModel.SectionTagViewModels.Add(sectionTagModel);
                }
            }
        }

        private string ParseCorrectAnswer(string correctAnswer)
        {
            if (correctAnswer.StartsWith("<rangeValue>"))
            {
                //add root elemnt for xml
                var xmlData = string.Format("<root>{0}</root>", correctAnswer);
                var textEntryRangeObject = new TextEntryRangeObject();
                var doc = XDocument.Parse(xmlData);
                if (doc.Root != null)
                {
                    foreach (var rangeValueElement in doc.Root.Elements())
                    {
                        var elements = rangeValueElement.Elements().ToList();
                        if (GetValueFromElement(SATConstants.RangeValueXmlNameTag, elements) == SATConstants.RangeValueXmlNameStartValue)
                        {
                            textEntryRangeObject.StartValue = GetValueFromElement(SATConstants.RangeValueXmlValueTag, elements);
                            textEntryRangeObject.IsStartExclusived =
                                GetValueFromElement(SATConstants.RangeValueXmlExclusivityTag, elements) == SATConstants.RangeValueXmlExclusitivityValue;
                        }

                        if (GetValueFromElement(SATConstants.RangeValueXmlNameTag, elements) == SATConstants.RangeValueXmlNameEndValue)
                        {
                            textEntryRangeObject.EndValue = GetValueFromElement(SATConstants.RangeValueXmlValueTag, elements);
                            textEntryRangeObject.IsEndExclusived =
                                GetValueFromElement(SATConstants.RangeValueXmlExclusivityTag, elements) == SATConstants.RangeValueXmlExclusitivityValue;
                        }
                    }
                }
                var outputStringBuilder = new StringBuilder();
                if (string.IsNullOrEmpty(textEntryRangeObject.StartValue) == false)
                {
                    outputStringBuilder.AppendFormat("{0}", textEntryRangeObject.StartValue);
                    outputStringBuilder.Append(textEntryRangeObject.IsStartExclusived ? "&lt;" : "&lt;=");
                }
                outputStringBuilder.Append("x");
                if (string.IsNullOrEmpty(textEntryRangeObject.EndValue) == false)
                {
                    outputStringBuilder.Append(textEntryRangeObject.IsEndExclusived ? "&lt;" : "&lt;=");
                    outputStringBuilder.AppendFormat("{0}", textEntryRangeObject.EndValue);
                }

                return outputStringBuilder.ToString();
            }
            else
            {
                return correctAnswer;
            }
        }

        private string GetValueFromElement(string tagValue, List<XElement> listElements)
        {
            if (listElements.Any(x => x.Name.LocalName.Equals(tagValue)))
            {
                return listElements.First(x => x.Name.LocalName.Equals(tagValue)).Value;
            }
            return string.Empty;
        }

        private void BuildTagCategoryData(SectionTagViewModel sectionTagModel, List<ACTSectionTagData> tagCategoryData,
                                          List<ACTAnswerSectionData> answerSectionData)
        {
            var listCategory = tagCategoryData.Select(x => x.CategoryID).Distinct().ToList();
            foreach (var categoryID in listCategory)
            {
                var tagCategoryModel = new TagCategoryReportViewModel();
                tagCategoryModel.TagCategoryName = tagCategoryData.First(x => x.CategoryID == categoryID).CategoryName;
                tagCategoryModel.TagCategoryDescription = tagCategoryData.First(x => x.CategoryID == categoryID).CategoryDescription;
                tagCategoryModel.SingleTagReportViewModels = new List<SingleTagReportViewModel>();

                var tagData = tagCategoryData.Where(x => x.CategoryID == categoryID).ToList();
                BuildSingleTagData(tagCategoryModel, tagData, answerSectionData);

                //tagCategoryModel.SingleTagReportViewModels =
                //    tagCategoryModel.SingleTagReportViewModels.Distinct(new DistinctSingleTagReportViewModelComparer()).ToList();

                sectionTagModel.TagCategoryReportViewModels.Add(tagCategoryModel);
            }
        }

        private void BuildSingleTagData(TagCategoryReportViewModel tagCategoryModel, List<ACTSectionTagData> tagData,
                                        List<ACTAnswerSectionData> answerSectionData)
        {
            var listProcessedTagID = new List<int>();
            foreach (var actSectionTagData in tagData)
            {
                if (listProcessedTagID.Contains(actSectionTagData.TagID)) continue;
                var tagModel = new SingleTagReportViewModel
                {
                    IncorrectAnswer = tagData.Where(x => x.TagID == actSectionTagData.TagID).Sum(x => x.IncorrectAnswer),
                    BlankAnswer = tagData.Where(x => x.TagID == actSectionTagData.TagID).Sum(x => x.BlankAnswer),
                    CorrectAnswer = tagData.Where(x => x.TagID == actSectionTagData.TagID).Sum(x => x.CorrectAnswer),
                    HistoricalAverage = Convert.ToInt32(tagData.Where(x => x.TagID == actSectionTagData.TagID).Average(x => x.HistoricalAvg)),
                    TotalAnswer = tagData.Where(x => x.TagID == actSectionTagData.TagID).Sum(x => x.TotalAnswer),
                    TagName = actSectionTagData.TagName,
                    TagNameForOrder = actSectionTagData.TagNameForOrder,
                    Percent = Convert.ToInt32(tagData.Where(x => x.TagID == actSectionTagData.TagID).Average(x => x.Percentage)),
                    ListAnswerInTag = new List<ACTAnswerSectionData>()
                };
                listProcessedTagID.Add(actSectionTagData.TagID);
                tagModel.ListAnswerInTag.AddRange(
                    answerSectionData.Where(
                        x => x.TagID == actSectionTagData.TagID && x.SectionID == actSectionTagData.SectionID).OrderBy(
                            x => x.QuestionOrder).ToList());
                if (tagCategoryModel.IsTechniqueCategory)
                {
                    if (tagModel.IncorrectAnswer > 0 || tagModel.BlankAnswer > 0)
                    {
                        tagCategoryModel.SingleTagReportViewModels.Add(tagModel);
                    }
                }
                else
                {
                    tagCategoryModel.SingleTagReportViewModels.Add(tagModel);
                }
            }

            tagCategoryModel.SingleTagReportViewModels =
                tagCategoryModel.SingleTagReportViewModels.OrderBy(en => en.TagNameForOrder).ToList();
        }

        private void BuildSummaryScoreData(SATReportMasterViewModel masterModel)
        {
            masterModel.SATSummaryScoreViewModel = new SATSummaryScoreViewModel();

            if (masterModel.DiagnosticHistoryViewModel.TestAndScoreViewModels.Any() == false) return;
            // Calculate Score summary
            var baseTestDate = masterModel.DiagnosticHistoryViewModel.TestAndScoreViewModels.Min(en => en.TestDate);
            var testResultID =
                masterModel.DiagnosticHistoryViewModel.TestAndScoreViewModels.FirstOrDefault(en => en.IsSelected).
                    TestResultID;

            var baseSummaryScore =
                masterModel.DiagnosticHistoryViewModel.TestAndScoreViewModels.Where(
                    en => en.TestDate == baseTestDate).OrderBy(en => en.TestResultID).FirstOrDefault();
            var currentSummaryScore =
                masterModel.DiagnosticHistoryViewModel.TestAndScoreViewModels.SingleOrDefault(
                    en => en.TestResultID == testResultID);
            var listSubScoreForSelectedTest =
                currentSummaryScore.SubScores.Select(x => x.SectionName).ToList();

            var allSubScores = new List<SATSubScore>();
            foreach (var testAndScoreViewModel in masterModel.DiagnosticHistoryViewModel.TestAndScoreViewModels)
            {
                allSubScores.AddRange(testAndScoreViewModel.SubScores);
            }


            var bestSummaryScore = new SATTestAndScoreViewModel
            {
                SubScores = allSubScores.GroupBy(en => en.SectionName).Select(grp => new SATSubScore
                {
                    SectionName = grp.Key,
                    Score = grp.Max(s => s.Score)
                }).ToList(),
                CompositeScore =
            masterModel.DiagnosticHistoryViewModel.TestAndScoreViewModels.Max(
                en => en.CompositeScore)
            };
            var improvementSummaryScore = new SATTestAndScoreViewModel
            {
                SubScores = bestSummaryScore.SubScores.Select(en => new SATSubScore
                {
                    SectionName = en.SectionName,
                    Score = en.Score - (baseSummaryScore.SubScores.Any(x => x.SectionName == en.SectionName) ? baseSummaryScore.SubScores.FirstOrDefault(x => x.SectionName == en.SectionName).Score : 0)
                }).ToList(),
                CompositeScore =
                bestSummaryScore.CompositeScore - baseSummaryScore.CompositeScore,
            };

            masterModel.SATSummaryScoreViewModel.SummaryScores = new List<SummaryScoreViewModel>();

            masterModel.SATSummaryScoreViewModel.SummaryScores =
                bestSummaryScore.SubScores
                    .Where(x => listSubScoreForSelectedTest.Contains(x.SectionName))
                    .Select(en => new SummaryScoreViewModel
                    {
                        Subject = en.SectionName,
                        Baseline =
                            (baseSummaryScore.SubScores.Any(
                                x => x.SectionName == en.SectionName)
                                ? baseSummaryScore.SubScores.FirstOrDefault(
                                    x => x.SectionName == en.SectionName).Score
                                : 0),
                        Current =
                            (currentSummaryScore.SubScores.Any(
                                x => x.SectionName == en.SectionName)
                                ? currentSummaryScore.SubScores.FirstOrDefault(
                                    x => x.SectionName == en.SectionName).Score
                                : 0),
                        Best = en.Score,
                        Improvement = (improvementSummaryScore.SubScores.Any(
                            x => x.SectionName == en.SectionName)
                            ? improvementSummaryScore.SubScores.
                            FirstOrDefault(
                                x =>
                            x.SectionName ==
                            en.SectionName).Score
                            : 0)
                    }).ToList();
            masterModel.SATSummaryScoreViewModel.SummaryScores.Add(new SummaryScoreViewModel
            {
                Subject = compositeName,
                Baseline = baseSummaryScore.CompositeScore,
                Current = currentSummaryScore.CompositeScore,
                Best = bestSummaryScore.CompositeScore,
                Improvement =
                    improvementSummaryScore.CompositeScore
            });




            // Calculate scores of each subject
            //masterModel.SATSummaryScoreViewModel.CompositeScores =
            //    masterModel.DiagnosticHistoryViewModel.TestAndScoreViewModels.OrderByDescending(en => en.TestDate).Take(
            //        8).OrderBy(en => en.TestDate).Select(
            //            en => new ScoreViewModel
            //                      {
            //                          DateString =
            //                              en.TestDate.
            //                              ToString(
            //                                  "MM/dd/yyyy"),
            //                          Score =
            //                              CalculateCompositeScoreForSummary
            //                              (en)
            //                      }).ToList();

            masterModel.SATSummaryScoreViewModel.CompositeScores =
                masterModel.DiagnosticHistoryViewModel.TestAndScoreViewModels.OrderByDescending(en => en.TestDate).Take(
                    8).OrderBy(en => en.TestDate).Select(
                        en => new ScoreViewModel()
                        {
                            DateString = en.TestDate.DisplayDateWithFormat(),
                            Score =
                                masterModel.DiagnosticHistoryViewModel.TestAndScoreViewModels.First(
                                    x => x.TestResultID == en.TestResultID).CompositeScore
                        }).ToList();

            masterModel.SATSummaryScoreViewModel.SectionScores = new List<SATScoreViewModel>();
            //var allSectionNames = allSubScores.GroupBy(en => en.SectionName).Select(grp => grp.Key).ToList();
            foreach (var sectionName in listSubScoreForSelectedTest)
            {
                if (string.IsNullOrEmpty(sectionName)) continue;
                var sectionScores = masterModel.DiagnosticHistoryViewModel.TestAndScoreViewModels.OrderByDescending(en => en.TestDate).Take(
                        8).OrderBy(en => en.TestDate).Select(
                            en => new ScoreViewModel
                            {
                                DateString =
                                    en.TestDate.DisplayDateWithFormat(),
                                Score = en.SubScores.Any(x => x.SectionName == sectionName) ? en.SubScores.FirstOrDefault(x => x.SectionName == sectionName).Score : 0
                            }).ToList();

                masterModel.SATSummaryScoreViewModel.SectionScores.Add(new SATScoreViewModel
                {
                    SectionName = sectionName,
                    SectionScores = sectionScores
                });
            }
            masterModel.SATSummaryScoreViewModel.SectionScores.Add(new SATScoreViewModel
            {
                SectionName = compositeName,
                SectionScores = masterModel.SATSummaryScoreViewModel.CompositeScores
            });

            //remove Essay sub score out of summary section
            //masterModel.SATSummaryScoreViewModel.SectionScores =
            //    masterModel.SATSummaryScoreViewModel.SectionScores.Where(
            //        x => x.SectionName.ToLower().Equals(essaySubScoreName.ToLower()) == false).ToList();

            //masterModel.SATSummaryScoreViewModel.SummaryScores =
            //    masterModel.SATSummaryScoreViewModel.SummaryScores.Where(
            //        x => x.Subject.ToLower().Equals(essaySubScoreName.ToLower()) == false).ToList();
        }

        private decimal CalculateCompositeScoreForSummary(SATTestAndScoreViewModel item)
        {
            return item.SubScores.Sum(en => en.Score) / item.SubScores.Count;
        }

        private decimal GetSectionScoreBySectionName(string sectionName, List<ACTTestHistoryData> data)
        {
            var scoreData =
                data.FirstOrDefault(
                    x =>
                    !string.IsNullOrEmpty(x.SectionName) &&
                    x.SectionName.Trim().Equals(sectionName, StringComparison.InvariantCultureIgnoreCase));
            return scoreData == null ? 0 : scoreData.SectionScore;

        }

        public ActionResult ReportPrinting(int studentID, int districtID, int testID, int teacherID, int generatedUserID, int classID, bool isLastTest, int reportContentOption, int generatedUserDistrictId, int? stateInformationId)
        {
            _districtID = districtID;
            Util.LoadDateFormatToCookies(districtID, districtDecodeService);
            var masterModel = BuildReportMasterModel(studentID, districtID, testID, teacherID, generatedUserID, classID, isLastTest, reportContentOption, generatedUserDistrictId, stateInformationId);
            return View(masterModel);
        }

        private byte[] PrintOneFileV2(List<int> listStudentIDs, int districtId, int testID, int teacherID, int timezoneOffset, int classID, int reportContentOption, int generatedUserDistrictId, int? stateInformationId, out string reportUrl)
        {
            var listPdfFiles = new List<byte[]>();
            var listUrls = new List<string>();
            for (int index = 0; index < listStudentIDs.Count; index++)
            {
                var isLastTest = index == listStudentIDs.Count - 1;
                var studentID = listStudentIDs[index];
                var url = Url.Action("ReportPrinting", "KnowsysSATReport",
                    new
                    {
                        studentID,
                        districtID = districtId,
                        testID,
                        teacherID,
                        generatedUserID = CurrentUser.Id,
                        classID,
                        isLastTest,
                        reportContentOption,
                        generatedUserDistrictId,
                        stateInformationId
                    }, HelperExtensions.GetHTTPProtocal(Request));
                listUrls.Add(url);

                var pdf = ExportToPDF(url, timezoneOffset, studentID, districtId);
                listPdfFiles.Add(pdf);

                // Make sure that each student report is printable on double sides
                var pageNo = CountPdfPageNo(pdf);
                if (pageNo % 2 != 0)
                {
                    pdf = ExportBlankPDF();
                    listPdfFiles.Add(pdf);
                }
            }
            var file = PdfHelper.MergeFiles(listPdfFiles);
            reportUrl = string.Join(",", listUrls);
            return file;
        }

        private int CountPdfPageNo(byte[] pdf)
        {
            var rx1 = new Regex(@"/Type\s*/Page[^s]");
            MatchCollection matches = rx1.Matches(System.Text.Encoding.UTF8.GetString(pdf));
            return matches.Count;
        }

        private byte[] ExportBlankPDF()
        {
            var url = Url.Action("BlankPage", "KnowsysSATReport", null, HelperExtensions.GetHTTPProtocal(Request));

            string args =
                string.Format(
                    "--page-size Letter --footer-html \"{0}\" --header-html \"{2}\" --header-spacing 5 \"{1}\" - "
                    //"--header-spacing 5 --footer-spacing 1 \"{0}\" - "
                    , ""
                    , url
                    , ""
                    );

            var startInfo = new ProcessStartInfo(Server.MapPath("~/PDFTool/wkhtmltopdf.exe"), args)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
            };
            var proc = new Process { StartInfo = startInfo };
            proc.Start();

            string output = proc.StandardOutput.ReadToEnd();
            byte[] buffer = proc.StandardOutput.CurrentEncoding.GetBytes(output);
            proc.WaitForExit();
            proc.Close();

            return buffer;
        }

        public ActionResult BlankPage()
        {
            return View();
        }

        private byte[] ExportToPDF(string url, int timezoneOffset, int studentID, int districtId)
        {
            var student = studentService.GetStudentById(studentID);
            var studentName = Util.FormatFullname(string.Format("{0}, {1}", student.LastName, student.FirstName));
            var customFooter = string.Empty;
            var districtDecode = districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(districtId,
                Util.KnowsysSATCustomFooter).FirstOrDefault();
            if (districtDecode != null)
            {
                customFooter = districtDecode.Value;
            }

            var generatedUser = userService.GetUserById(CurrentUser.Id);
            string footerUrl = Url.Action("RenderFooter", "KnowsysSATReport",
                new
                {
                    leftLine1 = DateTime.UtcNow.AddMinutes(timezoneOffset * (-1)).DisplayDateWithFormat(true),
                    //leftLine2 = string.Format("<div id='custom-footer-logon'><img src='{0}' style='position: relative; top: 5px;'><span>Copyright © 2014 | Powered by LinkIt!</span></div>", Url.Content("~/Content/images/loog-linkit-16x16.png")),
                    leftLine2 = string.Empty,
                    rightLine1 = studentName,
                    rightLine2 = string.Empty,
                    customLine = customFooter
                }, HelperExtensions.GetHTTPProtocal(Request));

            string headerUrl = Url.Action("RenderHeader", "KnowsysSATReport", null, HelperExtensions.GetHTTPProtocal(Request));

            string args =
                string.Format(
                    "--page-size Letter --footer-html \"{0}\" --header-html \"{2}\" --header-spacing 5 \"{1}\" - "
                    //"--header-spacing 5 --footer-spacing 1 \"{0}\" - "
                    , footerUrl
                    , url
                    , headerUrl
                    );

            var startInfo = new ProcessStartInfo(Server.MapPath("~/PDFTool/wkhtmltopdf.exe"), args)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
            };
            var proc = new Process { StartInfo = startInfo };
            proc.Start();

            string output = proc.StandardOutput.ReadToEnd();
            byte[] buffer = proc.StandardOutput.CurrentEncoding.GetBytes(output);
            proc.WaitForExit();
            proc.Close();

            return buffer;
        }

        public ActionResult RenderHeader()
        {
            return PartialView("_Header");
        }
        [ValidateInput(false)]
        public ActionResult RenderFooter(string leftLine1, string leftLine2, string rightLine1, string rightLine2, string customLine)
        {
            var footer = new FooterData
            {
                LeftLine1 = leftLine1,
                LeftLine2 = leftLine2,
                RightLine1 = rightLine1,
                RightLine2 = rightLine2,
                CustomLine = customLine
            };
            return PartialView("_Footer", footer);
        }

        [HttpPost]
        [AjaxAwareAuthorize]
        [AjaxOnly]
        public ActionResult Generate(ACTReportData model)
        {
            if (model.DistrictId.HasValue == false)
            {
                model.DistrictId = CurrentUser.DistrictId.GetValueOrDefault();
            }
            model.SetValidator(actReportDataValidator);
            if (!model.IsValid)
            {
                return Json(new { ErrorList = model.ValidationErrors, IsSuccess = false });
            }

            List<int> listStudentId = new List<int>();
            foreach (var studentIdString in model.StudentIdList)
            {
                int studentId;
                if (int.TryParse(studentIdString, out studentId))
                {
                    listStudentId.Add(studentId);
                }
            }

            string reportUrl = string.Empty;

            var pdf = PrintOneFileV2(listStudentId, model.DistrictId.GetValueOrDefault(),
                model.TestId,
                model.TeacherId, model.TimezoneOffset,
                model.ClassId, model.ReportContentOption, CurrentUser.DistrictId.GetValueOrDefault(), model.StateInformationId, out reportUrl);

            var stream = new MemoryStream(pdf);

            var folder = LinkitConfigurationManager.GetS3Settings().ACTReportFolder;
            var bucketName = LinkitConfigurationManager.GetS3Settings().ACTReportBucket;
            var result = _s3Service.UploadRubricFile(bucketName,
                folder + "/" + model.ActReportFileName, stream);

            return Json(new { IsSuccess = true, Url = reportUrl });
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult CheckS3FileExisted(string fileName)
        {
            var folder = LinkitConfigurationManager.GetS3Settings().ACTReportFolder;
            var bucketName = LinkitConfigurationManager.GetS3Settings().ACTReportBucket;
            var result = _s3Service.DownloadFile(bucketName, folder + "/" + fileName);

            if (result.IsSuccess)
            {
                var s3Url = string.Format("https://s3.amazonaws.com/{0}/{1}/{2}", bucketName, folder, fileName);
                return Json(new { Result = true, Url = s3Url });
            }
            else
            {
                return Json(new { Result = false });
            }
        }

        #region Populate data for drop down lists

        [CacheControl(HttpCacheability.NoCache), HttpGet]
        [AjaxAwareAuthorize]
        public ActionResult SATGetSchools(int virtualtestId, int districtId)
        {
            List<int> lstSchoolId = testResultService.GetTestResultsByVirtualTestId(virtualtestId)
                .Select(o => o.SchoolId).Distinct().ToList();
            //TODO: should check lstSchoolId < 2100 ^_^
            IQueryable<ListItem> data;
            if (CurrentUser.IsDistrictAdminOrPublisher)
            {
                int iDistrictId = districtId;
                if (iDistrictId <= 0)
                    iDistrictId = CurrentUser.DistrictId.GetValueOrDefault();
                data = schoolService.GetSchoolsByDistrictId(iDistrictId)
                    .Where(o => lstSchoolId.Contains(o.Id))
                    .Select(x => new ListItem { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name);
            }
            else
            {
                data = userSchoolService.GetSchoolsUserHasAccessTo(CurrentUser.Id)
                    .Where(o => lstSchoolId.Contains(o.SchoolId.Value))
                    .Select(x => new ListItem { Id = x.SchoolId.GetValueOrDefault(), Name = x.SchoolName })
                    .OrderBy(x => x.Name);
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [CacheControl(HttpCacheability.NoCache), HttpGet]
        [AjaxAwareAuthorize]
        public ActionResult SATGetTeachers(int schoolId, int virtualTestId)
        {
            List<int> lstTeacher = testResultService.GetTestResultsByVirtualTestId(virtualTestId)
                .Where(o => o.SchoolId == schoolId)
                .Select(o => o.UserId).Distinct().ToList();
            //TODO: should check lstTeacher < 2100 ^_^
            var validUserSchoolRoleId = new[] { 2, 3, 5, 8, 27 };
            var data =
                userSchoolService.GetSchoolsUserBySchoolId(schoolId)
                    .Where(x => validUserSchoolRoleId.Contains((int)x.Role));
            //Only Publisher, " + LabelHelper.DistrictLabel + " Admin, Shool Admin, Teacher;
            if (CurrentUser.RoleId == (int)Permissions.Teacher)
            {
                data = data.Where(o => o.UserId == CurrentUser.Id);
            }
            var vResult = data.Where(o => lstTeacher.Contains(o.UserId)).Select(x => new
            {
                Id = x.UserId,
                Name = x.UserName,
                FirstName = x.FirstName,
                LastName = x.LastName
            })
                .OrderBy(x => x.LastName)
                .ToList();
            return Json(vResult, JsonRequestBehavior.AllowGet);
        }

        [CacheControl(HttpCacheability.NoCache), HttpGet]
        [AjaxAwareAuthorize]
        public ActionResult ACTGetTerms(int? userId, int schoolId, int virtualTestId)
        {
            List<int> lstDistrictTerm = testResultService.GetTestResultsByVirtualTestId(virtualTestId)
                .Where(o => o.SchoolId == schoolId)
                .Select(o => o.DistrictTermId).Distinct().ToList();
            //TODO: should check lstTeacher < 2100 ^_^

            userId = userId ?? CurrentUser.Id;

            var teacher = userService.GetUserById(userId.GetValueOrDefault());
            if (!teacher.IsNull())
            {
                var data = teacherDistrictTermService.GetTermsByUserIdAndSchoolId(userId.GetValueOrDefault(), schoolId)
                    .Where(o => lstDistrictTerm.Contains(o.DistrictTermId))
                    .Select(x => new ListItem { Id = x.DistrictTermId, Name = x.DistrictName }).OrderBy(x => x.Name);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json(new List<ListItem>().AsQueryable(), JsonRequestBehavior.AllowGet);
        }

        [CacheControl(HttpCacheability.NoCache), HttpGet]
        [AjaxAwareAuthorize]
        public ActionResult SATGetClasses(int termId, int? userId, int virtualTestId)
        {
            var vUserId = userId ?? CurrentUser.Id;
            var teacher = userService.GetUserById(vUserId);
            if (teacher.IsNull())
            {
                return Json(new List<ListItem>().AsQueryable(), JsonRequestBehavior.AllowGet);
            }
            List<int> lstClassId = testResultService.GetTestResultsByVirtualTestId(virtualTestId)
                .Select(o => o.ClassId).Distinct().ToList();
            //TODO: should check lstTeacher < 2100 ^_^

            var classUsers = classUserService.GetClassUsersByUserId(teacher.Id)
                .Where(o => lstClassId.Contains(o.ClassId));
            var classItems = new List<ListItem>();
            foreach (var classUser in classUsers)
            {
                var singleClass = classService.GetClassesByDistrictTermIdAndClassId(termId, classUser.ClassId);
                if (singleClass.IsNull())
                {
                    continue;
                }
                classItems.Add(new ListItem { Id = singleClass.Id, Name = singleClass.Name });
            }
            classItems = classItems.OrderBy(x => x.Name).ToList();
            return Json(classItems.AsQueryable(), JsonRequestBehavior.AllowGet);
        }

        [CacheControl(HttpCacheability.NoCache), HttpGet]
        [AjaxAwareAuthorize]
        public ActionResult SATGetStudents(int classId, int virtualTestId)
        {
            List<int> lstStudentIds = testResultService.GetTestResultsByVirtualTestId(virtualTestId)
                .Where(o => o.ClassId == classId)
                .Select(o => o.StudentId).Distinct().ToList();
            //TODO: should check lstTeacher < 2100 ^_^

            var data = classStudentService.GetClassStudentsByClassId(classId)
                .Where(o => lstStudentIds.Contains(o.StudentId))
                .ToList().Select(x => new { x.StudentId, x.FullName }).OrderBy(x => x.FullName);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion

        private SATReportMasterViewModel BuildReportMasterModel(int studentID, int districtID, int testID, int teacherID, int generatedUserID, int classID, bool isLastTest, int reportContentOption, int generatedUserDistrictId, int? stateInformationId)
        {
            var masterModel = new SATReportMasterViewModel
            {
                SectionTagViewModels = new List<SectionTagViewModel>(),
                ReportContentOption = reportContentOption
            };

            var logoUrl = string.Format("{0}{1}-logo.png", LinkitConfigurationManager.GetS3Settings().S3CSSKey, districtID);

            if (UrlUtil.CheckUrlStatus(logoUrl))
            {
                masterModel.DistrictLogoUrl = logoUrl;
                masterModel.DistrictName = string.Empty;
            }
            else
            {
                var district = districtService.GetDistrictById(districtID);
                masterModel.DistrictLogoUrl = string.Empty;
                masterModel.DistrictName = district.Name;
            }

            BuildReportCustomizeOption(masterModel, districtID);

            BuildTestScoreRangeData(masterModel, districtID);

            BuildDiagnosticHistoryData(masterModel, studentID, testID, teacherID, classID);

            BuildSessionTagData(masterModel, studentID);

            BuildSectionScoreSummary(masterModel); // Must be put after BuildDiagnosticHistoryData && BuildSessionTagData functions

            BuildStudentInformationData(masterModel, studentID, generatedUserID);

            BuildSummaryScoreData(masterModel);

            ReorderSubjectNameForDiagnosticAndSummary(masterModel, districtID);

            BuildEssayCommentData(masterModel, districtID);

            if (stateInformationId.HasValue)
                BuildStateInformationImageData(masterModel, generatedUserDistrictId, stateInformationId.Value);

            masterModel.IsLastReportInList = isLastTest;

            return masterModel;
        }

        private void BuildSectionScoreSummary(SATReportMasterViewModel masterModel)
        {
            var selectedDiagnosticHistoryViewModel =
                masterModel.DiagnosticHistoryViewModel.TestAndScoreViewModels.FirstOrDefault(en => en.IsSelected);

            if (selectedDiagnosticHistoryViewModel != null)
            {
                foreach (var subScore in selectedDiagnosticHistoryViewModel.SubScores)
                {
                    var sectionTagViewModel =
                        masterModel.SectionTagViewModels.FirstOrDefault(x => x.SectionName == subScore.SectionName);

                    if (sectionTagViewModel != null)
                    {
                        subScore.IsCorrectNo =
                            sectionTagViewModel.AnswerSectionViewModels.Count(
                                x => (x.QTISchemaID == 1 || x.QTISchemaID == 3 || x.QTISchemaID == 9) && x.PointsEarned == x.PointsPossible && x.IsCorrected && !x.IsBlank);
                        subScore.IsIncorrectNo =
                            sectionTagViewModel.AnswerSectionViewModels.Count(
                                x => (x.QTISchemaID == 1 || x.QTISchemaID == 3) && (x.PointsEarned != x.PointsPossible || !x.IsCorrected) && !x.IsBlank);
                        subScore.IsBlankNo =
                            sectionTagViewModel.AnswerSectionViewModels.Count(x =>
                                (x.QTISchemaID == 1 || x.QTISchemaID == 3 || x.QTISchemaID == 9) && x.IsBlank);
                    }
                }
            }
        }

        private void BuildReportCustomizeOption(SATReportMasterViewModel model, int districtId)
        {
            try
            {
                var districtDecode =
                    districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(districtId,
                        Util.KNOWSYS_SATReport_SectionPageBreak).FirstOrDefault();
                if (districtDecode != null)
                    model.KNOWSYS_SATReport_SectionPageBreak = districtDecode.Value == "1";

                districtDecode =
                    districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(districtId,
                        Util.KNOWSYS_SATReport_ShowScoreRange).FirstOrDefault();
                if (districtDecode != null)
                    model.KNOWSYS_SATReport_ShowScoreRange = districtDecode.Value == "1";

                districtDecode =
                    districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(districtId,
                        Util.KNOWSYS_SATReport_ShowSectionScoreScaled).FirstOrDefault();
                if (districtDecode != null)
                    model.KNOWSYS_SATReport_ShowSectionScoreScaled = districtDecode.Value == "1";

                districtDecode =
                    districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(districtId,
                        Util.KNOWSYS_SATReport_ShowAssociatedTagName).FirstOrDefault();
                if (districtDecode != null)
                    model.KNOWSYS_SATReport_ShowAssociatedTagName = districtDecode.Value == "1";

                districtDecode =
                    districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(districtId,
                        Util.KNOWSYS_SATReport_ShowEssay).FirstOrDefault();
                if (districtDecode != null)
                    model.KNOWSYS_SATReport_ShowEssay = districtDecode.Value == "1";

                districtDecode =
                    districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(districtId,
                        Util.KNOWSYS_SATReport_ShowComment).FirstOrDefault();
                if (districtDecode != null)
                    model.KNOWSYS_SATReport_ShowComment = districtDecode.Value == "1";
            }
            catch (Exception)
            {
            }
        }

        private void BuildTestScoreRangeData(SATReportMasterViewModel model, int districtId)
        {

            var districtDecode =
                        districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(districtId, Util.KNOWSYS_SATScoreRange).FirstOrDefault();
            if (districtDecode != null)
            {
                // Sample data: Critical Reading:200-800;Math:200-800;Writing:200-800;Total:600-2400;Essay:0-12;Multiple-Choice Writing:20-80

                model.TestScoreRanges = new List<KeyValuePair<string, string>>();
                var items = districtDecode.Value.Split(';');
                foreach (var item in items)
                {
                    if (item.Split(':').Count() == 2)
                    {
                        var key = item.Split(':')[0];
                        var value = item.Split(':')[1];
                        model.TestScoreRanges.Add(new KeyValuePair<string, string>(key, value));
                    }
                }
            }
        }

        private void BuildStateInformationImageData(SATReportMasterViewModel model, int districtId, int stateInformationId)
        {
            try
            {
                var districtDecode =
                    districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(districtId,
                        Util.KNOWSYS_SATReport_StateInformationImage).FirstOrDefault();
                if (districtDecode != null)
                {
                    var imageItem =
                        districtDecode.Value.Split(';')
                            .FirstOrDefault(x => x.Split(':')[0] == stateInformationId.ToString());

                    if (imageItem != null)
                    {
                        var imageUrl = imageItem.Split(':')[1];
                        var stateInformationImageUrl = string.Format("{0}{1}/{2}",
                            LinkitConfigurationManager.GetS3Settings().S3CSSKey,
                            LinkitConfigurationManager.GetS3Settings().KnowsysStateImageFolder, imageUrl);
                        if (UrlUtil.CheckUrlStatus(stateInformationImageUrl))
                        {
                            model.StateInformationImageUrl = stateInformationImageUrl;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void BuildEssayCommentData(SATReportMasterViewModel model, int districtId)
        {
            var reportBannerUrl = string.Format("{0}{1}-report-banner-sat.png", LinkitConfigurationManager.GetS3Settings().S3CSSKey, districtId);
            if (UrlUtil.CheckUrlStatus(reportBannerUrl))
            {
                model.DistrictReportBannerUrl = reportBannerUrl;
            }

            var districtDecode =
                        districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(districtId, Util.KNOWSYS_SATEssayComment_Title).FirstOrDefault();
            if (districtDecode != null)
            {
                model.EssayCommentTitle = districtDecode.Value;
            }

            model.EssayComments = new List<string>();

            if (model.SectionTagViewModels != null && model.SectionTagViewModels.Any(x => x.AnswerSectionViewModels.Any(k => k.SectionID == model.EssaySectionId)))
            {
                var essayAnswerSections =
                    model.SectionTagViewModels.FirstOrDefault(
                        x => x.AnswerSectionViewModels.Any(k => k.SectionID == model.EssaySectionId))
                        .AnswerSectionViewModels.Where(x => x.SectionID == model.EssaySectionId)
                        .ToList(); // model.SectionTagViewModels[4];

                foreach (var answerSection in essayAnswerSections)
                {
                    var pointsEarned = answerSection.PointsEarned;

                    // If the two pointsEarned are equal ==> up one level for the second pointsEarned
                    if (essayAnswerSections.IndexOf(answerSection) == 1
                        && answerSection.PointsEarned == essayAnswerSections[0].PointsEarned)
                        pointsEarned++;

                    districtDecode =
                        districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(districtId,
                            string.Format(Util.KNOWSYS_SATEssayComment_ScoreKey, pointsEarned)).FirstOrDefault();

                    if (districtDecode != null)
                    {
                        var essayComment = districtDecode.Value;
                        model.EssayComments.Add(essayComment);
                    }
                }
            }
        }

        private void ReorderSubjectNameForDiagnosticAndSummary(SATReportMasterViewModel masterModel, int districtID)
        {
            var subScoreList =
                masterModel.DiagnosticHistoryViewModel.TestAndScoreViewModels.Select(
                    x => x.SubScores.Select(y => y.SectionName).ToList()).SelectMany(x => x).Distinct().ToList();
            var orderedSubScoreList = OrderSubjectByDistrictDecodeSetting(districtID, subScoreList);

            ReorderSummaryScores(masterModel, orderedSubScoreList);
            ReorderDiagnosticHistory(masterModel, orderedSubScoreList);

            subScoreList = masterModel.SectionTagViewModels.Select(x => x.SectionName).ToList();
            orderedSubScoreList = OrderSubjectByDistrictDecodeSetting(districtID, subScoreList);
            ReorderSectionTag(masterModel, orderedSubScoreList);
        }

        private void ReorderSummaryScores(SATReportMasterViewModel masterModel, List<string> orderedSubScoreList)
        {
            var tempSummaryScores = masterModel.SATSummaryScoreViewModel.SummaryScores;
            var orderedSummaryScores = new List<SummaryScoreViewModel>();
            foreach (var sectionName in orderedSubScoreList)
            {
                var summaryScore = tempSummaryScores.FirstOrDefault(x => x.Subject == sectionName);
                if (summaryScore != null)
                {
                    orderedSummaryScores.Add(summaryScore);
                }
            }

            masterModel.SATSummaryScoreViewModel.SummaryScores = orderedSummaryScores;

            var tempSectionScores = masterModel.SATSummaryScoreViewModel.SectionScores;
            var orderedSectionScores = new List<SATScoreViewModel>();
            foreach (var sectionName in orderedSubScoreList)
            {
                var summaryScore = tempSectionScores.FirstOrDefault(x => x.SectionName == sectionName);
                if (summaryScore != null)
                {
                    orderedSectionScores.Add(summaryScore);
                }
            }

            masterModel.SATSummaryScoreViewModel.SectionScores = orderedSectionScores;
        }

        private void ReorderDiagnosticHistory(SATReportMasterViewModel masterModel, List<string> orderedSubScoreList)
        {
            foreach (var satTestAndScoreViewModel in masterModel.DiagnosticHistoryViewModel.TestAndScoreViewModels)
            {
                var tempSubScores = satTestAndScoreViewModel.SubScores;
                var orderedSubScores = new List<SATSubScore>();
                foreach (var sectionName in orderedSubScoreList)
                {
                    var score = tempSubScores.FirstOrDefault(x => x.SectionName == sectionName);
                    if (score != null)
                    {
                        orderedSubScores.Add(score);
                    }
                    else if (sectionName == SATConstants.CompositeSubjectName)
                    {
                        orderedSubScores.Add(new SATSubScore()
                        {
                            Score = satTestAndScoreViewModel.CompositeScore,
                            SectionName = SATConstants.CompositeSubjectName
                        });
                    }
                }
                satTestAndScoreViewModel.SubScores = orderedSubScores;
            }


        }

        private void ReorderSectionTag(SATReportMasterViewModel masterModel, List<string> orderedSubScoreList)
        {
            var tempSectionTag = masterModel.SectionTagViewModels;
            var orderedSectionTag = new List<SectionTagViewModel>();
            foreach (var sectionName in orderedSubScoreList)
            {
                var sectionTag = tempSectionTag.FirstOrDefault(x => x.SectionName == sectionName);
                if (sectionTag != null)
                {
                    orderedSectionTag.Add(sectionTag);
                }
            }
            var listSectionNotAdded =
                tempSectionTag.Where(
                    x => orderedSectionTag.Select(y => y.SectionName).ToList().Contains(x.SectionName) == false)
                    .OrderBy(x => x.SectionName).ToList();
            orderedSectionTag.AddRange(listSectionNotAdded);
            masterModel.SectionTagViewModels = orderedSectionTag;
        }

        private List<string> OrderSubjectByDistrictDecodeSetting(int districtID, List<string> subScoreList)
        {
            var orderedSubScoreList = new List<string>();
            var subjectOrderSetting =
                districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(
                    districtID, SATConstants.SATReportSubjectOrderLabel).FirstOrDefault();
            if (subjectOrderSetting != null && !string.IsNullOrEmpty(subjectOrderSetting.Value))
            {
                string[] subjectOrderArray = subjectOrderSetting.Value.Split(new[] { ';' },
                    StringSplitOptions.RemoveEmptyEntries);
                foreach (var subject in subjectOrderArray)
                {
                    if (string.IsNullOrEmpty(subject)) continue;
                    var subscoreItem = subScoreList.Where(x => !string.IsNullOrEmpty(x) && x.ToLower().Contains(subject.ToLower())
                        && !x.ToLower().Equals(SATConstants.EssaySubScoreName.ToLower())
                        && !x.ToLower().Equals(SATConstants.MultipleChoiceWritingSubjectName.ToLower())
                        && !x.ToLower().Equals(SATConstants.CompositeSubjectName.ToLower())).ToList();
                    if (subscoreItem.Any())
                    {
                        orderedSubScoreList.AddRange(subscoreItem);
                    }
                    else if (subject == SATConstants.CompositeSubjectName || subject == SATConstants.EssaySubScoreName || subject == SATConstants.MultipleChoiceWritingSubjectName)
                    {
                        orderedSubScoreList.Add(subject);
                    }
                    //foreach (var item in subscoreItem)
                    //{
                    //    if (!string.IsNullOrEmpty(item))
                    //    {
                    //        orderedSubScoreList.Add(item);
                    //    }
                    //    else if (subject == compositeSubjectName || subject == essaySubScoreName || subject == multipleChoiceWritingSubjectName)
                    //    {
                    //        orderedSubScoreList.Add(subject);
                    //    }
                    //}
                }
                var listSubjectNotAdded = subScoreList.Where(x => orderedSubScoreList.Contains(x) == false)
                    .OrderBy(x => x).ToList();
                if (listSubjectNotAdded.Any())
                {
                    int compositeIndex = orderedSubScoreList.IndexOf(SATConstants.CompositeSubjectName);
                    orderedSubScoreList.InsertRange(compositeIndex, listSubjectNotAdded);
                }
            }
            return orderedSubScoreList;
        }

        private string AlternatingAnswer(string value)
        {
            // Convert from A, B, C, D, E, F ... ==> F, G, H, J, K, L ...
            // ==> Convert from A;C to F;H
            var values = value.Split(';');
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] != "U")
                {
                    values[i] = values[i][0] <= 'C'
                                    ? Convert.ToChar(values[i][0] + 5).ToString()
                                    : Convert.ToChar(values[i][0] + 6).ToString();
                }
            }

            return string.Join(";", values);
        }
    }
}
