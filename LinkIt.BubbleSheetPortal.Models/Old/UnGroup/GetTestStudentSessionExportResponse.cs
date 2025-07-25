namespace LinkIt.BubbleSheetPortal.Models
{
    public class GetTestStudentSessionExportResponse
    {
        public string SchoolName { get; set; }

        public string SchoolCode { get; set; }

        public string UserName { get; set; }

        public string UserCode { get; set; }

        public string Term { get; set; }

        public string ClassName { get; set; }

        public string ClassSection { get; set; }

        public string CourseNumber { get; set; }

        public string Grade { get; set; }

        public string TestCode { get; set; }

        public string TestName { get; set; }

        public string Students { get; set; }
        public int QTITestClassAssignmentId { get; set; }

        public System.DateTime? AssignmentDate { get; set; }
        public int NotStarted { get; set; }
        public int WaitingForReview { get; set; }
        public int Started { get; set; }
    }
}
