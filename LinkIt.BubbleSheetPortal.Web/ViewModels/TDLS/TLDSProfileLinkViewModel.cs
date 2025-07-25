using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.TDLS
{
    public class TLDSProfileLinkViewModel
    {
        public int ProfileId { get; set; }
        public string StudentFirstName { get; set; }
        public string StudentLastName { get; set; }
        public string Guardian { get; set; }
        public string SectionCompleted { get; set; }
        public string LinkUrl { get; set; }
        public DateTime ExpiredDate { get; set; }
        public string Status { get; set; }
        public string TLDSProfileLinkId { get; set; }
        public bool IsAccess { get; set; }

        public int? ProfileStatus { get; set; }

        public bool ProfileIsReadOnly { get; set; }
        public int? EnrolmentYear { get; set; }
        public int? TLDSGroupID { get; set; }
    }
}
