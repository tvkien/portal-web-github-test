using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.TeacherReviewModel
{
    public class TeacherReviewIndexModel
    {
        public int? QtiTestClassAssignmentID { get; set; }
        public int? QtiTestStudentAssignmentID { get; set; }
        public int? StudentId { get; set; }
        public int? SelectFirstStudentForReview { get; set; }
        public int? GradingShotcuts { get; set; }

        public string TeacherName { get; set; }
        public string ClassName { get; set; }

        public QTITestClassAssignmentData QTITestClassAssignment { get; set; }
        public VirtualTestData VirtualTest { get; set; }
        public string GetViewReferenceImgFullPath { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public bool? IsPassThrough { get; set; }
        public int DistrictId { get; set; }
        public bool IsAllowToManualGrade { get; set; }
        public bool IsAllowToPrint { get; set; }

        /// <summary>
        /// student
        /// item
        /// </summary>
        public string GradingType { get; set; }
    }
}