using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class GetTestClassAssignmentsResponse
    {
        public int TotalRecord { get; set; }
        public List<QTITestClassAssignment> Data { get; set; }
    }
}
