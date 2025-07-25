namespace LinkIt.BubbleSheetPortal.Models.SGO
{
    public class SGOStudentScoreInDataPoint
    {
        public int StudentID { get; set; }
        public decimal ScoreRaw { get; set; }
        public decimal ScoreScaled { get; set; }
        public decimal ScoreLexile { get; set; }
        public decimal ScorePercent { get; set; }
        public int? AchievementLevel { get; set; }
        public decimal TotalPointPossible { get; set; }
        public int TotalQuestion { get; set; }
        public string ScoreText { get; set; }
        public decimal ScorePercentage { get; set; }
        public decimal ScoreIndex { get; set; }
        public decimal ScoreCustomN_1 { get; set; }
        public decimal ScoreCustomN_2 { get; set; }
        public decimal ScoreCustomN_3 { get; set; }
        public decimal ScoreCustomN_4 { get; set; }
    }
}
