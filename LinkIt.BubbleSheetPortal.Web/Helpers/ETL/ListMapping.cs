using System.Collections.Generic;
using System.Xml.Serialization;
using LinkIt.BubbleSheetPortal.Models.ETL;

namespace LinkIt.BubbleSheetPortal.Web.Helpers.ETL
{
    [XmlRoot("local")]
    public class ListMapping
    {
        [XmlArray("mappings")]
        [XmlArrayItem("mapping")]
        public List<BaseMapping> Mappings { get; set; }
    }
}