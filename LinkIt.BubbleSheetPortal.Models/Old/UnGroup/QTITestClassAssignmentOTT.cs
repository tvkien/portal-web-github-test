using System;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class QTITestClassAssignmentOTT
    {
        public int? ClassID { get; set; }

        public string ClassName { get; set; }

        public string SchoolName { get; set; }

        public int? Status { get; set; }

        public string TeacherName { get; set; }

        public int? DistrictID { get; set; }

        public int? VirtualTestID { get; set; }

        public string TestName { get; set; }

        public string SubjectName { get; set; }

        public string GradeName { get; set; }

        public string BankName { get; set; }

        public int? QTITestClassAssignmentID { get; set; }

        public int? AssignmentType { get; set; }

        public string Code { get; set; }

        public DateTime? AssignmentDate { get; set; }

        public int? CodeTime { get; set; }

        public int? Paused { get; set; }

        public int? Autograding { get; set; }

        public int? Started { get; set; }

        public int? Completed { get; set; }

        public int? WaitingForReview { get; set; }


        public int? NotStarted { get; set; }



        public string StudentNames { get; set; }

                
        public int? Active { get; set; }

        public string AssignmentShortDate
        {
            get { return AssignmentDate.HasValue ? string.Format("{0:MM/dd/yy}", AssignmentDate.Value) : string.Empty; }
        }

        public string AssignmentShortTime
        {
            get { return AssignmentDate.HasValue ? AssignmentDate.Value.ToShortTimeString() : string.Empty; }
        }

        public string Assigned { get; set; }
    }
}
