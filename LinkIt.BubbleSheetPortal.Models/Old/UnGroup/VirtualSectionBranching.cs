using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class VirtualSectionBranching
    {
        public int VirtualSectionBranchingID { get; set; }
        //public int? VirtualSectionID { get; set; }
        public int? VirtualTestID { get; set; }
        public string TestletPath { get; set; }
        public decimal? LowScore { get; set; }
        public decimal? HighScore { get; set; }
        public int? TargetVirtualSectionID { get; set; }
        //public bool? IsDefault { get; set; }
        public int? CreatedUserID { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public bool? IsBranchBySectionScore { get; set; }
    }
}
