using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.TestResultRemover
{
    public class TestResultAuditModel
    {
        public string VisitorsIPAddr { get; set; }
        public int UserID { get; set; }
        public int TestResultID { get; set; }
        public int QTIOnlineTestSessionID { get; set; }
    }
}
