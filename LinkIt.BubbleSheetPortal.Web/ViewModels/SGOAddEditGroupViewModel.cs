using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class SGOAddEditGroupViewModel
    {
        public int SgoId { get; set; }
        public List<ListItemsViewModel> LstGroups;

        public SGOAddEditGroupViewModel()
        {
            LstGroups = new List<ListItemsViewModel>();
        }
    }
}