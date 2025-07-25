using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.SGO
{
    public class SGOReportDataViewModel
    {
        public int SgoId { get; set; }
        public string ReportFileName { get; set; }
        public int TimezoneOffset { get; set; }
        public string DateFormat { get; set; }
    }
}