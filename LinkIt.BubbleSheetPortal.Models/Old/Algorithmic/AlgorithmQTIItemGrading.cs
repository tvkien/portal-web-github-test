using System;

namespace LinkIt.BubbleSheetPortal.Models.Algorithmic
{
    public class AlgorithmQTIItemGrading
    {
        public int AlgorithmID { get; set; }
        public int QTIItemID { get; set; }
        public string Expression { get; set; }
        public int PointsEarned { get; set; }
        public int? Order { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string Rules { get; set; }
    }
}
