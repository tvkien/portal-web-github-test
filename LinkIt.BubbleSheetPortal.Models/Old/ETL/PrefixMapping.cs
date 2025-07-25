using System.Xml.Serialization;

namespace LinkIt.BubbleSheetPortal.Models.ETL
{
    public class PrefixMapping : BaseMapping
    {
        [XmlAttribute("prefix")]
        public string Prefix { get; set; }
    }
}