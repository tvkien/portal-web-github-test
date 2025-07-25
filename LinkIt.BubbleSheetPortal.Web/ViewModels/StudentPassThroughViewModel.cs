using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class StudentPassThroughViewModel
    {
        public string StudentID { get; set; }
        public string RedirectUrl { get; set; }
        public string Timestamp { get; set; }

        [ScriptIgnore]
        public int UserID { get; set; }
    }
}