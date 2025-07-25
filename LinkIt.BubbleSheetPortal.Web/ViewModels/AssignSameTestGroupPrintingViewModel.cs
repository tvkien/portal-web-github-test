using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class AssignSameTestGroupPrintingViewModel
    {
        public DateTime AssignmentDate { get; set; }
        public string FullName { get; set; }
        public DateTime? ResultDate { get; set; }
        public int StudentId { get; set; }
    }
}
