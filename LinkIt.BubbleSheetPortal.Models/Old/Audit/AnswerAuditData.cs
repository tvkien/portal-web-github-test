using System;

namespace LinkIt.BubbleSheetPortal.Models.Audit
{
    public class AnswerAuditData
    {
        public int AnswerAuditID { get; set; }
        public int AnswerID { get; set; }
        public int UserID { get; set; }
        public DateTime DateTimeStamp { get; set; }
        public int? PreviousValue { get; set; }
        public int? NewValue { get; set; }
    }
}
