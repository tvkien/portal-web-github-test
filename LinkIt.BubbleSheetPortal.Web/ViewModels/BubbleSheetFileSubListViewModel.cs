using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class BubbleSheetFileSubListViewModel
    {
        public List<BubbleSheetFileSubViewModel> ListFileSubViewModels;

        public BubbleSheetFileSubListViewModel()
        {
            ListFileSubViewModels = new List<BubbleSheetFileSubViewModel>();
        }
    }
}