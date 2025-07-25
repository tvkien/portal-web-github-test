using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class EditSectionTestPropertiesViewModel
    {
        public EditSectionTestPropertiesViewModel()
        {
            XmlContent = string.Empty;
        }
        public int VirtualSectionId { get; set; }
        public string Title { get; set; }
        public string Instruction { get; set; }
        public int? ConversionSetId { get; set; }
        public string XmlContent { get; set; }
        public int VirtualTestId { get; set; }
        public string AudioRef { get; set; }
        public bool IsTutorialSection { get; set; }
        public int VirtualSectionOrder { get; set; }
    }
}
