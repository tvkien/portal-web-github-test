namespace LinkIt.BubbleSheetPortal.Models
{
    public class PaggingInfo
    {
        public string SortColumn { get; set; }
        public string SortDirection { get; set; }
        public int StartRow { get; set; }
        public int PageSize { get; set; }
    }
}
