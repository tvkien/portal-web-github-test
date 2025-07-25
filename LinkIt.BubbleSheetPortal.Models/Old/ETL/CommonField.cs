using System.Collections.Generic;
using System.Xml.Serialization;

namespace LinkIt.BubbleSheetPortal.Models.ETL
{
    public class CommonField
    {
        [XmlArray("mappings")]
        [XmlArrayItem("mapping")]
        public List<BaseMapping> MappingList { get; set; }
    }
}