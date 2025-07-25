using System;

namespace LinkIt.BubbleSheetPortal.DynamoIsolating.Model
{
    public class QTIOnlineTestSessionAnswerSub
    {
        public int QTIOnlineTestSessionAnswerSubID { get; set; }
        public int QTIOnlineTestSessionAnswerID { get; set; }
        public int VirtualQuestionSubID { get; set; }
        public string AnswerChoice { get; set; }
        public bool? Answered { get; set; }
        public string AnswerText { get; set; }
        public DateTime Timestamp { get; set; }
        public int? PointsEarned { get; set; }
        public string ResponseIdentifier { get; set; }
        public string CrossedAnswer { get; set; }
        public string AnswerTemp { get; set; }
        public string QTISchemaID { get; set; }
    }
}
