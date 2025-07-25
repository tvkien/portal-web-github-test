using Newtonsoft.Json;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.LTI
{
    public class PlatformClaimValueType
    {
        [JsonProperty("contact_email")]
        public string ContactEmail { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("guid")]
        public string Guid { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("product_family_code")]
        public string ProductFamilyCode { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }
    }
}
