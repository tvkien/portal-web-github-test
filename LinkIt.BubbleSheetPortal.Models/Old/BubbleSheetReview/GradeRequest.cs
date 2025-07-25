using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.BubbleSheetReview
{
    public class GradeRequest
    {
        private List<ReadResult> readResults = new List<ReadResult>();

        public int ResultCount { get { return ReadResults.Count; } }
        public List<ReadResult> ReadResults
        {
            get { return readResults; }
            set { readResults = value ?? new List<ReadResult>(); }
        } 
    }
}
