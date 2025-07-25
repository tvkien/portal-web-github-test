using System;

namespace LinkIt.BubbleSheetPortal.Models.HelpResource
{
    public class HelpResourcesSearchItem
    {
        public int HelpResourceID { get; set; }

        public int? HelpResourceTypeID { get; set; }

        public string HelpResourceTypeIcon { get; set; }

        public int? HelpResourceFileTypeID { get; set; }

        public string HelpResourceFilePath { get; set; }

        public string HelpResourceLink { get; set; }

        public string CategoryText { get; set; }

        public string Topic { get; set; }

        public string Description { get; set; }

        public DateTime? DateUpdated { get; set; }

        public string HelpResourceFileTypeName { get; set; }
        
        public string HelpResourceTypeDisplayText { get; set; }
    }
}
