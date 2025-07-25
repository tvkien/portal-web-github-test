using System;
using System.Configuration;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    [Serializable]
    public class LoginAccountViewModel : AccountViewModelBase
    {
        public string Password { get; set; }
        public int District { get; set; }
        public bool IsKeepLoggedIn { get; set; }
        public bool HasEmailAddress { get; set; }
        public bool HasSecurityQuestion { get; set; }
        public bool HasTemporaryPassword { get; set; }
        public string LogOnHeaderHtmlContent { get; set; }
        public bool IsNetworkAdmin { get; set; }

        public LoginAccountViewModel()
        {
            IsNetworkAdmin = false;
        }

        public bool ShowAnnouncement { get; set; }
        public string AnnouncementText { get; set; }
        public bool ShowCaptcha { get; set; }

        //\
        public bool IsShowWarningLogOnUser { get; set; }

        public string MessageWarningLogOnUser { get; set; }
        public string UrlRecomment { get; set; }

        public bool IsDisableLoginForm
        {
            get
            {
                if (HttpContext.Current.Request.Url != null && (
                                   HttpContext.Current.Request.Url.AbsoluteUri.StartsWith("http://portal.")
                                    || HttpContext.Current.Request.Url.AbsoluteUri.StartsWith("https://portal.")
                                    || HttpContext.Current.Request.Url.AbsoluteUri.StartsWith("http://admin.")
                                    || HttpContext.Current.Request.Url.AbsoluteUri.StartsWith("https://admin."))
                        && !HttpContext.Current.Request.Url.AbsoluteUri.Contains("classlink")
                        && !HttpContext.Current.Request.Url.AbsoluteUri.Contains("LTI"))
                {
                    return true;
                }

                return false;
            }
            set { }
        }

        public bool TermsOfUse { get; set; }
        public bool ShowDisclaimerContent { get; set; }
        public string DisclaimerContent { get; set; }
        public string DisclaimerCheckboxLabel { get; set; }
        public bool IsFirstStudentLogonSSO { get; set; }
        public bool EnableLoginByGoogle { get; set; }
        public bool EnableLoginByMicrosoft { get; set; }
        public bool EnableLoginByClever { get; set; }
        public int SSOInformationId { get; set; }
        public string SSOType { get; set; }
        public int RoleId { get; set; }
        public bool IsPortalNewSkinNotLogin { get; set; }
        public bool HideLoginCredentials { get; set; }
        public bool EnableLoginByNYC { get; set; }
        public bool EnableLoginByClassLink { get; set; }
    }
}
