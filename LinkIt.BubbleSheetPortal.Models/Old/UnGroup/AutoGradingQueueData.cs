using System;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class AutoGradingQueueData
    {
        public int AutoGradingQueueID { get; set; }

        public int QTIOnlineTestSessionID { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ProcessingDate { get; set; }

        public int? ProcessingTime { get; set; }

        public int GradedAnswerCount { get; set; }

        public int Status { get; set; }

        public bool ForceGrading { get; set; }

        public int? RequestUserId { get; set; }
        public int Type { get; set; }
        public bool IsAwaitingRetry { get; set; }
    }
}
