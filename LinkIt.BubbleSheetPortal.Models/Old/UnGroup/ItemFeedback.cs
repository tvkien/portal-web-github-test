using System;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ItemFeedback
    {
        public int ItemFeedbackID { get; set; }
        public int QTIOnlineTestSessionAnswerID { get; set; }
        public int? AnswerID { get; set; }
        public string Feedback { get; set; }
        public int UserID { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Guid? DocumentGUID { get; set; }
    }
}

