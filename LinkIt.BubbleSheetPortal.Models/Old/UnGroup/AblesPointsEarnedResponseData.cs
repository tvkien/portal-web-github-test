using System;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class AblesPointsEarnedResponseData
    {
        public int TestResultID { get; set; }
        public int VirtualTestID { get; set; }
        public int ClassID { get; set; }
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public string StudentCode { get; set; }
        public int AssessmentRoundID { get; set; }
        public DateTime ResultDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Answers { get; set; }
        public string AblesTestName { get; set; }
        public int ValueMapping { get; set; }
        public string ClassName { get; set; }
        public string StateCode { get; set; }
        public string SchoolCode { get; set; }
    }
}
