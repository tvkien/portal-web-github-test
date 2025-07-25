namespace LinkIt.BubbleSheetPortal.Models
{
    public class ChoiceVariableVirtualQuestionAnswerScore
    {
        public int VirtualQuestionAnswerScoreId { get; set; }
        public int VirtualQuestionId { get; set; }
        public string QtiItemScore { get; set; }
        public int Score { get; set; }
        public string Answer { get; set; }
        public string ResponseIdentifier { get; set; }
        public int? QtiSchemaID { get; set; }
    }
}
