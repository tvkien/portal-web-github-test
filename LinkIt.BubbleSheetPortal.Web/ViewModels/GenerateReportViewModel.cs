using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class GenerateReportViewModel
    {
        public int? APIAccountID { get; set; }
        public string ReportType { get; set; }
        public int? TestResultId { get; set; }
        public string EnvId { get; set; }
    }
}
