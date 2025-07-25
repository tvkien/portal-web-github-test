using LinkIt.BubbleSheetPortal.Web.Helpers.LTI.Enums;
using Newtonsoft.Json;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.LTI
{
    public class DeepLinkingSettingsClaimValueType
    {
        [JsonProperty("accept_media_types")]
        public string AcceptMediaTypes { get; set; }

        [JsonProperty("accept_multiple")]
        public bool AcceptMultiple { get; set; }

        [JsonProperty("accept_presentation_document_targets")]
        public DocumentTarget[] AcceptPresentationDocumentTargets { get; set; }

        [JsonProperty("accept_types")]
        public string[] AcceptTypes { get; set; }

        [JsonProperty("auto_create")]
        public bool AutoCreate { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }

        [JsonProperty("deep_link_return_url")]
        public string DeepLinkReturnUrl { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }
}
