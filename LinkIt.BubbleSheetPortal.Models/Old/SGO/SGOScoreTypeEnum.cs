using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.SGO
{
    public enum SGOScoreTypeEnum
    {
        [Description("Score Raw")]
        ScoreRaw = 1,
        [Description("Score Scaled")]
        ScoreScaled = 2,
        [Description("Score Percentage")]
        ScorePercentage = 3,

        [Description("Score Percent")]
        ScorePercent = 4,
        [Description("Score CustomN 1")]
        ScoreCustomN1 = 5,
        [Description("Score CustomN 2")]
        ScoreCustomN2 = 6,
        [Description("Score CustomN 3")]
        ScoreCustomN3 = 7,
        [Description("Score CustomN 4")]
        ScoreCustomN4 = 8,

        [Description("Score CustomA 1")]
        ScoreCustomA1 = 9,
        [Description("Score CustomA 2")]
        ScoreCustomA2 = 10,
        [Description("Score CustomA 3")]
        ScoreCustomA3 = 11,
        [Description("Score CustomA 4")]
        ScoreCustomA4 = 12,
        [Description("Score Index")]
        ScoreIndex = 13,
        [Description("Score Lexile")]
        ScoreLexile = 14
    }

    public enum DataLockerScoreTypeEnum
    {
        Raw = 1,
        Scaled = 2,
        Percentile = 3,
        Percent = 4,
        CustomN_1 = 5,
        CustomN_2 = 6,
        CustomN_3 = 7,
        CustomN_4 = 8,
        CustomA_1 = 9,
        CustomA_2 = 10,
        CustomA_3 = 11,
        CustomA_4 = 12,
        UseIndex = 13,
        UseLexile = 14
    }
}
