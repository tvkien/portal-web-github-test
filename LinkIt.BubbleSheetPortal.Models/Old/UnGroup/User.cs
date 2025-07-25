using System;
using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Models.Enum;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class User : ValidatableEntity<User>, IIdentifiable
    {
        private string name = string.Empty;
        private string emailAddress = string.Empty;
        private string userName = string.Empty;
        private string firstName = string.Empty;
        private string lastName = string.Empty;
        private string phoneNumber = string.Empty;
        private string localCode = string.Empty;
        private string stateCode = string.Empty;
        private string password = string.Empty;
        private string hashedPassword = string.Empty;
        private string passwordQuestion = string.Empty;
        private string passwordAnswer = string.Empty;
        private string privileges = string.Empty;
        private string hint = string.Empty;
        private string messageNumber = string.Empty;
        private List<int> listDistrictId = null;
        private string modifiedBy = string.Empty;
        public int Id { get; set; }
        public int? DistrictId { get; set; }
        public int? StateId { get; set; }
        public int RoleId { get; set; }
        public int? DistrictGroupId { get; set; }
        public int? UserStatusId { get; set; }
        public int? AddedByUserId { get; set; }
        public int? SchoolId { get; set; }
        public bool? ApiAccess { get; set; }
        public bool IsAdmin { get; set; }
        public bool? Active { get; set; }
        public bool HasTemporaryPassword { get; set; }
        public DateTime LastLoginDate { get; set; }
        public DateTime PasswordLastSetDate { get; set; }
        public DateTime? DateConfirmedActive { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedUser { get; set; }
        public DateTime? TermOfUseAccepted { get; set; }
        public string SISID { get; set; }

        public bool IsNetworkAdmin
        {
            get { return RoleId.Equals((int)Permissions.NetworkAdmin); }
        }

        public bool IsDistrictAdminOrPublisher
        {
            get
            {
                return RoleId.Equals((int)Permissions.DistrictAdmin)
                    || RoleId.Equals((int)Permissions.Publisher)
                    || RoleId.Equals((int)Permissions.NetworkAdmin);
            }
        }
        public bool IsPublisherOrNetworkAdmin
        {
            get
            {
                return new int[] { (int)RoleEnum.Publisher, (int)RoleEnum.NetworkAdmin }.Contains(RoleId);
            }
        }
        public bool IsParent
        {
            get
            {
                return RoleId.Equals((int)Permissions.Parent);
            }
        }

        public bool IsTeacher
        {
            get
            {
                return RoleId.Equals((int)Permissions.Teacher);
            }
        }

        public bool IsSchoolAdmin
        {
            get
            {
                return RoleId.Equals((int)Permissions.SchoolAdmin);
            }
        }

        public bool IsPublisher
        {
            get
            {
                return RoleId.Equals((int)Permissions.Publisher);
            }
        }

        public bool IsDistrictAdmin
        {
            get
            {
                return RoleId.Equals((int)Permissions.DistrictAdmin);
            }
        }
        public bool IsStudent
        {
            get
            {
                return RoleId.Equals((int)Permissions.Student);
            }
        }

        public string Name
        {
            get { return name; }
            set { name = value.ConvertNullToEmptyString(); }
        }

        public string EmailAddress
        {
            get { return emailAddress; }
            set { emailAddress = value.ConvertNullToEmptyString(); }
        }

        public string UserName
        {
            get { return userName; }
            set { userName = value.ConvertNullToEmptyString(); }
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

        public string PasswordQuestion
        {
            get { return passwordQuestion; }
            set { passwordQuestion = value.ConvertNullToEmptyString(); }
        }

        public string PasswordAnswer
        {
            get { return passwordAnswer; }
            set { passwordAnswer = value.ConvertNullToEmptyString(); }
        }

        public string Privileges
        {
            get { return privileges; }
            set { privileges = value.ConvertNullToEmptyString(); }
        }

        public string Hint
        {
            get { return hint; }
            set { hint = value.ConvertNullToEmptyString(); }
        }

        public string MessageNumber
        {
            get { return messageNumber; }
            set { messageNumber = value.ConvertNullToEmptyString(); }
        }

        public string ModifiedBy
        {
            get { return modifiedBy; }
            set { modifiedBy = value.ConvertNullToEmptyString(); }
        }

        //For Impersonate Update
        public int OriginalID { get; set; }
        public int OriginalDistrictId { get; set; }
        public int OriginalStateId { get; set; }
        public int OriginalRoleId { get; set; }
        public string OriginalName { get; set; }
        public string OriginalEmailAddress { get; set; }
        public string OriginalUsername { get; set; }
        public bool IsOrginalPublisher
        {
            get
            {
                return OriginalRoleId.Equals((int)Permissions.Publisher);
            }
        }
        public bool IsOrginalNetworkAdmin
        {
            get { return OriginalRoleId.Equals((int)Permissions.NetworkAdmin); }
        }
        private List<int> listOriginalDistrictId = null;
        //public List<int> ListOriginalDistrictId
        //{
        //    get
        //    {
        //        if (listOriginalDistrictId == null)
        //            return new List<int>();
        //        return listOriginalDistrictId;
        //    }
        //    set
        //    {
        //        listOriginalDistrictId = value;
        //    }
        //}
        public string OriginalDistrictLiCode { get; set; }
        public string ImpersonateLogActivity { get; set; }
        public string ImpersonatedSubdomain { get; set; }
        public int OriginalNetworkAdminDistrictId { get; set; }
        public string SessionCookieGUID { get; set; } //For ImpersonateLog

        // for ensuring single user sign on
        public DateTime ExpiredUTCDateTimeCookie { get; set; }
        public string GUIDSession { get; set; }
        public bool IsVDETUser { get; set; } //TODO: for PassThroughV-DET

        public string CKSession { get; set; }
        public UserMetaValue UserMetaValue { get; set; }

        //[Username], [RoleTitle], [Groupname.n1], [Groupname.n2].
        public string WelcomeMessage
        {
            get; set;
        }

        public string WalkmeSnippetURL { get; set; }

        public UserWelcomeInfo RoleAndGroupName { get; set; }

    }
}
