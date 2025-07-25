using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class QtiRefObjectList
    {
        public int QtiRefObjectID { get; set; }
        public string Name { get; set; }
        public int UserID { get; set; }
        public int DistrictID { get; set; }
        public string Subject { get; set; }
        public string GradeName { get; set; }
        public int? GradeID { get; set; }
        public string TextType { get; set; }
        public int? TextTypeID { get; set; }
        public string TextSubType { get; set; }
        public int? TextSubTypeID { get; set; }
        public string FleschKincaid { get; set; }
        public int? FleschKincaidID { get; set; }
    }
}
