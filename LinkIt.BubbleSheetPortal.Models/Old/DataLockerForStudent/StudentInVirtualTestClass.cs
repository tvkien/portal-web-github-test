using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Models.Old.DataLockerForStudent
{
    public class StudentInVirtualTestClass
    {
        public int StudentID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Code { get; set; }
        public int? TestResultID { get; set; }
        public int? VirtualTestID { get; set; }
        public DateTime? ResultDate { get; set; }
        public int? ClassID { get; set; }
    }
}
