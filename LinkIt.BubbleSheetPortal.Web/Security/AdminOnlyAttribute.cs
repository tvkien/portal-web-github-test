using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models.Interfaces;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using StackExchange.Profiling;

namespace LinkIt.BubbleSheetPortal.Web.Security
{
    public class AdminOnlyAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            using (MiniProfiler.Current.Step("AdminOnlyAttribute"))
            {
                if (!httpContext.User.Identity.IsAuthenticated)
                {
                    return false;
                }

                string cookieName = FormsAuthentication.FormsCookieName;
                HttpCookie authCookie = httpContext.Request.Cookies[cookieName];
                if (authCookie == null || string.IsNullOrEmpty(authCookie.Value))
                {
                    return false;
                }

                var ticket = FormsAuthentication.Decrypt(authCookie.Value);
                if (ticket.IsNull())
                {
                    return false;
                }

                var ableToContinue = true;

                var formsAuthenticationService = DependencyResolver.Current.GetService<IFormsAuthenticationService>();
                ableToContinue = formsAuthenticationService.EnsureSingleSignOn();
                if (ableToContinue)
                {
                    var _formsAuthenticationService = DependencyResolver.Current.GetService<FormsAuthenticationService>();
                    _formsAuthenticationService.ResetAuthCookie(authCookie);

                }
                else
                {
                    formsAuthenticationService.SignOut();
                    return false;
                }

                var user = UserPrincipal.CreatePrincipalFromCookieData(ticket.UserData);
                return user.IsNotTeacher();
            }
        }
    }
}
