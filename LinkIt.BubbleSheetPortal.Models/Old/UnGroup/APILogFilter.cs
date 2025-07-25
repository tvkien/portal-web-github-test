using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class APILogFilter
    {
        public string APIName { get; set; }
        public string APIStatus { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int DistrictId { get; set; }

        public APILogFilter()
        {
            DistrictId = -1;
            APIName = string.Empty;
            APIStatus = string.Empty;
            FromDate = string.Empty;
            ToDate = string.Empty;
        }
    }
}
