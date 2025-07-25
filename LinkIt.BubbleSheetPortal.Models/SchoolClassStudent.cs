using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class SchoolClassStudent
    {
        public int ClassId { get; set; }
        public int StudentId { get; set; }
        public int SchoolId { get; set; }
        public int UserSchoolAdminId { get; set; }
    }
}
