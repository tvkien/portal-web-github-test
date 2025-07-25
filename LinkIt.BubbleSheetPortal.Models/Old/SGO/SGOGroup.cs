namespace LinkIt.BubbleSheetPortal.Models.SGO
{
    public class SGOGroup
    {
        public int SGOGroupID { get; set; }
        public int SGOID { get; set; }
        public int Order { get; set; }
        public string Name { get; set; }
        public decimal? TargetScore { get; set; }

        // Custom properties
        public int StudentNumberInGroup { get; set; }

        public decimal? PercentStudentAtTargetScore { get; set; }
        public decimal? TeacherSGOScore { get; set; }
        public decimal? Weight { get; set; }
        public string TargetScoreCustom { get; set; }
        public string TeacherSGOScoreCustom { get; set; }

    }
}
