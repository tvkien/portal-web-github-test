using System;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Xml.Linq;
using AutoMapper;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models.Interfaces;
using RestSharp;
using HttpCookie = System.Web.HttpCookie;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Common;
using System.Web.Mvc;
using LinkIt.BubbleSheetPortal.Models.Enum;

namespace LinkIt.BubbleSheetPortal.Web.Security
{
    public class FormsAuthenticationService : IFormsAuthenticationService
    {
        private readonly UserService userService;
        private readonly StateService stateService;
        private readonly DistrictService districtService;
        private readonly ConfigurationService _configurationService;
        private readonly UserLogonService _userLogonService;
        private readonly DistrictDecodeService _districtDecodeService;

        public FormsAuthenticationService(UserService userService,
            StateService stateService,
            DistrictService districtService,
            ConfigurationService configurationService,
            UserLogonService userLogonService,
            DistrictDecodeService districtDecodeService
            )
        {
            this.userService = userService;
            this.stateService = stateService;
            this.districtService = districtService;
            this._districtDecodeService = districtDecodeService;
            this._configurationService = configurationService;
            this._userLogonService = userLogonService;
            _districtDecodeService = districtDecodeService;
        }

        public void SignIn(User user, bool createPersistentCookie, bool isImpersonate = false)
        {
            if (user.IsNull()) throw new ArgumentNullException("user");

            userService.GetWelcomeMessage(user);
            SetWalkmeSnippet(user);
            var formsCookie = GetFormsAuthCookie(user, createPersistentCookie, isImpersonate);

            HttpContext.Current.Response.Cookies.Add(formsCookie);
            // write to DefaultDateFormat cookie
            Util.LoadDateFormatToCookies(user.DistrictId.GetValueOrDefault(), _districtDecodeService);

            var linkitARCookieDoman = ConfigurationManager.AppSettings["LKARCookieDomainName"];
            linkitARCookieDoman = string.IsNullOrEmpty(linkitARCookieDoman) ? formsCookie.Domain : linkitARCookieDoman.Trim();

            string reportingCookie = GetReportingCookieInternal(user);
            var domainInfo = new HttpCookie("DomainName");
            domainInfo.Expires = formsCookie.Expires;
            domainInfo.Value = HelperExtensions.GetSubdomain();
            domainInfo.HttpOnly = true;
            domainInfo.Secure = formsCookie.Secure;
            domainInfo.Domain = formsCookie.Domain;

            HttpContext.Current.Response.Cookies.Add(domainInfo);

            // Store user role for redirection page
            var userRoleCookie = HttpContext.Current.Request.Cookies[Constanst.UserRoleIdCookie];
            if (userRoleCookie == null)
            {
                userRoleCookie = new HttpCookie(Constanst.UserRoleIdCookie, user.RoleId.ToString());
                HttpContext.Current.Response.Cookies.Add(userRoleCookie);
            }
            else
            {
                userRoleCookie.Value = user.RoleId.ToString();
                HttpContext.Current.Response.Cookies.Set(userRoleCookie);
            }

            // Store userId for log
            var userLogoutCookie = HttpContext.Current.Request.Cookies[Constanst.UserLogoutCookie];
            if (userLogoutCookie == null)
            {
                userLogoutCookie = new HttpCookie(Constanst.UserLogoutCookie, (user.Id * 11).ToString()); //for security
                HttpContext.Current.Response.Cookies.Add(userLogoutCookie);
            }
            else
            {
                userLogoutCookie.Value = (user.Id * 11).ToString();
                HttpContext.Current.Response.Cookies.Set(userLogoutCookie);
            }

            if (!string.IsNullOrEmpty(reportingCookie))
            {
                var httpCookie = HttpContext.Current.Response.Cookies[Constanst.LKARCookie];
                if (httpCookie == null) return;
                httpCookie.Value = reportingCookie;
                httpCookie.Expires = DateTime.Now.AddMinutes(TimeOutMinutes);
                httpCookie.Domain = linkitARCookieDoman;
                httpCookie.HttpOnly = true;
                httpCookie.Secure = formsCookie.Secure;
            }
        }

