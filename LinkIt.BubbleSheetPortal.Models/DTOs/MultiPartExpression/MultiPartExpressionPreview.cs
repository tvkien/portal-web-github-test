using System;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.MultiPartExpression
{
    public class MultiPartExpressionPreview
    {
        public int MultiPartExpressionPreviewID { get; set; }
        public string QTIItemPreviewRequestID { get; set; }
        public string Expression { get; set; }
        public string EnableElements { get; set; }
        public int? Order { get; set; }
        public string Rules { get; set; }
    }
}
