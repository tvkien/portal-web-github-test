using System.Collections.Generic;
using Newtonsoft.Json;

namespace LinkIt.BubbleSheetPortal.Models.DTOs
{
    public class RubricQuestionCategoryDto : RubricQuestionCategoryItem
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal? PointEarn { get; set; }

        public List<RubricCategoryTierDto> RubricCategoryTiers { get; set; } = new List<RubricCategoryTierDto>();

        public List<RubricCategoryTagDto> RubricCategoryTags { get; set; } = new List<RubricCategoryTagDto>();
    }

    public class RubricQuestionCategorySelectList
    {
        public int VirtualQuestionID { get; set; }
        public List<RubricQuestionCategoryItem> RubricQuestionCategories { get; set; }
    }

    public class RubricQuestionCategoryItem
    {
        public int RubricQuestionCategoryID { get; set; }

        public string CategoryName { get; set; }

        public string CategoryCode { get; set; }

        public int OrderNumber { get; set; }

        public int VirtualQuestionID { get; set; }

        public decimal? PointsPossible { get; set; }
    }
}
