using System;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class TestAssignment
    {
        public int TestAssignmentId { get; set; }
        public int ClassId { get; set; }
        public int StudentId { get; set; }
        public int VirtualTestId { get; set; }
        public DateTime AvailableDateTime { get; set; }
        public DateTime ClosingDateTime { get; set; }
        public bool TestTaken { get; set; }
        public int? AssignedByUserId { get; set; }
    }
}
