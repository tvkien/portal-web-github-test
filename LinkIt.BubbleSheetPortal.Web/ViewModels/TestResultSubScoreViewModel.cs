using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class TestResultSubScoreViewModel
    {
        public int? TestResultSubScoreID { get; set; }
        public int? TestResultScoreID { get; set; } 
        public decimal? ScorePercentSub { get; set; }
        public decimal? ScorePercentageSub { get; set; }
        public decimal? ScoreRawSub { get; set; }
        public decimal? ScoreScaledSub { get; set; }
        public decimal? ScoreLexileSub { get; set; }
        public bool? UsePercentSub { get; set; }
        public bool? UsePercentageSub { get; set; }
        public bool? UseRawSub { get; set; }
        public bool? UseScaledSub { get; set; }
        public bool? UseLexileSub { get; set; }
        public int? PointsPossibleSub { get; set; }
        public int? AchievementLevelSub { get; set; }
        public bool? UseGradeLevelEquivSub { get; set; }
        public decimal? ScoreGradeLevelEquivSub { get; set; }
        public string NameSub { get; set; }
        public bool? MetStandardSub { get; set; }
        public decimal? ScoreCustomN_1Sub { get; set; }
        public decimal? ScoreCustomN_2Sub { get; set; }
        public decimal? ScoreCustomN_3Sub { get; set; }
        public decimal? ScoreCustomN_4Sub { get; set; }
        public string ScoreCustomA_1Sub { get; set; }
        public string ScoreCustomA_2Sub { get; set; }
        public string ScoreCustomA_3Sub { get; set; }
        public string ScoreCustomA_4Sub { get; set; }
    }
}