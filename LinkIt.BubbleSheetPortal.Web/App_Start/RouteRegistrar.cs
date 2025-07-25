using System.Web.Mvc;
using System.Web.Routing;

namespace LinkIt.BubbleSheetPortal.Web.App_Start
{
    public static class RouteRegistrar
    {
        public static void Initialize(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute("AssessmentItems", "AssessmentItems/{id}", new { controller = "ItemBank", action = "Index", id = UrlParameter.Optional });
            routes.MapRoute("SpecializedReport", "SpecializedReport/{id}", new { controller = "ACTReport", action = "Index", id = UrlParameter.Optional });
            routes.MapRoute("Parent", "Parent", defaults : new { routeName = "Parent", controller = "Student", action = "Index" });
            routes.MapRoute("ParentLogin", "Parent/Index/{id}", defaults: new { routeName = "Parent", controller = "Student", action = "Index", id = UrlParameter.Optional });
            routes.MapRoute("KeepAlive", "Home/KeepAlive", defaults: new { controller = "HomeSessionStateReadOnly", action = "KeepAlive" });
            routes.MapRoute("GetConfigTimeOutWarning", "Home/GetConfigTimeOutWarning", defaults: new { controller = "HomeSessionStateReadOnly", action = "GetConfigTimeOutWarning" });
            routes.MapRoute("Default", "{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional });
            routes.MapRoute("PageNotFound", "{*url}", new { controller = "Error", action = "NotFound" });            
        }
    }
}
