namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class ResetPasswordViewModel : AccountViewModelBase
    {
        public string PasswordQuestion { get; set; }
        public string PasswordAnswer { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
        public bool SubmittedCorrectPasswordAnswer { get; set; }
        public bool PasswordHasBeenReset { get; set; }
        public bool HasEmailAddress { get; set; }
        public bool HasSecurityQuestion { get; set; }
        public bool ShowCaptcha { get; set; }
        public bool IsStudentLogin { get; set; }
    }
}
