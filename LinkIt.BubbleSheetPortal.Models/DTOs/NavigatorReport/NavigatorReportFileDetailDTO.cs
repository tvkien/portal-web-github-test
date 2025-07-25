using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport
{
    public class NavigatorReportFileDetailDTO
    {
        public string OtherKeywords { get; set; }
        public string PrimaryKeyword { get; set; }
        public List<NavigatorReportFileDetailRoleListDTO> FilePublishStatus { get; set; }
        public bool HidePublishToSection { get; set; }
        public bool KeywordMandatory { get; set; }
    }
    public class NavigatorReportFileDetailRoleListDTO
    {
        public string RoleName { get; set; }
        public int Count { get; set; }
        public int Published { get; set; }
        public bool CanPublish { get; set; }
        public int RoleId { get; set; }
    }
}
