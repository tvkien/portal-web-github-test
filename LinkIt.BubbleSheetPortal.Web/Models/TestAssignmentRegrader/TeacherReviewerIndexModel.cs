namespace LinkIt.BubbleSheetPortal.Web.Models.TestAssignmentRegrader
{
    public class TeacherReviewerIndexModel
    {
        public int VirtualTestID { get; set; }
        public string VirtualTestName { get; set; }
        public int? VirtualTestSubtypeID { get; set; }
        public int? QTITestClassAssignmentID { get; set; }
        public int? QTITestStudentAssignmentID { get; set; }
        public int TutorialMode { get; set; }
        public int? TestScoreMethodID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? DataSetCategoryID { get; set; }
    }
}
