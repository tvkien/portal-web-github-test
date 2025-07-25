using System;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class QTIItemHistory
    {
        public int QTIItemHistoryID { get; set; }
        public int QTIItemID { get; set; }
        public DateTime ChangedDate { get; set; }
        public string XmlContent { get; set; }
        public int? AuthorID { get; set; }
    }
}
