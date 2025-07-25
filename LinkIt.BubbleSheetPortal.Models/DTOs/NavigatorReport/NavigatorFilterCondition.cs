using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport
{
    public class NavigatorFilterCondition
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public int? StateId { get; set; }
        public int? DistrictId { get; set; }
        public string ReportTypeIds { get; set; }
        public string SchoolIds { get; set; }
        public string KeywordIds { get; set; }
        public string ReportingPeriodIds { get; set; }
        public string Years { get; set; }
        
    }
}
