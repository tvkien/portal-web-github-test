namespace LinkIt.BubbleSheetPortal.Models.DTOs
{
    public class RubricCategoryTierDto
    {
        public int RubricCategoryTierID { get; set; }
        public int RubricQuestionCategoryID { get; set; }

        public int? Point { get; set; } = 0;

        public string Label { get; set; }

        public string Description { get; set; }

        public int OrderNumber { get; set; }

        public bool Selected { get; set; }

        public decimal? PointEarn => Selected ? Point : null;
    }
}
