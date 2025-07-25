using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport
{
    public class NavigatorReportUploadFileResponseDto
    {
        public int NavigatorReportId { get; set; }
        public string FileName { get; set; }
        public string ReportTypeName { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }
}
