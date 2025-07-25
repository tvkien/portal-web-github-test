using System;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport
{
    public class NavigatorReportDTO
    {
        public int NavigatorReportID { get; set; }
        public string Year { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? PublishedDate { get; set; }
        public string NavigatorCategory { get; set; }
        public string ReportingPeriod { get; set; }
        public string School { get; set; }
        public int SchoolID { get; set; }
        public int DistrictID { get; set; }
    }
}
