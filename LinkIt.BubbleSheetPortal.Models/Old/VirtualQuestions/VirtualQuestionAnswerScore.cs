namespace LinkIt.BubbleSheetPortal.Models
{
    public class VirtualQuestionAnswerScore
    {
        public int VirtualQuestionAnswerScoreId { get; set; }
        public int VirtualQuestionId { get; set; }
        public int QTIItemAnswerScoreId { get; set; }
        public int Score { get; set; }
    }
}
