using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.Audit
{
    public class AuditAnswerDTO
    {
        public int QTIOnlineTestSessionID { get; set; }
        public int UserID { get; set; }
        public int AnswerID { get; set; }
        public int PreviousValueOfAnser { get; set; }
        public int? AnswerSubID { get; set; }
        public int? PreviousValueOfAnserSub { get; set; }
    }
}
