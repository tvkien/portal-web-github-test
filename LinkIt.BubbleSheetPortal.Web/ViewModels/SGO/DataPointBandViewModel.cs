using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.SGO
{
    public class DataPointBandViewModel
    {
        public int Id { get; set; }
        public int DataPointId { get; set; }
        public string Name { get; set; }

        public double Low { get; set; }
        public double High { get; set; }
    }
}