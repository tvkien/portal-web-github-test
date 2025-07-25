using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class APILogCustom
    {
        private string requestURL = string.Empty;
        private string responseCode = string.Empty;
        private string aPIName = string.Empty;
        private string iPAddress = string.Empty;
        private string districtName = string.Empty;
        private string responseStatus = string.Empty;

        public int APILogID { get; set; }
        public DateTime RequestDate { get; set; }
        public int DistrictID { get; set; }

        public string ResponseStatus
        {
            get { return responseStatus; }
            set { responseStatus = value.ConvertNullToEmptyString(); }
        }

        public string RequestURL
        {
            get { return requestURL; }
            set { requestURL = value.ConvertNullToEmptyString(); }
        }

        public string ResponseCode
        {
            get { return responseCode; }
            set { responseCode = value.ConvertNullToEmptyString(); }
        }

        public string APIName
        {
            get { return aPIName; }
            set { aPIName = value.ConvertNullToEmptyString(); }
        }

        public string IPAddress
        {
            get { return iPAddress; }
            set { iPAddress = value.ConvertNullToEmptyString(); }
        }

        public string DistrictName
        {
            get { return districtName; }
            set { districtName = value.ConvertNullToEmptyString(); }
        }
    }
}
