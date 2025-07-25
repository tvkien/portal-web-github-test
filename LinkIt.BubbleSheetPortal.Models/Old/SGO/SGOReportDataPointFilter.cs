using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.SGO
{
    public class SGOReportDataPointFilter
    {
        public int SgoDataPointId { get; set; }
        public int FilterId { get; set; }
        public int FilterType { get; set; }
        public string FilterName { get; set; }
    }
}
