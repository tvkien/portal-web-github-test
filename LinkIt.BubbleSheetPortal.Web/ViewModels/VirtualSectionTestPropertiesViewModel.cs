using System.Collections.Generic;
using System.Web.Mvc;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class VirtualSectionTestPropertiesViewModel
    {
        public VirtualSectionTestPropertiesViewModel()
        {
            VirtualSectionId = 0;
            VirtualTestId = 0;
            Title = string.Empty;
            Instruction = string.Empty;
            ConversionSetId = 0;
            AvailableConversionSets = new List<SelectListItem>();
            IsTutorialSection = false;
            VirtualSectionOrder = 0;
            VirtualSectionOrders = new List<SelectListItem>();
        }
        public int VirtualSectionId { get; set; }
        public int VirtualTestId { get; set; }
        public string Title { get; set; }
        public string Instruction { get; set; }
        public int? ConversionSetId { get; set; }
        public List<SelectListItem> AvailableConversionSets { get; set; }
        public string AudioRef { get; set; }
        public bool IsTutorialSection { get; set; }
        public int VirtualSectionOrder { get; set; }
        public List<SelectListItem> VirtualSectionOrders { get; set; }
    }
}
