using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class SubjectGrade
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }

        public int GradeId { get; set; }
        public string GradeName { get; set; }

        public int StateId { get; set; }
    }
}
