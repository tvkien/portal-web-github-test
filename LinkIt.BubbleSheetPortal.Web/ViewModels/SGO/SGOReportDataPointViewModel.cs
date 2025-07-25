using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.SGO
{
    public class SGOReportDataPointViewModel
    {
        public SGODataPoint SgoDataPoint { get; set; }
        public List<SGODataPointBand> SgoDataPointBands { get; set; }
        public double WeightPercent { get; set; }
    }
}