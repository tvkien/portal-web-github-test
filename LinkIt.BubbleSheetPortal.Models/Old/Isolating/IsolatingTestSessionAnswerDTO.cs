using System;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.Isolating
{
    public class IsolatingTestSessionAnswerDTO
    {
        public List<IsolatingTestSessionAnswerSubDTO> QTIOTSessionAnswerSubs { get; set; }

        public int QTIOnlineTestSessionAnswerID { get; set; }
        public string AnswerChoice { get; set; }
        public bool WasAnswered { get; set; }
        public bool Answered { get; set; }
        public string AnswerText { get; set; }
        public int? PointsEarned { get; set; }
        public string HighlightPassage { get; set; }
        public string HighlightQuestion { get; set; }
        public string HighlightQuestionGroupCommon { get; set; }
        public DateTime Timestamp { get; set; }
        public string AnswerTemp { get; set; }
        public bool Flag { get; set; }
        public string AnswerImage { get; set; }
        public int? AnswerOrder { get; set; }
        public int? TimesVisited { get; set; }
        public int? TimeSpent { get; set; }
        public string DrawingContent { get; set; }
    }
}
