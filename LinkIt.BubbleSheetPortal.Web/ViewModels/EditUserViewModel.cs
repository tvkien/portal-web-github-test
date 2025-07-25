using System.Collections.Generic;
using System.Web.Mvc;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class EditUserViewModel : ValidatableEntity<EditUserViewModel>, IUserViewModel
    {
        private string userName = string.Empty;
        private string emailAddress = string.Empty;
        private string firstName = string.Empty;
        private string lastName = string.Empty;
        private string phoneNumber = string.Empty;
        private string localCode = string.Empty;
        private string stateCode = string.Empty;

        public int UserId { get; set; }
        public int StateId { get; set; }
        public int? DistrictId { get; set; }
        public int SchoolId { get; set; }
        public int RoleId { get; set; }
        public int CurrentUserRoleId { get; set; }
        
        public List<UserSchool> Schools { get; set; }
        public List<SelectListItem> AvailableStates { get; set; }
        public List<SelectListItem> AvailableDistricts { get; set; }
        public List<SelectListItem> AvailableSchools { get; set; }

        public EditUserViewModel()
        {
            Schools = new List<UserSchool>();
            AvailableStates = new List<SelectListItem>();
            AvailableDistricts = new List<SelectListItem>();
            AvailableSchools = new List<SelectListItem>();
        }

        public bool CanSelectState
        {
            get
            {
                return CurrentUserRoleId.Equals((int)Permissions.LinkItAdmin) || CurrentUserRoleId.Equals((int)Permissions.Publisher)|| CurrentUserRoleId.Equals((int)Permissions.NetworkAdmin);
            }
        }

        public bool CanSelectDistrict
        {
            get
            {
                return CanSelectState || CurrentUserRoleId.Equals((int)Permissions.DistrictAdmin);
            }
        }

        public List<SelectListItem> AvailableRoles
        {
            get
            {
                var availableRoles = new List<SelectListItem>
                                         {
                                             new SelectListItem {Text = "Select Role", Value = "select"},
                                             new SelectListItem {Text = "Teacher", Value = "2"},
                                             new SelectListItem {Text = "School Admin", Value = "8"}
                                         };
                switch (CurrentUserRoleId)
                {
                    case (int)Permissions.DistrictAdmin:
                        availableRoles.Add(new SelectListItem { Text = "" + LabelHelper.DistrictLabel + " Admin", Value = "3" });
                        break;
                    case (int)Permissions.Publisher:
                        availableRoles.Add(new SelectListItem { Text = "" + LabelHelper.DistrictLabel + " Admin", Value = "3" });
                        availableRoles.Add(new SelectListItem { Text = "Network Admin", Value = "27" });
                        availableRoles.Add(new SelectListItem { Text = "Publisher", Value = "5" });
                        break;
                    case (int)Permissions.LinkItAdmin:
                        availableRoles.Add(new SelectListItem { Text = "" + LabelHelper.DistrictLabel + " Admin", Value = "3" });
                        availableRoles.Add(new SelectListItem { Text = "LinkIt Admin", Value = "15" });
                        availableRoles.Add(new SelectListItem { Text = "Network Admin", Value = "27" });
                        break;
                    case (int)Permissions.NetworkAdmin:
                        availableRoles.Add(new SelectListItem { Text = "" + LabelHelper.DistrictLabel + " Admin", Value = "3" });
                        availableRoles.Add(new SelectListItem { Text = "Network Admin", Value = ((int)Permissions.NetworkAdmin).ToString() });
                        break;
                }
                return availableRoles;
            }
        }

        public bool CanBeAssociatedWithSchool
        {
            get { return RoleId == (int)Permissions.SchoolAdmin || RoleId == (int)Permissions.Teacher; }
        }

        public string UserName
        {
            get { return userName; }
            set { userName = value.ConvertNullToEmptyString(); }
        }

        public string EmailAddress
        {
            get { return emailAddress; }
            set { emailAddress = value.ConvertNullToEmptyString(); }
        }

        public string FirstName
        {
            get { return firstName; }
            set { firstName = value.ConvertNullToEmptyString(); }
        }

        public string LastName
        {
            get { return lastName; }
            set { lastName = value.ConvertNullToEmptyString(); }
        }

        public string PhoneNumber
        {
            get { return phoneNumber; }
            set { phoneNumber = value.ConvertNullToEmptyString(); }
        }

        public string LocalCode
        {
            get { return localCode; }
            set { localCode = value.ConvertNullToEmptyString(); }
        }

        public string StateCode
        {
            get { return stateCode; }
            set { stateCode = value.ConvertNullToEmptyString(); }
        }
    }
}
