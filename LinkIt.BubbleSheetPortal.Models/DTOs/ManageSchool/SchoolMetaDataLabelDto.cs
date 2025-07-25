using Newtonsoft.Json;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class SchoolMetaDataDto
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }

    public class SchoolMetaDataLabelsDto
    {
        [JsonProperty("Fields")]
        public List<SchoolMetaDataDto> SchoolMetaDataLabels { get; set; }
    }

}
