using System;

namespace LinkIt.BubbleSheetPortal.Web.Models.TestAssignmentRegrader
{
    public class StudentAssignment
    {
        public int? QTIOnlineTestSessionID { get; set; }
        public int StudentID { get; set; }
        public string AssignmentGUID { get; set; }
        public int StatusID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StudentName { get; set; }
        public int? QTOStatusID { get; set; }
        public bool? TimeOver { get; set; }
        public string StartDate { get; set; }
        public string TestName { get; set; }
        public string RealStudentName { get; set; }

        public bool IsComplete
        {
            get { return QTOStatusID == 4; }
        }

        public bool IsPendingReview
        {
            get { return QTOStatusID == 5; }
        }

        public bool InProgress
        {
            get { return QTOStatusID > 0 && QTOStatusID < 3; }
        }

        public bool Paused
        {
            get { return QTOStatusID == 3; }
        }

        public bool IsNotStart
        {
            get { return !QTOStatusID.HasValue; }
        }

        public bool CanBulkGrading { get; set; }

        public int StudentOrder { get; set; }
    }
}
