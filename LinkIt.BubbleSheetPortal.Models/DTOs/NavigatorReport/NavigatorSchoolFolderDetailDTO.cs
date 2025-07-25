using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport
{
    public class NavigatorSchoolFolderDetailDTO
    {
        public string ReportName { get; set; }
        public List<SchoolViewRolePublishingDesctiption> RoleDescriptions { get; set; }
    }
    public class SchoolViewRolePublishingDesctiption
    {
        public string RoleShortName { get; set; }
        public string RoleToolTip { get; set; }
        public int? PublishStatus { get; set; }
        public int RoleId { get; set; }
    }
}
