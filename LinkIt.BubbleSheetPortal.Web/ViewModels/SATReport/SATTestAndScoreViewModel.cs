using System;
using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Common;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.SATReport
{
    public class SATTestAndScoreViewModel
    {
        public int TestResultID { get; set; }
        public DateTime TestDate { get; set; }
        public bool IsSelected { get; set; }
        public string TestName { get; set; }

        public List<SATSubScore> SubScores { get; set; }

        public decimal CompositeScore { get; set; }        
        
        public string CompositeScoreText
        {
            get { return Math.Round(CompositeScore).ToString(); }
        }

        public string TestDateText
        {
            get { return TestDate.DisplayDateWithFormat(); }
        }
    }
}