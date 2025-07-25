using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class DistrictMemberViewModel
    {
        public string State { get; set; } 
        public string Name { get; set; }
        public string LiCode { get; set; }
        public int DistrictId { get; set; }
        public int StateId { get; set; }
    }
}