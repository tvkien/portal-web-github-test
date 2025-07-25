using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using StackExchange.Profiling;

namespace LinkIt.BubbleSheetPortal.Web.Security
{
    public class UploadifyPrincipalAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            using (MiniProfiler.Current.Step("UploadifyPrincipalAttribute"))
            {
                var formsAuthTicket = filterContext.HttpContext.Request.Form["AUTHID"];
                if (!string.IsNullOrEmpty(formsAuthTicket))
                {
                    var ticket = FormsAuthentication.Decrypt(formsAuthTicket);
                    if (ticket != null)
                    {
                        filterContext.HttpContext.User = UserPrincipal.CreatePrincipalFromCookieData(ticket.UserData);
                    }
                }
            }
        }
    }
}