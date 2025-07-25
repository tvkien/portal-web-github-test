using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class MasterStandardResourceViewModel
    {
        public string GUID { get; set; }
        public int MasterStandardId { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }
        public int CountChildren { get; set; }
        public string ParentGUID { get; set; }
        public string State { get; set; }
    }
}