using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class APILog
    {
        public int APILogID { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime ResponseDate { get; set; }
        public string RequestURL { get; set; }
        public string HTTPMethod { get; set; }
        public string ResponseCode { get; set; }
        public string Exception { get; set; }
        public string ResponseStatus { get; set; }
        public string DataPosted { get; set; }
        public string APIName { get; set; }
        public int DistrictID { get; set; }
        public string IPAddress { get; set; }
        public string ExceptionDetail { get; set; }
    }
}
