using System;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.TeacherReview
{
    public class AnswerAttachmentDto
    {
        public int QTIOnlineTestSessionAnswerID { get; set; }
        public int AnswerID { get; set; }
        public Guid? DocumentGuid { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string FilePath { get; set; }
        public int FileSize { get; set; }
        public int AttachmentType { get; set; }
    }
}
