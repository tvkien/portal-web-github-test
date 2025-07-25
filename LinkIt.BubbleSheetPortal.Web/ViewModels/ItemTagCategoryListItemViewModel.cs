using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class ItemTagCategoryListItemViewModel
    {
        public int ItemTagCategoryID { get; set; }
        public string District { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CountQtiItem { get; set; }
    }
}