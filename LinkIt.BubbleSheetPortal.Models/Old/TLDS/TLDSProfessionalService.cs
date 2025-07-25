using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.TLDS
{
    public class TLDSProfessionalService
    {
        public int ProfessionalServiceID { get; set; }
        public int ProfileID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string ContactPerson { get; set; }
        public string Position { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool? WrittenReportAvailable { get; set; }
        public DateTime? ReportForwardedToSchoolDate { get; set; }
        public bool? Attached { get; set; }
        public bool? AvailableUponRequested { get; set; }
    }
}
