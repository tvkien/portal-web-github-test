using System;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class LookupStudent
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Code { get; set; }
        public string StateCode { get; set; }
        public string SchoolName { get; set; }
        public string GradeName { get; set; }
        public string RaceName { get; set; }
        public string GenderCode { get; set; }
        public int? Status { get; set; }
        public int? AdminSchoolId { get; set; }
        public int? DistrictId { get; set; }
        public string RegistrationCode { get; set; }
        public int? TLDSProfileID { get; set; }
        public string AltCode { get; set; }
        public string FullName { get; set; }
        public bool HasEmailAddress { get; set; }
        public DateTime? RegistrationCodeEmailLastSent { get; set; }
        public bool TheUserLogedIn { get; set; }
        public string Email { get; set; }
        public string SharedSecret { get; set; }
        public string UserName { get; set; }
        public string Classes { get; set; }
        public bool HasPassword { get; set; }
    }
}
