using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.DTOs
{
    public class RubricCategoryTagDto
    {
        public int RubricCategoryTagID { get; set; }

        public int RubricQuestionCategoryID { get; set; }
        public string RubricQuestionCategoryName { get; set; }

        public int TagID { get; set; }

        public string TagType { get; set; }

        public string TagName { get; set; }

        public string TagDescription { get; set; }

        public int TagCategoryID { get; set; }

        public string TagCategoryName { get; set; }

        public int VirtualQuestionID { get; set; }
    }

    public class RubricQuestionCategoryTag
    {
        public int VirtualQuestionID { get; set; }
        public int TagID { get; set; }
        public string TagType { get; set; }
        public string RubricQuestionCategoryIDs { get; set; }
    }

    public class RubricTagByCategoryDisplay
    {
        public int VirtualQuestionID { get; set; }
        public List<string> Standards { get; set; } = new List<string>();
        public List<string> Topics { get; set; } = new List<string>();
        public List<string> Skills { get; set; } = new List<string>();
        public List<string> Others { get; set; } = new List<string>();
        public List<string> Customs { get; set; } = new List<string>();
    }
}
