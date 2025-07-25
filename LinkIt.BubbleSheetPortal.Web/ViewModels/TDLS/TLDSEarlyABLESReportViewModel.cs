using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Common;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.TDLS
{
    public class TLDSEarlyABLESReportViewModel
    {
        public int? EarlyABLESReportId { get; set; }
        public int? ProfileId { get; set; }
        public string ReportName { get; set; }
        public bool? LearningReadinessReportCompleted { get; set; }
        public DateTime? ReportDate { get; set; }
        public string ReportDateString
        {
            get { return ReportDate.HasValue ? ReportDate.Value.DisplayDateWithFormat() : ""; }
            set {
                if (!string.IsNullOrEmpty(value))
                {
                    DateTime date = DateTime.MinValue;
                    value.TryParseDateWithFormat(out date);
                    ReportDate = date;
                } }
        }
        public bool? AvailableOnRequest { get; set; }
        public string ReportDateFormated { get; set; }
    }
}
