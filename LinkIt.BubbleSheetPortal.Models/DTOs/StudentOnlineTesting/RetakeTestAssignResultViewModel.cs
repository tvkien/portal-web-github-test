using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.StudentOnlineTesting
{
    public class RetakeTestAssignResultViewModel
    {
        public int ID { get; set; }
        public string ShortGUID { get; set; }
        public DateTime Assigned { get; set; }       
        public string Test { get; set; }        
        public string ClassName { get; set; }
        public string StudentName { get; set; }
        public string StudentCode { get; set; }
        public string TestCode { get; set; }
        public string StudentFirstName { get; set; }
        public string StudentLastName { get; set; }
        public int StudentId { get; set; }
        public string TeacherName { get; set; }
        public string Tutorial { get; set; }
        public string PortalHyperLinkTestCode { get; set; }
        public bool IsActive { get; set; } = false;
        public string AuthenticationCode { get; set; }
    }
}
