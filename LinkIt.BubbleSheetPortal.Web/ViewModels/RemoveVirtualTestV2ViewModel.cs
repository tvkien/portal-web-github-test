using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class RemoveVirtualTestV2ViewModel
    {
        public int VirtualTestID { get; set; }
        public string VirtualTestName { get; set; }
        public string CategoryName { get; set; }
        public string GradeName { get; set; }
        public string SubjectName { get; set; }
        public int ResultCount { get; set; }
        public string StudentNameList { get; set; }
        public string TestResultIDList { get; set; }
        public string StudentIDList { get; set; }
    }
}
