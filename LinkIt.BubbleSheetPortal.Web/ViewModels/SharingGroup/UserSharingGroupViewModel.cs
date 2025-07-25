using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.SharingGroup
{
    public class UserSharingGroupViewModel
    {
        public int CurrentUserId { get; set; }

        public bool IsPublisher { get; set; }

        public bool IsNetworkAdmin { get; set; }

        public string Name;

        public int? DistrictID;
        public int SharingGroupID { get; set; }
    }
}
