using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.ManagePreference;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using LinkIt.BubbleSheetPortal.Web.Helpers.Constants;
using LinkIt.BubbleSheetPortal.Web.Models;
using LinkIt.BubbleSheetPortal.Models.Enum;
using LinkIt.BubbleSheetPortal.Models.DTOs.TestPreferences;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.ReadOnly)]
    [AjaxAwareAuthorize]
    [VersionFilter]
    public class TestPreferenceController : BaseController
    {
        private readonly PreferencesService preferenceService;
        private readonly ListBankService listBankService;
        private readonly BankDistrictService bankDistrictService;
        private readonly DistrictDecodeService _districtDecodeService;
        private readonly VulnerabilityService _vulnerabilityService;
        private readonly ConfigurationService _configurationService;
        private readonly StateService _stateService;
        private readonly UserService _userService;

        private Dictionary<int, string> levels = new Dictionary<int, string>()
        {
            { (int)TestPreferenceLevel.Enterprise, ContaintUtil.TestPreferenceLevelEnterprise },
            { (int)TestPreferenceLevel.SurveyEnterprise, ContaintUtil.TestPreferenceLevelEnterprise },
            { (int)TestPreferenceLevel.District, ContaintUtil.TestPreferenceLevelDistrict },
            { (int)TestPreferenceLevel.SurveyDistrict, ContaintUtil.TestPreferenceLevelDistrict },
            { (int)TestPreferenceLevel.School, ContaintUtil.TestPreferenceLevelSchool },
            { (int)TestPreferenceLevel.User, ContaintUtil.TestPreferenceLevelUser },
            { (int)TestPreferenceLevel.TestDesign, ContaintUtil.TestPreferenceLevelTest },
            { (int)TestPreferenceLevel.TestAssignment, ContaintUtil.TestPreferenceLevelTestAssignment }
        };

        public TestPreferenceController(PreferencesService pPreferenceService, ListBankService plistBankService
            , BankDistrictService pBankDistrictService, DistrictDecodeService districtDecodeService, VulnerabilityService vulnerabilityService,
            ConfigurationService configurationService, StateService stateService, UserService userService)
        {
            this.preferenceService = pPreferenceService;
            this.listBankService = plistBankService;
            this.bankDistrictService = pBankDistrictService;
            _districtDecodeService = districtDecodeService;
            _vulnerabilityService = vulnerabilityService;
            _configurationService = configurationService;
            _stateService = stateService;
            _userService = userService;
        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.OnlinetestPreference)]
        public ActionResult Index()
        {
            var obj = new TestPreferenceViewModel()
            {
                CurrentDistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0,
                CurrentRoleId = CurrentUser.RoleId
            };

            if (CurrentUser.IsNetworkAdmin)
            {
                obj.ListDistricIds = CurrentUser.GetMemberListDistrictId();
            }

            ViewBag.DateFormat = _configurationService.GetConfigurationByKey(Constanst.JQueryDateFormat)?.Value;
            return View(obj);
        }

        public ActionResult LoadSettings(int levelSetting, int districtId, int? schoolId)
        {
            if (!Util.HasRightOnDistrict(CurrentUser, districtId))
            {
                return Json(new { error = "Has no right on district." }, JsonRequestBehavior.AllowGet);
            }
            var testSettings = BindDataToModel(levelSetting, districtId, schoolId) ?? new TestSettingsMap();

            var hideSupportHightlightText = _configurationService.GetConfigurationByKeyWithDefaultValue(Util.HideSupportHighlightText, "false");
            ViewBag.HideSupportHighlightText = hideSupportHightlightText;
            testSettings.IsSupportQuestionGroup = CheckUserSupportQuestionGroup(districtId);

            ViewBag.IsSettingScope = true;
            ViewBag.EnableLock = true;
            ViewBag.DateFormat = _configurationService.GetConfigurationByKey(Constanst.JQueryDateFormat)?.Value;
            ViewBag.LastUpdatedDate = testSettings.TestPreferenceModel.LastUpdatedDateString;
            ViewBag.CurrentLevelId = levelSetting;
            ViewBag.IsTeacherOrSchoolAdmin = CurrentUser.IsTeacher || CurrentUser.IsSchoolAdmin;
            return PartialView("_TestSetting", testSettings);
        }

        [NonAction]
        public TestSettingsMap BindDataToModel(int levelSetting, int districtId, int? schoolId)
        {
            int currentDistrictId = districtId;
            int currentUserId = CurrentUser.Id;
            var isSurvey = (levelSetting == (int)TestPreferenceLevel.SurveyEnterprise || levelSetting == (int)TestPreferenceLevel.SurveyDistrict);
            if (!CurrentUser.IsPublisher() && !CurrentUser.IsNetworkAdmin)
            {
                currentDistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;
            }
            var tsmap = new TestSettingsMap()
            {
                DistrictId = districtId,
                SettingLevelID = levelSetting,
                IsSurvey = isSurvey,
                TestPreferenceModel = new TestPreferenceModel()
            };

            var preferences = preferenceService.GetPreferenceInCurrentLevel(new GetPreferencesParams
            {
                CurrrentLevelId = levelSetting,
                DistrictId = districtId,
                UserId = CurrentUser.Id,
                UserRoleId = CurrentUser.RoleId,
                IsSurvey = isSurvey,
                IsFromOnlineTestingPreferences = true,
                StateId = CurrentUser.StateId.GetValueOrDefault(),
                SchoolId = schoolId
            });
            if (preferences != null)
            {
                preferenceService.AddDefaultTagOptions(preferences.OptionTags);
                AddTooltipForPreferences(preferences);
                tsmap.TestPreferenceModel = preferences;
                var levelName = string.Empty;
                levels.TryGetValue(levelSetting, out levelName);
                tsmap.SettingLevel = levelName;
                ConvertTestSchedule(tsmap);
            }

            var useTestScoreExtract = _districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(districtId, Util.DistrictDecode_TestScoreExtract).Any();

            var gradebookSISValue = _districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(districtId, Util.DistrictDecode_SendTestResultToGenesis).Select(c => c.Value).FirstOrDefault();

            tsmap.TestExtractOptions = new TestExtractOptions();
            tsmap.TestExtractOptions.IsUseTestExtract = useTestScoreExtract
                || levelSetting == (int)TestPreferenceLevel.Enterprise
                || (gradebookSISValue != null && gradebookSISValue != "0");

            if (useTestScoreExtract)
            {
                tsmap.TestExtractOptions.Gradebook = true;
            }

            if (gradebookSISValue != null)
            {
                var gradebookSISIds = gradebookSISValue.ToIntArray("|").Where(c => c > 0).ToArray();
                if (gradebookSISIds.Any())
                {
                    if (gradebookSISIds.Contains((int)GradebookSIS.CleverApi))
                    {
                        tsmap.TestExtractOptions.CleverApi = true;
                    }
                    tsmap.TestExtractOptions.Gradebook = true;

                    if (gradebookSISIds.Contains((int)GradebookSIS.Realtime))
                    {
                        tsmap.TestExtractOptions.StudentRecord = true;
                    }
                    if (gradebookSISIds.DoesShowExportScoreTypeOption())
                    {
                        tsmap.TestExtractOptions.ShowRawOrPercentOption = true;
                    }
                }
            }

            if (!tsmap.IsUseTestExtract)
            {
                //TODO: only use thi value when " + LabelHelper.DistrictLabel + " have record on DistrictDecode
                var tmp = tsmap.TestPreferenceModel.OptionTags.FirstOrDefault(o => o.Key.Equals("TestExtract"));
                if (tmp != null)
                    tmp.Value = "0";
            }
            if (CurrentUser.RoleId == (int)Permissions.Publisher ||
                CurrentUser.RoleId == (int)Permissions.DistrictAdmin ||
                CurrentUser.RoleId == (int)Permissions.NetworkAdmin)
            {
                tsmap.CanEditOverrideAutoGradedItems = true;
            }

            var vDeadLine = tsmap.TestPreferenceModel.OptionTags.FirstOrDefault(o => o.Key.Equals("deadline"));
            var vDuration = tsmap.TestPreferenceModel.OptionTags.FirstOrDefault(o => o.Key.Equals("duration"));
            if (vDeadLine != null)
                tsmap.Deadline = vDeadLine.Value;
            if (vDuration != null)
                tsmap.Duration = vDuration.Value;
            return tsmap;
        }

        private void ConvertTestSchedule(TestSettingsMap tsmapEnterPrise)
        {
            // keep local time
            var startDateTag = tsmapEnterPrise.TestPreferenceModel.OptionTags.Where(m => m.Key == PreferenceTagConstant.TestScheduleFromDate).FirstOrDefault();
            var endDateTag = tsmapEnterPrise.TestPreferenceModel.OptionTags.Where(m => m.Key == PreferenceTagConstant.TestScheduleToDate).FirstOrDefault();
            var timeOffsetTag = tsmapEnterPrise.TestPreferenceModel.OptionTags.Where(m => m.Key == PreferenceTagConstant.TestScheduleTimezoneOffset).FirstOrDefault();

            if (startDateTag != null && endDateTag != null && timeOffsetTag != null)
            {
                tsmapEnterPrise.TestPreferenceModel.OptionTags.Remove(startDateTag);
                tsmapEnterPrise.TestPreferenceModel.OptionTags.Remove(endDateTag);

                var startDate = DateTime.Parse(startDateTag.Value).ToUniversalTime();
                var endDate = DateTime.Parse(endDateTag.Value).ToUniversalTime();
                var timeOffset = int.Parse(timeOffsetTag.Value);

                startDate = KeepLocalTimeWithDiffTimezone(startDate, timeOffset);
                endDate = KeepLocalTimeWithDiffTimezone(endDate, timeOffset);

                tsmapEnterPrise.TestPreferenceModel.OptionTags.Add(new Tag { Key = PreferenceTagConstant.TestScheduleFromDate, Value = startDate.ToString() });
                tsmapEnterPrise.TestPreferenceModel.OptionTags.Add(new Tag { Key = PreferenceTagConstant.TestScheduleToDate, Value = endDate.ToString() });
            }
        }

        private DateTime KeepLocalTimeWithDiffTimezone(DateTime startDate, int timeOffset)
        {
            var localSetTime = startDate.AddMinutes(-1 * timeOffset);
            var destDate = new DateTime(localSetTime.Year, localSetTime.Month, localSetTime.Day, localSetTime.Hour, localSetTime.Minute, localSetTime.Second, DateTimeKind.Unspecified);

            return destDate;
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult SaveSettings(TestPreferenceModel obj, int settingLevel, int districtId, int schoolId)
        {
            if (!Util.HasRightOnDistrict(CurrentUser, districtId))
            {
                return Json(new { Sucess = false, error = "Has no right on district." }, JsonRequestBehavior.AllowGet);
            }
            int currentDistrictId = districtId;
            if (!CurrentUser.IsPublisher() && !CurrentUser.IsNetworkAdmin)
            {
                currentDistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;
            }

            if (!CurrentUser.IsPublisher && settingLevel == (int)TestPreferenceLevel.Enterprise)
            {
                //not allow users that are not publisher set enterprise level preference
                return Json(new { Sucess = false });
            }
            if (CurrentUser.IsSchoolAdmin && settingLevel == (int)TestPreferenceLevel.District)
            {
                //not allow users that are not publisher set enterprise level preference
                return Json(new { Sucess = false });
            }

            if (CurrentUser.IsTeacher && settingLevel == (int)TestPreferenceLevel.District)
            {
                //not allow users that are not publisher set enterprise level preference
                return Json(new { Sucess = false });
            }

            if (obj != null && settingLevel > 0)
            {
                var hideSupportHightlightText = _configurationService.GetConfigurationByKeyWithDefaultValue(Util.HideSupportHighlightText, "false");
                if (hideSupportHightlightText.ToLower() == "true")
                {
                    preferenceService.OffSupportHighLightText(obj);
                }
                var pre = new Preferences();
                var preferences = preferenceService.ConvertTestPreferenceModelToString(obj);
                var preferenceLevels = BuildPreferencesDictionary(preferences, currentDistrictId, schoolId);
                preferenceLevels.TryGetValue(settingLevel, out pre);
                pre.UpdatedBy = CurrentUser.Id;

                preferenceService.Save(pre);
            }
            return Json(new { Success = true, InforUpdated = GetInforUpdated() }, JsonRequestBehavior.AllowGet);
        }

        private Dictionary<int, Preferences> BuildPreferencesDictionary(string preferences, int districtId, int schoolId)
        {
            Dictionary<int, Preferences> preferenceLevels = new Dictionary<int, Preferences>();
            // Enterprise level
            preferenceLevels.Add((int)TestPreferenceLevel.Enterprise, new Preferences { Value = preferences, Level = ContaintUtil.TestPreferenceLevelEnterprise, Id = 0, Label = ContaintUtil.TestPreferenceLabelTest });
            preferenceLevels.Add((int)TestPreferenceLevel.SurveyEnterprise, new Preferences { Value = preferences, Level = ContaintUtil.TestPreferenceLevelEnterprise, Id = 0, Label = ContaintUtil.TestPreferenceLabelSurvey });
            // District level
            preferenceLevels.Add((int)TestPreferenceLevel.District, new Preferences { Value = preferences, Level = ContaintUtil.TestPreferenceLevelDistrict, Id = districtId, Label = ContaintUtil.TestPreferenceLabelTest });
            preferenceLevels.Add((int)TestPreferenceLevel.SurveyDistrict, new Preferences { Value = preferences, Level = ContaintUtil.TestPreferenceLevelDistrict, Id = districtId, Label = ContaintUtil.TestPreferenceLabelSurvey });
            // User level
            preferenceLevels.Add((int)TestPreferenceLevel.User, new Preferences { Value = preferences, Level = ContaintUtil.TestPreferenceLevelUser, Id = CurrentUser.Id, Label = ContaintUtil.TestPreferenceLabelTest });
            preferenceLevels.Add((int)TestPreferenceLevel.School, new Preferences { Value = preferences, Level = ContaintUtil.TestPreferenceLevelSchool, Id = schoolId, Label = ContaintUtil.TestPreferenceLabelTest });

            return preferenceLevels;
        }

        public ActionResult GetBankByDistrict(BankCustomListViewModel model)
        {
            var parser = new DataTableParser<BankCustomListViewModel>();

            var data = GetBankByDistrictIdAndSubjectName(model);

            return new JsonNetResult { Data = parser.Parse(data, true) };
        }

        private IQueryable<BankCustomListViewModel> GetBankByDistrictIdAndSubjectName(BankCustomListViewModel model)
        {

            int? vDistrictId = null;
            if ((CurrentUser.IsPublisher || CurrentUser.IsNetworkAdmin) && model.DistrictId.HasValue)
            {
                vDistrictId = model.DistrictId;
            }
            else if (!CurrentUser.IsPublisher && !CurrentUser.IsNetworkAdmin && CurrentUser.DistrictId.HasValue)
            {
                vDistrictId = CurrentUser.DistrictId;
            }
            else if (!CurrentUser.IsPublisher && !CurrentUser.IsNetworkAdmin)
            {
                return new List<BankCustomListViewModel>().AsQueryable();
            }

            vDistrictId = vDistrictId ?? -1;
            //var subjectId = model.SubjectId.HasValue ? model.SubjectId.Value : 0;
            var subjectIds = model.SubjectIds != null && !model.SubjectIds.Contains(0) ? model.SubjectIds : null;
            var gradeId = model.GradeId.HasValue ? model.GradeId.Value : 0;
            var bankDistrictAccessId = model.BankDistrictAccessId.HasValue ? model.BankDistrictAccessId.Value : 0;
            return listBankService.GetBankByDistrictIdAndSubjectName(vDistrictId.Value, subjectIds, bankDistrictAccessId, gradeId).Where(x => x.Archived == false).Select(o => new BankCustomListViewModel
            {
                BankDistrictId = o.BankDistrictId,
                Name = o.Name,
                BankDistrictAccessId = o.BankDistrictAccessId == 0 ? o.BankAccessId : o.BankDistrictAccessId,
                Hide = o.Hide,
                SubjectId = o.SubjectId,
                DistrictId = o.DistrictId == 0 ? o.CreateBankDistrictId : o.DistrictId,
                BankAccessId = o.BankAccessId,
                SubjectName = o.SubjectName,
                GradeName = o.GradeName

            });
        }
        public ActionResult GetBankByDistrictNew(BankCustomListViewModel model)
        {
            var parser = new DataTableParser<BankCustomListViewModel>();
            int? vDistrictId = null;
            if (CurrentUser.IsPublisher && model.DistrictId.HasValue)
            {
                vDistrictId = model.DistrictId;
            }
            else if (!CurrentUser.IsPublisher && CurrentUser.DistrictId.HasValue)
            {
                vDistrictId = CurrentUser.DistrictId;
            }
            else if (!CurrentUser.IsPublisher)
            {
                return Json(parser.Parse(new List<BankCustomListViewModel>().AsQueryable()),
                            JsonRequestBehavior.AllowGet);
            }

            vDistrictId = vDistrictId ?? -1;
            var subjectId = model.SubjectId.HasValue ? model.SubjectId.Value : 0;
            var gradeId = model.GradeId.HasValue ? model.GradeId.Value : 0;
            var bankDistrictAccessId = model.BankDistrictAccessId.HasValue ? model.BankDistrictAccessId.Value : 0;

            var data = listBankService.GetBankByDistrictIdAndSubjectId(vDistrictId.Value, subjectId, bankDistrictAccessId, gradeId).Select(o => new BankCustomListViewModel
            {
                BankDistrictId = o.BankDistrictId,
                Name = o.Name,
                BankDistrictAccessId = o.BankDistrictAccessId == 0 ? o.BankAccessId : o.BankDistrictAccessId,
                SubjectId = o.SubjectId,
                DistrictId = o.DistrictId == 0 ? o.CreateBankDistrictId : o.DistrictId,
                BankAccessId = o.BankAccessId,
                SubjectName = o.SubjectName,
                GradeName = o.GradeName

            });
            //data = data.OrderBy(o => o.SubjectName).ThenBy(o => o.GradeName).ThenBy(o => o.Name);

            return new JsonNetResult { Data = parser.Parse(data) };
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult UpdateStatusBankDistrict(int bankDistrictId)
        {
            if (bankDistrictId > 0)
            {
                if (!CurrentUser.IsPublisher)
                {
                    var data = GetBankByDistrictIdAndSubjectName(new BankCustomListViewModel() { DistrictId = CurrentUser.DistrictId.GetValueOrDefault() });
                    if (!data.Any(x => x.BankDistrictId == bankDistrictId))
                    {
                        return Json(new { Data = "Has no righ on the test bank." }, JsonRequestBehavior.AllowGet);
                    }
                }

                if (bankDistrictService.UpdateStatus(bankDistrictId, CurrentUser, CurrentUser.GetMemberListDistrictId()))
                {
                    return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { Data = "Update Status Fail." }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult HideBankDistrict(int bankDistrictId, bool hide, int? selectDistrictId)
        {
            if (bankDistrictId > 0)
            {
                try
                {
                    var bankDistrict = bankDistrictService.GetBankDistrictById(bankDistrictId);
                    if (bankDistrict != null)
                    {
                        //check user right on the bank
                        //if(!_vulnerabilityService.HasRightToEditTestBank(CurrentUser,bankDistrict.BankId,CurrentUser.GetMemberListDistrictId()))
                        //{
                        //    return Json(new { Data = "Has no righ on the test bank." }, JsonRequestBehavior.AllowGet);
                        //}
                        if (!CurrentUser.IsPublisher)
                        {
                            var data = GetBankByDistrictIdAndSubjectName(new BankCustomListViewModel() { DistrictId = selectDistrictId.GetValueOrDefault() });
                            if (!data.Any(x => x.BankDistrictId == bankDistrictId))
                            {
                                return Json(new { Data = "Has no righ on the test bank." }, JsonRequestBehavior.AllowGet);
                            }
                        }

                    }
                    if (bankDistrictService.HideBankDistrict(bankDistrictId, hide, CurrentUser, CurrentUser.GetMemberListDistrictId()))
                    {
                        return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    PortalAuditManager.LogException(ex);
                    return Json(new { Data = "Change Hide Fail." }, JsonRequestBehavior.AllowGet);
                }

            }
            return Json(new { Data = "Change Hide Fail." }, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.OnlinetestLockUnlockBank)]
        public ActionResult LockUnlockTestBanks()
        {
            var obj = new TestPreferenceViewModel
            {
                CurrentRoleId = CurrentUser.RoleId
            };
            obj.IsNetworkAdmin = false;
            if (CurrentUser.IsNetworkAdmin)
            {
                obj.IsNetworkAdmin = true;
                obj.ListDistricIds = CurrentUser.GetMemberListDistrictId();
            }
            return View("LockUnlockTestBanks", obj);
        }

        [HttpGet]
        public ActionResult GetOverrideAutoGradedOfAssignment(int? qtiTestClassAssignmentID)
        {
            var overrideAutoGraded = false;
            var overrideAutoGradedOptionValue = string.Empty;

            if (qtiTestClassAssignmentID.HasValue)
            {
                var preference = preferenceService.GetPreferenceByAssignmentLeveAndID(qtiTestClassAssignmentID.Value);
                if (preference != null)
                {
                    var obj = new ETLXmlSerialization<TestSettingsMap>();
                    var tsmap = obj.DeserializeXmlToObject(preference.Value);
                    if (tsmap != null && tsmap.TestSettingViewModel != null)
                    {
                        overrideAutoGraded = tsmap.TestSettingViewModel.OverrideAutoGradedTextEntry == "1";
                        overrideAutoGradedOptionValue = tsmap.TestSettingViewModel.OverrideAutoGradedTextEntry;
                    }
                }
            }

            if (CurrentUser.IsPublisher || CurrentUser.IsNetworkAdmin || CurrentUser.IsDistrictAdmin)
            {
                overrideAutoGraded = true;
            }

            var overrideItems = GetOverrideItems(overrideAutoGraded);

            return Json(new
            {
                OverrideAutoGraded = overrideAutoGraded,
                OverrideItems = overrideItems,
                OverrideAutoGradedOptionValue = overrideAutoGradedOptionValue
            }, JsonRequestBehavior.AllowGet);
        }

        private List<int> GetOverrideItems(bool overrideAutoGraded)
        {
            return PreferenceUtil.GetOverrideItems(CurrentUser.RoleId, overrideAutoGraded);
        }

        private bool CheckUserSupportQuestionGroup(int districtId)
        {
            //TODO: Maybe check Publisher or NetworkAdmin
            var vConfiguration = _configurationService.GetConfigurationByKey(Constanst.IsSupportQuestionGroup);
            if (vConfiguration != null && vConfiguration.Value.Equals("true", StringComparison.CurrentCultureIgnoreCase))
                return true;
            return _districtDecodeService.GetDistrictDecodeByLabel(districtId, Constanst.IsSupportQuestionGroup);
        }

        private void AddTooltipForPreferences(TestPreferenceModel model)
        {
            model.OptionTags.ForEach(o =>
            {
                o.Tooltips = LocalizeHelper.LocalizedToString($"TestPreference_{o.Key}").ToString();
            });

            model.ToolTags.ForEach(o =>
            {
                o.Tooltips = LocalizeHelper.LocalizedToString($"TestPreference_{o.Key}").ToString();
            });
        }

        private object GetInforUpdated()
        {
            var now = DateTime.UtcNow;
            var timeZoneId = _stateService.GetTimeZoneId(CurrentUser.StateId.GetValueOrDefault());
            var lastUpdatedDate = !string.IsNullOrEmpty(timeZoneId) ? now.ConvertTimeFromUtc(timeZoneId).DisplayDateWithFormat(true) : now.DisplayDateWithFormat(true);
            var user = _userService.GetUserById(CurrentUser.Id);
            var fullName = user != null ? $"{user.LastName}, {user.FirstName}" : string.Empty;

            return new { LastUpdatedDate = lastUpdatedDate, LastUpdatedByUser = fullName };
        }
    }
}
