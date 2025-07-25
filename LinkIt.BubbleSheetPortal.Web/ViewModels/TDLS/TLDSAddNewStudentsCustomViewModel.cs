using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.TDLS
{
    public class TLDSAddNewStudentsCustomViewModel
    {
        public int ProfileId { get; set; }
        public int DistrictID { get; set; }

        public bool IsPublisherOrNetworkAdmin { get; set; }
        public bool IsPublisher { get; set; }
        public bool IsNetworkAdmin { get; set; }
        public string SubmittedStudentName { get; set; }
        public string DOBString { get; set; }
        public string Gender { get; set; }
    }
}