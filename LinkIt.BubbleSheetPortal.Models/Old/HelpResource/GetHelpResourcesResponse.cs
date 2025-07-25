using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.HelpResource
{
    public class GetHelpResourcesResponse
    {
        public int TotalRecord { get; set; }
        public List<HelpResourcesSearchItem> Data { get; set; }
    }
}
