namespace LinkIt.BubbleSheetPortal.Models
{
    public class TestStudentSessionExportItem
    {
        public string SchoolName { get; set; }

        public string SchoolCode { get; set; }

        public string UserName { get; set; }

        public string UserCode { get; set; }

        public string Term { get; set; }

        public string ClassName { get; set; }

        public string ClassSection { get; set; }

        public string CourseNumber { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public string StudentLocalID { get; set; }

        public string StudentStateID { get; set; }

        public string Gender { get; set; }

        public string Race { get; set; }

        public string Grade { get; set; }

        public string TestCode { get; set; }
        public string TestName { get; set; }

        public string TestSessionStatus { get; set; }

        public System.DateTime? AssignmentDate { get; set; }
        
        public string AssignmentShortDate
        {
            get { return AssignmentDate.HasValue ? string.Format("{0:MM/dd/yy}", AssignmentDate.Value) : string.Empty; }
        }

    }
}
