using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Common;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.TDLS
{
    public class TLDSProfessionalServiceViewModel
    {
        public int? ProfessionalServiceId { get; set; }
        public int? ProfileId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string ContactPerson { get; set; }
        public string Position { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool? WrittenReportAvailable { get; set; }
        public DateTime? ReportForwardedToSchoolDate { get; set; }

        public string ReportForwardedToSchoolDateString
        {
            get { return ReportForwardedToSchoolDate.HasValue ? ReportForwardedToSchoolDate.Value.DisplayDateWithFormat() : ""; }
            set {
                if (!string.IsNullOrEmpty(value))
                {
                    DateTime date = DateTime.MinValue;
                    value.TryParseDateWithFormat(out date);
                    ReportForwardedToSchoolDate = date;
                } }
        }

        public bool? Attached { get; set; }
        public bool? AvailableUponRequested { get; set; }
        public string ReportForwardedToSchoolDateFormated { get; set; }
    }
} 
