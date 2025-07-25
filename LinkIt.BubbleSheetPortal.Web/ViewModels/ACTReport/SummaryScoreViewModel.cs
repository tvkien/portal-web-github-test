using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.ACTReport
{
    public class SummaryScoreViewModel
    {
        public string Subject { get; set; }
        public decimal Baseline { get; set; }
        public decimal Current { get; set; }
        public decimal Best { get; set; }
        public decimal Improvement { get; set; }

        public string BaselineString
        {
            get { return Math.Round(Baseline).ToString(); }
        }

        public string CurrentString
        {
            get { return Math.Round(Current).ToString(); }
        }

        public string BestString
        {
            get { return Math.Round(Best).ToString(); }
        }

        public string ImprovementString
        {
            get { return Math.Round(Improvement).ToString(); }
        }
    }
}