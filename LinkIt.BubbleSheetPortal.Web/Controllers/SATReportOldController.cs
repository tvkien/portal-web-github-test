using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using System.Xml.Linq;
using DevExpress.Office.Utils;
using FluentValidation;
using LinkIt.BubbleSheetPortal.Common;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.ACTReport;
using LinkIt.BubbleSheetPortal.Models.Enum;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.BubbleSheetAws;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using LinkIt.BubbleSheetPortal.Web.ViewModels.ACTReport;
using LinkIt.BubbleSheetPortal.Web.ViewModels.SATReport;
using RestSharp.Extensions;
using Ionic.Zip;
using S3Library;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    public class SATReportOldController : BaseController
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
        private readonly VirtualTestConversionService virtualTestConversionService;
        private readonly PopulateReportingService populateReportingService;
        private readonly ConfigurationService configurationService;

        private int _districtID;
        private const string compositeName = "Composite";
        private const string _pageSize = "Letter";

        public SATReportOldController(ACTReportService actReportService, UserService userService,
            SchoolService schoolService, UserSchoolService userSchoolService,
            TestResultService testResultService,
            TeacherDistrictTermService teacherDistrictTermService,
            ClassUserService classUserService,
            ClassService classService,
            ClassStudentService classStudentService, DistrictService districtService,
            StudentService studentService,
            VirtualTestService virtualTestService, IValidator<ACTReportData> actReportDataValidator,
            BubbleSheetPrintingService bubbleSheetPrintingService, DistrictDecodeService districtDecodeService,
            BubbleSheetFileService bubbleSheetFileService,
            VirtualTestConversionService virtualTestConversionService,
            PopulateReportingService populateReportingService,
            ConfigurationService configurationService, IS3Service s3Service)
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
            this.virtualTestConversionService = virtualTestConversionService;
            this.populateReportingService = populateReportingService;
            this.configurationService = configurationService;
            _s3Service = s3Service;
        }

        private void BuildStudentInformationData(SATReportMasterViewModel masterModel, int studentID,
                                                 int generatedUserId, int? takenReportTeacherId, int? takenReportClassId)
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

            masterModel.StudentInformation = new ACTReportStudentInformation
            {
                DistrictTermName = studentInfo.DistrictTermName,
                StudentID = studentInfo.StudentCode,
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

            if (takenReportTeacherId.HasValue)
            {
                var takenReportTeacher = userService.GetUserById(takenReportTeacherId.Value);
                masterModel.StudentInformation.ReportTeacherName = Util.FormatFullname(takenReportTeacher.LastName + ", " + takenReportTeacher.FirstName);
            }
            if (takenReportClassId.HasValue)
            {
                var takenReportClass = classService.GetClassByIdWithoutFilterByActiveTerm(takenReportClassId.Value);
                masterModel.StudentInformation.ReportClassName = Util.FormatFullname(takenReportClass.Name + " (" + studentInfo.DistrictTermName + ")");
            }
        }

        private void BuildDiagnosticHistoryData(SATReportMasterViewModel masterModel, int studentID, int testID,
                                                int teacherID, int classID)
        {
            masterModel.DiagnosticHistoryViewModel = new SATDiagnosticHistoryViewModel
            {
                TestAndScoreViewModels = new List<SATTestAndScoreViewModel>()
            };
            var diagnotisHistoryData = actReportService.GetSATTestHistoryData(studentID, masterModel.VirtualTestSubTypeId);
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

        private void BuildSessionTagData(SATReportMasterViewModel masterModel, int studentID, int testId)
        {
            var subjectTagData = actReportService.GetSATSubjectTagData(studentID, masterModel.VirtualTestSubTypeId);

            // Get Domain tag category ID configuration using for New ACT
            var domainTagCategoryId = GetVirtualTestDistrictId(testId);
            masterModel.DomainTagData = subjectTagData.Where(x => x.CategoryID == domainTagCategoryId).ToList();

            // Do not display Essay tag data in other places of report
            subjectTagData = subjectTagData.Where(x => x.CategoryID != domainTagCategoryId).ToList();

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

                    var answerData = answerSectionData.Where(x => x.SubjectID == subjectId)
                        .Select(x => new AnswerSectionViewModel
                        {
                            WasAnswered = x.WasAnswered,
                            PointsEarned = x.PointsEarned,
                            PointsPossible = x.PointsPossible,
                            CorrectAnswer = ParseCorrectAnswer(x.CorrectAnswer),
                            AnswerLetter = x.AnswerLetter,
                            AnswerID = x.AnswerID,
                            QuestionOrder = x.QuestionOrder,
                            QTISchemaID = x.QTISchemaID,
                            SectionID = x.SectionID,
                            SectionName = x.SectionName,
                            VirtualQuestionID = x.VirtualQuestionID
                        }).Distinct(new AnswerSectionViewModel.Comparer()).ToList();
                    sectionTagModel.AnswerSectionViewModels = answerData;
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

        private int GetVirtualTestDistrictId(int virtualTestId)
        {
            // Get Domain tag category ID configuration using for New ACT
            var domainTagCategoryId = 0;

            var virtualTest = virtualTestService.GetTestById(virtualTestId);
            if (virtualTest != null && virtualTest.AuthorUserID != null)
            {
                var user = userService.GetUserById(virtualTest.AuthorUserID.GetValueOrDefault());
                if (user != null)
                {
                    var domainTagCategoryIdDistrictDecode = districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(user.DistrictId.GetValueOrDefault(), "SATDomainTagCategoryID").FirstOrDefault();
                    if (domainTagCategoryIdDistrictDecode != null)
                        domainTagCategoryId = Convert.ToInt32(domainTagCategoryIdDistrictDecode.Value);
                }
            }

            return domainTagCategoryId;
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
                        x => x.TagID == actSectionTagData.TagID && x.SubjectID == actSectionTagData.SubjectID).OrderBy(
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

        public ActionResult ReportPrinting(int studentID, int districtID, int testID, int teacherID, int generatedUserID, int classID, bool isLastTest, int reportContentOption, int? takenReportTeacherId, int? takenReportClassId)
        {
            _districtID = districtID;
            Util.LoadDateFormatToCookies(districtID, districtDecodeService);

            var masterModel = BuildReportMasterModel(studentID, districtID, testID, teacherID, generatedUserID, classID, isLastTest, reportContentOption, takenReportTeacherId, takenReportClassId);
            return View(masterModel);
        }

        //public ActionResult ReportPrintingMultipleStudents(string studentIDs, int districtID, int testID, int teacherID, int generatedUserID, int classID, bool isLastTest, int reportContentOption)
        //{
        //    _districtID = districtID;
        //    var listStudentID = new List<int>();
        //    var parseStrStudentID = studentIDs.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
        //    foreach (var strID in parseStrStudentID)
        //    {
        //        int studentID;
        //        if (int.TryParse(strID, out studentID))
        //        {
        //            listStudentID.Add(studentID);
        //        }
        //    }
        //    var model = new SATReportMasterCollectionViewModel();
        //    model.ListSATReportMasterViewModel = new List<SATReportMasterViewModel>();
        //    foreach (var studentID in listStudentID)
        //    {
        //        var masterModel = BuildReportMasterModel(studentID, districtID, testID, teacherID, generatedUserID, classID, isLastTest, reportContentOption);
        //        masterModel.IsLastReportInList = false;
        //        model.ListSATReportMasterViewModel.Add(masterModel);
        //    }
        //    if (model.ListSATReportMasterViewModel.Count > 0)
        //    {
        //        model.ListSATReportMasterViewModel[model.ListSATReportMasterViewModel.Count - 1].IsLastReportInList =
        //            true;
        //    }
        //    //return View(masterModel);
        //    return View(model);
        //}

        private byte[] PrintOneFileV2(List<int> listStudentIDs, int districtId, int testID, int teacherID, int timezoneOffset, int classID, int reportContentOption, out string reportUrl)
        {
            var listPdfFiles = new List<byte[]>();
            var listUrls = new List<string>();
            for (int index = 0; index < listStudentIDs.Count; index++)
            {
                var isLastTest = index == listStudentIDs.Count - 1;
                var studentID = listStudentIDs[index];
                var url = Url.Action("ReportPrinting", "SATReportOld",
                    new
                    {
                        studentID,
                        districtID = districtId,
                        testID,
                        teacherID,
                        generatedUserID = CurrentUser.Id,
                        classID,
                        isLastTest,
                        reportContentOption
                    }, HelperExtensions.GetHTTPProtocal(Request));
                listUrls.Add(url);

                var pdf = ExportToPDF(url, timezoneOffset, studentID);
                listPdfFiles.Add(pdf);
            }
            var file = PdfHelper.MergeFilesAddBlankToOddFile(listPdfFiles);
            reportUrl = string.Join(",", listUrls);
            return file;
        }

        private List<ReportFileResultModel> PrintOneFileNewSAT(int districtId, int? schoolId, int? teacherId, int? districtTermId, int? classId
            , List<int> listVirtualTestIds, List<int> listStudentIds
            , DateTime? resultDateFrom, DateTime? resultDateTo
            , int specializedReportJobId
            , int timezoneOffset, int reportContentOption, bool? isGetAllClass, out string reportUrl)
        {
            if (isGetAllClass.HasValue && isGetAllClass.Value == true)
            {
                return PrintOneFileNewSATWithGetAllClassOption(districtId, schoolId, teacherId, districtTermId, classId
                    , listVirtualTestIds, listStudentIds, resultDateFrom, resultDateTo, specializedReportJobId
                    , timezoneOffset, reportContentOption, out reportUrl);
            }
            else
            {
                return PrintOneFileNewSATWithoutGetAllClassOption(districtId, schoolId, teacherId, districtTermId, classId
                    , listVirtualTestIds, listStudentIds, resultDateFrom, resultDateTo, specializedReportJobId
                    , timezoneOffset, reportContentOption, out reportUrl);
            }
        }

        private List<ReportFileResultModel> PrintOneFileNewSATWithoutGetAllClassOption(int districtId, int? schoolId, int? teacherId, int? districtTermId, int? classId
            , List<int> listVirtualTestIds, List<int> listStudentIds
            , DateTime? resultDateFrom, DateTime? resultDateTo
            , int specializedReportJobId
            , int timezoneOffset, int reportContentOption, out string reportUrl)
        {
            var reportfileResultModels = new List<ReportFileResultModel>();

            var virtualTestIdString = string.Join(",", listVirtualTestIds);
            var virtualTestSubTypeIdString = "7";

            var listUrls = new List<string>();

            var listSchoolIds = new List<int>();
            if (schoolId.HasValue)
                listSchoolIds.Add(schoolId.Value);
            else
            {
                listSchoolIds.AddRange(NewSatGetListSchoolIds(districtId, virtualTestIdString,
                    virtualTestSubTypeIdString, resultDateFrom, resultDateTo));
            }

            var listStudentHasReport = new List<string>();

            foreach (var listSchoolId in listSchoolIds)
            {
                var listTeacherIds = new List<int>();

                if (teacherId.HasValue)
                    listTeacherIds.Add(teacherId.Value);
                else
                {
                    listTeacherIds.AddRange(NewSatGetListTeacherIds(districtId, listSchoolId, virtualTestIdString,
                        virtualTestSubTypeIdString, resultDateFrom, resultDateTo));
                }

                foreach (var listTeacherId in listTeacherIds)
                {
                    var listClassIds = new List<int>();

                    if (classId.HasValue)
                        listClassIds.Add(classId.Value);
                    else
                    {
                        listClassIds.AddRange(NewSatGetListClassIds(districtId, listSchoolId, listTeacherId, districtTermId,
                            virtualTestIdString,
                            virtualTestSubTypeIdString, resultDateFrom, resultDateTo));
                    }

                    foreach (var listClassId in listClassIds)
                    {
                        var currentTeacher = userService.GetUserById(listTeacherId);
                        var currentClass = classService.GetClassByIdWithoutFilterByActiveTerm(listClassId);

                        var reportFileResultModel = new ReportFileResultModel
                        {
                            ClassName = currentClass.Name,
                            TeacherLastName = currentTeacher.LastName,
                            PdfFiles = new List<byte[]>()
                        };


                        foreach (var virtualTestId in listVirtualTestIds)
                        {
                            var listReportStudentIds = new List<int>();
                            var studentIds = MultipleTestGetStudents(listClassId, virtualTestId.ToString(), resultDateFrom,
                                    resultDateTo, false).OrderBy(x => x.FullName).Select(x => x.StudentId);

                            if (listStudentIds != null && listStudentIds.Any())
                            {
                                studentIds = studentIds.Where(listStudentIds.Contains);
                            }

                            listReportStudentIds.AddRange(studentIds);

                            for (int index = 0; index < listReportStudentIds.Count; index++)
                            {
                                var studentId = listReportStudentIds[index];
                                if (listStudentHasReport.Any(x => x == studentId + ";" + virtualTestId))
                                {
                                    continue;
                                }
                                listStudentHasReport.Add(studentId + ";" + virtualTestId);

                                var url = Url.Action("ReportPrinting", "SATReportOld",
                                    new
                                    {
                                        studentID = studentId,
                                        districtID = districtId,
                                        testID = virtualTestId,
                                        teacherID = listTeacherId,
                                        generatedUserID = CurrentUser.Id,
                                        classID = listClassId,
                                        isLastTest = false,
                                        reportContentOption
                                    }, HelperExtensions.GetHTTPProtocal(Request));
                                listUrls.Add(url);

                                var pdf = ExportToPDF(url, timezoneOffset, studentId);
                                reportFileResultModel.PdfFiles.Add(pdf);
                                IncreaseGeneratedItemNo(specializedReportJobId);
                            }
                        }

                        reportfileResultModels.Add(reportFileResultModel);
                    }
                }
            }

            reportUrl = string.Join(",", listUrls);

            return reportfileResultModels;
        }

        private List<ReportFileResultModel> PrintOneFileNewSATWithGetAllClassOption(int districtId, int? schoolId, int? teacherId, int? districtTermId, int? classId
            , List<int> listVirtualTestIds, List<int> listStudentIds
            , DateTime? resultDateFrom, DateTime? resultDateTo
            , int specializedReportJobId
            , int timezoneOffset, int reportContentOption, out string reportUrl)
        {
            Dictionary<int, Dictionary<int, List<int>>> sumClassTestStudents = new Dictionary<int, Dictionary<int, List<int>>>();

            var reportfileResultModels = new List<ReportFileResultModel>();

            var virtualTestIdString = string.Join(",", listVirtualTestIds);
            var virtualTestSubTypeIdString = "7";

            var listUrls = new List<string>();
            var listSchoolIds = new List<int>();
            var listStudentTestDistrictTerms = new List<StudentTestDistrictTerm>();

            if (schoolId.HasValue)
            {
                listSchoolIds.Add(schoolId.Value);
            }
            else
            {
                listSchoolIds = populateReportingService.ReportingGetSchools(CurrentUser.Id, CurrentUser.RoleId,
                    districtId, virtualTestIdString, virtualTestSubTypeIdString, resultDateFrom, resultDateTo, isGetAllClass: true)
                    .Select(x => x.TakenTestSchoolId).Distinct().ToList();
            }

            var listStudentHasReport = new List<string>();
            foreach (var listSchoolId in listSchoolIds)
            {
                var listTeacherIds = new List<int>();

                if (teacherId.HasValue)
                {
                    listTeacherIds.Add(teacherId.Value);
                }
                else
                {
                    listTeacherIds = populateReportingService.ReportingGetTeachers(CurrentUser.Id, CurrentUser.RoleId,
                        districtId, listSchoolId, virtualTestIdString, virtualTestSubTypeIdString, resultDateFrom, resultDateTo, isGetAllClass: true)
                        .OrderBy(x => x.NameLast).ThenBy(x => x.NameFirst)
                        .Select(x => x.TakenTestTeacherId).Distinct().ToList();
                }

                foreach (var listTeacherId in listTeacherIds)
                {
                    var listClassIds = new List<int>();

                    if (classId.HasValue)
                    {
                        listClassIds.Add(classId.Value);
                    }
                    else
                    {
                        populateReportingService.ReportingGetClasses(CurrentUser.Id, CurrentUser.RoleId, districtId, listSchoolId, listTeacherId
                            , districtTermId, virtualTestIdString, virtualTestSubTypeIdString, resultDateFrom, resultDateTo, isGetAllClass: true)
                            .Select(x => x.TakenTestClassId).Distinct().ToList();
                    }

                    foreach (var listClassId in listClassIds)
                    {
                        foreach (var virtualTestId in listVirtualTestIds)
                        {
                            var listReportStudentIds = new List<int>();

                            var studentTestDistrictTerms = teacherDistrictTermService.GetStudentTestDistrictTerm(null, null, null, null
                                , classId, new List<int>() { virtualTestId }, null, resultDateFrom, resultDateTo);

                            if (listStudentIds != null && listStudentIds.Any())
                            {
                                studentTestDistrictTerms = studentTestDistrictTerms.Where(x => listStudentIds.Contains(x.StudentId));
                            }

                            foreach (var student in studentTestDistrictTerms)
                            {
                                if (listStudentTestDistrictTerms.All(x => x.StudentId != student.StudentId))
                                {
                                    listStudentTestDistrictTerms.Add(student);
                                }

                                if (!sumClassTestStudents.ContainsKey(student.TakenTestClassId))
                                {
                                    sumClassTestStudents.Add(student.TakenTestClassId, new Dictionary<int, List<int>>());
                                }

                                if (!sumClassTestStudents[student.TakenTestClassId].ContainsKey(student.VirtualTestId))
                                {
                                    sumClassTestStudents[student.TakenTestClassId].Add(student.VirtualTestId, new List<int>());
                                }

                                if (!sumClassTestStudents[student.TakenTestClassId][student.VirtualTestId].Contains(student.StudentId))
                                {
                                    sumClassTestStudents[student.TakenTestClassId][student.VirtualTestId].Add(student.StudentId);
                                }
                            }
                        }
                    }
                }
            }

            listStudentTestDistrictTerms = listStudentTestDistrictTerms.OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ToList();


            // If Class is not selected ==> print and group report based on taken test class
            if (!classId.HasValue)
            {
                foreach (var classData in sumClassTestStudents)
                {
                    var classUser = classUserService.GetPrimaryTeacherByClassId(classData.Key);
                    var currentTeacher = userService.GetUserById(classUser.UserId);
                    var currentClass = classService.GetClassByIdWithoutFilterByActiveTerm(classData.Key);

                    var reportFileResultModel = new ReportFileResultModel
                    {
                        ClassName = currentClass.Name,
                        TeacherLastName = currentTeacher.LastName,
                        PdfFiles = new List<byte[]>()
                    };

                    foreach (var virtualTestData in classData.Value)
                    {
                        // Order List by Student Name
                        var sortListStudentIds = new List<int>();
                        foreach (var student in listStudentTestDistrictTerms)
                        {
                            if (virtualTestData.Value.Any(x => x == student.StudentId))
                            {
                                sortListStudentIds.AddRange(virtualTestData.Value.Where(x => x == student.StudentId).ToList());
                            }
                        }
                        virtualTestData.Value.Clear();
                        virtualTestData.Value.AddRange(sortListStudentIds);

                        foreach (var studentId in virtualTestData.Value)
                        {
                            var url = Url.Action("ReportPrinting", "SATReportOld",
                                        new
                                        {
                                            studentID = studentId,
                                            districtID = districtId,
                                            testID = virtualTestData.Key,
                                            teacherID = currentTeacher.Id,
                                            generatedUserID = CurrentUser.Id,
                                            classID = classData.Key,
                                            isLastTest = false,
                                            reportContentOption
                                        }, HelperExtensions.GetHTTPProtocal(Request));
                            listUrls.Add(url);

                            var pdf = ExportToPDF(url, timezoneOffset, studentId);
                            reportFileResultModel.PdfFiles.Add(pdf);
                            IncreaseGeneratedItemNo(specializedReportJobId);
                        }
                    }

                    reportfileResultModels.Add(reportFileResultModel);
                }
            }
            else// Print and group report based on selected class
            {
                // Group student by Virtual test and print one file for each virtualtest
                var testStudents = new Dictionary<int, List<string>>();
                foreach (var classData in sumClassTestStudents)
                {
                    foreach (var virtualTestData in classData.Value)
                    {
                        foreach (var studentId in virtualTestData.Value)
                        {
                            if (!testStudents.ContainsKey(virtualTestData.Key))
                            {
                                testStudents.Add(virtualTestData.Key, new List<string>());
                            }

                            testStudents[virtualTestData.Key].Add(classData.Key + ";" + studentId);
                        }
                    }
                }

                var currentTeacher = userService.GetUserById(teacherId.Value);
                var currentClass = classService.GetClassByIdWithoutFilterByActiveTerm(classId.Value);

                foreach (var virtualTestData in testStudents)
                {
                    var reportFileResultModel = new ReportFileResultModel
                    {
                        ClassName = currentClass.Name,
                        TeacherLastName = currentTeacher.LastName,
                        PdfFiles = new List<byte[]>()
                    };

                    // Order List by Student Name
                    var sortListStrings = new List<string>();
                    foreach (var student in listStudentTestDistrictTerms)
                    {
                        if (virtualTestData.Value.Any(x => x.Split(';')[1] == student.StudentId.ToString()))
                        {
                            sortListStrings.AddRange(virtualTestData.Value.Where(x => x.Split(';')[1] == student.StudentId.ToString()).OrderBy(x => Convert.ToInt32(x.Split(';')[0])).ToList());
                        }
                    }
                    virtualTestData.Value.Clear();
                    virtualTestData.Value.AddRange(sortListStrings);

                    foreach (var studentData in virtualTestData.Value)
                    {
                        var takenTestClassId = Convert.ToInt32(studentData.Split(';')[0]);
                        var studentId = Convert.ToInt32(studentData.Split(';')[1]);
                        var classUser = classUserService.GetPrimaryTeacherByClassId(takenTestClassId);

                        var url = Url.Action("ReportPrinting", "SATReportOld",
                                        new
                                        {
                                            studentID = studentId,
                                            districtID = districtId,
                                            testID = virtualTestData.Key,
                                            teacherID = currentTeacher.Id,
                                            generatedUserID = CurrentUser.Id,
                                            classID = takenTestClassId,
                                            isLastTest = false,
                                            reportContentOption,
                                            takenReportTeacherId = teacherId.Value,
                                            takenReportClassId = classId.Value
                                        }, HelperExtensions.GetHTTPProtocal(Request));
                        listUrls.Add(url);

                        var pdf = ExportToPDF(url, timezoneOffset, studentId);
                        reportFileResultModel.PdfFiles.Add(pdf);
                        IncreaseGeneratedItemNo(specializedReportJobId);
                    }

                    reportfileResultModels.Add(reportFileResultModel);
                }
            }

            reportUrl = string.Join(",", listUrls);

            return reportfileResultModels;
        }

        private void IncreaseGeneratedItemNo(int specializedReportJobId)
        {
            var specializedReportJob = populateReportingService.GetSpecializedReportJob(specializedReportJobId);
            specializedReportJob.GeneratedItem++;
            populateReportingService.SaveSpecializedReportJob(specializedReportJob);
        }

        private byte[] PrintOneFile(List<int> listStudentIDs, int districtId, int testID, int teacherID, int timezoneOffset, int classID, out string reportUrl)
        {
            var url = Url.Action("ReportPrintingMultipleStudents", "SATReportOld",
                new
                {
                    studentIDs = string.Join(",", listStudentIDs.Select(x => x.ToString())),
                    districtID = districtId,
                    testID = testID,
                    teacherID = teacherID,
                    generatedUserID = CurrentUser.Id,
                    classID = classID
                }, HelperExtensions.GetHTTPProtocal(Request));
            reportUrl = url;
            var pdf = ExportToPDF(url, timezoneOffset, listStudentIDs, testID, teacherID, classID);
            return pdf;
        }

        private byte[] ExportToPDF(string url, int timezoneOffset, List<int> listStudentIDs, int testID, int teacherID, int classID)
        {
            List<string> listStudentName = new List<string>();
            List<string> listStudentCode = new List<string>();
            List<string> listPageNumber = new List<string>();
            foreach (var studentID in listStudentIDs)
            {
                var student = studentService.GetStudentById(studentID);
                var studentName = Util.FormatFullname(string.Format("{0}, {1}", student.LastName, student.FirstName));

                var masterModel = new SATReportMasterViewModel();
                BuildDiagnosticHistoryData(masterModel, studentID, testID, teacherID, classID);
                if (masterModel.DiagnosticHistoryViewModel.TestAndScoreViewModels.Count >=
                    Constanst.SATStudentReportMinAmountOfTestForChart)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        listStudentCode.Add("4");
                        listStudentName.Add(studentName);
                        listPageNumber.Add(string.Format("{0}", i + 1));
                    }
                }
                else
                {
                    for (int i = 0; i < 2; i++)
                    {
                        listStudentCode.Add("2");
                        listStudentName.Add(studentName);
                        listPageNumber.Add(string.Format("{0}", i + 1));
                    }
                }
            }

            var generatedUser = userService.GetUserById(CurrentUser.Id);
            string footerUrl = Url.Action("RenderFooter", "SATReportOld",
                new
                {
                    leftLine1 = DateTime.UtcNow.AddMinutes(timezoneOffset * (-1)).DisplayDateWithFormat(true),
                    //leftLine2 = string.Format("<div id='custom-footer-logon'><img src='{0}' style='position: relative; top: 5px;'><span>Copyright © 2014 | Powered by LinkIt!</span></div>", Url.Content("~/Content/images/loog-linkit-16x16.png")),
                    leftLine2 = string.Join("|", listPageNumber),
                    rightLine1 = string.Join("|", listStudentName),
                    rightLine2 = string.Join("|", listStudentCode)
                }, HelperExtensions.GetHTTPProtocal(Request));

            string headerUrl = Url.Action("RenderHeader", "SATReportOld", null, HelperExtensions.GetHTTPProtocal(Request));

            var pageSizeOption = "";
            if (!string.IsNullOrEmpty(_pageSize))
            {
                pageSizeOption = "--page-size " + _pageSize;
            }

            string args =
                string.Format(
                    pageSizeOption + " --footer-html \"{0}\" --header-html \"{2}\" --header-spacing 5 \"{1}\" - "
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

        private byte[] ExportToPDF(string url, int timezoneOffset, int studentID)
        {
            var student = studentService.GetStudentById(studentID);
            var studentName = Util.FormatFullname(string.Format("{0}, {1}", student.LastName, student.FirstName));

            var generatedUser = userService.GetUserById(CurrentUser.Id);
            string footerUrl = Url.Action("RenderFooter", "SATReportOld",
                new
                {
                    leftLine1 = DateTime.UtcNow.AddMinutes(timezoneOffset * (-1)).DisplayDateWithFormat(true),
                    //leftLine2 = string.Format("<div id='custom-footer-logon'><img src='{0}' style='position: relative; top: 5px;'><span>Copyright © 2014 | Powered by LinkIt!</span></div>", Url.Content("~/Content/images/loog-linkit-16x16.png")),
                    leftLine2 = string.Empty,
                    rightLine1 = studentName,
                    rightLine2 = string.Empty
                }, HelperExtensions.GetHTTPProtocal(Request));

            string headerUrl = Url.Action("RenderHeader", "SATReportOld", null, HelperExtensions.GetHTTPProtocal(Request));

            var pageSizeOption = "";
            if (!string.IsNullOrEmpty(_pageSize))
            {
                pageSizeOption = "--page-size " + _pageSize;
            }

            string args =
                string.Format(
                    pageSizeOption + " --footer-html \"{0}\" --header-html \"{2}\" --header-spacing 5 \"{1}\" - "
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
        public ActionResult RenderFooter(string leftLine1, string leftLine2, string rightLine1, string rightLine2)
        {
            var footer = new FooterData
            {
                LeftLine1 = leftLine1,
                LeftLine2 = leftLine2,
                RightLine1 = rightLine1,
                RightLine2 = rightLine2
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
                model.ClassId, model.ReportContentOption, out reportUrl);

            var stream = new MemoryStream(pdf);

            var folder = LinkitConfigurationManager.GetS3Settings().ACTReportFolder;
            var bucketName = LinkitConfigurationManager.GetS3Settings().ACTReportBucket;
            var result = _s3Service.UploadRubricFile(bucketName,
                folder + "/" + model.ActReportFileName, stream);

            return Json(new { IsSuccess = true, Url = reportUrl });
        }

        [HttpPost]
        [AjaxOnly]
        [AjaxAwareAuthorize]
        public ActionResult GenerateNewSAT(NewACTReportData model)
        {
            if (model.DistrictId.HasValue == false)
            {
                model.DistrictId = CurrentUser.DistrictId.GetValueOrDefault();
            }

            var listStudentId = new List<int>();
            foreach (var studentIdString in model.StudentIdList)
            {
                int studentId;
                if (int.TryParse(studentIdString, out studentId))
                {
                    listStudentId.Add(studentId);
                }
            }

            var reportUrl = "";


            var reportFileResultModels = PrintOneFileNewSAT(model.DistrictId.Value, model.SchoolId, model.TeacherId, model.DistrictTermId,
                model.ClassId
                , model.StrTestIdList.Split(',').Select(x => Convert.ToInt32(x)).ToList()
                , listStudentId
                , model.ResultDateFrom, model.ResultDateTo
                , model.SpecializedReportJobId
                , model.TimezoneOffset, model.ReportContentOption, model.isGetAllClass, out reportUrl);

            var s3Url = CreateNewSATFileAndUploadToS3(model, reportFileResultModels);
            CompletedSpecializedReportJob(model.SpecializedReportJobId, s3Url);
            return Json(new { IsSuccess = true, Url = reportUrl });
        }

        private string CreateNewSATFileAndUploadToS3(NewACTReportData model, List<ReportFileResultModel> reportFileResultModels)
        {
            var folder = ConfigurationManager.AppSettings["ACTReportFolder"];
            var bucketName = LinkitConfigurationManager.GetS3Settings().ACTReportBucket;

            var rawFileName = model.ActReportFileName.Substring(0, model.ActReportFileName.LastIndexOf("_"));
            var randomKey = model.ActReportFileName.Substring(model.ActReportFileName.LastIndexOf("_") + 1);

            var specializedReportTestResultPerFileConfiguration =
                GetSpecializedReportTestResultPerFileConfiguration(model.DistrictId.GetValueOrDefault());

            if (reportFileResultModels.Count == 1 &&
                reportFileResultModels[0].PdfFiles.Count <= specializedReportTestResultPerFileConfiguration)
            {
                model.ActReportFileName = string.Format("{0}_{1}_{2}_{3}.pdf", rawFileName,
                    reportFileResultModels[0].TeacherLastName, reportFileResultModels[0].ClassName, randomKey).Replace("/", "_").Replace("\\", "_");

                var file = PdfHelper.MergeFilesAddBlankToOddFile(reportFileResultModels[0].PdfFiles);
                var stream = new MemoryStream(file);
                var result = _s3Service.UploadRubricFile(bucketName, folder + "/" + model.ActReportFileName, stream);
            }
            else
            {
                var zipStream = new MemoryStream();
                var zipFile = new ZipFile();

                foreach (var reportFileResultModel in reportFileResultModels)
                {
                    var randomString = Guid.NewGuid().ToString().Substring(0, 6);
                    var numOfFile = reportFileResultModel.PdfFiles.Count / specializedReportTestResultPerFileConfiguration;
                    if (reportFileResultModel.PdfFiles.Count % specializedReportTestResultPerFileConfiguration > 0)
                        numOfFile++;

                    for (int i = 0; i < numOfFile; i++)
                    {
                        var onePdfFiles = new List<byte[]>();
                        for (int j = 0; j < specializedReportTestResultPerFileConfiguration; j++)
                        {
                            if (i * specializedReportTestResultPerFileConfiguration + j < reportFileResultModel.PdfFiles.Count)
                            {
                                onePdfFiles.Add(
                                    reportFileResultModel.PdfFiles[i * specializedReportTestResultPerFileConfiguration + j]);
                            }
                        }
                        var file = PdfHelper.MergeFilesAddBlankToOddFile(onePdfFiles);
                        zipFile.AddEntry(
                            string.Format("{0}_{1}_{2}_{3}.pdf", rawFileName,
                                reportFileResultModel.TeacherLastName, reportFileResultModel.ClassName + "_" + randomString, (i + 1)).Replace("/", "_").Replace("\\", "_"),
                            file);
                    }
                }

                zipFile.Save(zipStream);
                zipFile.Dispose();
                zipStream.Position = 0;

                model.ActReportFileName = model.ActReportFileName + ".zip";
                var result = _s3Service.UploadRubricFile(bucketName,
                    folder + "/" + model.ActReportFileName, zipStream);
            }

            var s3Url = string.Format("https://s3.amazonaws.com/{0}/{1}/{2}", bucketName, folder, model.ActReportFileName.CorrectFileNameOnURL());
            return s3Url;
        }



        private void CompletedSpecializedReportJob(int specializedReportJobId, string downloadUrl)
        {
            var specializedReportJob = populateReportingService.GetSpecializedReportJob(specializedReportJobId);
            specializedReportJob.DownloadUrl = downloadUrl;
            specializedReportJob.Status = 1;
            populateReportingService.SaveSpecializedReportJob(specializedReportJob);
        }

        public ActionResult CheckGenerateNewSATRequest(NewACTReportData model)
        {
            if (model.DistrictId.HasValue == false)
            {
                model.DistrictId = CurrentUser.DistrictId.GetValueOrDefault();
            }

            var listStudentId = new List<int>();
            foreach (var studentIdString in model.StudentIdList)
            {
                int studentId;
                if (int.TryParse(studentIdString, out studentId))
                {
                    listStudentId.Add(studentId);
                }
            }

            var testResultTotalItem = CountNewSATTotalItem(model.DistrictId.Value, model.SchoolId, model.TeacherId,
                model.DistrictTermId,
                model.ClassId
                , model.StrTestIdList.Split(',').Select(x => Convert.ToInt32(x)).ToList()
                , listStudentId
                , model.ResultDateFrom, model.ResultDateTo, model.isGetAllClass);
            var specializedReportMaxTestResultConfiguration =
                GetSpecializedReportMaxTestResultConfiguration(model.DistrictId.GetValueOrDefault());

            if (testResultTotalItem > specializedReportMaxTestResultConfiguration)
            {
                return
                    Json(
                        new
                        {
                            Result = false,
                            SubmittedTestResult = testResultTotalItem,
                            MaxTestResult = specializedReportMaxTestResultConfiguration
                        });
            }

            var specializedReportJob = new SpecializedReportJob
            {
                CreatedDate = DateTime.UtcNow,
                DownloadUrl = "",
                GeneratedItem = 0,
                Status = 0,
                TotalItem = testResultTotalItem,
                UserId = CurrentUser.Id,
                DistrictId = model.DistrictId.Value
            };

            populateReportingService.SaveSpecializedReportJob(specializedReportJob);

            return Json(new { Result = true, SpecializedReportJobId = specializedReportJob.SpecializedReportJobId });
        }

        private int GetSpecializedReportMaxTestResultConfiguration(int districtId)
        {
            var specializedReportMaxTestResultConfiguration = 0;
            var districtDecode = districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(districtId,
                DistrictDecodeLabelConstant.SpecializedReport_MaxTestResult).FirstOrDefault();
            if (districtDecode != null)
            {
                specializedReportMaxTestResultConfiguration = Convert.ToInt32(districtDecode.Value);
            }
            else
            {
                var configuration =
                    configurationService.GetConfigurationByKey(ConfigurationNameConstant.SpecializedReport_MaxTestResult);
                if (configuration != null)
                {
                    specializedReportMaxTestResultConfiguration = Convert.ToInt32(configuration.Value);
                }
            }

            return specializedReportMaxTestResultConfiguration;
        }

        private int GetSpecializedReportTestResultPerFileConfiguration(int districtId)
        {
            var specializedReportTestResultPerFileConfiguration = 0;
            var districtDecode = districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(districtId,
                DistrictDecodeLabelConstant.SpecializedReport_TestResultPerFile).FirstOrDefault();
            if (districtDecode != null)
            {
                specializedReportTestResultPerFileConfiguration = Convert.ToInt32(districtDecode.Value);
            }
            else
            {
                var configuration =
                    configurationService.GetConfigurationByKey(ConfigurationNameConstant.SpecializedReport_TestResultPerFile);
                if (configuration != null)
                {
                    specializedReportTestResultPerFileConfiguration = Convert.ToInt32(configuration.Value);
                }
            }

            return specializedReportTestResultPerFileConfiguration;
        }

        private int CountNewSATTotalItem(int districtId, int? schoolId, int? teacherId, int? districtTermId, int? classId
            , List<int> listVirtualTestIds, List<int> listStudentIds
            , DateTime? resultDateFrom, DateTime? resultDateTo, bool? isGetAllClass)
        {
            if (isGetAllClass.HasValue && isGetAllClass.Value == true)
            {
                return CountNewSATTotalItemWithGetAllClassOption(districtId, schoolId, teacherId, districtTermId, classId
                    , listVirtualTestIds, listStudentIds, resultDateFrom, resultDateTo);
            }
            else
            {
                return CountNewSATTotalItemWithoutGetAllClassOption(districtId, schoolId, teacherId, districtTermId, classId
                    , listVirtualTestIds, listStudentIds, resultDateFrom, resultDateTo);
            }
        }

        private int CountNewSATTotalItemWithoutGetAllClassOption(int districtId, int? schoolId, int? teacherId, int? districtTermId, int? classId
            , List<int> listVirtualTestIds, List<int> listStudentIds
            , DateTime? resultDateFrom, DateTime? resultDateTo)
        {
            var counter = 0;
            var virtualTestIdString = string.Join(",", listVirtualTestIds);
            var virtualTestSubTypeIdString = "7";

            var listSchoolIds = new List<int>();
            if (schoolId.HasValue)
                listSchoolIds.Add(schoolId.Value);
            else
            {
                listSchoolIds.AddRange(NewSatGetListSchoolIds(districtId, virtualTestIdString,
                    virtualTestSubTypeIdString, resultDateFrom, resultDateTo));
            }
            var listStudentHasReport = new List<string>();
            foreach (var listSchoolId in listSchoolIds)
            {
                var listTeacherIds = new List<int>();

                if (teacherId.HasValue)
                    listTeacherIds.Add(teacherId.Value);
                else
                {
                    listTeacherIds.AddRange(NewSatGetListTeacherIds(districtId, listSchoolId, virtualTestIdString,
                        virtualTestSubTypeIdString, resultDateFrom, resultDateTo));
                }

                foreach (var listTeacherId in listTeacherIds)
                {
                    var listClassIds = new List<int>();

                    if (classId.HasValue)
                        listClassIds.Add(classId.Value);
                    else
                    {
                        listClassIds.AddRange(NewSatGetListClassIds(districtId, listSchoolId, listTeacherId, districtTermId,
                            virtualTestIdString,
                            virtualTestSubTypeIdString, resultDateFrom, resultDateTo));
                    }

                    foreach (var listClassId in listClassIds)
                    {
                        foreach (var virtualTestId in listVirtualTestIds)
                        {
                            var studentIds = MultipleTestGetStudents(listClassId, virtualTestId.ToString(), resultDateFrom,
                                    resultDateTo, false).OrderBy(x => x.FullName).Select(x => x.StudentId).ToList();

                            if (listStudentIds != null && listStudentIds.Any())
                            {
                                studentIds = studentIds.Where(listStudentIds.Contains).ToList();
                            }

                            var count = studentIds.Count(x => !listStudentHasReport.Contains(x + ";" + virtualTestId));
                            listStudentHasReport.AddRange(
                                studentIds.Where(x => !listStudentHasReport.Contains(x + ";" + virtualTestId))
                                    .Select(x => x + ";" + virtualTestId)
                                    .ToList());

                            counter += count;
                        }
                    }
                }
            }

            return counter;
        }

        private int CountNewSATTotalItemWithGetAllClassOption(int districtId, int? schoolId, int? teacherId, int? districtTermId, int? classId
            , List<int> listVirtualTestIds, List<int> listStudentIds
            , DateTime? resultDateFrom, DateTime? resultDateTo)
        {
            var counter = 0;

            Dictionary<int, Dictionary<int, List<int>>> sumClassTestStudents = new Dictionary<int, Dictionary<int, List<int>>>();

            var virtualTestIdString = string.Join(",", listVirtualTestIds);
            var virtualTestSubTypeIdString = "7";

            var listUrls = new List<string>();

            var listSchoolIds = new List<int>();

            if (schoolId.HasValue)
            {
                listSchoolIds.Add(schoolId.Value);
            }
            else
            {
                listSchoolIds = populateReportingService.ReportingGetSchools(CurrentUser.Id, CurrentUser.RoleId,
                    districtId, virtualTestIdString, virtualTestSubTypeIdString, resultDateFrom, resultDateTo, isGetAllClass: true)
                    .Select(x => x.TakenTestSchoolId).Distinct().ToList();
            }

            var listStudentHasReport = new List<string>();
            foreach (var listSchoolId in listSchoolIds)
            {
                var listTeacherIds = new List<int>();

                if (teacherId.HasValue)
                {
                    listTeacherIds.Add(teacherId.Value);
                }
                else
                {
                    listTeacherIds = populateReportingService.ReportingGetTeachers(CurrentUser.Id, CurrentUser.RoleId,
                        districtId, listSchoolId, virtualTestIdString, virtualTestSubTypeIdString, resultDateFrom, resultDateTo, isGetAllClass: true)
                        .OrderBy(x => x.NameLast).ThenBy(x => x.NameFirst)
                        .Select(x => x.TakenTestTeacherId).Distinct().ToList();
                }

                foreach (var listTeacherId in listTeacherIds)
                {
                    var listClassIds = new List<int>();

                    if (classId.HasValue)
                    {
                        listClassIds.Add(classId.Value);
                    }
                    else
                    {
                        populateReportingService.ReportingGetClasses(CurrentUser.Id, CurrentUser.RoleId, districtId, listSchoolId, listTeacherId
                            , districtTermId, virtualTestIdString, virtualTestSubTypeIdString, resultDateFrom, resultDateTo, isGetAllClass: true)
                            .Select(x => x.TakenTestClassId).Distinct().ToList();
                    }

                    foreach (var listClassId in listClassIds)
                    {
                        foreach (var virtualTestId in listVirtualTestIds)
                        {
                            var listReportStudentIds = new List<int>();

                            var studentTestDistrictTerms = teacherDistrictTermService.GetStudentTestDistrictTerm(null, null, null, null
                                , classId, new List<int>() { virtualTestId }, null, resultDateFrom, resultDateTo);

                            if (listStudentIds != null && listStudentIds.Any())
                            {
                                studentTestDistrictTerms = studentTestDistrictTerms.Where(x => listStudentIds.Contains(x.StudentId));
                            }

                            foreach (var student in studentTestDistrictTerms)
                            {
                                if (!sumClassTestStudents.ContainsKey(student.TakenTestClassId))
                                {
                                    sumClassTestStudents.Add(student.TakenTestClassId, new Dictionary<int, List<int>>());
                                }

                                if (!sumClassTestStudents[student.TakenTestClassId].ContainsKey(student.VirtualTestId))
                                {
                                    sumClassTestStudents[student.TakenTestClassId].Add(student.VirtualTestId, new List<int>());
                                }

                                if (!sumClassTestStudents[student.TakenTestClassId][student.VirtualTestId].Contains(student.StudentId))
                                {
                                    sumClassTestStudents[student.TakenTestClassId][student.VirtualTestId].Add(student.StudentId);
                                    counter++;
                                }
                            }
                        }
                    }
                }
            }

            return counter;
        }

        private IEnumerable<int> NewSatGetListSchoolIds(int districtId, string virtualTestIdString, string virtualTestSubTypeIds,
            DateTime? resultDateFrom, DateTime? resultDateTo)
        {
            var teacherTestDistrictTerms = populateReportingService.ReportingGetSchools(CurrentUser.Id,
                CurrentUser.RoleId,
                districtId, virtualTestIdString, virtualTestSubTypeIds, resultDateFrom, resultDateTo);

            var data = teacherTestDistrictTerms
                .Select(x => new { x.SchoolId, x.SchoolName })
                .Distinct()
                .OrderBy(x => x.SchoolName)
                .Select(x => x.SchoolId.GetValueOrDefault())
                .ToList();

            return data;
        }

        private IEnumerable<int> NewSatGetListTeacherIds(int districtId, int schoolId, string virtualtestIdString, string virtualTestSubTypeIdString, DateTime? resultDateFrom, DateTime? resultDateTo)
        {
            var teacherTestDistrictTerms = populateReportingService.ReportingGetTeachers(CurrentUser.Id,
                CurrentUser.RoleId,
                districtId, schoolId, virtualtestIdString, virtualTestSubTypeIdString, resultDateFrom, resultDateTo);

            var data = teacherTestDistrictTerms
                .Select(x => new { x.UserId, x.UserName, x.NameLast, x.NameFirst })
                .Distinct()
                .OrderBy(x => x.NameLast).ThenBy(x => x.NameFirst)
                .Select(x => x.UserId)
                .ToList();

            return data;
        }

        private IEnumerable<int> NewSatGetListTermIds(int districtId, int schoolId, int userId, string virtualTestIdString,
            string virtualTestSubTypeIdString, DateTime? resultDateFrom, DateTime? resultDateTo)
        {
            var teacherTestDistrictTerms = populateReportingService.ReportingGetTerms(CurrentUser.Id,
                CurrentUser.RoleId,
                districtId, schoolId, userId, virtualTestIdString, virtualTestSubTypeIdString, resultDateFrom, resultDateTo);

            var data = teacherTestDistrictTerms
                .Select(x => new { x.DistrictTermId, x.DistrictTermName, x.DateStart })
                .Distinct()
                .OrderByDescending(x => x.DateStart)
                .Select(x => x.DistrictTermId)
                .ToList();

            return data;
        }

        private IEnumerable<int> NewSatGetListClassIds(int districtId, int schoolId, int teacherId, int? termId, string virtualTestIdString,
            string virtualTestSubTypeIdString, DateTime? resultDateFrom, DateTime? resultDateTo)
        {
            var teacherTestDistrictTerms = populateReportingService.ReportingGetClasses(CurrentUser.Id,
                CurrentUser.RoleId,
                districtId, schoolId, teacherId, termId, virtualTestIdString, virtualTestSubTypeIdString, resultDateFrom, resultDateTo);

            var data = teacherTestDistrictTerms
                .Select(x => new { x.ClassId, x.ClassName })
                .Distinct()
                .OrderBy(x => x.ClassName)
                .Select(x => x.ClassId);

            return data;
        }

        [HttpPost]
        public ActionResult CheckS3FileExisted(string fileName)
        {
            var folder = ConfigurationManager.AppSettings["ACTReportFolder"];
            var bucketName = LinkitConfigurationManager.GetS3Settings().ACTReportBucket;

            var result = _s3Service.DownloadFile(bucketName, folder + "/" + fileName);

            if (result.IsSuccess)
            {
                //var s3Url = string.Format("https://s3.amazonaws.com/{0}/{1}/{2}", bucketName, folder, fileName);
                var s3Url = _s3Service.GetPublicUrl(bucketName, folder + "/" + fileName);
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

        //[CacheControl(HttpCacheability.NoCache), HttpGet]
        //[AjaxAwareAuthorize]
        //public ActionResult SATGetClasses(int? schoolId, int termId, int? userId, int virtualTestId)
        //{
        //    var vUserId = userId;
        //    if (vUserId == null && CurrentUser.IsTeacher)
        //        vUserId = CurrentUser.Id;

        //    //var teacher = userService.GetUserById(vUserId);
        //    //if (teacher.IsNull())
        //    //{
        //    //    return Json(new List<ListItem>().AsQueryable(), JsonRequestBehavior.AllowGet);
        //    //}

        //    List<int> lstClassId = testResultService.GetTestResultsByVirtualTestId(virtualTestId)
        //        .Select(o => o.ClassId).Distinct().ToList();
        //    //TODO: should check lstTeacher < 2100 ^_^

        //    // Filter class by UserId
        //    if (vUserId.HasValue)
        //    {
        //        var classUsers = classUserService.GetClassUsersByUserId(vUserId.Value)
        //            .Where(o => lstClassId.Contains(o.ClassId));

        //        lstClassId = classUsers.Select(x => x.ClassId).Distinct().ToList();
        //    }

        //    var classItems = new List<ListItem>();
        //    foreach (var classId in lstClassId)
        //    {
        //        var singleClass = classService.GetClassesByDistrictTermIdAndClassId(termId, classId);
        //        if (singleClass != null && (!schoolId.HasValue || singleClass.SchoolId == schoolId))
        //        {
        //            classItems.Add(new ListItem {Id = singleClass.Id, Name = singleClass.Name});
        //        }
        //    }
        //    classItems = classItems.OrderBy(x => x.Name).ToList();
        //    return Json(classItems.AsQueryable(), JsonRequestBehavior.AllowGet);
        //}

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

        [CacheControl(HttpCacheability.NoCache), HttpGet]
        [AjaxAwareAuthorize]
        public ActionResult MultipleTestSATGetStudents(int classId, string virtualTestIdString, DateTime? resultDateFrom,
            DateTime? resultDateTo, bool? isGetAllClass)
        {
            var classStudents = MultipleTestGetStudents(classId, virtualTestIdString, resultDateFrom, resultDateTo, isGetAllClass ?? false);

            var data = classStudents.Select(x => new { x.StudentId, x.FullName }).OrderBy(x => x.FullName);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private List<ClassStudent> MultipleTestGetStudents(int classId, string virtualTestIdString, DateTime? resultDateFrom,
            DateTime? resultDateTo, bool isGetAllClasses)
        {
            var virtualTestIds = (virtualTestIdString ?? "").Split(',').Select(x => Convert.ToInt32(x)).ToList();

            var lstStudentIds = new List<int>();

            if (!isGetAllClasses)
            {
                lstStudentIds =
                testResultService.GetTestResultsByVirtualTestIds(virtualTestIds, resultDateFrom, resultDateTo)
                    .Where(o => o.ClassId == classId)
                    .Select(o => o.StudentId).Distinct().ToList();
                //TODO: should check lstTeacher < 2100 ^_^
            }
            else
            {
                lstStudentIds = teacherDistrictTermService.GetStudentTestDistrictTerm(
                    null, null, null, null, classId, virtualTestIds, null, resultDateFrom, resultDateTo).Select(x => x.StudentId).Distinct().ToList();
            }

            var data = classStudentService.GetClassStudentsByClassId(classId)
                .Where(o => lstStudentIds.Contains(o.StudentId))
                .ToList();

            return data;
        }

        [HttpGet]
        public ActionResult GetSchools(int? districtId, int? virtualtestId)
        {
            if (!districtId.HasValue)
            {
                districtId = CurrentUser.DistrictId;
            }

            var data = teacherDistrictTermService.GetTeacherTestDistrictTerm(districtId,
                    null, null, null, virtualtestId, (int)VirtualTestSubType.SAT).ToList()
                    .Select(x => new { x.SchoolId, x.SchoolName })
                    .Distinct()
                    .OrderBy(x => x.SchoolName)
                    .Select(x => new ListItem { Id = x.SchoolId.GetValueOrDefault(), Name = x.SchoolName })
                    .ToList();

            if (CurrentUser.IsTeacher)
            {
                // Return access schools only
                var accessSchools = userSchoolService.GetSchoolsUserHasAccessTo(CurrentUser.Id);
                data = data.Where(x => accessSchools.Any(s => s.SchoolId == x.Id)).ToList();
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetTeachers(int? districtId, int? schoolId, int? virtualtestId)
        {
            var data = teacherDistrictTermService.GetTeacherTestDistrictTerm(districtId,
                schoolId, null, null, virtualtestId, (int)VirtualTestSubType.SAT).ToList()
                .Select(x => new { x.UserId, x.UserName, x.NameLast, x.NameFirst })
                .Distinct()
                .OrderBy(x => x.NameLast).ThenBy(x => x.NameFirst)
                .Select(x => new
                {
                    Id = x.UserId,
                    Name = x.UserName,
                    FirstName = x.NameFirst,
                    LastName = x.NameLast
                })
                .ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetTerms(int? districtId, int? schoolId, int? userId, int? virtualTestId)
        {
            var data = teacherDistrictTermService.GetTeacherTestDistrictTerm(districtId,
                    schoolId, userId, null, virtualTestId, (int)VirtualTestSubType.SAT).ToList()
                    .Select(x => new { x.DistrictTermId, x.DistrictTermName, x.DateStart })
                    .Distinct()
                    .OrderByDescending(x => x.DateStart)
                    .Select(x => new ListItem { Id = x.DistrictTermId, Name = x.DistrictTermName })
                    .ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetClasses(int? districtId, int? schoolId, int? userId, int? termId, int? virtualTestId)
        {
            if (!userId.HasValue && CurrentUser.IsTeacher)
                userId = CurrentUser.Id;

            var data = teacherDistrictTermService.GetTeacherTestDistrictTerm(
                districtId, schoolId, userId, termId, virtualTestId, (int)VirtualTestSubType.SAT).ToList()
                .Select(x => new { x.ClassId, x.ClassName })
                .Distinct()
                .OrderBy(x => x.ClassName)
                .Select(x => new ListItem { Id = x.ClassId, Name = x.ClassName });

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion

        private SATReportMasterViewModel BuildReportMasterModel(int studentID, int districtID, int testID, int teacherID, int generatedUserID, int classID, bool isLastTest, int reportContentOption, int? takenReportTeacherId, int? takenReportClassId)
        {
            var masterModel = new SATReportMasterViewModel
            {
                SectionTagViewModels = new List<SectionTagViewModel>(),
                ReportContentOption = reportContentOption,
                DistrictId = districtID
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

            masterModel.VirtualTestSubTypeId = virtualTestService.GetTestById(testID).VirtualTestSubTypeID.GetValueOrDefault();
            masterModel.TagTableUseAlternativeStyle = GetTagTableUseAlternativeStyleOption(districtID);

            BuildDiagnosticHistoryData(masterModel, studentID, testID, teacherID, classID);

            BuildSessionTagData(masterModel, studentID, testID);

            BuildStudentInformationData(masterModel, studentID, generatedUserID, takenReportTeacherId, takenReportClassId);

            BuildSummaryScoreData(masterModel);

            ReorderSubjectNameForDiagnosticAndSummary(masterModel, districtID);

            BuildAdditionalFormatOption(masterModel);

            BuildOptionBoldZeroPercentScore(masterModel);

            masterModel.IsLastReportInList = isLastTest;
            masterModel.ListVirtualTestConversionName =
                virtualTestConversionService.GetByVirtualTestId(testID).Select(x => x.Name).ToList();

            BuildEssayCommentData(masterModel);

            return masterModel;
        }

        private bool GetTagTableUseAlternativeStyleOption(int districtId)
        {
            var result = false;
            var districtDecode =
                districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(
                    districtId, Util.SATReport_TagTable_UseAlternativeStyle)
                    .FirstOrDefault();
            if (districtDecode != null)
                result = districtDecode.Value == "true";

            return result;
        }

        private void BuildEssayCommentData(SATReportMasterViewModel model)
        {
            model.NewSATEssayComments = new List<NewSATEssayComment>();

            if (model.VirtualTestSubTypeId != (int)VirtualTestSubType.NewSAT)
                return;

            var districtDecode =
                districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(model.DistrictId,
                    Util.NewSATReport_ShowComment).FirstOrDefault();
            if (districtDecode == null || districtDecode.Value != "1")
                return;

            districtDecode =
                districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(model.DistrictId,
                    Util.NewSATEssayComment_Title).FirstOrDefault();
            if (districtDecode != null)
            {
                model.EssayCommentTitle = districtDecode.Value;
            }

            var testAndScoreViewModel =
                model.DiagnosticHistoryViewModel.TestAndScoreViewModels.FirstOrDefault(x => x.IsSelected);
            var domainTagDataOfTestResult =
                model.DomainTagData.Where(x => x.TestResultID == testAndScoreViewModel.TestResultID)
                    .OrderBy(x => x.MinQuestionOrder)
                    .ToList();


            var domainTagNames = GetDomainTagNamesOfSelectedTest(model);
            for (int i = 0; i < domainTagNames.Count; i++)
            {
                var tagName = domainTagNames[i];
                var domainTag = domainTagDataOfTestResult.FirstOrDefault(x => x.TagName.ToLower() == tagName.ToLower());

                model.NewSATEssayComments.Add(SetEssayCommentByTagIndex(model.DistrictId, domainTag, i));
            }
        }

        private List<string> GetDomainTagNamesOfSelectedTest(SATReportMasterViewModel model)
        {
            var domainTagNames = new List<string>();
            if (model.DomainTagData.Any())
            {
                var testResultId = model.DiagnosticHistoryViewModel.TestAndScoreViewModels.FirstOrDefault(x => x.IsSelected).TestResultID;
                domainTagNames = model.DomainTagData.Where(x => x.TestResultID == testResultId)
                    .Select(x => new { x.TagName, x.MinQuestionOrder })
                    .Distinct().OrderBy(x => x.MinQuestionOrder).Select(x => x.TagName).ToList();
            }

            return domainTagNames;
        }

        private NewSATEssayComment SetEssayCommentByTagIndex(int districtId, ACTSectionTagData sectionTagData, int tagIndex)
        {
            var essayComment = new NewSATEssayComment
            {
                Score = sectionTagData.CorrectAnswer,
                TagName = sectionTagData.TagName,
            };

            var districtDecode =
                districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(districtId,
                    string.Format(Util.NewSATEssayComment_ScoreRangeKey, tagIndex)).FirstOrDefault();
            if (districtDecode != null)
            {
                essayComment.ScoreRange = districtDecode.Value;
            }

            districtDecode =
                districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(districtId,
                    string.Format(Util.NewSATEssayComment_ScoreKey, tagIndex, sectionTagData.CorrectAnswer))
                    .FirstOrDefault();
            if (districtDecode != null)
            {
                essayComment.EssayComment = districtDecode.Value;
            }

            return essayComment;
        }

        private void BuildAdditionalFormatOption(SATReportMasterViewModel masterModel)
        {
            var districtDecodeLabel = masterModel.VirtualTestSubTypeId == (int)VirtualTestSubType.SAT
                ? Constanst.SATReportShowTableBorder
                : Constanst.NewSATReportShowTableBorder;
            masterModel.ShowTableBorder = districtDecodeService.GetDistrictDecodeByLabel(masterModel.DistrictId,
                districtDecodeLabel);
        }

        private void BuildOptionBoldZeroPercentScore(SATReportMasterViewModel masterModel)
        {
            var districtDecodeLabel = masterModel.VirtualTestSubTypeId == (int)VirtualTestSubType.SAT
                ? Constanst.SATReportBoldZeroPercentScore
                : Constanst.NewSATReportBoldZeroPercentScore;
            masterModel.BoldZeroPercentScore = districtDecodeService.GetDistrictDecodeByLabel(masterModel.DistrictId,
                districtDecodeLabel);
        }

        private void ReorderSubjectNameForDiagnosticAndSummary(SATReportMasterViewModel masterModel, int districtID)
        {
            var subTypeId = masterModel.VirtualTestSubTypeId;
            var subScoreList =
                masterModel.DiagnosticHistoryViewModel.TestAndScoreViewModels.Select(
                    x => x.SubScores.Select(y => y.SectionName).ToList()).SelectMany(x => x).Distinct().ToList();
            var orderedSubScoreList = OrderSubjectByDistrictDecodeSetting(districtID, subScoreList, subTypeId);

            ReorderSummaryScores(masterModel, orderedSubScoreList);
            ReorderDiagnosticHistory(masterModel, orderedSubScoreList);

            subScoreList = masterModel.SectionTagViewModels.Select(x => x.SectionName).ToList();
            orderedSubScoreList = OrderSubjectByDistrictDecodeSetting(districtID, subScoreList, subTypeId);
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

        private List<string> OrderSubjectByDistrictDecodeSetting(int districtID, List<string> subScoreList, int virtualTestSubTypeId)
        {
            var configKey = virtualTestSubTypeId == (int)VirtualTestSubType.NewSAT
                ? SATConstants.NewSATReportSubjectOrderLabel
                : SATConstants.SATReportSubjectOrderLabel;
            var orderedSubScoreList = new List<string>();
            var subjectOrderSetting =
                districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(
                    districtID, configKey).FirstOrDefault();
            if (subjectOrderSetting != null && !string.IsNullOrEmpty(subjectOrderSetting.Value))
            {
                string[] subjectOrderArray = subjectOrderSetting.Value.Split(new[] { ';' },
                    StringSplitOptions.RemoveEmptyEntries);
                foreach (var subject in subjectOrderArray)
                {
                    if (string.IsNullOrEmpty(subject)) continue; if (virtualTestSubTypeId == (int)VirtualTestSubType.NewSAT)
                    {
                        var subscoreItem = subScoreList.Where(x => !string.IsNullOrEmpty(x) && x.ToLower().Equals(subject.ToLower())
                                                                   && !x.ToLower().Equals(SATConstants.CompositeSubjectName.ToLower())).ToList();
                        if (subscoreItem.Any())
                        {
                            orderedSubScoreList.AddRange(subscoreItem);
                        }
                        else if (subject == SATConstants.CompositeSubjectName || subject == SATConstants.EssaySubScoreName || subject == SATConstants.MultipleChoiceWritingSubjectName)
                        {
                            orderedSubScoreList.Add(subject);
                        }
                    }
                    else
                    {
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
                    if (compositeIndex >= 0)
                        orderedSubScoreList.InsertRange(compositeIndex, listSubjectNotAdded);
                }
            }
            return orderedSubScoreList;
        }

        public ActionResult ReportPrintingForCoding()
        {
            Util.LoadDateFormatToCookies(272, districtDecodeService);

            var masterModel = BuildReportMasterModel(313562, 272, 54081, 325043, 327671, 39687, false, 1, 325043, 573717);
            return View("ReportPrinting", masterModel);
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

    class TextEntryRangeObject
    {
        public string StartValue { get; set; }
        public string EndValue { get; set; }
        public bool IsStartExclusived { get; set; }
        public bool IsEndExclusived { get; set; }
    }
}
