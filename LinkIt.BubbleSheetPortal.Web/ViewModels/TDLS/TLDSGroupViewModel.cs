using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.TDLS
{
    public class TLDSGroupViewModel
    {
        public int TLDSGroupID { get; set; }

        public string GroupName { get; set; }

        public int NumberOfProfile { get; set; }

        public bool Status { get; set; }

        public string GroupStatus { get; set; }
    }
}
