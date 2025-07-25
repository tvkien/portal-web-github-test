using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.AggregateSubjectMapping
{
    public class MetaConfigDto
    {
        public List <LabelConfigDto> Fields { get; set; }
    }
    public class LabelConfigDto
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }
    }
}
