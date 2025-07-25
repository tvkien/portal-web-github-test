using LinkIt.BubbleSheetPortal.Models.DTOs.DistrictDecode;
using System.Collections.Generic;
using System.Web.Mvc;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class TestDataUploadViewModel
    {
        public bool IsPublisherUploading { get; set; }
        public int StateId { get; set; }
        public int DistrictId { get; set; }
        public int AchievementLeveIdSelected { get; set; }

        public bool IncludeProgram { get; set; }

        public List<SelectListItem> AvailableStates { get; set; }
        public List<SelectListItem> AvailableDistricts { get; set; }

        public List<SelectListItem> AchievementLeveIds { get; set; }

        public List<AttendanceCustom> ListAttendance { get; set; }

        public TestDataUploadViewModel()
        {
            AvailableStates = new List<SelectListItem>();
            AvailableDistricts = new List<SelectListItem>();
            AchievementLeveIds = new List<SelectListItem>();
            ListAttendance = new List<AttendanceCustom>();
        }

         
    }
}
