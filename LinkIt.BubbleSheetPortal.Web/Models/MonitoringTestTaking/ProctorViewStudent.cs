using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Web.Models.MonitoringTestTaking
{
    public class ProctorViewStudent
    {
        public int QTIOnlineTestSessionID { get; set; }
        public string TestStatus { get; set; }
        public int? StudentID { get; set; }
        public string StudentName { get; set; }
        public int Active { get; set; }
        public int AutoGrading { get; set; }
        public List<ProctorViewQuestion> Questions { get; set; }
        public bool GradingProcessSuccess { get; set; }
        public int GradingProcessStatus { get; set; }
    }
}