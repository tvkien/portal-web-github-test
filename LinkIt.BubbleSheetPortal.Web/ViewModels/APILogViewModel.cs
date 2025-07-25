using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class APILogViewModel
    {
        public int APILogID { get; set; }
        public DateTime RequestDate { get; set; }
        public string RequestURL { get; set; }
        public string APIName { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseStatus { get; set; }
        //public string Districtname { get; set; }
        public int DistrictId { get; set; }
    }
}