using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class SectionBranchingViewModel
    {
        public int VirtualSectionBranchingID { get; set; }
        public string TestletPath { get; set; }
        public int LowScore { get; set; }
        public int HighScore { get; set; }
        public string TargetVirtualSection { get; set; }        
    }
}