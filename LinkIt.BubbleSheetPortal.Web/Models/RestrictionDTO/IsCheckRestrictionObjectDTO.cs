using LinkIt.BubbleSheetPortal.Models.RestrictionDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.Models.RestrictionDTO
{
    public class IsCheckRestrictionObjectDTO
    {
        public string ModuleCode { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public int DistrictId { get; set; }
        public int BankId { get; set; }
        public int TestId { get; set; }
        public RestrictionObjectType ObjectType { get; set; }
        public bool IsExport { get; set; } = false;
    }
}
