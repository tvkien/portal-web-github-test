using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.TLDS
{
    public class TLDSOtherReportPlan
    {
        public int OtherReportPlanID { get; set; }
        public int ProfileID { get; set; }
        public string ReportName { get; set; }
        public DateTime? ReportDate { get; set; }
        public string AttachmentURL { get; set; }
        public bool AvailableOnRequest { get; set; }
    }
}
