using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class VirtualQuestionXml: QtiItemAnswerKeyXml
    {
        [XmlAttribute("QtiItemId")]
        public string QtiItemId { get; set; }

        [XmlAttribute("QtiSchemaId")]
        public string QtiSchemaId { get; set; }

        [XmlAttribute("CorrectAnswer")]
        public string CorrectAnswer { get; set; }

        [XmlAttribute("NumberOfChoices")]
        public string NumberOfChoices { get; set; }

        [XmlAttribute("Points")]
        public string Points { get; set; }

        [XmlAttribute("ExtendedText")]
        public string ExtendedText { get; set; }

        [XmlAttribute("VirtualQuestionId")]
        public string VirtualQuestionId { get; set; }

    }
}
