using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels.ABLESReport;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using System.Web.Script.Serialization;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using System.Net.Http;
using System.Xml.Linq;
using System.Configuration;
using System.Text;
using LinkIt.BubbleSheetPortal.Models.DTOs.Ables;
using System.Net;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.Required)]
    [AjaxAwareAuthorize]
    [VersionFilter]

    public class AblesReportController : BaseController
    {
        private readonly AblesReportControllerParameter _parameter;
        public AblesReportController(AblesReportControllerParameter parameter)
        {
            _parameter = parameter;
        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.AblesReport)]
        public ActionResult Index()
        {
            var model = new ABLESReportViewModel
            {
                CanSelectTeachers = CurrentUser.IsDistrictAdminOrPublisher || CurrentUser.IsSchoolAdmin,
                IsSchoolAdmin = CurrentUser.IsSchoolAdmin,
                IsPublisher = CurrentUser.IsPublisher,
                IsNetworkAdmin = CurrentUser.IsNetworkAdmin
            };


            model.ReportTypes = _parameter.AblesReportService.GetAllReportTypes()
                    .Select(x => new SelectListItem() { Value = x.ReportTypeId.ToString(), Text = x.Name }).ToList();
            return View(model);
        }

        public ActionResult GenerateForAdminReporting(int? districtId, int reportType, int testresultId, int studentId, string fileName)
        {
            if (!districtId.HasValue || districtId.Value <= 0)
                districtId = CurrentUser.DistrictId;

            var assRounds = _parameter.AblesReportService.GetAssessmentRounds(districtId.GetValueOrDefault());
            var testresults = new List<AblesTestResultData>();
            var results = _parameter.AblesReportService.GetAblesDataForAdminReporting(studentId, testresultId);
            foreach (var data in results)
            {
                var responses = GetPointEarned(data.Answers).OrderBy(x => x.QuestionOrder).ToList();
                var asdStatus = GetAsdStatus(data.ValueMapping, ref responses);
                if ((asdStatus != 1 && (data.ValueMapping == 4 || data.ValueMapping == 5)) ||
                    (asdStatus != 2 && (data.ValueMapping == 2 || data.ValueMapping == 3))) //wrong data
                    continue;

                var testresult = BuildAblesTestResultData(reportType, data, assRounds, asdStatus, responses);
                testresults.Add(testresult);
            }

            testresults = testresults.GroupBy(x => new { x.StudentId, x.TestId, x.TestingPeriod })
                    .Select(x => x.OrderByDescending(y => DateTime.Parse(y.Stamp)).FirstOrDefault())
                    .ToList();

            var reportJob = SaveAblesReportJob(districtId, reportType);

            var testResultBeforeCount = testresults.Count;
            testresults = RemoveWrongDatas(testresults);

            var ablesModel = BuildAblesReportData(testresults);
            var jsonAblesDataPost = new JavaScriptSerializer().Serialize(ablesModel);
            jsonAblesDataPost = jsonAblesDataPost.Replace("\\u0027", "'").Replace("\\u0026", "&");

            if (!testresults.Any())
            {
                SaveReportJobLog(reportJob, -1, "No results found.", jsonAblesDataPost);
                return Json(new { success = false, error = "No results found." }, JsonRequestBehavior.AllowGet);
            }

            if (testresults.Count < testResultBeforeCount)
            {
                var message = string.Format("{0} {1}/{2} {3}", "There are ", (testResultBeforeCount - testresults.Count), testResultBeforeCount, "test results with incorrect data.");
                SaveReportJobLog(reportJob, -1, message, jsonAblesDataPost);
                return Json(new { success = true, warning = message, reportJobId = reportJob.AblesReportJobId }, JsonRequestBehavior.AllowGet);
            }

            //send request to get pdf and upload file pdf to s3
            if (SendingRequestToUOM(fileName, jsonAblesDataPost, reportJob))
            {
                var pdfData = (byte[])HttpContext.Session[fileName];
                return File(pdfData, "application/pdf", fileName);
            }

            return Json(new { success = false, error = "An error has occurred, please contact your administrator." }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ContinueGenerateForAdminReporting(int? reportJobId, string filename)
        {
            var reportJob = _parameter.AblesReportService.GetReportJobById(reportJobId ?? 0);

            if (reportJob != null && SendingRequestToUOM(filename, reportJob.JsonAblesDataPost, reportJob))
            {
                return Json(new { success = true, filename = filename }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false, messageError = "An error has occurred, please contact your administrator." }, JsonRequestBehavior.AllowGet);
        }

        private AblesReportJob SaveAblesReportJob(int? districtId, int reportType)
        {
            var reportJob = new AblesReportJob()
            {
                DistrictId = districtId.GetValueOrDefault(),
                ReportTypeId = reportType,
                UserId = CurrentUser.Id == 0 ? 4290 : CurrentUser.Id,
                Status = 0,
                CreatedDate = DateTime.UtcNow
            };
            _parameter.AblesReportService.SaveReportJob(reportJob);
            return reportJob;
        }

        private AblesTestResultData BuildAblesTestResultData(int reportType, AblesPointsEarnedResponseData data, List<AblesAssessmentRound> assRounds, int asdStatus, List<AblesResponsesFullData> responses)
        {
            var testresult = new AblesTestResultData
            {
                ReportType = reportType,
                SchoolId = data.SchoolCode,
                StudentClass = data.ClassName,
                StudentId = GetStudentCode(data.StateCode, data.StudentCode),
                ASDStatus = asdStatus,
                TestingPeriod = GetTestingPeriod(data.AssessmentRoundID, assRounds),
                TestId = data.ValueMapping,
                Responses = responses.Select(x => new AblesResponsesData()
                {
                    QuestionOrder = x.QuestionOrder,
                    PointEarned = x.PointEarned
                }).ToList(),
                Score = responses.Sum(x => x.PointEarned),
                Completed = 1,
                Stamp = data.ResultDate.ToString("yyyy-MM-dd HH:mm:ss"),
                TestIndex = 1,
                StudentName = data.StudentName,
            };
            return testresult;
        }


        [HttpPost]
        public ActionResult Generate(AblesReportDataParam model)
        {
            if (!model.DistrictId.HasValue || model.DistrictId.Value <= 0)
                model.DistrictId = CurrentUser.DistrictId;

            if (model.ReportType.HasValue && model.ReportType.Value != (int)AblesReportEnum.School)
            {
                if (!string.IsNullOrEmpty(model.SelectedStudent))
                {
                    if (!_parameter.VulnerabilityService.CheckUserPermissionOnStudent(CurrentUser, model.SelectedStudent))
                    {
                        return Json(new { error = "There are students that you do not have permission to access." }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { error = "Please select student." }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                if (model.SchoolId.HasValue && model.SchoolId.Value > 0)
                {
                    if (!_parameter.VulnerabilityService.CheckUserCanAccessSchool(CurrentUser, model.SchoolId.Value))
                    {
                        return Json(new { error = "Has no right to access school" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { error = "Please select school" }, JsonRequestBehavior.AllowGet);
                }
            }
            if (CurrentUser.IsTeacher)
                model.TeacherId = CurrentUser.Id;
            var reportJob = new AblesReportJob()
            {
                DistrictId = model.DistrictId.GetValueOrDefault(),
                ReportTypeId = model.ReportType.GetValueOrDefault(),
                UserId = CurrentUser.Id,
                Status = 0,
                CreatedDate = DateTime.UtcNow,
                LearningArea = model.AblesTestName,
                SchoolId = model.SchoolId,
                ClassId = model.ClassId,
                TeacherId = model.TeacherId,
                DistrictTermId = model.TermId
            };
            _parameter.AblesReportService.SaveReportJob(reportJob);

            //get schoolCode
            var schoolCode = _parameter.SchoolService.GetSchoolById(model.SchoolId.GetValueOrDefault()).Code;

            var testList = new List<int>();
            var roundList = new List<int>();

            var assRounds = _parameter.AblesReportService.GetAssessmentRounds(model.DistrictId.GetValueOrDefault());
            var virtualTestMappings = _parameter.AblesReportService.GetVirtuaTestMappings(model.DistrictId.GetValueOrDefault());
            var currentRound = GetCurrentRound(assRounds);
            switch (model.ReportType)
            {
                case (int)AblesReportEnum.Rocket:
                    roundList.Add(model.TermId.GetValueOrDefault());
                    var round = GetAssessementSuite(model.TermId.GetValueOrDefault(), assRounds);
                    var testId =
                        _parameter.AblesReportService.GetVirtuaTestMapping(model.DistrictId.GetValueOrDefault(), model.AblesTestName,
                            round).VirtualTestID;

                    testList.Add(testId);
                    break;
                case (int)AblesReportEnum.Class:
                    if (currentRound != null)
                    {
                        roundList.Add(currentRound.AssessmentRoundId);
                        //get pass 1 term
                        var pass1Term = GetPass1Round(currentRound, assRounds);
                        if (pass1Term != null)
                        {
                            roundList.Add(pass1Term.AssessmentRoundId);
                        }
                        testList = GetVirtualTestIdsByAblesTestName(model.DistrictId.Value, model.AblesTestName);
                    }
                    break;
                case (int)AblesReportEnum.Profile:
                    break;
                case (int)AblesReportEnum.School:
                    roundList.Add(model.TermId.GetValueOrDefault());
                    var roundSchool = GetAssessementSuite(model.TermId.GetValueOrDefault(), assRounds);
                    var virtualTestId =
                        _parameter.AblesReportService.GetVirtuaTestMapping(model.DistrictId.Value,
                            model.AblesTestName,
                            roundSchool).VirtualTestID;
                    if (!testList.Contains(virtualTestId))
                        testList.Add(virtualTestId);

                    var ablesAssessmentRound = assRounds.FirstOrDefault(x => x.AssessmentRoundId == model.TermId.GetValueOrDefault());
                    var pass2yearsRound = Get2YearPassRound(ablesAssessmentRound, assRounds);
                    //get pass 2 years round
                    if (pass2yearsRound != null)
                    {
                        roundList.Add(pass2yearsRound.AssessmentRoundId);
                        var nextRound = GetAssessementSuite(pass2yearsRound.AssessmentRoundId, assRounds);
                        var mapping = virtualTestMappings.FirstOrDefault(
                            x => x.AblesTestName == model.AblesTestName && x.Round == nextRound);
                        var nextVirtualTestId = mapping != null ? mapping.VirtualTestID : 0;
                        if (!testList.Contains(virtualTestId))
                            testList.Add(nextVirtualTestId);
                    }
                    break;
            }

            var testIdList = string.Join(",", testList);
            var roundIdList = String.Join(",", roundList);
            if (!model.ClassId.HasValue || model.ClassId.Value <= 0)
                model.ClassId = 0;

            if (model.SchoolId.GetValueOrDefault() <= 0)
                model.SchoolId = 0;

            var isASD = false;
            if (!string.IsNullOrEmpty(model.AblesTestName) && model.AblesTestName != "-1")
            {
                var ablesMappings = virtualTestMappings.Where(x => x.AblesTestName == model.AblesTestName).ToList();
                isASD = ablesMappings.Select(x => x.IsASD).FirstOrDefault();
            }

            var tempData =
                _parameter.AblesReportService.GetPointsEarnedResponsed(model.DistrictId.GetValueOrDefault(), model.SelectedStudent, model.SchoolId ?? 0, roundIdList,
                    testIdList, isASD).ToList();

            var responsesData = new List<AblesPointsEarnedResponseData>();
            if (model.ReportType == (int)AblesReportEnum.Profile) //get data of 4 most term for each student
            {
                roundList =
                    tempData.OrderByDescending(x => x.StartDate).Select(x => x.AssessmentRoundID).Distinct().ToList();

                var studentList =
                    tempData.GroupBy(x => x.StudentID).Select(g => new
                    {
                        StudentId = g.Key,
                        RoundIDList =
                            g.OrderByDescending(x => x.StartDate)
                                .Select(x => x.AssessmentRoundID)
                                .Distinct()
                                .Take(4)
                                .ToList()
                    }).ToList();

                foreach (var student in studentList)
                {
                    var studentResponse =
                        tempData.Where(
                            x =>
                                x.StudentID == student.StudentId &&
                                student.RoundIDList.Contains(x.AssessmentRoundID)).ToList();

                    responsesData.AddRange(studentResponse);
                }
            }
            else
            {
                responsesData = tempData;
            }

            var testresults = new List<AblesTestResultData>();

            foreach (var data in responsesData)
            {
                var responses = GetPointEarned(data.Answers).OrderBy(x => x.QuestionOrder).ToList();
                var asdStatus = GetAsdStatus(data.ValueMapping, ref responses);
                if ((asdStatus != 1 && (data.ValueMapping == 4 || data.ValueMapping == 5)) || (asdStatus != 2 && (data.ValueMapping == 2 || data.ValueMapping == 3)))
                    continue;

                var studentCode = GetStudentCode(data.StateCode, data.StudentCode);

                var testresult = new AblesTestResultData
                {
                    ReportType = model.ReportType.Value,
                    SchoolId = schoolCode,
                    StudentClass =
                        (model.ClassId.Value > 0)
                            ? model.ClassName
                            : data.ClassName,
                    StudentId = studentCode,
                    StudentName = data.StudentName,
                    ASDStatus = asdStatus,
                    TestingPeriod = GetTestingPeriod(data.AssessmentRoundID, assRounds),
                    TestId = data.ValueMapping,
                    Responses = responses.Select(x => new AblesResponsesData()
                    {
                        QuestionOrder = x.QuestionOrder,
                        PointEarned = x.PointEarned
                    }).ToList(),
                    Score = responses.Sum(x => x.PointEarned),
                    Completed = 1,
                    Stamp = data.ResultDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    TestIndex = roundList.IndexOf(data.AssessmentRoundID) + 1
                };
                testresults.Add(testresult);
            }

            if (!string.IsNullOrEmpty(model.AblesTestName) && model.AblesTestName != "-1")
            {
                if (isASD)
                    testresults = testresults.Where(x => x.ASDStatus == 1).ToList();
                else
                {
                    testresults = testresults.Where(x => x.ASDStatus == 2).ToList();
                }
            }

            testresults = testresults.GroupBy(x => new { x.StudentId, x.TestId, x.TestingPeriod })
                    .Select(x => x.OrderByDescending(y => DateTime.Parse(y.Stamp)).FirstOrDefault())
                    .ToList();

            var testResultBeforeCount = testresults.Count;
            testresults = RemoveWrongDatas(testresults);           
            var ablesModel = BuildAblesReportData(testresults);
            var jsonAblesDataPost = new JavaScriptSerializer().Serialize(ablesModel);
            jsonAblesDataPost = jsonAblesDataPost.Replace("\\u0027", "'").Replace("\\u0026", "&");

            if (!testresults.Any())
            {
                SaveReportJobLog(reportJob, -1, "No results found.", jsonAblesDataPost);
                return Json(new { success = false, messageError = "No results found." }, JsonRequestBehavior.AllowGet);
            }

            if (testresults.Count < testResultBeforeCount)
            {
                var message = string.Format("{0} {1}/{2} {3}", "There are ", (testResultBeforeCount - testresults.Count), testResultBeforeCount, "test results with incorrect data.");
                SaveReportJobLog(reportJob, -1, message, jsonAblesDataPost);
                return Json(new { success = false, messageError = message, warning = true, reportJobId = reportJob.AblesReportJobId }, JsonRequestBehavior.AllowGet);
            }

            //send request to get pdf and upload file pdf to s3
            if (SendingRequestToUOM(model.FileName, jsonAblesDataPost, reportJob))
                return Json(new { success = true, filename = model.FileName }, JsonRequestBehavior.AllowGet);

            return Json(new { success = false, messageError = "An error has occurred, please contact your administrator." }, JsonRequestBehavior.AllowGet);
        }

        private bool SendingRequestToUOM(string fileName, string jsonAblesDataPost, AblesReportJob reportJob)
        {
            try
            {
                var internalAblesReportUrl = ConfigurationManager.AppSettings["InternalReportGenerateUrl"];
                var request = new InternalAblesReportRequest
                {
                    url = LinkitConfigurationManager.AppSettings.AblesGenerateReportAPIURL,
                    body = new JavaScriptSerializer().Deserialize<AblesReportData>(jsonAblesDataPost)
                };

                using (var client = new HttpClient())
                {
                    var jsonBody = new JavaScriptSerializer().Serialize(request);
                    var bodyContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                    var response = client.PostAsync(internalAblesReportUrl, bodyContent).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var pdfData = response.Content.ReadAsByteArrayAsync().Result;

                        HttpContext.Session[fileName] = pdfData;
                        SaveReportJobLog(reportJob, 1, "", jsonAblesDataPost);
                        return true;
                    }

                    var errorMessage = response.Content.ReadAsStringAsync().Result;
                    SaveReportJobLog(reportJob, -1, errorMessage, jsonAblesDataPost);

                    return false;
                }

            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                SaveReportJobLog(reportJob, -1, ex.ToString(), jsonAblesDataPost);
                return false;
            }
        }


        private static AblesReportData BuildAblesReportData(List<AblesTestResultData> testresults)
        {
            var ablesModel = new AblesReportData()
            {
                TestResults = testresults
            };

            var ablesSecureKey = LinkitConfigurationManager.AppSettings.AblesSecureKey;
            var jsonTestResult = new JavaScriptSerializer().Serialize(testresults);
            jsonTestResult = jsonTestResult.Replace("\\u0027", "'").Replace("\\u0026", "&");
            ablesModel.TimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            ablesModel.CheckSum = Md5Hash.GetMd5Hash(jsonTestResult + ablesModel.TimeStamp + ablesSecureKey);
            return ablesModel;
        }

        private static List<AblesTestResultData> RemoveWrongDatas(List<AblesTestResultData> testresults)
        {
            testresults = testresults.Where(x => (x.TestId == 6 && x.Score <= 37)
                                                 || (x.TestId == 7 && x.Score <= 36)
                                                 || (x.TestId == 12 && x.Score <= 66)
                                                 || (x.TestId == 13 && x.Score <= 69)
                                                 || (x.TestId == 2 && x.Score <= 75)
                                                 || (x.TestId == 3 && x.Score <= 75)
                                                 || (x.TestId == 4 && x.Score <= 75)
                                                 || (x.TestId == 5 && x.Score <= 75)
                                                 || (x.TestId == 10 && x.Score <= 85)
                                                 || (x.TestId == 11 && x.Score <= 86)
                                                 || (x.TestId == 8 && x.Score <= 40)
                                                 || (x.TestId == 9 && x.Score <= 40)
                                                 || (x.TestId == 14 && x.Score <= 63)
                                                 || (x.TestId == 16 && x.Score <= 63)
                                                 || (x.TestId == 18 && x.Score <= 54)
                                                 || (x.TestId == 20 && x.Score <= 59)).ToList();
            return testresults;
        }

        private int GetAsdStatus(int valueMapping, ref List<AblesResponsesFullData> responses)
        {
            var asdTest = new List<int>() { 2, 3, 4, 5 };
            var asdStatus = 2;
            if (asdTest.Contains(valueMapping) && responses.Any())
            {
                asdStatus = ASDStatus(responses.FirstOrDefault().AnswerLetter);
                responses = responses.Skip(1).ToList();
                responses.ForEach(x => { x.QuestionOrder--; });
            }
            return asdStatus;
        }

        private string GetStudentCode(string stateCode, string studentCode)
        {
            if (!string.IsNullOrEmpty(stateCode))
            {
                return stateCode.Length >= 7 ? stateCode.Substring(stateCode.Length - 7, 7) : stateCode;
            }

            return studentCode ?? string.Empty;
        }

        [HttpPost]
        public ActionResult ContinueGenerate(int? reportJobId, string filename)
        {
            if (ContinueSendRequestToUOM(reportJobId, filename))
                return Json(new { success = true, filename = filename }, JsonRequestBehavior.AllowGet);

            return Json(new { success = false, messageError = "An error has occurred, please contact your administrator." }, JsonRequestBehavior.AllowGet);
        }

        private bool ContinueSendRequestToUOM(int? reportJobId, string filename)
        {
            var reportJob = _parameter.AblesReportService.GetReportJobById(reportJobId ?? 0);
            return reportJob == null || SendingRequestToUOM(filename, reportJob.JsonAblesDataPost, reportJob);
        }

        [HttpGet]
        public ActionResult DownloadFile(string filename)
        {
            if (!string.IsNullOrEmpty(filename) && HttpContext.Session[filename] != null)
            {
                var pdfData = (byte[])HttpContext.Session[filename];
                HttpContext.Session.Remove(filename);
                return File(pdfData, "application/pdf", filename);
            }
            return Content("<script language='javascript' type='text/javascript'>alert('File does not exists.');</script>");
        }

        [HttpGet]
        public ActionResult GetTests(int? districtId)
        {
            if (!districtId.HasValue || districtId.Value <= 0)
                districtId = CurrentUser.DistrictId;

            var data = _parameter.AblesReportService.GetVirtuaTestMappings(districtId.GetValueOrDefault()).DistinctBy(x => x.AblesTestName)
                            .Select(x => new ListItemStr() { Id = x.AblesTestName, Name = x.AblesTestName })
                            .OrderBy(x => x.Name).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetSchools(int? districtId)
        {
            if (!districtId.HasValue || districtId.Value == 0)
            {
                districtId = CurrentUser.DistrictId;
            }
            var data = new List<ListItem>();
            if (CurrentUser.IsDistrictAdminOrPublisher || CurrentUser.IsNetworkAdmin)
            {
                data = _parameter.SchoolService.GetSchoolsByDistrictId(districtId.GetValueOrDefault()).Select(x => new ListItem { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name).ToList();
            }
            else
            {
                // Return access schools only
                data =
                    _parameter.UserSchoolService.GetSchoolsUserHasAccessTo(CurrentUser.Id)
                        .Select(x => new ListItem { Name = x.SchoolName, Id = x.SchoolId.Value })
                        .OrderBy(x => x.Name)
                        .ToList();
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetAssRounds(int? districtId)
        {
            if (!districtId.HasValue || districtId.Value <= 0)
            {
                districtId = CurrentUser.DistrictId;
            }

            var data = _parameter.AblesReportService.GetAssessmentRounds(districtId.GetValueOrDefault()).OrderByDescending(x => x.RoundIndex).Select(x => new ListItem { Id = x.AssessmentRoundId, Name = x.Name }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetYearBySchool(int? schoolId)
        {
            var data = _parameter.AblesReportService.GetYearBySchool(schoolId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetClasses(int? districtId, int? schoolId, int? teacherId, int? termId, string ablesTestName, int? reportType, int? year)
        {
            if (!districtId.HasValue || districtId.Value <= 0)
            {
                districtId = CurrentUser.DistrictId;
            }
            if (CurrentUser.IsTeacher)
            {
                teacherId = CurrentUser.Id;
            }
            var termIdString = string.Empty;

            if (reportType.HasValue)
            {
                if (termId.HasValue && reportType.Value == (int)AblesReportEnum.Rocket)
                    termIdString = termId.Value.ToString();
                else
                    termIdString = GetAssRoundIdString(reportType.Value, districtId.GetValueOrDefault());
            }

            var testIds = new List<int>();
            var isASD = false;
            if (!string.IsNullOrEmpty(ablesTestName) && ablesTestName != "-1")
            {
                var ablesMappings =
                    _parameter.AblesReportService.GetVirtuaTestMappingByAblesTestName(districtId.GetValueOrDefault(),
                        ablesTestName)
                        .ToList();
                testIds = ablesMappings.Select(x => x.VirtualTestID).ToList();
                isASD = ablesMappings.Select(x => x.IsASD).FirstOrDefault();
            }
            var testIdStr = string.Join(",", testIds);
            var data = _parameter.AblesReportService.GetAblesDataDropDown(districtId, schoolId, teacherId, CurrentUser.Id, CurrentUser.RoleId, termIdString, testIdStr, isASD, year ?? 0);

            data = data.OrderBy(x => x.Name).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetStudents(int? districtId, int? classId, int? termId, string ablesTestName, int? reportType)
        {
            if (!districtId.HasValue || districtId.Value <= 0)
            {
                districtId = CurrentUser.DistrictId;
            }
            var termIdString = string.Empty;
            if (reportType.HasValue)
            {
                if (termId.HasValue && reportType.Value == (int)AblesReportEnum.Rocket)
                    termIdString = termId.Value.ToString();
                else
                    termIdString = GetAssRoundIdString(reportType.Value, districtId.GetValueOrDefault());
            }

            var testIds = new List<int>();
            var isASD = false;
            if (!string.IsNullOrEmpty(ablesTestName) && ablesTestName != "-1")
            {
                var ablesMappings =
                    _parameter.AblesReportService.GetVirtuaTestMappingByAblesTestName(districtId.GetValueOrDefault(),
                        ablesTestName)
                        .ToList();
                testIds = ablesMappings.Select(x => x.VirtualTestID).ToList();
                isASD = ablesMappings.Select(x => x.IsASD).FirstOrDefault();
            }
            var testIdStr = string.Join(",", testIds);
            var data = _parameter.AblesReportService.GetStudentHasData(districtId.GetValueOrDefault(), classId, termIdString, testIdStr,
                isASD);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private string GetAssRoundIdString(int reportType, int districtId)
        {
            List<int> termList = new List<int>();
            var assRounds = _parameter.AblesReportService.GetAssessmentRounds(districtId);
            var currentRound = GetCurrentRound(assRounds);

            switch (reportType)
            {
                case (int)AblesReportEnum.Class:
                    if (currentRound != null)
                    {
                        termList.Add(currentRound.AssessmentRoundId);
                        //get pass 1 round
                        var pass1Round = GetPass1Round(currentRound, assRounds);

                        if (pass1Round != null)
                        {
                            termList.Add(pass1Round.AssessmentRoundId);
                        }
                    }
                    break;
                case (int)AblesReportEnum.School:
                    if (currentRound != null)
                    {
                        termList.Add(currentRound.AssessmentRoundId);
                        var pass2yearsRound = Get2YearPassRound(currentRound, assRounds);
                        //get pass 2 years round
                        if (pass2yearsRound != null)
                        {
                            termList.Add(pass2yearsRound.AssessmentRoundId);
                        }
                    }
                    break;
            }

            return string.Join(",", termList);
        }

        private List<int> GetVirtualTestIdsByAblesTestName(int districtId, string ablesTestName)
        {
            return _parameter.AblesReportService.GetVirtuaTestMappingByAblesTestName(districtId, ablesTestName)
                 .Select(x => x.VirtualTestID).ToList();
        }

        private string GetAssessementSuite(int roundId, List<AblesAssessmentRound> assRounds)
        {
            var result = assRounds.FirstOrDefault(x => x.AssessmentRoundId == roundId);
            if (result != null)
                return result.Round;

            return "";
        }

        private string GetTestingPeriod(int roundId, List<AblesAssessmentRound> assRounds)
        {
            var round = assRounds.FirstOrDefault(x => x.AssessmentRoundId == roundId);
            string value = string.Empty;
            return round?.Name;
        }

        private AblesAssessmentRound GetCurrentRound(List<AblesAssessmentRound> assRounds)
        {
            var currentDate = DateTime.Now;
            var round = assRounds
                .Where(x => x.DateEnd <= currentDate || (x.DateStart < currentDate && x.DateEnd > currentDate))
                .OrderByDescending(x => x.DateStart)
                .FirstOrDefault();

            return round;
        }
        private AblesAssessmentRound GetPass1Round(AblesAssessmentRound currentRound, List<AblesAssessmentRound> assRounds)
        {
            var passRound =
                            assRounds
                                .Where(x => x.DistrictId == currentRound.DistrictId && x.RoundIndex < currentRound.RoundIndex)
                                .OrderByDescending(x => x.RoundIndex)
                                .FirstOrDefault();

            return passRound;
        }
        private AblesAssessmentRound Get2YearPassRound(AblesAssessmentRound currentRound, List<AblesAssessmentRound> assRounds)
        {
            AblesAssessmentRound passRound = null;
            if (currentRound != null && assRounds.Count > 0)
            {
                passRound = assRounds
                   .Where(x => x.DistrictId == currentRound.DistrictId && x.RoundIndex < currentRound.RoundIndex)
                   .OrderByDescending(x => x.RoundIndex).Skip(3)
                   .FirstOrDefault();
            }

            return passRound;
        }

        private int ASDStatus(string value)
        {
            if (!string.IsNullOrEmpty(value) && value == "A") //answer text of True/False question
                return 1;
            return 2;
        }

        private void SaveReportJobLog(AblesReportJob reportJob, int status, string errorMsg, string data)
        {
            reportJob.Status = status;
            if (!string.IsNullOrEmpty(errorMsg))
                reportJob.ErrorMessage = errorMsg;
            reportJob.JsonAblesDataPost = data;
            _parameter.AblesReportService.SaveReportJob(reportJob);
        }
        public ActionResult DownloadReport()
        {
            var model = new AblesDownloadReportViewModel
            {
                ResultDateFrom = DateTime.Now.AddDays(-7),
                ResultDateTo = DateTime.Now,
                IsPublisher = CurrentUser.IsPublisher,
                IsNetworkAdmin = CurrentUser.IsNetworkAdmin
            };
            return View(model);
        }

        public ActionResult SearchReportDownload(AblesDownloadReportViewModel model)
        {
            var ablesReportJobs = new List<AblesReportJobViewModel>();
            if (!CurrentUser.IsPublisher && !CurrentUser.IsNetworkAdmin)
                model.DistrictId = CurrentUser.DistrictId;

            if (model.DistrictId.HasValue)
            {
                ablesReportJobs = _parameter.AblesReportService.GetAblesReportJobs(model.DistrictId.Value, CurrentUser.Id, model.ResultDateFrom, model.ResultDateTo.AddDays(1))
                .Select(x => new AblesReportJobViewModel
                {
                    CreatedDate = x.CreatedDate,
                    CreatedDateString = x.CreatedDate.ToString("yyyy/MM/dd HH:mm:ss"),
                    DownloadUrl = x.DownloadUrl,
                    AblesReportJobId = x.AblesReportJobId,
                    LearningArea = x.LearningArea,
                    SchoolName = x.SchoolName,
                    TeacherName = x.TeacherName,
                    ClassName = x.ClassName,
                    AssessmentRound = x.AssessmentRound,
                    ReportName = x.ReportName
                }).ToList();
            }

            var parser = new DataTableParser<AblesReportJobViewModel>();
            return Json(parser.Parse(ablesReportJobs.AsQueryable()), JsonRequestBehavior.AllowGet);
        }

        private List<AblesResponsesFullData> GetPointEarned(string answers)
        {
            if (string.IsNullOrWhiteSpace(answers)) return new List<AblesResponsesFullData>();
            var xdoc = XDocument.Parse(answers);
            var result = new List<AblesResponsesFullData>();
            foreach (var item in xdoc.Element("Answers").Elements("Answer"))
            {
                var data = new AblesResponsesFullData();
                data.QuestionOrder = GetIntValue(item.Element("QuestionOrder"));
                data.PointEarned = GetIntValue(item.Element("PointsEarned"));
                data.AnswerLetter = GetStringValue(item.Element("AnswerLetter"));
                result.Add(data);
            }

            return result;
        }
        private int GetIntValue(XElement element)
        {
            if (element == null) return 0;
            return Convert.ToInt32(element.Value);
        }
        private string GetStringValue(XElement element)
        {
            if (element == null) return string.Empty;
            return element.Value;
        }
    }
}
