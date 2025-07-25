using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class QTIOnlineTestSession
    {
        public int QTIOnlineTestSessionId { get; set; }
        public int VirtualTestId { get; set; }
        public int StudentId { get; set; }
        public DateTime StartDate { get; set; }
        public int StatusId { get; set; }
        public string AssignmentGUId { get; set; }
        public string SessionQuestionOrder { get; set; }
        public bool? TimeOver { get; set; }
        public DateTime? LastLoginDate { get; set; }
    }
}
