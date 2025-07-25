using LinkIt.BubbleSheetPortal.Models.DTOs.TestPreferences;
using LinkIt.BubbleSheetPortal.Models.ManagePreference;
using LinkIt.BubbleSheetPortal.Models;
using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using Envoc.Core.Shared.Data;
using System.Linq;
using LinkIt.BubbleSheetPortal.Models.Constants;
using System.Xml;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class SecurityPreferencesService
    {
        private readonly IPreferencesRepository repository;
        private readonly IRepository<User> _userRepository;
        private readonly IStateRepository _stateRepository;

        private Dictionary<int, PreferencesConfig> levels = new Dictionary<int, PreferencesConfig>()
        {
            { (int)SecurityPreferenceLevel.Enterprise, new PreferencesConfig { Level = ContaintUtil.TestPreferenceLevelEnterprise, PropertyName = nameof(GetPreferencesParams.EnterpriseId) } },
            { (int)SecurityPreferenceLevel.District, new PreferencesConfig { Level = ContaintUtil.TestPreferenceLevelDistrict, PropertyName = nameof(GetPreferencesParams.DistrictId) } },
            { (int)SecurityPreferenceLevel.User, new PreferencesConfig  { Level = ContaintUtil.TestPreferenceLevelUser, PropertyName = nameof(GetPreferencesParams.UserId)} },
        };

        public SecurityPreferencesService(IPreferencesRepository repository,
            IRepository<User> userRepository,
            IStateRepository stateRepository)
        {
            this.repository = repository;
            _userRepository = userRepository;
            _stateRepository = stateRepository;
        }

        public SecuritySettingPreferenceModel GetPreferenceInCurrentLevel(GetPreferencesParams model)
        {
            var preferences = new List<SecuritySettingPreferenceModel>();

            for (int i = model.CurrrentLevelId; i >= (int)SecurityPreferenceLevel.Enterprise; i--)
            {
                var levelConfig = new PreferencesConfig();
                levels.TryGetValue(i, out levelConfig);
                if (model.CurrrentLevelId == (int)SecurityPreferenceLevel.User
                    && (model.UserRoleId == (int)Permissions.Publisher || model.UserRoleId == (int)Permissions.NetworkAdmin)
                    && levelConfig.Level == ContaintUtil.TestPreferenceLevelDistrict)
                    continue;
                var id = model.GetPropValue<int>(levelConfig.PropertyName);

                var preference = repository.Select().Where(o => o.Label == TextConstants.SECURITY_PREFERENCES_LABEL).FirstOrDefault(x => x.Level.Equals(levelConfig.Level) && x.Id == id);
                if (preference != null)
                {
                    var preferenceModel = ConvertToSecurityPreferenceModel(preference.Value);
                    preferenceModel.LevelId = i;
                    preferenceModel.LastUpdatedDate = preference.UpdatedDate;
                    preferenceModel.LastUpdatedBy = preference.UpdatedBy.GetValueOrDefault();
                    preferences.Add(preferenceModel);
                }
            }
            var finalPreference = CalculateFinalPreferences(preferences, model.UserRoleId, model.CurrrentLevelId);
            var lastedPreference = preferences.Select(o => new { o.LastUpdatedDate, o.LastUpdatedBy })
                                    .OrderByDescending(x => x.LastUpdatedDate)
                                    .FirstOrDefault();

            var timeZoneId = _stateRepository.GetTimeZoneId(model.StateId);
            finalPreference.LastUpdatedDate = (!string.IsNullOrEmpty(timeZoneId) && lastedPreference.LastUpdatedDate.HasValue) ? lastedPreference.LastUpdatedDate.Value.ConvertTimeFromUtc(timeZoneId) : lastedPreference.LastUpdatedDate;
            finalPreference.LastUpdatedByUser = GetUserNameById(lastedPreference.LastUpdatedBy);
            return finalPreference;
        }

        private SecuritySettingPreferenceModel CalculateFinalPreferences(List<SecuritySettingPreferenceModel> preferences, int userRoleId, int modelCurrentLevel)
        {
            var maxCurrentLevel = preferences.Select(v => v.LevelId).Distinct().OrderByDescending(x => x).FirstOrDefault();
            var options = CalculatedTags(preferences.SelectMany(o => o.OptionTags.Select(x => new Tag
            {
                LevelId = o.LevelId,
                Key = x.Key,
                Value = x.Value,
                Attributes = x.Attributes,
                SectionItems = x.SectionItems
            })).ToList(), maxCurrentLevel, userRoleId);

            if (modelCurrentLevel == (int)SecurityPreferenceLevel.User)
            {
                switch (userRoleId)
                {
                    case (int)Permissions.Publisher:
                        options = UpdateUserOption(options, TextConstants.ENABLE_MFA_EMAIL_PUBLISHER);
                        break;
                    case (int)Permissions.NetworkAdmin:
                        options = UpdateUserOption(options, TextConstants.ENABLE_MFA_EMAIL_NETWORKADMIN);
                        break;
                    case (int)Permissions.DistrictAdmin:
                        options = UpdateUserOption(options, TextConstants.ENABLE_MFA_EMAIL_DISTRICTADMIN);
                        break;
                    case (int)Permissions.SchoolAdmin:
                        options = UpdateUserOption(options, TextConstants.ENABLE_MFA_EMAIL_SCHOOLADMIN);
                        break;
                    case (int)Permissions.Teacher:
                        options = UpdateUserOption(options, TextConstants.ENABLE_MFA_EMAIL_TEACHER);
                        break;
                    case (int)Permissions.Student:
                        options = UpdateUserOption(options, TextConstants.ENABLE_MFA_EMAIL_STUDENT);
                        break;
                    case (int)Permissions.Parent:
                        options = UpdateUserOption(options, TextConstants.ENABLE_MFA_EMAIL_PARENT);
                        break;
                    default:
                        break;
                }
            }

            return new SecuritySettingPreferenceModel
            {
                OptionTags = options,
                LevelId = maxCurrentLevel
            };
        }

        private List<Tag> CalculatedTags(List<Tag> tags, int currentLevel, int userRoleId)
        {
            var calculatedTags = tags
            .GroupBy(g => g.Key)
            .Select(tagGrouped =>
            {
                var groupedTags = tagGrouped.OrderBy(x => x.LevelId).ToList();

                var tag = CalculatedTag(groupedTags, currentLevel, userRoleId);

                return tag;
            })
            .Where(x => x != null)
            .ToList();

            return calculatedTags;
        }

        private Tag CalculatedTag(List<Tag> tags, int currentLevel, int userRoleId)
        {
            var keyLocked = tags.FirstOrDefault(c => c.Attributes.FirstOrDefault(x => x?.Key.ToLower() == "lock")?.Value.ToLower() == "true");

            if (keyLocked != null)
            {
                return keyLocked;
            }

            var tag = tags.FirstOrDefault(x => x.LevelId == currentLevel);

            return tag != null ? tag : tags.Select(o => new Tag
            {
                Key = o.Key,
                Value = "0",
                LevelId = currentLevel,
                Attributes = new List<TagAttr>
                {
                    new TagAttr
                    {
                        Key = "lock",
                        Value = "false"
                    }
                }
            }).FirstOrDefault();
        }

        private string GetUserNameById(int userId)
        {
            var user = _userRepository.Select().FirstOrDefault(x => x.Id == userId);
            if (user == null)
                return string.Empty;

            return $"{user.LastName}, {user.FirstName}";
        }

        public SecuritySettingPreferenceModel ConvertToSecurityPreferenceModel(string preferenceValue)
        {
            if (string.IsNullOrWhiteSpace(preferenceValue)) return null;
            var result = new SecuritySettingPreferenceModel();
            result.OptionTags = GetTags(preferenceValue, "options");
            return result;
        }

        private List<Tag> GetTags(string value, string type)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            var doc = new XmlDocument();
            doc.LoadXml(value);
            var result = new List<Tag>();
            var xnList = doc.SelectNodes("/securitySettings/" + type);
            if (xnList == null) return null;
            foreach (XmlNode xn in xnList)
            {
                if (!xn.HasChildNodes) continue;
                foreach (XmlNode option in xn.ChildNodes)
                {
                    var tag = new Tag();
                    result.Add(tag);
                    tag.Key = option.Name;
                    tag.Value = option.InnerText;
                    tag.Attributes = new List<TagAttr>();
                    if (option.Attributes.Count > 0)
                    {
                        foreach (XmlAttribute attr in option.Attributes)
                        {
                            var optionAttr = new TagAttr { Key = attr.Name, Value = attr.Value };
                            tag.Attributes.Add(optionAttr);
                        }
                    }

                    if (option.ChildNodes.Count > 1)
                    {
                        foreach (XmlNode item in option.ChildNodes)
                        {
                            var sectionItem = new Tag();
                            sectionItem.Key = item.Name;
                            sectionItem.Value = item.InnerText;
                            sectionItem.Attributes = new List<TagAttr>();
                            if (item.Attributes == null || item.Attributes.Count == 0) continue;
                            foreach (XmlAttribute attr in item.Attributes)
                            {
                                var optionAttr = new TagAttr { Key = attr.Name, Value = attr.Value };
                                sectionItem.Attributes.Add(optionAttr);
                            }
                            tag.SectionItems.Add(sectionItem);
                        }
                    }
                }
            }

            return result;
        }

        public bool Save(Preferences obj)
        {
            if (obj != null)
            {
                var tmp = repository.Select().FirstOrDefault(o => o.Id == obj.Id && o.Level == obj.Level && o.Label == obj.Label);
                if (tmp != null)
                {
                    obj.PreferenceId = tmp.PreferenceId;
                }
                repository.Save(obj);
                return true;
            }
            return false;
        }

        public string ConvertSecurityPreferenceModelToString(SecuritySettingPreferenceModel model)
        {
            if (model == null) return null;
            var doc = new XmlDocument();

            XmlNode testSettingsNode = doc.CreateElement("securitySettings");
            doc.AppendChild(testSettingsNode);

            if (model.OptionTags != null)
            {
                XmlNode optionsNode = doc.CreateElement("options");
                testSettingsNode.AppendChild(optionsNode);
                foreach (var tag in model.OptionTags)
                {
                    XmlNode optionNode = doc.CreateElement(tag.Key);
                    optionNode.InnerText = tag.Value;
                    if (tag.SectionItems.Count > 0)
                    {
                        foreach (var sectionItem in tag.SectionItems)
                        {
                            XmlNode sectionNode = doc.CreateElement(sectionItem.Key);
                            sectionNode.InnerText = sectionItem.Value;
                            if (sectionItem.Attributes != null)
                            {
                                foreach (var attribute in sectionItem.Attributes)
                                {
                                    XmlAttribute attr = doc.CreateAttribute(attribute.Key);
                                    attr.Value = attribute.Value;
                                    sectionNode.Attributes.Append(attr);
                                }
                            }
                            optionNode.AppendChild(sectionNode);
                        }
                    }
                    optionsNode.AppendChild(optionNode);
                    if (tag.Attributes == null) continue;
                    foreach (var attribute in tag.Attributes)
                    {
                        XmlAttribute attr = doc.CreateAttribute(attribute.Key);
                        attr.Value = attribute.Value;
                        optionNode.Attributes.Append(attr);
                    }
                }
            }

            var result = doc.OuterXml;
            return result;
        }

        private List<Tag> UpdateUserOption(List<Tag> options, string key)
        {
            var optionUser = options.Find(x => x.Key == TextConstants.ENABLE_MFA_EMAIL_USER);
            var optionFiltered = options.Find(x => x.Key.ToLower() == key.ToLower()); //teacher
            if (optionUser.LevelId != (int)SecurityPreferenceLevel.User
                || (optionUser.LevelId == (int)SecurityPreferenceLevel.User && optionFiltered.Locked))
            {
                optionUser.Attributes = optionFiltered.Attributes;
                optionUser.Value = optionFiltered.Value;
                optionUser.LevelId = optionFiltered.LevelId;
            }
            return options;
        }

    }
}
