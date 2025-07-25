using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class RemoveTestResultsViewModel
    {
        public int ID { get; set; }
        public string TestNameCustom { get; set; }
        public DateTime ResultDate { get; set; }
        public string SchoolName { get; set; }
        public string TeacherCustom { get; set; }
        public string ClassNameCustom { get; set; }
        public string StudentCustom { get; set; }
        public string IsExported { get; set; }
    }
}