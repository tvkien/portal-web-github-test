using System;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class AnswerLog
    {            
        public int AnswerLogID { get; set; }
        public int AnswerID { get; set; }
        public int PointsEarned { get; set; }
        public int PointsPossible { get; set; }
        public bool WasAnswered { get; set; }
        public int TestResultID { get; set; }
        public int VirtualQuestionID { get; set; }
        public bool Blocked { get; set; }
        public bool? Overridden { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public string AnswerLetter { get; set; }
        public string AnswerText { get; set; }
        public string BubbleSheetErrorType { get; set; }
        public string ResponseIdentifier { get; set; }
        public string AnswerImage { get; set; }
        public string HighlightQuestion { get; set; }
        public string HighlightPassage { get; set; }
    }
}
