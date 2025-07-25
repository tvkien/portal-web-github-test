using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class GenerateReportResultViewModel
    {
        public bool IsSuccess { get; set; }
        public string ReportUrl { get; set; }
        public string ErrorMessage { get; set; }
    }
}
