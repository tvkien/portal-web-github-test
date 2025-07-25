namespace LinkIt.BubbleSheetPortal.Models.Interfaces
{
    public interface IFormsAuthenticationService
    {
        void SignIn(User user, bool createPersistentCookie, bool isImpersonate = false);
        void RefreshFormsAuthCookie(User user, bool createPeristentCookie, bool isImpersonate);
        void SignOut();
        string GetRedirectUrl();
        bool EnsureSingleSignOn();
    }
}