using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.TDLS
{
    public class TLDSDevelopmentOutcomeProfileViewModel
    {
        public int DevelopmentOutcomeProfileId { get; set; }
        public int ProfileId { get; set; }
        public int DevelopmentOutcomeTypeId { get; set; }

        public string DevelopmentOutcomeTypeName { get; set; }
        public string DevelopmentOutcomeContent { get; set; }
        public string StrategyContent { get; set; }
        public string OriginalFileName {  get; set; }
        public string S3FileName { get; set; }
        public string S3Url { get; set; }
    }
}
