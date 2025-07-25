using System;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Common
{
    public static class HttpContextExtention
    {
        public static string GetLICodeFromRequest(this HttpContext httpContext)
        {
            if (httpContext == null || httpContext.Request == null)
            {
                return string.Empty;
            }

            return httpContext.Request.Url.Host.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0];
        }

        public static string GetLICodeFromRequest(this HttpContextBase httpContext)
        {
            if (httpContext == null || httpContext.Request == null)
            {
                return string.Empty;
            }

            return httpContext.Request.Url.Host.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0];
        }
    }
}
