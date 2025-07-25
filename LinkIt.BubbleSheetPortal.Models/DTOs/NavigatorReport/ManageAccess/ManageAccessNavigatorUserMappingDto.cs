using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport.ManageAccess
{
    public class ManageAccessNavigatorUserMappingDto
    {
        public int NavigatorReportID { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
    }
}
