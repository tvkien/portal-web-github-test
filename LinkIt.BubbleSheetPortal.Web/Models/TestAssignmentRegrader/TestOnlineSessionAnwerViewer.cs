using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Web.Models.TestAssignmentRegrader
{
    public class TestOnlineSessionAnwerViewer
    {
        public int QTIOnlineTestSessionAnswerID { get; set; }
        public int AnswerID { get; set; }
        public int QTIOnlineTestSessionID { get; set; }
        public int VirtualQuestionID { get; set; }
        public string AnswerChoice { get; set; }
        public bool Answered { get; set; }
        public string AnswerText { get; set; }
        public string AnswerImage { get; set; }
        public int? PointsEarned { get; set; }
        public int QTISchemaID { get; set; }
        public List<TestOnlineSessionAnswerSub> TestOnlineSessionAnswerSubs { get; set; }
        public string XmlContent { get; set; }
        public string CorrectAnswer { get; set; }
        public int PointsPossible { get; set; }
    }
}