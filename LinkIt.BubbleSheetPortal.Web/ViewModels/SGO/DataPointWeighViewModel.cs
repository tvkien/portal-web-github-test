using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.SGO
{
    public class DataPointWeighViewModel
    {
        public List<ListItemExtra> LstWeights { get; set; }

        public DataPointWeighViewModel()
        {
            LstWeights = new List<ListItemExtra>();
        }
    }
}