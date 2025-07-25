namespace LinkIt.BubbleSheetPortal.Models
{
    public class QTITestClassAssignmentOTTRefresh
    {
        public int? QTITestClassAssignmentID { get; set; }
        public int? Assigned { get; set; }
        public int? NotStarted { get; set; }
        public int? Started { get; set; }
        public int? Paused { get; set; }
        public int? NotActive { get; set; }

        public int? Autograding { get; set; }
        public int? WaitingForReview { get; set; }
        public int? Completed { get; set; }
    }
}
