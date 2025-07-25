using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class QtiOnlineTestSessionAnswer
    {
        public int QtiOnlineTestSessionAnswerID { get; set; }
        public int QtiOnlineTestSessionID { get; set; }
        public int VirtualQuestionID { get; set; }
        public string AnswerChoice { get; set; }
        public bool Answered { get; set; }
        public string AnswerText { get; set; }
        public DateTime Timestamp { get; set; }
        public int? PointsEarned { get; set; }
        public string ResponseIdentifier { get; set; }
        public string CrossedAnswer { get; set; }
        public bool Flag { get; set; }
        public string AnswerImage { get; set; }
        public string HighlightQuestion { get; set; }

        public string HighlightPassage { get; set; }
        public bool Status { get; set; }
        public int? QuestionOrder { get; set; }
        public string AnswerTemp { get; set; }
        public int? AnswerOrder { get; set; }
    }
}
