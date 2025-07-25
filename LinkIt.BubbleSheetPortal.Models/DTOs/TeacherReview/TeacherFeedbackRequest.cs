using System;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.TeacherReview
{
    public class TeacherFeedbackRequest
    {
        public int ItemFeedbackId { get; set; }

        public int QTIOnlineTestSessionID { get; set; }

        public int VirtualQuestionID { get; set; }

        public int QTIOnlineTestSessionAnswerID { get; set; }

        public int AnswerId { get; set; }

        public int UserID { get; set; }

        public int DistrictID { get; set; }

        public string FeedbackContent { get; set; }

        public bool HasChanged { get; set; }

        public AudioFileDelete FileDelete { get; set; }
    }

    public class AudioFileDelete
    {
        public Guid DocumentGuid { get; set; }
    }
}
