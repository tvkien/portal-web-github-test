using System;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class PreviousBubbleSheetDetailsViewModel
    {
        public int? BubbleSheetFileId { get; set; }
        public DateTime? UploadedDate { get; set; }
        public string UploadedBy { get; set; }
        public string FileName { get; set; }
        public string ImageUrl { get; set; }

        public bool OnlyOnePage { get; set; }
    }
}