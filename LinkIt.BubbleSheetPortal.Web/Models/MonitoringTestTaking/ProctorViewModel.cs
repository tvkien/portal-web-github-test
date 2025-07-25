using System;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Web.Models.MonitoringTestTaking
{
    public class ProctorViewModel
    {
        public int QTITestClassAssignmentID { get; set; }
        public string SchoolName { get; set; }
        public string ClassName { get; set; }
        public string TeacherName { get; set; }
        public string TestName { get; set; }
        public int VirtualTestId { get; set; }
        public bool? BranchingTest { get; set; }
        public string QuestionNumberLabel { get; set; }

        public string TestTitle
        {
            get { return SchoolName + ", " + ClassName + ", " + TeacherName + ", " + TestName; }
        }

        public List<ProctorViewStudent> Students { get; set; }

        public List<int?> QTIOnlineTestSessionIDs { get; set; }
        public string LastUpdated { get; set; }
    }
}
