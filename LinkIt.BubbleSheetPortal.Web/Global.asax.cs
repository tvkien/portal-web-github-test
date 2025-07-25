using System;
using System.Configuration;
using System.Diagnostics;
using System.Deployment.Internal.CodeSigning;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Web.App_Start;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Security;
using System.Net;
using Serilog;
using LinkIt.BubbleSheetPortal.Models.Constants;
using LinkIt.BubbleSheetPortal.Web.HealthCheck;

namespace LinkIt.BubbleSheetPortal.Web
{
    public class MvcApplication : HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new LinkitHandleErrorAttribute());
            filters.Add(new PortalLoggingAttribute());
        }

        protected void Application_Start()
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.AppSettings() // Reads from web.config
                .CreateLogger();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                    | SecurityProtocolType.Tls11
                                    | SecurityProtocolType.Tls12;
            var isSsl =  ConfigurationManager.AppSettings["ForceSSL"];
            bool SslRequired = true;
            if (isSsl != null)  
            {
                bool.TryParse(isSsl, out SslRequired);
            }

            AreaRegistration.RegisterAllAreas();
            AutofacConfigurator.Initialize();
            AutoMapperConfigurator.Initialize();
            ValidationConfiguration.Configure();
            DevExpressStorageConfig.Initialize();

            if (SslRequired)
            {
                GlobalFilters.Filters.Add(new SslFilter());
            }
            RegisterGlobalFilters(GlobalFilters.Filters);    
            RouteRegistrar.Initialize(RouteTable.Routes);
            ControllerBuilder.Current.SetControllerFactory(new SessionStateControllerFactory());            

            MvcHandler.DisableMvcResponseHeader = true;

            // Enable SHA-256 XML signature support.
            CryptoConfig.AddAlgorithm(
                typeof(RSAPKCS1SHA256SignatureDescription),
                "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256");
            BundleConfig.RegisterBundles();

            //find the default JsonVAlueProviderFactory
            JsonValueProviderFactory jsonValueProviderFactory = null;

            foreach (var factory in ValueProviderFactories.Factories)
            {
                if (factory is JsonValueProviderFactory)
                {
                    jsonValueProviderFactory = factory as JsonValueProviderFactory;
                }
            }

            //remove the default JsonVAlueProviderFactory
            if (jsonValueProviderFactory != null) ValueProviderFactories.Factories.Remove(jsonValueProviderFactory);

            //add the custom one
            ValueProviderFactories.Factories.Add(new CustomJsonValueProviderFactory());
                        
        }
        
        protected void Application_Error(object sender, EventArgs e)
        {
            Response.Headers.Remove("Server");
            Response.Headers.Remove("X-AspNet-Version");
            Response.Headers.Remove("X-AspNetMvc-Version");
            Response.Headers.Remove("X-Powered-By");

            var isSsl = ConfigurationManager.AppSettings["ForceSSL"];
            bool sslRequired = true;
            if (isSsl != null)
                bool.TryParse(isSsl, out sslRequired);

            foreach (var cName in Response.Cookies.AllKeys)
            {
                if (cName == Constanst.DefaultDateFormat || cName == Constanst.DefaultTimeFormat || cName == Constanst.DefaultJqueryDateFormat || cName == Constanst.WarningExpire)
                    Response.Cookies[cName].HttpOnly = false;
                else
                    Response.Cookies[cName].HttpOnly = true;

                if (sslRequired)
                    Response.Cookies[cName].Secure = true;

                Response.Cookies[cName].SameSite = SameSiteMode.Lax;
            }

            HttpApplication application = (HttpApplication)sender;
            if (application == null)
            {
                return;
            }

            var httpContext = application.Context;
            Exception lastError = Server.GetLastError();

            if (httpContext == null || lastError == null)
            {
                return;
            }

            var httpException = lastError as HttpException;
            if (DataScopeManager.LinkitConfigurationManager.IsVaultException
                || (httpException != null && httpException.GetHttpCode() == 404))
            {
                Response.Clear();
                Server.ClearError();
                Response.Redirect("~/Error/NotFound");
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            var hostType = Uri.CheckHostName(Request.Url.Host);

            // We only accept a request with subdomain.linkit.com pattern because we use subdomain to detect information on Dynamo database
            // If hostType is IPv4, IPv6, use the default value in web.config for session timeout
            if (hostType == UriHostNameType.Dns)
            {
                if (DataScopeManager.LinkitConfigurationManager.CanAccessVault)
                {
                    var _formsAuthenticationService = DependencyResolver.Current.GetService<FormsAuthenticationService>();
                    Session.Timeout = _formsAuthenticationService.TimeOutMinutes;
                }
                else
                {
                    Session.Timeout = 30;
                }
            }
        }

        protected void Application_EndRequest()
        {
            // add HttpOnly and Secure by custom ForceSSL configuration 
            var isSsl =  ConfigurationManager.AppSettings["ForceSSL"];
            bool sslRequired = true;
            if (isSsl != null)  
                bool.TryParse(isSsl, out sslRequired);

            foreach (var cName in Response.Cookies.AllKeys)
            {
                if (cName == Constanst.DefaultDateFormat || cName == Constanst.DefaultTimeFormat || cName==Constanst.DefaultJqueryDateFormat || cName == Constanst.WarningExpire)
                    Response.Cookies[cName].HttpOnly = false;
                else
                    Response.Cookies[cName].HttpOnly = true;

                if(sslRequired)
                    Response.Cookies[cName].Secure = true;

                Response.Cookies[cName].SameSite = SameSiteMode.Lax;
            }

            //Response.AppendHeader("X-Frame-Options", "SAMEORIGIN");
            //Response.AppendHeader("Content-Security-Policy", "frame-ancestors 'self'");
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var healthCheckDto = InternalHealthChecker.HealthyCheck(Request);

            if (healthCheckDto != null)
            {
                Response.StatusCode = healthCheckDto.StatusCode;
                Response.Write(healthCheckDto.Message);

                CompleteRequest();
            }
        }

        protected void Application_PreSendRequestHeaders(Object source, EventArgs e)
        {
            // removing excessive headers. They don't need to see this.
            Response.Headers.Remove("Server");
            Response.Headers.Remove("X-AspNet-Version");
            Response.Headers.Remove("X-AspNetMvc-Version");
            Response.Headers.Remove("X-Powered-By");

            Response.Headers.Add("X-Server-Name", Environment.MachineName);
        }

        protected void Session_End(object sender, EventArgs e)
        {
            Debug.WriteLine("Session end: " + Session.SessionID);
        }
    }
}
