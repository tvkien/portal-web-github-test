using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class BrowserSupport
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public BrowserSupport()
        {
            Name = string.Empty;
            Value = string.Empty;
        }
    }
}