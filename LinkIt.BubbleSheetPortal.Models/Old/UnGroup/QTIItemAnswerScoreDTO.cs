using System;

namespace LinkIt.BubbleSheetPortal.Models
{
    [Serializable()]
    public class QTIItemAnswerScoreDTO
    {
        public int QTIItemAnswerScoreID { get; set; }
        public int QTIItemID { get; set; }
        public string ResponseIdentifier { get; set; }
        public string Answer { get; set; }
        public int Score { get; set; }
        public int? VirtualQuestionAnswerScore { get; set; }
    }
}
