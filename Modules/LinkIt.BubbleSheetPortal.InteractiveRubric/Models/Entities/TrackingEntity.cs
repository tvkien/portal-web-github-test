using System;

namespace LinkIt.BubbleSheetPortal.InteractiveRubric.Models.Entities
{
    public class TrackingEntity
    {
        public DateTime? CreatedDate { get; set; } = DateTime.Now;

        public DateTime? UpdatedDate { get; set; }

        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }
    }
}
