namespace LinkIt.BubbleSheetPortal.Models
{
    public class QTIItemAnswerScore
    {
        public int QTIItemAnswerScoreId { get; set; }
        public int QTIItemId { get; set; }
        public string ResponseIdentifier { get; set; }
        public string Answer { get; set; }
        public string Score { get; set; }
        public string AnswerText { get; set; }
    }
}
