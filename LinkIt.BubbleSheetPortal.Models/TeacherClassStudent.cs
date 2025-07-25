using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class TeacherClassStudent
    {
        public int ClassId { get; set; }
        public int UserId { get; set; }
        public int StudentId { get; set; }
        public int? ClassUserLOEId { get; set; }
    }
}
