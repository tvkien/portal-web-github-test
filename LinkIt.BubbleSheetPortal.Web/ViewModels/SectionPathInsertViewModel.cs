using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class SectionPathInsertViewModel
    {        
        public string sectionselected { get; set; }
        public int VirtualTestId { get; set; }
        public int TargetId { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public bool IsBranchBySectionScore { get; set; }

    }    
}
