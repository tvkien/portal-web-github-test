using System.Collections.Generic;
using System.Web.Mvc;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public interface IUserViewModel
    {
        int UserId { get; set; }
        int StateId { get; set; }
        int? DistrictId { get; set; }
        int SchoolId { get; set; }
        int RoleId { get; set; }
        int CurrentUserRoleId { get; set; }

        List<SelectListItem> AvailableStates { get; set; }
        List<SelectListItem> AvailableDistricts { get; set; }
        List<SelectListItem> AvailableSchools { get; set; }
    }
}
