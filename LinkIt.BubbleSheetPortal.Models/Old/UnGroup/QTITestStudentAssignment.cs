using System;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class QTITestStudentAssignment
    {
        public int? QTITestClassAssignmentID { get; set; }
        public string TestName { get; set; }
        public string TeacherName { get; set; }
        public string ClassName { get; set; }
        public string StudentName { get; set; }
        public int StudentID { get; set; }
        public string Code { get; set; }
        public string PassCode { get; set; }
        public string AssignmentState { get; set; }
        public string SubjectName { get; set; }
        public string GradeName { get; set; }
        public string BankName { get; set; }
        public int? DistrictID { get; set; }
        public int? Status { get; set; }
        public DateTime? AssignmentDate { get; set; }
        public int? QTITestStudentAssignmentID { get; set; }
        public int VirtualTestID { get; set; }
        public int? QTIOnlineTestSessionID { get; set; }
        public int? QTIOnlineStatusID { get; set; }
        public bool? TimeOver { get; set; }
        public DateTime? StartDate { get; set; }

        public bool? CanBulkGrading { get; set; }
        public string RealStudentName { get; set; }
    }
}
