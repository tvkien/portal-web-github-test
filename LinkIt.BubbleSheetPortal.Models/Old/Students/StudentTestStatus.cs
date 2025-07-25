namespace LinkIt.BubbleSheetPortal.Models
{
    public class StudentTestStatus
    {
        public string StudentFullName { get; set; }
        public int Started { get; set; }
        public int Completed { get; set; }
        public int WaitingForReview { get; set; }
        public int NotStarted { get; set; }
    }
}
