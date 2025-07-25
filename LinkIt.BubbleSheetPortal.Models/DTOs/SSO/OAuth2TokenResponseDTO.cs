using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.SSO
{
    public class OAuth2TokenResponseDTO
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("response_type")]
        public string ResponseType { get; set; }

        [JsonProperty("id_token")]
        public string IdToken { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("error_description")]
        public string ErrorDescription { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("user")]
        public OAuth2CanvasUserDTO User { get; set; }
    }
    public class OAuth2CanvasUserDTO
    {
        [JsonProperty("id")]
        public int SISId { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("global_id")]
        public string GlobalId { get; set; }
    }
}
