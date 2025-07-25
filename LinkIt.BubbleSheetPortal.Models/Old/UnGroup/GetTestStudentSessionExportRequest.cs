namespace LinkIt.BubbleSheetPortal.Models
{
    public class GetTestStudentSessionExportRequest : PaggingInfo
    {
        public string AssignDate { get; set; }
        public bool OnlyShowPedingReview { get; set; }
        public bool ShowActiveClassTestAssignment { get; set; }
        public int? UserID { get; set; }
        public int? DistrictID { get; set; }
        public int? QtiTestClassAssignmentId { get; set; }
        public int SchoolID { get; set; }
        public string AssignmentCodes { get; set; }
        public string GradeName { get; set; }
        public string SubjectName { get; set; }
        public string BankName { get; set; }
        public string ClassName { get; set; }
        public string TeacherName { get; set; }
        public string StudentName { get; set; }
        public string TestName { get; set; }
        public string GeneralSearch { get; set; }
    }
}
