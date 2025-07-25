using System;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.TeacherReview
{
    public class TeacherFeedbackResponse
    {
        public int ItemFeedbackId { get; set; }

        public string FileName { get; set; }

        public string FilePath { get; set; }

        public int FileSize { get; set; }

        public Guid? DocumentGuid { get; set; }

        public bool HasChanged { get; set; }

        public string LastUserUpdatedFeedback { get; set; }

        public string LastDateUpdatedFeedback { get; set; }
    }
}
