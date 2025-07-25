using System;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Tests.Common
{
    public class FakeFormsAuthenticationService : IFormsAuthenticationService
    {
        public void SignIn(User user, bool createPersistentCookie, bool isImpersonate = false)
        {
        }

        public void RefreshFormsAuthCookie(User user, bool createPersistentCookie, bool isImpersonate)
        {
        }

        public void SignOut()
        {
        }

        public string GetRedirectUrl()
        {
            return "Home/Index";
        }

        public bool EnsureSingleSignOn()
        {
            return false;
        }
    }
}
