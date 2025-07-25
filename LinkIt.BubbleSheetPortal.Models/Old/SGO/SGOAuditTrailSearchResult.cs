using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.SGO
{
    public class SGOAuditTrailSearchResult
    {
        public List<SGOAuditTrailSearchItem> SGOAuditTrailSearchItems { get; set; }
        public List<int> StudentIDs { get; set; } 
        public List<int> GroupIDs { get; set; } 
        public List<int> DataPointIDs { get; set; } 
        public List<SGOGroup> Groups { get; set; }

        public List<Student> Students { get; set; }

        public List<SGODataPoint> DataPoints { get; set; } 
    }
}
