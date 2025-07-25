using System;

namespace LinkIt.BubbleSheetPortal.Models.SGO
{
    public class SGOAuditTrailData
    {
        public int SGOAuditTrailID { get; set; }
        public int ChagedByUserID { get; set; }
        public DateTime ChangedOn { get; set; }
        public int SGOID { get; set; }
        public int SGOActionTypeID { get; set; }
        public string ActionDetail { get; set; }
    }
}