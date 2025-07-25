namespace LinkIt.BubbleSheetPortal.Web.Helpers.BubbleSheetAws
{
    public class BubbleSheetProcessingRequestSheetResultResponse
    {
        public int RequestSheetResultResponseId { get; set; }
        public int EstimatedDelay { get; set; }
        public int ProgressPercent { get; set; }
        public string DownloadUrl { get; set; }
        public string StorageName { get; set; }
        public string Error { get; set; }
        public System.Guid SheetJobAccountId { get; set; }
        public string SheetJobTicket { get; set; }
    }
}