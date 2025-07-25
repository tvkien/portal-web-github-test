namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class LoginViewModel
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int District { get; set; }
        public string RedirectUrl { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Type { get; set; }
        public bool IsKeepLoggedIn { get; set; }
        public string Message { get; set; }
        public string PasswordQuestion { get; set; }
        public string PasswordAnswer { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
        public bool SubmittedCorrectPasswordAnswer { get; set; }
        public bool PasswordHasBeenReset { get; set; }
    }
}