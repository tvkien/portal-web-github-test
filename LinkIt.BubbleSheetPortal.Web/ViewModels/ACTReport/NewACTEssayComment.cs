using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.ACTReport
{
    public class NewACTEssayComment
    {
        public string TagName { get; set; }
        public int Score { get; set; }
        public string ScoreRange { get; set; }
        public string EssayComment { get; set; }
    }
}