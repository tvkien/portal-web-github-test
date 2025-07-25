using System;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.MfaSettings
{
    public class MfaVerificationVM
    {
        public string Code { get; set; }
        public string g_recaptcha_response { get; set; }
        public bool HasEmailAddress { get; set; }
    }
}
