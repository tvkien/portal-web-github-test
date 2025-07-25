using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.SATReport
{
    public class SATSubScore
    {
        public string SectionName { get; set; }
        public decimal Score { get; set; }
        public string ScoreText
        {
            get { return (Score).ToString("#.##"); }
        }

        public decimal ScoreRaw { get; set; }
        public string ScoreRawText
        {
            get { return (ScoreRaw).ToString("#.##"); }
        }

        public int IsCorrectNo { get; set; }
        public int IsIncorrectNo { get; set; }
        public int IsBlankNo { get; set; }
    }
}