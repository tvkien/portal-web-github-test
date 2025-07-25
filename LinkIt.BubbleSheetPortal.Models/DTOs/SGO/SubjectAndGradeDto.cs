using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.SGO
{
    public class SubjectAndGradeDto
    {
        public int SubjectId { get; set; }

        public string SubjectName { get; set; }

        public int GradeId { get; set; }

        public string GradeName { get; set; }
    }
}
