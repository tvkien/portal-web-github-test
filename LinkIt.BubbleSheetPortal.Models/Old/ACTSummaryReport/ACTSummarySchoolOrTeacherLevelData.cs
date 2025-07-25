namespace LinkIt.BubbleSheetPortal.Models.ACTSummaryReport
{
    public class ACTSummarySchoolOrTeacherLevelData
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public string TeacherName { get; set; }
        public string DistrictTermName { get; set; }
        public int VirtualTestId { get; set; }
        public string TestName { get; set; }
        public string SectionName { get; set; }        
        public decimal ScoreRaw { get; set; }
        public decimal ScoreScaled { get; set; }
        public int StudentNo { get; set; }
    }
}