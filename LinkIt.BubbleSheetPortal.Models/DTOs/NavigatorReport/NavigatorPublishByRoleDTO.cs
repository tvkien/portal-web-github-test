using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport
{
    public class NavigatorPublishByRoleDTO
    {
        public int[] RoleIds { get; set; }
        public string[] NodePaths { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public int? DistrictId { get; set; }
        public bool AlsoSendEmail { get; set; }
        public string[] EmailTo { get; set; }
        public string[] EmailCC { get; set; }
        public string CustomNote { get; set; }
        public string[] NotSendingTo { get; set; }
        public bool IsNotSendingTo { get; set; }
        public string ExcludeUserIds { get; set; }
        public string GeneralUrl { get; set; }
    }
}
