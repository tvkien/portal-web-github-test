using System;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.LTI
{
    public class LTIRequestHistory
    {
        public int LTIRequestHistoryID { get; set; }
        public string PlatformID { get; set; }
        public string ClientID { get; set; }
        public string DeploymentID { get; set; }
        public string State { get; set; }
        public string Nonce { get; set; }
        public bool IsCompleted { get; set; }
    }
}
