using System.Collections.Generic;
using System.Web.Mvc;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Web.Helpers;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class UserImpersonationViewModel : ValidatableEntity<UserImpersonationViewModel>
    {
        private string username = string.Empty;

        public int StateId { get; set; }
        public int DistrictId { get; set; }
        public int RoleId { get; set; }
        public List<SelectListItem> AvailableStates { get; set; }
        public List<SelectListItem> AvailableDistricts { get; set; }
        public List<SelectListItem> AvailableSchools { get; set; }


        public UserImpersonationViewModel()
        {
            AvailableStates = new List<SelectListItem>();
            AvailableDistricts = new List<SelectListItem>();
            AvailableSchools = new List<SelectListItem>();
            AvailableMemberStates = new List<SelectListItem>();
            AvailableMemberDistricts = new List<SelectListItem>();
            SchoolDistricts  =new List<SelectListItem>();
        }

        public string UserName
        {
            get { return username; }
            set { username = value.ConvertNullToEmptyString(); }
        }

        public List<SelectListItem> AvailableRoles
        {
            get
            {
                var listAvailableRoles = new List<SelectListItem>
                                        {
                                            new SelectListItem { Text = "Select Role", Value = "select" }
                                        };
                listAvailableRoles.AddRange(Util.GetListAvailableByRoleID(RoleId));

                return listAvailableRoles;
            }
        }
        public int? MemberStateId { get; set; }
        public int? MemberDistrictId { get; set; }
        public List<SelectListItem> AvailableMemberStates { get; set; }
        public List<SelectListItem> AvailableMemberDistricts { get; set; }
        public int SchoolID { get; set; }
        public List<SelectListItem> SchoolDistricts { get; set; }
    }
}
