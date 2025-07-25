using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Envoc.Core.Shared.Extensions;
using FluentValidation;
using FluentValidation.Results;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using Newtonsoft.Json;
using Configuration = LinkIt.BubbleSheetPortal.Models.Configuration;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize]
    public class StudentTestLaunchController : BaseController
    {
        private readonly StudentTestLaunchControllerParameters parameters;
        private readonly IValidator<TestAssignmentData> testAssignmentValidator;

        public StudentTestLaunchController(StudentTestLaunchControllerParameters parameters, IValidator<TestAssignmentData> testAssignmentValidator)
        {
            this.parameters = parameters;
            this.testAssignmentValidator = testAssignmentValidator;
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.TestLaunch)]
        public ActionResult Index()
        {
            var model = new TestAssignmentViewModel
            {
                IsAdmin = IsUserAdmin(),
                CanSelectTeachers = CurrentUser.IsDistrictAdminOrPublisher,
                IsSchoolAdmin = CurrentUser.RoleId.Equals(8),
                IsPublisher = CurrentUser.RoleId.Equals((int)Permissions.Publisher),
                DistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0
            };
            return View(model);
        }

        private bool IsUserAdmin()
        {
            return parameters.UserServices.GetAdminByUsernameAndDistrict(CurrentUser.UserName, CurrentUser.DistrictId.GetValueOrDefault()).IsNotNull();
        }

        [HttpGet]
        public ActionResult GetTimingOptionByVirtualTestId(int districtId, int virtualTestId)
        {
            var virtualTestTimings =
                parameters.VirtualTestTimingService.Select().Where(x => x.DistrictId == districtId && x.VirtualTestId == virtualTestId).ToList();
            if (!virtualTestTimings.Any())
                virtualTestTimings = parameters.VirtualTestTimingService.Select().Where(x => x.DistrictId == districtId && x.VirtualTestId == 0).ToList();
            if (!virtualTestTimings.Any())
                virtualTestTimings = parameters.VirtualTestTimingService.Select().Where(x => x.DistrictId == 0 && x.VirtualTestId == 0).ToList();

            var timingOptionIds = virtualTestTimings.Select(x => x.TimingOptionId);

            var timingOptions =
                parameters.TimingOptionService.Select().Where(x => timingOptionIds.Contains(x.TimingSettingId)).ToList();

            List<ListItem> data = timingOptions.Select(o => new ListItem
            {
                Id = virtualTestTimings.SingleOrDefault(x => x.TimingOptionId == o.TimingSettingId).VirtualTestTimingId,
                Name = o.Name
            }).ToList();

            data = data.OrderBy(x => x.Id).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.TestLaunch)]
        public ActionResult Index(TestAssignmentData data)
        {
            var studentId = parameters.StudentMetaService.GetStudentIdViaStudentUser(CurrentUser.Id); 
            string testCode = "";
            var isNewTest = true;

            var qtiOnlineTestSession =
                parameters.QTIOnlineTestSessionService.Select().FirstOrDefault(x => x.StudentId == studentId && x.VirtualTestId == data.TestId);

            // If there's an already started test --> retake this test
            if (qtiOnlineTestSession != null)
            {
                // Test already completed
                if (qtiOnlineTestSession.StatusId == 4 || qtiOnlineTestSession.StatusId == 5)
                {
                    return Json(new { Result = false });
                }
                else
                {
                    testCode =
                        parameters.QTITestClassAssignmentServices.GetTestCodeByAssignmentGUID(
                            qtiOnlineTestSession.AssignmentGUId);
                    isNewTest = false;
                }
            }
            else // Create new test
            {
                var obj = new TestAssignResultViewModel();
                var districtId = CurrentUser.DistrictId ?? 0;

                var classId = parameters.ClassStudentService.GetClassStudentByStudentId(studentId).ClassId;

                data.StudentIdList = new List<string>();
                data.StudentIdList.Add(studentId.ToString());

                data.SchoolId = Convert.ToInt32(parameters.DistrictDecodeService
                    .GetDistrictDecodesOfSpecificDistrictByLabel(districtId, "SchoolStudentTest").FirstOrDefault().Value);
                data.ClassId = classId;
                data.UserId = 1; // To bypass model validation
                data.DistrictTermId = 1; // To bypass model validation

                data.GradeId = parameters.StudentServices.GetStudentById(studentId).CurrentGradeId ?? 0;

                data.SetValidator(testAssignmentValidator);
                if (!data.IsValid)
                {
                    if (Session["PrintResult"] != null)
                    {
                        obj.CurrentTab = ((TestAssignResultViewModel)Session["PrintResult"]).CurrentTab;
                    }
                    obj.ErrorList = data.ValidationErrors.ToList();
                    return Json(new { Result = false });
                }

                obj.IsClassAssign = true;
                obj.IsStudentAssign = true;
                AssignTestToStudent(data, obj);

                if (obj.ListStudentAssign.Any())
                {
                    testCode = obj.ListStudentAssign[0].TestCode;
                }
            }


            if (!string.IsNullOrEmpty(testCode))
            {
                APIAccount apiAccount = parameters.APIAccountServices.GetAPIAccountByDistrictId(CurrentUser.DistrictId.Value);

                if (apiAccount != null)
                {
                    var assignmentData =
                    parameters.QTITestClassAssignmentServices.GetAssignmentByTestCode(
                        testCode);

                    var timestamp = DateTime.UtcNow.ToString();

                    var privateKey = apiAccount.LinkitPrivateKey;

                    var rawDataObject = new
                    {
                        StudentID = studentId,
                        TestCode = testCode,
                        AssignmentGUID = assignmentData.AssignmentGuId,
                        Timestamp = timestamp,
                        IsFromSPP = true
                    };
                    string rawData = JsonConvert.SerializeObject(rawDataObject, Formatting.Indented);


                    string strData = Util.EncryptString(rawData, privateKey);
                    var url = ConfigurationManager.AppSettings["TestLaunchURL"];
                    var redirectUrl = string.Format(url, HttpUtility.UrlEncode(strData), HttpUtility.UrlEncode(apiAccount.ClientAccessKeyID.ToString()), LinkitConfigurationManager.Vault.DatabaseID);

                    var virtualTestTimmingDuration = 0;

                    if (isNewTest)
                        virtualTestTimmingDuration = GetVirtualTestTimmingDuration(data.VirtualTestTimingId);
                    else
                    {
                        virtualTestTimmingDuration = GetTestRemainingDuration(assignmentData);
                    }

                    return Json(new { Result = true, Url = redirectUrl, IsNewTest = isNewTest, VirtualTestTimmingDuration = virtualTestTimmingDuration });
                }
                else
                {
                    return Json(new { Result = false, ErrorMessage = "Error. Unable to launch a test due to invalid account. Please contact your administrator." });
                }


            }

            return Json(new { Result = false });
        }

        private int GetTestRemainingDuration(QTITestClassAssignmentData assignmentData)
        {
            var remainingDuration = 0;

            var qtiOnlineTestSession =
                parameters.QTIOnlineTestSessionService.Select()
                    .FirstOrDefault(x => x.AssignmentGUId == assignmentData.AssignmentGuId);

            if (qtiOnlineTestSession != null)
            {
                var testStartDate = qtiOnlineTestSession.StartDate;
                var minuteLasting = (int)DateTime.UtcNow.Subtract(testStartDate).TotalMinutes;

                var preference = parameters.PreferencesServices.GetPreferenceByAssignmentLeveAndID(assignmentData.QTITestClassAssignmentId);

                if (preference != null)
                {
                    var testDuration = GetDurationFromPreferenceValue(preference.Value);

                    if (testDuration > minuteLasting)
                        remainingDuration = testDuration - minuteLasting;
                }
            }

            return remainingDuration;
        }

        private int GetDurationFromPreferenceValue(string xmlValue)
        {
            try
            {
                return
                    Convert.ToInt32(
                        xmlValue.Replace("|", "").Replace("<duration>", "|").Replace("</duration>", "|").Split('|')[1]);
            }
            catch (Exception)
            {
            }

            return 0;
        }

        private int GetVirtualTestTimmingDuration(int virtualTestTimingId)
        {
            var virtualTestTiming =
                parameters.VirtualTestTimingService.Select()
                    .FirstOrDefault(x => x.VirtualTestTimingId == virtualTestTimingId);

            if (virtualTestTiming != null)
                return virtualTestTiming.Value;
            return 0;
        }

        [HttpGet]
        public ActionResult GetTests(int bankId)
        {
            var studentId = parameters.StudentMetaService.GetStudentIdViaStudentUser(CurrentUser.Id);

            var qtiOnlineTestSessions = parameters.QTIOnlineTestSessionService.Select().Where(x => x.StudentId == studentId);

            // Status: 4: Completed; 5: WaitingForReview
            var takenVirtualTestIds = qtiOnlineTestSessions.Where(x => x.StatusId == 4 || x.StatusId == 5)
                .Select(x => x.VirtualTestId);

            var alreadyStartVirtualTestIds = qtiOnlineTestSessions.Where(x => x.StatusId == 1 || x.StatusId == 2 || x.StatusId == 3)
                .Select(x => x.VirtualTestId);

            var virtualTest = parameters.TestService.GetValidTestsByBank(new List<int>() { bankId }).ToList().Where(x => takenVirtualTestIds.All(en => en != x.Id));
            var data = virtualTest.Select(x => new ListItem { Id = x.Id, Name = alreadyStartVirtualTestIds.Contains(x.Id) ? x.Name + " *" : x.Name }).OrderBy(x => x.Name);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private void AssignTestToStudent(TestAssignmentData data, TestAssignResultViewModel obj)
        {
            var lstStudent = new List<Student>();
            foreach (var item in data.StudentIdList)
            {
                int studentId = 0;
                if (int.TryParse(item, out studentId))
                {
                    var currentStudent = parameters.StudentServices.GetStudentById(studentId);
                    if (currentStudent != null)
                        lstStudent.Add(currentStudent);
                }
            }
            if (lstStudent.Count > 0)
            {
                AssignStudent(data.ClassId, lstStudent, data, obj);
            }
            else
            {
                obj.ErrorList = new List<ValidationFailure> { new ValidationFailure("error", "Class have no student active") };
            }
        }

        private void AssignStudent(int classId, List<Student> lstStudent, TestAssignmentData data, TestAssignResultViewModel obj)
        {
            DateTime currentDateTime = DateTime.UtcNow;
            if (lstStudent.Count > 0)
            {
                foreach (var item in lstStudent)
                {
                    var qtiTest = new QTITestClassAssignmentData
                    {
                        VirtualTestId = data.TestId,
                        ClassId = classId,
                        AssignmentDate = currentDateTime,
                        Code = GenerateStudentTestCode(),
                        CodeTimestamp = currentDateTime,
                        AssignmentGuId = Guid.NewGuid().ToString(),
                        TestSetting = string.Empty,
                        Status = 1,
                        ModifiedDate = currentDateTime,
                        ModifiedUserId = CurrentUser.Id,
                        ComparisonPasscodeLength = RanComparisonPasscodeLength(),
                        Type = (int)AssignmentType.Student,
                        ModifiedBy = Constanst.PortalContain
                    };
                    if(IsRequireTestTakerAuthentication(data.DistrictId, data.BankId))
                    {
                        var authenticationCode = parameters.AuthenticationCodeGenerator.GenerateAuthenticationCode();
                        var expirationDate = parameters.AuthenticationCodeGenerator.GetExpirationDate(classId);
                        qtiTest.SetAuthenticationCode(authenticationCode, expirationDate);
                    }
                    parameters.QTITestClassAssignmentServices.AssignClass(qtiTest);
                    SaveTestClassStudent(item.Id, qtiTest.QTITestClassAssignmentId);
                    //TODO: Save Setting
                    SaveSettingPreference(qtiTest.QTITestClassAssignmentId, data.DistrictId, data.BankId, data.VirtualTestTimingId);
                    if (qtiTest.ComparisonPasscodeLength != null && qtiTest.AssignmentGuId.Length > qtiTest.ComparisonPasscodeLength)
                    {
                        var testStudent = new TestStudentAssignResultViewModel
                        {
                            ID = qtiTest.QTITestClassAssignmentId,
                            StudentId = item.Id,
                            TestCode = qtiTest.Code,
                            ShortGUID = qtiTest.AssignmentGuId.Substring(0, qtiTest.ComparisonPasscodeLength.Value),
                            StudentFirstName = item.FirstName,
                            StudentLastName = item.LastName,
                            Assigned = qtiTest.AssignmentDate,
                            Test = data.TestName,
                            AuthenticationCode = qtiTest.AuthenticationCode
                        };
                        obj.ListStudentAssign.Add(testStudent);
                    }
                }
            }
        }

        private bool SaveTestClassStudent(int studentId, int classAssignmentId)
        {
            if (studentId > 0 && classAssignmentId > 0)
            {
                var obj = new QTITestStudentAssignmentData
                {
                    StudentId = studentId,
                    QTITestClassAssignmentId = classAssignmentId
                };
                parameters.QTITestStudentAssignmentServices.AssignStudent(obj);

                return true;
            }
            return false;
        }

        //ComparisonPasscodeLength: 6,7,8,9
        private int RanComparisonPasscodeLength()
        {
            var r = new Random();
            Configuration objCon = parameters.ConfigurationServices.GetConfigurationByKey(Constanst.ComparisonPasscodeLength);
            int iComparisonPasscodeLength = 9;
            if (objCon != null)
            {
                string[] arr = objCon.Value.Split(',');
                int iLength = r.Next(0, arr.Length);
                iComparisonPasscodeLength = CommonUtils.ConverStringToInt(arr[iLength], 9);
            }
            return iComparisonPasscodeLength;
        }

        private string GenerateStudentTestCode()
        {
            var r = new Random();
            Configuration objCon = parameters.ConfigurationServices.GetConfigurationByKey(Constanst.StudentTestCodeLength);
            int iTestCodeLength = 9;
            if (objCon != null)
            {
                string[] arr = objCon.Value.Split(',');
                int iLength = r.Next(0, arr.Length);
                iTestCodeLength = CommonUtils.ConverStringToInt(arr[iLength], 9);
            }

            var testCode = parameters.TestCodeGenerator.GenerateTestCode(iTestCodeLength,
                LinkitConfigurationManager.GetLinkitSettings().DatabaseID);

            return testCode;
        }

        private bool IsRequireTestTakerAuthentication(int districtId, int bankId)
        {
            var preferences = GetSettingPreference(districtId, bankId);
            var preferenceModel = parameters.PreferencesServices.ConvertToTestPreferenceModel(preferences.Value);
            var requireTestTakerAuthentication = preferenceModel?.OptionTags?.FirstOrDefault(x => x.Key == Constanst.REQUIRE_TEST_TAKER_AUTHENTICATION);
            return requireTestTakerAuthentication?.Value == "1";
        }

        private Preferences GetSettingPreference(int districtId, int bankId)
        {
            Preferences preferences;
            var districtBank = parameters.ListBankServices.GetBankDistrictByDistrictIdAndBankId(districtId, bankId);
            bool isLockedBank = districtBank != null && districtBank.BankDistrictAccessId == (int)LockBankStatus.Restricted;
            if (isLockedBank)//TODO: isMultiClass ||
            {
                preferences = parameters.PreferencesServices.GetPreferenceByLevelAndId(CurrentUser.Id, districtId, ContaintUtil.TestPreferenceLevelDistrict);
            }
            else
            {
                preferences = parameters.PreferencesServices.GetPreferenceByLevelAndId(CurrentUser.Id, districtId, ContaintUtil.TestPreferenceLevelUser);
            }
            return preferences;
        }

        private void SaveSettingPreference(int id, int districtId, int bankId, int virtualTestTimingId)
        {
            var virtualTestTiming =
                parameters.VirtualTestTimingService.Select().FirstOrDefault(x => x.VirtualTestTimingId == virtualTestTimingId);

            Preferences preferences = GetSettingPreference(districtId, bankId);

            SetPreferenceTimeLimit(preferences, virtualTestTiming);
            preferences.Id = id;
            preferences.Level = ContaintUtil.TestPreferenceLevelTestAssignment;

            var hideSupportHightlightText = parameters.ConfigurationServices.GetConfigurationByKeyWithDefaultValue(Util.HideSupportHighlightText, "false");
            if (hideSupportHightlightText.ToLower() == "true")
            {
                preferences.Value = preferences.Value.Replace("<supportHighlightText>1</supportHighlightText>",
                    "<supportHighlightText>0</supportHighlightText>").Replace("<supportHighlightText>2</supportHighlightText>",
                    "<supportHighlightText>0</supportHighlightText>");
            }
            parameters.PreferencesServices.SaveAssignment(preferences);
        }

        private void SetPreferenceTimeLimit(Preferences preferences, VirtualTestTiming virtualTestTiming)
        {
            preferences.Value = preferences.Value.Replace("<timeLimit>0</timeLimit>", "<timeLimit>1</timeLimit>");
            preferences.Value = preferences.Value.Substring(0, preferences.Value.IndexOf("<duration>") + 10)
                                + virtualTestTiming.Value.ToString()
                                + preferences.Value.Substring(preferences.Value.IndexOf("</duration>"));
        }
    }
}
