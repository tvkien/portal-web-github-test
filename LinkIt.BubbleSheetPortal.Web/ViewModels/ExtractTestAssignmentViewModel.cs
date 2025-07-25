using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class ExtractTestAssignmentViewModel
    {
        public int QTITestClassAssignmentId { get; set; }

        public DateTime? Assigned { get; set; }
        public string TestName { get; set; }

        public string Teacher { get; set; }
        public string Class { get; set; }
        public string Code { get; set; }
    }
}
