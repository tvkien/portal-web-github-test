using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExpress.Data.WcfLinq.Helpers;
using FluentValidation;
using LinkIt.BubbleSheetPortal.Common;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.ACTReport;
using LinkIt.BubbleSheetPortal.Models.Enum;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.BubbleSheetAws;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using LinkIt.BubbleSheetPortal.Web.ViewModels.ACTReport;
using Ionic.Zip;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using S3Library;
using LinkIt.BubbleSheetPortal.Web.ServiceConsumer;
using System.Net;
using LinkIt.BubbleSheetPortal.Models.Old.UnGroup;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    public class ACTReportController : BaseController
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
        private readonly IS3Service s3Service;
        private readonly IValidator<ACTReportData> actReportDataValidator;
        private readonly DistrictDecodeService districtDecodeService;
        private readonly BubbleSheetFileService bubbleSheetFileService;
        private readonly PopulateReportingService populateReportingService;
        private readonly ConfigurationService configurationService;
        private readonly APIAccountService APIAccountService;

        public ACTReportController(ACTReportService actReportService, UserService userService,
            SchoolService schoolService, UserSchoolService userSchoolService,
            TestResultService testResultService,
            TeacherDistrictTermService teacherDistrictTermService,
            ClassUserService classUserService,
            ClassService classService,
            ClassStudentService classStudentService, DistrictService districtService,
            StudentService studentService,
            VirtualTestService virtualTestService, IValidator<ACTReportData> actReportDataValidator,
            DistrictDecodeService districtDecodeService,
            BubbleSheetFileService bubbleSheetFileService,
            PopulateReportingService populateReportingService,
            ConfigurationService configurationService,
            APIAccountService apiAccountService, IS3Service s3Service)
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
            this.districtDecodeService = districtDecodeService;
            this.bubbleSheetFileService = bubbleSheetFileService;
            this.populateReportingService = populateReportingService;
            this.configurationService = configurationService;
            APIAccountService = apiAccountService;
            this.s3Service = s3Service;
        }

        [AjaxAwareAuthorize]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.ReportItemACTReport)]
        [VersionFilter]
        public ActionResult Index()
        {
            var model = new ACTReportViewModel()
            {
                IsAdmin = userService.GetAdminByUsernameAndDistrict(CurrentUser.UserName, CurrentUser.DistrictId.GetValueOrDefault()).IsNotNull(),
                CanSelectTeachers = CurrentUser.IsDistrictAdminOrPublisher,
                IsSchoolAdmin = CurrentUser.RoleId.Equals(8),
                IsPublisher = CurrentUser.RoleId.Equals((int)Permissions.Publisher),
                IsNetworkAdmin = CurrentUser.IsNetworkAdmin
            };


            if (model.IsPublisher)
            {
                model.ReportTypes = actReportService.GetAllReportTypes()
                    .Select(x => new SelectListItem() { Value = x.Name.Replace(" ", ""), Text = x.DisplayName }).ToList();
            }
            else
            {
                var districtDecodes =
                    districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(
                        CurrentUser.DistrictId.GetValueOrDefault(), Util.HasAccessToReport);
                model.ReportTypes = new List<SelectListItem>();
                if (districtDecodes.Any())
                {
                    var reportTypeIds = districtDecodes.Select(x => x.Value).Distinct().ToList();
                    var idList = new List<int>();
                    foreach (var reportTypeId in reportTypeIds)
                    {
                        int id;
                        if (int.TryParse(reportTypeId, out id))
                            idList.Add(id);
                    }
                    var reportTypes = actReportService.GetReportTypes(idList);
                    model.ReportTypes =
                        reportTypes.Select(x => new SelectListItem() { Value = x.Name.Replace(" ", ""), Text = x.DisplayName })
                            .ToList();
                }
            }

            var districtDecode = districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(
                    CurrentUser.DistrictId.GetValueOrDefault(), Util.KNOWSYS_SATReport_IncludeStateInformation)
                    .FirstOrDefault();

            model.IncludeStateInformation = false;
            if (districtDecode != null)
                model.IncludeStateInformation = districtDecode.Value == "1";

            return View(model);
        }

        [AjaxAwareAuthorize]
        public ActionResult Report()
        {
            //TODO: hard-code for testing, need to remove when finish
            //var testResultID = 5745554;
            //var studentID = 152664;

            //var masterModel = BuildReportMasterModel(studentID, CurrentUser.DistrictId.GetValueOrDefault());

            //return View(masterModel);
            return View();
        }

        private void BuildStudentInformationData(ACTReportMasterViewModel masterModel, int studentID,
                                                 int generatedUserId, int? takenReportTeacherId, int? takenReportClassId)
        {
            //var testResultID = masterModel.DiagnosticHistoryViewModel.TestAndScoreViewModels[0].TestResultID;
            var testResultID =
                masterModel.DiagnosticHistoryViewModel.TestAndScoreViewModels.FirstOrDefault(en => en.IsSelected).
                    TestResultID;

            var studentInfo = actReportService.GetACTStudentInformation(studentID, testResultID);

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

        private void BuildDiagnosticHistoryData(ACTReportMasterViewModel masterModel, int studentID, int testID,
                                                int teacherID)
        {
            masterModel.DiagnosticHistoryViewModel = new DiagnosticHistoryViewModel
            {
                TestAndScoreViewModels = new List<TestAndScoreViewModel>()
            };

            var diagnotisHistoryData = masterModel.UseNewACTStudentFormat ?
                actReportService.GetACTTestHistoryData(studentID, 6) :
                actReportService.GetACTTestHistoryData(studentID, masterModel.VirtualTestSubTypeId);
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
                    diagnotisHistoryData.Where(en => en.VirtualTestID == testID && en.TeacherID == teacherID).
                        OrderByDescending(en => en.UpdatedDate).FirstOrDefault();
                if (selectedTestResult == null)
                {
                    selectedTestResult =
                    diagnotisHistoryData.Where(en => en.VirtualTestID == testID).
                        OrderByDescending(en => en.UpdatedDate).FirstOrDefault();
                }
                var selectedTestResultId = selectedTestResult == null ? 0 : selectedTestResult.TestResultID;

                foreach (var testResultData in listTestResult)
                {
                    var data = diagnotisHistoryData.Where(x => x.TestResultID == testResultData.TestResultID).ToList();
                    var englishScore = GetSectionScoreBySectionName("english", data);
                    var mathScore = GetSectionScoreBySectionName("math", data);
                    var readingScore = GetSectionScoreBySectionName("reading", data);
                    var scienceScore = GetSectionScoreBySectionName("science", data);
                    var writingScoreScaled = GetSectionScoreBySectionName("writing", data);
                    // Assign scoreraw to Writing section score for new act test type instead of scorescaled
                    // Task: https://basecamp.com/2632355/projects/5857408/todos/261149269
                    var writingScore = GetSectionScoreBySectionName("writing", data);
                    if (masterModel.VirtualTestSubTypeId == (int)VirtualTestSubType.NewACT)
                    {
                        writingScore = GetWritingSectionScoreOfNewACT(data);
                    }
                    var ewScore = GetSectionScoreBySectionName("e/w", data);
                    var compositeScore = data.First().CompositeScore;

                    var testScore = new TestAndScoreViewModel
                    {
                        VirtualTestSubTypeID = data.Any() ? data.First().VirtualTestSubTypeID : 0,
                        TestResultID = testResultData.TestResultID,
                        TestDate = testResultData.UpdatedDate,
                        EnglishScore = englishScore,
                        EnglishWritingScore = ewScore,
                        MathScore = mathScore,
                        ReadingScore = readingScore,
                        ScienceScore = scienceScore,
                        WritingScore = writingScore,
                        WritingScoreScaled = writingScoreScaled,
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

        private void BuildSessionTagData(ACTReportMasterViewModel masterModel, int studentID, int testId, int districtId)
        {
            var sectionTagData = actReportService.GetACTSectionTagData(studentID, masterModel.VirtualTestSubTypeId).OrderBy(m => m.ItemTagCategoryOrder).ToList();

            if (masterModel.UseNewACTStudentFormat && masterModel.VirtualTestSubTypeId == (int)VirtualTestSubType.ACT)
            {
                sectionTagData =
                    sectionTagData.Union(actReportService.GetACTSectionTagData(studentID,
                        (int)VirtualTestSubType.NewACT)).ToList();
            }
            //get latest virtual test
            //var latestSectionTag = sectionTagData.Where(x => x.UpdatedDate == sectionTagData.Max(y => y.UpdatedDate)).ToList();
            var testResultID =
                masterModel.DiagnosticHistoryViewModel.TestAndScoreViewModels.FirstOrDefault(en => en.IsSelected).
                    TestResultID;
            var latestSectionTag = sectionTagData.Where(x => x.TestResultID == testResultID).ToList();

            var listSection = latestSectionTag.OrderBy(m => m.SectionOrder).Select(x => x.SectionID).Distinct().ToList();

            //Get Lastest essay pages
            var bubbleSheetId = testResultService.GetTestResultById(testResultID).BubbleSheetId;
            // Page 3,4,5,6 are essay pages
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
                        SubFileName = x.InputFileName,
                        PageType = Util.BubbleSheetFileEssayPageType
                    }).Where(m => m.PageType == Util.BubbleSheetFileEssayPageType && !string.IsNullOrEmpty(m.ImageUrl)).ToList();
            }

            //get answer data for each section
            var answerSectionData = new List<ACTAnswerSectionData>();
            if (latestSectionTag.Any())
                answerSectionData = actReportService.GetACTAnswerSectionData(latestSectionTag.First().TestResultID);

            // Get Domain tag category ID configuration using for New ACT
            var domainTagCategoryId = GetVirtualTestDistrictId(testId);
            masterModel.DomainTagData = sectionTagData.Where(x => x.CategoryID == domainTagCategoryId).ToList();

            foreach (var sectionID in listSection)
            {
                var sectionTagModel = new SectionTagViewModel();
                sectionTagModel.SectionName = latestSectionTag.First(x => x.SectionID == sectionID).SectionName;
                sectionTagModel.TagCategoryReportViewModels = new List<TagCategoryReportViewModel>();

                var tagCategoryData = latestSectionTag.Where(x => x.SectionID == sectionID).ToList();
                BuildTagCategoryData(sectionTagModel, tagCategoryData, answerSectionData);

                sectionTagModel.AnswerSectionViewModels = answerSectionData.Where(x => x.SectionID == sectionID)
                    .Select(x => new AnswerSectionViewModel
                    {
                        WasAnswered = x.WasAnswered,
                        PointsEarned = x.PointsEarned,
                        PointsPossible = x.PointsPossible,
                        CorrectAnswer = x.CorrectAnswer,
                        AnswerLetter = x.AnswerLetter,
                        AnswerID = x.AnswerID,
                        QuestionOrder = x.QuestionOrder
                    }).Distinct(new AnswerSectionViewModel.ACTComparer()).ToList();
                for (int i = 0; i < sectionTagModel.AnswerSectionViewModels.Count; i++)
                {
                    GetAlternatingOptionsForAnswer(sectionTagModel.AnswerSectionViewModels[i]);
                }

                sectionTagModel.DomainTagCategoryId = domainTagCategoryId;
                masterModel.SectionTagViewModels.Add(sectionTagModel);
            }
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
                    var domainTagCategoryIdDistrictDecode = districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(user.DistrictId.GetValueOrDefault(), "DomainTagCategoryID").FirstOrDefault();
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
                var first = tagCategoryData.First(x => x.CategoryID == categoryID);

                var tagCategoryModel = new TagCategoryReportViewModel();
                tagCategoryModel.TagCategoryId = categoryID;
                tagCategoryModel.TagCategoryName = first.CategoryName;
                tagCategoryModel.TagCategoryDescription = first.CategoryDescription;

                if (first.PresentationType.HasValue)
                {
                    tagCategoryModel.PresentationType = (ACTPresentationType)first.PresentationType.Value;
                }

                tagCategoryModel.Order = first.ItemTagCategoryOrder;

                tagCategoryModel.SingleTagReportViewModels = new List<SingleTagReportViewModel>();

                var tagData = tagCategoryData.Where(x => x.CategoryID == categoryID).ToList();
                BuildSingleTagData(tagCategoryModel, tagData, answerSectionData);

                sectionTagModel.TagCategoryReportViewModels.Add(tagCategoryModel);
            }

            sectionTagModel.TagCategoryReportViewModels = sectionTagModel.TagCategoryReportViewModels
                                                                .OrderBy(m => m.PresentationType)
                                                                .ThenBy(m => m.Order).ToList();
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
                    Order = actSectionTagData.ItemTagOrder,
                    ListAnswerInTag = new List<ACTAnswerSectionData>()
                };
                tagModel.ListAnswerInTag.AddRange(
                    answerSectionData.Where(
                        x => x.TagID == actSectionTagData.TagID && x.SectionID == actSectionTagData.SectionID).OrderBy(
                            x => x.QuestionOrder).ToList());
                if (tagCategoryModel.IsTechniqueCategory)
                {
                    tagCategoryModel.SingleTagReportViewModels.Add(tagModel);
                }
                else
                {
                    tagCategoryModel.SingleTagReportViewModels.Add(tagModel);
                }
            }

            tagCategoryModel.SingleTagReportViewModels =
                tagCategoryModel.SingleTagReportViewModels.OrderBy(en => en.Order).ToList(); // TagNameForOrder
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
            //masterModel.ACTSummaryScoreViewModel.SummaryScores.Add(new SummaryScoreViewModel
            //                                                           {
            //                                                               Subject = "Composite",
            //                                                               Baseline =
            //                                                                   CalculateCompositeScoreForSummary(
            //                                                                       baseSummaryScore),
            //                                                               Current =
            //                                                                   CalculateCompositeScoreForSummary(
            //                                                                       currentSummaryScore),
            //                                                               Best =
            //                                                                   CalculateCompositeScoreForSummary(
            //                                                                       bestSummaryScore),
            //                                                               Improvement =
            //                                                                   CalculateCompositeScoreForSummary(
            //                                                                       improvementSummaryScore)
            //                                                           });

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

        private decimal GetWritingSectionScoreOfNewACT(List<ACTTestHistoryData> data)
        {
            var sectionName = "writing";
            var scoreData =
                data.FirstOrDefault(
                    x =>
                    !string.IsNullOrEmpty(x.SectionName) &&
                    x.SectionName.Trim().Equals(sectionName, StringComparison.InvariantCultureIgnoreCase));
            return scoreData == null ? 0 : scoreData.SectionScoreRaw;
        }

        public ActionResult ReportPrinting(int studentID, int districtID, int testID, int teacherID, int generatedUserID, int reportContentOption, bool useNewACTStudentFormat, int? takenReportTeacherId, int? takenReportClassId)
        {
            Util.LoadDateFormatToCookies(districtID, districtDecodeService);
            var masterModel = BuildReportMasterModel(studentID, districtID, testID, teacherID, generatedUserID, reportContentOption, useNewACTStudentFormat, takenReportTeacherId, takenReportClassId);
            return View("ReportPrinting", masterModel);
        }

        public ActionResult ReportPrintingUseForCoding()
        {
            Util.LoadDateFormatToCookies(272, districtDecodeService);
            var masterModel = BuildReportMasterModel(1972899, 2621, 118338, 428203, 327671, 1, true, null, null);
            //var masterModel = BuildReportMasterModel(studentID, districtID, testID, teacherID, generatedUserID, reportContentOption, useNewACTStudentFormat, takenReportTeacherId, takenReportClassId);
            //return View(masterModel);
            return View("ReportPrinting", masterModel);
        }

        public ACTReportMasterViewModel BuildReportPrintingModel(int studentID, int districtID, int testID, int teacherID, int generatedUserID, int reportContentOption, bool useNewACTStudentFormat, int? takenReportTeacherId, int? takenReportClassId)
        {
            Util.LoadDateFormatToCookies(districtID, districtDecodeService);
            var masterModel = BuildReportMasterModel(studentID, districtID, testID, teacherID, generatedUserID, reportContentOption, useNewACTStudentFormat, takenReportTeacherId, takenReportClassId);
            IncludeJSCss(masterModel);

            return masterModel;
        }

        private void IncludeJSCss(ACTReportMasterViewModel model)
        {
            var cssPaths = new List<string>
            {
                "Content/themes/Constellation/css/reset.css",
                "Content/themes/Constellation/css/common.css",
                "Content/themes/Constellation/css/custom.css",
                "Content/themes/Constellation/css/standard.css",
                "Content/themes/Constellation/css/special-pages.css",
                "Content/themes/Constellation/css/table.css",
                "Content/css/reports/princeXML.css",
                "Content/css/reports/act-report.css"
            };

            var jsPaths = new List<string>
            {
                "Scripts/jquery-1.7.1.min.js",
                "Scripts/underscore.js",
                "Content/themes/Constellation/js/html5.js",
                "Scripts/Reports/ACTReport.js",
            };

            var mapPath = HttpContext.Server.MapPath("~/");

            foreach (var item in cssPaths)
            {
                model.Css.Add(System.IO.File.ReadAllText(string.Format("{0}/{1}", mapPath, item)));
            }

            foreach (var item in jsPaths)
            {
                model.JS.Add(System.IO.File.ReadAllText(string.Format("{0}/{1}", mapPath, item)));
            }
        }

        private byte[] PrintOneFileV2(List<int> listStudentIDs, int districtId, int testID, int teacherID, int timezoneOffset, int reportContentOption, bool useNewACTStudentFormat, out string reportUrl)
        {
            var listPdfFiles = new List<byte[]>();
            var listUrls = new List<string>();

            var pageSize = "";
            var virtualTestSubTypeId = virtualTestService.GetTestById(testID).VirtualTestSubTypeID.GetValueOrDefault();
            if (virtualTestSubTypeId == (int)VirtualTestSubType.NewACT)
            {
                pageSize = "Letter";
            }

            for (int index = 0; index < listStudentIDs.Count; index++)
            {
                var studentID = listStudentIDs[index];
                var url = Url.Action("ReportPrinting", "ACTReport",
                    new
                    {
                        studentID,
                        districtID = districtId,
                        testID,
                        teacherID,
                        generatedUserID = CurrentUser.Id,
                        reportContentOption,
                        useNewACTStudentFormat
                    }, HelperExtensions.GetHTTPProtocal(Request));
                listUrls.Add(url);

                var model = BuildReportPrintingModel(studentID, districtId, testID, teacherID, CurrentUser.Id, reportContentOption, useNewACTStudentFormat, null, null);
                if (reportContentOption != 3 || model.BubbleSheetFileSubViewModels.Count > 0)
                {
                    var html = this.RenderRazorViewToString("ReportPrinting", model);
                    var pdf = ExportToPDFByPrinceXML(html);
                    listPdfFiles.Add(pdf);
                }
            }

            var file = PdfHelper.MergeFilesAddBlankToOddFile(listPdfFiles);
            reportUrl = string.Join(",", listUrls);
            return file;
        }

        private int CountNewACTTotalItem(int districtId, int? schoolId, int? teacherId, int? districtTermId, int? classId
            , List<int> listVirtualTestIds, List<int> listStudentIds
            , DateTime? resultDateFrom, DateTime? resultDateTo, bool? isGetAllClass)
        {
            if (isGetAllClass.HasValue && isGetAllClass.Value == true)
            {
                return CountNewACTTotalItemWithGetAllClassOption(districtId, schoolId, teacherId, districtTermId, classId
                    , listVirtualTestIds, listStudentIds, resultDateFrom, resultDateTo);
            }
            else
            {
                return CountNewACTTotalItemWithoutGetAllClassOption(districtId, schoolId, teacherId, districtTermId, classId
                    , listVirtualTestIds, listStudentIds, resultDateFrom, resultDateTo);
            }
        }

        private int CountNewACTTotalItemWithoutGetAllClassOption(int districtId, int? schoolId, int? teacherId, int? districtTermId, int? classId
            , List<int> listVirtualTestIds, List<int> listStudentIds
            , DateTime? resultDateFrom, DateTime? resultDateTo)
        {
            var counter = 0;
            var virtualTestIdString = string.Join(",", listVirtualTestIds);
            var virtualTestSubTypeIdString = "2;6";

            var listSchoolIds = new List<int>();
            if (schoolId.HasValue)
                listSchoolIds.Add(schoolId.Value);
            else
            {
                listSchoolIds.AddRange(NewActGetListSchoolIds(districtId, virtualTestIdString,
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
                    listTeacherIds.AddRange(NewActGetListTeacherIds(districtId, listSchoolId, virtualTestIdString,
                        virtualTestSubTypeIdString, resultDateFrom, resultDateTo));
                }

                foreach (var listTeacherId in listTeacherIds)
                {
                    var listClassIds = new List<int>();

                    if (classId.HasValue)
                        listClassIds.Add(classId.Value);
                    else
                    {
                        listClassIds.AddRange(NewActGetListClassIds(districtId, listSchoolId, listTeacherId,
                            districtTermId,
                            virtualTestIdString,
                            virtualTestSubTypeIdString, resultDateFrom, resultDateTo));
                    }

                    foreach (var listClassId in listClassIds)
                    {
                        foreach (var virtualTestId in listVirtualTestIds)
                        {
                            var studentIds =
                                MultipleTestGetStudents(listClassId, virtualTestId.ToString(), resultDateFrom,
                                    resultDateTo).OrderBy(x => x.FullName).Select(x => x.StudentId).ToList();

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

        private int CountNewACTTotalItemWithGetAllClassOption(int districtId, int? schoolId, int? teacherId, int? districtTermId, int? classId
            , List<int> listVirtualTestIds, List<int> listStudentIds
            , DateTime? resultDateFrom, DateTime? resultDateTo)
        {
            var counter = 0;

            Dictionary<int, Dictionary<int, List<int>>> sumClassTestStudents = new Dictionary<int, Dictionary<int, List<int>>>();

            var virtualTestIdString = string.Join(",", listVirtualTestIds);
            var virtualTestSubTypeIdString = "2;6";

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

        private List<ReportFileResultModel> PrintOneFileNewACT(int districtId, int? schoolId, int? teacherId, int? districtTermId, int? classId
            , List<int> listVirtualTestIds, List<int> listStudentIds
            , DateTime? resultDateFrom, DateTime? resultDateTo
            , int specializedReportJobId
            , int timezoneOffset, int reportContentOption, bool? isGetAllClass, out string reportUrl)
        {
            if (isGetAllClass.HasValue && isGetAllClass.Value == true)
            {
                return PrintOneFileNewACTWithGetAllClassOption(districtId, schoolId, teacherId, districtTermId, classId
                    , listVirtualTestIds, listStudentIds, resultDateFrom, resultDateTo, specializedReportJobId
                    , timezoneOffset, reportContentOption, out reportUrl);
            }
            else
            {
                return PrintOneFileNewACTWithoutGetAllClassOption(districtId, schoolId, teacherId, districtTermId, classId
                    , listVirtualTestIds, listStudentIds, resultDateFrom, resultDateTo, specializedReportJobId
                    , timezoneOffset, reportContentOption, out reportUrl);
            }
        }

        private List<ReportFileResultModel> PrintOneFileNewACTWithoutGetAllClassOption(int districtId, int? schoolId, int? teacherId, int? districtTermId, int? classId
            , List<int> listVirtualTestIds, List<int> listStudentIds
            , DateTime? resultDateFrom, DateTime? resultDateTo
            , int specializedReportJobId
            , int timezoneOffset, int reportContentOption, out string reportUrl)
        {
            var reportfileResultModels = new List<ReportFileResultModel>();

            var virtualTestIdString = string.Join(",", listVirtualTestIds);
            var virtualTestSubTypeIdString = "2;6";

            var listUrls = new List<string>();

            var listSchoolIds = new List<int>();
            if (schoolId.HasValue)
                listSchoolIds.Add(schoolId.Value);
            else
            {
                listSchoolIds.AddRange(NewActGetListSchoolIds(districtId, virtualTestIdString,
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
                    listTeacherIds.AddRange(NewActGetListTeacherIds(districtId, listSchoolId, virtualTestIdString,
                        virtualTestSubTypeIdString, resultDateFrom, resultDateTo));
                }

                foreach (var listTeacherId in listTeacherIds)
                {
                    var listClassIds = new List<int>();

                    if (classId.HasValue)
                        listClassIds.Add(classId.Value);
                    else
                    {
                        listClassIds.AddRange(NewActGetListClassIds(districtId, listSchoolId, listTeacherId, districtTermId,
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
                                    resultDateTo).OrderBy(x => x.FullName).Select(x => x.StudentId);

                            if (listStudentIds != null && listStudentIds.Any())
                            {
                                studentIds = studentIds.Where(listStudentIds.Contains);
                            }

                            listReportStudentIds.AddRange(studentIds);

                            var pageSize = "";
                            var virtualTestSubTypeId = virtualTestService.GetTestById(virtualTestId).VirtualTestSubTypeID.GetValueOrDefault();
                            if (virtualTestSubTypeId == (int)VirtualTestSubType.NewACT)
                            {
                                pageSize = "Letter";
                            }

                            for (int index = 0; index < listReportStudentIds.Count; index++)
                            {
                                var studentId = listReportStudentIds[index];
                                if (listStudentHasReport.Any(x => x == studentId + ";" + virtualTestId))
                                {
                                    continue;
                                }
                                listStudentHasReport.Add(studentId + ";" + virtualTestId);

                                var url = Url.Action("ReportPrinting", "ACTReport",
                                    new
                                    {
                                        studentID = studentId,
                                        districtID = districtId,
                                        testID = virtualTestId,
                                        teacherID = listTeacherId,
                                        generatedUserID = CurrentUser.Id,
                                        reportContentOption,
                                        useNewACTStudentFormat = true
                                    }, HelperExtensions.GetHTTPProtocal(Request));
                                listUrls.Add(url);

                                var model = BuildReportPrintingModel(studentId, districtId, virtualTestId, listTeacherId, CurrentUser.Id, reportContentOption, true, null, null);
                                if (reportContentOption != 3 || model.BubbleSheetFileSubViewModels.Count > 0)
                                {
                                    var html = this.RenderRazorViewToString("ReportPrinting", model);
                                    var pdf = ExportToPDFByPrinceXML(html);
                                    reportFileResultModel.PdfFiles.Add(pdf);
                                }
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

        private List<ReportFileResultModel> PrintOneFileNewACTWithGetAllClassOption(int districtId, int? schoolId, int? teacherId, int? districtTermId, int? classId
            , List<int> listVirtualTestIds, List<int> listStudentIds
            , DateTime? resultDateFrom, DateTime? resultDateTo
            , int specializedReportJobId
            , int timezoneOffset, int reportContentOption, out string reportUrl)
        {
            Dictionary<int, Dictionary<int, List<int>>> sumClassTestStudents = new Dictionary<int, Dictionary<int, List<int>>>();

            var reportfileResultModels = new List<ReportFileResultModel>();

            var virtualTestIdString = string.Join(",", listVirtualTestIds);
            var virtualTestSubTypeIdString = "2;6";

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
                        var pageSize = "";
                        var virtualTestSubTypeId = virtualTestService.GetTestById(virtualTestData.Key).VirtualTestSubTypeID.GetValueOrDefault();
                        if (virtualTestSubTypeId == (int)VirtualTestSubType.NewACT)
                        {
                            pageSize = "Letter";
                        }

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
                            var url = Url.Action("ReportPrinting", "ACTReport",
                                new
                                {
                                    studentID = studentId,
                                    districtID = districtId,
                                    testID = virtualTestData.Key,
                                    teacherID = classUser.UserId,
                                    generatedUserID = CurrentUser.Id,
                                    reportContentOption,
                                    useNewACTStudentFormat = true
                                }, HelperExtensions.GetHTTPProtocal(Request));
                            listUrls.Add(url);

                            var model = BuildReportPrintingModel(studentId, districtId, virtualTestData.Key, classUser.UserId, CurrentUser.Id, reportContentOption, true, null, null);
                            if (reportContentOption != 3 || model.BubbleSheetFileSubViewModels.Count > 0)
                            {
                                var html = this.RenderRazorViewToString("ReportPrinting", model);
                                var pdf = ExportToPDFByPrinceXML(html);

                                reportFileResultModel.PdfFiles.Add(pdf);
                            }
                            IncreaseGeneratedItemNo(specializedReportJobId);
                        }
                    }

                    reportfileResultModels.Add(reportFileResultModel);
                }
            }
            else // Print and group report based on selected class
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

                    var pageSize = "";
                    var virtualTestSubTypeId = virtualTestService.GetTestById(virtualTestData.Key).VirtualTestSubTypeID.GetValueOrDefault();
                    if (virtualTestSubTypeId == (int)VirtualTestSubType.NewACT)
                    {
                        pageSize = "Letter";
                    }

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

                        var url = Url.Action("ReportPrinting", "ACTReport",
                            new
                            {
                                studentID = studentId,
                                districtID = districtId,
                                testID = virtualTestData.Key,
                                teacherID = classUser.UserId,
                                generatedUserID = CurrentUser.Id,
                                reportContentOption,
                                useNewACTStudentFormat = true,
                                takenReportTeacherId = teacherId.Value,
                                takenReportClassId = classId.Value
                            }, HelperExtensions.GetHTTPProtocal(Request));
                        listUrls.Add(url);

                        var model = BuildReportPrintingModel(studentId, districtId, virtualTestData.Key, classUser.UserId, CurrentUser.Id, reportContentOption, true, teacherId.Value, classId.Value);
                        if (reportContentOption != 3 || model.BubbleSheetFileSubViewModels.Count > 0)
                        {
                            var html = this.RenderRazorViewToString("ReportPrinting", model);
                            var pdf = ExportToPDFByPrinceXML(html);

                            reportFileResultModel.PdfFiles.Add(pdf);
                        }
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
            if (specializedReportJob != null)
            {
                specializedReportJob.GeneratedItem++;
                populateReportingService.SaveSpecializedReportJob(specializedReportJob);
            }
        }

        private IEnumerable<int> ConvertListIdFromString(string ids)
        {
            string[] idStrings = ids.Split(new char[] { ';' },
                StringSplitOptions.RemoveEmptyEntries);
            var listIds = new List<int>();
            foreach (var idString in idStrings)
            {
                int temp;
                if (int.TryParse(idString, out temp))
                {
                    listIds.Add(temp);
                }
            }
            return listIds;
        }

        private IEnumerable<int> NewActGetListSchoolIds(int districtId, string virtualTestIdString, string virtualTestSubTypeIds,
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

        private IEnumerable<int> NewActGetListTeacherIds(int districtId, int schoolId, string virtualtestIdString
            , string virtualTestSubTypeIdString, DateTime? resultDateFrom, DateTime? resultDateTo)
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

        private IEnumerable<int> NewActGetListTermIds(int districtId, int schoolId, int userId, string virtualTestIdString,
            string virtualTestSubTypeIdString, DateTime? resultDateFrom, DateTime? resultDateTo, bool? isGetAllClass)
        {
            var teacherTestDistrictTerms = populateReportingService.ReportingGetTerms(CurrentUser.Id,
                CurrentUser.RoleId,
                districtId, schoolId, userId, virtualTestIdString, virtualTestSubTypeIdString, resultDateFrom, resultDateTo, isGetAllClass ?? false);

            var data = teacherTestDistrictTerms
                .Select(x => new { x.DistrictTermId, x.DistrictTermName, x.DateStart })
                .Distinct()
                .OrderByDescending(x => x.DateStart)
                .Select(x => x.DistrictTermId)
                .ToList();

            return data;
        }

        private IEnumerable<int> NewActGetListClassIds(int districtId, int schoolId, int teacherId, int? termId, string virtualTestIdString,
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

        private byte[] ExportToPDFByPrinceXML(string html)
        {
            var pdfUrl = PdfGeneratorConsumer.InvokePdfGeneratorService(html, Guid.NewGuid().ToString(), Util.ACTReportFolder, CurrentUser.UserName);
            byte[] buffer = null;

            var hostSaveFile = ConfigurationManager.AppSettings["DownloadPdfFolderPath"];

            buffer = System.IO.File.ReadAllBytes(hostSaveFile + pdfUrl);
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
            if (!model.DistrictId.HasValue)
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
            //                              model.TeacherId, model.TimezoneOffset, model.ReportContentOption);

            var reportUrl = "";

            var pdf = PrintOneFileV2(listStudentId, model.DistrictId.GetValueOrDefault(),
                model.TestId,
                model.TeacherId, model.TimezoneOffset, model.ReportContentOption, model.UseNewACTStudentReport,
                out reportUrl);

            var stream = new MemoryStream(pdf);

            //var folder = LinkitConfigurationManager.GetS3Settings().ACTReportFolder;
            var folder = ConfigurationManager.AppSettings["ACTReportFolder"];
            var bucketName = LinkitConfigurationManager.GetS3Settings().ACTReportBucket;

            var result = s3Service.UploadRubricFile(bucketName,
                folder + "/" + model.ActReportFileName, stream);

            //return Json(new {IsSuccess = true});
            return Json(new { IsSuccess = true, Url = reportUrl });
        }

        [AjaxAwareAuthorize]
        public ActionResult CheckGenerateNewACTRequest(NewACTReportData model)
        {
            if (!model.DistrictId.HasValue)
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

            var testResultTotalItem = CountNewACTTotalItem(model.DistrictId.Value, model.SchoolId, model.TeacherId,
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

        [HttpPost]
        [AjaxAwareAuthorize]
        public ActionResult GenerateNewACT(NewACTReportData model)
        {
            if (!model.DistrictId.HasValue)
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


            var reportFileResultModels = PrintOneFileNewACT(model.DistrictId.Value, model.SchoolId, model.TeacherId, model.DistrictTermId,
                model.ClassId
                , model.StrTestIdList.Split(',').Select(x => Convert.ToInt32(x)).ToList()
                , listStudentId
                , model.ResultDateFrom, model.ResultDateTo
                , model.SpecializedReportJobId
                , model.TimezoneOffset, model.ReportContentOption
                , model.isGetAllClass
                , out reportUrl);

            var s3Url = CreateNewACTFileAndUploadToS3(model, reportFileResultModels);
            CompletedSpecializedReportJob(model.SpecializedReportJobId, s3Url);
            if (string.IsNullOrEmpty(s3Url))
                return Json(new { IsSuccess = false, Url = string.Empty });

            return Json(new { IsSuccess = true, Url = reportUrl });
        }

        private string CreateNewACTFileAndUploadToS3(NewACTReportData model, List<ReportFileResultModel> reportFileResultModels)
        {
            var folder = ConfigurationManager.AppSettings["ACTReportFolder"];
            var bucketName = LinkitConfigurationManager.GetS3Settings().ACTReportBucket;

            var rawFileName = model.ActReportFileName.Substring(0, model.ActReportFileName.LastIndexOf("_"));
            var randomKey = model.ActReportFileName.Substring(model.ActReportFileName.LastIndexOf("_") + 1);

            var specializedReportTestResultPerFileConfiguration =
                GetSpecializedReportTestResultPerFileConfiguration(model.DistrictId.GetValueOrDefault());

            if (reportFileResultModels[0].PdfFiles.Count > 0)
            {

                if (reportFileResultModels.Count == 1 && reportFileResultModels[0].PdfFiles.Count <= specializedReportTestResultPerFileConfiguration)
                {
                    model.ActReportFileName = string.Format("{0}_{1}_{2}_{3}.pdf", rawFileName,
                        reportFileResultModels[0].TeacherLastName, reportFileResultModels[0].ClassName, randomKey).Replace("/", "_").Replace("\\", "_");

                    var file = PdfHelper.MergeFilesAddBlankToOddFile(reportFileResultModels[0].PdfFiles);
                    var stream = new MemoryStream(file);
                    var result = s3Service.UploadRubricFile(bucketName,
                        folder + "/" + model.ActReportFileName, stream);
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
                                    onePdfFiles.Add(reportFileResultModel.PdfFiles[i * specializedReportTestResultPerFileConfiguration + j]);
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
                    var result = s3Service.UploadRubricFile(bucketName,
                    folder + "/" + model.ActReportFileName, zipStream);
                }

                var s3Url = string.Format("https://s3.amazonaws.com/{0}/{1}/{2}", bucketName, folder, model.ActReportFileName.CorrectFileNameOnURL());
                return s3Url;
            }

            return null;
        }

        private void CompletedSpecializedReportJob(int specializedReportJobId, string downloadUrl)
        {
            var specializedReportJob = populateReportingService.GetSpecializedReportJob(specializedReportJobId);
            specializedReportJob.DownloadUrl = downloadUrl;
            specializedReportJob.Status = 1;
            populateReportingService.SaveSpecializedReportJob(specializedReportJob);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult CheckACTS3FileExisted(string fileName)
        {
            var folder = ConfigurationManager.AppSettings["ACTReportFolder"];
            var bucketName = LinkitConfigurationManager.GetS3Settings().ACTReportBucket;

            var result = s3Service.DownloadFile(bucketName, folder + "/" + fileName);

            if (result.IsSuccess)
            {
                //var s3Url = string.Format("https://s3.amazonaws.com/{0}/{1}/{2}", bucketName, folder, fileName);
                var s3Url = s3Service.GetPublicUrl(bucketName, folder + "/" + fileName);
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
        public ActionResult ACTGetSchools(int virtualtestId, int districtId)
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
        public ActionResult ACTGetTeachers(int schoolId, int virtualTestId)
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
        public ActionResult ACTGetStudents(int classId, int virtualTestId)
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
        public ActionResult MultipleTestACTGetStudents(int classId, string virtualTestIdString, DateTime? resultDateFrom,
            DateTime? resultDateTo, bool? isGetAllClass)
        {
            var classStudents = MultipleTestGetStudents(classId, virtualTestIdString, resultDateFrom, resultDateTo, isGetAllClass ?? false);

            var data = classStudents.Select(x => new { x.StudentId, x.FullName }).OrderBy(x => x.FullName);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private List<ClassStudent> MultipleTestGetStudents(int classId, string virtualTestIdString, DateTime? resultDateFrom,
            DateTime? resultDateTo, bool isGetAllClass = false)
        {
            var virtualTestIdsList = virtualTestIdString.ToIntList(",");

            var lstStudentIds = new List<int>();

            if (!isGetAllClass)
            {
                lstStudentIds =
                testResultService.GetTestResultsByVirtualTestIds(virtualTestIdsList, resultDateFrom, resultDateTo)
                    .Where(o => o.ClassId == classId)
                    .Select(o => o.StudentId).Distinct().ToList();
                //TODO: should check lstTeacher < 2100 ^_^
            }
            else
            {
                var param = new StudentTestDistrictTermParam
                {
                    ClassId = classId,
                    VirtualTestIds = virtualTestIdsList,
                    ResultDateFrom = resultDateFrom,
                    ResultDateTo = resultDateTo
                };
                lstStudentIds = teacherDistrictTermService.GetStudentTestDistrictTerm_New(param).Select(x => x.StudentId).Distinct().ToList();
            }

            var data = classStudentService.GetClassStudentsByClassId(classId)
                .Where(o => lstStudentIds.Contains(o.StudentId))
                .ToList();

            return data;
        }

        #endregion

        private ACTReportMasterViewModel BuildReportMasterModel(int studentID, int districtID, int testID, int teacherID, int generatedUserID
            , int reportContentOption, bool useNewACTStudentFormat, int? takenReportTeacherId, int? takenReportClassId)
        {
            var masterModel = new ACTReportMasterViewModel
            {
                SectionTagViewModels = new List<SectionTagViewModel>(),
                ReportContentOption = reportContentOption,
                UseNewACTStudentFormat = useNewACTStudentFormat,
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

            BuildDiagnosticHistoryData(masterModel, studentID, testID, teacherID);

            BuildSessionTagData(masterModel, studentID, testID, districtID);

            BuildStudentInformationData(masterModel, studentID, generatedUserID, takenReportTeacherId, takenReportClassId);

            BuildSummaryScoreData(masterModel);

            BuildAdditionalFormatOption(masterModel);

            BuildOptionBoldZeroPercentScore(masterModel);

            BuildEssayCommentData(masterModel);

            return masterModel;
        }

        private void BuildEssayCommentData(ACTReportMasterViewModel model)
        {
            model.NewACTEssayComments = new List<NewACTEssayComment>();

            if (model.VirtualTestSubTypeId != (int)VirtualTestSubType.NewACT)
                return;

            var districtDecode =
                districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(model.DistrictId,
                    Util.NewACTReport_ShowComment).FirstOrDefault();
            if (districtDecode == null || districtDecode.Value != "1")
                return;

            districtDecode =
                districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(model.DistrictId,
                    Util.NewACTEssayComment_Title).FirstOrDefault();
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

                model.NewACTEssayComments.Add(SetEssayCommentByTagIndex(model.DistrictId, domainTag, i));
            }
        }

        private List<string> GetDomainTagNamesOfSelectedTest(ACTReportMasterViewModel model)
        {
            var domainTagNames = new List<string>();
            if (model.DomainTagData.Any())
            {
                var testResultId =
                    model.DiagnosticHistoryViewModel.TestAndScoreViewModels.FirstOrDefault(x => x.IsSelected)
                        .TestResultID;
                domainTagNames =
                    model.DomainTagData.Where(x => x.TestResultID == testResultId)
                        .Select(x => new { x.TagName, x.MinQuestionOrder })
                        .Distinct()
                        .OrderBy(x => x.MinQuestionOrder)
                        .Select(x => x.TagName)
                        .ToList();
                if (domainTagNames.Any() == false)
                {
                    var newACTTest =
                        model.DiagnosticHistoryViewModel.TestAndScoreViewModels.FirstOrDefault(
                            x => x.VirtualTestSubTypeID == (int)VirtualTestSubType.NewACT);
                    if (newACTTest != null)
                    {
                        testResultId = newACTTest.TestResultID;
                        domainTagNames =
                            model.DomainTagData.Where(x => x.TestResultID == testResultId)
                                .Select(x => new { x.TagName, x.MinQuestionOrder })
                                .Distinct()
                                .OrderBy(x => x.MinQuestionOrder)
                                .Select(x => x.TagName)
                                .ToList();
                    }
                }
            }

            return domainTagNames;
        }

        private NewACTEssayComment SetEssayCommentByTagIndex(int districtId, ACTSectionTagData sectionTagData, int tagIndex)
        {
            var essayComment = new NewACTEssayComment
            {
                Score = sectionTagData.CorrectAnswer,
                TagName = sectionTagData.TagName,
            };

            var districtDecode =
                districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(districtId,
                    string.Format(Util.NewACTEssayComment_ScoreRangeKey, tagIndex)).FirstOrDefault();
            if (districtDecode != null)
            {
                essayComment.ScoreRange = districtDecode.Value;
            }

            districtDecode =
                districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(districtId,
                    string.Format(Util.NewACTEssayComment_ScoreKey, tagIndex, sectionTagData.CorrectAnswer))
                    .FirstOrDefault();
            if (districtDecode != null)
            {
                essayComment.EssayComment = districtDecode.Value;
            }

            return essayComment;
        }

        private void BuildAdditionalFormatOption(ACTReportMasterViewModel masterModel)
        {
            var districtDecodeLabel = masterModel.VirtualTestSubTypeId == (int)VirtualTestSubType.ACT
                ? Constanst.ACTReportShowTableBorder
                : Constanst.NewACTReportShowTableBorder;
            masterModel.ShowTableBorder = districtDecodeService.GetDistrictDecodeByLabel(masterModel.DistrictId,
                districtDecodeLabel);
        }

        private void BuildOptionBoldZeroPercentScore(ACTReportMasterViewModel masterModel)
        {
            var districtDecodeLabel = masterModel.VirtualTestSubTypeId == (int)VirtualTestSubType.ACT
                ? Constanst.ACTReportBoldZeroPercentScore
                : Constanst.NewACTReportBoldZeroPercentScore;
            masterModel.BoldZeroPercentScore = districtDecodeService.GetDistrictDecodeByLabel(masterModel.DistrictId,
                districtDecodeLabel);
        }

        private bool GetTagTableUseAlternativeStyleOption(int districtId)
        {
            var result = false;
            var districtDecode =
                districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(
                    districtId, Util.ACTReport_TagTable_UseAlternativeStyle)
                    .FirstOrDefault();
            if (districtDecode != null)
                result = districtDecode.Value == "true";

            return result;
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
            if (!string.IsNullOrEmpty(value))
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

            return value;
        }

        [HttpPost]
        [AjaxAwareAuthorize]
        public ActionResult LoadACTStudentReportTab(ACTReportViewModel model)
        {
            model.IsAdmin = userService.GetAdminByUsernameAndDistrict(CurrentUser.UserName, CurrentUser.DistrictId.GetValueOrDefault()).IsNotNull();
            model.CanSelectTeachers = CurrentUser.IsDistrictAdminOrPublisher;
            model.IsSchoolAdmin = CurrentUser.RoleId.Equals(8);
            model.IsPublisher = CurrentUser.RoleId.Equals((int)Permissions.Publisher);
            model.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
            model.ResultDateFrom = DateTime.Now.AddMonths(-1).DisplayDateWithFormat();
            model.ResultDateTo = DateTime.Now.DisplayDateWithFormat();
            return PartialView("_StudentReportTab", model);
        }

        public ActionResult LoadACTStudentReportTabV2(ACTReportViewModel model)
        {
            model.IsAdmin = userService.GetAdminByUsernameAndDistrict(CurrentUser.UserName, CurrentUser.DistrictId.GetValueOrDefault()).IsNotNull();
            model.CanSelectTeachers = CurrentUser.IsDistrictAdminOrPublisher;
            model.IsSchoolAdmin = CurrentUser.RoleId.Equals(8);
            model.IsPublisher = CurrentUser.RoleId.Equals((int)Permissions.Publisher);
            model.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
            model.ResultDateFrom = DateTime.Now.AddMonths(-1).DisplayDateWithFormat();
            model.ResultDateTo = DateTime.Now.DisplayDateWithFormat();
            return PartialView("v2/_StudentReportTab", model);
        }

        [HttpPost]
        public ActionResult LoadNewACTStudentReportTab(ACTReportViewModel model)
        {
            model.IsAdmin = userService.GetAdminByUsernameAndDistrict(CurrentUser.UserName, CurrentUser.DistrictId.GetValueOrDefault()).IsNotNull();
            model.CanSelectTeachers = CurrentUser.IsDistrictAdminOrPublisher;
            model.IsSchoolAdmin = CurrentUser.RoleId.Equals(8);
            model.IsPublisher = CurrentUser.RoleId.Equals((int)Permissions.Publisher);
            model.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
            model.ResultDateFrom = DateTime.Now.AddMonths(-1).DisplayDateWithFormat();
            model.ResultDateTo = DateTime.Now.DisplayDateWithFormat();
            return PartialView("_NewStudentReportTab", model);
        }

        [HttpPost]
        public ActionResult LoadNewACTStudentReportTabV2(ACTReportViewModel model)
        {
            model.IsAdmin = userService.GetAdminByUsernameAndDistrict(CurrentUser.UserName, CurrentUser.DistrictId.GetValueOrDefault()).IsNotNull();
            model.CanSelectTeachers = CurrentUser.IsDistrictAdminOrPublisher;
            model.IsSchoolAdmin = CurrentUser.RoleId.Equals(8);
            model.IsPublisher = CurrentUser.RoleId.Equals((int)Permissions.Publisher);
            model.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
            model.ResultDateFrom = DateTime.Now.AddMonths(-1).DisplayDateWithFormat();
            model.ResultDateTo = DateTime.Now.DisplayDateWithFormat();
            return PartialView("v2/_NewStudentReportTab", model);
        }

        public ActionResult LoadACTSummaryReportTab(ACTReportViewModel model)
        {
            model.IsAdmin = userService.GetAdminByUsernameAndDistrict(CurrentUser.UserName, CurrentUser.DistrictId.GetValueOrDefault()).IsNotNull();
            model.CanSelectTeachers = CurrentUser.IsDistrictAdminOrPublisher;
            model.IsSchoolAdmin = CurrentUser.RoleId.Equals(8);
            model.IsPublisher = CurrentUser.RoleId.Equals((int)Permissions.Publisher);
            model.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
            return PartialView("_SummaryReportTab", model);
        }

        public ActionResult LoadACTSummaryReportTabV2(ACTReportViewModel model)
        {
            model.IsAdmin = userService.GetAdminByUsernameAndDistrict(CurrentUser.UserName, CurrentUser.DistrictId.GetValueOrDefault()).IsNotNull();
            model.CanSelectTeachers = CurrentUser.IsDistrictAdminOrPublisher;
            model.IsSchoolAdmin = CurrentUser.RoleId.Equals(8);
            model.IsPublisher = CurrentUser.RoleId.Equals((int)Permissions.Publisher);
            model.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
            return PartialView("v2/_SummaryReportTab", model);
        }

        public ActionResult LoadSATStudentReportTab(ACTReportViewModel model)
        {
            model.IsAdmin = userService.GetAdminByUsernameAndDistrict(CurrentUser.UserName, CurrentUser.DistrictId.GetValueOrDefault()).IsNotNull();
            model.CanSelectTeachers = CurrentUser.IsDistrictAdminOrPublisher;
            model.IsSchoolAdmin = CurrentUser.RoleId.Equals(8);
            model.IsPublisher = CurrentUser.RoleId.Equals((int)Permissions.Publisher);
            model.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
            return PartialView("_SATStudentReportTab", model);
        }

        public ActionResult LoadSATStudentReportTabV2(ACTReportViewModel model)
        {
            model.IsAdmin = userService.GetAdminByUsernameAndDistrict(CurrentUser.UserName, CurrentUser.DistrictId.GetValueOrDefault()).IsNotNull();
            model.CanSelectTeachers = CurrentUser.IsDistrictAdminOrPublisher;
            model.IsSchoolAdmin = CurrentUser.RoleId.Equals(8);
            model.IsPublisher = CurrentUser.RoleId.Equals((int)Permissions.Publisher);
            model.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
            return PartialView("v2/_SATStudentReportTab", model);
        }

        public ActionResult LoadNewSATStudentReportTab(ACTReportViewModel model)
        {
            model.IsAdmin = userService.GetAdminByUsernameAndDistrict(CurrentUser.UserName, CurrentUser.DistrictId.GetValueOrDefault()).IsNotNull();
            model.CanSelectTeachers = CurrentUser.IsDistrictAdminOrPublisher;
            model.IsSchoolAdmin = CurrentUser.RoleId.Equals(8);
            model.IsPublisher = CurrentUser.RoleId.Equals((int)Permissions.Publisher);
            model.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
            model.ResultDateFrom = DateTime.Now.AddMonths(-1).DisplayDateWithFormat();
            model.ResultDateTo = DateTime.Now.DisplayDateWithFormat();
            return PartialView("_NewSATStudentReportTab", model);
        }

        public ActionResult LoadNewSATStudentReportTabV2(ACTReportViewModel model)
        {
            model.IsAdmin = userService.GetAdminByUsernameAndDistrict(CurrentUser.UserName, CurrentUser.DistrictId.GetValueOrDefault()).IsNotNull();
            model.CanSelectTeachers = CurrentUser.IsDistrictAdminOrPublisher;
            model.IsSchoolAdmin = CurrentUser.RoleId.Equals(8);
            model.IsPublisher = CurrentUser.RoleId.Equals((int)Permissions.Publisher);
            model.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
            model.ResultDateFrom = DateTime.Now.AddMonths(-1).DisplayDateWithFormat();
            model.ResultDateTo = DateTime.Now.DisplayDateWithFormat();
            return PartialView("v2/_NewSATStudentReportTab", model);
        }

        public ActionResult LoadSATSummaryReportTab(ACTReportViewModel model)
        {
            model.IsAdmin = userService.GetAdminByUsernameAndDistrict(CurrentUser.UserName, CurrentUser.DistrictId.GetValueOrDefault()).IsNotNull();
            model.CanSelectTeachers = CurrentUser.IsDistrictAdminOrPublisher;
            model.IsSchoolAdmin = CurrentUser.RoleId.Equals(8);
            model.IsPublisher = CurrentUser.RoleId.Equals((int)Permissions.Publisher);
            model.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
            return PartialView("_SATSummaryReportTab", model);
        }

        public ActionResult LoadSATSummaryReportTabV2(ACTReportViewModel model)
        {
            model.IsAdmin = userService.GetAdminByUsernameAndDistrict(CurrentUser.UserName, CurrentUser.DistrictId.GetValueOrDefault()).IsNotNull();
            model.CanSelectTeachers = CurrentUser.IsDistrictAdminOrPublisher;
            model.IsSchoolAdmin = CurrentUser.RoleId.Equals(8);
            model.IsPublisher = CurrentUser.RoleId.Equals((int)Permissions.Publisher);
            model.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
            return PartialView("v2/_SATSummaryReportTab", model);
        }

        [HttpGet]
        public ActionResult GetSchools(int? districtId, int? virtualtestId)
        {
            if (!districtId.HasValue)
            {
                districtId = CurrentUser.DistrictId;
            }

            var data = teacherDistrictTermService.GetTeacherTestDistrictTerm(districtId,
                    null, null, null, virtualtestId, (int)VirtualTestSubType.ACT).ToList()
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
                schoolId, null, null, virtualtestId, (int)VirtualTestSubType.ACT).ToList()
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
                    schoolId, userId, null, virtualTestId, (int)VirtualTestSubType.ACT).ToList()
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
                districtId, schoolId, userId, termId, virtualTestId, (int)VirtualTestSubType.ACT).ToList()
                .Select(x => new { x.ClassId, x.ClassName })
                .Distinct()
                .OrderBy(x => x.ClassName)
                .Select(x => new ListItem { Id = x.ClassId, Name = x.ClassName });

            return Json(data, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult GenerateFromAPI(GenerateReportViewModel model)
        {
            var result = new GenerateReportResultViewModel();
            if (!string.IsNullOrEmpty(model.ReportType) && model.TestResultId.HasValue && model.APIAccountID.HasValue)
            {
                var apiAccount = GetApiAccount(model.APIAccountID.Value);

                var newACTReportData = BuildModelForGenerateNewACTStudentReport(model, apiAccount);
                result = GenerateNewACTStudentReport(newACTReportData);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private GenerateReportResultViewModel GenerateNewACTStudentReport(NewACTReportData model)
        {
            if (!model.DistrictId.HasValue)
            {
                model.DistrictId = 272;
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

            var reportFileResultModels = PrintOneFileNewACT(model.DistrictId.Value, model.SchoolId, model.TeacherId, model.DistrictTermId,
                model.ClassId
                , model.StrTestIdList.Split(',').Select(x => Convert.ToInt32(x)).ToList()
                , listStudentId
                , model.ResultDateFrom, model.ResultDateTo
                , model.SpecializedReportJobId
                , model.TimezoneOffset, model.ReportContentOption
                , model.isGetAllClass
                , out reportUrl);

            var s3Url = CreateNewACTFileAndUploadToS3(model, reportFileResultModels);
            if (model.SpecializedReportJobId > 0)
                CompletedSpecializedReportJob(model.SpecializedReportJobId, s3Url);

            return new GenerateReportResultViewModel() { IsSuccess = true, ReportUrl = s3Url };
        }

        private NewACTReportData BuildModelForGenerateNewACTStudentReport(GenerateReportViewModel model, APIAccountViewModel apiAccount)
        {
            var testResult = testResultService.GetTestResultById(model.TestResultId.Value);
            if (testResult != null)
            {
                var suffix = DateTime.UtcNow.AddMinutes(apiAccount.TimezoneOffset * (-1)).ToString("MM-dd-HH-mm");
                suffix += "_";
                suffix += Guid.NewGuid().ToString().Replace("-", "").Substring(2, 14);

                var prefix = "ACT_";

                var data = new NewACTReportData()
                {
                    StrTestIdList = testResult.VirtualTestId.ToString(),
                    UserId = apiAccount.UserId == 0 ? testResult.UserId : apiAccount.UserId,
                    RoleId = apiAccount.RoleId,
                    DistrictId = apiAccount.DistrictId,
                    SchoolId = testResult.SchoolId,
                    ClassId = testResult.ClassId,
                    TeacherId = testResult.UserId,
                    DistrictTermId = testResult.DistrictTermId,
                    StudentIdList = new List<string> { testResult.StudentId.ToString() },
                    TimezoneOffset = apiAccount.TimezoneOffset,
                    ReportContentOption = 1,
                    ResultDateFrom = testResult.ResultDate,
                    ResultDateTo = testResult.ResultDate,
                    ActReportFileName = prefix + suffix
                };
                return data;
            }
            return new NewACTReportData();
        }

        private APIAccountViewModel GetApiAccount(int apiAccountId)
        {
            var apiAccountModel = new APIAccountViewModel();
            var apiAccount = APIAccountService.GetAPIAccountById(apiAccountId);
            if (apiAccount != null)
            {
                if (apiAccount.APIAccountTypeID == 1) //district
                {
                    apiAccountModel.DistrictId = apiAccount.TargetID;
                    apiAccountModel.RoleId = 3; //districtadmin
                }
                else
                {
                    var user = userService.GetUserById(apiAccount.TargetID);
                    if (user != null)
                    {
                        apiAccountModel.DistrictId = user.DistrictId ?? 0;
                        apiAccountModel.UserId = user.Id;
                        apiAccountModel.RoleId = user.RoleId;
                    }
                }
            }
            return apiAccountModel;
        }
    }
}
