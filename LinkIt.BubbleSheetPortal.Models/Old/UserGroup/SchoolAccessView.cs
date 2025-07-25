using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.Old.UserGroup
{
    public class SchoolAccess
    {
        public int SchoolID { get; set; }

        public string SchoolName { get; set; }

        public string RoleAccess { get; set; }
    }

    public class SchoolAccessView
    {
        public IEnumerable<SchoolAccess> SchoolAccesses { get; set; }
    }
}
