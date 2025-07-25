using System.Collections.Generic;
using System.Web.Mvc;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class UploadRosterViewModel
    {
        public bool IsPublisherUploading { get; set; }
        public int StateId { get; set; }
        public int DistrictId { get; set; }
        public int RequestTypeId { get; set; }

        public List<SelectListItem> AvailableStates { get; set; }
        public List<SelectListItem> AvailableDistricts { get; set; }

        public UploadRosterViewModel()
        {
            AvailableStates = new List<SelectListItem>();
            AvailableDistricts = new List<SelectListItem>();
        }

        public List<SelectListItem> RequestTypes { get; set; }

        //public List<SelectListItem> RequestTypes
        //{
        //    get
        //    {
        //        return new List<SelectListItem>
        //            {
        //                new SelectListItem { Text = "Student Full Refresh Roster", Value = "0" },
        //                new SelectListItem { Text = "Student Add/Changes Only Roster", Value = "1" },
        //                new SelectListItem { Text = "Staff Roster", Value = "2" },
        //                new SelectListItem { Text = "Student Program Roster", Value = "3" }
        //            };
        //    }
        //}
    }
}
