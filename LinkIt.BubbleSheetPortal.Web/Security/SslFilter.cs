using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LinkIt.BubbleSheetPortal.Web.Security
{
    public class SslFilter : RequireHttpsAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            if (filterContext.HttpContext.Request.IsSecureConnection)
            {
                return;
            }

            if (string.Equals(filterContext.HttpContext.Request.Headers["X-Forwarded-Proto"], "https", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            if (filterContext.HttpContext.Request.IsLocal)
            {
                return;
            }

            //HandleNonHttpsRequest(filterContext);
            var url = filterContext.HttpContext.Request.Url.ToString().Replace("http:", "https:");
            url = url.Replace("https://www.", "https://");
            filterContext.Result = new RedirectResult(url);
        }
    }
}
