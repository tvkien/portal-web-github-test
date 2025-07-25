using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.SGO
{
    public class SGOAddEditBandViewModel
    {
        public int DataPointId { get; set; }
        public List<DataPointBandViewModel> LstDataPointBands;

        public SGOAddEditBandViewModel()
        {
            LstDataPointBands = new List<DataPointBandViewModel>();
        }
    }
}