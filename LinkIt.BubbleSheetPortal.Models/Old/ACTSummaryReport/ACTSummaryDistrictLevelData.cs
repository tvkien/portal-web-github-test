using System;

namespace LinkIt.BubbleSheetPortal.Models.ACTSummaryReport
{
    public class ACTSummaryDistrictLevelData
    {
        public int SchoolID { get; set; }
        public string SchoolName { get; set; }
        public int StudentID { get; set; }
        public int? TestResultID { get; set; }
        public DateTime? ResultDate { get; set; }
        public decimal ScoreRaw { get; set; }
        public decimal ScoreScaled { get; set; }
        public string SectionName { get; set; }
    }
}