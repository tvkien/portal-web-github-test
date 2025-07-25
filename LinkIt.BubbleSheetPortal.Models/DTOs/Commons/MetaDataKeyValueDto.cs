using Newtonsoft.Json;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.Commons
{
    public class MetaDataKeyValueDto
    {
        public string Label { get; set; }
        public string Name { get; set; }        
        public string Type { get; set; }
        public string Value { get; set; }
        public MetaDataKeyValueDto()
        {
            Value = string.Empty;
            Type = string.Empty;
            Label = string.Empty;
            Name = string.Empty;
        }
    }

    public class MetaDataKeyValueLabelsDto
    {
        [JsonProperty("Fields")]
        public List<MetaDataKeyValueDto> MetaDataLabels { get; set; }
    }
}
