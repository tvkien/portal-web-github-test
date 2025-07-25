using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class TestResultScoreViewModel
    {
        public int? TestResultScoreID { get; set; }
        public int? TestResultID { get; set; }
        public decimal? ScorePercent { get; set; }
        public decimal? ScorePercentage { get; set; }
        public decimal? ScoreRaw { get; set; }
        public decimal? ScoreLexile { get; set; }
        public decimal? ScoreIndex { get; set; }
        public decimal? ScoreScaled { get; set; }
        public bool? UsePercent { get; set; }
        public bool? UsePercentage { get; set; }
        public bool? UseRaw { get; set; }
        public bool? UseScaled { get; set; }
        public bool? UseLexile { get; set; }
        public bool? UseIndex { get; set; }
        public int? PointsPossible { get; set; }
        public int? AchievementLevel { get; set; }
        public bool? UseGradeLevelEquiv { get; set; }
        public decimal? ScoreGradeLevelEquiv { get; set; }
        public string Name { get; set; }
        public bool? MetStandard { get; set; }
        public string AchieveLevelName { get; set; }
        public decimal? ScoreCustomN_1 { get; set; }
        public decimal? ScoreCustomN_2 { get; set; }
        public decimal? ScoreCustomN_3 { get; set; }
        public decimal? ScoreCustomN_4 { get; set; }
        public string ScoreCustomA_1 { get; set; }
        public string ScoreCustomA_2 { get; set; }
        public string ScoreCustomA_3 { get; set; }
        public string ScoreCustomA_4 { get; set; }
    }
}