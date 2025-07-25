using System;
using System.Collections.Generic;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class AblesResponsesData
    {
        public int QuestionOrder { get; set; } 
        public int PointEarned { get; set; }
    }

    public class AblesResponsesFullData
    {
        public int QuestionOrder { get; set; }
        public int PointEarned { get; set; }
        public string AnswerLetter { get; set; }
    }
}