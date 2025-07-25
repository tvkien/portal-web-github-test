using System;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class QTIItemPreviewRequest
    {
        public string QTIItemPreviewRequestId { get; set; }
        public int VirtualTestId { get; set; }
        public string XmlContent { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
