namespace LinkIt.BubbleSheetPortal.Models
{
    public class QTIVirtualTest
    {
        public int QTIItemID { get; set; }
        public int VirtualQuestionID { get; set; }
        public int QTIItemSchemaID { get; set; }
        public int QuestionOrder { get; set; }
        public string XmlContent { get; set; }
        public int PointsPossible { get; set; }
        public string SectionInstruction { get; set; }
        public string SectionTitle { get; set; }
        public int? BaseVirtualQuestionID { get; set; }
        public string CorrectAnswer { get; set; }
        public string ResponseProcessing { get; set; }
        public int VirtualSectionMode { get; set; }
        public int? ResponseProcessingTypeID { get; set; }

        public bool? IsRubricBasedQuestion { get; set; }
    }
}
