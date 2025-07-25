namespace LinkIt.BubbleSheetPortal.InteractiveRubric.Models.Entities
{
    public class RubricCategoryTier
    {
        public int RubricCategoryTierID { get; set; }

        public int RubricQuestionCategoryID { get; set; }

        public decimal? Point { get; set; } = 0;

        public string Label { get; set; }

        public string Description { get; set; }

        public int OrderNumber { get; set; }
    }
}
