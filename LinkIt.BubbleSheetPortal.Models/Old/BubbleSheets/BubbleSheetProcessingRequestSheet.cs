namespace LinkIt.BubbleSheetPortal.Models
{
    public class BubbleSheetProcessingRequestSheet
    {
        public int RequestSheetId { get; set; }
        public string SheetJobTicket { get; set; }
        public int NumberOfGraphExtraPages { get; set; }
        public int NumberOfPlainExtraPages { get; set; }
        public int NumberOfLinedExtraPages { get; set; }
    }
}
