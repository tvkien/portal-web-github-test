using System;

namespace LinkIt.BubbleSheetPortal.Models.Isolating
{
    public class OnlineTestSessionDTO
    {
        public int QTITestClassAssignmentId { get; set; }
        public int QTIOnlineTestSessionId { get; set; }
        public int StatusId { get; set; }
        public DateTime? Timestamp { get; set; }
        public DateTime? LastLoginDate { get; set; }

        public int Paused { get; set; }
        public int Started { get; set; }
        public int Completed { get; set; }
        public int WaitingForReview { get; set; }
        public int AutoGrading { get; set; }
        public int InActive { get; set; }
    }
}
