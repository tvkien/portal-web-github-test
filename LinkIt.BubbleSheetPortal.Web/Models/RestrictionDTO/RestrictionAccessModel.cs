using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.Models.RestrictionDTO
{
    public class RestrictionAccessModel
    {
        public bool AllowToPrint { get; set; }
        public bool AllowToReviewOnline { get; set; }
        public bool AllowToExport { get; set; }

        public RestrictionAccessModel()
        {
            AllowToPrint = true;
            AllowToReviewOnline = true;
            AllowToExport = true;
        }
    }
}
