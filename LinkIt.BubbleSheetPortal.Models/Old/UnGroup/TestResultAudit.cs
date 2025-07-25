using System;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class TestResultAudit
    {
        private string type = string.Empty;
        private string ipAddress = string.Empty;

        public int TestResultAuditId { get; set; }
        public int TestResultId { get; set; }
        public int UserId { get; set; }
        public DateTime AuditDate { get; set; }       

        public string Type
        {
            get { return type; }
            set { type = value.ConvertNullToEmptyString(); }
        }

        public string IPAddress
        {
            get { return ipAddress; }
            set { ipAddress = value.ConvertNullToEmptyString(); }
        }
        public string TestResultIDs { get; set; }
    }
}
