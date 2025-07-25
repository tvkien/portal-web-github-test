using System.Collections.Generic;
using System.Xml.Serialization;
namespace LinkIt.BubbleSheetPortal.Web.Models.TestAssignmentRegrader
{
    [XmlRoot("DrawAnswer")]
    public class DrawAnswerDOM
    {
        [XmlArrayItem("DrawImg", Type = typeof(DrawImg))]
        public List<DrawImg> DrawImgs;
    }

    public class DrawImg
    {
        [XmlAttribute("Type")]
        public string Type { get; set; }

        [XmlAttribute("Src")]
        public string Source { get; set; }

        [XmlAttribute("Data")]
        public string Data { get; set; }

        [XmlAttribute("Index")]
        public string Index { get; set; }

        [XmlAttribute("RefObjectID")]
        public string RefObjectID { get; set; }
    }
}