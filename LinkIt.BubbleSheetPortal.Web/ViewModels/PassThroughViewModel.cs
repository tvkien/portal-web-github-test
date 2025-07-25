using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class PassThroughViewModel
    {
        public string UserID { get; set; }
        public string LandingPage  { get; set; }
        public string RedirectUrl { get; set; }
        public string Timestamp { get; set; }
        public int? StudentID { get; set; }
        public string ActiveTab { get; set; }
    }
}
