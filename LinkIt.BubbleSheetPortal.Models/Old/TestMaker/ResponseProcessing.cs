using System.Xml.Serialization;

namespace LinkIt.BubbleSheetPortal.Models.TestMaker
{
    [XmlRoot(ElementName = "process")]
    public class ResponseProcessing
    {
        [XmlAttribute("method")]
        public string Method { get; set; }

        [XmlAttribute("caseSensitive")]
        public string CaseSensitive { get; set; }
        [XmlAttribute("spelling")]
        public string Spelling { get; set; }

        [XmlAttribute("spellingDeduction")]
        public string SpellingDeduction { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlElement("correctResponses")]
        public CorrectResponse CorrectResponse;
    }
}
