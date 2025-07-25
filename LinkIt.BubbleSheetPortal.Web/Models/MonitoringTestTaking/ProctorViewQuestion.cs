namespace LinkIt.BubbleSheetPortal.Web.Models.MonitoringTestTaking
{
    public class ProctorViewQuestion
    {
        public int VirtualQuestionID { get; set; }
        public int QuestionID { get; set; }
        public int QuestionOrder { get; set; }
        public int AnswerOrder { get; set; }
        public bool Answered { get; set; }
        public bool ManualReview { get; set; }
        public int QuestionGroupId { get; set; }
        public int TimesVisited { get; set; }
        public int TimeSpent { get; set; }
    }
}
