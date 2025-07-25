using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class MasterStandardResourceListViewModel
    {
        public int MasterStandardId { get; set; }
        public string State { get; set; }
        public string Subject { get; set; }
        public int Year { get; set; }
        public string Grade { get; set; }
        public int Level { get; set; }
        public string Label { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }
        
    }
}