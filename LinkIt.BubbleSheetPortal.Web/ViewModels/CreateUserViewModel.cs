using System.CodeDom;
using System.Collections.Generic;
using System.Web.Mvc;
using dotless.Core.Parser.Tree;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class CreateUserViewModel : ValidatableEntity<CreateUserViewModel>, IUserViewModel
    {
        private string userName = string.Empty;
        private string password = string.Empty;
        private string confirmPassword = string.Empty;
        private string firstName = string.Empty;
        private string lastName = string.Empty;
        private string phoneNumber = string.Empty;
        private string emailAddress = string.Empty;
        private string localCode = string.Empty;
        private string stateCode = string.Empty;

        public int UserId { get; set; }
        public int StateId { get; set; }
        public int? DistrictId { get; set; }
        public int SchoolId { get; set; }
        public int RoleId { get; set; }
        public int CurrentUserRoleId { get; set; }

        public List<SelectListItem> AvailableStates { get; set; }
        public List<SelectListItem> AvailableDistricts { get; set; }
        public List<SelectListItem> AvailableSchools { get; set; }

        public CreateUserViewModel()
        {
            AvailableStates = new List<SelectListItem>();
            AvailableDistricts = new List<SelectListItem>();
            AvailableSchools = new List<SelectListItem>();
        }

        public bool CanSelectState
        {
            get
            {
                return CurrentUserRoleId.Equals((int)Permissions.LinkItAdmin) 
                    || CurrentUserRoleId.Equals((int)Permissions.Publisher)
                    ||CurrentUserRoleId.Equals((int)Permissions.NetworkAdmin);
            }
        }

        public bool CanSelectDistrict
        {
            get
            {
                return CanSelectState || CurrentUserRoleId.Equals((int)Permissions.DistrictAdmin);
            }
        }

        private string NetworkAdminNumber = ((int)Permissions.NetworkAdmin).ToString();
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
                        availableRoles.Add(new SelectListItem { Text = "Network Admin", Value = NetworkAdminNumber });
                        availableRoles.Add(new SelectListItem { Text = "Publisher", Value = "5" });
                        break;
                    case (int)Permissions.LinkItAdmin:
                        availableRoles.Add(new SelectListItem { Text = "District Admin", Value = "3" });
                        availableRoles.Add(new SelectListItem { Text = "Network Admin", Value = NetworkAdminNumber });
                        availableRoles.Add(new SelectListItem { Text = "Publisher", Value = "5" });
                        availableRoles.Add(new SelectListItem { Text = "LinkIt Admin", Value = "15" });
                        break;
                    case (int)Permissions.NetworkAdmin:
                        availableRoles.Add(new SelectListItem { Text = "" + LabelHelper.DistrictLabel + " Admin", Value = "3" });
                        availableRoles.Add(new SelectListItem { Text = "Network Admin", Value = NetworkAdminNumber });
                        break;
                }
                return availableRoles;
            }
        }

        public string UserName
        {
            get { return userName; }
            set { userName = value.ConvertNullToEmptyString(); }
        }

        public string Password
        {
            get { return password; }
            set { password = value.ConvertNullToEmptyString(); }
        }

        public string ConfirmPassword
        {
            get { return confirmPassword; }
            set { confirmPassword = value.ConvertNullToEmptyString(); }
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

        public string EmailAddress
        {
            get { return emailAddress; }
            set { emailAddress = value.ConvertNullToEmptyString(); }
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
        public bool IsPublisher
        {
            get { return CurrentUserRoleId == (int) Permissions.Publisher; }
        }
        public bool IsNetworkAdmin
        {
            get { return CurrentUserRoleId == (int)Permissions.NetworkAdmin; }
        }
    }
}
