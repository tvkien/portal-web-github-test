using System;
using System.Security.Policy;
using Newtonsoft.Json;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.SSO.Clever
{
    public class CleverGetAccessTokenDto
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("grant_type")]
        public string GrantType = "authorization_code";

        [JsonProperty("redirect_uri")]
        public string RedirectUri { get; set; }

        public CleverGetAccessTokenDto(string code, string redirectUri)
        {
            this.Code = code;
            this.RedirectUri = redirectUri;
        }
    }

    public class CleverTokenInfoDto
    {
        [JsonProperty("id")]
        public string UserId { get; set; }
        // only has value in case use instant login link and login by clever portal
        [JsonProperty("district")]
        public string District { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("authorized_by")]
        public string AuthorizedBy { get; set; }
    }

    public class CleverTokenInfoResponse
    {
        public string Type { get; set; }
        public CleverTokenInfoDto Data { get; set; }
    }

    public class CleverUserInfoResponseDto<T>
    {
        public T Data { get; set; }
    }

    public class BaseCleverUserInfoDto
    {
        public string Email { get; set; }
        public string Id { get; set; }
        public CleverNameDto Name { get; set; }
        public CleverRoles Roles { get; set; }

        public bool IsStudent
        {
            get
            {
                if (Roles == null)
                    throw new ArgumentNullException("Role can't not null");

                if (Roles.Student != null)
                    return true;

                return false;
            }
        }
    }

    public class LoginInstantCleverUserInfoDto: BaseCleverUserInfoDto
    {
        public string District { get; set; }
    }

    public class CleverRoles
    {
        public dynamic Student { get; set; }
        public dynamic Teacher { get; set; }
        public dynamic Staff { get; set; }
        [JsonProperty("district_admin")]
        public dynamic DistrictAdmin { get; set; }
    }

    public class CleverNameDto
    {
        public string First { get; set; }
        public string Last { get; set; }
        public string Middle { get; set; }

        public string ToFullName()
        {
            return string.Format("{0}, {1}", this.Last, this.First);
        }
    }
}
