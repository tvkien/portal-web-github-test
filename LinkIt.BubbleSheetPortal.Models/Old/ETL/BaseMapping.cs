using System.Xml.Serialization;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models.ETL
{
    [XmlInclude(typeof(PrefixMapping))]
    [XmlInclude(typeof(LookupMapping))]
    [XmlInclude(typeof(FixedValueMapping))]
    [XmlInclude(typeof(SourceColumnMapping))]
    public abstract class BaseMapping : ValidatableEntity<BaseMapping>
    {
        [XmlAttribute("source")]
        public string Source { get; set; }

        [XmlAttribute("sourceposition")]
        public int SourcePosition { get; set; }

        [XmlAttribute("destination")]
        public string Destination { get; set; }

        [XmlAttribute("destinationid")]
        public int DestinationColumnID { get; set; }
    }
}