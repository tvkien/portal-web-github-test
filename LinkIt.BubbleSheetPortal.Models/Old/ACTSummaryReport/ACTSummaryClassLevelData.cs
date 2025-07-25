using System;

namespace LinkIt.BubbleSheetPortal.Models.ACTSummaryReport
{
    public class ACTSummaryClassLevelData
    {
        public string StudentFirstName { get; set; }
        public string StudentLastName { get; set; }
        public string StudentCode { get; set; }
        public int StudentID { get; set; }
        public int TestResultID { get; set; }
        public DateTime? ResultDate { get; set; }
        public int ClassID { get; set; }
        public decimal ScoreRaw { get; set; }
        public decimal ScoreScaled { get; set; }
        public string SectionName { get; set; }

        public string StudentDisplayName
        {
            get
            {
                return string.Format("{0}, {1} ({2})", StudentLastName, StudentFirstName, StudentCode);    
            }
        }
    }
}