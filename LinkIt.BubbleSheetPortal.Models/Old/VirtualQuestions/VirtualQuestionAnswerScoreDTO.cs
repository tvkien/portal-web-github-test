using System;

namespace LinkIt.BubbleSheetPortal.Models
{
    [Serializable()]
    public class VirtualQuestionAnswerScoreDTO
    {
        public int VirtualQuestionAnswerScoreID { get; set; }
        public int VirtualQuestionID { get; set; }
        public int QTIItemAnswerScoreID { get; set; }
        public int Score { get; set; }
    }
}
