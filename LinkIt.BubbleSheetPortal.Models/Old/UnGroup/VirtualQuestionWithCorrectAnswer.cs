namespace LinkIt.BubbleSheetPortal.Models
{
    public class VirtualQuestionWithCorrectAnswer
    {
        public int VirtualSectionID { get; set; }
        public int VirtualQuestionID { get; set; }
        public int Order { get; set; }
        public int QTIItemID { get; set; }
        public string XmlContent { get; set; }
        public int QTIGroupID { get; set; }
        public int? VirtualSectionOrder { get; set; }
        public string VirtualSectionTitle { get; set; }
        public int VirtualSectionQuestionID { get; set; }
        public int QuestionOrder { get; set; }
        public int BaseVirtualQuestionId { get; set; }
        public int ItemNumber { get; set; }
        public int QTISchemaID { get; set; }
        public string AnswerIdentifiers { get; set; }

        public int PointsPossible { get; set; }

        public string CorrectAnswer { get; set; }
        public bool? IsRubricBasedQuestion { get; set; }
    }
}
