namespace LinkIt.BubbleSheetPortal.InteractiveRubric.Models.Entities
{
    public class RubricQuestionCategory : TrackingEntity
    {
        public int RubricQuestionCategoryID { get; set; }

        public int VirtualQuestionID { get; set; }

        public string CategoryName { get; set; }

        public string CategoryCode { get; set; }

        public int OrderNumber { get; set; }

        public decimal? PointsPossible { get; set; }
    }
}
