using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using LinkIt.BubbleSheetPortal.Models.ManagePreference;
using LinkIt.BubbleSheetPortal.Web.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    [XmlRoot("testSettings")]
    public class TestSettingsMap
    {
        [XmlElement("options")]
        public TestSettingsViewModel TestSettingViewModel { get; set; }

        [XmlElement("tools")]
        public TestSettingToolViewModel TestSettingToolViewModel { get; set; }

        [XmlElement("lockItems")]
        public LockItemViewModel LockItemViewModel { get; set; }

        [XmlIgnore]
        public bool CanEditOverrideAutoGradedItems { get; set; }

        [XmlIgnore]
        public int TestId { get; set; }

        [XmlIgnore]
        public int VirtualTestSubTypeID { get; set; }
        [XmlIgnore]
        public int VirtualTestSourceId { get; set; }
        [XmlIgnore]
        public int? NavigationMethodID { get; set; }
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
        public TestExtractOptions TestExtractOptions { get; set; }

        [XmlIgnore]
        public DateTime DeadlineDisplay { get; set; }

        [XmlIgnore]
        public string Duration { get; set; }

        [XmlIgnore]
        public string TimeLimitDurationType { get; set; }

        [XmlIgnore]
        public string Deadline { get; set; }

        [XmlIgnore]
        public bool IsExportGenesisGradebook { get; set; }

        [XmlIgnore]
        public bool IsSupportQuestionGroup { get; set; }

        [XmlIgnore]
        public int? DataSetCaterogyID { get; set; }

        public void InitDefault()
        {
            if(TestSettingViewModel == null) 
                TestSettingViewModel = new TestSettingsViewModel();

            if(TestSettingToolViewModel == null)
                TestSettingToolViewModel = new TestSettingToolViewModel();

            if(LockItemViewModel == null)
                LockItemViewModel = new LockItemViewModel();

            LockedAll = false;

            if (TestPreferenceModel == null)
            {
                TestPreferenceModel = new TestPreferenceModel();
            }

            IsSupportQuestionGroup = false;
        }
        public int DataSetOriginId { get; set; }
        public string Label { get; set; }
        public bool IsSurvey { get; set; }
        public bool ShowAssignmentSettings { get; set; }
        public int? CreatedByUserId { get; set; }
    }
}
