using System;

namespace LinkIt.BubbleSheetPortal.Models.Audit
{
    public class AnswerSubAuditData
    {
        public int AnswerSubAuditID { get; set; }
        public int AnswerSubID { get; set; }
        public int UserID { get; set; }
        public DateTime DateTimeStamp { get; set; }
        public int? PreviousValue { get; set; }
        public int? NewValue { get; set; }
    }
}
