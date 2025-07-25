using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class AnswerSubViewModel
    {
        public int? AnswerSubID { get; set; }
        public int? AnswerID { get; set; } 
        public int? VirtualQuestionSubID { get; set; }
        public int? PointsEarnedSub { get; set; }
        public int? AnswerPointsPossibleSub { get; set; }
        public string AnswerLetterSub { get; set; }
        public string AnswerTextSub { get; set; }
    }
}