using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models.DTOs.VirtualSection;
using System;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.ManagePreference
{
    public class TestPreferenceModel
    {
        public List<Tag> OptionTags { get; set; }
        public List<Tag> ToolTags { get; set; }
        public List<VirtualSectionDto> VirtualSections { get; set; }
        public int LevelId { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public string LastUpdatedDateString { get { return LastUpdatedDate.DisplayDateWithFormat(true); } }
        public int LastUpdatedBy { get; set; }
        public string LastUpdatedByUser { get; set; }

        public TestPreferenceModel()
        {
            OptionTags = new List<Tag>();
            ToolTags = new List<Tag>();
        }
    }
}
