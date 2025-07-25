using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.PerformanceBandAutomations
{
    public class ApplySettingForPBSDto
    {
        public IEnumerable<ApplySettingForPBSItemDto> VirtualTests { get; set; }
    }

    public class ApplySettingForPBSItemDto
    {
        public int VirtualTestID { get; set; }
        public string PBSInEffect { get; set; }
        public bool IsChange { get; set; }
    }
}
