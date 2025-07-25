using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.TestPreferences;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize]
    [VersionFilter]
    public class SecuritySettingController : BaseController
    {
        private readonly ConfigurationService _configurationService;
        private readonly SecurityPreferencesService _preferenceService;
        private readonly StateService _stateService;
        private readonly UserService _userService;

        private Dictionary<int, string> levels = new Dictionary<int, string>()
        {
            { (int)SecurityPreferenceLevel.Enterprise, ContaintUtil.TestPreferenceLevelEnterprise },
            { (int)SecurityPreferenceLevel.District, ContaintUtil.TestPreferenceLevelDistrict },
            { (int)SecurityPreferenceLevel.User, ContaintUtil.TestPreferenceLevelUser }
        };

        public SecuritySettingController(ConfigurationService configurationService,
            SecurityPreferencesService preferenceService, StateService stateService,
            UserService userService)
        {
            _configurationService = configurationService;
            _preferenceService = preferenceService;
            _stateService = stateService;
            _userService = userService;
        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.DadSecuritySettings)]
        [HttpGet]
        public ActionResult Index()
        {
            var obj = new SecurityPreferenceViewModel()
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

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.DadSecuritySettings)]
        public ActionResult LoadSettings(int levelSetting, int districtId)
        {
            if (!Util.HasRightOnDistrict(CurrentUser, districtId))
            {
                return Json(new { error = "Has no right on district." }, JsonRequestBehavior.AllowGet);
            }

            var securitySettings = BindDataToModel(levelSetting, districtId) ?? new SecuritySettingsMap();

            ViewBag.DateFormat = _configurationService.GetConfigurationByKey(Constanst.JQueryDateFormat)?.Value;
            ViewBag.LastUpdatedDate = securitySettings.SecuritySettingModel.LastUpdatedDateString;
            ViewBag.CurrentLevelId = levelSetting;
            return PartialView("_SecuritySetting", securitySettings);
        }

        [NonAction]
        public SecuritySettingsMap BindDataToModel(int levelSetting, int districtId)
        {
            int currentDistrictId = districtId;
            int currentUserId = CurrentUser.Id;

            if (!CurrentUser.IsPublisher() && !CurrentUser.IsNetworkAdmin)
            {
                currentDistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;
            }
            var securityMap = new SecuritySettingsMap()
            {
                DistrictId = districtId,
                SettingLevelID = levelSetting
            };

            var preferences = _preferenceService.GetPreferenceInCurrentLevel(new GetPreferencesParams
            {
                CurrrentLevelId = levelSetting,
                DistrictId = districtId,
                UserId = CurrentUser.Id,
                UserRoleId = CurrentUser.RoleId,
                StateId = CurrentUser.StateId.GetValueOrDefault()
            });
            if (preferences != null)
            {
                securityMap.SecuritySettingModel = preferences;
                var levelName = string.Empty;
                levels.TryGetValue(levelSetting, out levelName);
                securityMap.SettingLevel = levelName;
            }

            return securityMap;
        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.DadSecuritySettings)]
        [HttpPost]
        [AjaxOnly]
        public ActionResult SaveSettings(SecuritySettingPreferenceModel obj, int settingLevel, int districtId)
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

            if (!CurrentUser.IsPublisher && settingLevel == (int)SecurityPreferenceLevel.Enterprise)
            {
                //not allow users that are not publisher set enterprise level preference
                return Json(new { Sucess = false });
            }
            if (CurrentUser.IsSchoolAdmin && settingLevel == (int)SecurityPreferenceLevel.District)
            {
                //not allow users that are not publisher set enterprise level preference
                return Json(new { Sucess = false });
            }

            if (CurrentUser.IsTeacher && settingLevel == (int)SecurityPreferenceLevel.District)
            {
                //not allow users that are not publisher set enterprise level preference
                return Json(new { Sucess = false });
            }

            if (obj != null && settingLevel > 0)
            {
                var pre = new Preferences();
                var preferences = _preferenceService.ConvertSecurityPreferenceModelToString(obj);
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
            preferenceLevels.Add((int)SecurityPreferenceLevel.Enterprise, new Preferences { Value = preferences, Level = ContaintUtil.TestPreferenceLevelEnterprise, Id = 0, Label = ContaintUtil.SecurityPreferencesSetting });
            // District level
            preferenceLevels.Add((int)SecurityPreferenceLevel.District, new Preferences { Value = preferences, Level = ContaintUtil.TestPreferenceLevelDistrict, Id = districtId, Label = ContaintUtil.SecurityPreferencesSetting });
            // User level
            preferenceLevels.Add((int)SecurityPreferenceLevel.User, new Preferences { Value = preferences, Level = ContaintUtil.TestPreferenceLevelUser, Id = CurrentUser.Id, Label = ContaintUtil.SecurityPreferencesSetting });

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
