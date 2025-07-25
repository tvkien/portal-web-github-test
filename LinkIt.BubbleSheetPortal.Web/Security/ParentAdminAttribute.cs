using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using StackExchange.Profiling;

namespace LinkIt.BubbleSheetPortal.Web.Security
{
    public class ParentAdminAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            using (MiniProfiler.Current.Step("ParentAdminAttribute"))
            {
                if (!httpContext.User.Identity.IsAuthenticated)
                {
                    return false;
                }

                string cookieName = FormsAuthentication.FormsCookieName;
                HttpCookie authCookie = httpContext.Request.Cookies[cookieName];
                var _formsAuthenticationService = DependencyResolver.Current.GetService<FormsAuthenticationService>();
                if (authCookie == null || string.IsNullOrEmpty(authCookie.Value))
                {
                    return false;
                }

                var ticket = FormsAuthentication.Decrypt(authCookie.Value);
                if (ticket.IsNull())
                {
                    return false;
                }
                _formsAuthenticationService.ResetAuthCookie(authCookie);

                var user = UserPrincipal.CreatePrincipalFromCookieData(ticket.UserData);
                return (user.IsDistrictAdminOrPublisher || user.IsSchoolAdmin || user.IsTeacher||user.IsNetworkAdmin);
            }
        }
    }
}
