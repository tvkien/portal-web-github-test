using System;
using System.Collections.Generic;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class AblesReportJob
    {

        public int AblesReportJobId { get; set; }
        public int ReportTypeId { get; set; }
        public int UserId { get; set; }
        public string LearningArea { get; set; }
        public int? SchoolId { get; set; }
        public int? ClassId { get; set; }
        public int? TeacherId { get; set; }
        public int? DistrictTermId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string DownloadUrl { get; set; }
        public int Status { get; set; }
        public int DistrictId { get; set; }
        public string ErrorMessage { get; set; }
        public string JsonAblesDataPost { get; set; }
    }
}