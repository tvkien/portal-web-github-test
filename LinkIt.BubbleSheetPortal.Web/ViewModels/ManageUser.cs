using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class ManageUser
    {
        public int CurrentUserId { get; set; }
        public int RoleId { get; set; }
        public int DistrictId { get; set; }
        public int SchoolId { get; set; }
    }
}