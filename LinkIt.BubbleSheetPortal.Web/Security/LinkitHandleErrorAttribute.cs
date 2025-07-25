using System.Web.Mvc;

namespace LinkIt.BubbleSheetPortal.Web.Security
{
    public class LinkitHandleErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);
            if(filterContext.Exception.GetType().IsAssignableFrom(typeof(HttpAntiForgeryException)))
            {
                filterContext.ExceptionHandled = true;
                filterContext.Result = new RedirectResult(filterContext.HttpContext.Request.RawUrl);
                return;
            }
            if (!filterContext.ExceptionHandled)
            {
                return;
            }
        }
    }
}
