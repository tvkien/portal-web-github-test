using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class CategoryTestRestrictAccess
    {
        public int CategoryTestId { set; get; }
        public int XLITeacherModuleRoleId { get; set; }
        public int XLISchoolAdminModuleRoleId { get; set; }
        public bool IsTeacherAccess { get; set; }
        public bool IsSchoolAdminAccess { get; set; }
        public int DistrictId { get; set; }
        public string DisplayName { get; set; }
        public string RestrictionTypeName { set; get; }
    }
}
