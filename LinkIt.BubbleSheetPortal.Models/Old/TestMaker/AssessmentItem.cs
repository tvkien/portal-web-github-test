using System.Collections.Generic;
using System.Xml.Serialization;

namespace LinkIt.BubbleSheetPortal.Models.TestMaker
{
    [XmlRoot(ElementName = "assessmentItem")]
    public class AssessmentItem
    {
        [XmlAttribute("qtiSchemeID")]
        public string QtiSchemeID { get; set; }


        [XmlAttribute("timeDependent")]
        public string TimeDependent { get; set; }

        [XmlElement("responseDeclaration")]
        public List<ResponseDeclaration> ResponseDeclarations;

        public List<ResponseIdentifier> ResponseIdentifiers { get; set; } 
    }
}
