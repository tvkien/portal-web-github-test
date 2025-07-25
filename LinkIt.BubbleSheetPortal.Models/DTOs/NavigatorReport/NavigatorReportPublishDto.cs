using System;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport
{
    public class NavigatorReportPublishDto
    {
        public int UserID { get; set; }
        public bool IsPublished { get; set; }
        public DateTime? PublishFirstTime { get; set; }
        public DateTime? PublishTime { get; set; }
        public int PublisherId { get; set; }
        public string PublisherFullName { get; set; }
    }
}
