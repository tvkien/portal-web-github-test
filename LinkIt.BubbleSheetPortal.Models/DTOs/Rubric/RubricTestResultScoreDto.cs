namespace LinkIt.BubbleSheetPortal.Models.DTOs
{
    public class RubricTestResultScoreDto
    {
        public int RubricTestResultScoreID { get; set; }

        public int RubricQuestionCategoryID { get; set; }

        public int QTIOnlineTestSessionID { get; set; }

        public int VirtualQuestionID { get; set; }

        public int? Score { get; set; }
    }
}
