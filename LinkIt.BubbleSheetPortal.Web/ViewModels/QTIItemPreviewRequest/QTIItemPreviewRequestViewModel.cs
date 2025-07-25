using LinkIt.BubbleSheetPortal.Models.DTOs.MultiPartExpression;
using LinkIt.BubbleSheetPortal.Models.DTOs.TestMaker;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.QTIItemPreviewRequest
{
    public class QTIItemPreviewRequestViewModel
    {
        public int VirtualTestId { get; set; }
        public string XmlContent { get; set; }
        public List<MultiPartExpressionPreview> MultiPartExpressions { get; set; }
    }
}
