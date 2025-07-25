using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class HomeViewModel
    {
        public bool UseCustomSlideShow { get; set; }
        public bool ShowWidgets { get; set; }
        public string RoleUrlConfig { get; set; }
        public List<DistrictSlide> DistrictSlideList { get; set; }
    }
}
