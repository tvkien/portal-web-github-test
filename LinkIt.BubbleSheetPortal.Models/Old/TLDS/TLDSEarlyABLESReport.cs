using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.TLDS
{
    public class TLDSEarlyABLESReport
    {
        public int EarlyABLESReportId { get; set; }
        public int ProfileId { get; set; }
        public string ReportName { get; set; }
        public bool LearningReadinessReportCompleted { get; set; }
        public DateTime? ReportDate { get; set; }
        public bool AvailableOnRequest { get; set; }
    }
}
