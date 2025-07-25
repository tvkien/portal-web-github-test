using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class ItemTagListItemViewModel
    {

        public ItemTagListItemViewModel()
        {
        }
        public int ItemTagID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CountQtiItem { get; set; }

        public int ItemTagCategoryID { get; set; }
        public string CategoryName { get; set; }
        
        

    }
}