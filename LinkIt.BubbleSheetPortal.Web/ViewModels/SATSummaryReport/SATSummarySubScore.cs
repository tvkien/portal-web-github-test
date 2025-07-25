using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.SATSummaryReport
{
    public class SATSummarySubScore
    {
        public string SectionName { get; set; }
        public decimal Score { get; set; }
        public string ScoreText
        {
            get { return Math.Round(Score).ToString(); }
        }
    }
}