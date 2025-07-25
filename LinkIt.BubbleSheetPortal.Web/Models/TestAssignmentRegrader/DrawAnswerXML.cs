using System.Collections.Generic;
using System.Xml.Serialization;
namespace LinkIt.BubbleSheetPortal.Web.Models.TestAssignmentRegrader
{
    public class DrawAnswerXML
    {
        [XmlArray("Types"), XmlArrayItem("Type", Type = typeof(Type))]
        public List<Type> Types;
    }

    public class Type
    {
        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }

        [XmlArray("Contents"), XmlArrayItem("Content", Type = typeof(Content))]
        public List<Content> Contents;

        [XmlArray("Passages"), XmlArrayItem("Passage", Type = typeof(Passage))]
        public List<Passage> Passages;
    }

    public class Content
    {
        [XmlElement("Src")]
        public string Source { get; set; }

        [XmlElement("Data")]
        public string Data { get; set; }

        [XmlElement("Index")]
        public string Index { get; set; }
    }

    public class Passage
    {
        [XmlElement("RefObjectID")]
        public string RefObjectID { get; set; }

        [XmlArray("Contents"), XmlArrayItem("Content", Type = typeof(Content))]
        public List<Content> Contents;
    }
}