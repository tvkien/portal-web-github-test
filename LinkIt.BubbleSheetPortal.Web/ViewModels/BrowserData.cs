using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class BrowserData
    {
        public List<BrowserSupport> BrowserSupports { get; set; }
        public string MessageAlert { get; set; }

        public BrowserData()
        {
            MessageAlert = string.Empty;
            BrowserSupports = new List<BrowserSupport>();
        }
    }
}