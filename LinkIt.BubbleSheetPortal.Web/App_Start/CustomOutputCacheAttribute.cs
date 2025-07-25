using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Web
{
    public class CustomOutputCacheAttribute : System.Web.Mvc.OutputCacheAttribute
    {
        bool IgnoreChildCache(System.Web.Mvc.ControllerContext filterContext)
        {
            return filterContext.IsChildAction
                && (Location == System.Web.UI.OutputCacheLocation.None || Duration == 0);
        }

        public override void OnActionExecuting(System.Web.Mvc.ActionExecutingContext filterContext)
        {
            if (IgnoreChildCache(filterContext))
            {
                return;
            }
            else {
                base.OnActionExecuting(filterContext);
            }
        }
        public override void OnResultExecuted(System.Web.Mvc.ResultExecutedContext filterContext)
        {
            if (IgnoreChildCache(filterContext))
            {
                return;
            }
            else {
                base.OnResultExecuted(filterContext);
            }
        }
        public override void OnResultExecuting(System.Web.Mvc.ResultExecutingContext filterContext)
        {
            if (IgnoreChildCache(filterContext))
            {
                return;
            }
            else {
                base.OnResultExecuting(filterContext);
            }
        }
    }
}
