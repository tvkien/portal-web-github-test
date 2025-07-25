using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.SSO
{
    public class SSOUserDistrictRole
    {
        public string UserName { get; set; }
        public List<DistrictRole> ListDistrictRoles { get; set; }
    }
}
