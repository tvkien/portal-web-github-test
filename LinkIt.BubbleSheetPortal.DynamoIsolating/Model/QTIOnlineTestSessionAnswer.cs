using System;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.DynamoIsolating.Model
{
    public class QTIOnlineTestSessionAnswer
    {
        public List<QTIOnlineTestSessionAnswerSub> QTIOnlineTestSessionAnswerSubs { get; set; }
        public int QTIOnlineTestSessionAnswerID { get; set; }
        public int QTIOnlineTestSessionID { get; set; }
        public int VirtualQuestionID { get; set; }
        public string AnswerChoice { get; set; }
        public bool? Answered { get; set; }
        public string AnswerText { get; set; }
        public DateTime Timestamp { get; set; }
        public int? PointsEarned { get; set; }
        public string ResponseIdentifier { get; set; }
        public string CrossedAnswer { get; set; }
        public string Flag { get; set; }
        public string AnswerImage { get; set; }
        public string HighlightQuestion { get; set; }
        public string HighlightPassage { get; set; }
        public string HighlightQuestionGroupCommon { get; set; }
        public bool Status { get; set; }
        public int QuestionOrder { get; set; }
        public string AnswerTemp { get; set; }
        public int QTISchemaID { get; set; }
        public string BaseVirtualQuestionID { get; set; }
        public string ResponseProcessingTypeID { get; set; }
        public int AnswerOrder { get; set; }
        public string NextVirtualQuestionID { get; set; }
        public string NextResponseIdentifier { get; set; }
        public string XmlContent { get; set; }
        public string WasAnswered { get; set; }
        public int? TimesVisited { get; set; }
        public int? TimeSpent { get; set; }

        public string DrawingContent { get; set; }
    }
}
