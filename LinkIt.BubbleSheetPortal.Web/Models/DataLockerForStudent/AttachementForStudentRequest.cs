using LinkIt.BubbleSheetPortal.Web.Models.DataTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.Models.DataLockerForStudent
{
    public class AttachementForStudentRequest : GenericDataTableRequest
    {
        public string SearchString { get; set; }
        public string VirtualTestName { get; set; }
        public string TeacherName { get; set; }
        public string ClassName { get; set; }
    }
}
