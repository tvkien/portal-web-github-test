using System;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class QTITestState
    {
        public int QTIOnlineTestSessionAnswerID { get; set; }
        public int AnswerID { get; set; }
        public int QTIOnlineTestSessionID { get; set; }
        public int VirtualQuestionID { get; set; }
        public string AnswerChoice { get; set; }
        public bool? Answered { get; set; }
        public string AnswerText { get; set; }
        public string AnswerImage { get; set; }
        public int? PointsEarned { get; set; }
        public int? ResponseProcessingTypeID { get; set; }
        public int QTISchemaID { get; set; }
        public string AnswerSubs { get; set; }
        public string HighlightQuestion { get; set; }
        public string HighlightPassage { get; set; }
        public string HighlightQuestionGroupCommon { get; set; }
        public decimal? ScoreRaw { get; set; }
        public string AnswerTemp { get; set; }
        public bool? Overridden { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? ItemFeedbackID { get; set; }
        public int? ItemAnswerID { get; set; }
        public string Feedback { get; set; }
        public int? UserIdFeedback { get; set; }
        public string UserUpdatedFeedback { get; set; }
        public DateTime? DateUpdatedFeedback { get; set; }
        public int? AnswerOrder { get; set; }
        public int VirtualSectionMode { get; set; }
        public int TimesVisited { get; set; }
        public int TimeSpent { get; set; }

        public string DrawingContent { get; set; }

    }
}
