namespace LinkIt.BubbleSheetPortal.Web.Models
{
    public class PrintTestOfStudentRequest
    {
        public int? VirtualTestID { get; set; }
        public string QTIOnlineTestSessionIDs { get; set; }
        public int? QTITestClassAssignmentID { get; set; }
        public string TestName { get; set; }
        public string StudentName { get; set; }
        public string ClassName { get; set; }
        public bool ManuallyGradedOnly { get; set; }
        public int? TotalPointsEarned { get; set; }
        public int? TotalPointsPossible { get; set; }
        public string ManuallyGradedOnlyQuestionIds { get; set; }
        public bool? PrintGuidance { get; set; }

        public bool TeacherFeedback { get; set; }
        public bool TheCoverPage { get; set; }
        public bool TheCorrectAnswer { get; set; }
        public bool Passages { get; set; }
        public bool GuidanceAndRationale { get; set; }
        public bool TheQuestionContent { get; set; }

        public int NumberOfColumn { get; set; }
        public bool ShowQuestionPrefix { get; set; }
        public bool ShowBorderAroundQuestions { get; set; }
        public bool ExcludeTestScore { get; set; }
        public bool IncorrectQuestionsOnly { get; set; }
        public bool IncludeAttachment { get; set; }

        public int StudentType { get; set; }        
        public int QuestionType { get; set; }
        public string PrintQuestionIDs { get; set; }
        public string IncorrectQuestionIds { get; set; }
    }
}
