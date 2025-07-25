using System;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class TestResultScore
    {
        public int TestResultScoreID { get; set; }
        public int TestResultID { get; set; }
        public int? PointsPossible { get; set; }
        public decimal? ScorePercent { get; set; }
        public decimal? ScoreRaw { get; set; }
        public decimal? ScoreScaled { get; set; }
        public decimal? ScoreLexile { get; set; }
        public decimal? ScoreIndex { get; set; }
        public decimal? ScorePercentage { get; set; }
        public int? AchievementLevel { get; set; }
        public decimal? ScoreCustomN_1 { get; set; }
        public decimal? ScoreCustomN_2 { get; set; }
        public decimal? ScoreCustomN_3 { get; set; }
        public decimal? ScoreCustomN_4 { get; set; }
        public string ScoreCustomA_1 { get; set; }
        public string ScoreCustomA_2 { get; set; }
        public string ScoreCustomA_3 { get; set; }
        public string ScoreCustomA_4 { get; set; }
        public bool? UsePercent { get; set; }
        public bool? UsePercentage { get; set; }
        public bool? UseRaw { get; set; }
        public bool? UseScaled { get; set; }
    }
}
