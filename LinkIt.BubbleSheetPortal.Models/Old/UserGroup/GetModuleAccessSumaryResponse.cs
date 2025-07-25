using LinkIt.BubbleSheetPortal.Models.Old.UserGroup;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class GetModuleAccessSumaryResponse
    {
        public int TotalRecord { get; set; }

        public IEnumerable<XLIModuleAccessSummaryDto> Data { get; set; }
    }
}
