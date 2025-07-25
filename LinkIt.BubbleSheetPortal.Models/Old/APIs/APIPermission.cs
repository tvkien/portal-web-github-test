using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class APIPermission
    {
        public int APIPermissionId { get; set; }
        public int APIFunctionId { get; set; }
        public int TargetId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsAllow { get; set; }
        public int DistrictId { get; set; }
        public int APIAccountTypeId { get; set; }
    }
}
