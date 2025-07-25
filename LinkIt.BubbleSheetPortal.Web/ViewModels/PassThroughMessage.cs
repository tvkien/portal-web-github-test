using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class PassThroughMessage
    {
        public string MessageError { get; set; }
        public bool ReturnLoginPage { get; set; }
        public string RedirectUrl { get; set; }

        public string MessageType { get; set; }
    }
}