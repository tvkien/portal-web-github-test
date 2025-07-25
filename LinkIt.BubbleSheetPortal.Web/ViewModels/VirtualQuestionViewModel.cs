namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class VirtualQuestionViewModel
    {
        public int QTIItemID { get; set; }
        public int QuestionOrder { get; set; }
        public string CorrectAnswer { get; set; }
        public int NumberOfChoices { get; set; }
        public int PointsPossible { get; set; }
        public int QTISchemaID { get; set; }
        public string AnswerIdentifiers { get; set; }
        public int VirtualQuestionId { get; set; }

        public bool? IsRubricBasedQuestion { get; set; }
    }
}
