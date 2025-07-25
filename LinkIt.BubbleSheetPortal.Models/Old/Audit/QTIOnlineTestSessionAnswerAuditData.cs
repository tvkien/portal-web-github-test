using System;

namespace LinkIt.BubbleSheetPortal.Models.Audit
{
    public class QTIOnlineTestSessionAnswerAuditData
    {
        public int QTIOnlineTestSessionAnswerAuditID { get; set; }
        public int QTIOnlineTestSessionAnswerID { get; set; }
        public int UserID { get; set; }
        public DateTime DateTimeStamp { get; set; }
        public int? PreviousValue { get; set; }
        public int? NewValue { get; set; }
        public int VirtualQuestionID { get; set; }
        public int QTIOnlineTestSessionID { get; set; }
    }
}
