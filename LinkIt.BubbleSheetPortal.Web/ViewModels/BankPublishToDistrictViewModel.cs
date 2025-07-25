using System.Collections.Generic;
using System.Web.Mvc;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class BankPublishToDistrictViewModel //: ValidatableEntity<BankPublishToDistrictViewModel>
    {
        public int BankId { get; set; }
        public int StateId { get; set; }
        public int DistrictId { get; set; }

        public List<SelectListItem> AvailableStates { get; set; }
        public List<SelectListItem> AvailableDistricts { get; set; }

        public BankPublishToDistrictViewModel()
        {
            AvailableStates = new List<SelectListItem>();
            AvailableDistricts = new List<SelectListItem>();
        }
        public bool IsPublisher { get; set; }
        public bool IsNetworkAdmin { get; set; }
    }
}