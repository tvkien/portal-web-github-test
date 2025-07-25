using System;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class SpecializedReportJob
    {
        public int SpecializedReportJobId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string DownloadUrl { get; set; }
        public int Status { get; set; }
        public int? TotalItem { get; set; }
        public int? GeneratedItem { get; set; }       
        public int DistrictId { get; set; }
    }
}
