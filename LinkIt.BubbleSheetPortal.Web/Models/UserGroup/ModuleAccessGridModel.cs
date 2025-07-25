using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.Models.UserGroup
{
    public class ModuleAccessGridModel
    {
        public string Module { get; set; }

        public string Area { get; set; }

        public string DistrictAccess { get; set; }

        public string UserGroupAccess { get; set; }

        public string CurrentUserAccessForGroup { get; set; }

    }
}
