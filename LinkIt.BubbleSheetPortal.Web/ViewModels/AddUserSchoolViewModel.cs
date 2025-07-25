using System.Collections.Generic;
using System.Web.Mvc;
using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class AddUserSchoolViewModel : ValidatableEntity<AddUserSchoolViewModel>, IUserViewModel
    {
        public int CurrentUserRoleId { get; set; }
        public int RoleId { get; set; }
        public int StateId { get; set; }
        public int? DistrictId { get; set; }
        public int SchoolId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public List<SelectListItem> AvailableStates { get; set; }
        public List<SelectListItem> AvailableDistricts { get; set; }
        public List<SelectListItem> AvailableSchools { get; set; }

        public bool CanSelectState
        {
            get
            {
                return !RoleId.Equals((int) Permissions.Teacher) && !RoleId.Equals((int) Permissions.SchoolAdmin);
            }
        }

        public bool CanSelectDistrict
        {
            get
            {
                return CanSelectState || CurrentUserRoleId.Equals((int)Permissions.DistrictAdmin);
            }
        }

        public AddUserSchoolViewModel()
        {
            AvailableStates = new List<SelectListItem>();
            AvailableDistricts = new List<SelectListItem>();
            AvailableSchools = new List<SelectListItem>();
        }
    }
}