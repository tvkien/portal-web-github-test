using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class ItemSetViewModel
    {
        public int QTIGroupId { get; set; }
        public string Name { get; set; }
        public string AuthorGroup { get; set; }
        public int AuthorGroupId { get; set; }
        public string AuthorName { get; set; }
        public string ModifiedDate { get; set; }
    }
}