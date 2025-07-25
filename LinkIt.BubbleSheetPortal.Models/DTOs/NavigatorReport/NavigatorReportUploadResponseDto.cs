using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport
{
    public class NavigatorReportUploadResponseDto
    {
        public bool IsSuccess { get; set; } = true;
        public string ExceptionMessage { get; set; }
    }
}
