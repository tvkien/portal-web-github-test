using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.SGO
{
    [XmlRoot("SGOStudentData")]
    public class SGOStudentData
    {
        [XmlElement(ElementName = "ClassId")]
        public int ClassId { get; set; }
        [XmlElement(ElementName = "StudentId")]
        public int StudentId { get; set; }

        public SGOStudentData()
        {
            ClassId = 0;
            StudentId = 0;
        }
    }
}