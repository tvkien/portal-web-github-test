using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class QtiItemItemTag
    {
        public int QtiItemItemTagID { get; set; }
        public int QtiItemID { get; set; }
        public int ItemTagID { get; set; }
        public int DistricId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }
        public string TagName { get; set; }
        public string TagDescription { get; set; }
        public int ItemTagCategoryID { get; set; }

    }
}

