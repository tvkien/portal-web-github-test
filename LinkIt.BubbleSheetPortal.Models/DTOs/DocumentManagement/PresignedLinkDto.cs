using System;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.DocumentManagement
{
    public class PresignedLinkRequestDto
    {
        public string FileName { get; set; }
        public int DocumentTypeId { get; set; }
        public int Author { get; set; }
        public string DistrictId { get; set; }
        public Guid? DocumentGuid { get; set; }
        public string UploadId { get; set; }
        public string PrevETags { get; set; }
        public int PartNumber { get; set; }
        public bool IsFinished { get; set; }
        public bool IsMultiPart { get; set; }
    }
    public class PresignedLinkResponseDto
    {
        public string Url { get; set; }
        public Guid DocumentGuid { get; set; }
        public string UploadId { get; set; }
    }
}
