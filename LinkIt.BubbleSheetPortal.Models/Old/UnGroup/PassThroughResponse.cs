using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class PassThroughResponse
    {
        public string StudentID { get; set; }
        public string TestCode { get; set; }
        public string AssignmentGUID { get; set; }
        public string RedirectUrl { get; set; }
        public string Timestamp { get; set; }
        public string PassThroughType { get; set; }
    }
}
