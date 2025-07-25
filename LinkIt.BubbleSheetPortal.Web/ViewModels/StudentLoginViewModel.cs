namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class StudentLoginViewModel : AccountViewModelBase
    {
        public string Password { get; set; }

        public bool KeepLogged { get; set; }

        public int DistrictId { get; set; }

        public string LogOnHeaderHtmlContent { get; set; }

        public bool ShowAnnouncement { get; set; }

        public string AnnouncementText { get; set; }

        public bool HasTemporaryPassword { get; set; }

        public bool HasEmailAddress { get; set; }

        public bool HasSecurityQuestion { get; set; }
        public bool ShowCaptcha { get; set; }

        public string RCode { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int StudentId { get; set; }

        public string CPassword { get; set; }
        public bool EnableLoginByGoogle { get; set; }
        public bool EnableLoginByMicrosoft { get; set; }
        public bool EnableLoginByClever { get; set; }
        public bool IsStudentLogin { get; set; }

        public bool IsRequireKioskMode { get; set; }
        public bool HideLoginCredentials { get; set; }
        public bool EnableLoginByNYC { get; set; }
        public bool EnableLoginByClassLink { get; set; }
    }
}
