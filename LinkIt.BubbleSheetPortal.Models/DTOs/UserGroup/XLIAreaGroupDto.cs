using System;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.UserGroup
{
    public class XLIAreaGroupDto
    {
        public int XLIAreaGroupID { get; set; }
        public int XLIAreaID { get; set; }
        public int XLIGroupID { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Expires { get; set; }
    }
}
