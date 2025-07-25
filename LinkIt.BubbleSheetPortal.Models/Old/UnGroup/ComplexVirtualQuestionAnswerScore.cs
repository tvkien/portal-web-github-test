namespace LinkIt.BubbleSheetPortal.Models
{
    public class ComplexVirtualQuestionAnswerScore
    {
        public int VirtualQuestionAnswerScoreId { get; set; }
        public int VirtualQuestionId { get; set; }
        public int QTIItemAnswerScoreId { get; set; }
        public int Score { get; set; }
        public string ResponseIdentifier { get; set; }
        public string SubPointsPossible { get; set; }
        public string Answer { get; set; }
        public string QtiItemScore { get; set; }
        public string QiCorrectAnswer { get; set; }
        public string QiSubCorrectAnswer { get; set; }
        public string QiSubPointsPossible { get; set; }
        public int QTISchemaID { get; set; }
        public int ResponseProcessingTypeId { get; set; }
    }
}
