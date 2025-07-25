using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Web.ViewModels.ACTReport;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.SATReport
{
    public class SATScoreViewModel
    {
        public string SectionName { get; set; }
        public List<ScoreViewModel> SectionScores { get; set; }
    }
}