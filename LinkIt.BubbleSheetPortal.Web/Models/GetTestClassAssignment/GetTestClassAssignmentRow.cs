using System;

namespace LinkIt.BubbleSheetPortal.Web.Models.GetTestClassAssignment
{
    public class GetTestClassAssignmentRow
    {
        public int QTITestClassAssignmentID { get; set; }
        public DateTime? AssignmentDate { get; set; }
        public string TestName { get; set; }
        public string TeacherName { get; set; }
        public string ClassName { get; set; }
        public int? NotStarted { get; set; }
        public int? Started { get; set; }
        public int? WaitingForReview { get; set; }
        public int? Completed { get; set; }
        public string Code { get; set; }
        public int ClassID { get; set; }
        public string SubjectName { get; set; }
        public string GradeName { get; set; }
        public string BankName { get; set; }
        public int VirtualTestID { get; set; }
        public int? CodeTime { get; set; }
        public int? DistrictID { get; set; }
        public int? Status { get; set; }


        public string AssignmentShortDate
        {
            get { return AssignmentDate.HasValue ? string.Format("{0:MM/dd/yy}", AssignmentDate.Value) : string.Empty; }
        }
        public string AssignmentShortTime
        {
            get { return AssignmentDate.HasValue ? AssignmentDate.Value.ToShortTimeString() : string.Empty; }
        }
        public string StudentNames { get; set; }
        public int? AssignmentType { get; set; }
        public int? BankIsLocked { get; set; }
        public bool? IsTeacherLed { get; set; }
        public bool IsHide { get; set; }

        public int? AssignmentModifiedUserID { get; set; }


        public string AssignmentFirstName { get; set; }
        public string AssignmentLastName { get; set; }
        public string SchoolName { get; set; }
        public bool IsAllowReview { get; set; }

        public GetTestClassAssignmentRow()
        {
            IsAllowReview = true;
        }
        public int BankId { get; set; }
        public bool IsVirtualTestRetake { get; set; }
        public bool IsVirtualTestPartialRetake { get; set; }
        public string AuthenticationCode { get; set; }
        public bool IsAuthenticationCodeExpired { get; set; }
        public string Assigned { get; set; }
    }
}
