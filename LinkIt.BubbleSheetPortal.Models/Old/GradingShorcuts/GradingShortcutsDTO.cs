using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.GradingShorcuts
{
    public class GradingShortcutsDTO
    {
        public int? QTIOnlineTestSessionID { get; set; }
        public int? QTITestClassAssignmentID { get; set; }
        public int? AnswerID { get; set; }
        public int? AnswerSubID { get; set; }
        public string AssignPointsEarned { get; set; }
        public string GradeType { get; set; }
        public bool? UnAnswered { get; set; }
        public bool? Answered { get; set; }
        public string QTIOnlineTestSessionIDs { get; set; }
        public int UserId { get; set; }
    }
}
