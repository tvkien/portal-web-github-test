using System;
using System.Xml.Serialization;
using LinkIt.BubbleSheetPortal.Models.ManagePreference;

namespace LinkIt.BubbleSheetPortal.Models
{
    [XmlRoot("testSettings")]
    public class TestSettingsMapDTO
    {
        [XmlElement("options")]
        public TestSettingsDTO TestSettingViewModel { get; set; }

        [XmlElement("tools")]
        public TestSettingToolDTO TestSettingToolViewModel { get; set; }

        [XmlElement("lockItems")]
        public LockItemDTO LockItemViewModel { get; set; }

        [XmlIgnore]
        public bool CanEditOverrideAutoGradedItems { get; set; }

        [XmlIgnore]
        public int TestId { get; set; }

        [XmlIgnore]
        public int VirtualTestSubTypeID { get; set; }
        [XmlIgnore]
        public bool LockedAll { get; set; }

        [XmlIgnore]
        public string Value { get; set; }

        [XmlIgnore]
        public TestPreferenceModel TestPreferenceModel { get; set; }

        [XmlIgnore]
        public string SettingLevel { get; set; }
        [XmlIgnore]
        public int SettingLevelID { get; set; }
        [XmlIgnore]
        public bool IsAssignGroup { get; set; }
        [XmlIgnore]
        public bool IslockedBank { get; set; }
        [XmlIgnore]
        public int DistrictId { get; set; }
        [XmlIgnore]
        public int SettingType { get; set; }

        [XmlIgnore]
        public bool IsUseTestExtract { get; set; }

        [XmlIgnore]
        public DateTime DeadlineDisplay { get; set; }

        [XmlIgnore]
        public string Duration { get; set; }

        [XmlIgnore]
        public string Deadline { get; set; }

        public void InitDefault()
        {
            if (TestSettingViewModel == null)
                TestSettingViewModel = new TestSettingsDTO();

            if (TestSettingToolViewModel == null)
                TestSettingToolViewModel = new TestSettingToolDTO();

            if (LockItemViewModel == null)
                LockItemViewModel = new LockItemDTO();

            LockedAll = false;
        }
    }
}
