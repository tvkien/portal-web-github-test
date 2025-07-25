using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.ACTReport
{
    public class ACTReportViewModel
    {
        public ACTReportViewModel()
        {
            IsAdmin = false;
            CanSelectTeachers = false;
            IsSchoolAdmin = false;
            IsPublisher = false;
            IsNetworkAdmin = false;
        }
        public bool IsAdmin { get; set; }
        public bool CanSelectTeachers { get; set; }
        public bool IsSchoolAdmin { get; set; }
        public bool IsPublisher { get; set; }
        public bool IsNetworkAdmin { get; set; }
        public List<SelectListItem> ReportTypes { get; set; }

        public bool IncludeStateInformation { get; set; }

        public string ResultDateFrom { get; set; }
        public string ResultDateTo { get; set; }
    }
}