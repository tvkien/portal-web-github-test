using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Models.ACTSummaryReport;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.ViewModels.ACTReport;
using LinkIt.BubbleSheetPortal.Web.ViewModels.ACTSummaryReport;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using S3Library;
using LinkIt.BubbleSheetPortal.Web.ServiceConsumer;
using System.Net.Http;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class ACTSummaryReportController : BaseController
    {
        private readonly DistrictService districtService;
        private readonly SchoolService schoolService;
        private readonly ClassService classService;
        private readonly UserService userService;
        private readonly ACTReportService actReportService;
        private readonly VirtualTestService virtualTestService;
        private readonly DistrictTermService districtTermService;

        private readonly IS3Service s3Service;

        public ACTSummaryReportController(DistrictService districtService, ACTReportService actReportService, VirtualTestService virtualTestService
            , SchoolService schoolService, ClassService classService, UserService userService
            , DistrictTermService districtTermService, IS3Service s3Service)
        {
            this.districtService = districtService;
            this.actReportService = actReportService;
            this.virtualTestService = virtualTestService;
            this.schoolService = schoolService;
            this.classService = classService;
            this.userService = userService;
            this.districtTermService = districtTermService;
            this.s3Service = s3Service;
        }

        public ActionResult ReportPrinting(ActSummaryReportData model)
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

            var vm = BuildActSummaryReportViewModel(model.DistrictId.Value, parseTestIDList, model.SchoolId, model.ClassId, model.TeacherId, model.DistrictTermId);
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
                    vm.ListTestReport.Add(BuildClassLevelReport(model.ClassId.Value, model.TestId, listStudentID, model.VirtualTestSubTypeId, out data));
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
                            testID, listStudentID, model.VirtualTestSubTypeId, out subData));
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

            // Add virtual test sub type to custom summary report layout            
            foreach (var testReport in vm.ListTestReport)
            {
                testReport.VirtualTestSubTypeId = model.VirtualTestSubTypeId;
            }
            IncludeJSCss(vm);

            return View("ReportPrinting", vm);
        }

        private void IncludeJSCss(ActSummaryReportViewModel model)
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


        private void ReOrderVirtualTest(ActSummaryReportViewModel vm)
        {
            var testNames = vm.TestNames.Split(',').Select(x => x.Trim()).ToList();
            vm.ListTestReport = vm.ListTestReport.OrderBy(x => testNames.IndexOf(x.TestName.Trim())).ToList();
        }

        private void AddMissingDataInTestItem(List<ActSingleTestReportViewModel> listTestReports, ActSingleTestReportViewModel baselineTestReport)
        {
            foreach (var testReport in listTestReports)
            {
                foreach (var dataInTest in baselineTestReport.ListDataInTest)
                {
                    if (testReport.ListDataInTest.All(x => x.ID != dataInTest.ID))
                    {
                        testReport.ListDataInTest.Add(new ActDataInTestReportViewModel
                        {
                            ReportType = dataInTest.ReportType,
                            CompositeScore = -1,
                            DisplayName = dataInTest.DisplayName,
                            EWScore = -1,
                            EnglishScore = -1,
                            ID = dataInTest.ID,
                            MathScore = -1,
                            ReadingScore = -1,
                            ScienceScore = -1,
                            SortName = dataInTest.SortName,
                            TotalStudents = 0,
                            WritingScore = -1
                        });
                    }
                }
                testReport.ListDataInTest = testReport.ListDataInTest.OrderBy(en => en.SortName).ThenBy(en => en.ID).ToList();
            }
        }


        private ActSummaryReportViewModel BuildActSummaryReportViewModel(int districtId, List<int> testIds, int? schoolID, int? classId, int? teacherId, int? districtTermId)
        {
            var viewModel = new ActSummaryReportViewModel
            {
                SchoolName = "ALL",
                ClassName = "ALL",
                TeacherName = "ALL",
                DistrictTermName = "ALL"
            };
            var listTestReport = new List<ActSingleTestReportViewModel>();

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

        private ActSingleTestReportViewModel BuildClassLevelReport(int classID, int virtualTestID, List<int> listStudentIDs, int virtualTestSubTypeId, out List<ACTSummaryClassLevelData> data)
        {
            data = actReportService.GetACTSummaryClassLevelData(classID, virtualTestID, virtualTestSubTypeId);
            data = data.Where(x => listStudentIDs.Contains(x.StudentID)).ToList();
            var studentList = data.Select(x => new
            {
                x.StudentDisplayName,
                x.StudentID
            }).Distinct().OrderBy(x => x.StudentDisplayName).ToList();
            var test = virtualTestService.Select().First(x => x.VirtualTestID == virtualTestID);
            var viewModel = new ActSingleTestReportViewModel();
            viewModel.ListDataInTest = new List<ActDataInTestReportViewModel>();
            viewModel.ReportType = ActSummaryReportType.ClassLevel;
            viewModel.TestName = test.Name;

            foreach (var student in studentList)
            {
                var dataInTest = new ActDataInTestReportViewModel();
                dataInTest.DisplayName = student.StudentDisplayName;
                dataInTest.SortName = student.StudentDisplayName;
                dataInTest.ID = student.StudentID;
                dataInTest.EnglishScore = GetSectionScoreBySectionNameAndStudentID("english", student.StudentID, data, virtualTestSubTypeId);
                dataInTest.MathScore = GetSectionScoreBySectionNameAndStudentID("math", student.StudentID, data, virtualTestSubTypeId);
                dataInTest.ReadingScore = GetSectionScoreBySectionNameAndStudentID("reading", student.StudentID, data, virtualTestSubTypeId);
                dataInTest.ScienceScore = GetSectionScoreBySectionNameAndStudentID("science", student.StudentID, data, virtualTestSubTypeId);
                dataInTest.WritingScore = GetSectionScoreBySectionNameAndStudentID("writing", student.StudentID, data, virtualTestSubTypeId);
                dataInTest.EWScore = GetSectionScoreBySectionNameAndStudentID("e/w", student.StudentID, data, virtualTestSubTypeId);
                dataInTest.CompositeScore = GetSectionScoreBySectionNameAndStudentID("composite", student.StudentID, data, virtualTestSubTypeId);
                dataInTest.ReportType = ActSummaryReportType.ClassLevel;

                viewModel.ListDataInTest.Add(dataInTest);
            }

            //viewModel.ListDataInTest = viewModel.ListDataInTest.OrderBy(en => en.DisplayName).ToList();
            viewModel.CalculateAverageScores();
            return viewModel;
        }

        private List<ActSingleTestReportViewModel> BuildTeacherLevelReport(int userId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            var data = actReportService.GetACTSummaryTeacherLevelData(userId, districtTermId, virtualTestIds, virtualTestSubTypeId);
            var averageData = actReportService.GetACTSummaryTeacherLevelAverageData(userId, districtTermId, virtualTestIds, virtualTestSubTypeId);

            var testIds = data.Select(en => en.VirtualTestId).Distinct();
            var testModels = new List<ActSingleTestReportViewModel>();

            foreach (var testId in testIds)
            {
                var testData = data.Where(en => en.VirtualTestId == testId).ToList();
                var testName = testData.FirstOrDefault().TestName;

                var singleTestModel = BuildSchoolOrTeacherSingleTestModel(testData, testName, ActSummaryReportType.TeacherLevel, virtualTestSubTypeId);
                singleTestModel.AverageScores = BuildSchoolOrTeacherAverageData(averageData, testId, ActSummaryReportType.TeacherLevel, virtualTestSubTypeId);
                testModels.Add(singleTestModel);
            }

            return testModels;
        }

        private ActSingleTestReportViewModel BuildTeacherLevelBaselineReport(int userId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            var testData = actReportService.GetACTSummaryTeacherLevelBaselineData(userId, districtTermId, virtualTestIds, virtualTestSubTypeId);

            var singleTestModel = BuildSchoolOrTeacherSingleTestModel(testData, "Baseline", ActSummaryReportType.TeacherLevel, virtualTestSubTypeId);
            singleTestModel.AverageScores = BuildSchoolOrTeacherImprovementOrBaselineData(testData, ActSummaryReportType.TeacherLevel, virtualTestSubTypeId);
            return singleTestModel;
        }

        private ActSingleTestReportViewModel BuildTeacherLevelImprovementReport(int userId, int districtTermId, List<int> virtualTestIds, string improvementOption, int virtualTestSubTypeId)
        {
            var testData = actReportService.GetACTSummaryTeacherLevelImprovementData(userId, districtTermId, virtualTestIds, improvementOption, virtualTestSubTypeId);

            var singleTestModel = BuildSchoolOrTeacherSingleTestModel(testData, "Improvement", ActSummaryReportType.TeacherLevel, virtualTestSubTypeId);
            singleTestModel.AverageScores = BuildSchoolOrTeacherImprovementOrBaselineData(testData, ActSummaryReportType.TeacherLevel, virtualTestSubTypeId);
            return singleTestModel;
        }

        private ActSingleTestReportViewModel BuildSchoolLevelImprovementReport(int schoolId, int districtTermId, List<int> virtualTestIds, string improvementOption, int virtualTestSubTypeId)
        {
            var testData = actReportService.GetACTSummarySchoolLevelImprovementData(schoolId, districtTermId, virtualTestIds, improvementOption, virtualTestSubTypeId);

            var singleTestModel = BuildSchoolOrTeacherSingleTestModel(testData, "Improvement", ActSummaryReportType.SchoolLevel, virtualTestSubTypeId);
            singleTestModel.AverageScores = BuildSchoolOrTeacherImprovementOrBaselineData(testData, ActSummaryReportType.SchoolLevel, virtualTestSubTypeId);
            return singleTestModel;
        }

        private ActSingleTestReportViewModel BuildDistrictLevelImprovementReport(int districtId, int districtTermId, List<int> virtualTestIds, string improvementOption, int virtualTestSubTypeId)
        {
            var testData = actReportService.GetACTSummaryDistrictLevelImprovementData(districtId, districtTermId, virtualTestIds, improvementOption, virtualTestSubTypeId);

            var singleTestModel = BuildSchoolOrTeacherSingleTestModel(testData, "Improvement", ActSummaryReportType.DistrictLevel, virtualTestSubTypeId);
            singleTestModel.AverageScores = BuildSchoolOrTeacherImprovementOrBaselineData(testData, ActSummaryReportType.DistrictLevel, virtualTestSubTypeId);
            return singleTestModel;
        }

        private List<ActSingleTestReportViewModel> BuildSchoolLevelReport(int schoolId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            var data = actReportService.GetACTSummarySchoolLevelData(schoolId, districtTermId, virtualTestIds, virtualTestSubTypeId);
            var averageData = actReportService.GetACTSummarySchoolLevelAverageData(schoolId, districtTermId, virtualTestIds, virtualTestSubTypeId);

            var testIds = data.Select(en => en.VirtualTestId).Distinct();
            var testModels = new List<ActSingleTestReportViewModel>();

            foreach (var testId in testIds)
            {
                var testData = data.Where(en => en.VirtualTestId == testId).ToList();
                var testName = testData.FirstOrDefault().TestName;

                var singleTestModel = BuildSchoolOrTeacherSingleTestModel(testData, testName, ActSummaryReportType.SchoolLevel, virtualTestSubTypeId);
                singleTestModel.AverageScores = BuildSchoolOrTeacherAverageData(averageData, testId, ActSummaryReportType.SchoolLevel, virtualTestSubTypeId);
                testModels.Add(singleTestModel);
            }

            return testModels;
        }

        private ActSingleTestReportViewModel BuildSchoolLevelBaselineReport(int schoolId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            var testData = actReportService.GetACTSummarySchoolLevelBaselineData(schoolId, districtTermId, virtualTestIds, virtualTestSubTypeId);

            var singleTestModel = BuildSchoolOrTeacherSingleTestModel(testData, "Baseline", ActSummaryReportType.SchoolLevel, virtualTestSubTypeId);
            singleTestModel.AverageScores = BuildSchoolOrTeacherImprovementOrBaselineData(testData, ActSummaryReportType.SchoolLevel, virtualTestSubTypeId);
            return singleTestModel;
        }

        private List<ActSingleTestReportViewModel> BuildDistrictLevelReport(int districtId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            var data = actReportService.GetACTSummaryDistrictLevelData(districtId, districtTermId, virtualTestIds, virtualTestSubTypeId);
            var averageData = actReportService.GetACTSummaryDistrictLevelAverageData(districtId, districtTermId, virtualTestIds, virtualTestSubTypeId);

            var testIds = data.Select(en => en.VirtualTestId).Distinct();
            var testModels = new List<ActSingleTestReportViewModel>();

            foreach (var testId in testIds)
            {
                var testData = data.Where(en => en.VirtualTestId == testId).ToList();
                var testName = testData.FirstOrDefault().TestName;

                var singleTestModel = BuildSchoolOrTeacherSingleTestModel(testData, testName, ActSummaryReportType.DistrictLevel, virtualTestSubTypeId);
                singleTestModel.AverageScores = BuildSchoolOrTeacherAverageData(averageData, testId, ActSummaryReportType.DistrictLevel, virtualTestSubTypeId);
                testModels.Add(singleTestModel);
            }

            return testModels;
        }

        private ActSingleTestReportViewModel BuildDistrictLevelBaselineReport(int districtId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            var testData = actReportService.GetACTSummaryDistrictLevelBaselineData(districtId, districtTermId, virtualTestIds, virtualTestSubTypeId);

            var singleTestModel = BuildSchoolOrTeacherSingleTestModel(testData, "Baseline", ActSummaryReportType.DistrictLevel, virtualTestSubTypeId);
            singleTestModel.AverageScores = BuildSchoolOrTeacherImprovementOrBaselineData(testData, ActSummaryReportType.DistrictLevel, virtualTestSubTypeId);
            return singleTestModel;
        }


        private ActSingleTestReportViewModel BuildClassLevelImprovementReport(int classId, List<int> virtualTestIds, List<int> studentIds, string improvementOption, int virtualTestSubTypeId)
        {
            var testData = actReportService.GetACTSummaryClassLevelImprovementData(classId, virtualTestIds, improvementOption, virtualTestSubTypeId);
            testData = testData.Where(x => studentIds.Contains(x.StudentID)).ToList();

            var singleTestModel = BuildClassSingleTestModel(testData, "Improvement", virtualTestSubTypeId);
            singleTestModel.CalculateAverageScores();
            return singleTestModel;
        }

        private ActSingleTestReportViewModel BuildClassLevelBaselineReport(int classId, List<int> virtualTestIds, List<int> studentIds, int virtualTestSubTypeId)
        {
            var testData = actReportService.GetACTSummaryClassLevelBaselineData(classId, virtualTestIds, virtualTestSubTypeId);
            testData = testData.Where(x => studentIds.Contains(x.StudentID)).ToList();

            var singleTestModel = BuildClassSingleTestModel(testData, "Baseline", virtualTestSubTypeId);
            singleTestModel.CalculateAverageScores();
            return singleTestModel;
        }

        private ActSingleTestReportViewModel BuildSchoolOrTeacherSingleTestModel(List<ACTSummarySchoolOrTeacherLevelData> testData, string testName, ActSummaryReportType reportType, int virtualTestSubTypeId)
        {
            var singleTestModel = new ActSingleTestReportViewModel
            {
                TestName = testName,
                ReportType = ActSummaryReportType.TeacherLevel,
                ListDataInTest = new List<ActDataInTestReportViewModel>()
            };

            var classIds = testData.Where(en => en.ClassId > 0).Select(en => en.ClassId).Distinct();
            foreach (var classId in classIds)
            {
                var dataInTest = new ActDataInTestReportViewModel();

                if (reportType == ActSummaryReportType.DistrictLevel)
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
                dataInTest.EnglishScore = GetSectionScoreBySectionNameAndClassID("english", classId, testData, virtualTestSubTypeId);
                dataInTest.MathScore = GetSectionScoreBySectionNameAndClassID("math", classId, testData, virtualTestSubTypeId);
                dataInTest.ReadingScore = GetSectionScoreBySectionNameAndClassID("reading", classId, testData, virtualTestSubTypeId);
                dataInTest.ScienceScore = GetSectionScoreBySectionNameAndClassID("science", classId, testData, virtualTestSubTypeId);
                dataInTest.WritingScore = GetSectionScoreBySectionNameAndClassID("writing", classId, testData, virtualTestSubTypeId);
                dataInTest.EWScore = GetSectionScoreBySectionNameAndClassID("e/w", classId, testData, virtualTestSubTypeId);
                dataInTest.CompositeScore = GetSectionScoreBySectionNameAndClassID("composite", classId, testData, virtualTestSubTypeId);

                dataInTest.TotalStudents = GetTotalStudentByClassID(classId, testData);
                dataInTest.ReportType = reportType;

                singleTestModel.ListDataInTest.Add(dataInTest);
            }

            // Sort result by sort name
            singleTestModel.ListDataInTest = singleTestModel.ListDataInTest.OrderBy(en => en.SortName).ThenBy(en => en.ID).ToList();
            return singleTestModel;
        }

        private ActSingleTestReportViewModel BuildClassSingleTestModel(List<ACTSummaryClassLevelData> testData, string testName, int virtualTestSubTypeId)
        {
            var singleTestModel = new ActSingleTestReportViewModel
            {
                TestName = testName,
                ReportType = ActSummaryReportType.ClassLevel,
                ListDataInTest = new List<ActDataInTestReportViewModel>()
            };

            var studentIds = testData.Select(en => en.StudentID).Distinct();
            foreach (var studentId in studentIds)
            {
                var dataInTest = new ActDataInTestReportViewModel();

                dataInTest.DisplayName = GetStudentName(studentId, testData);
                dataInTest.SortName = GetStudentName(studentId, testData);

                dataInTest.ID = studentId;
                dataInTest.EnglishScore = GetSectionScoreBySectionNameAndStudentID("english", studentId, testData, virtualTestSubTypeId);
                dataInTest.MathScore = GetSectionScoreBySectionNameAndStudentID("math", studentId, testData, virtualTestSubTypeId);
                dataInTest.ReadingScore = GetSectionScoreBySectionNameAndStudentID("reading", studentId, testData, virtualTestSubTypeId);
                dataInTest.ScienceScore = GetSectionScoreBySectionNameAndStudentID("science", studentId, testData, virtualTestSubTypeId);
                dataInTest.WritingScore = GetSectionScoreBySectionNameAndStudentID("writing", studentId, testData, virtualTestSubTypeId);
                dataInTest.EWScore = GetSectionScoreBySectionNameAndStudentID("e/w", studentId, testData, virtualTestSubTypeId);
                dataInTest.CompositeScore = GetSectionScoreBySectionNameAndStudentID("composite", studentId, testData, virtualTestSubTypeId);

                dataInTest.ReportType = ActSummaryReportType.ClassLevel;

                singleTestModel.ListDataInTest.Add(dataInTest);
            }

            // Sort result by sort name
            singleTestModel.ListDataInTest = singleTestModel.ListDataInTest.OrderBy(en => en.SortName).ThenBy(en => en.ID).ToList();
            return singleTestModel;
        }

        private ActDataInTestReportViewModel BuildSchoolOrTeacherAverageData(List<ACTSummarySchoolOrTeacherLevelData> testData, int testId, ActSummaryReportType reportType, int virtualTestSubTypeId)
        {
            var dataInTest = new ActDataInTestReportViewModel();
            dataInTest.DisplayName = "Average";
            dataInTest.EnglishScore = GetSectionAverageScoreBySectionNameAndTestID("english", testId, testData, virtualTestSubTypeId);
            dataInTest.MathScore = GetSectionAverageScoreBySectionNameAndTestID("math", testId, testData, virtualTestSubTypeId);
            dataInTest.ReadingScore = GetSectionAverageScoreBySectionNameAndTestID("reading", testId, testData, virtualTestSubTypeId);
            dataInTest.ScienceScore = GetSectionAverageScoreBySectionNameAndTestID("science", testId, testData, virtualTestSubTypeId);
            dataInTest.WritingScore = GetSectionAverageScoreBySectionNameAndTestID("writing", testId, testData, virtualTestSubTypeId);
            dataInTest.EWScore = GetSectionAverageScoreBySectionNameAndTestID("e/w", testId, testData, virtualTestSubTypeId);
            dataInTest.CompositeScore = GetSectionAverageScoreBySectionNameAndTestID("composite", testId, testData, virtualTestSubTypeId);
            dataInTest.TotalStudents = GetTotalStudentByTestID(testId, testData);
            dataInTest.ReportType = reportType;

            return dataInTest;
        }

        private ActDataInTestReportViewModel BuildSchoolOrTeacherImprovementOrBaselineData(List<ACTSummarySchoolOrTeacherLevelData> testData, ActSummaryReportType reportType, int virtualTestSubTypeId)
        {
            var dataInTest = new ActDataInTestReportViewModel();
            const int classId = 0;

            if (reportType == ActSummaryReportType.DistrictLevel)
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
            dataInTest.EnglishScore = GetSectionScoreBySectionNameAndClassID("english", classId, testData, virtualTestSubTypeId);
            dataInTest.MathScore = GetSectionScoreBySectionNameAndClassID("math", classId, testData, virtualTestSubTypeId);
            dataInTest.ReadingScore = GetSectionScoreBySectionNameAndClassID("reading", classId, testData, virtualTestSubTypeId);
            dataInTest.ScienceScore = GetSectionScoreBySectionNameAndClassID("science", classId, testData, virtualTestSubTypeId);
            dataInTest.WritingScore = GetSectionScoreBySectionNameAndClassID("writing", classId, testData, virtualTestSubTypeId);
            dataInTest.EWScore = GetSectionScoreBySectionNameAndClassID("e/w", classId, testData, virtualTestSubTypeId);
            dataInTest.CompositeScore = GetSectionScoreBySectionNameAndClassID("composite", classId, testData, virtualTestSubTypeId);

            dataInTest.TotalStudents = GetTotalStudentByClassID(classId, testData);
            dataInTest.ReportType = reportType;

            return dataInTest;
        }

        private int GetSchoolOrTeacherBaselineAverageStudentNo(List<ACTSummarySchoolOrTeacherLevelData> testData, ActSummaryReportType reportType)
        {
            int averageStudentNo = -1;
            if (testData.Any(en => en.ClassId == 0))
            {
                averageStudentNo = testData.FirstOrDefault(en => en.ClassId == 0).StudentNo;
                if (averageStudentNo == 0)
                    averageStudentNo = -1;
            }

            return averageStudentNo;
        }

        private decimal GetSectionScoreBySectionNameAndStudentID(string sectionName, int studentID,
            List<ACTSummaryClassLevelData> data, int virtualTestSubTypeId)
        {
            var scoreData = data.FirstOrDefault(x => !string.IsNullOrEmpty(x.SectionName) &&
                                                     x.SectionName.Trim()
                                                         .Equals(sectionName,
                                                             StringComparison.InvariantCultureIgnoreCase)
                                                     && x.StudentID == studentID);

            if (sectionName == "writing" && virtualTestSubTypeId == (int)VirtualTestSubType.NewACT)
            {
                return scoreData == null ? -1 : scoreData.ScoreRaw;
            }

            return scoreData == null ? -1 : scoreData.ScoreScaled;
        }

        private decimal GetSectionScoreBySectionNameAndSchoolID(string sectionName, int schoolID,
            List<ACTSummaryDistrictLevelData> data, int virtualTestSubTypeId)
        {
            var query = data.Where(x =>
                !string.IsNullOrEmpty(x.SectionName)
                && x.SectionName.Trim().Equals(sectionName, StringComparison.InvariantCultureIgnoreCase)
                && x.SchoolID == schoolID);

            if (sectionName == "writing" && virtualTestSubTypeId == (int)VirtualTestSubType.NewACT)
            {
                return query.Sum(x => x.ScoreRaw);
            }

            return query.Sum(x => x.ScoreScaled);
        }

        private decimal GetSectionScoreBySectionNameAndClassID(string sectionName, int classID,
            List<ACTSummarySchoolOrTeacherLevelData> data, int virtualTestSubTypeId)
        {
            var scoreData = data.FirstOrDefault(x => !string.IsNullOrEmpty(x.SectionName) &&
                                                     x.SectionName.Trim()
                                                         .Equals(sectionName,
                                                             StringComparison.InvariantCultureIgnoreCase)
                                                     && x.ClassId == classID);

            if (sectionName == "writing" && virtualTestSubTypeId == (int)VirtualTestSubType.NewACT)
            {
                return scoreData == null ? -1 : scoreData.ScoreRaw;
            }

            return scoreData == null ? -1 : scoreData.ScoreScaled;
        }

        private decimal GetSectionAverageScoreBySectionNameAndTestID(string sectionName, int testID,
            List<ACTSummarySchoolOrTeacherLevelData> data, int virtualTestSubTypeId)
        {
            var scoreData = data.FirstOrDefault(x => !string.IsNullOrEmpty(x.SectionName) &&
                                                     x.SectionName.Trim()
                                                         .Equals(sectionName,
                                                             StringComparison.InvariantCultureIgnoreCase)
                                                     && x.VirtualTestId == testID);

            if (sectionName == "writing" && virtualTestSubTypeId == (int)VirtualTestSubType.NewACT)
            {
                return scoreData == null ? -1 : scoreData.ScoreRaw;
            }

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
            var pdf = Print(model, out url);

            var folder = LinkitConfigurationManager.GetS3Settings().ACTReportFolder;
            var bucketName = LinkitConfigurationManager.GetS3Settings().ACTReportBucket;
            var result = s3Service.UploadRubricFile(bucketName, folder + "/" + model.ReportFileName, new MemoryStream(pdf));

            return Json(new { IsSuccess = true, Url = url });
        }


        private byte[] Print(ActSummaryReportData model, out string url)
        {
            if (model.StudentIdList.Any())
            {
                model.StrStudentIdList = string.Join(",", model.StudentIdList);
            }

            url = Url.Action("ReportPrinting", "ACTSummaryReport", model, HelperExtensions.GetHTTPProtocal(Request));
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

            //var pdf = ExportToPDF(url, model.TimezoneOffset, "CLASS, TEACHER OR SCHOOL NAME");
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
        public ActionResult CheckACTS3FileExisted(string fileName)
        {
            var folder = ConfigurationManager.AppSettings["ACTReportFolder"];
            var bucketName = LinkitConfigurationManager.GetS3Settings().ACTReportBucket;
            var result = s3Service.DownloadFile(bucketName, folder + "/" + fileName);

            if (result.IsSuccess)
            {
                var s3Url = s3Service.GetPublicUrl(bucketName, folder + "/" + fileName);
                return Json(new { Result = true, Url = s3Url });
            }
            else
            {
                return Json(new { Result = false });
            }
        }
    }
}
