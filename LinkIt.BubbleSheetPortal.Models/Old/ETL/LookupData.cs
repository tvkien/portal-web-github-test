using System.Xml.Serialization;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models.ETL
{
    public class LookupData : ValidatableEntity<LookupData>
    {
        [XmlAttribute("existing")]
        public string Existing { get; set; }

        [XmlAttribute("new")]
        public string New { get; set; }
    }
}