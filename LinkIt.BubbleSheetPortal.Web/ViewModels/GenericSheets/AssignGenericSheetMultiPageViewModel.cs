using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.GenericSheets
{
    public class AssignGenericSheetMultiPageViewModel
    {
        public AssignGenericSheetMultiPageViewModel()
        {
            ListAssignGenericSheetViewModels = new List<AssignGenericSheetViewModel>();
        }
        public List<AssignGenericSheetViewModel> ListAssignGenericSheetViewModels { get; set; } 
    }
}