using System.Collections.Generic;
using System.Web.Mvc;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class BankPublishToSchoolViewModel
    {
        public int BankId { get; set; }
        public int StateId { get; set; }
        public int DistrictId { get; set; }
        public int SchoolId { get; set; }

        public List<SelectListItem> AvailableStates { get; set; }
        public List<SelectListItem> AvailableDistricts { get; set; }
        public List<SelectListItem> AvailableSchools { get; set; }

        public BankPublishToSchoolViewModel()
        {
            AvailableStates = new List<SelectListItem>();
            AvailableDistricts = new List<SelectListItem>();
            AvailableSchools = new List<SelectListItem>();
        }
        public bool IsPublisher { get; set; }
        public bool IsNetworkAdmin { get; set; }
    }
}
