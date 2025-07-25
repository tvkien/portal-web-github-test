using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Old.SharingGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.SharingGroup
{
    public class AssignUserSharingGroupViewModel
    {
        public SharingGroupDto SharingGroup { get; set; }
        public List<ListItem> Roles { get; set; }
        public List<ListItem> Schools { get; set; }
    }
}
