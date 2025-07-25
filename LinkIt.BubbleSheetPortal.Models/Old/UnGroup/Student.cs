using System;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class Student : ValidatableEntity<Student>
    {
        private string firstName = string.Empty;
        private string middleName = string.Empty;
        private string lastName = string.Empty;
        private string code = string.Empty;
        private string altCode = string.Empty;
        private string password = string.Empty;
        private string email = string.Empty;
        private string phone = string.Empty;
        private string loginCode = string.Empty;
        private string stateCode = string.Empty;
        private string note01 = string.Empty;
        private string modifiedBy = string.Empty;
        private string registrationCode = string.Empty;
        private string userName = string.Empty;

        public int Id { get; set; }
        public int GenderId { get; set; }
        public int RaceId { get; set; }
        public int DistrictId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? PrimaryLanguageId { get; set; }
        public int? Status { get; set; }
        public int? SISID { get; set; }
        public int? CurrentGradeId { get; set; }
        public int? AdminSchoolId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public int? ModifiedUser { get; set; }

        public string FirstName
        {
            get { return firstName; }
            set { firstName = value.ConvertNullToEmptyString(); }
        }

        public string MiddleName
        {
            get { return middleName; }
            set { middleName = value.ConvertNullToEmptyString(); }
        }

        public string LastName
        {
            get { return lastName; }
            set { lastName = value.ConvertNullToEmptyString(); }
        }

        public string Code
        {
            get { return code; }
            set { code = value.ConvertNullToEmptyString(); }
        }

        public string AltCode
        {
            get { return altCode; }
            set { altCode = value.ConvertNullToEmptyString(); }
        }

        public string Password
        {
            get { return password; }
            set { password = value.ConvertNullToEmptyString(); }
        }

        public string Email
        {
            get { return email; }
            set { email = value.ConvertNullToEmptyString(); }
        }

        public string Phone
        {
            get { return phone; }
            set { phone = value.ConvertNullToEmptyString(); }
        }

        public string LoginCode
        {
            get { return loginCode; }
            set { loginCode = value.ConvertNullToEmptyString(); }
        }

        public string StateCode
        {
            get { return stateCode; }
            set { stateCode = value.ConvertNullToEmptyString(); }
        }

        public string Note01
        {
            get { return note01; }
            set { note01 = value.ConvertNullToEmptyString(); }
        }

        public string ModifiedBy
        {
            get { return modifiedBy; }
            set { modifiedBy = value.ConvertNullToEmptyString(); }
        }

        public string RegistrationCode
        {
            get { return registrationCode; }
            set { registrationCode = value.ConvertNullToEmptyString(); }
        }
        public string UserName
        {
            get { return userName; }
            set { userName = value.ConvertNullToEmptyString(); }
        }

        public string FullName { get; set; }

        public string SharedSecret { get; set; }
    }
}
