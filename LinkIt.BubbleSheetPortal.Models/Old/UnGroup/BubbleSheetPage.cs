using System;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class BubbleSheetPage
    {
        public Guid SheetPageId { get; set; }
        public int PageNumber { get; set; }
        public string Ticket { get; set; }
        public int? PageNumberSub { get; set; }
        public string SheetPageIdText { get; set; }
    }
}
