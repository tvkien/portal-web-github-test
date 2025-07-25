using System.Configuration;
using System.Web.Mvc;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Models.Infrastructure;

namespace LinkIt.BubbleSheetPortal.Web
{
    public class NewAllowCrossSiteJsonAttribute : ActionFilterAttribute
    {
        private static CorsSettings _corsSettings;
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            SetCorsSettings();

            var request = filterContext.RequestContext.HttpContext.Request;
            var origin = request.Headers["Origin"];

            if (_corsSettings.AllowAllOrigins || origin.IsOriginAnAllowedSubdomain(_corsSettings.AllowedOrigins))
            {
                filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Origin", origin);
            }

            base.OnActionExecuting(filterContext);
        }

        private static void SetCorsSettings()
        {
            if (_corsSettings != null)
                return;

            var corsSettings = ConfigurationManager.AppSettings[Constanst.CORS_SETTING_KEY];

            _corsSettings = corsSettings.DeserializeObject<CorsSettings>();
        }
    }
}
