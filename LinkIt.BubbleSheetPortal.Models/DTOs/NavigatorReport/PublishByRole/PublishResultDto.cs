using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport.PublishByRole
{
    public class PublishResultDto
    {
        public int ReportCount { get; set; }
        public NewPublishRecordInformation NewPublishRecordInformation { get; set; }
        public string InitiatorEmail { get; set; }
        public string InitiatorName { get; set; }
    }
}
