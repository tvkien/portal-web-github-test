using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using LinkIt.BubbleSheetPortal.Models.Interfaces;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using StackExchange.Profiling;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Common;

namespace LinkIt.BubbleSheetPortal.Web.Security
{
    public class AjaxAwareAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var profiler = MiniProfiler.Current;

            using (profiler.Step("AjaxAwareAuthorize"))
            {
                var userRoleId = 0;
                var userRoleCookie = HttpContext.Current.Request.Cookies[Constanst.UserRoleIdCookie];
                if (userRoleCookie != null)
                    userRoleId = int.Parse(userRoleCookie.Value);

                var passThroughCookie = HttpContext.Current.Request.Cookies["UserPassThrough"];
                var returnURL = string.Empty;
                if (passThroughCookie != null)
                {
                    returnURL = passThroughCookie["PassThroughReturnURL"];
                }

                var subDomain = HelperExtensions.GetSubdomain().ToLower();
                var domainCookie = filterContext.HttpContext.Request.Cookies["DomainName"];
                var domainName = domainCookie != null && !string.IsNullOrEmpty(domainCookie.Value) ?
                    domainCookie.Value.ToLower() : string.Empty;

                if (subDomain.Equals(domainName) == false)
                {
                    if (domainName != "portal" && domainName != "admin")
                    {
                        //check if this current user is being impersonated from another user or not
                        var currentUserCookie = SetUserPrincipal(filterContext.HttpContext);
                        if (!string.IsNullOrEmpty(domainName) && currentUserCookie != null)
                        {
                            if (currentUserCookie.OriginalID == 0) //No one is impersonating
                            {
                                //If a publisher or networkadmin has been going back, no log off happen
                                if (currentUserCookie.ImpersonateLogActivity != ImpersonateLog.ActionTypeEnum.GoBack
                                    && currentUserCookie.OriginalNetworkAdminDistrictId == 0)
                                {
                                    LogOffUser(filterContext);
                                }
                            }
                            else
                            {
                                // currentUserCookie.OriginalID > 0 , a publiser or Network Admin is impersonating
                                // If user manually change the subdomain on web address of browser -> log off
                                if (!currentUserCookie.ImpersonatedSubdomain.ToLower().Equals(subDomain.ToLower()))
                                {
                                    LogOffUser(filterContext);
                                }
                            }
                        }
                        else
                        {
                            LogOffUser(filterContext);
                        }
                    }
                }
                UserPrincipal vUser = null;

                var ableToContinue = true;
                var formsAuthenticationService = DependencyResolver.Current.GetService<IFormsAuthenticationService>();
                ableToContinue = formsAuthenticationService.EnsureSingleSignOn();
                if (ableToContinue)
                {
                    vUser = SetUserPrincipal(filterContext.HttpContext);
                    RefreshCookie();
                }
                else
                {
                    formsAuthenticationService.SignOut();
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                        action = "LogOff",
                        controller = "Account",
                        area = ""
                    }));
                }

                base.OnAuthorization(filterContext);

                if (filterContext.Result is HttpUnauthorizedResult)
                {
                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        HttpContext.Current.Response.Cookies.Add(new HttpCookie(Constanst.UserRoleIdCookie, userRoleId.ToString()));
                        if (passThroughCookie != null)
                            HttpContext.Current.Response.Cookies.Add(passThroughCookie);
                        filterContext.HttpContext.Items["RequestWasNotAuthorized"] = true;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(returnURL))
                        {
                            var viewData = new ViewDataDictionary();
                            viewData["returnURL"] = returnURL;
                            filterContext.Result = new ViewResult
                            {
                                ViewName = "~/Views/Shared/_LogOff.cshtml",
                                ViewData = viewData
                            };

                            var formsAuthCookie = new HttpCookie("UserPassThrough")
                            {
                                HttpOnly = true,
                                Path = FormsAuthentication.FormsCookiePath,
                                Secure = FormsAuthentication.RequireSSL
                            };
                            if (FormsAuthentication.CookieDomain != null)
                            {
                                formsAuthCookie.Domain = FormsAuthentication.CookieDomain;
                            }
                            formsAuthCookie["PassThroughUserID"] = "";
                            formsAuthCookie["PassThroughReturnURL"] = "";
                            formsAuthCookie.Expires = DateTime.Now.AddDays(-1d);
                            formsAuthCookie["GUIDSession"] = "";
                            filterContext.HttpContext.Response.Cookies.Add(formsAuthCookie);
                        }
                        else
                        {
                            if (RoleUtil.IsStudent(userRoleId))
                            {
                                userRoleCookie.Expires = DateTime.UtcNow.AddDays(-10);
                                HttpContext.Current.Response.Cookies.Add(userRoleCookie);
                                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Student", action = "" }));
                            }
                        }
                    }
                }
            }

        }
        private void LogOffUser(AuthorizationContext filterContext)
        {
            IFormsAuthenticationService formAuthenticationService =
                (IFormsAuthenticationService)DependencyResolver.Current.GetService(typeof(IFormsAuthenticationService));
            formAuthenticationService.SignOut();
            base.HandleUnauthorizedRequest(filterContext);
        }

        private UserPrincipal SetUserPrincipal(HttpContextBase context)
        {
            string cookieName = FormsAuthentication.FormsCookieName;
            HttpCookie authCookie = context.Request.Cookies[cookieName];
            var _formsAuthenticationService = DependencyResolver.Current.GetService<FormsAuthenticationService>();
            if (authCookie == null || string.IsNullOrEmpty(authCookie.Value))
            {
                return null;
            }

            _formsAuthenticationService.ResetAuthCookie(authCookie);
            var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            var vUser = UserPrincipal.CreatePrincipalFromCookieData(authTicket.UserData);

            context.User = vUser;
            return vUser;
        }

        private void RefreshCookie()
        {
            var domainInfo = HttpContext.Current.Request.Cookies.Get("DomainName");
            var _formsAuthenticationService = DependencyResolver.Current.GetService<FormsAuthenticationService>();
            if (domainInfo == null) domainInfo = new HttpCookie("DomainName");

            domainInfo.Expires = DateTime.Now.AddMinutes(_formsAuthenticationService.TimeOutMinutes);
            domainInfo.Value = HelperExtensions.GetSubdomain();
            domainInfo.HttpOnly = true;
            domainInfo.Secure = FormsAuthentication.RequireSSL;
            domainInfo.Domain = FormsAuthentication.CookieDomain;

            HttpContext.Current.Response.Cookies.Set(domainInfo);
        }

    }
}