        private void SetWalkmeSnippet(User objUser)
        {
            objUser.WalkmeSnippetURL = string.Empty;
            var districtDecode = _districtDecodeService.GetDistrictDecodeOfDistrictOrConfigurationByLabel(objUser.DistrictId.GetValueOrDefault(), DistrictDecodeLabelConstant.WalkmeSnippetURL);
            if (districtDecode != null && districtDecode.Value != null)
            {
                objUser.WalkmeSnippetURL = !string.IsNullOrEmpty(districtDecode.Value) ? districtDecode.Value : string.Empty;
            }
        }

        public void RefreshFormsAuthCookie(User user, bool createPeristentCookie, bool isImpersonate)
        {
            var formsCookie = GetFormsAuthCookie(user, createPeristentCookie, isImpersonate);
            HttpContext.Current.Response.Cookies.Add(formsCookie);
        }

        private HttpCookie GetFormsAuthCookie(User user, bool createPeristentCookie, bool isImpersonate)
        {
            var authTicket = GetFormsAuthTicket(user, createPeristentCookie, isImpersonate);
            string encTicket = FormsAuthentication.Encrypt(authTicket);

            var formsAuthCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket)
                {
                    HttpOnly = true,
                    Path = FormsAuthentication.FormsCookiePath,
                    Secure = FormsAuthentication.RequireSSL
                };

            if (FormsAuthentication.CookieDomain != null)
            {
                formsAuthCookie.Domain = FormsAuthentication.CookieDomain;
            }
            //if (authTicket.IsPersistent)
            //{
            //    formsAuthCookie.Expires = authTicket.Expiration;
            //}
            string domain = FormsAuthentication.CookieDomain;
            formsAuthCookie.Domain = domain;
            formsAuthCookie.Expires = DateTime.Now.AddMinutes(TimeOutMinutes);
            return formsAuthCookie;
        }

        private FormsAuthenticationTicket GetFormsAuthTicket(User user, bool createPeristentCookie, bool isImpersonate)
        {
            
            var principal = Mapper.Map<User, IUserCookieData>(user);
            var principalMin = new IUserCookieDataMin(principal);
            var userData = UserPrincipal.GetCookieUserData(principalMin, isImpersonate);
            var authTicket = new FormsAuthenticationTicket(1, principal.Username, DateTime.Now, DateTime.Now.AddMinutes(TimeOutMinutes), createPeristentCookie, userData, FormsAuthentication.FormsCookiePath);

            return authTicket;
        }

