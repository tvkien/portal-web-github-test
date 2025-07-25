using LinkIt.BubbleSheetPortal.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LinkIt.BubbleSheetPortal.Web.Helpers
{
    public class VersionFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// ignore render new ui for action
        /// Dictionary<controller, actions>
        /// </summary>
        private Dictionary<string, List<string>> _ignoreActions = new Dictionary<string, List<string>>
        {
            {
                "TestAssignment",
                new List<string>
                {
                    "TestSettingForTestProperty"
                }
            }
        };

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var action = filterContext.RouteData.GetRequiredString("action");
            var controller = filterContext.RouteData.GetRequiredString("controller");

            DistrictService districtService = DependencyResolver.Current.GetService<DistrictService>();
            var subDomain = HttpContext.Current.Request.Headers["HOST"].Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0];
            var districtId = districtService.GetLiCodeBySubDomain(subDomain);

            if (IsRenderNewDesign(districtId, controller, action))
            {
                if (filterContext.Result is ViewResultBase viewResult && filterContext != null)
                {

                    string viewName;
                    if (!string.IsNullOrEmpty(viewResult.ViewName))
                    {
                        viewName = $"~/Views/{controller}/v2/{viewResult.ViewName}.cshtml";
                    }
                    else
                    {
                        viewName = $"~/Views/{controller}/v2/{action}.cshtml";
                    }

                    if (File.Exists(filterContext.HttpContext.Server.MapPath(viewName)))
                    {
                        viewResult.ViewName = viewName;
                    }
                }
            }

            base.OnResultExecuting(filterContext);
        }

        private bool IsRenderNewDesign(int districtId, string controller, string action)
        {
            var isUseNewDesign = HelperExtensions.IsUseNewDesign(districtId);
            _ignoreActions.TryGetValue(controller, out List<string> actions);
            return isUseNewDesign && ( actions == null || !actions.Any(x => x == action) );
        }
    }
}
