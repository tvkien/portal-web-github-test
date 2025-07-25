using System;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class AnswerSubLog
    {
        public int AnswerSubLogID { get; set; }
        public int AnswerSubID { get; set; }
        public int AnswerID { get; set; }
        public int VirtualQuestionSubID { get; set; }
        public int PointsEarned { get; set; }
        public int PointsPossible { get; set; }
        public string AnswerLetter { get; set; }
        public string AnswerText { get; set; }
        public string ResponseIdentifier { get; set; }        
        public bool? Overridden { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }       
    }
}
