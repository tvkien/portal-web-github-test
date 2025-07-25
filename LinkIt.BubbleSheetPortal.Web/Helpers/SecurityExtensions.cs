using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Web.Security;

namespace LinkIt.BubbleSheetPortal.Web.Helpers
{
    public static class SecurityExtensions
    {
        public static bool IsNotTeacher(this User user)
        {
            if (user.IsNull())
            {
                return false;
            }

            return user.RoleId != (int) Permissions.Teacher;
        }

        public static bool IsLinkItAdminOrPublisher(this User user)
        {
            if (user.IsNull())
            {
                return false;
            }

            return user.RoleId == (int) Permissions.LinkItAdmin || IsPublisher(user);
        }

        public static bool IsTeacher(this User user)
        {
            if(user.IsNull())
            {
                return false;
            }

            return user.RoleId == (int) Permissions.Teacher;
        }

        public static bool IsSchoolAdmin(this User user)
        {
            if (user.IsNull())
            {
                return false;
            }

            return user.RoleId == (int)Permissions.SchoolAdmin;
        }

        public static bool IsPublisher(this User user)
        {
            if (user.IsNull())
            {
                return false;
            }

            return user.RoleId == (int) Permissions.Publisher;
        }

        public static bool IsNetworkAdmin(this User user)
        {
            if (user.IsNull())
            {
                return false;
            }

            return user.RoleId == (int)Permissions.NetworkAdmin;
        }

        public static User GetCurrentUser(this HttpContext httpContext)
        {
            if (httpContext.Request.Cookies[FormsAuthentication.FormsCookieName] == null) return null;

            var formsAuthTicket = httpContext.Request.Cookies[FormsAuthentication.FormsCookieName].Value ?? string.Empty;
            if (string.IsNullOrEmpty(formsAuthTicket))
            {
                return null;
            }

            var ticket = FormsAuthentication.Decrypt(formsAuthTicket);
            if (ticket.IsNull())
            {
                return null;
            }

            return UserPrincipal.CreatePrincipalFromCookieData(ticket.UserData);
        }

        public static int GetCurrentDistrictID(this HttpContext httpContext)
        {
            int districtID = 0;

            HttpCookie passThroughCookie = httpContext.Request.Cookies["UserPassThrough"];
            if (passThroughCookie != null)
            {
                var passThroughDistrictID = passThroughCookie["PassThroughDistrictID"];

                return !string.IsNullOrEmpty(passThroughDistrictID) && int.TryParse(passThroughDistrictID, out districtID) ? districtID : 0;
            }

            var currentUser = httpContext.GetCurrentUser();
            if(currentUser != null && currentUser.DistrictId.HasValue)
            {
                return currentUser.DistrictId.Value;
            }

            districtID = HelperExtensions.GetDistrictIdBySubdomain();
            return districtID;
        }

        public static bool IsInAdminRole(this User user)
        {
            if (user.IsNull())
            {
                return false;
            } 
            return user.RoleId != 0;//(int) Permissions.Teacher;
        }

        /// <summary>
        /// Check if the input district is correct for the current user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="districtId"></param>
        /// <returns></returns>
        public static bool IsCorrectDistrict(this User user, int districtId)
        {
            if (!user.IsPublisherOrNetworkAdmin)
            {
                return districtId == user.DistrictId;
            }

            if (user.IsNetworkAdmin)
            {
                return user.GetMemberListDistrictId().Contains(districtId);
            }

            return true;
        }

        public static bool IsAdmin(this User user)
        {
            if (user.IsNull()) return false;

            var adminRoles = new List<int>
            {
                (int)Permissions.SchoolAdmin,
                (int)Permissions.DistrictAdmin,
                (int)Permissions.NetworkAdmin,
                (int)Permissions.Publisher
            };

            return adminRoles.Contains(user.RoleId);
        }
    }
}
