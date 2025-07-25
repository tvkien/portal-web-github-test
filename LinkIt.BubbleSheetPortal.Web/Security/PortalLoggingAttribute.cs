using System.Web.Mvc;

namespace LinkIt.BubbleSheetPortal.Web.Security
{
    public class PortalLoggingAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                var request = filterContext.HttpContext.Request;
                var currentUser = filterContext.HttpContext.User.Identity as UserPrincipal;
                PortalAuditManager.LogRequest(request, currentUser, filterContext);
            }
            catch { }

            base.OnActionExecuting(filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            try
            {
                if (filterContext.Exception != null)
                {
                    PortalAuditManager.LogException(filterContext.Exception);
                }

                var response = filterContext.HttpContext.Response;
                PortalAuditManager.LogResponse(response);
                PortalAuditManager.LogFull();
            }
            catch { }

            base.OnActionExecuted(filterContext);
        }
    }
}
