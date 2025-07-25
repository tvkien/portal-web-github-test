using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.TLDSDigital
{
    public class TldsDigitalReportViewModel
    {
        public int ProfileId { get; set; }
        public string ReportFileName { get; set; }
        public int TimezoneOffset { get; set; }
        public string DateFormat { get; set; }
        public string ProfileIdList { get; set; }
        public string ZipFileName { get; set; }
        public int? EnrollmentYear { get; set; }
        public int? CurrentUserDistrictId { get; set; }
        public bool? ShowArchived { get; set; }
        public Guid TldsProfileLinkId { get; set; }
    }
}
