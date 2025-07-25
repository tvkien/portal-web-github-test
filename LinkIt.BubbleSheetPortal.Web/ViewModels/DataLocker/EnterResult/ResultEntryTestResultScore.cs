using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.DataLocker.EnterResult
{
    public class ResultEntryTestResultScore
    {
        public int StudentID { get; set; }
        public int? VirtualTestID { get; set; }
        public int? ClassID { get; set; }
        public int TestResultID { get; set; }
        public int? TestResultScoreID { get; set; }
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
        public bool? UseArtifact { get; set; }
        public string ArtifactFileName { get; set; }
        public string ArtifactUrl { get; set; }
        public bool IsUrl { get; set; }
        public int LineIndex { get; set; }
    }
}
