using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.TestMaker
{
    public class MultiPartQtiItemExpressionDto
    {
        public int MultiPartQtiItemExpressionId { get; set; }
        public int QtiItemId { get; set; }
        public string Expression { get; set; }
        public string EnableElements { get; set; }
        public int? Order { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string Rules { get; set; }
    }
}
