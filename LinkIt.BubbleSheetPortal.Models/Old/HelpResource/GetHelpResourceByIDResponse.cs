using System;

namespace LinkIt.BubbleSheetPortal.Models.HelpResource
{
    public class GetHelpResourceByIDResponse
    {
        public int? HelpResourceID { get; set; }

        public int? HelpResourceTypeID { get; set; }

        public int? HelpResourceCategoryID { get; set; }

        public int? HelpResourceFileTypeID { get; set; }

        public string Topic { get; set; }

        public string Description { get; set; }

        public DateTime DateUpdated { get; set; }

        public DateTime DateCreated { get; set; }

        public string KeyWords { get; set; }

        public string HelpResourceFilePath { get; set; }

        public string HelpResourceLink { get; set; }
        public bool? HelpResourceLinkValid { get; set; }
    }
}
