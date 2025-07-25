using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Models.Old.DataLockerForStudent
{
    public class GetDatalockerForStudentPaginationRequest : PaggingInfo
    {
        public int UserId { get; set; }
        public string SearchString { get; set; }
        public string VirtualTestName { get; set; }
        public string TeacherName { get; set; }
        public string ClassName { get; set; }
        public DateTime CurrentDateDistrict { get; set; }
    }
}
