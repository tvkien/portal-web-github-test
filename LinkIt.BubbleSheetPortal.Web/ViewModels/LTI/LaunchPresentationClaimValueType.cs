using LinkIt.BubbleSheetPortal.Web.Helpers.LTI.Enums;
using Newtonsoft.Json;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.LTI
{
    public class LaunchPresentationClaimValueType
    {
        [JsonProperty("document_target")]
        public DocumentTarget? DocumentTarget { get; set; }

        [JsonProperty("height")]
        public int? Height { get; set; }

        [JsonProperty("locale")]
        public string Locale { get; set; }

        [JsonProperty("return_url")]
        public string ReturnUrl { get; set; }

        [JsonProperty("width")]
        public int? Width { get; set; }
    }
}
