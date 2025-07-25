using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class PassThroughVDETMessageViewModel
    {
        public string Status { get; set; }
        public string Code { get; set; }
        public string RedirectUrl { get; set; }
        public string Message { get; set; }    
    }
}