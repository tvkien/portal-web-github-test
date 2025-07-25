using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class RemoveTestResultsV2ViewModel
    {
        public int ID { get; set; }
        public string VirtualTestName { get; set; }
        public string StudentName { get; set; }
        public string ClassTermName { get; set; }
        public DateTime ResultDate { get; set; }
        public string CategoryName { get; set; }
        public string GradeName { get; set; }
        public string SubjectName { get; set; }
        public string IsExported { get; set; }
    }
}
