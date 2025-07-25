using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models.Constants;
using LinkIt.BubbleSheetPortal.Models.DTOs.SSO.Clever;
using Microsoft.IdentityModel.Tokens;
using RestSharp;

namespace LinkIt.BubbleSheetPortal.Web.Helpers.SSO
{
    public static class CleverHelper
    {
        public static IRestResponse SendGetRequest(string resource, string accessToken)
        {
            var client = new RestClient();
            var request = new RestRequest(resource);
            request.AddHeader("accept", "application/json");
            request.AddHeader("Authorization", $"Bearer {accessToken}");
            var response = client.Execute(request);

            return response;
        }

        public static (string UserName, int DistrictId, bool IsStudent, bool IsParent) ExtractInfoFromToken(string token)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            TokenValidationParameters parameters = new TokenValidationParameters()
            {
                RequireExpirationTime = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(Util.Secret)),
                ClockSkew = TimeSpan.Zero
            };

            SecurityToken securityToken;
            ClaimsPrincipal principal = tokenHandler.ValidateToken(token, parameters, out securityToken);

            var userName = principal.Claims.FirstOrDefault(m => m.Type == "UserName").Value;
            var districtId = int.Parse(principal.Claims.FirstOrDefault(m => m.Type == "DistrictID").Value);
            var isStudent = bool.Parse(principal.Claims.FirstOrDefault(m => m.Type == "IsStudent").Value);
            var isParent = bool.Parse(principal.Claims.FirstOrDefault(m => m.Type == "IsParent").Value);

            return new ValueTuple<string, int, bool, bool>(userName, districtId, isStudent, isParent);
        }

        public static string CreateCleverAuthorizeToken(string clientId,string secretId)
        {
            var encode = UTF8Encoding.UTF8.GetBytes(string.Format("{0}:{1}", clientId, secretId));
            return Convert.ToBase64String(encode);
        }

        public static string CreateCallbackUrl(HttpRequestBase requestUrl, string loginType, bool isPrimaryCallBack = false)
        {
            string host = $"portal.{ConfigurationManager.AppSettings[Constanst.LINKIT_URL_KEY]}";

            var callbackUrl = $"{HelperExtensions.GetHTTPProtocal(requestUrl)}://{host}/account/clevercallback";

            if (isPrimaryCallBack)
            {
                callbackUrl = callbackUrl.Replace("/account/clevercallback", "/account/primaryclevercallback");
                return callbackUrl;
            }

            if (!string.IsNullOrEmpty(loginType) && loginType == TextConstants.LOGIN_ROLE_STUDENT)
            {
                callbackUrl = callbackUrl.Replace("/account/clevercallback", "/student/clevercallback");
                return callbackUrl;
            }

            return callbackUrl;
        }

        public static string CreateProcessCallBackUrl(string requestUrl, string licode)
        {
            string oldHost = $"portal.{ConfigurationManager.AppSettings[Constanst.LINKIT_URL_KEY]}";
            string newHost = oldHost.Replace("portal", licode);
            requestUrl = requestUrl.Replace(oldHost, newHost);
            requestUrl = Regex.Replace(requestUrl,"clevercallback", "cleverprocesscallback", RegexOptions.IgnoreCase);
            return requestUrl;
        }

        public static string GeneratePrimaryCleverCallBackToken(string accessToken, bool isStudent)
        {
            var claims = new Dictionary<string, string>();
            claims.Add("isstudent", (isStudent).ToString());
            claims.Add("accesstoken", accessToken);
            var token = Util.GenerateToken(claims, 1);
            return token;
        }

        public static string CreatePrimaryProcessCleverCallBackUrl(string token, string liCode, HttpRequestBase request)
        {
            string url = $"{HelperExtensions.GetHTTPProtocal(request)}://{liCode}.{ConfigurationManager.AppSettings[Constanst.LINKIT_URL_KEY]}/account/processprimaryclevercallback?token={token}";
            return url;
        }

        public static CleverPrimaryTokenInfoDto ParseCleverToken(string token)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            TokenValidationParameters parameters = new TokenValidationParameters()
            {
                RequireExpirationTime = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(Util.Secret)),
                ClockSkew = TimeSpan.Zero
            };

            ClaimsPrincipal principal = tokenHandler.ValidateToken(token, parameters, out var securityToken);

            var accessToken = principal.Claims.First(m => m.Type.Equals("accessToken", StringComparison.OrdinalIgnoreCase)).Value;
            var isStudent = principal.Claims.First(m => m.Type.Equals("IsStudent", StringComparison.OrdinalIgnoreCase)).Value;
            Claim exp = principal.FindFirst("exp");
            var expDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp.Value)).DateTime;

            return new CleverPrimaryTokenInfoDto
            {
                IsStudent = bool.Parse(isStudent),
                AccessToken = accessToken,
                ExpireOn = expDate
            };
        }

    }
}
