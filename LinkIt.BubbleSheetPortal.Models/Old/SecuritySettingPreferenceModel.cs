using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models.ManagePreference;
using System;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class SecuritySettingPreferenceModel
    {
        public List<Tag> OptionTags { get; set; }
        public int LevelId { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public string LastUpdatedDateString { get { return LastUpdatedDate.DisplayDateWithFormat(true); } }
        public int LastUpdatedBy { get; set; }
        public string LastUpdatedByUser { get; set; }
        public SecuritySettingPreferenceModel()
        {
            OptionTags = new List<Tag>();
        }
    }
}
