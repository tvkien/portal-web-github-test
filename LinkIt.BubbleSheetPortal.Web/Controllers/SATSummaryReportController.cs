using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web.Mvc;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Models.ACTSummaryReport;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.ViewModels.ACTReport;
using LinkIt.BubbleSheetPortal.Web.ViewModels.ACTSummaryReport;
using LinkIt.BubbleSheetPortal.Web.ViewModels.SATSummaryReport;
using LinkIt.BubbleSheetPortal.Web.ServiceConsumer;
using S3Library;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class SATSummaryReportController : BaseController
    {
        private readonly DistrictService districtService;
        private readonly SchoolService schoolService;
        private readonly ClassService classService;
        private readonly UserService userService;
        private readonly ACTReportService actReportService;
        private readonly VirtualTestService virtualTestService;
        private readonly DistrictDecodeService districtDecodeService;
        private readonly DistrictTermService districtTermService;

        private readonly IS3Service _s3Service;

        public SATSummaryReportController(DistrictService districtService, ACTReportService actReportService, VirtualTestService virtualTestService
            , SchoolService schoolService, ClassService classService, UserService userService, DistrictDecodeService districtDecodeService
            , DistrictTermService districtTermService, IS3Service s3Service)
        {
            this.districtService = districtService;
            this.actReportService = actReportService;
            this.virtualTestService = virtualTestService;
            this.schoolService = schoolService;
            this.classService = classService;
            this.userService = userService;
            this.districtDecodeService = districtDecodeService;
            this.districtTermService = districtTermService;
            _s3Service = s3Service;
        }

        public ActionResult ReportPrinting(SATSummaryReportData model)
        {
            if (model.ImprovementOption == "select")
                model.ImprovementOption = "";

            var listStudentID = new List<int>();
            var parseStudentIDList = string.IsNullOrEmpty(model.StrStudentIdList)
                                         ? new List<string>()
                                         : model.StrStudentIdList.Split(new[] { ',' },
                                                                        StringSplitOptions.RemoveEmptyEntries).ToList();
            var parseTestIDList = string.IsNullOrEmpty(model.StrTestIdList)
                                      ? new List<int>()
                                      : model.StrTestIdList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).
                                            Select(
                                                en => Convert.ToInt32(en)).ToList();

            var vm = BuildSatSummaryReportViewModel(model.DistrictId.Value, parseTestIDList, model.SchoolId, model.ClassId, model.TeacherId, model.DistrictTermId);
            vm.VirtualTestSubTypeId = model.VirtualTestSubTypeId;

            if (model.ClassId.HasValue) //student/class level
            {
                List<ACTSummaryClassLevelData> data;

                foreach (var strStudentID in parseStudentIDList)
                {
                    int studentID;
                    if (int.TryParse(strStudentID, out studentID))
                    {
                        listStudentID.Add(studentID);
                    }
                }
                if (string.IsNullOrEmpty(model.StrTestIdList))
                {
                    vm.ListTestReport.Add(BuildClassLevelReport(model.ClassId.Value, model.TestId, listStudentID, out data, model.VirtualTestSubTypeId));
                }
                else
                {
                    data = new List<ACTSummaryClassLevelData>();
                    var listTestID = new List<int>();
                    List<ACTSummaryClassLevelData> subData;
                    foreach (var testID in parseTestIDList)
                    {
                        listTestID.Add(testID);
                        vm.ListTestReport.Add(BuildClassLevelReport(model.ClassId.Value,
                            testID, listStudentID, out subData, model.VirtualTestSubTypeId));
                        data.AddRange(subData);
                    }
                }
                ReOrderVirtualTest(vm);

                var baseLineTestReport = BuildClassLevelBaselineReport(model.ClassId.Value, parseTestIDList, listStudentID, model.VirtualTestSubTypeId);
                AddMissingDataInTestItem(vm.ListTestReport, baseLineTestReport);
                vm.ListTestReport.Add(baseLineTestReport);

                vm.ListTestReport.Add(BuildClassLevelImprovementReport(model.ClassId.Value, parseTestIDList, listStudentID, model.ImprovementOption, model.VirtualTestSubTypeId));
            }
            else if (model.TeacherId.HasValue) //teacher level
            {
                vm.ListTestReport = BuildTeacherLevelReport(model.TeacherId.Value, model.DistrictTermId.GetValueOrDefault(), parseTestIDList, model.VirtualTestSubTypeId);
                ReOrderVirtualTest(vm);
                
                var baseLineTestReport = BuildTeacherLevelBaselineReport(model.TeacherId.Value, model.DistrictTermId.GetValueOrDefault(), parseTestIDList, model.VirtualTestSubTypeId);
                AddMissingDataInTestItem(vm.ListTestReport, baseLineTestReport);
                vm.ListTestReport.Add(baseLineTestReport);
                vm.ListTestReport.Add(BuildTeacherLevelImprovementReport(model.TeacherId.Value, model.DistrictTermId.GetValueOrDefault(), parseTestIDList, model.ImprovementOption, model.VirtualTestSubTypeId));
            }
            else if (model.SchoolId.HasValue) //school level
            {
                vm.ListTestReport = BuildSchoolLevelReport(model.SchoolId.Value, model.DistrictTermId.GetValueOrDefault(), parseTestIDList, model.VirtualTestSubTypeId);
                ReOrderVirtualTest(vm);
                
                var baseLineTestReport = BuildSchoolLevelBaselineReport(model.SchoolId.Value, model.DistrictTermId.GetValueOrDefault(), parseTestIDList, model.VirtualTestSubTypeId);
                AddMissingDataInTestItem(vm.ListTestReport, baseLineTestReport);

                vm.ListTestReport.Add(baseLineTestReport);
                vm.ListTestReport.Add(BuildSchoolLevelImprovementReport(model.SchoolId.Value, model.DistrictTermId.GetValueOrDefault(), parseTestIDList, model.ImprovementOption, model.VirtualTestSubTypeId));
            }
            else //district level
            {
                vm.ListTestReport = BuildDistrictLevelReport(model.DistrictId.Value, model.DistrictTermId.GetValueOrDefault(), parseTestIDList, model.VirtualTestSubTypeId);
                ReOrderVirtualTest(vm);
                
                var baseLineTestReport = BuildDistrictLevelBaselineReport(model.DistrictId.Value, model.DistrictTermId.GetValueOrDefault(), parseTestIDList, model.VirtualTestSubTypeId);
                AddMissingDataInTestItem(vm.ListTestReport, baseLineTestReport);

                vm.ListTestReport.Add(baseLineTestReport);
                vm.ListTestReport.Add(BuildDistrictLevelImprovementReport(model.DistrictId.Value, model.DistrictTermId.GetValueOrDefault(), parseTestIDList, model.ImprovementOption, model.VirtualTestSubTypeId));
            }

            FixDataForImprovement(vm);
            ReorderSubjectInViewModel(vm, model.DistrictId.GetValueOrDefault());

            IncludeJSCss(vm);
            return View("ReportPrinting", vm);
        }

        private void IncludeJSCss(SATSummaryReportViewModel model)
        {
            var cssPaths = new List<string>
            {

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

        private void ReOrderVirtualTest(SATSummaryReportViewModel vm)
        {
            var testNames = vm.TestNames.Split(',').Select(x => x.Trim()).ToList();
            vm.ListTestReport = vm.ListTestReport.OrderBy(x => testNames.IndexOf(x.TestName.Trim())).ToList();
        }

        private void FixDataForImprovement(SATSummaryReportViewModel vm)
        {
            var improvementTest = vm.ListTestReport.FirstOrDefault(x => x.TestName == SATConstants.ImprovementTestName);
            var baselineTest = vm.ListTestReport.FirstOrDefault(x => x.TestName == SATConstants.BaselineTestName);
            if (improvementTest == null || baselineTest == null) return;

            var allBaselineSubject =
                baselineTest.ListDataInTest.SelectMany(x => x.SubScores.Select(y => y.SectionName)).Distinct().ToList();
            var allImprovementTest =
                improvementTest.ListDataInTest.SelectMany(x => x.SubScores.Select(y => y.SectionName)).Distinct().ToList();
            var listTestImprovementMissing =
                allBaselineSubject.Where(x => allImprovementTest.Contains(x) == false).ToList();
            foreach (var improvementDataInTest in improvementTest.ListDataInTest)
            {
                AddMissingSubjectForDataInTest(listTestImprovementMissing, improvementDataInTest);
            }
            AddMissingSubjectForDataInTest(listTestImprovementMissing, improvementTest.AverageScores);
            vm.ListTestReport.RemoveAt(vm.ListTestReport.FindIndex(x => x.TestName == SATConstants.ImprovementTestName));
            vm.ListTestReport.Add(improvementTest);
        }

        private static void AddMissingSubjectForDataInTest(List<string> listTestImprovementMissing,
            SATDataInTestReportViewModel improvementDataInTest)
        {
            foreach (var missingSubject in listTestImprovementMissing)
            {
                if (improvementDataInTest.SubScores.Any(x => x.SectionName == missingSubject)) continue;
                improvementDataInTest.SubScores.Add(new SATSummarySubScore()
                                                    {
                                                        Score = -1,
                                                        SectionName = missingSubject
                                                    });
            }
        }

        private void AddMissingDataInTestItem(List<SATSingleTestReportViewModel> listTestReports, SATSingleTestReportViewModel baselineTestReport)
        {
            foreach (var testReport in listTestReports)
            {
                foreach (var dataInTest in baselineTestReport.ListDataInTest)
                {
                    if (testReport.ListDataInTest.All(x => x.ID != dataInTest.ID))
                    {
                        testReport.ListDataInTest.Add(new SATDataInTestReportViewModel
                        {
                            ReportType = dataInTest.ReportType,
                            CompositeScore = -1,
                            DisplayName = dataInTest.DisplayName,
                            ID = dataInTest.ID,
                            SortName = dataInTest.SortName,
                            TotalStudents = 0,
                            SubScores = new List<SATSummarySubScore>()
                        });

                    }
                }
                testReport.ListDataInTest = testReport.ListDataInTest.OrderBy(en => en.SortName).ThenBy(en => en.ID).ToList();
            }
        }

        private SATSummaryReportViewModel BuildSatSummaryReportViewModel(int districtId, List<int> testIds, int? schoolID, int? classId, int? teacherId, int? districtTermId)
        {
            var viewModel = new SATSummaryReportViewModel
            {
                SchoolName = "ALL",
                ClassName = "ALL",
                TeacherName = "ALL",
                DistrictTermName = "ALL"
            };
            var listTestReport = new List<SATSingleTestReportViewModel>();

            var logoUrl = string.Format("{0}{1}-logo.png", LinkitConfigurationManager.GetS3Settings().S3CSSKey, districtId);

            if (UrlUtil.CheckUrlStatus(logoUrl))
            {
                viewModel.DistrictLogoUrl = logoUrl;
                viewModel.DistrictName = string.Empty;
            }
            else
            {
                var district = districtService.GetDistrictById(districtId);
                viewModel.DistrictLogoUrl = string.Empty;
                viewModel.DistrictName = district.Name;
            }

            if (schoolID.HasValue)
            {
                viewModel.SchoolName = schoolService.GetSchoolById(schoolID.Value).Name;
            }

            if (classId.HasValue)
            {
                viewModel.ClassName = classService.GetClassByIdWithoutFilterByActiveTerm(classId.Value).Name;
            }

            if (teacherId.HasValue)
            {
                var teacher = userService.GetUserById(teacherId.Value);
                viewModel.TeacherName = Helpers.Util.FormatFullname(teacher.LastName + ", " + teacher.FirstName);
            }

            if (districtTermId.HasValue)
            {
                var districtTerm = districtTermService.GetDistrictTermById(districtTermId.Value);
                viewModel.DistrictTermName = districtTerm.Name;                
            }

            var testNames = "";
            foreach (var testId in testIds)
            {

                var virtualTest = virtualTestService.Select().FirstOrDefault(en => en.VirtualTestID == testId);
                if (virtualTest != null)
                {
                    testNames += virtualTest.Name + ", ";
                }
            }
            if (testNames.Trim().EndsWith(","))
                testNames = testNames.Trim().Substring(0, testNames.Trim().Length - 1);
            viewModel.TestNames = testNames;

            viewModel.ListTestReport = listTestReport;
            return viewModel;
        }

        private List<SATDataInTestReportViewModel> BuildBaselineDistrictLevelReport(List<ACTSummaryDistrictLevelData> data)
        {
            var baseLineData = new List<SATDataInTestReportViewModel>();
            var schoolList = data.Select(x => new
            {
                x.SchoolID,
                x.SchoolName
            }).DistinctBy(x => x.SchoolID).OrderBy(x => x.SchoolName).ToList();
            foreach (var school in schoolList)
            {
                var schoolData = data.Where(x => x.SchoolID == school.SchoolID).ToList();

                var allSectionNames = GetAllSectionNames(schoolData);
                var firstResultDates =
                    allSectionNames.Select(
                        sectionName => new
                                           {
                                               SectionName = sectionName,
                                               FirstSectionResultDate =
                                           schoolData.Where(
                                               x => !string.IsNullOrEmpty(x.SectionName) && x.SectionName.Trim()
                                                                                                .Equals(sectionName,
                                                                                                        StringComparison
                                                                                                            .
                                                                                                            InvariantCultureIgnoreCase) &&
                                                    x.SchoolID == school.SchoolID).Min(x => x.ResultDate)
                                           }
                        ).ToList();

                var dataInTest = new SATDataInTestReportViewModel();
                dataInTest.DisplayName = school.SchoolName;
                dataInTest.ID = school.SchoolID;
                dataInTest.TotalStudents = schoolData.Count;
                dataInTest.SubScores = firstResultDates
                    .Select(x => new SATSummarySubScore
                                     {
                                         SectionName = x.SectionName,
                                         Score = GetSectionScoreBySectionNameAndSchoolID(x.SectionName,
                                                                                         school.SchoolID,
                                                                                         schoolData.Where(
                                                                                             en =>
                                                                                             en.ResultDate ==
                                                                                             x.FirstSectionResultDate).
                                                                                             ToList())
                                     }).ToList();
                baseLineData.Add(dataInTest);
            }
            return baseLineData;
        }

        private SATSingleTestReportViewModel BuildClassLevelReport(int classID, int virtualTestID, List<int> listStudentIDs, out List<ACTSummaryClassLevelData> data, int virtualTestSubTypeId)
        {
            data = actReportService.GetSATSummaryClassLevelData(classID, virtualTestID, virtualTestSubTypeId);
            data = data.Where(x => listStudentIDs.Contains(x.StudentID)).ToList();
            var studentList = data.Select(x => new
            {
                x.StudentDisplayName,
                x.StudentID
            }).Distinct().OrderBy(x => x.StudentDisplayName).ToList();
            var test = virtualTestService.Select().First(x => x.VirtualTestID == virtualTestID);
            var viewModel = new SATSingleTestReportViewModel();
            viewModel.ListDataInTest = new List<SATDataInTestReportViewModel>();
            viewModel.ReportType = SATSummaryReportType.ClassLevel;
            viewModel.TestName = test.Name;

            var allSectionNames = GetAllSectionNames(data);

            foreach (var student in studentList)
            {
                var dataInTest = new SATDataInTestReportViewModel();
                dataInTest.DisplayName = student.StudentDisplayName;
                dataInTest.SortName = student.StudentDisplayName;
                dataInTest.ID = student.StudentID;

                dataInTest.SubScores = new List<SATSummarySubScore>();
                foreach (var sectionName in allSectionNames)
                {
                    var subScore = new SATSummarySubScore
                                       {
                                           SectionName = sectionName,
                                           Score = GetSectionScoreBySectionNameAndStudentID(
                                               sectionName, student.StudentID, data)
                                       };
                    dataInTest.SubScores.Add(subScore);
                }

                dataInTest.CompositeScore = GetSectionScoreBySectionNameAndStudentID("composite", student.StudentID, data);
                dataInTest.ReportType = SATSummaryReportType.ClassLevel;

                viewModel.ListDataInTest.Add(dataInTest);
            }

            viewModel.CalculateAverageScores();
            return viewModel;
        }

        private List<SATSingleTestReportViewModel> BuildTeacherLevelReport(int userId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            var data = actReportService.GetSATSummaryTeacherLevelData(userId, districtTermId, virtualTestIds, virtualTestSubTypeId);
            var averageData = actReportService.GetSATSummaryTeacherLevelAverageData(userId, districtTermId, virtualTestIds, virtualTestSubTypeId);

            var testIds = data.Select(en => en.VirtualTestId).Distinct();
            var testModels = new List<SATSingleTestReportViewModel>();

            var allSectionNames = GetAllSectionNames(data);

            foreach (var testId in testIds)
            {
                var testData = data.Where(en => en.VirtualTestId == testId).ToList();
                var testName = testData.FirstOrDefault().TestName;

                var singleTestModel = BuildSchoolOrTeacherSingleTestModel(testData, testName, allSectionNames, SATSummaryReportType.TeacherLevel);
                singleTestModel.AverageScores = BuildSchoolOrTeacherAverageData(averageData, allSectionNames, testId, SATSummaryReportType.TeacherLevel);
                testModels.Add(singleTestModel);
            }

            return testModels;
        }

        private SATSingleTestReportViewModel BuildTeacherLevelBaselineReport(int userId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            var testData = actReportService.GetSATSummaryTeacherLevelBaselineData(userId, districtTermId, virtualTestIds, virtualTestSubTypeId);

            var allSectionNames = GetAllSectionNames(testData);

            var singleTestModel = BuildSchoolOrTeacherSingleTestModel(testData, SATConstants.BaselineTestName, allSectionNames, SATSummaryReportType.TeacherLevel);
            singleTestModel.AverageScores = BuildSchoolOrTeacherImprovementOrBaselineData(testData, allSectionNames,
                                                                                SATSummaryReportType.TeacherLevel);
            return singleTestModel;
        }

        private SATSingleTestReportViewModel BuildTeacherLevelImprovementReport(int userId, int districtTermId, List<int> virtualTestIds, string improvementOption, int virtualTestSubTypeId)
        {
            var testData = actReportService.GetSATSummaryTeacherLevelImprovementData(userId, districtTermId, virtualTestIds, improvementOption, virtualTestSubTypeId);

            var allSectionNames = GetAllSectionNames(testData);

            var singleTestModel = BuildSchoolOrTeacherSingleTestModel(testData, SATConstants.ImprovementTestName, allSectionNames, SATSummaryReportType.TeacherLevel);
            singleTestModel.AverageScores = BuildSchoolOrTeacherImprovementOrBaselineData(testData, allSectionNames,
                                                                                SATSummaryReportType.TeacherLevel);
            return singleTestModel;
        }

        private SATSingleTestReportViewModel BuildSchoolLevelImprovementReport(int schoolId, int districtTermId, List<int> virtualTestIds, string improvementOption, int virtualTestSubTypeId)
        {
            var testData = actReportService.GetSATSummarySchoolLevelImprovementData(schoolId, districtTermId, virtualTestIds, improvementOption, virtualTestSubTypeId);

            var allSectionNames = GetAllSectionNames(testData);

            var singleTestModel = BuildSchoolOrTeacherSingleTestModel(testData, SATConstants.ImprovementTestName, allSectionNames, SATSummaryReportType.SchoolLevel);
            singleTestModel.AverageScores = BuildSchoolOrTeacherImprovementOrBaselineData(testData, allSectionNames,
                                                                                SATSummaryReportType.SchoolLevel);
            return singleTestModel;
        }

        private SATSingleTestReportViewModel BuildDistrictLevelImprovementReport(int districtId, int districtTermId, List<int> virtualTestIds, string improvementOption, int virtualTestSubTypeId)
        {
            var testData = actReportService.GetSATSummaryDistrictLevelImprovementData(districtId, districtTermId, virtualTestIds, improvementOption, virtualTestSubTypeId);

            var allSectionNames = GetAllSectionNames(testData);

            var singleTestModel = BuildSchoolOrTeacherSingleTestModel(testData, SATConstants.ImprovementTestName, allSectionNames, SATSummaryReportType.DistrictLevel);
            singleTestModel.AverageScores = BuildSchoolOrTeacherImprovementOrBaselineData(testData, allSectionNames,
                                                                                SATSummaryReportType.DistrictLevel);
            return singleTestModel;
        }

        private List<SATSingleTestReportViewModel> BuildSchoolLevelReport(int schoolId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            var data = actReportService.GetSATSummarySchoolLevelData(schoolId, districtTermId, virtualTestIds, virtualTestSubTypeId);
            var averageData = actReportService.GetSATSummarySchoolLevelAverageData(schoolId, districtTermId, virtualTestIds, virtualTestSubTypeId);

            var testIds = data.Select(en => en.VirtualTestId).Distinct();
            var testModels = new List<SATSingleTestReportViewModel>();

            var allSectionNames = GetAllSectionNames(data);

            foreach (var testId in testIds)
            {
                var testData = data.Where(en => en.VirtualTestId == testId).ToList();
                var testName = testData.FirstOrDefault().TestName;

                var singleTestModel = BuildSchoolOrTeacherSingleTestModel(testData, testName, allSectionNames, SATSummaryReportType.SchoolLevel);
                singleTestModel.AverageScores = BuildSchoolOrTeacherAverageData(averageData, allSectionNames, testId, SATSummaryReportType.SchoolLevel);
                testModels.Add(singleTestModel);
            }

            return testModels;
        }

        private SATSingleTestReportViewModel BuildSchoolLevelBaselineReport(int schoolId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            var testData = actReportService.GetSATSummarySchoolLevelBaselineData(schoolId, districtTermId, virtualTestIds, virtualTestSubTypeId);

            var allSectionNames = GetAllSectionNames(testData);

            var singleTestModel = BuildSchoolOrTeacherSingleTestModel(testData, SATConstants.BaselineTestName, allSectionNames, SATSummaryReportType.SchoolLevel);
            singleTestModel.AverageScores = BuildSchoolOrTeacherImprovementOrBaselineData(testData, allSectionNames,
                                                                                SATSummaryReportType.SchoolLevel);
            return singleTestModel;
        }

        private List<SATSingleTestReportViewModel> BuildDistrictLevelReport(int districtId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            var data = actReportService.GetSATSummaryDistrictLevelData(districtId, districtTermId, virtualTestIds, virtualTestSubTypeId);
            var averageData = actReportService.GetSATSummaryDistrictLevelAverageData(districtId, districtTermId, virtualTestIds, virtualTestSubTypeId);

            var testIds = data.Select(en => en.VirtualTestId).Distinct();
            var testModels = new List<SATSingleTestReportViewModel>();

            var allSectionNames = GetAllSectionNames(data);

            foreach (var testId in testIds)
            {
                var testData = data.Where(en => en.VirtualTestId == testId).ToList();
                var testName = testData.FirstOrDefault().TestName;

                var singleTestModel = BuildSchoolOrTeacherSingleTestModel(testData, testName, allSectionNames, SATSummaryReportType.DistrictLevel);
                singleTestModel.AverageScores = BuildSchoolOrTeacherAverageData(averageData, allSectionNames, testId, SATSummaryReportType.DistrictLevel);
                testModels.Add(singleTestModel);
            }

            return testModels;
        }

        private SATSingleTestReportViewModel BuildDistrictLevelBaselineReport(int districtId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            var testData = actReportService.GetSATSummaryDistrictLevelBaselineData(districtId, districtTermId, virtualTestIds, virtualTestSubTypeId);

            var allSectionNames = GetAllSectionNames(testData);

            var singleTestModel = BuildSchoolOrTeacherSingleTestModel(testData, SATConstants.BaselineTestName, allSectionNames, SATSummaryReportType.DistrictLevel);
            singleTestModel.AverageScores = BuildSchoolOrTeacherImprovementOrBaselineData(testData, allSectionNames,
                                                                                SATSummaryReportType.DistrictLevel);
            return singleTestModel;
        }

        private SATSingleTestReportViewModel BuildClassLevelImprovementReport(int classId, List<int> virtualTestIds, List<int> studentIds, string improvementOption, int virtualTestSubTypeId)
        {
            var testData = actReportService.GetSATSummaryClassLevelImprovementData(classId, virtualTestIds, improvementOption, virtualTestSubTypeId);
            testData = testData.Where(x => studentIds.Contains(x.StudentID)).ToList();

            var allSectionNames = GetAllSectionNames(testData);

            var singleTestModel = BuildClassSingleTestModel(testData, SATConstants.ImprovementTestName, allSectionNames);
            singleTestModel.CalculateAverageScores();
            return singleTestModel;
        }

        private SATSingleTestReportViewModel BuildClassLevelBaselineReport(int classId, List<int> virtualTestIds, List<int> studentIds, int virtualTestSubTypeId)
        {
            var testData = actReportService.GetSATSummaryClassLevelBaselineData(classId, virtualTestIds, virtualTestSubTypeId);
            testData = testData.Where(x => studentIds.Contains(x.StudentID)).ToList();

            var allSectionNames = GetAllSectionNames(testData);

            var singleTestModel = BuildClassSingleTestModel(testData, SATConstants.BaselineTestName, allSectionNames);
            singleTestModel.CalculateAverageScores();
            return singleTestModel;
        }

        private List<SATDataInTestReportViewModel> BuildBaselineClassLevelReport(List<ACTSummaryClassLevelData> data)
        {
            var baselineData = new List<SATDataInTestReportViewModel>();
            var studentList = data.Select(x => new
            {
                x.StudentDisplayName,
                x.StudentID
            }).Distinct().OrderBy(x => x.StudentDisplayName).ToList();

            var allSectionNames = GetAllSectionNames(data);

            foreach (var student in studentList)
            {                
                var firstResultDates =
                    allSectionNames.Select(
                        sectionName => new
                                           {
                                               SectionName = sectionName,
                                               FirstSectionResultDate = data.Where(x =>!string.IsNullOrEmpty(x.SectionName) &&x.SectionName.Trim().Equals(sectionName,StringComparison.InvariantCultureIgnoreCase) &&x.StudentID == student.StudentID && x.ScoreScaled > 0).Min(x => x.ResultDate)
                                           }
                        ).ToList();
                                
                var firstCompositeResultDate = data.Where(x => !string.IsNullOrEmpty(x.SectionName) && x.SectionName.Trim()
                    .Equals("composite", StringComparison.InvariantCultureIgnoreCase) && x.StudentID == student.StudentID && x.ScoreScaled > 0).Min(x => x.ResultDate);

                int firstEnglishId = 0;
                int firstMathId = 0;
                int firstReadingId = 0;
                int firstScienceId = 0;
                int firstWritingId = 0;
                int firstEWId = 0;
                int firstCompositeId = 0;

                var firstResultIds = firstResultDates
                    .Select(en => new
                                      {
                                          SectionName = en.SectionName,
                                          FirstTestResultId =
                                      data.Any(x => !string.IsNullOrEmpty(x.SectionName) && x.SectionName.Trim()
                                                                                                .Equals(en.SectionName,
                                                                                                        StringComparison
                                                                                                            .
                                                                                                            InvariantCultureIgnoreCase) &&
                                                    x.StudentID == student.StudentID && x.ScoreScaled > 0 &&
                                                    x.ResultDate == en.FirstSectionResultDate)
                                          ? data.Where(
                                              x => !string.IsNullOrEmpty(x.SectionName) && x.SectionName.Trim()
                                                                                               .Equals(
                                                                                                   en.SectionName,
                                                                                                   StringComparison.
                                                                                                       InvariantCultureIgnoreCase) &&
                                                   x.StudentID == student.StudentID && x.ScoreScaled > 0 &&
                                                   x.ResultDate == en.FirstSectionResultDate).Min(
                                                       x => x.TestResultID)
                                          : 0
                                      });
                
                if (data.Any(x => !string.IsNullOrEmpty(x.SectionName) && x.SectionName.Trim()
                    .Equals("composite", StringComparison.InvariantCultureIgnoreCase) && x.StudentID == student.StudentID && x.ScoreScaled > 0 && x.ResultDate == firstCompositeResultDate))
                firstCompositeId = data.Where(x => !string.IsNullOrEmpty(x.SectionName) && x.SectionName.Trim()
                    .Equals("composite", StringComparison.InvariantCultureIgnoreCase) && x.StudentID == student.StudentID && x.ScoreScaled > 0 && x.ResultDate == firstCompositeResultDate).Min(x => x.TestResultID);

                var dataInTest = new SATDataInTestReportViewModel();
                dataInTest.DisplayName = student.StudentDisplayName;
                dataInTest.ID = student.StudentID;

                dataInTest.SubScores = new List<SATSummarySubScore>();
                foreach (var item in firstResultIds)
                {
                    var subScore = new SATSummarySubScore
                                       {
                                           SectionName = item.SectionName,
                                           Score =
                                               GetSectionScoreBySectionNameAndStudentID(item.SectionName,
                                                                                        student.StudentID,
                                                                                        data.Where(
                                                                                            x =>
                                                                                            x.TestResultID ==
                                                                                            item.FirstTestResultId).
                                                                                            ToList())
                                       };
                    dataInTest.SubScores.Add(subScore);
                }

                dataInTest.CompositeScore = GetSectionScoreBySectionNameAndStudentID("composite", student.StudentID, data.Where(x => x.TestResultID == firstCompositeId).ToList());
                dataInTest.ReportType = SATSummaryReportType.ClassLevel;

                baselineData.Add(dataInTest);
            }

            return baselineData;
        }

        private SATSingleTestReportViewModel BuildSchoolOrTeacherSingleTestModel(List<ACTSummarySchoolOrTeacherLevelData> testData, string testName, List<string> allSectionNames, SATSummaryReportType reportType)
        {
            var singleTestModel = new SATSingleTestReportViewModel
            {
                TestName = testName,
                ReportType = SATSummaryReportType.TeacherLevel,
                ListDataInTest = new List<SATDataInTestReportViewModel>()
            };

            var classIds = testData.Where(en => en.ClassId > 0).Select(en => en.ClassId).Distinct();            

            foreach (var classId in classIds)
            {
                var dataInTest = new SATDataInTestReportViewModel();

                if (reportType == SATSummaryReportType.DistrictLevel)
                {
                    dataInTest.DisplayName = GetDisplayNameByID(classId, testData);
                    dataInTest.SortName = GetDisplayNameByID(classId, testData);
                }
                else
                {
                    dataInTest.DisplayName = GetClassNameByClassID(classId, testData);
                    dataInTest.SortName = GetSortNameByClassID(classId, testData);
                }

                dataInTest.ID = classId;

                dataInTest.SubScores = new List<SATSummarySubScore>();
                foreach (var sectionName in allSectionNames)
                {
                    var subScore = new SATSummarySubScore
                                       {
                                           SectionName = sectionName,
                                           Score = GetSectionScoreBySectionNameAndClassID(sectionName, classId, testData)
                                       };
                    dataInTest.SubScores.Add(subScore);
                }
                
                dataInTest.CompositeScore = GetSectionScoreBySectionNameAndClassID("composite", classId, testData);

                dataInTest.TotalStudents = GetTotalStudentByClassID(classId, testData);
                dataInTest.ReportType = reportType;

                singleTestModel.ListDataInTest.Add(dataInTest);
            }

            // Sort result by sort name
            singleTestModel.ListDataInTest = singleTestModel.ListDataInTest.OrderBy(en => en.SortName).ToList();
            return singleTestModel;
        }

        private SATSingleTestReportViewModel BuildClassSingleTestModel(List<ACTSummaryClassLevelData> testData, string testName, List<string> allSectionNames)
        {
            var singleTestModel = new SATSingleTestReportViewModel
            {
                TestName = testName,
                ReportType = SATSummaryReportType.ClassLevel,
                ListDataInTest = new List<SATDataInTestReportViewModel>()
            };

            var studentIds = testData.Select(en => en.StudentID).Distinct();
            
            foreach (var studentId in studentIds)
            {
                var dataInTest = new SATDataInTestReportViewModel();

                dataInTest.DisplayName = GetStudentName(studentId, testData);
                dataInTest.SortName = GetStudentName(studentId, testData);

                dataInTest.ID = studentId;
                dataInTest.SubScores = new List<SATSummarySubScore>();
                foreach (var sectionName in allSectionNames)
                {
                    var subScore = new SATSummarySubScore
                    {
                        SectionName = sectionName,
                        Score = GetSectionScoreBySectionNameAndStudentID(sectionName, studentId, testData)
                    };
                    dataInTest.SubScores.Add(subScore);
                }
                
                dataInTest.CompositeScore = GetSectionScoreBySectionNameAndStudentID("composite", studentId, testData);

                dataInTest.ReportType = SATSummaryReportType.ClassLevel;

                singleTestModel.ListDataInTest.Add(dataInTest);
            }

            // Sort result by sort name
            singleTestModel.ListDataInTest = singleTestModel.ListDataInTest.OrderBy(en => en.SortName).ToList();
            return singleTestModel;
        }

        private SATDataInTestReportViewModel BuildSchoolOrTeacherAverageData(List<ACTSummarySchoolOrTeacherLevelData> testData, List<string> allSectionNames, int testId, SATSummaryReportType reportType)
        {
            var dataInTest = new SATDataInTestReportViewModel();
            dataInTest.DisplayName = "Average";
            
            dataInTest.SubScores = new List<SATSummarySubScore>();
            foreach (var sectionName in allSectionNames)
            {
                var subScore = new SATSummarySubScore
                {
                    SectionName = sectionName,
                    Score = GetSectionAverageScoreBySectionNameAndTestID(sectionName, testId, testData)
                };
                dataInTest.SubScores.Add(subScore);
            }

            dataInTest.CompositeScore = GetSectionAverageScoreBySectionNameAndTestID("composite", testId, testData);
            dataInTest.TotalStudents = GetTotalStudentByTestID(testId, testData);
            dataInTest.ReportType = reportType;

            return dataInTest;
        }

        private SATDataInTestReportViewModel BuildSchoolOrTeacherImprovementOrBaselineData(List<ACTSummarySchoolOrTeacherLevelData> testData, List<string> allSectionNames, SATSummaryReportType reportType)
        {
            var dataInTest = new SATDataInTestReportViewModel();
            const int classId = 0;

            if (reportType == SATSummaryReportType.DistrictLevel)
            {
                dataInTest.DisplayName = GetDisplayNameByID(classId, testData);
                dataInTest.SortName = GetDisplayNameByID(classId, testData);
            }
            else
            {
                dataInTest.DisplayName = GetClassNameByClassID(classId, testData);
                dataInTest.SortName = GetSortNameByClassID(classId, testData);
            }

            dataInTest.ID = classId;

            dataInTest.SubScores = new List<SATSummarySubScore>();
            foreach (var sectionName in allSectionNames)
            {
                var subScore = new SATSummarySubScore
                {
                    SectionName = sectionName,
                    Score = GetSectionScoreBySectionNameAndClassID(sectionName, classId, testData)
                };
                dataInTest.SubScores.Add(subScore);
            }            
            dataInTest.CompositeScore = GetSectionScoreBySectionNameAndClassID("composite", classId, testData);

            dataInTest.TotalStudents = GetTotalStudentByClassID(classId, testData);
            dataInTest.ReportType = reportType;

            return dataInTest;
        }

        private decimal GetSectionScoreBySectionNameAndStudentID(string sectionName, int studentID,
            List<ACTSummaryClassLevelData> data)
        {
            var scoreData = data.FirstOrDefault(x => !string.IsNullOrEmpty(x.SectionName) &&
                                                     x.SectionName.Trim()
                                                         .Equals(sectionName,
                                                             StringComparison.InvariantCultureIgnoreCase)
                                                     && x.StudentID == studentID);
            return scoreData == null
                ? -1
                : scoreData.ScoreScaled;
        }

        private decimal GetSectionScoreBySectionNameAndSchoolID(string sectionName, int schoolID,
            List<ACTSummaryDistrictLevelData> data)
        {
            var query = data.Where(x =>
                !string.IsNullOrEmpty(x.SectionName)
                && x.SectionName.Trim().Equals(sectionName, StringComparison.InvariantCultureIgnoreCase)
                && x.SchoolID == schoolID);
            return query.Sum(x => x.ScoreScaled);
        }

        private decimal GetSectionScoreBySectionNameAndClassID(string sectionName, int classID,
            List<ACTSummarySchoolOrTeacherLevelData> data)
        {
            var scoreData = data.FirstOrDefault(x => !string.IsNullOrEmpty(x.SectionName) &&
                                                     x.SectionName.Trim()
                                                         .Equals(sectionName,
                                                             StringComparison.InvariantCultureIgnoreCase)
                                                     && x.ClassId == classID);
            return scoreData == null ? -1 : scoreData.ScoreScaled;
        }

        private decimal GetSectionAverageScoreBySectionNameAndTestID(string sectionName, int testID,
            List<ACTSummarySchoolOrTeacherLevelData> data)
        {
            var scoreData = data.FirstOrDefault(x => !string.IsNullOrEmpty(x.SectionName) &&
                                                     x.SectionName.Trim()
                                                         .Equals(sectionName,
                                                             StringComparison.InvariantCultureIgnoreCase)
                                                     && x.VirtualTestId == testID);
            return scoreData == null ? -1 : scoreData.ScoreScaled;
        }

        private string GetClassNameByClassID(int classID,
            List<ACTSummarySchoolOrTeacherLevelData> data)
        {
            var item = data.FirstOrDefault(x => x.ClassId == classID);
            return item.ClassName + " (" + item.TeacherName + ", " + item.DistrictTermName + ")";
        }

        private string GetSortNameByClassID(int classID,
            List<ACTSummarySchoolOrTeacherLevelData> data)
        {
            var item = data.FirstOrDefault(x => x.ClassId == classID);
            return item.DistrictTermName + "_" + item.TeacherName + "_" + item.ClassName + "_" + item.ClassId.ToString();
        }

        private string GetDisplayNameByID(int Id,
            List<ACTSummarySchoolOrTeacherLevelData> data)
        {
            var item = data.FirstOrDefault(x => x.ClassId == Id);
            return item.ClassName;
        }

        private string GetStudentName(int Id,
            List<ACTSummaryClassLevelData> data)
        {
            var item = data.FirstOrDefault(x => x.StudentID == Id);
            return item.StudentDisplayName;
        }

        private int GetTotalStudentByClassID(int classID,
            List<ACTSummarySchoolOrTeacherLevelData> data)
        {
            return data.Any(x => x.ClassId == classID) ? data.FirstOrDefault(x => x.ClassId == classID).StudentNo : 0;
        }

        private int GetTotalStudentByTestID(int testID,
            List<ACTSummarySchoolOrTeacherLevelData> data)
        {
            return data.Any(x => x.VirtualTestId == testID)
                       ? data.Where(x => x.VirtualTestId == testID).Max(en => en.StudentNo)
                       : 0;
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult Generate(ActSummaryReportData model)
        {
            if (model.DistrictId.HasValue == false)
            {
                model.DistrictId = CurrentUser.DistrictId.GetValueOrDefault();
            }

            string url = string.Empty;
            model.CurrentUserDistrict = CurrentUser.DistrictId.GetValueOrDefault();
            var pdf = Print(model, out url);

            var folder = LinkitConfigurationManager.GetS3Settings().ACTReportFolder;
            var bucketName = LinkitConfigurationManager.GetS3Settings().ACTReportBucket;
            var result = _s3Service.UploadRubricFile(bucketName, folder + "/" + model.ReportFileName, new MemoryStream(pdf));

            return Json(new { IsSuccess = true, Url = url });
        }

        private byte[] Print(ActSummaryReportData model, out string url)
        {
            if (model.StudentIdList.Any())
            {
                model.StrStudentIdList = string.Join(",", model.StudentIdList);
            }

            url = Url.Action("ReportPrinting", "SATSummaryReport", model, HelperExtensions.GetHTTPProtocal(Request));
            string html = "";

            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = client.GetAsync(url).Result)
                {
                    using (HttpContent content = response.Content)
                    {
                        html = content.ReadAsStringAsync().Result;
                    }
                }
            }
            var pdf = ExportToPDFByPrinceXML(html);
            return pdf;
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
        [AjaxOnly]
        public ActionResult CheckSATS3FileExisted(string fileName)
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

        private List<string> GetAllSectionNames(List<ACTSummaryClassLevelData> data)
        {
            return data.Where(x => !string.IsNullOrEmpty(x.SectionName) && !x.SectionName.Trim()
                                                           .Equals("Composite",
                                                                   StringComparison.InvariantCultureIgnoreCase)).Select(
                                                                       x => x.SectionName).Distinct().ToList();
        }

        private List<string> GetAllSectionNames(List<ACTSummarySchoolOrTeacherLevelData> data)
        {
            return data.Where(x => !string.IsNullOrEmpty(x.SectionName) && !x.SectionName.Trim()
                                                           .Equals("Composite",
                                                                   StringComparison.InvariantCultureIgnoreCase)).Select(
                                                                       x => x.SectionName).Distinct().ToList();
        }

        private List<string> GetAllSectionNames(List<ACTSummaryDistrictLevelData> data)
        {
            return data.Where(x => !string.IsNullOrEmpty(x.SectionName) && !x.SectionName.Trim()
                                                           .Equals("Composite",
                                                                   StringComparison.InvariantCultureIgnoreCase)).Select(
                                                                       x => x.SectionName).Distinct().ToList();
        }

        private void ReorderSubjectInViewModel(SATSummaryReportViewModel viewModel, int districtId)
        {
            var subScoreList =
                viewModel.ListTestReport.Select(
                    x => x.ListDataInTest.Select(y => y.SubScores.Select(z => z.SectionName).ToList())
                        .SelectMany(y => y))
                    .SelectMany(x => x)
                    .Distinct()
                    .ToList();
            var orderedSubScoreList = OrderSubjectByDistrictDecodeSetting(districtId, subScoreList, viewModel.VirtualTestSubTypeId);
            AddCompositeScoreToDataInTestSubScore(viewModel);
            ReorderSubjectNameForReport(viewModel, orderedSubScoreList);
            ReorderListDataInTest(orderedSubScoreList, viewModel.BaselineReport);
            ReorderListDataInTest(orderedSubScoreList, viewModel.ImprovementReport);
        }

        private void AddCompositeScoreToDataInTestSubScore(SATSummaryReportViewModel viewModel)
        {
            if (viewModel.ListTestReport == null) return;
            foreach (var singleTestReport in viewModel.ListTestReport)
            {
                if (singleTestReport.ListDataInTest != null)
                {
                    foreach (var dataInTest in singleTestReport.ListDataInTest)
                    {
                        if (dataInTest.SubScores == null) continue;
                        dataInTest.SubScores.Add(new SATSummarySubScore
                                                 {
                                                     Score = dataInTest.CompositeScore,
                                                     SectionName = "Composite"
                                                 });
                    }
                }

                if (singleTestReport.AverageScores != null && singleTestReport.AverageScores.SubScores != null)
                {
                    singleTestReport.AverageScores.SubScores.Add(new SATSummarySubScore
                                                                 {
                                                                     Score =
                                                                         singleTestReport.AverageScores.CompositeScore,
                                                                     SectionName = "Composite"
                                                                 });
                }
            }
        }

        private void ReorderSubjectNameForReport(SATSummaryReportViewModel viewModel, List<string> orderedSubScoreList)
        {
            foreach (var satSingleTestReportViewModel in viewModel.ListTestReport)
            {
                ReorderListDataInTest(orderedSubScoreList, satSingleTestReportViewModel);
                satSingleTestReportViewModel.AverageScores.SubScores = 
                    ReorderDataInTestSubScores(orderedSubScoreList, satSingleTestReportViewModel.AverageScores);
            }
        }

        private void ReorderListDataInTest(List<string> orderedSubScoreList, SATSingleTestReportViewModel satSingleTestReportViewModel)
        {
            if (satSingleTestReportViewModel == null) return;
            foreach (var satDataInTestReportViewModel in satSingleTestReportViewModel.ListDataInTest)
            {
                var orderedSubScores = ReorderDataInTestSubScores(orderedSubScoreList, satDataInTestReportViewModel);
                satDataInTestReportViewModel.SubScores = orderedSubScores;
            }
        }

        private static List<SATSummarySubScore> ReorderDataInTestSubScores(List<string> orderedSubScoreList,
            SATDataInTestReportViewModel satDataInTestReportViewModel)
        {
            var tempSubScore = satDataInTestReportViewModel.SubScores;
            var orderedSubScores = new List<SATSummarySubScore>();
            foreach (var sectionName in orderedSubScoreList)
            {
                var subscore =
                    tempSubScore.FirstOrDefault(
                        x => !string.IsNullOrEmpty(x.SectionName) && x.SectionName == sectionName);
                if (subscore == null) continue;
                orderedSubScores.Add(subscore);
            }
            return orderedSubScores;
        }

        private List<string> OrderSubjectByDistrictDecodeSetting(int districtID, List<string> subScoreList, int virtualTestSubTypeID)
        {
            string districtDecodeLabel = virtualTestSubTypeID == (int)VirtualTestSubType.SAT
                ? SATConstants.SATSummaryReportSubjectOrderLabel
                : SATConstants.NewSATSummaryReportSubjectOrderLabel;
            var orderedSubScoreList = new List<string>();
            var subjectOrderSetting =
                districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(
                    districtID, districtDecodeLabel).FirstOrDefault();
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
                    else if (subject == SATConstants.CompositeSubjectName
                        || subject == SATConstants.EssaySubScoreName 
                        || subject == SATConstants.MultipleChoiceWritingSubjectName)
                    {
                        orderedSubScoreList.Add(subject);
                    }
                }
                var listSubjectNotAdded = subScoreList.Where(x => orderedSubScoreList.Contains(x) == false)
                    .OrderBy(x => x).ToList();
                if (listSubjectNotAdded.Any())
                {
                    orderedSubScoreList.AddRange(listSubjectNotAdded);
                }
            }
            return orderedSubScoreList;
        }
    }
}
