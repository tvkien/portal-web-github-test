using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.TDLS
{
    public class TLDSReportDataViewModel
    {
        public int ProfileId { get; set; }
        public string ReportFileName { get; set; }
        public int TimezoneOffset { get; set; }
        public string DateFormat { get; set; }
        public string ProfileIdList { get; set; }
        public string ZipFileName { get; set; }
        public int? EnrollmentYear { get; set; }
        public int? CurrentUserId { get; set; }
        public int? CurrentUserDistrictId { get; set; }
        public bool? ShowArchived { get; set; }
        public string SortingColumns { get; set; }//the current sorted column of the datatable, it looks like 1,asc,0
    }
}
