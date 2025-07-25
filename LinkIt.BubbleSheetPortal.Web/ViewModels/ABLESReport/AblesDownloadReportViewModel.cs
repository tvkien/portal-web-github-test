using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.ABLESReport
{
    public class AblesDownloadReportViewModel
    {
        public DateTime ResultDateFrom { get; set; }
        public DateTime ResultDateTo { get; set; }
        public int? StateId { get; set; }
        public int? DistrictId { get; set; }

        public bool IsPublisher { get; set; }
        public bool IsNetworkAdmin { get; set; }
    }
}