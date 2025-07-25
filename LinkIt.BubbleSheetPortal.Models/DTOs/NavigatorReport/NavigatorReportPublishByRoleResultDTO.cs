using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport
{
    public class NavigatorReportPublishByRoleResultDTO
    {
        public int TotalRelatedReportCount { get; set; }
        public BaseResponseModel<bool> SendNotifyEmailResult { get; set; }
    }
}
