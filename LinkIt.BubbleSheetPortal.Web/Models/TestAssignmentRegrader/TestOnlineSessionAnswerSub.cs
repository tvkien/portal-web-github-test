using System;

namespace LinkIt.BubbleSheetPortal.Web.Models.TestAssignmentRegrader
{
    public class TestOnlineSessionAnswerSub
    {
        public int QTIOnlineTestSessionAnswerSubID { get; set; }
        public int QTIOnlineTestSessionAnswerID { get; set; }
        public int VirtualQuestionSubID { get; set; }
        public int QTISchemaID { get; set; }
        public string AnswerChoice { get; set; }
        public bool Answered { get; set; }
        public string AnswerText { get; set; }
        public int? PointsEarned { get; set; }
        public int? PointsPossible { get; set; }
        public string ResponseIdentifier { get; set; }
        public int? ResponseProcessingTypeID { get; set; }
        public string AnswerTemp { get; set; }
        public bool IsReviewed
        {
            get
            {
                return PointsEarned.HasValue || ResponseProcessingTypeID == 3;
            }
        }

        public bool? Overridden { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedDate { get; set; }
    }
}