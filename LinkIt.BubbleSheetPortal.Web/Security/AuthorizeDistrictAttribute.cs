using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using System.Web.Routing;

namespace LinkIt.BubbleSheetPortal.Web.Security
{
    public class AuthorizeDistrictAttribute : AuthorizeAttribute
    {
        public string UrlCode { get; set; }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
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

            var currentUser = HttpContext.Current.GetCurrentUser();
            var menuAccess = HelperExtensions.GetMenuForDistrict(currentUser);
            var displayItems = menuAccess.DisplayedItems;
            if (!displayItems.Contains("|" + UrlCode + "|"))
                return false;
            return true;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var currentUser = HttpContext.Current.GetCurrentUser();
            if(currentUser != null && currentUser.IsStudent)
            {
                var currentPage = HttpContext.Current.Request.RawUrl.ToString();
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary { { "Controller", "Student" }, { "Action", "Logon" }, { "ReturnUrl", currentPage } });
            }else
            {                
                // default MVC result was:
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}
