using System;
using System.Text.RegularExpressions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class QTITestClassAssignment
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
        public int? AssignmentType { get; set; }      

        public string AssignmentShortDate
        {
            get { return AssignmentDate.HasValue ? string.Format("{0:MM/dd/yy}", AssignmentDate.Value) : string.Empty; }
        }
        public string StudentNames { get; set; }
        public string StudentIds { get; set; }
        public string AssignmentShortTime
        {
            get { return AssignmentDate.HasValue ? AssignmentDate.Value.ToShortTimeString() : string.Empty; }
        }
        public int? BankIsLocked { get; set; }

        public int? AssignmentModifiedUserID { get; set; }
        public bool? IsTeacherLed { get; set; }

        public string AssignmentFirstName { get; set; }
        public string AssignmentLastName { get; set; }
        public bool IsHide { get; set; }
        public string SchoolName { get; set; }
        public bool IsAllowReview { get; set; }

        public QTITestClassAssignment()
        {
            IsAllowReview = true;
        }
        public int BankId { get; set; }
        public bool IsVirtualTestRetake { get; set; }
        public bool IsVirtualTestPartialRetake
        {
            get
            {
                return IsVirtualTestRetake && Regex.IsMatch(TestName, @"^.+ PR\d+$");
            }
        }
        public string AuthenticationCode { get; set; }
        public bool IsAuthenticationCodeExpired { get; set; }
        public string Assigned { get; set; }
    }
}
