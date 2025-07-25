using LinkIt.BubbleSheetPortal.Web.Helpers.LTI.Enums;
using Newtonsoft.Json;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.LTI
{
    public class ContextClaimValueType
    {
        [JsonProperty("id", Required = Required.Always)]
        public string Id { get; set; }
        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("type")]
        public ContextType[] Type { get; set; }
    }
}
