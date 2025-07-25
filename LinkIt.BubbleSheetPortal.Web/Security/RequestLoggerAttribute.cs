using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;

namespace LinkIt.BubbleSheetPortal.Web.Security
{
    public class RequestLoggerAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var postData = filterContext.ActionParameters;

            Console.WriteLine(postData);
        }        
    }
}