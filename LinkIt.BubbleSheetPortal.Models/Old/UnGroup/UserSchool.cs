using System;
using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class UserSchool : ValidatableEntity<UserSchool>
    {
        private string userName = string.Empty;
        private string firstName = string.Empty;
        private string lastName = string.Empty;
        private string districtName = string.Empty;
        private string stateName = string.Empty;
        private string schoolName = string.Empty;
        private string roleName = string.Empty;

        public int? UserSchoolId { get; set; }
        public int UserId { get; set; }
        public Permissions Role { get; set; }
        public int? UserStatusId { get; set; }
        public int? SchoolId { get; set; }
        public int? DistrictId { get; set; }
        public int? SchoolCount { get; set; }
        public bool InActive { get; set; }
        public DateTime DateActive { get; set; }

        public string DisplayName {
            get
            {
                var names = new List<string>()
                    {
                        lastName,
                        firstName
                    };

                var eNames = names.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

                return string.Format("{0} ({1})", string.Join(", ", eNames), userName);
            }
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

        public string DistrictName
        {
            get { return districtName; }
            set { districtName = value.ConvertNullToEmptyString(); }
        }

        public string StateName
        {
            get { return stateName; }
            set { stateName = value.ConvertNullToEmptyString(); }
        }

        public string SchoolName
        {
            get { return schoolName; }
            set { schoolName = value.ConvertNullToEmptyString(); }
        }

        public string RoleName
        {
            get { return roleName; }
            set { roleName = value.ConvertNullToEmptyString(); }
        }
    }
}
