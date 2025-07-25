using System.Collections.Generic;
using System.Xml.Serialization;

namespace LinkIt.BubbleSheetPortal.Web.Print
{
    public class AnswerKeyXml
    {
        [XmlArray("Answers"), XmlArrayItem("Answer", Type = typeof(AnserKeyItemXml))]
        public List<AnserKeyItemXml> AnserKeyItems { get; set; }
    }

    public class AnserKeyItemXml
    {
        public int QTIItemID { get; set; }
        public string ResponseIdentifier { get; set; }
        public int Score { get; set; }
        public string Answer { get; set; }
        public int Count { get; set; }
        public int Index { get; set; }
    }
}