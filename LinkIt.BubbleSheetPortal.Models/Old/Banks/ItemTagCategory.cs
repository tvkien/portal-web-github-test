using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Common.Enum;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ItemTagCategory
    {
        public int ItemTagCategoryID { get; set; }
        public string Name { get; set; }
        public int DistrictID { get; set; }
        public string District { get; set; }
        public int StateId { get; set; }
        public string Description { get; set; }
        public int CountQtiItem { get; set; }
        public ACTPresentationType? PresentationType { get; set; }
        public int? Order { get; set; }
    }
}

