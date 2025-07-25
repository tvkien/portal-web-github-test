using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport
{
    public class NavigatorRecordExistResultDto
    {
        public string HasCode { get; set; }
        public int School { get; set; }
        public int District { get; set; }
        public string SchoolYear { get; set; }
        public int ReportType { get; set; }
        public string ReportSuffix { get; set; }
        public int ReportingPeriod { get; set; }
        public string KeywordShortNames { get; set; }
    }
}
