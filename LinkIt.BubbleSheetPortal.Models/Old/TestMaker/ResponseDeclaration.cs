using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace LinkIt.BubbleSheetPortal.Models.TestMaker
{
    public class ResponseDeclaration
    {
        [XmlAttribute("identifier")]
        public string Identifier { get; set; }

        [XmlAttribute("baseType")]
        public string BaseType { get; set; }

        [XmlAttribute("cardinality")]
        public string Cardinality { get; set; }

        [XmlAttribute("method")]
        public string Method { get; set; }

        [XmlAttribute("caseSensitive")]
        public string CaseSensitive { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("pointsValue")]
        public string PointsValue { get; set; }

        [XmlAttribute("spelling")]
        public string Spelling { get; set; }

        [XmlAttribute("spellingDeduction")]
        public string SpellingDeduction { get; set; }

        [XmlElement("correctResponse")]
        public List<CorrectResponse> CorrectResponses;
        //For QtiSchemaId = 30 (Drag and Drop)
        [XmlAttribute("absoluteGrading")]
        public string AbsoluteGrading { get; set; }

        [XmlAttribute("depending")]
        public string Depending { get; set; }

        [XmlAttribute("absoluteGradingPoints")]
        public string AbsoluteGradingPoints { get; set; }
        [XmlAttribute("major")]
        public string Major { get; set; }

        [XmlAttribute("partialGradingThreshold")]
        public string PartialGradingThreshold { get; set; }

        [XmlAttribute("relativeGrading")]
        public string RelativeGrading { get; set; }

        [XmlAttribute("relativeGradingPoints")]
        public string RelativeGradingPoints { get; set; }

        [XmlAttribute("range")]
        public string Range { get; set; }

        [XmlAttribute("algorithmicGrading")]
        public string AlgorithmicGrading { get; set; }

        [XmlIgnore]
        public string ResponseDeclarationXml { get; set; }

        [XmlAttribute("allOrNothingGrading")]
        public string AllOrNothingGrading { get; set; }
    }
}
