using System;

namespace LinkIt.BubbleSheetPortal.Models.Isolating
{
    public class IsolatingTestSessionAnswerSubDTO
    {
        public int QTIOnlineTestSessionAnswerSubID { get; set; }
        public int QTIOnlineTestSessionAnswerID { get; set; }
        public int VirtualQuestionSubID { get; set; }
        public string AnswerChoice { get; set; }
        public bool Answered { get; set; }
        public string AnswerText { get; set; }
        public int? PointsEarned { get; set; }
        public string AnswerTemp { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
