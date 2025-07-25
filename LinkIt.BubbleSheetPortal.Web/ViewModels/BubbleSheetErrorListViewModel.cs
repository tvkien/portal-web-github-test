using System;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class BubbleSheetErrorListViewModel
    {
        public int BubbleSheetErrorId { get; set; }
        public string FileName { get; set; }
        public string Message { get; set; }
        public string UploadedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int ErrorCode { get; set; }
    }
}