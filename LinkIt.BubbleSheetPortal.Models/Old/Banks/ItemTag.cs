using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ItemTag
    {
        public int ItemTagID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public int ItemTagCategoryID { get; set; }
        public string Category { get; set; }
        public string CategoryDescription { get; set; }
        public int CountQtiItem { get; set; }
        public int DistrictID { get; set; }
        public int? Order { get; set; }


    }
}

