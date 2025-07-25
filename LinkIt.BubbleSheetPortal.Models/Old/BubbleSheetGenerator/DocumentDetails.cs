using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.BubbleSheetGenerator
{
    [Serializable]
    public class DocumentDetails
    {
        public string Title { get; set; }
        public int PageCount { get; set; }
        public int DistrictId { get; set; }
    }
}
