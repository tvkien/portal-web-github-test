using System.Xml.Serialization;

namespace LinkIt.BubbleSheetPortal.Models.ETL
{
    public class FixedValueMapping : BaseMapping
    {
        [XmlAttribute("value")]
        public string Value { get; set; }
    }
}