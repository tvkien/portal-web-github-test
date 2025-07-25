using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.SGO
{
    public class ScoringDetailModel
    {
        public List<SGOScoringDetail> SgoScoringDetails { get; set; }
        public int TargetScoreType { get; set; }

        public string PreAssessmentTestName { get; set; }
        public string PostAssessmentTestName { get; set; }

        public int GroupCount { get; set; }
        public bool IsTemporaryScoring { get; set; }
    }
}