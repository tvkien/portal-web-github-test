using LinkIt.BubbleSheetPortal.Web.Models.RestrictionDTO;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class VirtualTestViewModel
    {
        public int VirtualTestId { get; set; }
        public string Name { get; set; }
        public List<VirtualSectionViewModel> VirtualSectionList = new List<VirtualSectionViewModel>();
        public int VirtualQuestionId { get; set; }
        public int QtiItemId { get; set; }
        public int VirtualTestSubTypeID { get; set; }
        public int VirtualTestSourceId { get; set; }

        public bool IsSupportQuestionGroup { get; set; }
        public RestrictionAccessModel RestrictionAccessList { get; set; }

        public VirtualTestViewModel()
        {
            RestrictionAccessList = new RestrictionAccessModel();
        }

        public bool IsNumberQuestions { get; set; }
        public bool IsSurvey { get; set; }
        public int? NavigationMethodID { get; set; }
        public bool HasRetakeRequest { get; set; }
    }
}
