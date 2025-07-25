using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport
{
    public class NavigatorReportFullDto
    {
        public int NavigatorReportID { get; set; }
        public string S3FileFullName { get; set; }
        public string Year { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public DateTime? PublishedDate { get; set; }
        public int SchoolId { get; set; }
        public string KeywordIDs { get; set; }
        public int NavigatorConfigurationID { get; set; }
        public int? ReportingPeriodID { get; set; }
        public int SchoolID { get; set; }
        public int DistrictID { get; set; }
        public int? CreatedBy { get; set; }
        public string Status { get; set; }
    }
}
