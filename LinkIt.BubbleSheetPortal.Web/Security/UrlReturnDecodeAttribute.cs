using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using System.Web.Security;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using StackExchange.Profiling;

namespace LinkIt.BubbleSheetPortal.Web.Security
{
    public class UrlReturnDecodeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext httpContext)
        {
            var url = httpContext.HttpContext.Request.Url.AbsoluteUri;
            if (url.Contains("amp;"))
            {
                url = url.Replace("amp;", "");
                httpContext.Result = new RedirectResult(url);
            }
        }
    }
}