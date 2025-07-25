using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using FluentValidation;
using LinkIt.BubbleSheetPortal.Common;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.ACTReport;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.BubbleSheetAws;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using LinkIt.BubbleSheetPortal.Web.ViewModels.ACTReport;
using S3Library;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class KnowsysReportController : BaseController
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
        private readonly BubbleSheetFileService bubbleSheetFileService;
        private readonly DistrictDecodeService districtDecodeService;

        public KnowsysReportController(ACTReportService actReportService, UserService userService,
            SchoolService schoolService, UserSchoolService userSchoolService,
            TestResultService testResultService,
            TeacherDistrictTermService teacherDistrictTermService,
            ClassUserService classUserService,
            ClassService classService,
            ClassStudentService classStudentService, DistrictService districtService,
            StudentService studentService,
            VirtualTestService virtualTestService, IValidator<ACTReportData> actReportDataValidator,
            BubbleSheetFileService bubbleSheetFileService,
            DistrictDecodeService districtDecodeService, IS3Service s3Service)
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
            this.bubbleSheetFileService = bubbleSheetFileService;
            this.districtDecodeService = districtDecodeService;
            _s3Service = s3Service;
        }

        private void BuildStudentInformationData(ACTReportMasterViewModel masterModel, int studentID,
                                                 int generatedUserId)
        {
            var testResultID =
                masterModel.DiagnosticHistoryViewModel.TestAndScoreViewModels.FirstOrDefault(en => en.IsSelected).
                    TestResultID;

            var studentInfo = actReportService.GetACTStudentInformation(studentID, testResultID);

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

        private void BuildDiagnosticHistoryData(ACTReportMasterViewModel masterModel, int studentID, int testID,
                                                int teacherID)
        {
            masterModel.DiagnosticHistoryViewModel = new DiagnosticHistoryViewModel
            {
                TestAndScoreViewModels = new List<TestAndScoreViewModel>()
            };

            var diagnotisHistoryData = actReportService.GetACTTestHistoryData(studentID, masterModel.VirtualTestSubTypeId);
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
                    diagnotisHistoryData.Where(en => en.TeacherID == teacherID && en.VirtualTestID == testID).
                        OrderByDescending(en => en.UpdatedDate).FirstOrDefault();
                var selectedTestResultId = selectedTestResult == null ? 0 : selectedTestResult.TestResultID;

                foreach (var testResultData in listTestResult)
                {
                    var data = diagnotisHistoryData.Where(x => x.TestResultID == testResultData.TestResultID).ToList();
                    var englishScore = GetSectionScoreBySectionName("english", data);
                    var mathScore = GetSectionScoreBySectionName("math", data);
                    var readingScore = GetSectionScoreBySectionName("reading", data);
                    var scienceScore = GetSectionScoreBySectionName("science", data);
                    var writingScore = GetSectionScoreBySectionName("writing", data);
                    var ewScore = GetSectionScoreBySectionName("e/w", data);
                    var compositeScore = data.First().CompositeScore;
                    var testScore = new TestAndScoreViewModel
                    {
                        TestResultID = testResultData.TestResultID,
                        TestDate = testResultData.UpdatedDate,
                        EnglishScore = englishScore,
                        EnglishWritingScore = ewScore,
                        MathScore = mathScore,
                        ReadingScore = readingScore,
                        ScienceScore = scienceScore,
                        WritingScore = writingScore,
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

        private void BuildSessionTagData(ACTReportMasterViewModel masterModel, int studentID)
        {
            var sectionTagData = actReportService.GetACTSectionTagData(studentID, masterModel.VirtualTestSubTypeId);

            //get latest virtual test
            var testResultID =
                masterModel.DiagnosticHistoryViewModel.TestAndScoreViewModels.FirstOrDefault(en => en.IsSelected).
                    TestResultID;
            var latestSectionTag = sectionTagData.Where(x => x.TestResultID == testResultID).ToList();
            var listSection = latestSectionTag.Select(x => x.SectionID).Distinct().ToList();

            //Get Lastest essay pages
            var bubbleSheetId = testResultService.GetTestResultById(testResultID).BubbleSheetId;
            var bubbleSheetFileSubs = bubbleSheetFileService.GetLastestBubbleSheetFileSubByBubbleSheetId(bubbleSheetId).Where(x => x.PageNumber == 3 || x.PageNumber == 4 || x.PageNumber == 5 || x.PageNumber == 6).OrderBy(p => p.PageNumber).ToList();
            masterModel.BubbleSheetFileSubViewModels = new List<BubbleSheetFileSubViewModel>();
            if (bubbleSheetFileSubs.Any())
            {
                masterModel.BubbleSheetFileSubViewModels =
                    bubbleSheetFileSubs.Select(x => new BubbleSheetFileSubViewModel()
                    {
                        ImageUrl =
                                                            BubbleSheetWsHelper.GetTestImageUrl(x.OutputFileName,
                                                                ConfigurationManager.AppSettings["ApiKey"]),
                        SubFileName = x.InputFileName
                    }).ToList();
            }
            //get answer data for each section
            var answerSectionData = new List<ACTAnswerSectionData>();
            if (latestSectionTag.Any())
                answerSectionData = actReportService.GetACTAnswerSectionData(latestSectionTag.First().TestResultID);

            foreach (var sectionID in listSection)
            {
                var sectionTagModel = new SectionTagViewModel();
                sectionTagModel.SectionName = latestSectionTag.First(x => x.SectionID == sectionID).SectionName;
                sectionTagModel.TagCategoryReportViewModels = new List<TagCategoryReportViewModel>();

                var tagCategoryData = latestSectionTag.Where(x => x.SectionID == sectionID).ToList();
                BuildTagCategoryData(sectionTagModel, tagCategoryData, answerSectionData);

                sectionTagModel.AnswerSectionViewModels = answerSectionData.Where(x => x.SectionID == sectionID)
                    .GroupBy(x => new { x.WasAnswered, x.PointsEarned, x.PointsPossible, x.CorrectAnswer, x.AnswerLetter, x.AnswerID, x.QuestionOrder })
                    .Select(g => new AnswerSectionViewModel
                    {
                        WasAnswered = g.Key.WasAnswered,
                        PointsEarned = g.Key.PointsEarned,
                        PointsPossible = g.Key.PointsPossible,
                        CorrectAnswer = g.Key.CorrectAnswer,
                        AnswerLetter = g.Key.AnswerLetter,
                        AnswerID = g.Key.AnswerID,
                        QuestionOrder = g.Key.QuestionOrder,
                        TagNames = string.Join(", ", g.Select(x => x.TagName))
                    }).ToList();

                for (int i = 0; i < sectionTagModel.AnswerSectionViewModels.Count; i++)
                {
                    GetAlternatingOptionsForAnswer(sectionTagModel.AnswerSectionViewModels[i]);
                }
                masterModel.SectionTagViewModels.Add(sectionTagModel);
            }
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

                sectionTagModel.TagCategoryReportViewModels.Add(tagCategoryModel);
            }
        }

        private void BuildSingleTagData(TagCategoryReportViewModel tagCategoryModel, List<ACTSectionTagData> tagData,
                                        List<ACTAnswerSectionData> answerSectionData)
        {
            foreach (var actSectionTagData in tagData)
            {
                var tagModel = new SingleTagReportViewModel
                {
                    IncorrectAnswer = actSectionTagData.IncorrectAnswer,
                    BlankAnswer = actSectionTagData.BlankAnswer,
                    CorrectAnswer = actSectionTagData.CorrectAnswer,
                    HistoricalAverage = actSectionTagData.HistoricalAvg,
                    TotalAnswer = actSectionTagData.TotalAnswer,
                    TagName = actSectionTagData.TagName,
                    TagNameForOrder = actSectionTagData.TagNameForOrder,
                    Percent = actSectionTagData.Percentage,
                    ListAnswerInTag = new List<ACTAnswerSectionData>()
                };
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

        private void BuildSummaryScoreData(ACTReportMasterViewModel masterModel)
        {
            masterModel.ACTSummaryScoreViewModel = new ACTSummaryScoreViewModel();

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
            var bestSummaryScore = new TestAndScoreViewModel
            {
                EnglishScore =
                                               masterModel.DiagnosticHistoryViewModel.TestAndScoreViewModels.Max(
                                                   en => en.EnglishScore),
                MathScore =
                                               masterModel.DiagnosticHistoryViewModel.TestAndScoreViewModels.Max(
                                                   en => en.MathScore),
                ReadingScore =
                                               masterModel.DiagnosticHistoryViewModel.TestAndScoreViewModels.Max(
                                                   en => en.ReadingScore),
                ScienceScore =
                                               masterModel.DiagnosticHistoryViewModel.TestAndScoreViewModels.Max(
                                                   en => en.ScienceScore),
                CompositeScore =
                                       masterModel.DiagnosticHistoryViewModel.TestAndScoreViewModels.Max(
                                           en => en.CompositeScore)
            };
            var improvementSummaryScore = new TestAndScoreViewModel
            {
                EnglishScore =
                                                      bestSummaryScore.EnglishScore - baseSummaryScore.EnglishScore,
                MathScore = bestSummaryScore.MathScore - baseSummaryScore.MathScore,
                ReadingScore =
                                                      bestSummaryScore.ReadingScore - baseSummaryScore.ReadingScore,
                ScienceScore =
                                                      bestSummaryScore.ScienceScore - baseSummaryScore.ScienceScore,
                CompositeScore =
                                                  bestSummaryScore.CompositeScore - baseSummaryScore.CompositeScore,
            };

            masterModel.ACTSummaryScoreViewModel.SummaryScores = new List<SummaryScoreViewModel>();

            masterModel.ACTSummaryScoreViewModel.SummaryScores.Add(new SummaryScoreViewModel
            {
                Subject = "Composite",
                Baseline = baseSummaryScore.CompositeScore,
                Current = currentSummaryScore.CompositeScore,
                Best = bestSummaryScore.CompositeScore,
                Improvement =
                                                                               improvementSummaryScore.CompositeScore
            });

            masterModel.ACTSummaryScoreViewModel.SummaryScores.Add(new SummaryScoreViewModel
            {
                Subject = "English",
                Baseline = baseSummaryScore.EnglishScore,
                Current = currentSummaryScore.EnglishScore,
                Best = bestSummaryScore.EnglishScore,
                Improvement =
                                                                               improvementSummaryScore.EnglishScore
            });

            masterModel.ACTSummaryScoreViewModel.SummaryScores.Add(new SummaryScoreViewModel
            {
                Subject = "Math",
                Baseline = baseSummaryScore.MathScore,
                Current = currentSummaryScore.MathScore,
                Best = bestSummaryScore.MathScore,
                Improvement =
                                                                               improvementSummaryScore.MathScore
            });

            masterModel.ACTSummaryScoreViewModel.SummaryScores.Add(new SummaryScoreViewModel
            {
                Subject = "Reading",
                Baseline = baseSummaryScore.ReadingScore,
                Current = currentSummaryScore.ReadingScore,
                Best = bestSummaryScore.ReadingScore,
                Improvement =
                                                                               improvementSummaryScore.ReadingScore
            });

            masterModel.ACTSummaryScoreViewModel.SummaryScores.Add(new SummaryScoreViewModel
            {
                Subject = "Science",
                Baseline = baseSummaryScore.ScienceScore,
                Current = currentSummaryScore.ScienceScore,
                Best = bestSummaryScore.ScienceScore,
                Improvement =
                                                                               improvementSummaryScore.ScienceScore
            });



            // Calculate scores of each subject
            masterModel.ACTSummaryScoreViewModel.CompositeScores =
                masterModel.DiagnosticHistoryViewModel.TestAndScoreViewModels.OrderByDescending(en => en.TestDate).Take(
                    8).OrderBy(en => en.TestDate).Select(
                        en => new ScoreViewModel
                        {
                            DateString =
                                          en.TestDate.DisplayDateWithFormat(),
                            Score =
                                          CalculateCompositeScoreForSummary
                                          (en)
                        }).ToList();

            masterModel.ACTSummaryScoreViewModel.EnglishScores =
                masterModel.DiagnosticHistoryViewModel.TestAndScoreViewModels.OrderByDescending(en => en.TestDate).Take(
                    8).OrderBy(en => en.TestDate).Select(
                        en => new ScoreViewModel
                        {
                            DateString =
                                          en.TestDate.DisplayDateWithFormat(),
                            Score = en.EnglishScore
                        }).ToList();

            masterModel.ACTSummaryScoreViewModel.MathScores =
                masterModel.DiagnosticHistoryViewModel.TestAndScoreViewModels.OrderByDescending(en => en.TestDate).Take(
                    8).OrderBy(en => en.TestDate).Select(
                        en => new ScoreViewModel
                        {
                            DateString =
                                          en.TestDate.DisplayDateWithFormat(),
                            Score = en.MathScore
                        }).ToList();

            masterModel.ACTSummaryScoreViewModel.ReadingScores =
                masterModel.DiagnosticHistoryViewModel.TestAndScoreViewModels.OrderByDescending(en => en.TestDate).Take(
                    8).OrderBy(en => en.TestDate).Select(
                        en => new ScoreViewModel
                        {
                            DateString =
                                          en.TestDate.DisplayDateWithFormat(),
                            Score = en.ReadingScore
                        }).ToList();

            masterModel.ACTSummaryScoreViewModel.ScienceScores =
                masterModel.DiagnosticHistoryViewModel.TestAndScoreViewModels.OrderByDescending(en => en.TestDate).Take(
                    8).OrderBy(en => en.TestDate).Select(
                        en => new ScoreViewModel
                        {
                            DateString =
                                          en.TestDate.DisplayDateWithFormat(),
                            Score = en.ScienceScore
                        }).ToList();
        }

        private decimal CalculateCompositeScoreForSummary(TestAndScoreViewModel item)
        {
            return (item.EnglishScore + item.MathScore + item.ReadingScore + item.ScienceScore) / 4;
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

        public ActionResult ReportPrinting(int studentID, int districtID, int testID, int teacherID, int generatedUserID)
        {
            //_districtID = districtID;
            Util.LoadDateFormatToCookies(districtID, districtDecodeService);
            var masterModel = BuildReportMasterModel(studentID, districtID, testID, teacherID, generatedUserID);
            masterModel.IsLastReportInList = false;
            return View(masterModel);
        }

        public ActionResult ReportPrintingMultipleStudents(string studentIDs, int districtID, int testID, int teacherID, int generatedUserID)
        {
            var listStudentID = new List<int>();
            var parseStrStudentID = studentIDs.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var strID in parseStrStudentID)
            {
                int studentID;
                if (int.TryParse(strID, out studentID))
                {
                    listStudentID.Add(studentID);
                }
            }
            var model = new ACTReportMasterCollectionViewModel();
            model.ListACTReportMasterViewModel = new List<ACTReportMasterViewModel>();
            foreach (var studentID in listStudentID)
            {
                var masterModel = BuildReportMasterModel(studentID, districtID, testID, teacherID, generatedUserID);
                masterModel.IsLastReportInList = false;
                model.ListACTReportMasterViewModel.Add(masterModel);
            }
            if (model.ListACTReportMasterViewModel.Count > 0)
            {
                model.ListACTReportMasterViewModel[model.ListACTReportMasterViewModel.Count - 1].IsLastReportInList =
                    true;
            }
            return View(model);
        }

        private byte[] PrintOneFile(List<int> listStudentIDs, int districtId, int testID, int teacherID, int timezoneOffset)
        {
            var url = Url.Action("ReportPrintingMultipleStudents", "KnowsysReport",
                new
                {
                    studentIDs = string.Join(",", listStudentIDs.Select(x => x.ToString())),
                    districtID = districtId,
                    testID = testID,
                    teacherID = teacherID,
                    generatedUserID = CurrentUser.Id
                }, HelperExtensions.GetHTTPProtocal(Request));
            var pdf = ExportToPDF(url, timezoneOffset, listStudentIDs, testID, teacherID);
            return pdf;
        }

        private byte[] PrintOneFileV2(List<int> listStudentIDs, int districtId, int testID, int teacherID, int timezoneOffset, out string reportUrl)
        {
            var listPdfFiles = new List<byte[]>();
            var listUrls = new List<string>();
            foreach (var studentID in listStudentIDs)
            {
                var url = Url.Action("ReportPrinting", "KnowsysReport",
                    new
                    {
                        studentID = studentID,
                        districtID = districtId,
                        testID = testID,
                        teacherID = teacherID,
                        generatedUserID = CurrentUser.Id
                    }, HelperExtensions.GetHTTPProtocal(Request));
                var pdf = ExportToPDF(url, timezoneOffset, studentID);
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

        private byte[] ExportToPDF(string url, int timezoneOffset, List<int> listStudentIDs, int testID, int teacherID)
        {
            List<string> listStudentName = new List<string>();
            List<string> listStudentCode = new List<string>();
            List<string> listPageNumber = new List<string>();

            var virtualTestSubTypeId = virtualTestService.GetTestById(testID).VirtualTestSubTypeID.GetValueOrDefault();

            foreach (var studentID in listStudentIDs)
            {
                var student = studentService.GetStudentById(studentID);
                var studentName = Util.FormatFullname(string.Format("{0}, {1}", student.LastName, student.FirstName));

                var masterModel = new ACTReportMasterViewModel();
                masterModel.VirtualTestSubTypeId = virtualTestSubTypeId;

                BuildDiagnosticHistoryData(masterModel, studentID, testID, teacherID);
                if (masterModel.DiagnosticHistoryViewModel.TestAndScoreViewModels.Count >=
                    Constanst.ACTStudentReportMinAmountOfTestForChart)
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
            string footerUrl = Url.Action("RenderFooter", "KnowsysReport",
                new
                {
                    leftLine1 = DateTime.UtcNow.AddMinutes(timezoneOffset * (-1)).DisplayDateWithFormat(true),
                    //leftLine2 = string.Format("<div id='custom-footer-logon'><img src='{0}' style='position: relative; top: 5px;'><span>Copyright © 2014 | Powered by LinkIt!</span></div>", Url.Content("~/Content/images/loog-linkit-16x16.png")),
                    leftLine2 = string.Join("|", listPageNumber),
                    rightLine1 = string.Join("|", listStudentName),
                    rightLine2 = string.Join("|", listStudentCode)
                }, HelperExtensions.GetHTTPProtocal(Request));

            string headerUrl = Url.Action("RenderHeader", "KnowsysReport", null, HelperExtensions.GetHTTPProtocal(Request));

            string args =
                string.Format(
                    "--footer-html \"{0}\" --header-html \"{2}\" --header-spacing 5 {1} - "
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
            string footerUrl = Url.Action("RenderFooter", "KnowsysReport",
                new
                {
                    leftLine1 = DateTime.UtcNow.AddMinutes(timezoneOffset * (-1)).DisplayDateWithFormat(true),
                    //leftLine2 = string.Format("<div id='custom-footer-logon'><img src='{0}' style='position: relative; top: 5px;'><span>Copyright © 2014 | Powered by LinkIt!</span></div>", Url.Content("~/Content/images/loog-linkit-16x16.png")),
                    leftLine2 = string.Empty,
                    rightLine1 = studentName,
                    rightLine2 = string.Empty
                }, HelperExtensions.GetHTTPProtocal(Request));

            string headerUrl = Url.Action("RenderHeader", "KnowsysReport", null, HelperExtensions.GetHTTPProtocal(Request));

            string args =
                string.Format(
                    "--footer-html \"{0}\" --header-html \"{2}\" --header-spacing 5 \"{1}\" - "
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

        private byte[] ExportBlankPDF()
        {
            var url = Url.Action("BlankPage", "KnowsysReport", null, HelperExtensions.GetHTTPProtocal(Request));

            string args =
                string.Format(
                    "--footer-html \"{0}\" --header-html \"{2}\" --header-spacing 5 \"{1}\" - "
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
                return Json(new { ErrorList = model.ValidationErrors, Success = false });
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

            //var pdf = PrintOneFile(listStudentId, model.DistrictId.GetValueOrDefault(),
            //                              model.TestId,
            //                              model.TeacherId, model.TimezoneOffset);
            //var stream = new MemoryStream(pdf);

            //var result = S3Service.UploadRubricFile(bucketName,
            //    folder + "/" + model.ActReportFileName, stream);
            string reportUrl = string.Empty;

            var pdf = PrintOneFileV2(listStudentId, model.DistrictId.GetValueOrDefault(),
                model.TestId,
                model.TeacherId, model.TimezoneOffset, out reportUrl);

            var stream = new MemoryStream(pdf);

            var folder = LinkitConfigurationManager.GetS3Settings().ACTReportFolder;
            var bucketName = LinkitConfigurationManager.GetS3Settings().ACTReportBucket;
            var result = _s3Service.UploadRubricFile(bucketName,
                folder + "/" + model.ActReportFileName, stream);

            return Json(new { IsSuccess = true, Url = reportUrl });
        }

        private ACTReportMasterViewModel BuildReportMasterModel(int studentID, int districtID, int testID, int teacherID,
                                                                int generatedUserID)
        {
            var masterModel = new ACTReportMasterViewModel { SectionTagViewModels = new List<SectionTagViewModel>() };

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

            BuildDiagnosticHistoryData(masterModel, studentID, testID, teacherID);

            BuildSessionTagData(masterModel, studentID);

            BuildStudentInformationData(masterModel, studentID, generatedUserID);

            BuildSummaryScoreData(masterModel);

            BuildEssayCommentData(masterModel, districtID);

            return masterModel;
        }

        private void BuildEssayCommentData(ACTReportMasterViewModel model, int districtId)
        {
            var reportBannerUrl = string.Format("{0}{1}-report-banner.png", LinkitConfigurationManager.GetS3Settings().S3CSSKey, districtId);
            if (UrlUtil.CheckUrlStatus(reportBannerUrl))
            {
                model.DistrictReportBannerUrl = reportBannerUrl;
            }

            var districtDecode =
                        districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(districtId, Util.KNOWSYS_EssayComment_Title).FirstOrDefault();
            if (districtDecode != null)
            {
                model.EssayCommentTitle = districtDecode.Value;
            }

            model.EssayComments = new List<string>();

            if (model.SectionTagViewModels != null && model.SectionTagViewModels.Count >= 5)
            {
                var essaySectionTag = model.SectionTagViewModels[4];
                foreach (var answerSection in essaySectionTag.AnswerSectionViewModels)
                {
                    var pointsEarned = answerSection.PointsEarned;

                    // If the two pointsEarned are equal ==> up one level for the second pointsEarned
                    if (essaySectionTag.AnswerSectionViewModels.IndexOf(answerSection) == 1
                        && answerSection.PointsEarned == essaySectionTag.AnswerSectionViewModels[0].PointsEarned)
                        pointsEarned++;

                    districtDecode =
                        districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(districtId,
                            string.Format(Util.KNOWSYS_EssayComment_ScoreKey, pointsEarned)).FirstOrDefault();

                    if (districtDecode != null)
                    {
                        var essayComment = districtDecode.Value;
                        model.EssayComments.Add(essayComment);
                    }
                }
            }
        }

        private void GetAlternatingOptionsForAnswer(AnswerSectionViewModel answer)
        {
            // Update letter value of alternative question
            // Convert from A, B, C, D, E, F ... ==> F, G, H, J, K, L ...
            if (answer.QuestionOrder % 2 == 0)
            {
                answer.CorrectAnswer = AlternatingAnswer(answer.CorrectAnswer);
                answer.AnswerLetter = AlternatingAnswer(answer.AnswerLetter);
            }
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
