using System;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class QtiRefObjectHistory
    {
        public int QTIRefObjectHistoryId { get; set; }
        public int QTIRefObjectId { get; set; }
        public DateTime ChangedDate { get; set; }
        public string XmlContent { get; set; }
        public int AuthorId { get; set; }
    }
}
