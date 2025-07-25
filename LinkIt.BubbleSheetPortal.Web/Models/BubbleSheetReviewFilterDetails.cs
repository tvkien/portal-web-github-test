using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Web.Models
{
    public class BubbleSheetReviewFilterDetails
    {
        public bool Archived { get; set; }
        public IEnumerable<BubbleSheetReviewFilterDetail> FilterDetails { get; set; }

        public BubbleSheetReviewFilterDetails()
        {
            FilterDetails = new List<BubbleSheetReviewFilterDetail>();
        }
    }
}