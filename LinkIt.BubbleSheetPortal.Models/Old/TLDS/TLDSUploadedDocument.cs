using System;

namespace LinkIt.BubbleSheetPortal.Models.TLDS
{
    public class TLDSUploadedDocument
    {
        public int UploadedDocumentId { get; set; }
        public int ProfileId { get; set; }
        public string OriginalFileName { get; set; }
        public string S3FileName { get; set; }
        public DateTime UploadedDate { get; set; }
        public int UploadedUserId { get; set; }
        public string S3Url { get; set; }
    }
}
