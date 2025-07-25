using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class PassThroughVDETViewModel
    {
        public string UserID { get; set; }
        public string RedirectUrl { get; set; }
        public string Timestamp { get; set; }
        public string ActionType { get; set; }
        public string Sector { get; set; }
    }
}