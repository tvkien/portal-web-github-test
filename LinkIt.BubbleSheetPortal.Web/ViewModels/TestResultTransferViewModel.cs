using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class TestResultTransferViewModel
    {
        public bool IsAdmin { get; set; }
        public bool CanSelectTeachers { get; set; }
        public bool IsSchoolAdmin { get; set; }
        public bool IsPublisher { get; set; }
        public int DistrictId { get; set; }
        public bool IsTeacher{get; set;}
        public bool IsNetworkAdmin { get; set; }
    }
}