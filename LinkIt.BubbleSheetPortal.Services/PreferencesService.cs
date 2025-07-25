using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.ManagePreference;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models.Constants;
using LinkIt.BubbleSheetPortal.Models.DTOs.TestPreferences;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models.Old.ManagePreference;
using System;
using LinkIt.BubbleSheetPortal.Models.DataLockerPreferencesSetting;
using Newtonsoft.Json;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class PreferencesService
    {
        private readonly IPreferencesRepository repository;
        private readonly IRepository<User> _userRepository;
        private readonly IStateRepository _stateRepository;
        private readonly IReadOnlyRepository<LocalizeResource> _localizeResourceRepository;

        private Dictionary<int, PreferencesConfig> levels = new Dictionary<int, PreferencesConfig>()
        {
            { (int)TestPreferenceLevel.Enterprise, new PreferencesConfig { Level = ContaintUtil.TestPreferenceLevelEnterprise, PropertyName = nameof(GetPreferencesParams.EnterpriseId) } },
            { (int)TestPreferenceLevel.SurveyEnterprise, new PreferencesConfig { Level = ContaintUtil.TestPreferenceLevelEnterprise, PropertyName = nameof(GetPreferencesParams.EnterpriseId) } },
            { (int)TestPreferenceLevel.District, new PreferencesConfig { Level = ContaintUtil.TestPreferenceLevelDistrict, PropertyName = nameof(GetPreferencesParams.DistrictId) } },
            { (int)TestPreferenceLevel.SurveyDistrict, new PreferencesConfig { Level = ContaintUtil.TestPreferenceLevelDistrict, PropertyName = nameof(GetPreferencesParams.DistrictId) } },
            { (int)TestPreferenceLevel.School, new PreferencesConfig { Level = ContaintUtil.TestPreferenceLevelSchool, PropertyName = nameof(GetPreferencesParams.SchoolId) } },
            { (int)TestPreferenceLevel.User, new PreferencesConfig  { Level = ContaintUtil.TestPreferenceLevelUser, PropertyName = nameof(GetPreferencesParams.UserId)} },
            { (int)TestPreferenceLevel.TestDesign, new PreferencesConfig { Level = ContaintUtil.TestPreferenceLevelTest, PropertyName= nameof(GetPreferencesParams.VirtualTestId) } },
            { (int)TestPreferenceLevel.TestAssignment, new PreferencesConfig { Level = ContaintUtil.TestPreferenceLevelTestAssignment, PropertyName = nameof(GetPreferencesParams.TestAssignmentId) } },
        };

        private Dictionary<int, PreferencesConfig> dllevels = new Dictionary<int, PreferencesConfig>()
        {
            { (int)DataLockerPreferencesLevel.Enterprise, new PreferencesConfig { Level = ContaintUtil.DataLockerPreferenceLevelEnterprise, PropertyName = nameof(GetPreferencesParams.EnterpriseId) } },
            { (int)DataLockerPreferencesLevel.District, new PreferencesConfig { Level = ContaintUtil.DataLockerPreferenceLevelDistrict, PropertyName = nameof(GetPreferencesParams.DistrictId) } },
            { (int)DataLockerPreferencesLevel.School, new PreferencesConfig  { Level = ContaintUtil.DataLockerPreferenceLevelSchool, PropertyName = nameof(GetPreferencesParams.SchoolId)} },
            { (int)DataLockerPreferencesLevel.Form, new PreferencesConfig { Level = ContaintUtil.DataLockerPreferenceLevelForm, PropertyName = nameof(GetPreferencesParams.VirtualTestId) } },
            { (int)DataLockerPreferencesLevel.Publishing, new PreferencesConfig  { Level = ContaintUtil.DataLockerPreferenceLevelPublish, PropertyName = nameof(GetPreferencesParams.TestAssignmentId)} },
        };

        private string[] optionsException = new[] { "testScheduleFromDate", "testScheduleToDate", "testScheduleTimezoneOffset", "testScheduleDayOfWeek", "testScheduleStartTime", "testScheduleEndTime", "deadline" };
        private string[] _inheritPreferenceKeys = new string[] { "overrideAutoGradedTextEntry" };

        private List<TagHasDependent> optionsIndependent = new List<TagHasDependent>
        {
            new TagHasDependent{ Key = "timeLimit", ChildTag = "duration" },
            new TagHasDependent{ Key = "timeLimit", ChildTag = "deadline" },
            new TagHasDependent{ Key = "timeLimit", ChildTag = "timeLimitDurationType" },
            new TagHasDependent{ Key = "testSchedule", ChildTag = "testScheduleTimezoneOffset" },
            new TagHasDependent{ Key = "testSchedule", ChildTag = "testScheduleDayOfWeek" },
            new TagHasDependent{ Key = "testSchedule", ChildTag = "testScheduleFromDate" },
            new TagHasDependent{ Key = "testSchedule", ChildTag = "testScheduleToDate" },
            new TagHasDependent{ Key = "testSchedule", ChildTag = "testScheduleStartTime" },
            new TagHasDependent{ Key = "testSchedule", ChildTag = "testScheduleEndTime" },
            new TagHasDependent{ Key = "displayTexttospeechOption", ChildTag = "rate" },
            new TagHasDependent{ Key = "displayTexttospeechOption", ChildTag = "volume" }
        };

        public PreferencesService(IPreferencesRepository repository,
            IRepository<User> userRepository,
            IStateRepository stateRepository,
            IReadOnlyRepository<LocalizeResource> localizeResourceRepository)
        {
            this.repository = repository;
            _userRepository = userRepository;
            _stateRepository = stateRepository;
            _localizeResourceRepository = localizeResourceRepository;
        }

        public Preferences GetPreferencesById(int preferenceId)
        {
            return Select().FirstOrDefault(x => x.PreferenceId.Equals(preferenceId));
        }

        public Preferences GetPreferenceByLevelAndId(int userId, int districtId, string level, bool isSurvey = false)
        {
            var result = new Preferences();

            switch (level)
            {
                case ContaintUtil.TestPreferenceLevelEnterprise:
                    {
                        if (isSurvey)
                            result = repository.Select().FirstOrDefault(x => x.Id == 0 && x.Level.Equals(level) && x.Label == TextConstants.SURVEY_PREFERENCES_LABEL);
                        else
                            result = Select().FirstOrDefault(x => x.Id == 0 && x.Level.Equals(level));
                    }
                    break;
                case ContaintUtil.TestPreferenceLevelDistrict:
                    {
                        if (isSurvey)
                            result = repository.Select().FirstOrDefault(x => x.Id == districtId && x.Level.Equals(level) && x.Label == TextConstants.SURVEY_PREFERENCES_LABEL) ??
                                 repository.Select().FirstOrDefault(x => x.Id == 0 && x.Level.Equals(ContaintUtil.TestPreferenceLevelEnterprise) && x.Label == TextConstants.SURVEY_PREFERENCES_LABEL);
                        else
                            result = Select().FirstOrDefault(x => x.Id == districtId && x.Level.Equals(level)) ??
                                 Select().FirstOrDefault(x => x.Id == 0 && x.Level.Equals(ContaintUtil.TestPreferenceLevelEnterprise));
                    }
                    break;
                case ContaintUtil.TestPreferenceLevelUser:
                    {
                        if (isSurvey)
                            return null;
                        else
                            result = (Select().FirstOrDefault(x => x.Id == userId && x.Level.Equals(level)) ??
                                  Select().FirstOrDefault(x => x.Id == districtId && x.Level.Equals(ContaintUtil.TestPreferenceLevelDistrict))) ??
                                 Select().FirstOrDefault(x => x.Id == 0 && x.Level.Equals(ContaintUtil.TestPreferenceLevelEnterprise));
                    }
                    break;
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

        public bool SaveAssignment(Preferences obj)
        {
            if (obj != null)
            {
                var tmp = new Preferences();
                tmp.Id = obj.Id;
                tmp.Label = obj.Label;
                tmp.Level = obj.Level;
                tmp.Value = obj.Value;
                tmp.UpdatedBy = obj.UpdatedBy;
                repository.Save(tmp);
                return true;
            }
            return false;
        }

        public void Delete(Preferences obj)
        {
            repository.Delete(obj);
        }

        public Preferences GetPreferenceByAssignmentLeveAndID(int id)
        {
            return Select().FirstOrDefault(o => o.Id == id && o.Level.Equals(ContaintUtil.TestPreferenceLevelTestAssignment) && o.Label == ContaintUtil.TestPreferenceLabelTest);
        }

        public Preferences GetPreferenceByLevelAndID(int id, string level)
        {
            return Select().FirstOrDefault(o => o.Id == id && o.Level.Equals(level.Trim()) && o.Label == ContaintUtil.TestPreferenceLabelTest);
        }


        public Preferences GetPreferenceDataLockerPortalByLevelAndID(int id, string level)
        {
            return repository.Select().FirstOrDefault(o => o.Id == id && o.Level.Equals(level.Trim()) && o.Label == ContaintUtil.DataLockerPreferencesSetting);
        }

        public Preferences GetPreferenceByLabelAndId(int id, string label)
        {
            return repository.Select().Where(o => o.Id == id && o.Label == label).FirstOrDefault();
        }

        public bool IsAnonymizedScoring(int id)
        {
            var preferences = GetPreferenceByAssignmentLeveAndID(id);
            return preferences?.Value?.ToLower().Contains("<anonymizedscoring>1</anonymizedscoring>") ?? false;
        }

        public TestPreferenceModel ConvertToTestPreferenceModel(string preferenceValue)
        {
            if (string.IsNullOrWhiteSpace(preferenceValue)) return null;
            var result = new TestPreferenceModel();
            result.OptionTags = GetTags(preferenceValue, "options");
            result.ToolTags = GetTags(preferenceValue, "tools");
            return result;
        }
        public DataLockerPreferencesSettingModal ConvertToDataLockerPreferencesSettingModal(string preferenceValue)
        {
            if (string.IsNullOrWhiteSpace(preferenceValue)) return null;
            var result = new DataLockerPreferencesSettingModal();
            result.DataSettings = JsonConvert.DeserializeObject<DataSettings>(preferenceValue);
            return result;
        }

        private List<Tag> GetTags(string value, string type)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            var doc = new XmlDocument();
            doc.LoadXml(value);
            var result = new List<Tag>();
            var xnList = doc.SelectNodes("/testSettings/" + type);
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

        public string ConvertTestPreferenceModelToString(TestPreferenceModel model)
        {
            if (model == null) return null;
            var doc = new XmlDocument();

            XmlNode testSettingsNode = doc.CreateElement("testSettings");
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


            if (model.ToolTags != null)
            {
                XmlNode toolsNode = doc.CreateElement("tools");
                testSettingsNode.AppendChild(toolsNode);
                foreach (var tag in model.ToolTags)
                {
                    XmlNode toolNode = doc.CreateElement(tag.Key);
                    toolNode.InnerText = tag.Value;
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
                            toolNode.AppendChild(sectionNode);
                        }
                    }
                    toolsNode.AppendChild(toolNode);

                    if (tag.Attributes == null) continue;
                    foreach (var attribute in tag.Attributes)
                    {
                        XmlAttribute attr = doc.CreateAttribute(attribute.Key);
                        attr.Value = attribute.Value;
                        toolNode.Attributes.Append(attr);
                    }
                }
            }

            var result = doc.OuterXml;
            return result;
        }

        public TestPreferenceModel ClearAllAtribute(TestPreferenceModel obj)
        {
            if (obj != null)
            {
                var testPreferenceModel = new TestPreferenceModel();
                if (obj.OptionTags.Count > 0)
                {
                    foreach (var optionTag in obj.OptionTags)
                    {
                        testPreferenceModel.OptionTags.Add(new Tag()
                        {
                            Key = optionTag.Key,
                            Value = optionTag.Value,
                            Attributes = optionTag.Attributes.Where(x => x.Key != "lock").Select(o => new TagAttr
                            {
                                Key = o.Key,
                                Value = o.Value
                            }).ToList(),
                            SectionItems = optionTag.SectionItems.Select(o => new Tag
                            {
                                Key = o.Key,
                                Value = o.Value,
                                Attributes = o.Attributes.Where(x => x.Key != "lock").Select(y => new TagAttr
                                {
                                    Key = y.Key,
                                    Value = y.Value
                                }).ToList()
                            }).ToList()
                        });
                    }
                }
                if (obj.ToolTags.Count > 0)
                {
                    foreach (var toolTag in obj.ToolTags)
                    {
                        testPreferenceModel.ToolTags.Add(new Tag()
                        {
                            Key = toolTag.Key,
                            Value = toolTag.Value,
                            Attributes = toolTag.Attributes.Where(x => x.Key != "lock").Select(o => new TagAttr
                            {
                                Key = o.Key,
                                Value = o.Value
                            }).ToList(),
                            SectionItems = toolTag.SectionItems.Select(o => new Tag
                            {
                                Key = o.Key,
                                Value = o.Value,
                                Attributes = o.Attributes.Where(x => x.Key != "lock").Select(y => new TagAttr
                                {
                                    Key = y.Key,
                                    Value = y.Value
                                }).ToList()
                            }).ToList()
                        });
                    }
                }
                return testPreferenceModel;
            }
            return null;
        }

        public void OffSupportHighLightText(TestPreferenceModel obj)
        {
            if (obj != null)
            {
                if (obj.OptionTags.Count > 0)
                {
                    foreach (var optionTag in obj.OptionTags)
                    {
                        if (optionTag.Key.ToLower().Equals("supporthighlighttext"))
                        {
                            optionTag.Value = "0";
                        }
                    }
                }
            }
        }

        public void OffRequireTestTakerAuthentication(TestPreferenceModel obj)
        {
            if (obj != null)
            {
                if (obj.OptionTags.Count > 0)
                {
                    foreach (var optionTag in obj.OptionTags)
                    {
                        if (optionTag.Key.Equals(Constanst.REQUIRE_TEST_TAKER_AUTHENTICATION, StringComparison.OrdinalIgnoreCase))
                        {
                            optionTag.Value = "0";
                        }
                    }
                }
            }
        }

        private IQueryable<Preferences> Select(bool isSurvey = false)
        {
            if (isSurvey)
                return repository.Select().Where(o => o.Label == TextConstants.SURVEY_PREFERENCES_LABEL);
            else
                return repository.Select().Where(o => o.Label == TextConstants.TEST_PREFERENCES_LABEL);
        }

        /// <summary>
        /// Addjust test schedule end date
        /// Author: Hiep Bui
        /// Date: 01/13/2018
        /// </summary>
        /// <param name="testAssignmentId"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public bool AdjustEndDateOfTestPreferencesSchedule(int testAssignmentId, string endDate, int offset)
        {
            var preference = repository.Select().FirstOrDefault(o => o.Level == Constanst.PreferenceTypeTestAssignment && o.Id == testAssignmentId);

            if (preference != null && !string.IsNullOrEmpty(preference.Value))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(preference.Value);
                XmlNodeList nodes = doc.GetElementsByTagName("testScheduleToDate");

                if (nodes.Count > 0)
                {
                    nodes[0].InnerText = endDate;

                }
                else
                {
                    XmlNodeList options = doc.GetElementsByTagName("options");

                    if (options.Count > 0)
                    {
                        XmlNode newNode = doc.CreateNode(XmlNodeType.Element, "testScheduleToDate", endDate);
                        options[0].AppendChild(newNode);
                    }

                }

                XmlNodeList offsetNodes = doc.GetElementsByTagName("testScheduleTimezoneOffset");

                if (offsetNodes.Count > 0)
                {
                    offsetNodes[0].InnerText = offset.ToString();
                }
                else
                {
                    XmlNodeList options = doc.GetElementsByTagName("options");

                    if (options.Count > 0)
                    {
                        XmlNode newNode = doc.CreateNode(XmlNodeType.Element, "testScheduleTimezoneOffset", offset.ToString());
                        options[0].AppendChild(newNode);
                    }
                }

                preference.Value = doc.OuterXml;
                repository.Save(preference);
                return true;
            }

            return false;
        }

        public void UpdateNodeByTag(XmlDocument doc, XmlNodeList options, Tag tag)
        {
            XmlNode tagNode = doc.GetElementsByTagName(tag.Key)?[0];
            if (tagNode != null)
                tagNode.InnerText = tag.Value;
            else
            {
                XmlNode newNode = doc.CreateNode(XmlNodeType.Element, tag.Key, tag.Value);
                newNode.InnerText = tag.Value;
                options[0].AppendChild(newNode);
            }
        }
        public void UpdateNodeByTag_FullOption(XmlDocument doc, XmlNodeList optionsNode, Tag tag)
        {
            XmlNode currentNode = doc.GetElementsByTagName(tag.Key)?[0];
            if (currentNode != null)
            {
                currentNode.RemoveAll();
                currentNode.InnerText = tag.Value;
            }
            else
            {
                currentNode = doc.CreateNode(XmlNodeType.Element, tag.Key, tag.Value);
                optionsNode[0].AppendChild(currentNode);
            }


            if (tag.Attributes != null)
                foreach (var attribute in tag.Attributes)
                {
                    var newAttribute = doc.CreateAttribute(attribute.Key);
                    newAttribute.Value = attribute.Value;
                    currentNode.Attributes.Append(newAttribute);
                }
            if (tag.SectionItems != null)
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
                    currentNode.AppendChild(sectionNode);
                }
            }

        }
        public UpdateTestPreferencesResult UpdatePartOfPreference(int testAssignmentId, List<Tag> tagsToBeUpdated)
        {
            var preference = repository.Select().FirstOrDefault(o => o.Level == Constanst.PreferenceTypeTestAssignment && o.Id == testAssignmentId);

            if (preference != null && !string.IsNullOrEmpty(preference.Value))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(preference.Value);
                XmlNodeList options = doc.GetElementsByTagName("options");
                if (options.Count > 0)
                {
                    foreach (Tag tag in tagsToBeUpdated)
                    {
                        UpdateNodeByTag_FullOption(doc, options, tag);
                    }
                }
                preference.Value = doc.OuterXml;
                repository.Save(preference);
                return new UpdateTestPreferencesResult { Status = true, UpdatedDate = preference.UpdatedDate };
            }
            return new UpdateTestPreferencesResult { Status = false, UpdatedDate = DateTime.MinValue };
        }
        public UpdateTestPreferencesResult UpdateOptionTags(int testAssignmentId, List<Tag> tags)
        {
            var preference = repository.Select().FirstOrDefault(o => o.Level == Constanst.PreferenceTypeTestAssignment && o.Id == testAssignmentId);

            if (preference != null && !string.IsNullOrEmpty(preference.Value))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(preference.Value);
                XmlNodeList options = doc.GetElementsByTagName("options");
                if (options.Count > 0)
                {
                    foreach (Tag tag in tags)
                    {
                        UpdateNodeByTag(doc, options, tag);
                    }
                }
                preference.Value = doc.OuterXml;
                repository.Save(preference);
                return new UpdateTestPreferencesResult { Status = true, UpdatedDate = preference.UpdatedDate.Value };
            }
            return new UpdateTestPreferencesResult { Status = false, UpdatedDate = DateTime.MinValue };
        }

        public Preferences GetPreferenceByLevelLabelAndId(string strlevel, string strlabel, int id)
        {
            return repository.Select().FirstOrDefault(o => o.Level.Equals(strlevel.Trim()) && o.Label.Equals(strlabel.Trim()) && o.Id == id);
        }

        public Preferences GetPreferenceEntryResultForm(int virtualTestId, int districtId, int stateId, int schoolId = 0)
        {
            var preferences = repository.Select().FirstOrDefault(o => o.Level.Equals(ContaintUtil.DataLockerPreferenceLevelForm) && o.Label.Equals(ContaintUtil.DataLockerPreferencesSetting) && o.Id == virtualTestId);
            var enterPrisePrefer = repository.Select().FirstOrDefault(o => o.Level.Equals(ContaintUtil.DataLockerPreferenceLevelEnterprise) && o.Label.Equals(ContaintUtil.DataLockerPreferencesSetting));
            var districtPrefer = repository.Select().FirstOrDefault(o => o.Level.Equals(ContaintUtil.DataLockerPreferenceLevelDistrict) && o.Label.Equals(ContaintUtil.DataLockerPreferencesSetting) && o.Id == districtId);
            var schoolPrefer = repository.Select().FirstOrDefault(o => o.Level.Equals(ContaintUtil.DataLockerPreferenceLevelSchool) && o.Label.Equals(ContaintUtil.DataLockerPreferencesSetting) && o.Id == schoolId);

            if (preferences == null && schoolId > 0)
            {
                preferences = schoolPrefer;
            }

            if (preferences == null && districtId != 0)
            {
                preferences = districtPrefer;
            }

            if (districtId == 0 || preferences == null)
            {
                preferences = enterPrisePrefer;
            }

            var timeZoneId = _stateRepository.GetTimeZoneId(stateId);
            if (preferences != null)
            {
                preferences.UpdatedDate = (!string.IsNullOrEmpty(timeZoneId) && preferences.UpdatedDate.HasValue) ? preferences.UpdatedDate.Value.ConvertTimeFromUtc(timeZoneId) : preferences.UpdatedDate;
                if (preferences.Level != ContaintUtil.DataLockerPreferenceLevelEnterprise && enterPrisePrefer != null && districtPrefer != null)
                {
                    var currentPreferences = new DataLockerPreferencesSettingModal { DataSettings = JsonConvert.DeserializeObject<DataSettings>(preferences.Value) };
                    var enterPrisePreferences = new DataLockerPreferencesSettingModal { DataSettings = JsonConvert.DeserializeObject<DataSettings>(enterPrisePrefer.Value) };
                    var parentPreferences = new DataLockerPreferencesSettingModal { DataSettings = JsonConvert.DeserializeObject<DataSettings>(districtPrefer.Value) };
                    var settingConfigs = AssignLocktoChildConfiguration(currentPreferences, parentPreferences, enterPrisePreferences);
                    preferences.Value = JsonConvert.SerializeObject(settingConfigs.ChildSetting);
                }
            }
            return preferences;
        }

        private bool IsValidToGetPreference(bool isSurvey, int level)
        {
            return (isSurvey && (level == (int)TestPreferenceLevel.SurveyDistrict || level == (int)TestPreferenceLevel.SurveyEnterprise))
                || (!isSurvey && level != (int)TestPreferenceLevel.SurveyDistrict && level != (int)TestPreferenceLevel.SurveyEnterprise);
        }

        public TestPreferenceModel GetPreferenceInCurrentLevel(GetPreferencesParams model)
        {
            var preferences = new List<TestPreferenceModel>();

            for (int i = model.CurrrentLevelId; i >= (int)TestPreferenceLevel.Enterprise; i--)
            {
                if (model.IsFromOnlineTestingPreferences && !IsValidToGetPreference(model.IsSurvey, i))
                {
                    continue;
                }

                var levelConfig = new PreferencesConfig();
                levels.TryGetValue(i, out levelConfig);
                var id = model.GetPropValue<int>(levelConfig.PropertyName);

                var preference = Select(model.IsSurvey).FirstOrDefault(x => x.Level.Equals(levelConfig.Level) && x.Id == id);
                if (preference != null)
                {
                    var preferenceModel = ConvertToTestPreferenceModel(preference.Value);
                    preferenceModel.LevelId = i;
                    preferenceModel.LastUpdatedDate = preference.UpdatedDate;
                    preferenceModel.LastUpdatedBy = preference.UpdatedBy.GetValueOrDefault();
                    preferences.Add(preferenceModel);
                }
            }
            var finalPreference = CalculateFinalPreferences(preferences, model.UserRoleId);
            var lastedPreference = preferences.Select(o => new { o.LastUpdatedDate, o.LastUpdatedBy })
                                    .OrderByDescending(x => x.LastUpdatedDate)
                                    .FirstOrDefault();

            var timeZoneId = _stateRepository.GetTimeZoneId(model.StateId);
            finalPreference.LastUpdatedDate = (!string.IsNullOrEmpty(timeZoneId) && lastedPreference.LastUpdatedDate.HasValue) ? lastedPreference.LastUpdatedDate.Value.ConvertTimeFromUtc(timeZoneId) : lastedPreference.LastUpdatedDate;
            finalPreference.LastUpdatedByUser = GetUserNameById(lastedPreference.LastUpdatedBy);
            return finalPreference;
        }

        public DataLockerPreferencesSettingModal GetDataLockerPreferencesLevel(GetPreferencesParams model)
        {
            var preferences = new List<DataLockerPreferencesSettingModal>();
            var data = repository.Select().Where(o => o.Label == ContaintUtil.DataLockerPreferencesSetting);
            for (int i = model.CurrrentLevelId; i >= (int)DataLockerPreferencesLevel.Enterprise; i--)
            {
                var levelConfig = new PreferencesConfig();
                dllevels.TryGetValue(i, out levelConfig);
                var id = model.GetPropValue<int>(levelConfig.PropertyName);
                var preference = data?.FirstOrDefault(x => x.Level.Equals(levelConfig.Level) && x.Id == id);
                if (preference != null)
                {
                    var preferenceModel = ConvertToDataLockerPreferencesSettingModal(preference.Value);
                    if (preferenceModel != null)
                    {
                        preferenceModel.LevelId = i;
                        preferenceModel.LastUpdatedDate = preference.UpdatedDate;
                        preferenceModel.LastUpdatedBy = preference.UpdatedBy.GetValueOrDefault();
                        preferenceModel.LastUpdatedByUser = GetUserNameById(preferenceModel.LastUpdatedBy);
                        preferences.Add(preferenceModel);
                    }
                }
            }
            var finalPreference = CalculateFinalDataLockerPreferences(preferences, model.CurrrentLevelId);
            var lastedPreference = preferences.Select(o => new { o.LastUpdatedDate, o.LastUpdatedBy })
                                   .OrderByDescending(x => x.LastUpdatedDate)
                                   .FirstOrDefault();

            var timeZoneId = _stateRepository.GetTimeZoneId(model.StateId);
            finalPreference.LastUpdatedDate = (!string.IsNullOrEmpty(timeZoneId) && lastedPreference.LastUpdatedDate.HasValue) ? lastedPreference.LastUpdatedDate.Value.ConvertTimeFromUtc(timeZoneId) : lastedPreference.LastUpdatedDate;
            finalPreference.LastUpdatedByUser = GetUserNameById(lastedPreference.LastUpdatedBy);
            return finalPreference;
        }

        private TestPreferenceModel CalculateFinalPreferences(List<TestPreferenceModel> preferences, int userRoleId)
        {
            var currentLevel = preferences.Select(v => v.LevelId).Distinct().OrderByDescending(x => x).FirstOrDefault();
            var options = CalculatedTags(preferences.SelectMany(o => o.OptionTags.Select(x => new Tag
            {
                LevelId = o.LevelId,
                Key = x.Key,
                Value = x.Value,
                Attributes = x.Attributes,
                SectionItems = x.SectionItems
            })).ToList(), currentLevel, userRoleId);

            var tools = CalculatedTags(preferences.SelectMany(o => o.ToolTags.Select(x => new Tag
            {
                LevelId = o.LevelId,
                Key = x.Key,
                Value = x.Value,
                Attributes = x.Attributes,
                SectionItems = x.SectionItems
            })).ToList(), currentLevel, userRoleId);

            return new TestPreferenceModel
            {
                OptionTags = options,
                ToolTags = tools,
                LevelId = currentLevel
            };
        }

        private List<Tag> CalculatedTags(List<Tag> tags, int currentLevel, int userRoleId)
        {
            var calculatedTags = tags
            .GroupBy(g => g.Key)
            .Select(tagGrouped =>
            {
                var groupedTags = tagGrouped.OrderBy(x => x.LevelId).ToList();

                if (optionsIndependent.Any(x => x.ChildTag == tagGrouped.First().Key))
                {
                    var levelId = optionsIndependent.First(x => x.ChildTag == tagGrouped.First().Key).CurrentLevelId;
                    var currentLevelId = levelId > 0 ? levelId : currentLevel;
                    return CalculatedTag(groupedTags, currentLevelId, userRoleId);
                }
                else
                {
                    var tag = CalculatedTag(groupedTags, currentLevel, userRoleId);
                    var isLocked = tag.Attributes.FirstOrDefault(x => x?.Key.ToLower() == "lock")?.Value.ToLower() == "true";
                    var optionIndependent = optionsIndependent.Where(x => x.Key == tag.Key);
                    if (optionIndependent != null && isLocked)
                        optionIndependent.Select(o => { o.CurrentLevelId = tag.LevelId; return o; }).ToList();

                    return tag;
                }
            })
            .Where(x => x != null)
            .ToList();

            return calculatedTags;
        }

        private Tag CalculatedTag(List<Tag> tags, int currentLevel, int userRoleId)
        {
            var isInheritValue = _inheritPreferenceKeys.Contains(tags.First().Key) &&
                (userRoleId == (int)Permissions.SchoolAdmin || userRoleId == (int)Permissions.Teacher) && currentLevel > (int)TestPreferenceLevel.District;
            var inheritLevel = tags.Select(v => v.LevelId)
                .Where(x => x <= (int)TestPreferenceLevel.District)
                .OrderByDescending(x => x).FirstOrDefault();
            var levelGetValue = isInheritValue ? inheritLevel : currentLevel;

            if (tags.Count == 1)
            {
                var tagSection = tags.First();
                if (tagSection.LevelId == (int)TestPreferenceLevel.TestDesign
                    && (tagSection.Key == "sectionAvailability" || tagSection.Key.Contains("SectionItems")))
                    return tagSection;
            }
            Tag keyLocked = null;

            if (isInheritValue)
            {
                keyLocked = tags
                    .OrderByDescending(x => x.LevelId)
                    .FirstOrDefault(c => c.Attributes.FirstOrDefault(x => x?.Key.ToLower() == "lock")?.Value.ToLower() == "true" && c.LevelId <= (int)TestPreferenceLevel.District);
            }
            else
            {
                keyLocked = tags.FirstOrDefault(c => c.Attributes.FirstOrDefault(x => x?.Key.ToLower() == "lock")?.Value.ToLower() == "true");
            }

            if (keyLocked != null)
            {
                if (keyLocked.LevelId == (int)TestPreferenceLevel.User)
                    keyLocked.Attributes.Select(x =>
                    {
                        if (x.Key.ToLower() == "lock")
                            x.Value = "false";
                        return x;
                    }).ToList();

                return keyLocked;
            }

            var tag = tags.FirstOrDefault(x => x.LevelId == levelGetValue);

            return tag != null ? tag : tags.Select(o => new Tag
            {
                Key = o.Key,
                Value = !optionsException.Any(x => x == o.Key) ? "0" : o.Value,
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
        private DataLockerPreferencesSettingModal CalculateFinalDataLockerPreferences(List<DataLockerPreferencesSettingModal> preferences, int currrentLevelId)
        {
            var currentPreferences = preferences.FirstOrDefault(x => x.LevelId == currrentLevelId);
            var enterPrisePreferences = preferences.FirstOrDefault(x => x.LevelId == (int)DataLockerPreferencesLevel.Enterprise);
            if (currentPreferences != null)
            {                         
                var currentLevelId = currentPreferences.LevelId;               
                for (int i = currentLevelId - 1; i >= (int)TestPreferenceLevel.Enterprise; i--)
                {
                    var parentPreferences = preferences.FirstOrDefault(x => x.LevelId == i);
                    if (parentPreferences != null)
                    {
                        var settingConfigs = AssignLocktoChildConfiguration(currentPreferences, parentPreferences, enterPrisePreferences);
                        currentPreferences.DataSettings = settingConfigs.ChildSetting;
                        currentPreferences.ParentDataSettings = settingConfigs.ParentSetting;
                        break;
                    }
                }
                return currentPreferences;

            }
            else
            {
                var result = preferences.FirstOrDefault();
                if (result != null)
                {
                    var settingConfigs = AssignLocktoChildConfiguration(result, result, enterPrisePreferences);
                    result.ParentDataSettings = settingConfigs.ChildSetting;
                }
                return result;
            }
        }
        private (DataSettings ParentSetting, DataSettings ChildSetting) AssignLocktoChildConfiguration(DataLockerPreferencesSettingModal currentPreferences, DataLockerPreferencesSettingModal parentPreferences, DataLockerPreferencesSettingModal enterPrisePreferences)
        {
            var settingParentConfig = parentPreferences.DataSettings;
            var settingChildConfig = currentPreferences.DataSettings;
            var enterPriseConfig = enterPrisePreferences.DataSettings;
            if (enterPriseConfig.PublishingToStudentPortal.Lock || settingParentConfig.PublishingToStudentPortal.Lock)
            {
                settingChildConfig.PublishingToStudentPortal.Lock = enterPriseConfig.PublishingToStudentPortal.Lock ? enterPriseConfig.PublishingToStudentPortal.Lock : settingParentConfig.PublishingToStudentPortal.Lock;
                settingChildConfig.PublishingToStudentPortal.AllowPublishing = enterPriseConfig.PublishingToStudentPortal.Lock ? enterPriseConfig.PublishingToStudentPortal.AllowPublishing : settingParentConfig.PublishingToStudentPortal.AllowPublishing;
                settingParentConfig.PublishingToStudentPortal.Lock = enterPriseConfig.PublishingToStudentPortal.Lock ? enterPriseConfig.PublishingToStudentPortal.Lock : settingParentConfig.PublishingToStudentPortal.Lock;
                settingParentConfig.PublishingToStudentPortal.AllowPublishing = enterPriseConfig.PublishingToStudentPortal.Lock ? enterPriseConfig.PublishingToStudentPortal.AllowPublishing : settingParentConfig.PublishingToStudentPortal.AllowPublishing;
            }
            if(enterPriseConfig.ExpriedOn.Lock || settingParentConfig.ExpriedOn.Lock)
            {
                settingChildConfig.ExpriedOn.Lock = enterPriseConfig.ExpriedOn.Lock ? enterPriseConfig.ExpriedOn.Lock : settingParentConfig.ExpriedOn.Lock;
                settingChildConfig.ExpriedOn.TypeExpiredOn = enterPriseConfig.ExpriedOn.Lock ? enterPriseConfig.ExpriedOn.TypeExpiredOn : settingParentConfig.ExpriedOn.TypeExpiredOn;
                settingChildConfig.ExpriedOn.TimeExpried = enterPriseConfig.ExpriedOn.Lock ? enterPriseConfig.ExpriedOn.TimeExpried : settingParentConfig.ExpriedOn.TimeExpried;
                settingChildConfig.ExpriedOn.DateExpried = enterPriseConfig.ExpriedOn.Lock ? enterPriseConfig.ExpriedOn.DateExpried : settingParentConfig.ExpriedOn.DateExpried;
                settingChildConfig.ExpriedOn.DateDeadline = enterPriseConfig.ExpriedOn.Lock ? enterPriseConfig.ExpriedOn.DateDeadline : settingParentConfig.ExpriedOn.DateDeadline;

                settingParentConfig.ExpriedOn.Lock = enterPriseConfig.ExpriedOn.Lock ? enterPriseConfig.ExpriedOn.Lock : settingParentConfig.ExpriedOn.Lock;
                settingParentConfig.ExpriedOn.TypeExpiredOn = enterPriseConfig.ExpriedOn.Lock ? enterPriseConfig.ExpriedOn.TypeExpiredOn : settingParentConfig.ExpriedOn.TypeExpiredOn;
                settingParentConfig.ExpriedOn.TimeExpried = enterPriseConfig.ExpriedOn.Lock ? enterPriseConfig.ExpriedOn.TimeExpried : settingParentConfig.ExpriedOn.TimeExpried;
                settingParentConfig.ExpriedOn.DateExpried = enterPriseConfig.ExpriedOn.Lock ? enterPriseConfig.ExpriedOn.DateExpried : settingParentConfig.ExpriedOn.DateExpried;
                settingParentConfig.ExpriedOn.DateDeadline = enterPriseConfig.ExpriedOn.Lock ? enterPriseConfig.ExpriedOn.DateDeadline : settingParentConfig.ExpriedOn.DateDeadline;
            }
            if (enterPriseConfig.ModificationUploadedArtifacts.Lock || settingParentConfig.ModificationUploadedArtifacts.Lock)
            {
                settingChildConfig.ModificationUploadedArtifacts.Lock = enterPriseConfig.ModificationUploadedArtifacts.Lock ? enterPriseConfig.ModificationUploadedArtifacts.Lock : settingParentConfig.ModificationUploadedArtifacts.Lock;
                settingChildConfig.ModificationUploadedArtifacts.AllowModification = enterPriseConfig.ModificationUploadedArtifacts.Lock ? enterPriseConfig.ModificationUploadedArtifacts.AllowModification : settingParentConfig.ModificationUploadedArtifacts.AllowModification;
                settingParentConfig.ModificationUploadedArtifacts.Lock = enterPriseConfig.ModificationUploadedArtifacts.Lock ? enterPriseConfig.ModificationUploadedArtifacts.Lock : settingParentConfig.ModificationUploadedArtifacts.Lock;
                settingParentConfig.ModificationUploadedArtifacts.AllowModification = enterPriseConfig.ModificationUploadedArtifacts.Lock ? enterPriseConfig.ModificationUploadedArtifacts.AllowModification : settingParentConfig.ModificationUploadedArtifacts.AllowModification;
            }
            if (enterPriseConfig.AudioRecording.Lock || settingParentConfig.AudioRecording.Lock )
            {
                settingChildConfig.AudioRecording.Lock = enterPriseConfig.AudioRecording.Lock ? enterPriseConfig.AudioRecording.Lock : settingParentConfig.AudioRecording.Lock;
                settingChildConfig.AudioRecording.AllowRecording = enterPriseConfig.AudioRecording.Lock ? enterPriseConfig.AudioRecording.AllowRecording  : settingParentConfig.AudioRecording.AllowRecording;
                settingParentConfig.AudioRecording.Lock = enterPriseConfig.AudioRecording.Lock ? enterPriseConfig.AudioRecording.Lock : settingParentConfig.AudioRecording.Lock;
                settingParentConfig.AudioRecording.AllowRecording = enterPriseConfig.AudioRecording.Lock ? enterPriseConfig.AudioRecording.AllowRecording : settingParentConfig.AudioRecording.AllowRecording;
            }
            if (enterPriseConfig.VideoRecording.Lock || settingParentConfig.VideoRecording.Lock)
            {
                settingChildConfig.VideoRecording.Lock = enterPriseConfig.VideoRecording.Lock ? enterPriseConfig.VideoRecording.Lock : settingParentConfig.VideoRecording.Lock;
                settingChildConfig.VideoRecording.AllowRecording = enterPriseConfig.VideoRecording.Lock ? enterPriseConfig.VideoRecording.AllowRecording : settingParentConfig.VideoRecording.AllowRecording;
                settingParentConfig.VideoRecording.Lock = enterPriseConfig.VideoRecording.Lock ? enterPriseConfig.VideoRecording.Lock : settingParentConfig.VideoRecording.Lock;
                settingParentConfig.VideoRecording.AllowRecording = enterPriseConfig.VideoRecording.Lock ? enterPriseConfig.VideoRecording.AllowRecording : settingParentConfig.VideoRecording.AllowRecording;
            }
            if (enterPriseConfig.CameraCapture.Lock || settingParentConfig.CameraCapture.Lock)
            {
                settingChildConfig.CameraCapture.Lock = enterPriseConfig.CameraCapture.Lock ? enterPriseConfig.VideoRecording.Lock : settingParentConfig.CameraCapture.Lock;
                settingChildConfig.CameraCapture.AllowCapture = enterPriseConfig.CameraCapture.Lock ? enterPriseConfig.CameraCapture.AllowCapture : settingParentConfig.CameraCapture.AllowCapture;
                settingParentConfig.CameraCapture.Lock = enterPriseConfig.CameraCapture.Lock ? enterPriseConfig.VideoRecording.Lock : settingParentConfig.CameraCapture.Lock;
                settingParentConfig.CameraCapture.AllowCapture = enterPriseConfig.CameraCapture.Lock ? enterPriseConfig.CameraCapture.AllowCapture : settingParentConfig.CameraCapture.AllowCapture;
            }
            if (enterPriseConfig.Upload.Lock || settingParentConfig.Upload.Lock)
            {
                settingChildConfig.Upload.Lock = enterPriseConfig.Upload.Lock ? enterPriseConfig.Upload.Lock : settingParentConfig.Upload.Lock;
                settingChildConfig.Upload.AllowUpload = enterPriseConfig.Upload.Lock ? enterPriseConfig.Upload.AllowUpload : settingParentConfig.Upload.AllowUpload;
                settingParentConfig.Upload.Lock = enterPriseConfig.Upload.Lock ? enterPriseConfig.Upload.Lock : settingParentConfig.Upload.Lock;
                settingParentConfig.Upload.AllowUpload = enterPriseConfig.Upload.Lock ? enterPriseConfig.Upload.AllowUpload : settingParentConfig.Upload.AllowUpload;
            }
            if (enterPriseConfig.DisplayPerformanceBandsInEnterResults.Lock || settingParentConfig.DisplayPerformanceBandsInEnterResults.Lock)
            {
                settingChildConfig.DisplayPerformanceBandsInEnterResults.Lock = enterPriseConfig.DisplayPerformanceBandsInEnterResults.Lock ? enterPriseConfig.DisplayPerformanceBandsInEnterResults.Lock : settingParentConfig.DisplayPerformanceBandsInEnterResults.Lock;
                settingChildConfig.DisplayPerformanceBandsInEnterResults.AllowDisplay = enterPriseConfig.DisplayPerformanceBandsInEnterResults.Lock ? enterPriseConfig.DisplayPerformanceBandsInEnterResults.AllowDisplay : settingParentConfig.DisplayPerformanceBandsInEnterResults.AllowDisplay;
                settingParentConfig.DisplayPerformanceBandsInEnterResults.Lock = enterPriseConfig.DisplayPerformanceBandsInEnterResults.Lock ? enterPriseConfig.DisplayPerformanceBandsInEnterResults.Lock : settingParentConfig.DisplayPerformanceBandsInEnterResults.Lock;
                settingParentConfig.DisplayPerformanceBandsInEnterResults.AllowDisplay = enterPriseConfig.DisplayPerformanceBandsInEnterResults.Lock ? enterPriseConfig.DisplayPerformanceBandsInEnterResults.AllowDisplay : settingParentConfig.DisplayPerformanceBandsInEnterResults.AllowDisplay;
            }
            if (enterPriseConfig.AllowResultDateChange.Lock || settingParentConfig.AllowResultDateChange.Lock)
            {
                settingChildConfig.AllowResultDateChange.Lock = enterPriseConfig.AllowResultDateChange.Lock ? enterPriseConfig.AllowResultDateChange.Lock : settingParentConfig.AllowResultDateChange.Lock;
                settingChildConfig.AllowResultDateChange.AllowChange = enterPriseConfig.AllowResultDateChange.Lock ? enterPriseConfig.AllowResultDateChange.AllowChange : settingParentConfig.AllowResultDateChange.AllowChange;
                settingParentConfig.AllowResultDateChange.Lock = enterPriseConfig.AllowResultDateChange.Lock ? enterPriseConfig.AllowResultDateChange.Lock : settingParentConfig.AllowResultDateChange.Lock;
                settingParentConfig.AllowResultDateChange.AllowChange = enterPriseConfig.AllowResultDateChange.Lock ? enterPriseConfig.AllowResultDateChange.AllowChange : settingParentConfig.AllowResultDateChange.AllowChange;
            }
            return (settingParentConfig, settingChildConfig);
        }
        public DataLockerPreferenceLocalize GetDataLockerPreferenceLocalize(int districtID)
        {
            var resources = _localizeResourceRepository
                .Select()
                .Where(x => (x.Key.Equals(LocalizeResourceKeys.DataLockerPreference.CAMERA_CAPTURE) ||
                        x.Key.Equals(LocalizeResourceKeys.DataLockerPreference.EXPRIED_ON) ||
                        x.Key.Equals(LocalizeResourceKeys.DataLockerPreference.MODIFICATION_UPLOADED_ARTIFACTS) ||
                        x.Key.Equals(LocalizeResourceKeys.DataLockerPreference.PUBLISHING_TO_STUDENT_PORTAL) ||
                        x.Key.Equals(LocalizeResourceKeys.DataLockerPreference.UPLOAD) ||
                        x.Key.Equals(LocalizeResourceKeys.DataLockerPreference.VIDEO_RECORDING) ||
                        x.Key.Equals(LocalizeResourceKeys.DataLockerPreference.AUDIO_RECORDING) ||
                        x.Key.Equals(LocalizeResourceKeys.DataLockerPreference.DISPLAY_PERFORMANCE_BANDS_IN_ENTER_RESULTS) ||
                        x.Key.Equals(LocalizeResourceKeys.DataLockerPreference.ALLOW_RESULT_DATE_CHANGE))
                        && (x.DistrictID == districtID || x.DistrictID.HasValue == false)
                ).ToList();

            return new DataLockerPreferenceLocalize() {
                AudioRecording = GetLocalize(resources, LocalizeResourceKeys.DataLockerPreference.AUDIO_RECORDING),
                CameraCapture = GetLocalize(resources, LocalizeResourceKeys.DataLockerPreference.CAMERA_CAPTURE),
                ExpriedOn = GetLocalize(resources, LocalizeResourceKeys.DataLockerPreference.EXPRIED_ON),
                ModificationUploadedArtifacts = GetLocalize(resources, LocalizeResourceKeys.DataLockerPreference.MODIFICATION_UPLOADED_ARTIFACTS),
                PublishingToStudentPortal = GetLocalize(resources, LocalizeResourceKeys.DataLockerPreference.PUBLISHING_TO_STUDENT_PORTAL),
                Upload = GetLocalize(resources, LocalizeResourceKeys.DataLockerPreference.UPLOAD),
                VideoRecording = GetLocalize(resources, LocalizeResourceKeys.DataLockerPreference.VIDEO_RECORDING),
                DisplayPerformanceBandsInEnterResults = GetLocalize(resources, LocalizeResourceKeys.DataLockerPreference.DISPLAY_PERFORMANCE_BANDS_IN_ENTER_RESULTS),
                AllowResultDateChange = GetLocalize(resources, LocalizeResourceKeys.DataLockerPreference.ALLOW_RESULT_DATE_CHANGE)
            };
        }
        private string GetLocalize(List<LocalizeResource> resources, string key)
        {
            if (resources == null || resources.Count() == 0) return key;

            var localeByDistrict = resources.FirstOrDefault(x => x.DistrictID.HasValue && x.Key.Equals(key));
            if (localeByDistrict == null)
            {
                localeByDistrict = resources.FirstOrDefault(x => !x.DistrictID.HasValue && x.Key.Equals(key));
            }

            return localeByDistrict?.Label ?? key;
        }

        public IEnumerable<Preferences> GetPreferenceByLevelAndIds(string level, IEnumerable<int> ids)
        {
            return Select().Where(o => o.Level == level && ids.Contains(o.Id)).ToList();
        }

        public void AddDefaultTagOptions(IList<Tag> tags)
        {
            var defaultOptions = new List<(string Key, string Value)> { (Constanst.REQUIRE_TEST_TAKER_AUTHENTICATION, "0") };
            foreach (var defaultOption in defaultOptions)
            {
                if (!tags.Any(x => x.Key == defaultOption.Key))
                {
                    tags.Add(new Tag
                    {
                        Key = defaultOption.Key,
                        Value = defaultOption.Value
                    });
                }
            }
        }

        public void InsertMultipleRecord(List<Preferences> items)
        {
            if (items == null || items.Count == 0)
            {
                return;
            }

            repository.InsertMultipleRecord(items);
        }
    }
}
