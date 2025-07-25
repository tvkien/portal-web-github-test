using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class APIAccountViewModel
    {
        public int DistrictId { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public int TimezoneOffset { get; set; }
    }
}
