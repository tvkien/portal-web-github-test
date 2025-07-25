using System;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class VirtualQuestionData
    {
        public int VirtualQuestionID { get; set; }
        public int? MasterTestID { get; set; }
        public int? MasterQuestionID { get; set; }
        public int VirtualTestID { get; set; }
        public int QuestionOrder { get; set; }
        public int PointsPossible { get; set; }
        public int? ETSItemID { get; set; }
        public int? QTIItemID { get; set; }
        public int? PreQTIVirtualQuestionID { get; set; }
        public int? PreProdVQID { get; set; }
        public int? Deductions { get; set; }
        public int? BaseVirtualQuestionId { get; set; }
        public string QuestionLabel { get; set; }
        public string QuestionNumber { get; set; }
    }
}
