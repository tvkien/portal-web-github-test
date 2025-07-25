using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using System.Web.Routing;

namespace LinkIt.BubbleSheetPortal.Web.App_Start
{
    public class TldsDigitalActionFilter : ActionFilterAttribute
    {
        public string IdParamName { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            HttpCookie httpCookie = new HttpCookie("tldscookie");

            if (HttpContext.Current.Request.Cookies.AllKeys.Contains("tldscookie"))
            {
                httpCookie = HttpContext.Current.Request.Cookies["tldscookie"];
            }

            if (filterContext.ActionParameters.ContainsKey(IdParamName))
            {
                var tldsProfileId = filterContext.ActionParameters[IdParamName]?.ToString();
                if(string.IsNullOrEmpty(tldsProfileId))
                {
                    HandleInValidRequest(filterContext);
                    return;
                }
                var service = DependencyResolver.Current.GetService<TLDSDigitalSection23Service>();
                var deactive = service.CheckLinkStatus(new Guid(tldsProfileId));
                var isExpired = service.CheckLinkExpired(new Guid(tldsProfileId));

                if (deactive)
                {
                    HandleDeactiveOrExpiredRequest(filterContext, false);
                }
                else if (isExpired)
                {
                    HandleDeactiveOrExpiredRequest(filterContext, true);
                }

                if (httpCookie.Value != tldsProfileId)
                {
                    HandleUnauthorizedRequest(filterContext);
                }
            }
        }

        private void HandleUnauthorizedRequest(ActionExecutingContext filterContext)
        {
            var routeValues = new RouteValueDictionary();

            routeValues["controller"] = "TLDSDigitalSection23";
            routeValues["action"] = "Login";
            routeValues.Add("id", filterContext.ActionParameters[IdParamName]);

            filterContext.Result = new RedirectToRouteResult(routeValues);
        }

        private void HandleDeactiveOrExpiredRequest(ActionExecutingContext filterContext, bool isExpired)
        {
            var routeValues = new RouteValueDictionary();

            routeValues["controller"] = "TLDSDigitalSection23";
            routeValues["action"] = "Error";
            routeValues.Add("isExpired", isExpired);

            filterContext.Result = new RedirectToRouteResult(routeValues);
        }

        private void HandleInValidRequest(ActionExecutingContext filterContext)
        {
            var routeValues = new RouteValueDictionary();

            routeValues["controller"] = "Error";
            routeValues["action"] = "NotFound";

            filterContext.Result = new RedirectToRouteResult(routeValues);
        }
    }
}
