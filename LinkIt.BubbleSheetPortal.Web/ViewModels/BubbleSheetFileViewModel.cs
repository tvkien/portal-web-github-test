using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class BubbleSheetFileViewModel
    {
        public BubbleSheetFileViewModel()
        {
            ListFileSubViewModels = new List<BubbleSheetFileSubViewModel>();
        }
        public int BubbleSheetFileId { get; set; }
        public List<BubbleSheetFileSubViewModel> ListFileSubViewModels;
    }
}