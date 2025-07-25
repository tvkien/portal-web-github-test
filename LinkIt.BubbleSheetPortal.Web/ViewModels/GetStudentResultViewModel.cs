using System;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class GetStudentResultViewModel
    {
        public int StudentId { get; set; }
        public string Action { get; set; }
        public string FullName { get; set; }
        public string Code { get; set; }
        public string StateCode { get; set; }
        public string SchoolName { get; set; }
        public string Classes { get; set; }
        public string GradeName { get; set; }
        public string RaceName { get; set; }
        public string GenderCode { get; set; }
        public int? Status { get; set; }
        public bool CanAccess { get; set; }
        public string RegistrationCode { get; set; }
        public string UserName { get; set; }
        public string SharedSecret { get; set; }
        public string Email { get; internal set; }
        public DateTime? RegistrationCodeEmailLastSent { get; set; }
        public bool HasEdit { get; set; }
        public bool HasDelete { get; set; }
        public bool HasView { get; set; }
        public bool HasResetPassword { get; set; }
        public bool HasGenerateLogin { get; set; }
    }
}
