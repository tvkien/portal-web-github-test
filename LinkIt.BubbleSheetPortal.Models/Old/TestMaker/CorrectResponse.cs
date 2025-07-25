using System.Collections.Generic;
using System.Xml.Serialization;

namespace LinkIt.BubbleSheetPortal.Models.TestMaker
{
    public class CorrectResponse
    {
        [XmlIgnore]
        public List<string> Values;

        [XmlIgnore]
        public List<string> ValuesXML;

        //For qtiSchemeID=30
        [XmlAttribute("identifier")]
        public string Identifier { get; set; }

        [XmlAttribute("destIdentifier")]
        public string DestIdentifier { get; set; }

        [XmlAttribute("srcIdentifier")]
        public string SrcIdentifier { get; set; }

        //For qtiSchemeID=31,32,33,34
        [XmlAttribute("pointValue")]
        public int PointValue { get; set; }
        [XmlIgnore]
        public List<string> AnswerTexts;
    }
}
