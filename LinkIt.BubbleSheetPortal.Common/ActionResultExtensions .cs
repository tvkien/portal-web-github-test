using System.Web.Mvc;

namespace LinkIt.BubbleSheetPortal.Common
{
    public static class ActionResultExtensions 
    {
        public static string Capture(this ActionResult result, ControllerContext controllerContext)
        {
            using (var it = new ResponseCapture(controllerContext.RequestContext.HttpContext.Response))
            {
                result.ExecuteResult(controllerContext);
                return it.ToString();
            }
        }
    }
}