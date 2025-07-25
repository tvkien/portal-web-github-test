using System;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ExportClassTestAssignmentData
    {
        public DateTime? Assigned { get; set; }
        public int VirtualTestID { get; set; }
        public string Test { get; set; }

        public int? UserID { get; set; }

        public string Username { get; set; }

        public int? SchoolID { get; set; }

        public string SchoolName { get; set; }

        public string ClassName { get; set; }

        public string Code { get; set; }
        public int NotStarted { get; set; }
        public int Started { get; set; }
        public int WaitingForReview { get; set; }
        public int Completed { get; set; }
    }
}