using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DataLockerPreferencesSetting;
using LinkIt.BubbleSheetPortal.Models.DTOs.TestPreferences;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.SessionState;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{

    [SessionState(SessionStateBehavior.ReadOnly)]
    [AjaxAwareAuthorize]
    [VersionFilter]
    public class DataLockerPreferencesController : BaseController
    {
        private readonly PreferencesService _preferenceService;
        private readonly ConfigurationService _configurationService;
        private readonly TestAssignmentControllerParameters _parameters;
        private readonly StateService _stateService;
        private readonly UserService _userService;

        private Dictionary<int, string> levels = new Dictionary<int, string>()
        {
            { (int)DataLockerPreferencesLevel.Enterprise, ContaintUtil.DataLockerPreferenceLevelEnterprise },
            { (int)DataLockerPreferencesLevel.District, ContaintUtil.DataLockerPreferenceLevelDistrict },
            { (int)DataLockerPreferencesLevel.School, ContaintUtil.DataLockerPreferenceLevelSchool }
        };

        public DataLockerPreferencesController(PreferencesService pPreferenceService, ConfigurationService configurationService, TestAssignmentControllerParameters parameters, StateService stateService, UserService userService)
        {
            _preferenceService = pPreferenceService;
            _configurationService = configurationService;
            _parameters = parameters;
            _stateService = stateService;
            _userService = userService;
        }

        public ActionResult Index()
        {
            var obj = new DataLockerPreferencesViewModel()
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
            if (levelSetting != (int)DataLockerPreferencesLevel.School)
            {
                if (!Util.HasRightOnDistrict(CurrentUser, districtId))
                {
                    return Json(new { error = "Has no right on district." }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                if (!Util.HasRightOnSchoolLevel(CurrentUser, levelSetting))
                {
                    return Json(new { error = "Has no right on school." }, JsonRequestBehavior.AllowGet);
                }
            }

            var dataSettings = BindDataToModel(levelSetting, districtId, null, schoolId) ?? new DataLockerPreferencesSettingMap();

            ViewBag.IsSettingScope = true;
            ViewBag.EnableLock = true;
            ViewBag.DateFormat = _configurationService.GetConfigurationByKey(Constanst.JQueryDateFormat)?.Value;
            ViewBag.LastUpdatedDate = dataSettings.DataLockerPreferencesSettingModal.LastUpdatedDateString;
            ViewBag.LastUpdatedByUser = dataSettings.DataLockerPreferencesSettingModal.LastUpdatedByUser;
            ViewBag.IsTeacherOrSchoolAdmin = CurrentUser.IsTeacher || CurrentUser.IsSchoolAdmin;
            ViewBag.CurrentLevelId = levelSetting;
            return PartialView("_DataLockerPreferences", dataSettings);
        }
        [NonAction]
        public DataLockerPreferencesSettingMap BindDataToModel(int levelSetting, int districtId, int? virtualTestId, int? schoolId)
        {            
            if (!CurrentUser.IsPublisher() && !CurrentUser.IsNetworkAdmin)
            {
                districtId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;
            }
            var dataSettingsMap = new DataLockerPreferencesSettingMap()
            {
                DistrictId = districtId,
                SettingLevelID = levelSetting,
                DataLockerPreferencesSettingModal = new DataLockerPreferencesSettingModal(),
                Tooltips = _preferenceService.GetDataLockerPreferenceLocalize(districtId)
            };
            var preferences = _preferenceService.GetDataLockerPreferencesLevel(new GetPreferencesParams
            {
                CurrrentLevelId = levelSetting,
                DistrictId = districtId,
                SchoolId = schoolId ?? 0,
                UserId = CurrentUser.Id,
                UserRoleId = CurrentUser.RoleId,
                IsSurvey = false,
                StateId = CurrentUser.StateId.GetValueOrDefault(),
                VirtualTestId = virtualTestId.GetValueOrDefault()
            });

            if (preferences != null)
            {
                dataSettingsMap.DataLockerPreferencesSettingModal = preferences;
                var levelName = string.Empty;
                levels.TryGetValue(levelSetting, out levelName);
                dataSettingsMap.SettingLevel = levelName;
            }

            return dataSettingsMap;
        }
        public ActionResult LoadDataLockerPreferenceByVirtualTest(int virtualTestId)
        {
            ViewBag.DateFormat = _configurationService.GetConfigurationByKey(Constanst.JQueryDateFormat)?.Value;
            ViewBag.IsFormUse = true;
            ViewBag.VirtualTestId = virtualTestId;
            var dataSettings = BindDataToModel((int)DataLockerPreferencesLevel.Form, CurrentUser.DistrictId.GetValueOrDefault(), virtualTestId, null) ?? new DataLockerPreferencesSettingMap();
            ViewBag.LastUpdatedDate = dataSettings.DataLockerPreferencesSettingModal.LastUpdatedDateString;
            ViewBag.CurrentLevelId = (int)DataLockerPreferencesLevel.Form;
            return PartialView("_DataLockerPreferences", dataSettings);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult SaveSettings(DataLockerPreferencesSettingModal obj, int settingLevel, int Id)
        {
            if (settingLevel == (int)DataLockerPreferencesLevel.Form && !_parameters.VulnerabilityService.HasRighToEditVirtualTest(CurrentUser, Id, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(new { Sucess = false, error = "Has no right on the form." }, JsonRequestBehavior.AllowGet);
            }
            if (settingLevel != (int)DataLockerPreferencesLevel.Form && settingLevel != (int)DataLockerPreferencesLevel.School && !Util.HasRightOnDistrict(CurrentUser, Id))
            {
                return Json(new { Sucess = false, error = "Has no right on district." }, JsonRequestBehavior.AllowGet);
            }
            if (settingLevel == (int)DataLockerPreferencesLevel.School && !Util.HasRightOnSchoolLevel(CurrentUser, settingLevel))
            {
                return Json(new { Sucess = false, error = "Has no right on school." }, JsonRequestBehavior.AllowGet);
            }
            int currentDistrictId = Id;
            if (!CurrentUser.IsPublisher()
                    && !CurrentUser.IsNetworkAdmin
                    && settingLevel != (int)DataLockerPreferencesLevel.Form
                    && settingLevel != (int)DataLockerPreferencesLevel.School)
            {
                currentDistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;
            }
            if (!Util.HasRightOnLevel(CurrentUser, settingLevel))
            {
                //not allow users that are not publisher set enterprise level preference
                return Json(new { Sucess = false, error = "Has no right on Level." }, JsonRequestBehavior.AllowGet);
            }            

            if (obj != null && settingLevel > 0)
            {
                var pre = new Preferences();
                var preferences = JsonConvert.SerializeObject(obj.DataSettings);
                var preferenceLevels = BuildPreferencesDictionary(preferences, currentDistrictId);
                preferenceLevels.TryGetValue(settingLevel, out pre);
                pre.UpdatedBy = CurrentUser.Id;
                _preferenceService.Save(pre);
            }
            return Json(new { Success = true, InforUpdated = GetInforUpdated() }, JsonRequestBehavior.AllowGet);
        }
        private Dictionary<int, Preferences> BuildPreferencesDictionary(string preferences, int districtId)
        {
            Dictionary<int, Preferences> preferenceLevels = new Dictionary<int, Preferences>();
            // Enterprise level
            preferenceLevels.Add((int)DataLockerPreferencesLevel.Enterprise, new Preferences { Value = preferences, Level = ContaintUtil.DataLockerPreferenceLevelEnterprise, Id = 0, Label = ContaintUtil.DataLockerPreferencesSetting });
            // District level
            preferenceLevels.Add((int)DataLockerPreferencesLevel.District, new Preferences { Value = preferences, Level = ContaintUtil.DataLockerPreferenceLevelDistrict, Id = districtId, Label = ContaintUtil.DataLockerPreferencesSetting });
            //School Level
            preferenceLevels.Add((int)DataLockerPreferencesLevel.School, new Preferences { Value = preferences, Level = ContaintUtil.DataLockerPreferenceLevelSchool, Id = districtId, Label = ContaintUtil.DataLockerPreferencesSetting });
            //Form
            preferenceLevels.Add((int)DataLockerPreferencesLevel.Form, new Preferences { Value = preferences, Level = ContaintUtil.DataLockerPreferenceLevelForm, Id = districtId, Label = ContaintUtil.DataLockerPreferencesSetting });
            return preferenceLevels;
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
