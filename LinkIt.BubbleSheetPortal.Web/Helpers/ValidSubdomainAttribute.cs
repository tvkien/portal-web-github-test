using System;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.Security;
using StackExchange.Profiling;

namespace LinkIt.BubbleSheetPortal.Web.Helpers
{
    public class ValidSubdomainAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            using (MiniProfiler.Current.Step("ValidSubdomainAttribute.OnActionExecuting"))
            {
                var service = DependencyResolver.Current.GetService<DistrictService>();
                var subDomain = filterContext.HttpContext.Request.Headers["HOST"].Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0];
                var districtId = service.GetLiCodeBySubDomain(subDomain);
                if (districtId == 0 && !(subDomain.Equals("portal") || subDomain.Equals("admin")))
                {
                    var url = ConfigurationManager.AppSettings["LinkItUrl"];
                    var redirectUrl = string.Format("{0}://{1}.{2}", HelperExtensions.GetHTTPProtocal(filterContext.HttpContext.Request), GetCorrectUrl(filterContext), url);
                    filterContext.Result = new RedirectResult(redirectUrl);
                }
            }
        }

        private string GetCorrectUrl(ActionExecutingContext filterContext)
        {
            using (MiniProfiler.Current.Step("ValidSubdomainAttribute.GetCorrectUrl"))
            {
                var user = SetUserPrincipal(filterContext.HttpContext);
                if (user.IsNotNull())
                {
                    if (user.RoleId.Equals((int)Permissions.Publisher))
                    {
                        return "admin";
                    }
                    return user.DistrictId.HasValue ? DependencyResolver.Current.GetService<DistrictService>().GetDistrictById(user.DistrictId.GetValueOrDefault()).LICode : "portal";
                }
                return "portal";
            }
        }

        private UserPrincipal SetUserPrincipal(HttpContextBase context)
        {
            using (MiniProfiler.Current.Step("ValidSubdomainAttribute.SetUserPrincipal"))
            {
                string cookieName = FormsAuthentication.FormsCookieName;
                HttpCookie authCookie = context.Request.Cookies[cookieName];
                if (authCookie == null || string.IsNullOrEmpty(authCookie.Value))
                {
                    return null;
                }

                var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                return UserPrincipal.CreatePrincipalFromCookieData(authTicket.UserData);
            }
        }
    }
}
