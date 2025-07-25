using System;

namespace LinkIt.BubbleSheetPortal.Models.Audit
{
    public class QTIOnlineTestSessionAnswerSubAuditData
    {
        public int QTIOnlineTestSessionAnswerSubAuditID { get; set; }
        public int QTIOnlineTestSessionAnswerSubID { get; set; }
        public int UserID { get; set; }
        public DateTime DateTimeStamp { get; set; }
        public int? PreviousValue { get; set; }
        public int? NewValue { get; set; }
        public int VirtualQuestionSubID { get; set; }
        public int QTIOnlineTestSessionID { get; set; }
    }
}