        private string GetReportingCookieInternal(User user)
        {
            try
            {
                return AuthenticateAR2(user.UserName, user.HashedPassword, user.StateId ?? 0, user.DistrictId ?? 0);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        private string GetReportingCookie(User user)
        {
            var client = new RestClient();
            var loginUrl = CreateLoginUrl(user);
            var request = new RestRequest(loginUrl);
            try
            {
                var response = client.Execute(request);
                var xml = response.Content;
                var xElement = XElement.Parse(xml);
                return xElement.Value;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public void SignOut()
        {
            SetExpireTokenSQL();
            FormsAuthentication.SignOut();
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();

            HttpContext.Current.Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));

            // clearup all cookies and sessions
            foreach (string cookie in HttpContext.Current.Request.Cookies.AllKeys)
                ClearCookies(cookie);
            foreach (string cookie in HttpContext.Current.Response.Cookies.AllKeys)
                ClearCookies(cookie);

            ClearCookies(Constanst.DefaultDateFormat);
            ClearCookies(Constanst.DefaultTimeFormat);
            ClearCookies(Constanst.DefaultJqueryDateFormat);
            ClearCookies(Constanst.DefaultDateFormat);
            ClearCookies(Constanst.DefaultTimeFormat);
            ClearCookies(Constanst.DefaultJqueryDateFormat);
            ClearCookies(Constanst.WarningExpire);
        }

        public void ClearCookies(string name)
        {
            var httpCookie = HttpContext.Current.Response.Cookies[name];

            if (httpCookie == null)
            {
                httpCookie = new HttpCookie(name, "")
                {
                    Expires = DateTime.Now.AddYears(-1)
                };
                HttpContext.Current.Response.Cookies.Add(httpCookie);
            }
            else
            {
                // Clear the existing cookie by setting its expiration time to a past value
                httpCookie.Expires = DateTime.Now.AddYears(-1);
                HttpContext.Current.Response.SetCookie(httpCookie);
            }
        }

        public string GetRedirectUrl()
        {
            return FormsAuthentication.GetRedirectUrl(string.Empty, false);
        }

        private static string CreateLoginUrl(User user)
        {
            var loginUrl = ConfigurationManager.AppSettings["FlashTokenUrl"];
            loginUrl += string.Format("?UserName={0}&Password={1}&DistrictID={2}&StateID={3}",
                                      HttpContext.Current.Server.UrlEncode(user.UserName),
                                      HttpContext.Current.Server.UrlEncode(user.HashedPassword),
                                      HttpContext.Current.Server.UrlEncode(user.DistrictId.ToString()),
                                      HttpContext.Current.Server.UrlEncode(user.StateId.ToString()));
            return loginUrl;
        }
        public void ResetAuthCookie(HttpCookie authCookie)
        {
            /* Reset expiry time for authentication cookie */
            authCookie.Expires = DateTime.Now.AddMinutes(TimeOutMinutes);
            string domain = System.Web.Security.FormsAuthentication.CookieDomain;
            

            authCookie.Domain = domain;

            /* Reset expiry time for authentication ticket */
            var formsAuthTicket = authCookie.Value ?? string.Empty;
            if (!string.IsNullOrEmpty(formsAuthTicket))
            {
                var oldAuthTicket = FormsAuthentication.Decrypt(formsAuthTicket);
                if (oldAuthTicket != null)
                {
                    var authTicket = new FormsAuthenticationTicket(1, oldAuthTicket.Name, DateTime.Now,
                        DateTime.Now.AddMinutes(TimeOutMinutes), oldAuthTicket.IsPersistent,
                        oldAuthTicket.UserData, FormsAuthentication.FormsCookiePath);

                    string encTicket = FormsAuthentication.Encrypt(authTicket);
                    authCookie.Value = encTicket;
                }
            }

            // reset timeout LKARCookie
            var linkitARCookieDoman = ConfigurationManager.AppSettings["LKARCookieDomainName"];
            linkitARCookieDoman = string.IsNullOrEmpty(linkitARCookieDoman) ? authCookie.Domain : linkitARCookieDoman.Trim();
            var lKARCookie = System.Web.HttpContext.Current.Request.Cookies[LinkIt.BubbleSheetPortal.Common.Constanst.LKARCookie];
            if (lKARCookie != null)
            {
                lKARCookie.Expires = DateTime.Now.AddMinutes(TimeOutMinutes);
                lKARCookie.Domain = linkitARCookieDoman;
                System.Web.HttpContext.Current.Response.Cookies.Set(lKARCookie);
            }

            System.Web.HttpContext.Current.Response.Cookies.Set(authCookie);

            var waringExpire = HttpContext.Current.Request.Cookies.Get(Constanst.WarningExpire);
            if (waringExpire == null) waringExpire = new HttpCookie(Constanst.WarningExpire);

            waringExpire.Domain = linkitARCookieDoman;
            waringExpire.Expires = DateTime.UtcNow.AddMinutes(TimeOutMinutes);
            waringExpire.Value = waringExpire.Expires.ToString();

            System.Web.HttpContext.Current.Response.Cookies.Set(waringExpire);
        }
        private static int timeoutMinutes = -1;//default
        public int TimeOutMinutes
        {
            get
            {
                if (timeoutMinutes > 0)
                {
                    return timeoutMinutes;
                }

                try
                {
                    timeoutMinutes = _configurationService.GetConfigurationByKeyWithDefaultValue(Constanst.DefaultCookieTimeOut, (int)FormsAuthentication.Timeout.TotalMinutes);
                }
                catch
                {
                    return 30;
                }
                return timeoutMinutes;
            }
        }

        string CreateTokenSQL(User objUser)
        {
            var currentSessionID = HttpContext.Current.Session.SessionID;
            var lKARCookie = HttpContext.Current.Request.Cookies[Constanst.LKARCookie];

            var objToken = userService.GetTokenSQL(lKARCookie?.Value);

            if (objToken != null && objToken.UserID == objUser.Id)
            {
                if (objToken.CKSession == currentSessionID && objToken.expires >= DateTime.UtcNow)
                {
                    return objToken.SessionToken;
                }

                objToken.CKSession = currentSessionID;
                objToken.expires = DateTime.UtcNow.AddDays(1);
            }
            else
            {
                objToken = new ASPSession();

                //Set the User ID
                objToken.UserID = objUser.Id;
                //Set the User name
                objToken.UserName = objUser.UserName;
                //Set the DateLastAccessed as UTC time
                objToken.expires = DateTime.UtcNow.AddDays(1);
                //Set the GUID for the token
                objToken.SessionToken = Guid.NewGuid().ToString();
                objToken.CKSession = currentSessionID;
            }

            userService.SaveTokenSQL(objToken);

            //Return the string value for the GUID
            return objToken.SessionToken.ToString();
        }

        private void SetExpireTokenSQL()
        {
            var lKARCookie = HttpContext.Current.Request.Cookies[Constanst.LKARCookie];
            var objToken = userService.GetTokenSQL(lKARCookie?.Value);

            if (objToken == null || string.IsNullOrEmpty(objToken.SessionToken))
            {
                return;
            }

            objToken.expires = DateTime.UtcNow.AddSeconds(-1);
            userService.SaveTokenSQL(objToken);
        }

        string AuthenticateAR2(string userName, string password, int stateID, int districtID)
        {
            //Create the return string object
            string strReturn = string.Empty;

            //Check if a valid StateID was passed in
            //A value of 0 indicates a globally unique user
            if (stateID != 0)
            {
                //Check if the StateID is valid
                var objState = stateService.GetStateById(stateID);
                //State objState = State.GetStateByStateID(StateID);

                if (objState == null)
                    throw new Exception("The StateID provided is not valid");
            }

            //Check if a valid DistrictID was passed in
            //A valud of 0 indicates a user unique to the state
            if (districtID != 0)
            {
                //Check if the DistrictID is valid
                var objDistrict = districtService.GetDistrictById(districtID);
                //District objDistrict = District.GetDistrictByDistrictID(DistrictID);

                //Check if the DistrictID is null
                if (objDistrict == null)
                    throw new Exception("The DistrictID provided is not valid");
            }

            //get a list of User objects
            var lstUsers = userService.GetUsersByUserNameandHashedPassword(userName, password);
            //lstUsers = LinkItEntity.User.GetUsersByUserNameandHashedPassword(UserName, Password);

            //Check if the user count is 0
            if (lstUsers.Count == 0)
            {
                //Return a string of nodata.  This tells the ARM that there was no match at all for the credentials
                strReturn = "badcredentials";
            }
            //Otherwise, is there one entry returned
            else if (lstUsers.Count == 1)
            {
                //Create a user object
                var objUserfromList = lstUsers[0];
                //LinkItEntity.User objUserfromList = lstUsers[0];

                //Check if the user is a unique global user


                if (objUserfromList.IsPublisher)
                //if (LinkItEntity.User.IsGlobalUser(objUserfromList) == true)
                {
                    //If so, get the token to be returned
                    strReturn = CreateTokenSQL(objUserfromList);
                }
                //if not, check to see if the StateID matches
                else
                {
                    if (stateID == 0)
                    {
                        strReturn = "";
                    }
                    else
                    {
                        //check if districtid matches
                        if (districtID == 0)
                        {
                            strReturn = "";
                        }
                        else
                        {
                            if (objUserfromList.DistrictId != districtID)
                            {
                                strReturn = "";
                            }
                            else
                            {
                                //If so, get the token to be returned
                                strReturn = CreateTokenSQL(objUserfromList);
                            }
                        }
                    }
                }
            }
            //If there is more than one user returned
            else
            {
                //Check for the DistrictID among the collection of users
                if (districtID != 0)
                {
                    //Check if there is " + LabelHelper.DistrictLabel + "
                    bool districtMatch = lstUsers.Exists(ElementWithThisIdExists(districtID));

                    //Check the value of the " + LabelHelper.DistrictLabel + " match
                    if (districtMatch == false)
                    {
                        //Throw an exception
                        throw new Exception("No match for the User/" + LabelHelper.DistrictLabel + " combination");
                    }
                    else
                    {
                        //Set the user object
                        User objSelectUser = lstUsers.Find(ElementWithThisIdExists(districtID));

                        //Get a token value to return
                        strReturn = CreateTokenSQL(objSelectUser);
                    }
                }
            }

            //Return the token
            return strReturn;
        }
        static Predicate<User> ElementWithThisIdExists(int matchDistrictID)
        {
            return delegate(User objUser)
            {
                if (objUser.DistrictId == matchDistrictID)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            };
        }
        /// <summary>
        /// Make sure there's only one session for each username working on system
        /// </summary>
        /// <returns>True if user can continue working</returns>

        public bool EnsureSingleSignOn()
        {
            var user = HttpContext.Current.User as UserPrincipal;
            if (user == null)
                return false;

            var configurationService = DependencyResolver.Current.GetService<ConfigurationService>();
            var validLoginWords = configurationService.GetConfigurationByKey(Constanst.IgnoreSingleLoginWord);
            var validLoginWordList = validLoginWords == null || string.IsNullOrEmpty(validLoginWords.Value) ? new string[0] : validLoginWords.Value.Split(';');
            bool isForcePassLogin = false;
            foreach (var validLoginWord in validLoginWordList)
            {
                if (user.UserName.IndexOf(validLoginWord, StringComparison.OrdinalIgnoreCase) >= 0
                    && user.LocalCode.IndexOf(validLoginWord, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    isForcePassLogin = true;
                    break;
                }
            }

            if (isForcePassLogin)
                return true;

            //pass through
            HttpCookie passThroughCookie = HttpContext.Current.Request.Cookies["UserPassThrough"];
            if (passThroughCookie != null)
            {
                string userId = string.Empty;

                userId = passThroughCookie["PassThroughUserID"];
                var gUIDSession = passThroughCookie["GUIDSession"];
                if (!string.IsNullOrEmpty(userId))
                {
                    var userLogon = _userLogonService.GetById(Int32.Parse(userId));
                    if (userLogon != null)
                    {
                        if (userLogon.GUIDSession != gUIDSession)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }

            var ableToContinue = true;
            string cookieName = FormsAuthentication.FormsCookieName;
            HttpCookie authCookie = HttpContext.Current.Request.Cookies[cookieName];
            if (authCookie != null && !string.IsNullOrEmpty(authCookie.Value))
            {
                 var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                var userCookie = UserPrincipal.CreatePrincipalFromCookieData(authTicket.UserData);
                var userLogon = _userLogonService.GetById(userCookie.Id);
                if (userLogon != null)
                {
                    if (userCookie.OriginalID > 0) //A user is impersonating
                    {
                        ableToContinue = true;//always allow impersonated user to work
                    }
                    else
                    {
                        //Check if there's another new log on of this user
                        if (userLogon.GUIDSession != userCookie.GUIDSession)
                        {
                            //There's another log on -> log out this session
                            ableToContinue = false;
                            //Sign out: Clear cookie, session

                        }
                    }
                    
                }
                else
                {
                    ableToContinue = userCookie.OriginalID > 0; //allow to continue when impersonate
                }
            }
            return ableToContinue;
        }
    }
}
