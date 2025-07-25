using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class SpecializedReportJobViewModel
    {
        public int SpecializedReportJobId { get; set; }        
        public DateTime CreatedDate { get; set; }
        public int Status { get; set; }
        public string DownloadUrl { get; set; }
        public string CreatedDateString { get; set; }
        public int PercentCompleted { get; set; }        
        public int TotalItem { get; set; }
    }
}