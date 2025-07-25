using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Models.DTOs;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class UpdateAnswerPointsEarnedDto
    {
        public int QTIOnlineTestSessionID { get; set; }
        public int AnswerID { get; set; }
        public int? AnswerSubID { get; set; }
        public int PointsEarned { get; set; }
        public int UserID { get; set; }
        public int VirtualQuestionID { get; set; }
        public List<RubricTestResultScoreDto> RubricTestResultScores { get; set; }
    }
}
