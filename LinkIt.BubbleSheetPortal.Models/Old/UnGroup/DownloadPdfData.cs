using System;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class DownloadPdfData
    {
        public Guid DownloadPdfID { get; set; }
        public string FilePath { get; set; }
        public int UserID { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
