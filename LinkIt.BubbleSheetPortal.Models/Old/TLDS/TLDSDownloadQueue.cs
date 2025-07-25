using System;

namespace LinkIt.BubbleSheetPortal.Models.TLDS
{
    public class TLDSDownloadQueue
    {
        public int TLDSDownloadQueueID { get; set; }
        public string ProfileIDs { get; set; }
        public string FileName { get; set; }
        public int Status { get; set; }
        public int Total { get; set; }
        public string Errors { get; set; }
        public int CompletedFiles { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public int CreatedUserID { get; set; }
    }
}
