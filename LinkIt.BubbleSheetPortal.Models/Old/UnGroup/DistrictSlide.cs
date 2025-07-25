using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class DistrictSlide
    {
        public int DistrictId { get; set; }
        public int SlideOrder { get; set; }
        public string ImageName { get; set; }
        public string LinkTo { get; set; }
        public bool NewTabOpen { get; set; }
        public int? RoleID { get; set; }

    }
}