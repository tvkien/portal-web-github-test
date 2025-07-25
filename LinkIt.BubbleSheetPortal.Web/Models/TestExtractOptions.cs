using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.Models
{
    public class TestExtractOptions
    {
        public bool Gradebook { get; set; }
        public bool ShowRawOrPercentOption { get; set; }
        public bool StudentRecord { get; set; }
        public bool IsUseTestExtract { get; set; }
        public bool GradebookChecked { get; set; }
        public bool StudentRecordChecked { get; set; }
        public bool CleverApi { get; set; }
    }
}
