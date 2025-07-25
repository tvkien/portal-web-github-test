using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.ManageClass
{
    public class ClassMetaDataDto
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }


    public class ClassMetaDataLabelDto
    {
        [JsonProperty("Fields")]
        public List<ClassMetaDataDto> ClassMetaDataLabels { get; set; }
    }
}
