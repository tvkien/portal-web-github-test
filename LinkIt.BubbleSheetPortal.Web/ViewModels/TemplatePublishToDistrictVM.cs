using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class TemplatePublishToDistrictVM
    {
        public TemplatePublishToDistrictVM()
        {
            AvailableStates = new List<SelectListItem>();
            AvailableDistricts = new List<SelectListItem>();
        }

        public int TemplateId { get; set; }
        public int StateId { get; set; }
        public int DistrictId { get; set; }

        public List<SelectListItem> AvailableStates { get; set; }
        public List<SelectListItem> AvailableDistricts { get; set; }

        public bool IsPublisher { get; set; }
        public bool IsNetworkAdmin { get; set; }
    }
}