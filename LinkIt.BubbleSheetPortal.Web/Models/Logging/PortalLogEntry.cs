using System;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Web.Models.Logging
{
    public class PortalLogEntry
    {
        public DateTime StartTime { get; set; }
        public double Duration { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public int RoleID { get; set; }
        public int DistrictID { get; set; }
        public string IPAddress { get; set; }
        public string InstanceName { get; set; }
        public string Url { get; set; }
        public string Method { get; set; }
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public string QueryString { get; set; }
        public string Payload { get; set; }
        public int StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public List<string> Exceptions { get; set; }
        public List<string> ErrorLogs { get; set; }
    }
}
