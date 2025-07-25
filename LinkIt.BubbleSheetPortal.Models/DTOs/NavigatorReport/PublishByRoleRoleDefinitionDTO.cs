using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport
{
    public class PublishByRoleRoleDefinitionDTO
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }

        public string[] ReportTypesThatCanPublish { get; set; }
    }
}
