using System.Collections.Generic;
using System.Web.Mvc;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class QtiBankPublishToSchoolViewModel : ValidatableEntity<QtiBankPublishToSchoolViewModel>
    {
        public int StateId { get; set; }
        public int DistrictId { get; set; }
        public int SchoolId { get; set; }

        public List<SelectListItem> AvailableStates { get; set; }
        public List<SelectListItem> AvailableDistricts { get; set; }
        public List<SelectListItem> AvailableSchools { get; set; }

        public QtiBankPublishToSchoolViewModel()
        {
            AvailableStates = new List<SelectListItem>();
            AvailableDistricts = new List<SelectListItem>();
            AvailableSchools = new List<SelectListItem>();
        }
    }
}