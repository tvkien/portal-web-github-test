using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.Monitoring
{
    public class GetTestClassAssignmentsOTTResponse
    {
        public int TotalRecord { get; set; }
        public List<QTITestClassAssignmentOTT> Data { get; set; }
    }
}
