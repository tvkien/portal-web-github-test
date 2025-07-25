using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.ResponseProcessingDTO
{
    public class DragDropNumericalRPDTO : BaseResponseProcessingDTO
    {
        public string ExpressionPattern { get; set; }
        public List<SrcOfDragDropNumericalRPDTO> Sources { get; set; } 
        public List<string> Destinations { get; set; } 
    }
}
