using System.Web;
using System.Web.Mvc;
using System.Linq;
using System.Web.Security;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Security;
using StackExchange.Profiling;
using StackExchange.Profiling.MVCHelpers;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using System.Configuration;
using System;

[assembly: WebActivator.PreApplicationStartMethod(typeof(LinkIt.BubbleSheetPortal.Web.App_Start.MiniProfilerPackage), "PreStart")]
[assembly: WebActivator.PostApplicationStartMethod(typeof(LinkIt.BubbleSheetPortal.Web.App_Start.MiniProfilerPackage), "PostStart")]

namespace LinkIt.BubbleSheetPortal.Web.App_Start
{
    public static class MiniProfilerPackage
    {
        public static void PreStart()
        {
                MiniProfiler.Settings.MaxJsonResponseSize = Int32.MaxValue;
                MiniProfiler.Settings.SqlFormatter = new StackExchange.Profiling.SqlFormatters.SqlServerFormatter();
                MiniProfiler.Settings.PopupRenderPosition = RenderPosition.Right;                
                DynamicModuleUtility.RegisterModule(typeof(MiniProfilerStartupModule));
                GlobalFilters.Filters.Add(new ProfilingActionFilter());                
        }

        public static void PostStart()
        {
                var copy = ViewEngines.Engines.ToList();
                ViewEngines.Engines.Clear();
                foreach (var item in copy)
                {
                    ViewEngines.Engines.Add(new ProfilingViewEngine(item));
                }
        }
    }

    public class MiniProfilerStartupModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.BeginRequest += (sender, e) => MiniProfiler.Start();

            context.AuthenticateRequest += (sender, e) =>
            {
                SetUserPrincipal(new HttpContextWrapper(HttpContext.Current));
                var user = (UserPrincipal)context.User;
                if (!user.IsPublisher()
                    || string.IsNullOrEmpty(ConfigurationManager.AppSettings["EnableMiniProfiler"])
                    || ConfigurationManager.AppSettings["EnableMiniProfiler"] == "0"
                    || context.Request.Path.Contains("/ItemBank/ShowAddItemsFromLibrary")
                    || context.Request.Path.Contains("/VirtualTest/ShowImportItemsFromLibrary")) // Do not start miniprofiler on these pages to fix IE slow loading on these pages
                    {
                    MiniProfiler.Stop(discardResults: true);
                }
            };

            context.EndRequest += (sender, e) => MiniProfiler.Stop();
        }

        private void SetUserPrincipal(HttpContextBase context)
        {
            string cookieName = FormsAuthentication.FormsCookieName;
            HttpCookie authCookie = context.Request.Cookies[cookieName];
            if (authCookie == null || string.IsNullOrEmpty(authCookie.Value))
            {
                return;
            }

            var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            context.User = UserPrincipal.CreatePrincipalFromCookieData(authTicket.UserData);
        }

        public void Dispose() { }        
    }
}
