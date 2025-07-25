using System.Collections.Generic;
using System.Xml.Serialization;

namespace LinkIt.BubbleSheetPortal.Models.ETL
{
    [XmlRoot("transform")]
    public class MappingRule
    {
        [XmlElement("commonfields")]
        public CommonField CommonField { get; set; }

        [XmlElement("test")]
        public List<TestMapping> TestList { get; set; }
    }
}