using System;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class TestLibraryExportData
    {
        public int VirtualTestID { get; set; }
        public string TestName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public int QuestionCount { get; set; }
        public int TotalPointsPossible { get; set; }
        public int TestResultCount { get; set; }
        public DateTime? EarliestResultDate { get; set; }
        public DateTime? MostRecentResultDate { get; set; }
        public string TestCategory { get; set; }
        public bool? InterviewStyleAssessment { get; set; }
        public string BankName { get; set; }
        public string BankGrade { get; set; }
        public string BankSubject { get; set; }
    }
}
