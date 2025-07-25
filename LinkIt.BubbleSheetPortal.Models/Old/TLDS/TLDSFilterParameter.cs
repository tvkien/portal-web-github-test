using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.TLDS
{
    public class TLDSFilterParameter
    {
        public int? DistrictId { get; set; }
        public int? CreatedUserId { get; set; }
        public int? SubmittedSchoolID { get; set; }
        public int? TldsProfileId { get; set; }
        public bool? ShowArchived { get; set; }
        public int? EnrollmentYear { get; set; }

        public int? TldsGroupID { get; set; }
    }
}
