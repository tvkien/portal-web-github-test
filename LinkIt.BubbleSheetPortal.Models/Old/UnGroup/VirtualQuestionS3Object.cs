using System;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class VirtualQuestionS3Object
    {
        public int VirtualQuestionID { get; set; }
        public int QTIItemID { get; set; }
        public int QTISchemaID { get; set; }
        public int QTIGroupID { get; set; }
        public string XmlContent { get; set; }
        public int PointsPossible { get; set; }
        public int QuestionOrder { get; set; }
        public int? VirtualSectionQuestionID { get; set; }
        public int? VirtualSectionQuestionOrder { get; set; }
        public int? VirtualSectionID { get; set; }
        public int? BaseVirtualQuestionID { get; set; }
        public string QuestionLabel { get; set; }
    }
}
