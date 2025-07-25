namespace LinkIt.BubbleSheetPortal.InteractiveRubric.Models.Entities
{
    public class RubricCategoryTag
    {
        public int RubricCategoryTagID { get; set; }

        public int RubricQuestionCategoryID { get; set; }

        public int TagID { get; set; }

        public string TagType { get; set; }

        public string TagName { get; set; }

        public string TagDescription { get; set; }

        public int? TagCategoryID { get; set; }

        public string TagCategoryName { get; set; }

        public int? VirtualQuestionID { get; set; }
    }
}
