using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.SGO
{
    public class FinalSignOffViewModel
    {
        public int SGOId { get; set; }
        public string AdminComments { get; set; }
        public string TeacherComments { get; set; }
        public bool IsApprover { get; set; }
        public int PermissionAccess { get; set; }
        public int ApproverUserID { get; set; }

        public int SGOStatusId { get; set; }

        public string comments { get; set; }

        public string FinalSignoffDirection { get; set; }
    }
}