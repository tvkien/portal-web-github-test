using System;

namespace LinkIt.BubbleSheetPortal.Models.SGO
{
    public class SGOMilestone
    {
        public int SGOMilestoneID { get; set; }
        public int SGOID { get; set; }
        public int SGOStatusID { get; set; }
        public DateTime MilestoneDate { get; set; }
        public int UserID { get; set; }
    }
}