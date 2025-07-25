using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.ManageStudent
{
    public class StudentMetaDataDto
    {
        public string Name { get; set; }
        public string Label { get; set; }

        public string Type { get; set; }

        public string Value { get; set; }
        public string ViewColumn { get; set; }
        public bool IsCalculatedField => !string.IsNullOrEmpty(ViewColumn);
    }

    public class StudentMetaDataLabelsDto
    {
        [JsonProperty("Fields")]
        public List<StudentMetaDataDto> StudentMetaDataLabels { get; set; }
    }
}
