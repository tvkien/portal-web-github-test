using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.BubbleSheetReview
{
    public class GetBubbleSheetReviewResponse
    {
        public int TotalRecord { get; set; }

        public List<BubbleSheetListItem> Data { get; set; }
    }
}
