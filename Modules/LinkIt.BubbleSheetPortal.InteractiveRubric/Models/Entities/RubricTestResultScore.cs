namespace LinkIt.BubbleSheetPortal.InteractiveRubric.Models.Entities
{
    public class RubricTestResultScore : TrackingEntity
    {
        public int RubricTestResultScoreID { get; set; }

        public int RubricQuestionCategoryID { get; set; }

        public int? QTIOnlineTestSessionID { get; set; }

        public int VirtualQuestionID { get; set; }

        public decimal? Score { get; set; }
    }
}
