using System.Collections.Generic;
using Newtonsoft.Json;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class Select2ListItem
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Text { get; set; }

        [JsonProperty("subs", NullValueHandling = NullValueHandling.Ignore)]
        public List<Select2ListItem> Childrens { get; set; }

        [JsonProperty("selected", NullValueHandling = NullValueHandling.Ignore)]
        public string Selected { get; set; }

        [JsonProperty("isSelectable", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsSelectable { get; set; } = true;
    }
}
