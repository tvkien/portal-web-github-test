using System.Collections.Generic;
using System.Xml.Serialization;

namespace LinkIt.BubbleSheetPortal.Models.ETL
{
    public class TestMapping
    {
        [XmlAttribute("id")]
        public int ID { get; set; }

        [XmlArray("mappings")]
        [XmlArrayItem("mapping")]
        public List<BaseMapping> MappingList { get; set; }
    }
}