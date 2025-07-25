using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.SharingGroup
{
    public class UserInSharingGroupViewModel
    {
        public int CurrentUserId { get; set; }
        public bool IsPublisher { get; set; }
        public bool IsNetworkAdmin { get; set; }
        public string Name { get; set; }
        public int? DistrictID { get; set; }
        public int? SharingGroupID { get; set; }
        public int CreatedBySharingGroup { get; set; }
        public bool IsOwner { get; set; }
        public bool ShowInactiveSharingGroup { get; set; }
        public int? TabActive { get; set; } = 1;
    }
}
