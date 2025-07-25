using System.Collections.Generic;
using System.Xml.Serialization;

namespace LinkIt.BubbleSheetPortal.Models.ETL
{
    public class LookupMapping : BaseMapping
    {
        [XmlArray("lookup")]
        [XmlArrayItem("value")]
        public List<LookupData> LookupValue { get; set; }
    }
}