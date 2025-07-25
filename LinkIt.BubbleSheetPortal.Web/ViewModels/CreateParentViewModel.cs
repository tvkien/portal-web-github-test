using System.Collections.Generic;
using System.Web.Mvc;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class CreateParentViewModel : ValidatableEntity<CreateParentViewModel>, IUserViewModel
    {
        #region Implementation of IUserViewModel

        public int UserId { get; set; }
        public int StateId { get; set; }
        public int? DistrictId { get; set; }
        public int SchoolId { get; set; }
        public int RoleId { get; set; }
        public int CurrentUserRoleId { get; set; }
        public List<SelectListItem> AvailableStates { get; set; }
        public List<SelectListItem> AvailableDistricts { get; set; }
        public List<SelectListItem> AvailableSchools { get; set; }

        #endregion

        public CreateParentViewModel()
        {
            AvailableStates = new List<SelectListItem>();
            AvailableDistricts = new List<SelectListItem>();
            AvailableSchools = new List<SelectListItem>();
        }

        public bool CanSelectState
        {
            get
            {
                return CurrentUserRoleId.Equals((int)Permissions.LinkItAdmin) || CurrentUserRoleId.Equals((int)Permissions.Publisher);
            }
        }

        public bool CanSelectDistrict
        {
            get
            {
                return CanSelectState;
            }
        }

        private string userName = string.Empty;
        private string emailAddress = string.Empty;
        private string firstName = string.Empty;
        private string lastName = string.Empty;
        private string phoneNumber = string.Empty;
        private string messageNumber = string.Empty;
        private string password = string.Empty;
        private string hashedPassword = string.Empty;

        public string RandomPassword { get; set; }

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

        public string MessageNumber
        {
            get { return messageNumber; }
            set { messageNumber = value.ConvertNullToEmptyString(); }
        }

        public string Password
        {
            get { return password; }
            set { password = value.ConvertNullToEmptyString(); }
        }

        public string HashedPassword
        {
            get { return hashedPassword; }
            set { hashedPassword = value.ConvertNullToEmptyString(); }
        }
    }
}