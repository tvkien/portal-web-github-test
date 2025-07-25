using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class AnswerViewModel
    {
        public int TestResultID { get; set; }
        public int? AnswerID { get; set; }
        public int? VirtualQuestionID { get; set; }
        public int? QuestionOrder { get; set; }
        public int? QTIItemID { get; set; }
        public string CorrectAnswer { get; set; }
        public int? PointsEarned { get; set; }
        public int? AnswerPointsPossible { get; set; }
        public bool? WasAnswered { get; set; }
        public string AnswerLetter { get; set; }
        public string AnswerText { get; set; }
    }
}