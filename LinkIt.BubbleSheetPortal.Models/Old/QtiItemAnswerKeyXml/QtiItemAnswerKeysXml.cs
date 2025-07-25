using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace LinkIt.BubbleSheetPortal.Models
{
    [XmlRoot(ElementName = "AnswerKeys")]
    public class QtiItemAnswerKeysXml
    {
        [XmlElement("AnswerKey")]
        public List<QtiItemAnswerKeyXml> AnswerKeys;
    }
}
