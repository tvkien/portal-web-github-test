using System;

namespace LinkIt.BubbleSheetPortal.Models.ACTReport
{
    public class ACTTestHistoryData
    {
        public int TestResultID { get; set; }
        public int VirtualTestID { get; set; }
        public int StudentID { get; set; }
        public int TeacherID { get; set; }
        public DateTime UpdatedDate { get; set; }
        public decimal SectionScore { get; set; }
        public decimal CompositeScore { get; set; }
        public decimal SectionScoreRaw { get; set; }
        public string SectionName { get; set; }
        public int TestResultSubScoreID { get; set; }
        public string TestName { get; set; }
        public int ClassID { get; set; }
        public int VirtualTestSubTypeID { get; set; }
    }
}