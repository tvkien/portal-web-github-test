using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.Security
{
    public class UserPrincipal : User, IPrincipal, IIdentity
    {
        [ScriptIgnore]
        public string AuthenticationType
        {
            get { return "LinkItUser"; }
        }

        [ScriptIgnore]
        public bool IsAuthenticated
        {
            get { return !string.IsNullOrEmpty(UserName); }
        }

        [ScriptIgnore]
        public IIdentity Identity
        {
            get { return this; }
        }

        public bool IsInRole(string role)
        {
            throw new NotImplementedException();
        }

        public static string GetCookieUserData(IUserCookieDataMin principal, bool isImpersonate)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            principal.y = isImpersonate ? "impersonate" : System.Web.HttpContext.Current.Session.SessionID;
            return serializer.Serialize(principal);
        }

        public static UserPrincipal CreatePrincipalFromCookieData(string userData)
        {
            UserPrincipal user = null;

            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var userMin = serializer.Deserialize<IUserCookieDataMin>(userData);
                var cookieUser = new IUserCookieData(userMin);
                var cookieUserString = serializer.Serialize(cookieUser);
                user = serializer.Deserialize<UserPrincipal>(cookieUserString);
            }
            catch (Exception)
            {
                //nothing to do
            }

            if (HttpContext.Current.Session == null || HttpContext.Current.Request.Cookies["ar_authorize"] != null)
            {
                return user;
            }

            if (user != null && !string.IsNullOrEmpty(user.CKSession)
                    && (user.CKSession == HttpContext.Current.Session.SessionID || user.CKSession.StartsWith("impersonate")))
            {
                if (user.CKSession == "impersonate")
                {
                    user.CKSession = HttpContext.Current.Session.SessionID;
                    var formsAuthenticationService = DependencyResolver.Current.GetService<BubbleSheetPortal.Models.Interfaces.IFormsAuthenticationService>();
                    var userService = DependencyResolver.Current.GetService<UserService>();
                    var dbUser = userService.GetUserById(user.Id);
                    user.HashedPassword = dbUser.HashedPassword;

                    formsAuthenticationService.SignIn(user, false);
                }
                return user;
            }

            return new UserPrincipal();
        }
    }

    public static class UserExtension
    {
        public static List<int> GetMemberListDistrictId(this User user) //for NetworkAdmin
        {
            if (SessionManager.ListDistrictId != null)
            {
                return SessionManager.ListDistrictId;
            }

            if (user.IsNetworkAdmin)
            {
                //get from database
                var dspDistrictService = DependencyResolver.Current.GetService<DSPDistrictService>();
                var memberDistrict = dspDistrictService.GetDistrictsByUserId(user.Id);
                SessionManager.ListDistrictId = memberDistrict;
            }
            else
            {
                SessionManager.ListDistrictId = new List<int>();
            }

            return SessionManager.ListDistrictId;
        }
    }
}
