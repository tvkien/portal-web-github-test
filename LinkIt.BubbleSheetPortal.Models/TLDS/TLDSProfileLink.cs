using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinkIt.BubbleSheetPortal.Common;

namespace LinkIt.BubbleSheetPortal.Models.TLDS
{
    public class TLDSProfileLink
    {
        public Guid TLDSProfileLinkID { get; set; }

        public int ProfileId { get; set; }

        public string LinkUrl { get; set; }
        
        public DateTime ExpiredDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public bool IsActive { get; set; }

        public TLDSFormSection2 TLDSFormSection2 { get; set; }

        public TLDSFormSection3 TLDSFormSection3 { get; set; }

        public bool IsShowDeactivate{ get; set; }
        public bool IsShowRefresh { get; set; }
        public bool IsShowActivate { get; set; }
        public string StudentFirstName { get; set; }
        public string StudentLastName { get; set; }
        public string Guardian { get; set; }
        public string SectionCompleted { get; set; }
        public string Status { get; set; }

        public int LoginFailed { get; set; }

        public int? ProfileStatus { get; set; }

        public int? EnrolmentYear { get; set; }
        public int? TLDSGroupID { get; set; }

        public bool IsReadOnly { get; set; }
    }
}
