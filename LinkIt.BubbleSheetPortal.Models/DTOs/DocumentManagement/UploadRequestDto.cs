using System;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.DocumentManagement
{
    public class UploadRequestDto
    {
        public string FileName { get; set; }
        public Guid? DocumentGuid { get; set; }
        public string UploadId { get; set; }
        public string PrevETags { get; set; }
        public int PartNumber { get; set; }
        public bool IsFinished { get; set; }
        public bool IsMultiPart { get; set; }
    }
}
