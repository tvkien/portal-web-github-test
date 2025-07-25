using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Web.ViewModels.ACTReport;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.SATReport
{
    public class SATSummaryScoreViewModel
    {
        public List<SummaryScoreViewModel> SummaryScores { get; set; }

        public List<ScoreViewModel> CompositeScores { get; set; }
        public List<SATScoreViewModel> SectionScores { get; set; }
        
        public bool ShowChart
        {
            get
            {
                if (CompositeScores != null)
                    return CompositeScores.Count >= Constanst.SATStudentReportMinAmountOfTestForChart;
                return false;
            }
        }
    }
}