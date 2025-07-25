using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Common;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.ACTReport
{
    public class ACTSummaryScoreViewModel
    {
        public List<SummaryScoreViewModel> SummaryScores { get; set; }

        public List<ScoreViewModel> CompositeScores { get; set; }
        public List<ScoreViewModel> EnglishScores { get; set; }
        public List<ScoreViewModel> MathScores { get; set; }
        public List<ScoreViewModel> ReadingScores { get; set; }
        public List<ScoreViewModel> ScienceScores { get; set; }

        public bool ShowChart
        {
            get { return CompositeScores.Count >= Constanst.ACTStudentReportMinAmountOfTestForChart; }
        }
    }
}