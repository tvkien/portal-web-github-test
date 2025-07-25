using System.Xml.Serialization;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class TestSettingToolDTO
    {
        [XmlElement(ElementName = "simplePalette")]
        public string SimplePalette { get; set; }

        [XmlElement(ElementName = "mathPalette")]
        public string MathPalette { get; set; }

        [XmlElement(ElementName = "spanishPalette")]
        public string SpanishPalette { get; set; }

        [XmlElement(ElementName = "frenchPalette")]
        public string FrenchPalette { get; set; }

        [XmlElement(ElementName = "protractor")]
        public string Protractor { get; set; }

        [XmlElement(ElementName = "supportCalculator")]
        public string SupportCalculator { get; set; }

        [XmlElement(ElementName = "scientificCalculator")]
        public string ScientificCalculator { get; set; }

        public TestSettingToolDTO()
        {
            SimplePalette = string.Empty;
            MathPalette = string.Empty;
            SpanishPalette = string.Empty;
            FrenchPalette = string.Empty;
            Protractor = string.Empty;
            SupportCalculator = string.Empty;
            ScientificCalculator = string.Empty;
        }
    }
}
